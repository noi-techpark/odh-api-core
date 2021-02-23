# odh-api-core

Port of ODH Tourim Api to .Net Core.

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

## Database Info

Postgres 12 
Extensions active on DB

* extension earthdistance;
* extension cube;
* extension pg_trgm;

Test Server on https://api.tourism.testingmachine.eu
Production Server on https://tourism.api.opendatahub.bz.it

## Getting started:

Clone the repository

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
* S3_BUCKET_ACCESSPOINT (S3 Bucket for Image Upload accesspoint)
* S3_IMAGEUPLOADER_ACCESSKEY (S3 Bucket for Image Upload accesskey)
* S3_IMAGEUPLOADER_SECRETKEY (S3 Bucket for Image Upload secretkey)
* OAUTH_AUTORITY (Oauth Server Authority URL)
* ELK_URL (Serilog Elasticsearch Sink Elastic URL)
* ELK_TOKEN (Serilog Elasticsearch Access Token)

### using Docker

`docker-compose up` starts the appliaction on http://localhost:60209/

### using .Net Core CLI

Install .Net Core SDK 5\
go into \odh-api-core\ folder \
`dotnet run`
starts the application on 
https://localhost:5001;
http://localhost:5000

