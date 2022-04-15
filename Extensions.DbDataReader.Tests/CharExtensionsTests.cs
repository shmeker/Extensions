using Microsoft.Data.Sqlite;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
    public class CharExtensionsTests
    {
        [Theory]
        [InlineData('d', '7', 'd', '7')]
        [InlineData('A', 'z', 'A', 'z')]
        [InlineData(null, '0', null, '0')]
        public void GetCharNullable_Success(char? nullableParam, char notNullableParam, char? expectedNullableValue, char expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (char? nullableValue, char? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [InlineData('2', '2', '5', '5')]
        [InlineData('v', 'y', 'd', 'Y')]
        [InlineData(null, 'm', 'n', 'P')]
        public void GetCharNullable_Failure(char? nullableParam, char notNullableParam, char? expectedNullableValue, char expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (char? nullableValue, char? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        private SqliteConnection InitializeSqlData(char? nullableParam, char notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    NVARCHAR(1) NULL
                ,   NotNullableData NVARCHAR(1) NOT NULL
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

		private (char? nullableValue, char? notNullableValue) ReadData(SqliteConnection connection)
		{
            char? nullableValue = null;
            char? notNullableValue = null;

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
                    nullableValue = reader.GetCharNullable(0);
                    notNullableValue = reader.GetCharNullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}