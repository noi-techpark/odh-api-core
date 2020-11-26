namespace RawQueryParser

type SortDirection =
    | Ascending
    | Descending

type Property = Property of fields: string list

type SortStatement =
    { Property: Property
      Direction: SortDirection }

type SortStatements = SortStatement list


