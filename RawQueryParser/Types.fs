namespace RawQueryParser

/// <summary>
/// Defines a field with hierarchial access fields.
/// E.g. <c>Field ["Detail"; "de"; "Title"]</c>
/// </summary>
type Field = Field of fields: string list

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

    /// Defines the values of tye booleans, number or string.
    type Value =
        | Boolean of bool
        | Number of float
        | String of string

    /// The condition is the combination of a property, an operator and a value.
    type Condition =
        { Field: Field
          Operator: Operator
          Value: Value }

    /// A filter statement can be a simple condition or a bunch
    /// of conditions inside an AND or an OR binary statement.
    type FilterStatement =
        | And of left: FilterStatement * right: FilterStatement
        | Or of left: FilterStatement * right: FilterStatement
        | Condition of Condition
        | In of field: Field * values: Value list
        | NotIn of field: Field * values: Value list
        | IsNull of Field
        | IsNotNull of Field

