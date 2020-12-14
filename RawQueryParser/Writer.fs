/// Generates PostgreSQL commands
module RawQueryParser.Writer

/// <summary>
/// Write a property like
/// <c>Property ["Detail"; "de"; "Title"]</c>
/// as
/// <c>data#>>'{Detail,de,Title}'</c>
/// </summary>
let writeRawProperty (Property fields) =
    fields
    |> String.concat ","
    |> sprintf "data#>'\\{%s\\}'"

let writeTextProperty (Property fields) =
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
        let s = writeDirection statement.Direction
        let p = writeTextProperty statement.Property
        $"{p} {s}"

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
        let p =
            let p = writeRawProperty condition.Property
            match condition.Value with
            | Boolean _ -> $"({p})::boolean"
            | Number _ -> $"({p})::float"
            | String _ -> $"({p})::text"
        let op = writeOperator condition.Operator
        let v = writeValue condition.Value
        $"{p} {op} {v}"

    let rec writeStatement = function 
        | Condition value -> writeCondition value
        | And (left, right) -> $"(%s{writeStatement left} AND %s{writeStatement right})"
        | Or (left, right) -> $"(%s{writeStatement left} OR %s{writeStatement right})"
        | IsNull property -> $"{writeRawProperty property} IS NULL"
        | IsNotNull property -> $"{writeRawProperty property} IS NOT NULL"

