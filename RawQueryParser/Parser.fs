module RawQueryParser.Parser

open FParsec

/// Type alias to simplify the type annotation of a Parser.
type Parser<'a> = Parser<'a, unit>

/// <summary>
/// Parse a property path.
/// <c>Detail.de.Title => Property ["Detail"; "de"; "Title"]</c>
/// </summary>
let property =
    let options = IdentifierOptions()
    sepBy (identifier options) (pstring ".")
    |>> Property
    <?> "property"

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
        orderBy .>>. property
        |>> fun (order, prop) ->
            { Property = prop
              Direction = order }

    /// sortStatements consist of multiple statements divided by a comma.
    let sortStatements: Parser<SortStatements> =
        sepBy sortStatement (pstring ",") .>> eof

module Filtering =
    open Filtering

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
        let bracket = pchar '\"' <|> pchar '\''
        let bla = manySatisfy (fun char -> char <> '\"' && char <> '\'')
        between bracket bracket bla
        |>> String
        <?> "string"

    let value: Parser<Value> =
        choice [
            boolean
            number
            string
        ]

    let inside: Parser<Property * Value> =
        property .>>. (pchar ',' >>. spaces >>. value)
        <?> "field"

    let call: Parser<Property * Value> =
        between (pchar '(') (pchar ')') inside

    let condition: Parser<Condition> =
        operator .>>. call
        |>> (fun (op, (prop, value)) ->
            { Property = prop
              Operator = op
              Value = value })
        <?> "condition, e.g. `eq(field, value)`"

    let statement', statementRef = createParserForwardedToRef<FilterStatement, unit>()

    do statementRef :=
        let innerParser: Parser<FilterStatement list> =
            between (pchar '(')
                    (pchar ')')
                    (sepBy1 statement' (pchar ',' >>. spaces))
        let rec recurse f (statements: FilterStatement list) =
            match statements with
            | [] ->
                // TODO: Find a more idiomatic solution instead of throwing an exception.
                // Not a big problem, because the parser never reaches this state because
                // of `sepBy1` already guarantees a non empty list
                failwith "Empty statements are not allowed"
            | [value] ->
                value
            | head::tail ->
                f (head, recurse f tail)
        choice [
            pstring "and" >>. innerParser |>> recurse And
            pstring "or" >>. innerParser |>> recurse Or
            condition |>> Condition
        ]

    let statement = statement' .>> eof
