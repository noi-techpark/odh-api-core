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

--calculates the distance from given date
CREATE OR REPLACE FUNCTION public.get_nearest_tsrange_distance(tsrange_array tsrange[], ts_tocalculate timestamp without time zone, sortorder text, prioritizesingledayevents bool)
 RETURNS bigint
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$
DECLARE    
	result bigint;
	intarr bigint[];
	mytsrange tsrange;
    tsr timestamp;
    singledayadd int;
--    tsresult timestamp;
BEGIN
	IF tsrange_array IS NOT NULL THEN
    -- Durchlaufen des ts-Arrays
    FOREACH mytsrange IN array tsrange_array
    loop
	    singledayadd = 0;
	   
	   if(prioritizesingledayevents = true) then
	    
	    -- if prioritiyesingledayevents is used, increase all distances for multidayevents with 1
		     if(lower(mytsrange)::date = upper(mytsrange)::date) then
	         	singledayadd = 0;
	         else
	         	singledayadd = 1;
	         end if;
	        
	   end if;
	    
	    -- get tsrange which is matching (ongoing event) and add 0
	    if mytsrange @> ts_tocalculate then
           intarr := array_append(intarr, singledayadd);
        else
       -- check if period has already ended
           if upper(mytsrange)::timestamp > ts_tocalculate then
               --if period has not ended calculate distance
               intarr := array_append(intarr, extract(epoch from (lower(mytsrange)::timestamp - ts_tocalculate))::bigint + singledayadd);
	    	
           end if;
        
       end if;
       
    END LOOP;

      --get distance (default minimum distance is returned, desc gets the highest distance)
	  if sortorder = 'desc' then
	  	result = (select unnest(intarr) as x order by x desc limit 1);
	  else
	   result = (select unnest(intarr) as x order by x asc limit 1);
	  end if;
   	  
	END IF;		

    RETURN result;
END;
$function$
;

-- GETS the next tsrange to a given date
CREATE OR REPLACE FUNCTION public.get_nearest_tsrange(tsrange_array tsrange[], ts_tocalculate timestamp without time zone)
 RETURNS tsrange
 LANGUAGE plpgsql
 IMMUTABLE STRICT
AS $function$
DECLARE    
	result tsrange;
	distance bigint;
    distancetemp bigint;
	mytsrange tsrange;
    tsr timestamp;    
BEGIN
	IF tsrange_array IS NOT NULL then
	
	distance = 9999999999;
	-- Durchlaufen des ts-Arrays
    FOREACH mytsrange IN array tsrange_array
    loop
	    -- get tsrange which is matching (ongoing event)
	    if mytsrange @> ts_tocalculate then
            result = mytsrange;
            distance = 0;
        else
       -- check if period has already ended
           if upper(mytsrange)::timestamp > ts_tocalculate then
           
               --if period has not ended calculate distance
               distancetemp = extract(epoch from (lower(mytsrange)::timestamp - ts_tocalculate))::bigint;
	    	
              if(distance > distancetemp) then
              	distance = distancetemp;
              	result = mytsrange;
              end if;
              
           end if;
        
       end if;
       
    END LOOP;
 
	END IF;		

    RETURN result;
END;
$function$
;

ALTER TABLE public.events ADD gen_eventdates tsmultirange NULL GENERATED ALWAYS AS (convert_tsrange_array_to_tsmultirange(json_2_tsrange_array(data #> '{EventDate}'::text[]))) STORED;

ALTER TABLE public.events ADD gen_eventdatearray _tsrange NULL GENERATED ALWAYS AS (json_2_tsrange_array(data #> '{EventDate}'::text[])) STORED;

