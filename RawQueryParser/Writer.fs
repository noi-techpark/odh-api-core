/// Generates PostgreSQL commands
module RawQueryParser.Writer

/// <summary>
/// Write a field like
/// <c>Field ["Detail"; "de"; "Title"]</c>
/// as
/// <c>data#>'{Detail,de,Title}'</c>
/// </summary>
let writeRawField (Field fields) =
    fields
    |> String.concat ","
    |> sprintf "data#>'\\{%s\\}'"

/// <summary>
/// Write a field like
/// <c>Field ["Detail"; "de"; "Title"]</c>
/// as
/// <c>data#>>'{Detail,de,Title}'</c>
/// </summary>
let writeTextField (Field fields) =
    fields
    |> String.concat ","
    |> sprintf "data#>>'\\{%s\\}'"

module Sorting =
    open Sorting

    /// <summary>
    /// A sort direction gets written
    /// as <c>ASC</c> or <c>DESC</c>.
    /// </summary>
    let writeDirection = function
        | Ascending -> "ASC"
        | Descending -> "DESC"

    /// A statement gets written by concatenating
    /// a property with a sort direction.
    let writeSortStatement statement =
        let direction = writeDirection statement.Direction
        let field = writeTextField statement.Field
        $"{field} {direction}"

    /// Statements are concatenated <see cref=">writeStatement</c>
    let writeStatements (statements: SortStatements) =
        statements
        |> List.map writeSortStatement
        |> String.concat ", "

module Filtering =
    open Filtering

    let writeOperator = function
        | Eq -> "="
        | Ne -> "<>"
        | Gt -> ">"
        | Ge -> ">="
        | Lt -> "<"
        | Le -> "<="

    let writeValue = function
        | Boolean true -> "TRUE"
        | Boolean false -> "FALSE"
        | Number value -> $"{value}"
        | String value -> $"'%s{value}'"

    let writeComparison comparison =
        let field =
            match comparison.Value with
            | Boolean _ -> $"({writeRawField comparison.Field})::boolean"
            | Number _ -> $"({writeRawField comparison.Field})::float"
            | String _ -> writeTextField comparison.Field
        let operator = writeOperator comparison.Operator
        let value = writeValue comparison.Value
        $"{field} {operator} {value}"

    let rec writeStatement = function 
        | And (left, right) -> $"(%s{writeStatement left} AND %s{writeStatement right})"
        | Or (left, right) -> $"(%s{writeStatement left} OR %s{writeStatement right})"
        | Condition (Comparison value) -> writeComparison value
        | Condition (In (field, values)) ->
            let values =
                values
                |> List.map writeValue
                |> String.concat ","
            $"({writeRawField field})::jsonb @> to_jsonb(array\[{values}\])"
        | Condition (NotIn (field, values)) ->
            failwith "Not implemented for PostgreSQL"
        | Condition (IsNull property) -> $"{writeRawField property} IS NULL"
        | Condition (IsNotNull property) -> $"{writeRawField property} IS NOT NULL"

