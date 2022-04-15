using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
	public class DateTimeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(SuccessData))]
        public void GetDateTimeNullable_Success(DateTime? nullableParam, DateTime notNullableParam, DateTime? expectedNullableValue, DateTime expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (DateTime? nullableValue, DateTime? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [MemberData(nameof(FailureData))]
        public void GetDateTimeNullable_Failure(DateTime? nullableParam, DateTime notNullableParam, DateTime? expectedNullableValue, DateTime expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (DateTime? nullableValue, DateTime? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        public static IEnumerable<object[]> SuccessData =>
            new List<object[]>
            {
                new object[] { new DateTime(2022, 5, 4), new DateTime(1957, 12, 31), new DateTime(2022, 5, 4), new DateTime(1957, 12, 31) },
                new object[] { new DateTime(2022, 5, 4, 21, 2, 31), new DateTime(1957, 12, 31, 0, 0, 1), new DateTime(2022, 5, 4, 21, 2, 31), new DateTime(1957, 12, 31, 0, 0, 1) },
                new object[] { null, DateTime.MinValue, null, DateTime.MinValue },
            };

        public static IEnumerable<object[]> FailureData =>
            new List<object[]>
            {
                new object[] { new DateTime(2022, 5, 4), new DateTime(1957, 12, 31), new DateTime(2022, 5, 3), new DateTime(1957, 12, 30) },
                new object[] { new DateTime(2022, 5, 4, 21, 2, 30), new DateTime(1957, 12, 31, 0, 0, 1), new DateTime(2022, 5, 4, 21, 2, 31), new DateTime(1957, 12, 31, 0, 0, 10) },
                new object[] { null, DateTime.MinValue, DateTime.MinValue, DateTime.MaxValue },
            };

        private SqliteConnection InitializeSqlData(DateTime? nullableParam, DateTime notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    DATETIME2(7) NULL
                ,   NotNullableData DATETIME2(7) NOT NULL
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

		private (DateTime? nullableValue, DateTime? notNullableValue) ReadData(SqliteConnection connection)
		{
            DateTime? nullableValue = null;
            DateTime? notNullableValue = null;

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
                    nullableValue = reader.GetDateTimeNullable(0);
                    notNullableValue = reader.GetDateTimeNullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}