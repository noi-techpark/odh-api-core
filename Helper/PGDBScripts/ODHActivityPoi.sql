// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

CREATE OR REPLACE FUNCTION public.extract_keys_from_jsonb_object_array(jsonarray jsonb, key text DEFAULT 'Id'::text)
 RETURNS text[]
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$ begin if jsonarray <> 'null' then return (select array(select data2::jsonb->> key from (select jsonb_array_elements_text(jsonarray) as data2) as subsel)); else return null; end if; end; $function$
;

CREATE OR REPLACE FUNCTION public.extract_tagkeys(jsonarray jsonb)
 RETURNS text[]
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$ begin return (array(select distinct unnest(json_array_to_pg_array(jsonb_path_query_array(jsonarray, '$[*].Id'))))); end; $function$
;

CREATE OR REPLACE FUNCTION public.extract_tags(jsonarray jsonb)
 RETURNS text[]
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$ begin return (select array(select concat(x.tags->> 'Source', '.', x.tags->> 'Id') from (select jsonb_path_query(jsonarray, '$[*]') tags) x) x); end; $function$
;

ALTER TABLE public.smgpois ADD gen_tags _text NULL GENERATED ALWAYS AS (array_cat(extract_tagkeys(data #> '{Tags}'::text[]), extract_tags(data #> '{Tags}'::text[]))) STORED;

CREATE OR REPLACE FUNCTION public.createshapejson(
id int, 
country text, 
code_rip float, 
code_reg float, 
code_prov float,
code_cm float,
code_uts float, 
istatnumber text, 
abbrev text, 
type_uts text,
type text,
name text, 
name_alternative text, 
shape_leng float, 
shape_area float, 
geom geometry)
 RETURNS jsonb
 LANGUAGE plpgsql
 IMMUTABLE 
AS $function$ begin
	return
		json_build_object(
		'Id',id,
		'Country',country,
		'Code_Rip',code_rip,
		'Code_Reg',code_reg,
		'Code_Prov',code_prov,
		'Code_Cm',code_cm,
		'Code_Uts',code_uts,
		'Istatnumber',istatnumber,
		'Abbrev',abbrev,
		'Type_Uts',type_uts,
		'Name',name,
		'Name_Alternative',name_alternative,
		'Shape_length',shape_leng,
		'Shape_area',shape_area,
		'Source','Istat',
		'Type', type,
		'Geometry',geom);
end; $function$

ALTER TABLE shapes ADD IF NOT EXISTS data jsonb GENERATED ALWAYS AS ((
createshapejson(id, country, code_rip, code_reg, code_prov, code_cm, code_uts, 
istatnumber, abbrev, type_uts, type, name, name_alternative, shape_leng, shape_area, geom)
))stored;