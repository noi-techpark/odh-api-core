# odh-api-core

Port of ODH Tourim Api to .Net Core.

[![CI/CD API](https://github.com/noi-techpark/odh-api-core/actions/workflows/main_api.yml/badge.svg)](https://github.com/noi-techpark/odh-api-core/actions/workflows/main_api.yml)
[![CI/CD Importer](https://github.com/noi-techpark/odh-api-core/actions/workflows/main_importer.yml/badge.svg)](https://github.com/noi-techpark/odh-api-core/actions/workflows/main_importer.yml)

## Project Goals/Requirements:

* .Net Core 5
* PostgreSQL Database
* Docker Support
* Swagger Support
* Identity Server Integration (Keycloak)
* Full Async Api
* improved Api Error Handling
* Browseable Api
* Swagger / Openapi Support
* Advanced api operations 1/2 (fields chooser, language selector, search terms)
* Advanced api operation 2/2 (raw sorting, raw filtering)
* Compatibility tourism api

## Project Structure

### DataModel

Definition of all ODH Tourism Models

### OdhApiCore

Api to retrieve ODH Tourism Data

### OdhApiImporter

Workerservice which is importing the Data with help of the Data Collectors

### Helper

Class Library with Extension Methods and other ODH Helper Methods

### CDB, DSS, EBMS, LCS, MSS, NINJA, RAVEN, SIAG, STA, SuedtirolWein

Data Collectors used by Api and Importer, usually containing classes that retrieve Data, and classes that parse the data to ODH Objects (defined in DataModel)

## Database

Postgres 12 
Extensions active on DB

* extension earthdistance;
* extension cube;
* extension pg_trgm;

Custom Functions on DB

* json_array_to_pg_array
* extract_keys_from_jsonb_object_array
* text2ts
* json_array_to_pg_array
* extract_keys_from_jsonb_object_array
* extract_tags
* extract_tagkeys
* is_valid_jsonb

These custom functions are used for the generated Columns

Test Server on https://api.tourism.testingmachine.eu  
Production Server on https://tourism.api.opendatahub.bz.it

## Getting started:

Clone the repository  
Copy `.env.example` to `.env`  
Set the needed environment variables

### Environment Variables

* PG_CONNECTION (Connection to Postgres Database)
* MSS_USER; (Optional User to retrieve availability request from HGV Mss)
* MSS_PSWD; (Optional Pswd to retrieve availability request from HGV Mss)
* LCS_USER; (Optional User to retrieve availability request from Lts)
* LCS_PSWD; (Optional Pswd to retrieve availability request from Lts)
* LCS_MSGPSWD; (Optional Messagepswd to retrieve availability requests from LTS)
* SIAG_USER; (Optional User to retrieve data from Siag)
* SIAG_PSWD; (Optional Pswd to retrieve data from Siag)
* XMLDIR; (Directory where xml config file is stored)
* JSONPATH; (Directory where json files are stored)
* S3_BUCKET_ACCESSPOINT; (S3 Bucket for Image Upload accesspoint)
* S3_IMAGEUPLOADER_ACCESSKEY; (S3 Bucket for Image Upload accesskey)
* S3_IMAGEUPLOADER_SECRETKEY; (S3 Bucket for Image Upload secretkey)
* OAUTH_AUTORITY; (Oauth Server Authority URL)
* ELK_URL; (Serilog Elasticsearch Sink Elastic URL)
* ELK_TOKEN; (Serilog Elasticsearch Access Token)
* EBMS_USER; (Optional User to access EBMS interface)
* EBMS_PASS; (Optional Pswd to access EBMS interface)
* DSS_USER; (Optional User to access DSS interface)
* DSS_PSWD; (Optional Pswd to access DSS interface)
* DSS_SERVICEURL; (Optional DSS interface serviceurl)
* RAVEN_USER; (Optional User to access Raven interface)
* RAVEN_PSWD; (Optional Pswd to access Raven interface)
* RAVEN_SERVICEURL; (Optional Raven interface serviceurl)

### using Docker

go into \OdhApiCore\ folder \
`docker-compose up` starts the odh-api appliaction on http://localhost:60209/

go into \OdhApiImporter\ folder \
`docker-compose up` starts the odh-api appliaction on http://localhost:60210/

### using .Net Core CLI

Install .Net Core SDK 5\
go into \OdhApiCore\ folder \
`dotnet run`
starts the application on 
https://localhost:5001;
http://localhost:5000

### Postgres

Activate extensions

```
CREATE EXTENSION cube;
```
```
CREATE EXTENSION earthdistance;
```
```
CREATE EXTENSION pg_trgm;
```

Custom functions for Postgres Generated Columns creation

* text2ts

```sql
CREATE OR REPLACE FUNCTION text2ts(text)
 RETURNS timestamp without time zone
 LANGUAGE sql
 IMMUTABLE
AS $function$SELECT CASE WHEN $1 ~'^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?(?:Z|\+\d{2}:\d{2})?$' THEN CAST($1 AS timestamp without time zone) END; $function$;
```
* json_array_to_pg_array

```sql
CREATE OR REPLACE FUNCTION json_array_to_pg_array(jsonarray jsonb)
 RETURNS text[]
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$ begin if jsonarray <> 'null' then return (select array(select jsonb_array_elements_text(jsonarray))); else return null; end if; end; $function$;
```

* extract_keys_from_jsonb_object_array

```sql
CREATE OR REPLACE FUNCTION extract_keys_from_jsonb_object_array(jsonarray jsonb, key text DEFAULT 'Id'::text)
 RETURNS text[]
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$ begin if jsonarray <> 'null' then return (select array(select data2::jsonb->> key from (select jsonb_array_elements_text(jsonarray) as data2) as subsel)); else return null; end if; end; $function$;
```

* extract_tags

```sql
CREATE OR REPLACE FUNCTION public.extract_tags(jsonarray jsonb)
 RETURNS text[]
 LANGUAGE plpgsql
 IMMUTABLE strict
AS $function$ begin
	return
		(select array(select concat(x.tags->>'Source', '.', x.tags->>'Id') from
		(select jsonb_path_query(jsonarray, '$.*[*]') tags) x) x);
end; $function$
```

* extract_tagkeys

```sql
CREATE OR REPLACE FUNCTION public.extract_tagkeys(jsonarray jsonb)
 RETURNS text[]
 LANGUAGE plpgsql
 IMMUTABLE strict
AS $function$ begin
	return (array(select distinct unnest(json_array_to_pg_array(jsonb_path_query_array(jsonarray, '$.*[*].Id')))));
end; $function$
```

* is_valid_jsonb

```sql
CREATE OR REPLACE FUNCTION is_valid_jsonb(p_json text) 
RETURNS JSONB
AS $$
begin
  return p_json::jsonb;
exception 
  when others then
     return null;  
end; $$ 
LANGUAGE plpgsql IMMUTABLE;
```

