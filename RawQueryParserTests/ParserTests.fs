module RawQueryParser.ParserTests

open Expecto
open Parser

[<Tests>]
let parserTests =
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
                    let actual = run Sorting.statements "-Detail.de.Title"
                    Expect.equal actual expected ""
                }
                test "Sort by multiple" {
                    let expected: Result<Sorting.SortStatements, _> = Ok [
                        { Property = Property ["Detail"; "de"; "Title"]; Direction = Sorting.Descending }
                        { Property = Property ["Detail"; "de"; "Body"]; Direction = Sorting.Ascending }
                    ]
                    let actual = run Sorting.statements "-Detail.de.Title,Detail.de.Body"
                    Expect.equal actual expected ""
                }
            ]
        ]
        testList "Filtering" [
            test "Simple condition with boolean" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Filtering.Condition { Property = Property ["Active"]
                                              Operator = Filtering.Operator.Eq
                                              Value = Filtering.Boolean true })
                let actual = run Filtering.statement "eq(Active, true)"
                Expect.equal actual expected ""
            }
            test "Simple condition with number" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Filtering.Condition { Property = Property ["Geo"; "Altitude"]
                                              Operator = Filtering.Operator.Eq
                                              Value = Filtering.Number 200. })
                let actual = run Filtering.statement "eq(Geo.Altitude, 200)"
                Expect.equal actual expected ""
            }
            test "Simple condition with single quote string" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Filtering.Condition { Property = Property ["Detail"; "de"; "Title"]
                                              Operator = Filtering.Operator.Eq
                                              Value = Filtering.String "Foo" })
                let actual = run Filtering.statement "eq(Detail.de.Title, 'Foo')"
                Expect.equal actual expected ""
            }
            test "Simple condition with double quote string" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Filtering.Condition { Property = Property ["Detail"; "de"; "Title"]
                                              Operator = Filtering.Operator.Eq
                                              Value = Filtering.String "Foo" })
                let actual = run Filtering.statement """eq(Detail.de.Title, "Foo")"""
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
            test "Test for NULL" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Filtering.IsNull (Property ["Detail"; "ru"; "Title"]))
                let actual = run Filtering.statement "isnull(Detail.ru.Title)"
                Expect.equal actual expected ""
            }
        ]
    ]

