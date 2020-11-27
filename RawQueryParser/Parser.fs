module RawQueryParser.Parser

open FParsec

type Parser<'a> = Parser<'a, unit>

let sortOrder: Parser<_> =
    opt (pstring "-")
    |>> function
        | Some _ -> Descending
        | None -> Ascending

let property =
    let options = IdentifierOptions()
    sepBy (identifier options) (pstring ".")
    |>> Property

let statement =
    sortOrder .>>. property
    |>> fun (order, prop) ->
        { Property = prop
          Direction = order }

let statements: Parser<SortStatements> =
    sepBy statement (pstring ",") .>> eof

