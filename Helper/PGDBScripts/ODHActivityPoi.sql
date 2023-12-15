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
