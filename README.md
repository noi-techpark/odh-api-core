# odh-api-core

Port of ODH Tourim Api to .Net Core.

## Project Goals/Requirements:

* Using .Net Core 5
* PostgreSQL Database
* Docker Support
* Swagger Support
* Identity Server Integration (Keycloak)
* Full Async Api
* better Api Error Handling
* Advanced api operation (fields chooser, language selector, search terms)

## Database Info

Used Postgres 12 
Extensions active on DB

* extension earthdistance;
* extension cube;
* extension pg_trgm;

Test Server on https://api.tourism.testingmachine.eu

## Getting started:

Clone the repository

### Environment Variables

* MSS_USER;
* MSS_PSWD;
* LCS_USER;
* LCS_PSWD;
* LCS_MSGPSWD;
* SIAG_USER;
* SIAG_PSWD;
* XMLDIR;

### using Docker

`docker-compose up` starts the appliaction on http://localhost:60209/

### using .Net Core CLI

Install .Net Core SDK 5\
go into \odh-api-core\ folder \
`dotnet run`
starts the application on 
https://localhost:5001;
http://localhost:5000

