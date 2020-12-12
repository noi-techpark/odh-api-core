module RawQueryParser.Transformer

open FParsec

/// Defines an exception that is thrown when parsing failed.
exception ParserException of string

/// <summary>
/// Transform a sort expression into a
/// PostgreSQL <c>ORDER BY</c> statement.
/// </summary>
[<CompiledName "TransformSort">]
let transformSort input =
    if isNull input then
        nullArg (nameof input)
    else
        match run Parser.Sorting.sortStatements input with
        | Success (statements, _, _) ->
            Writer.Sorting.writeSortStatements statements
        | Failure (msg, _, _) ->
            raise (ParserException msg)

let transformFilter input =
    if isNull input then
        nullArg (nameof input)
    else
        match run Parser.Filtering.statement input with
        | Success (statement, _, _) ->
            Writer.Filtering.writeStatement statement
        | Failure (msg, _, _) ->
            raise (ParserException msg)

