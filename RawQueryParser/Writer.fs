/// Generates PostgreSQL commands
module RawQueryParser.Writer

/// <summary>
/// A sort direction gets written
/// as <c>ASC</c> or <c>DESC</c>.
/// </summary>
let writeDirection = function
    | Ascending -> "ASC"
    | Descending -> "DESC"

/// <summary>
/// Write a property like
/// <c>Property ["Detail"; "de"; "Title"]</c>
/// as
/// <c>data#>>'{Detail, de, Title}'</c>
/// </summary>
let writeProperty (Property fields) =
    fields
    |> String.concat ","
    |> sprintf "data#>>'\\{%s\\}'"

/// A statement gets written by concatenating
/// a property with a sort direction.
let writeSortStatement statement =
    let s = writeDirection statement.Direction
    let p = writeProperty statement.Property
    $"{p} {s}"

/// Statements are concatenated <see cref=">writeStatement</c>
let writeSortStatements (statements: SortStatements) =
    statements
    |> List.map writeSortStatement
    |> String.concat ", "

