using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
    public class DecimalExtensionsTests
    {
        [Theory]
        [MemberData(nameof(SuccessData))]
        public void GetDecimalNullable_Success(decimal? nullableParam, decimal notNullableParam, decimal? expectedNullableValue, decimal expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (decimal? nullableValue, decimal? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [MemberData(nameof(FailureData))]
        public void GetDecimalNullable_Failure(decimal? nullableParam, decimal notNullableParam, decimal? expectedNullableValue, decimal expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (decimal? nullableValue, decimal? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        public static IEnumerable<object[]> SuccessData =>
            new List<object[]>
            {
                new object[] { 0m, 185.11m, 0m, 185.11m },
                new object[] { -99.1m, 5676575767m, -99.1m, 5676575767m },
                new object[] { null, -1m, null, -1m }
            };

        public static IEnumerable<object[]> FailureData =>
            new List<object[]>
            {
                new object[] { 17.25544m, 67576m, -4545.1m, -0.005m },
                new object[] { -545m, -5.11m, null, 1m },
                new object[] { null, 5m, 34.22m, 5.000001m }
            };

        private SqliteConnection InitializeSqlData(decimal? nullableParam, decimal notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    Decimal NULL
                ,   NotNullableData Decimal NOT NULL
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

		private (decimal? nullableValue, decimal? notNullableValue) ReadData(SqliteConnection connection)
		{
            decimal? nullableValue = null;
            decimal? notNullableValue = null;

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
                    nullableValue = reader.GetDecimalNullable(0);
                    notNullableValue = reader.GetDecimalNullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}