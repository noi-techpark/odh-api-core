module RawQueryParser.Transformer

open FParsec

exception ParserException of string

[<CompiledName "TransformSort">]
let transformSort input =
    if isNull input then
        nullArg (nameof input)
    else
        match run Parser.statements input with
        | Success (statements, _, _) ->
            statements
            |> Writer.writeStatements
        | Failure (msg, _, _) ->
            raise (ParserException msg)

