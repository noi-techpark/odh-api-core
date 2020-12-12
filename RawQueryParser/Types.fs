namespace RawQueryParser

/// Defines the sort direction.
type OrderBy =
    | Ascending
    | Descending

/// <summary>
/// Defines a Property with hierarchial access fields.
/// E.g. <c>Property ["Detail"; "de"; "Title"]</c>
/// </summary>
type Property = Property of fields: string list

/// A sort statement is composed of a
/// property and a sort direction.
type SortStatement =
    { Property: Property
      Direction: OrderBy }

/// Sort statements are a list of sort statement.
type SortStatements = SortStatement list

