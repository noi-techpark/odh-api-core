[<AutoOpen>]
module Prelude

[<AutoOpen>]
module TestHelpers =
    open FParsec

    let run p input =
        match run p input with
        | Success (x, _, _) -> Core.Ok x
        | Failure (msg, _, _) -> Core.Error msg

