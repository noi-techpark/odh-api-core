module RawQueryParser.Parser

open FParsec

/// Type alias to simplify the type annotation of a Parser.
type Parser<'a> = Parser<'a, unit>

/// <summary>
/// Parse a field path.
/// <c>Detail.de.Title => Field ["Detail"; "de"; "Title"]</c>
/// </summary>
let field =
    let options = IdentifierOptions()
    sepBy (
        identifier options <|> (pint32 |>> string)
    ) (pchar '.')
    |>> Field
    <?> "field"

module Sorting =
    open Sorting

    /// <summary>
    /// Parse a sort direction.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>If omitted the sort is ascending</item>
    /// <item>If <c>-</c> the sort is descending</item>
    /// </list>
    /// </remarks>
    let orderBy: Parser<_> =
        opt (pstring "-")
        |>> function
            | Some _ -> Descending
            | None -> Ascending
        <?> "order by"

    /// A sortStatement consists of a sort direction and a property.
    let sortStatement =
        orderBy .>>. field
        |>> (fun (order, prop) ->
            { Field = prop
              Direction = order })
        <?> "sort statement, e.g. Detail.de.Title"

    /// sortStatements consist of multiple statements divided by a comma.
    let statements: Parser<SortStatements> =
        sepBy1 sortStatement (pchar ',' >>. spaces) .>> eof

module Filtering =
    open Filtering

    let betweenBrackets p =
        between (pchar '(') (pchar ')') p

    let betweenQuotes p =
        let quotes = pchar '\"' <|> pchar '\''
        between quotes quotes p

    let operator: Parser<Operator> =
        choice [
            pstring "eq" >>% Eq
            pstring "ne" >>% Ne
            pstring "gt" >>% Gt
            pstring "ge" >>% Ge
            pstring "lt" >>% Lt
            pstring "le" >>% Le
        ]

    let boolean: Parser<Value> =
        choice [
            pstring "true" >>% Boolean true
            pstring "false" >>% Boolean false
        ]
        <?> "boolean"

    let number: Parser<Value> =
        pfloat |>> Number
        <?> "number"

    let string: Parser<Value> =
        betweenQuotes (
            manySatisfy (isNoneOf ['\"'; '\''])
        )
        |>> String
        <?> "string"

    let value: Parser<Value> =
        choice [
            boolean
            number
            string
        ]

    let call: Parser<Field * Value> =
        betweenBrackets (
            field .>>. (pchar ',' >>. spaces >>. value)
        )

    let condition: Parser<Condition> =
        operator .>>. call
        |>> (fun (op, (prop, value)) ->
            { Field = prop
              Operator = op
              Value = value })
        <?> "condition, e.g. `eq(field, value)`"

    let statement', statementRef = createParserForwardedToRef<FilterStatement, unit>()

    do statementRef :=
        let innerParser: Parser<FilterStatement list> =
            betweenBrackets (sepBy1 statement' (pchar ',' >>. spaces))
        let combineWith f (statements: FilterStatement list) =
            statements |> List.rev |> List.reduce (fun y x -> f (x, y))
        choice [
            pstring "and" >>. innerParser |>> combineWith And
            pstring "or" >>. innerParser |>> combineWith Or
            pstring "isnull" >>. betweenBrackets field |>> IsNull
            pstring "isnotnull" >>. betweenBrackets field |>> IsNotNull
            condition |>> Condition
        ]

    let statement = statement' .>> eof

