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

    let writeCondition condition =
        let field =
            match condition.Value with
            | Boolean _ -> $"({writeRawField condition.Field})::boolean"
            | Number _ -> $"({writeRawField condition.Field})::float"
            | String _ -> writeTextField condition.Field
        let operator = writeOperator condition.Operator
        let value = writeValue condition.Value
        $"{field} {operator} {value}"

    let rec writeStatement = function 
        | Condition value -> writeCondition value
        | And (left, right) -> $"(%s{writeStatement left} AND %s{writeStatement right})"
        | Or (left, right) -> $"(%s{writeStatement left} OR %s{writeStatement right})"
        | In (field, values) ->
            let values =
                values
                |> List.map writeValue
                |> String.concat ","
            $"{writeRawField field} && ARRAY({values})"
        | NotIn (field, values) ->
            failwith "Not implemented for PostgreSQL"
        | IsNull property -> $"{writeRawField property} IS NULL"
        | IsNotNull property -> $"{writeRawField property} IS NOT NULL"

