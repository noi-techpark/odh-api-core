// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

CREATE OR REPLACE FUNCTION public.calculate_access_array(source text, closeddata boolean, reduced boolean)
 RETURNS text[]
 LANGUAGE plpgsql
 IMMUTABLE
AS $function$
	begin 
	if source = null or closeddata = null or reduced = null then return (array['A22','ANONYMOUS','IDM','STA']);
	end if;
	if source = 'lts' and not reduced then return (array['IDM']);
	end if;    
    if source = 'a22' then return (array['A22']);  
	end if;
	if source = 'lts' and reduced and not closeddata then return (array['A22','ANONYMOUS','STA']);
	end if; 
	if source <> 'lts' and source <> 'a22' then return (array['A22','ANONYMOUS','IDM','STA']);
	end if;
	return (array['A22','ANONYMOUS','IDM','STA']);
end;
$function$;

--TODO license is not used....
CREATE OR REPLACE FUNCTION public.calculate_access_array_rawdata(source text, license text)
RETURNS text[]
LANGUAGE plpgsql
IMMUTABLE
AS $function$
begin
-- if data is from source lts and not reduced IDM only access --
if source = 'lts' then return (array['IDM']);
end if;
-- If data is from source a22 only access A22 --
if source = 'a22' then return (array['A22']);
end if;
-- if data is not from source lts and a22 and not closed data give all access --
return (array['A22','ANONYMOUS','IDM','STA']);
end;
$function$


alter table smgpois drop column gen_access_role 

ALTER TABLE smgpois
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table activities drop column gen_access_role

ALTER TABLE activities
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table pois drop column gen_access_role

ALTER TABLE pois
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table accommodations drop column gen_access_role

ALTER TABLE accommodations
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table accommodationrooms drop column gen_access_role

ALTER TABLE accommodationrooms
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table webcams drop column gen_access_role

ALTER TABLE webcams
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table measuringpoints drop column gen_access_role

ALTER TABLE measuringpoints
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table events drop column gen_access_role

ALTER TABLE events
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table gastronomies drop column gen_access_role

ALTER TABLE gastronomies
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table articles drop column gen_access_role

ALTER TABLE articles
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table regions drop column gen_access_role

ALTER TABLE regions
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table tvs drop column gen_access_role

ALTER TABLE tvs
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table municipalities drop column gen_access_role

ALTER TABLE municipalities
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table districts drop column gen_access_role

ALTER TABLE districts
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table areas drop column gen_access_role

ALTER TABLE areas
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table eventeuracnoi drop column gen_access_role

ALTER TABLE eventeuracnoi
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table skiareas drop column gen_access_role

ALTER TABLE skiareas
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table skiregions drop column gen_access_role

ALTER TABLE skiregions
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table metaregions drop column gen_access_role

ALTER TABLE metaregions
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table experienceareas drop column gen_access_role

ALTER TABLE experienceareas
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table publishers drop column gen_access_role

ALTER TABLE publishers
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table venues_v2 drop column gen_access_role

ALTER TABLE venues_v2
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table wines drop column gen_access_role

ALTER TABLE wines
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table weatherdatahistory drop column gen_access_role

ALTER TABLE weatherdatahistory
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table metadata drop column gen_access_role

ALTER TABLE metadata
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table packages drop column gen_access_role

ALTER TABLE packages
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array(data#>>'{_Meta,Source}',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

--no lts restriction here--

alter table smgtags drop column gen_access_role 

ALTER TABLE smgtags
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array('all',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;

alter table tags drop column gen_access_role 

ALTER TABLE tags
ADD IF NOT EXISTS gen_access_role text[]
GENERATED ALWAYS AS (calculate_access_array('all',(data#>'{LicenseInfo,ClosedData}')::bool,(data#>'{_Meta,Reduced}')::bool)) stored;


ALTER TABLE rawdata  
ADD IF NOT EXISTS gen_access_role text[] 
GENERATED ALWAYS AS (calculate_access_array_rawdata(datasource,license)) stored;