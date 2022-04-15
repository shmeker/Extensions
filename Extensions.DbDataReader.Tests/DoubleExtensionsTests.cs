using Microsoft.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
    public class DoubleExtensionsTests
    {
        [Theory]
        [MemberData(nameof(SuccessData))]
        public void GetDoubleNullable_Success(double? nullableParam, double notNullableParam, double? expectedNullableValue, double expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (double? nullableValue, double? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [MemberData(nameof(FailureData))]
        public void GetDoubleNullable_Failure(double? nullableParam, double notNullableParam, double? expectedNullableValue, double expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (double? nullableValue, double? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        public static IEnumerable<object[]> SuccessData =>
            new List<object[]>
            {
                new object[] { 0, 185.11, 0, 185.11 },
                new object[] { -99.1, 5676575767, -99.1, 5676575767 },
                new object[] { null, -1, null, -1 }
            };

        public static IEnumerable<object[]> FailureData =>
            new List<object[]>
            {
                new object[] { 17.25544, 67576, -4545.1, -0.005 },
                new object[] { -545, -5.11, null, 1 },
                new object[] { null, 5, 34.22, 5.000001 }
            };

        private SqliteConnection InitializeSqlData(double? nullableParam, double notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    FLOAT NULL
                ,   NotNullableData FLOAT NOT NULL
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

		private (double? nullableValue, double? notNullableValue) ReadData(SqliteConnection connection)
		{
            double? nullableValue = null;
            double? notNullableValue = null;

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
                    nullableValue = reader.GetDoubleNullable(0);
                    notNullableValue = reader.GetDoubleNullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}