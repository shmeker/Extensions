using Microsoft.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
    public class FloatExtensionsTests
    {
        [Theory]
        [MemberData(nameof(SuccessData))]
        public void GetFloatNullable_Success(float? nullableParam, float notNullableParam, float? expectedNullableValue, float expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (float? nullableValue, float? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [MemberData(nameof(FailureData))]
        public void GetFloatNullable_Failure(float? nullableParam, float notNullableParam, float? expectedNullableValue, float expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (float? nullableValue, float? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        public static IEnumerable<object[]> SuccessData =>
            new List<object[]>
            {
                new object[] { 0f, 185.11f, 0f, 185.11f },
                new object[] { -99.1f, 5676575767f, -99.1f, 5676575767f },
                new object[] { null, -1f, null, -1f }
            };

        public static IEnumerable<object[]> FailureData =>
            new List<object[]>
            {
                new object[] { 17.25544f, 67576f, -4545.1f, -0.005f },
                new object[] { -545f, -5.11f, null, 1f },
                new object[] { null, 5f, 34.22f, 5.000001f }
            };

        private SqliteConnection InitializeSqlData(float? nullableParam, float notNullableParam)
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

		private (float? nullableValue, float? notNullableValue) ReadData(SqliteConnection connection)
		{
            float? nullableValue = null;
            float? notNullableValue = null;

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
                    nullableValue = reader.GetFloatNullable(0);
                    notNullableValue = reader.GetFloatNullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}