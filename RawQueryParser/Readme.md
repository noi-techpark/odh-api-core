<!--
SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>

SPDX-License-Identifier: CC0-1.0
-->

﻿# Raw Query Parser Project

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

### `rawfilter`

Usage: `?rawfilter=<filter(s)>`

```javascript
eq(<field>, <value>)
eq(Active, true)                                               // all active entries
eq(Type, 'Wandern')                                            // all entries of type 'Wandern'
eq(ODHTags, [])                                                // all entries where Array ODHTags is empty
isnotnull(Detail.ru.Title)                                     // all entries with a russian title set
and(ge(GpsInfo.0.Altitude, 200), le(GpsInfo.0.Altitude, 400))  // all entries with an altitude between 200 and 400 meters
in(Features.[*].Id, "a3067617-771a-4b84-b85e-206e5cf4402b")    // all entries in an Array with a specific feature ID
in(Features.[].Id, "a3067617-771a-4b84-b85e-206e5cf4402b")     // all entries in an Array with a specific feature ID - alternative notation
```

**Attention**: On the field _ImageGallery _if data is retrieved as anonymous there is a Transformer applied after getting the Result from the DB which cuts out all not CC0 license Images. So there the rawfilter possible can produce false results.  


> `<field>` is described [here](#Supported-value-types:)   
> `<value>` is described [here](#Field-syntax:)

Keywords/functions: `eq`, `ne`, `gt`, `ge`, `lt`, `le`, `and`, `or`, `isnull`, `isnotnull`, `in`, `nin`

Precedence is archieved by nesting `and` and `or`.  
You can chain multiple conditions inside `and` and `or`:

```
and(foo, bar, baz)
```

Negation isn't supported altough it might be implemented later if needed. I need to think of a case where negation can't be replaced by the keywords from above. 

No `between`, `startswith` and other special functions. Altough this may change dependeing of the actual use cases.

> To legitimate the addition of such special functions a production use case has to be satisfied.
> Prefer composition on the value type level over special operators.

Testing a field if it is NULL or not NULL has special meaning in SQL. You cannot simply query a field for NULL with equality or inequality. You have to use special syntax for that, e.g. `FIELD IS NULL` or `FIELD IS NOT NULL`.    
Because of this special meaning of NULL there exist `isnull` and `isnotnull` functions.

### `rawsort`

The `rawsort` syntax follows the proposal for sorting on the destination data: https://gitlab.com/alpinebits/destinationdata/standard-specification/-/issues/4

Usage: `?rawsort=<sorting(s)>`

```javascript
[-]<field>
Detail.de.Title
Geo.0.Altitude
-Geo.0.Altitude, Detail.de.Title
```
> `<field>` is described [here](#Supported-value-types:)

Prepending a minus does significate that the field gets sorted descending.

## Field syntax

*The `<field>` placeholder in the examples above.*    
A field  can be a simple flat selection like `Active` or `Type`, which matches a document with the following JSON structure:

```json
{ "Active": <value>, "Type": <value>, ... }
```

But it is possible to traverse throgh the document's hierarchy by separating the names by a dot (.).

`Detail.ru.Title` matches on a document with the following JSON structure:

```json
{ "Detail": { "ru": { "Title": <value>, ... }, ... }, ... }
```

It is also possible to query arrays:   
`Features.[0].Id`: returns the first Element of the Array as single field  
`Features.[n].Id`: returns the n-th Element of the Array as single field  
`Features.[*].Id`: returns all Elements of the Array (as array)  
  
This field matches on a document with the following JSON structure:  
  
```json
{ "Features": [{ "Id": <value>, ... }, { "Id": <value>, ... }, ...], ... }
```

## Supported value types:

*The `<value>` placeholder in the examples above.*

- Boolean: `true`, `false`
- Number: `1`, `1.12` (exponential notation is not allowed) are always interpreted as a floating point number
- Strings need to be defined in quotes (single or double quotes are both legal, unicode escapes are not allowed) 
  They are special in that they leverage PostgreSQL special capability to represent different data types (e.g. dates) as strings (`#>>`) which allows to filter them by strings.
- The literal `[]` which denotes an empty JSON array.

No special or magical conversion happens between the data types.    
E.g. `1` applied to a boolean field doesn't get converterted into a boolean type 'automagically'. It is the underlying DB's responsibility to handle such a type missmatch.

The following error will be thrown by PostreSQL if you try to compare a boolean field with a numeric value (`eq(Active,1)`):

```
22023: cannot cast jsonb boolean to type double precision
```

## Is it safe?

As safe as you can expect a feature like that to be!

Keep in mind that at the end you get a string that gets inserted as a raw string into the SQL statement. So every SQL injection that you are able to get into the query strings are sent to the database engine.

## Is it performant?

The query parsing is performant, but the execution time on the DB is nothing that can be easily predicted.    
The execution performance totally depends on the JSON capabilities of PostgreSQL, the query planner and the indizes that are defined on the fields.

> There exist special filters on the endpoints that are optimized for specific production use cases.

## Why F#?

This project is written in F# as it allows for a very expressive and concise code and the usage of a parser combinator as an integrated DSL.

If you are interested in the why's of F# you can view the following [video](https://www.youtube.com/watch?v=_q_7-fEEH10).  
If that video has provoked your curiosity you can get in touch with me! ;-)

## What is a parser combinator?

A parser combinator library is a collection of larger parsers built from smaller parsers via composition.

For more information about parser combinators you can read through the [FParsec documentation](https://www.quanttec.com/fparsec/). FParsec is the library that is used in this project.    
If you want to get more general information on how parser combinators work and how
the composition of those parsers works i recommend to view this [video](https://www.youtube.com/watch?v=RDalzi7mhdY).
