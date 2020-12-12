module RawQueryParser.Tests

open Expecto
open Parser

[<AutoOpen>]
module TestHelpers =
    open FParsec

    let run p input =
        match run p input with
        | Success (x, _, _) -> x
        | Failure (msg, _, _) -> failwith $"{msg}"

[<Tests>]
let testParser =
    testList "Parser" [
        testList "Property" [
            test "Simple property should parse correctly" {
                let expected = Property [ "Detail" ]
                let actual = run property "Detail"
                Expect.equal actual expected ""
            }
            test "Hierarchial property should parse correctly" {
                let expected = Property [ "Detail"; "de"; "Title" ]
                let actual = run property "Detail.de.Title"
                Expect.equal actual expected ""
            }
        ]
        testList "Sort order" [
            test "Order ascending" {
                let actual = run orderBy ""
                Expect.equal actual Ascending ""
            }
            test "Order descending" {
                let actual = run orderBy "-"
                Expect.equal actual Descending ""
            }
        ]
        testList "Sort order and property" [
            test "Sort by single" {
                let expected = [
                    { Property = Property ["Detail"; "de"; "Title"]; Direction = Descending }
                ]
                let actual = run sortStatements "-Detail.de.Title"
                Expect.equal actual expected ""
            }
            test "Sort by multiple" {
                let expected = [
                    { Property = Property ["Detail"; "de"; "Title"]; Direction = Descending }
                    { Property = Property ["Detail"; "de"; "Body"]; Direction = Ascending }
                ]
                let actual = run sortStatements "-Detail.de.Title,Detail.de.Body"
                Expect.equal actual expected ""
            }
        ]
    ]

// Erroneous input should also be tested:
//run statements "Detail:de.Title,-Detail.de.Body"
//run statements "Detail.de.Title;-Detail.de.Body"
//run statements "Detail.de.Title,+Detail.de.Body"
//run statements "Detail.de.Title -- something bad"
//run statements "Detail.12de"



