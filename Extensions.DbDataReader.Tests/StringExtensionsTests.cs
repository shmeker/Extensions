using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [MemberData(nameof(SuccessData))]
        public void GetStringNullable_Success(string? nullableParam, string notNullableParam, string? expectedNullableValue, string expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (string? nullableValue, string? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue);
        }

        [Theory]
        [MemberData(nameof(FailureData))]
        public void GetStringNullable_Failure(string? nullableParam, string notNullableParam, string? expectedNullableValue, string expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (string? nullableValue, string? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue);
        }

        public static IEnumerable<object[]> SuccessData =>
            new List<object[]>
            {
                new object[] { "", "test", "", "test" },
                new object[] { "Avn$$$dfs'2", "15158888", "Avn$$$dfs'2", "15158888" },
                new object[] { null, "6n66n66n6n6", null, "6n66n66n6n6" }
            };

        public static IEnumerable<object[]> FailureData =>
            new List<object[]>
            {
                new object[] { "", "mkm50904905", "fdmiofsmiof489", "1" },
                new object[] { "555565", "fdsfd00d0dsvcyy", null, "00'0''''  dsfsf sd " },
                new object[] { null, " ", "", "  " }
            };

        private SqliteConnection InitializeSqlData(string? nullableParam, string notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    NVARCHAR(500) NULL
                ,   NotNullableData NVARCHAR(500) NOT NULL
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

		private (string? nullableValue, string? notNullableValue) ReadData(SqliteConnection connection)
		{
            string? nullableValue = null;
            string? notNullableValue = null;

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
                    nullableValue = reader.GetStringNullable(0);
                    notNullableValue = reader.GetStringNullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}