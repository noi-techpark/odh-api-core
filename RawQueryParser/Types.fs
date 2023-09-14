// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace RawQueryParser

/// <summary>
/// Differentiate between an identifier segment, e.g. 'Detail' and an array segment: []
/// </summary>
type FieldSegment =
    | IdentifierSegment of string
    | ArraySegment

/// <summary>
/// Defines a field with hierarchical access fields.
/// E.g. <c>Field [IdentifierSegment "Detail"; IdentifierSegment "de"; IdentifierSegment "Title"]</c>
/// or <c>Field [IdentifierSegment "Features"; ArraySegment; IdentifierSegment "Id"]</c>
/// </summary>
type Field = Field of fields: FieldSegment list
with member self.ToNestedObject(value) =
        let (Field fields) = self
        let rec loop m = function
            | [] ->
                (box value)
            | IdentifierSegment h::t ->
                box (dict [h, box (loop m t)])
            | ArraySegment::t ->
                box [loop m t]
        loop value fields

module Sorting =
    /// Defines the sort direction.
    type OrderBy =
        | Ascending
        | Descending

    /// A sort statement is composed of a
    /// property and a sort direction.
    type SortStatement =
        { Field: Field
          Direction: OrderBy }

    /// Sort statements are a list of sort statement.
    type SortStatements = SortStatement list

module Filtering =
    /// Defines the possible operators.
    type Operator =
        | Eq
        | Ne
        | Lt
        | Le
        | Gt
        | Ge
        | Like

    /// Defines values of booleans, numbers and strings.
    type Value =
        | Boolean of bool
        | Number of float
        | String of string
        | DateTime of System.DateTime
        | Array

    /// The condition is the combination of a property, an operator and a value.
    type Comparison =
        { Field: Field
          Operator: Operator
          Value: Value }

    type Condition =
        | Comparison of Comparison
        | In of field: Field * values: Value list
        | NotIn of field: Field * values: Value list
        | LikeIn of Field: Field * value: Value list
        | IsNull of Field
        | IsNotNull of Field

    /// A filter statement can be a simple condition or a bunch
    /// of conditions inside an AND or an OR binary statement.
    type FilterStatement =
        | And of left: FilterStatement * right: FilterStatement
        | Or of left: FilterStatement * right: FilterStatement
        | Condition of Condition

