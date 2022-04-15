
# Welcome to the `DbDataReader` extensions library

This is a simple library for reading with `DBNull` checking from the `DbDataReader` object.
This library is also working with inheriting types from `DbDataReader`, like `SqlDataReader` or `SqliteDataReader`

Every method returns *nullable* version of types, with value of `null` when value in `DbDataReader` object is `DBNull`.

## Supported types

Following are supported types for data read:
- `Bool`
- `Byte`
- `Char`
- `DateTime`
- `Decimal`
- `Double`
- `Float`
- `Guid`
- `Int16`
- `Int32`
- `Int64`
- `String`
