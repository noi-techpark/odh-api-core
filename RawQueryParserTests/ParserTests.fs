module RawQueryParser.ParserTests

open Expecto
open Parser

[<Tests>]
let parserTests =
    testList "Parser" [
        testList "Field" [
            test "Simple field should parse correctly" {
                let expected = Ok (Field [ "Detail" ])
                let actual = run field "Detail"
                Expect.equal actual expected ""
            }
            test "Hierarchial field should parse correctly" {
                let expected = Ok (Field [ "Detail"; "de"; "Title" ])
                let actual = run field "Detail.de.Title"
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
            testList "Sort order and field" [
                test "Sort by single" {
                    let expected: Result<Sorting.SortStatements, _> = Ok [
                        { Field = Field ["Detail"; "de"; "Title"]; Direction = Sorting.Descending }
                    ]
                    let actual = run Sorting.statements "-Detail.de.Title"
                    Expect.equal actual expected ""
                }
                test "Sort by multiple" {
                    let expected: Result<Sorting.SortStatements, _> = Ok [
                        { Field = Field ["Detail"; "de"; "Title"]; Direction = Sorting.Descending }
                        { Field = Field ["Detail"; "de"; "Body"]; Direction = Sorting.Ascending }
                    ]
                    let actual = run Sorting.statements "-Detail.de.Title,Detail.de.Body"
                    Expect.equal actual expected ""
                }
            ]
        ]
        testList "Filtering" [
            test "Simple condition with boolean" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Comp { Field = Field ["Active"]
                               Operator = Filtering.Operator.Eq
                               Value = Filtering.Boolean true })
                let actual = run Filtering.statement "eq(Active, true)"
                Expect.equal actual expected ""
            }
            test "Simple condition with number" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Comp { Field = Field ["Geo"; "Altitude"]
                               Operator = Filtering.Operator.Eq
                               Value = Filtering.Number 200. })
                let actual = run Filtering.statement "eq(Geo.Altitude, 200)"
                Expect.equal actual expected ""
            }
            test "Simple condition with single quote string" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Comp { Field = Field ["Detail"; "de"; "Title"]
                               Operator = Filtering.Operator.Eq
                               Value = Filtering.String "Foo" })
                let actual = run Filtering.statement "eq(Detail.de.Title, 'Foo')"
                Expect.equal actual expected ""
            }
            test "Simple condition with double quote string" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Comp { Field = Field ["Detail"; "de"; "Title"]
                               Operator = Filtering.Operator.Eq
                               Value = Filtering.String "Foo" })
                let actual = run Filtering.statement """eq(Detail.de.Title, "Foo")"""
                Expect.equal actual expected ""
            }
            test "AND condition" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (
                        And (
                            Comp { Field = Field ["Geo"; "Altitude"]
                                   Operator = Filtering.Operator.Ge
                                   Value = Filtering.Number 200. },
                            Comp { Field = Field ["Geo"; "Altitude"]
                                   Operator = Filtering.Operator.Le
                                   Value = Filtering.Number 400. }
                        )
                    )
                let actual = run Filtering.statement "and(ge(Geo.Altitude, 200), le(Geo.Altitude, 400))"
                Expect.equal actual expected ""
            }
            test "AND with multiple conditions" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (
                        And (
                            Comp { Field = Field ["Active"]
                                   Operator = Filtering.Operator.Eq
                                   Value = Filtering.Boolean true },
                            And (
                                Comp { Field = Field ["Geo"; "Altitude"]
                                       Operator = Filtering.Operator.Ge
                                       Value = Filtering.Number 200. },
                                Comp { Field = Field ["Geo"; "Altitude"]
                                       Operator = Filtering.Operator.Le
                                       Value = Filtering.Number 400. }
                            )
                        )
                    )
                let actual = run Filtering.statement "and(eq(Active, true), ge(Geo.Altitude, 200), le(Geo.Altitude, 400))"
                Expect.equal actual expected ""
            }
            test "AND with nested OR" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (
                        Or (
                            Comp { Field = Field ["Active"]
                                   Operator = Filtering.Operator.Eq
                                   Value = Filtering.Boolean true },
                            And (
                                Comp { Field = Field ["Geo"; "Altitude"]
                                       Operator = Filtering.Operator.Ge
                                       Value = Filtering.Number 200. },
                                Comp { Field = Field ["Geo"; "Altitude"]
                                       Operator = Filtering.Operator.Le
                                       Value = Filtering.Number 400. }
                            )
                        )
                    )
                let actual = run Filtering.statement "or(eq(Active, true), and(ge(Geo.Altitude, 200), le(Geo.Altitude, 400)))"
                Expect.equal actual expected ""
            }
            test "Condition with NULL check" {
                let expected: Result<Filtering.FilterStatement, _> =
                    Ok (Filtering.IsNull (Field ["Detail"; "ru"; "Title"]))
                let actual = run Filtering.statement "isnull(Detail.ru.Title)"
                Expect.equal actual expected ""
            }
            test "Condition with IN" {
                let expected =
                    Ok (
                        In (Field ["HasLanguage"], [Filtering.String "de"; Filtering.String "it"])
                    )
                let actual = run Filtering.statement "in(HasLanguage,'de','it')"
                Expect.equal actual expected ""
            }
        ]
    ]

