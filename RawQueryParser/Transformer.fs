module RawQueryParser.Transformer

open FParsec

/// <summary>
/// Transform a sort expression into a
/// PostgreSQL <c>ORDER BY</c> expression.
/// </summary>
[<CompiledName "TransformSort">]
let transformSort input =
    if isNull input then
        nullArg (nameof input)
    else
        match run Parser.Sorting.statements input with
        | Success (statements, _, _) ->
            Writer.Sorting.writeStatements statements
        | Failure (msg, _, _) ->
            failwith msg

/// <summary>
/// Transform a filter expression into a
/// PostgreSQL <c>WHERE</c> expression.
/// </summary>
[<CompiledName "TransformFilter">]
let transformFilter input =
    if isNull input then
        nullArg (nameof input)
    else
        match run Parser.Filtering.statement input with
        | Success (statement, _, _) ->
            Writer.Filtering.writeStatement statement
        | Failure (msg, _, _) ->
            failwith msg

