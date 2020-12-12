namespace RawQueryParser

/// <summary>
/// Defines a Property with hierarchial access fields.
/// E.g. <c>Property ["Detail"; "de"; "Title"]</c>
/// </summary>
type Property = Property of fields: string list

module Sorting =
    /// Defines the sort direction.
    type OrderBy =
        | Ascending
        | Descending

    /// A sort statement is composed of a
    /// property and a sort direction.
    type SortStatement =
        { Property: Property
          Direction: OrderBy }

    /// Sort statements are a list of sort statement.
    type SortStatements = SortStatement list

module Filtering =
    type Operator =
        | Eq
        | Ne
        | Lt
        | Le
        | Gt
        | Ge

    type Value =
        | Boolean of bool
        | Number of float
        | String of string

    type Condition =
        { Property: Property
          Operator: Operator
          Value: Value }

    type FilterStatement =
        | And of left: FilterStatement * right: FilterStatement
        | Or of left: FilterStatement * right: FilterStatement
        | Condition of Condition

