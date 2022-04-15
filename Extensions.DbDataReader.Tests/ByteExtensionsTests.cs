using Microsoft.Data.Sqlite;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
	public class ByteExtensionsTests
	{
        [Theory]
        [InlineData((byte)5, (byte)1, (byte)5, (byte)1)]
        [InlineData((byte)0, (byte)255, (byte)0, (byte)255)]
        [InlineData(null, (byte)100, null, (byte)100)]
        public void GetByteNullable_Success(byte? nullableParam, byte notNullableParam, byte? expectedNullableValue, byte expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (byte? nullableValue, byte? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [InlineData((byte)87, (byte)11, (byte)44, (byte)51)]
        [InlineData((byte)21, (byte)55, null, (byte)5)]
        [InlineData(null, (byte)2, (byte)111, (byte)102)]
        public void GetByteNullable_Failure(byte? nullableParam, byte notNullableParam, byte? expectedNullableValue, byte expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (byte? nullableValue, byte? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        private SqliteConnection InitializeSqlData(byte? nullableParam, byte notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    TINYINT NULL
                ,   NotNullableData TINYINT NOT NULL
                )
            ";
            createSchemaCommand.ExecuteNonQuery();

            SqliteCommand insertDataCommand = connection.CreateCommand();
            insertDataCommand.CommandText =
            @"
                INSERT INTO Test
                    (NullableData, NotNullableData)
                VALUES
                    (@NullableParam, @NotNullableParam)
            ";
            insertDataCommand.Parameters.Add(new SqliteParameter("NullableParam", (object)nullableParam ?? System.DBNull.Value));
            insertDataCommand.Parameters.Add(new SqliteParameter("NotNullableParam", notNullableParam));
            insertDataCommand.ExecuteNonQuery();

            return connection;
        }

        private (byte? nullableValue, byte? notNullableValue) ReadData(SqliteConnection connection)
        {
            byte? nullableValue = null;
            byte? notNullableValue = null;

            SqliteCommand readDataCommand = connection.CreateCommand();
            readDataCommand.CommandText =
            @"
                SELECT
                    NullableData
                ,   NotNullableData
                FROM
                    Test
            ";

            using (SqliteDataReader reader = readDataCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    nullableValue = reader.GetByteNullable(0);
                    notNullableValue = reader.GetByteNullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
    }
}
