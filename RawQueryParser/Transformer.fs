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
        match run Parser.sortStatements input with
        | Success (statements, _, _) ->
            Writer.writeSortStatements statements
        | Failure (msg, _, _) ->
            raise (ParserException msg)

