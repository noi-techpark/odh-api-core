module RawQueryParser.Tests

open Expecto
open Parser

[<AutoOpen>]
module TestHelpers =
    open FParsec

    let run p input =
        match run p input with
        | Success (x, _, _) -> Core.Ok x
        | Failure (msg, _, _) -> Core.Error msg

[<Tests>]
let testParser =
    testList "Parser" [
        testList "Property" [
            test "Simple property should parse correctly" {
                let expected = Ok (Property [ "Detail" ])
                let actual = run property "Detail"
                Expect.equal actual expected ""
            }
            test "Hierarchial property should parse correctly" {
                let expected = Ok (Property [ "Detail"; "de"; "Title" ])
                let actual = run property "Detail.de.Title"
                Expect.equal actual expected ""
            }
        ]
        testList "Sorting" [
            testList "Sort order" [
                test "Order ascending" {
                    let actual = run Sorting.orderBy ""
                    Expect.equal actual (Ok Sorting.Ascending) ""
                }
                test "Order descending" {
                    let actual = run Sorting.orderBy "-"
                    Expect.equal actual (Ok Sorting.Descending) ""
                }
            ]
            testList "Sort order and property" [
                test "Sort by single" {
                    let expected: Result<Sorting.SortStatements, _> = Ok [
                        { Property = Property ["Detail"; "de"; "Title"]; Direction = Sorting.Descending }
                    ]
                    let actual = run Sorting.sortStatements "-Detail.de.Title"
                    Expect.equal actual expected ""
                }
                test "Sort by multiple" {
                    let expected: Result<Sorting.SortStatements, _> = Ok [
                        { Property = Property ["Detail"; "de"; "Title"]; Direction = Sorting.Descending }
                        { Property = Property ["Detail"; "de"; "Body"]; Direction = Sorting.Ascending }
                    ]
                    let actual = run Sorting.sortStatements "-Detail.de.Title,Detail.de.Body"
                    Expect.equal actual expected ""
                }
            ]
        ]
        testList "Filtering" [
            test "Simple condition" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Filtering.Condition { Property = Property ["Geo"; "Altitude"]
                                              Operator = Filtering.Operator.Eq
                                              Value = Filtering.Number 200. })
                let actual = run Filtering.statement "eq(Geo.Altitude, 200)"
                Expect.equal actual expected ""
            }
            test "And condition" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (
                        Filtering.And (
                            Filtering.Condition { Property = Property ["Geo"; "Altitude"]
                                                  Operator = Filtering.Operator.Ge
                                                  Value = Filtering.Number 200. },
                            Filtering.Condition { Property = Property ["Geo"; "Altitude"]
                                                  Operator = Filtering.Operator.Le
                                                  Value = Filtering.Number 400. }
                        )
                    )
                let actual = run Filtering.statement "and(ge(Geo.Altitude, 200), le(Geo.Altitude, 400))"
                Expect.equal actual expected ""
            }
            test "And with multiple conditions" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (
                        Filtering.And (
                            Filtering.Condition { Property = Property ["Active"]
                                                  Operator = Filtering.Operator.Eq
                                                  Value = Filtering.Boolean true },
                            Filtering.And (
                                Filtering.Condition { Property = Property ["Geo"; "Altitude"]
                                                      Operator = Filtering.Operator.Ge
                                                      Value = Filtering.Number 200. },
                                Filtering.Condition { Property = Property ["Geo"; "Altitude"]
                                                      Operator = Filtering.Operator.Le
                                                      Value = Filtering.Number 400. }
                            )
                        )
                    )
                let actual = run Filtering.statement "and(eq(Active, true), ge(Geo.Altitude, 200), le(Geo.Altitude, 400))"
                Expect.equal actual expected ""
            }
            test "And with nested or" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (
                        Filtering.Or (
                            Filtering.Condition { Property = Property ["Active"]
                                                  Operator = Filtering.Operator.Eq
                                                  Value = Filtering.Boolean true },
                            Filtering.And (
                                Filtering.Condition { Property = Property ["Geo"; "Altitude"]
                                                      Operator = Filtering.Operator.Ge
                                                      Value = Filtering.Number 200. },
                                Filtering.Condition { Property = Property ["Geo"; "Altitude"]
                                                      Operator = Filtering.Operator.Le
                                                      Value = Filtering.Number 400. }
                            )
                        )
                    )
                let actual = run Filtering.statement "or(eq(Active, true), and(ge(Geo.Altitude, 200), le(Geo.Altitude, 400)))"
                Expect.equal actual expected ""
            }
        ]
    ]

