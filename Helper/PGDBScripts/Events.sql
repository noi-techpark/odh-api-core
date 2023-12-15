// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

CREATE OR REPLACE FUNCTION public.json_array_to_pg_array(jsonarray jsonb)
 RETURNS text[]
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$ begin if jsonarray <> 'null' then return (select array(select jsonb_array_elements_text(jsonarray))); else return null; end if; end; $function$
;

CREATE OR REPLACE FUNCTION public.json_2_ts_array(jsonarray jsonb)
 RETURNS timestamp[]
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$ begin if jsonarray <> 'null' then return (
  select 
    array(
      select 
        (event ->> 'From')::timestamp + (event ->> 'Begin')::time        
      from 
        jsonb_array_elements(jsonarray) as event 
      where 
        (event ->> 'From')::timestamp + (event ->> 'Begin')::time < (event ->> 'To')::timestamp + (event ->> 'End')::time
    )
);
else return null;
end if;
end;
$function$
;

CREATE OR REPLACE FUNCTION public.json_2_tsrange_array(jsonarray jsonb)
 RETURNS tsrange[]
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$ begin if jsonarray <> 'null' then return (
  select 
    array(
      select 
        tsrange(
          ( (event ->> 'From')::timestamp + (event ->> 'Begin')::time), 
          ( (event ->> 'To')::timestamp + (event ->> 'End')::time)
        ) 
      from 
        jsonb_array_elements(jsonarray) as event 
      where 
        (event ->> 'From')::timestamp + (event ->> 'Begin')::time < (event ->> 'To')::timestamp + (event ->> 'End')::time
    )
);
else return null;
end if;
end;
$function$
;

CREATE OR REPLACE FUNCTION public.convert_tsrange_array_to_tsmultirange(tsrange_array tsrange[])
 RETURNS tsmultirange
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$
DECLARE
    result tsmultirange := tsmultirange();
    tsr tsrange;
BEGIN
	IF tsrange_array IS NOT NULL THEN
    -- Durchlaufen des tsrange-Arrays
    FOREACH tsr IN ARRAY tsrange_array
    LOOP
        -- Hinzufügen des tsrange-Elements zum tsmultirange
        result := result + tsmultirange(tsrange(tsr));
    END LOOP;
	END IF;

    RETURN result;
END;
$function$
;

CREATE OR REPLACE FUNCTION public.get_abs_eventdate_single(ts_array timestamp[],ts_tocalculate timestamp)
 RETURNS bigint
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$
DECLARE
    intarr bigint[];
	result bigint;
    tsr timestamp;    
BEGIN
	IF ts_array IS NOT NULL THEN
    -- Durchlaufen des ts-Arrays
    FOREACH tsr IN ARRAY ts_array
    LOOP
        -- berechne datumsabs    	
        intarr := array_append(intarr, abs(extract(epoch from (tsr - ts_tocalculate)))::bigint);
    END LOOP;

    result = (select unnest(intarr) as x order by x limit 1);
   
	END IF;		

    RETURN result;
END;
$function$
;


ALTER TABLE public.events ADD gen_eventdates tsmultirange NULL GENERATED ALWAYS AS (convert_tsrange_array_to_tsmultirange(json_2_tsrange_array(data #> '{EventDate}'::text[]))) STORED;

ALTER TABLE events ADD IF NOT EXISTS gen_eventdatearray timestamp[] GENERATED ALWAYS AS (json_2_ts_array(data#>'{EventDate}')) stored;

