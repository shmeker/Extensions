using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
    public class Int16ExtensionsTests
    {
        [Theory]
        [MemberData(nameof(SuccessData))]
        public void GetInt16Nullable_Success(short? nullableParam, short notNullableParam, short? expectedNullableValue, short expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (short? nullableValue, short? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [MemberData(nameof(FailureData))]
        public void GetInt16Nullable_Failure(short? nullableParam, short notNullableParam, short? expectedNullableValue, short expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (short? nullableValue, short? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        public static IEnumerable<object[]> SuccessData =>
            new List<object[]>
            {
                new object[] { (short?)0, (short?)185, (short?)0, (short?)185 },
                new object[] { (short?)-99, (short?)5676, (short?)-99, (short?)5676 },
                new object[] { null, (short?)-1, null, (short?)-1 }
            };

        public static IEnumerable<object[]> FailureData =>
            new List<object[]>
            {
                new object[] { (short?)17, (short?)675, (short?)-4545, (short?)-1 },
                new object[] { (short?)-545, (short?)-5, null, (short?)1 },
                new object[] { null, (short?)5, (short?)34, (short?)6 }
            };

        private SqliteConnection InitializeSqlData(short? nullableParam, short notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    SMALLINT NULL
                ,   NotNullableData SMALLINT NOT NULL
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

		private (short? nullableValue, short? notNullableValue) ReadData(SqliteConnection connection)
		{
            short? nullableValue = null;
            short? notNullableValue = null;

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
                    nullableValue = reader.GetInt16Nullable(0);
                    notNullableValue = reader.GetInt16Nullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}