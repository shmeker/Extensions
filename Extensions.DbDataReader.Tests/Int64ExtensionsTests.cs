using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
    public class Int64ExtensionsTests
    {
        [Theory]
        [MemberData(nameof(SuccessData))]
        public void GetInt64Nullable_Success(long? nullableParam, long notNullableParam, long? expectedNullableValue, long expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (long? nullableValue, long? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [MemberData(nameof(FailureData))]
        public void GetInt64Nullable_Failure(long? nullableParam, long notNullableParam, long? expectedNullableValue, long expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (long? nullableValue, long? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        public static IEnumerable<object[]> SuccessData =>
            new List<object[]>
            {
                new object[] { 0L, 185L, 0L, 185L },
                new object[] { -99L, 567685677655523L, -99L, 567685677655523L },
                new object[] { null, -1L, null, -1L }
            };

        public static IEnumerable<object[]> FailureData =>
            new List<object[]>
            {
                new object[] { 17L, 675L, -4545L, -1L },
                new object[] { -545L, -5L, null, 1L },
                new object[] { null, 5L, 34L, 6L }
            };

        private SqliteConnection InitializeSqlData(long? nullableParam, long notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    BIGINT NULL
                ,   NotNullableData BIGINT NOT NULL
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

		private (long? nullableValue, long? notNullableValue) ReadData(SqliteConnection connection)
		{
            long? nullableValue = null;
            long? notNullableValue = null;

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
                    nullableValue = reader.GetInt64Nullable(0);
                    notNullableValue = reader.GetInt64Nullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}