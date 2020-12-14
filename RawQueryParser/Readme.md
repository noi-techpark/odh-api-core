# Raw Query Parser Project

This project enables the tourism open data hub to support custom filtering and sorting.

## What does 'custom' mean?

With custom filtering and sorting a consumer of the API can filter and sort for things
that aren't supported otherwise.

> The tourism API has some predefined filters and sorting suited
> especially for the domain and the
> different endpoints of the tourism sector, like points of interest, activities, and
> so on...  
> These filters are documented in the OpenAPI specification.

The custom filtering and sorting is enabled via query strings.

- `rawfilter` allows the filtering of the data returned by the API endpoint
- `rawsort` allows the sorting of the data returned by the API endpoint

## `rawfilter`

Usage: `?rawfilter=<filter(s)>`

```javascript
eq(Active, true)                                       // all active entries
eq(Type, 'Wandern')                                    // all entries of type 'Wandern'
isnotnull(Detail.ru.Title)                             // all entries with a russian title set
and(ge(Geo.0.Altitude, 200), le(Geo.0.Altitude, 400))  // all entries with an altitude between 200 and 400 meters
```

Keywords/functions: `eq`, `ne`, `gt`, `ge`, `lt`, `le`, `and`, `or`, `isnull`, `isnotnull`

Precedence is archieved by nesting `and` and `or`.  
You can chain multiple conditions inside `and` and `or`:

```
and(foo, bar, baz)
```

Negation isn't supported altough it might be implemented later if needed. I need to think of a case where negation can't be replaced by the keywords from above. 

No `between` or other special functions.

Testing a field if it is NULL or not NULL has special meaning in SQL. You cannot simply query a field for NULL with equality or inequality. You have to use special syntax for that, e.g. `FIELD IS NULL` or `FIELD IS NOT NULL`.    
Because of this special meaning of NULL there exist `isnull` and `isnotnull` functions.

### Supported data types:

- Boolean: `true`, `false`
- Number: `1`, `1.12` (exponential notation is not allowed) are always interpreted as a floating point number
- No magic treatment of strings, they need to be defined in quotes (single or double quotes are both legal, unicode escapes are not allowed) 
## `rawsort`

The `rawsort` syntax follows the proposal for sorting on the destination data: https://gitlab.com/alpinebits/destinationdata/standard-specification/-/issues/4

Usage: `?rawsort=<sorting(s)>`

```javascript
Detail.de.Title
Geo.0.Altitude
-Geo.0.Altitude, Detail.de.Title
```

## Is it safe?

As safe as you can expect a feature like that to be!

Keep in mind that at the end you get a string that gets inserted as a raw string into the SQL statement. So every SQL injection that you are able to get into the query strings are sent to the database engine.

## Why F#?

This project is written in F# as it allows for a very expressive and concise code and the usage of a parser combinator as an integrated DSL.

If you are interested in the why's of F# you can view the following [video](https://www.youtube.com/watch?v=_q_7-fEEH10).  
If that video has provoked your curiosity you can get in touch with me! ;-)

## What is a parser combinator?

A parser combinator library is a collection of larger parsers built from smaller parsers via composition.

For more information about parser combinators you can read through the [FParsec documentation](https://www.quanttec.com/fparsec/). FParsec is the library that is used in this project.    
If you want to get more general information on how parser combinators work and how
the composition of those parsers works i recommend to view this [video](https://www.youtube.com/watch?v=RDalzi7mhdY).
