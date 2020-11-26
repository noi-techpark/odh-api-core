module RawQueryParser.Writer

let writeDirection = function
    | Ascending -> "ASC"
    | Descending -> "DESC"

let writeProperty (Property fields) =
    fields
    |> String.concat ","
    |> sprintf "data#>>'\\{%s\\}'"

let writeStatement statement =
    let s = writeDirection statement.Direction
    let p = writeProperty statement.Property
    $"{p} {s}"

let writeStatements (statements: SortStatements) =
    statements
    |> List.map writeStatement
    |> String.concat ", "

