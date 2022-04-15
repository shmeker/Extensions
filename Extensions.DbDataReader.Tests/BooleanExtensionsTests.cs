using Microsoft.Data.Sqlite;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
    public class BooleanExtensionsTests
    {
        [Theory]
        [InlineData(true, true, true, true)]
        [InlineData(false, true, false, true)]
        [InlineData(null, false, null, false)]
        public void GetBooleanNullable_Success(bool? nullableParam, bool notNullableParam, bool? expectedNullableValue, bool expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (bool? nullableValue, bool? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [InlineData(true, true, false, false)]
        [InlineData(false, true, null, false)]
        [InlineData(null, false, true, true)]
        public void GetBooleanNullable_Failure(bool? nullableParam, bool notNullableParam, bool? expectedNullableValue, bool expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (bool? nullableValue, bool? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        private SqliteConnection InitializeSqlData(bool? nullableParam, bool notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    BIT NULL
                ,   NotNullableData BIT NOT NULL
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

		private (bool? nullableValue, bool? notNullableValue) ReadData(SqliteConnection connection)
		{
            bool? nullableValue = null;
            bool? notNullableValue = null;

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
                    nullableValue = reader.GetBooleanNullable(0);
                    notNullableValue = reader.GetBooleanNullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}