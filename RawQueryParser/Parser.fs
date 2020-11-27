module RawQueryParser.Parser

open FParsec

/// Type alias to simplify the type annotation of a Parser.
type Parser<'a> = Parser<'a, unit>

/// <summary>
/// Parse a sort direction.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>If omitted the sort is ascending</item>
/// <item>If <c>-</c> the sort is descending</item>
/// </list>
/// </remarks>
let sortDirection: Parser<_> =
    opt (pstring "-")
    |>> function
        | Some _ -> Descending
        | None -> Ascending

/// <summary>
/// Parse a property path.
/// <c>Detail.de.Title => Property ["Detail"; "de"; "Title"]</c>
/// </summary>
let property =
    let options = IdentifierOptions()
    sepBy (identifier options) (pstring ".")
    |>> Property

/// A sortStatement consists of a sort direction and a property.
let sortStatement =
    sortDirection .>>. property
    |>> fun (order, prop) ->
        { Property = prop
          Direction = order }

/// sortStatements consist of multiple statements divided by a comma.
let sortStatements: Parser<SortStatements> =
    sepBy sortStatement (pstring ",") .>> eof

