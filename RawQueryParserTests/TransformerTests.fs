module RawQueryParser.TransformerTests

open Expecto
open Parser

[<Tests>]
let transfomerTests =
    testList "Transformer" [
        testList "Sorting" [
            test "Simple sort statement" {
                let expected = "data#>>'\{Detail,de,Title\}' ASC"
                let actual = Transformer.transformSort "Detail.de.Title"
                Expect.equal actual expected ""
            }
            test "Simple sort statement descending" {
                let expected = "data#>>'\{Detail,de,Title\}' DESC"
                let actual = Transformer.transformSort "-Detail.de.Title"
                Expect.equal actual expected ""
            }
            test "Combined sort statements" {
                let expected = "data#>>'\{Detail,de,Title\}' ASC, data#>>'\{Detail,de,Body\}' DESC"
                let actual = Transformer.transformSort "Detail.de.Title, -Detail.de.Body"
                Expect.equal actual expected ""
            }
        ]
    ]
