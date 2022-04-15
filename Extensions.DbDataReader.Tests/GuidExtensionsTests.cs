using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Extensions.DbDataReader.Tests
{
    public class GuidExtensionsTests
    {
        [Theory]
        [MemberData(nameof(SuccessData))]
        public void GetGuidNullable_Success(Guid? nullableParam, Guid notNullableParam, Guid? expectedNullableValue, Guid expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (Guid? nullableValue, Guid? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.Equal(expectedNullableValue, nullableValue);
            Assert.Equal(expectedNotNullableValue, notNullableValue!.Value);
        }

        [Theory]
        [MemberData(nameof(FailureData))]
        public void GetGuidNullable_Failure(Guid? nullableParam, Guid notNullableParam, Guid? expectedNullableValue, Guid expectedNotNullableValue)
        {
            SqliteConnection connection = InitializeSqlData(nullableParam, notNullableParam);

            (Guid? nullableValue, Guid? notNullableValue) = ReadData(connection);

            Assert.NotNull(notNullableValue);
            Assert.NotEqual(expectedNullableValue, nullableValue);
            Assert.NotEqual(expectedNotNullableValue, notNullableValue!.Value);
        }

        public static IEnumerable<object[]> SuccessData =>
            new List<object[]>
            {
                new object[] { new Guid("4ded2d06-1343-4765-a6a4-656164f7e7c0"), new Guid("d798d33a-7781-4fe4-b01b-203fdab3ae52"), new Guid("4ded2d06-1343-4765-a6a4-656164f7e7c0"), new Guid("d798d33a-7781-4fe4-b01b-203fdab3ae52") },
                new object[] { new Guid("8d3595eb-ac0a-4c35-bdce-eb469f74085d"), new Guid("f34968e4-8d74-44c7-b548-792ceba31178"), new Guid("8d3595eb-ac0a-4c35-bdce-eb469f74085d"), new Guid("f34968e4-8d74-44c7-b548-792ceba31178") },
                new object[] { null, new Guid("afa0abda-062b-42af-ba58-04caa9cd286a"), null, new Guid("afa0abda-062b-42af-ba58-04caa9cd286a") }
            };

        public static IEnumerable<object[]> FailureData =>
            new List<object[]>
            {
                new object[] { new Guid("e1eda411-5a8c-4b9b-af7c-7adce2e593fa"), new Guid("91fc1d6b-9d9c-4b10-8d49-b0aadfe94a8b"), new Guid("e39852a2-49a3-4de0-98bc-4d2a55dfa431"), new Guid("f728edbe-79c4-461e-a924-5d3418fabc8c") },
                new object[] { new Guid("7c0b3066-03ea-40c1-b0eb-5ab7054b7b1e"), new Guid("ed8588d3-628f-4b0f-a7be-59d39b4f6fed"), null, new Guid("2127e988-d122-446b-88fb-ac44b553bdbf") },
                new object[] { null, new Guid("c8d34ad1-50f4-457e-a3f5-5bd9ac3c0531"), new Guid("f5aab519-3cb0-4079-9912-fb8989fe3374"), new Guid("23b8e7d9-fde0-4dc0-932a-876db6326ce6") }
            };

        private SqliteConnection InitializeSqlData(Guid? nullableParam, Guid notNullableParam)
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            SqliteCommand createSchemaCommand = connection.CreateCommand();
            createSchemaCommand.CommandText =
            @"
                CREATE TABLE Test
                (
                    NullableData    UNIQUEIDENTIFIER NULL
                ,   NotNullableData UNIQUEIDENTIFIER NOT NULL
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

		private (Guid? nullableValue, Guid? notNullableValue) ReadData(SqliteConnection connection)
		{
            Guid? nullableValue = null;
            Guid? notNullableValue = null;

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
                    nullableValue = reader.GetGuidNullable(0);
                    notNullableValue = reader.GetGuidNullable("NotNullableData");
                }
            }

            connection.Close();
            connection.Dispose();

            return (nullableValue, notNullableValue);
        }
	}
}