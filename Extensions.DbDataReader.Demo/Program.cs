// See https://aka.ms/new-console-template for more information

using Microsoft.Data.Sqlite;
using Extensions.DbDataReader;

SqliteConnection connection = new("Data Source=:memory:");
connection.Open();

InitializeSchema(connection);
InitializeData(connection);

PrintData(connection);

connection.Close();
connection.Dispose();


void InitializeSchema(SqliteConnection connection)
{
    SqliteCommand createSchemaCommand = connection.CreateCommand();
    createSchemaCommand.CommandText =
    @"
        CREATE TABLE DemoData
        (
            BoolNullable        BIT NULL
        ,   BoolNotNullable     BIT NOT NULL
        ,   ByteNullable        TINYINT NULL
        ,   ByteNotNullable     TINYINT NOT NULL
        ,   CharNullable        NVARCHAR(1) NULL
        ,   CharNotNullable     NVARCHAR(1) NOT NULL
        ,   DateTimeNullable    DATETIME2(7) NULL
        ,   DateTimeNotNullable DATETIME2(7) NOT NULL
        ,   DecimalNullable     Decimal NULL
        ,   DecimalNotNullable  Decimal NOT NULL
        ,   DoubleNullable      FLOAT NULL
        ,   DoubleNotNullable   FLOAT NOT NULL
        ,   FloatNullable       FLOAT NULL
        ,   FloatNotNullable    FLOAT NOT NULL
        ,   GuidNullable        UNIQUEIDENTIFIER NULL
        ,   GuidNotNullable     UNIQUEIDENTIFIER NOT NULL
        ,   ShortNullable       SMALLINT NULL
        ,   ShortNotNullable    SMALLINT NOT NULL
        ,   IntNullable         INT NULL
        ,   IntNotNullable      INT NOT NULL
        ,   LongNullable        BIGINT NULL
        ,   LongNotNullable     BIGINT NOT NULL
        ,   StringNullable      NVARCHAR(500) NULL
        ,   StringNotNullable   NVARCHAR(500) NOT NULL
        )
    ";
    createSchemaCommand.ExecuteNonQuery();
}

void InitializeData(SqliteConnection connection)
{
    SqliteCommand insertDataCommand = connection.CreateCommand();
    insertDataCommand.CommandText =
    @"
        INSERT INTO DemoData
        (
            BoolNullable        
        ,   BoolNotNullable     
        ,   ByteNullable        
        ,   ByteNotNullable     
        ,   CharNullable        
        ,   CharNotNullable     
        ,   DateTimeNullable    
        ,   DateTimeNotNullable 
        ,   DecimalNullable     
        ,   DecimalNotNullable  
        ,   DoubleNullable      
        ,   DoubleNotNullable   
        ,   FloatNullable       
        ,   FloatNotNullable    
        ,   GuidNullable        
        ,   GuidNotNullable     
        ,   ShortNullable       
        ,   ShortNotNullable    
        ,   IntNullable         
        ,   IntNotNullable      
        ,   LongNullable        
        ,   LongNotNullable     
        ,   StringNullable      
        ,   StringNotNullable   
        )
        VALUES
        (
            1
        ,   0
        ,   15
        ,   30
        ,   'd'
        ,   '5'
        ,   '2022-01-01 17:52:11'
        ,   '1952-12-31 23:59:59'
        ,   125.21111
        ,   -87.01
        ,   88.1111
        ,   -0.005
        ,   9852211.1222
        ,   -85555.44
        ,   '0695b66b-b33e-4d11-9945-7684f611fa1f'
        ,   '1306aeae-06a8-4cd8-95d0-0fbccbaef523'
        ,   189
        ,   -215
        ,   5884212
        ,   -399999
        ,   3989478945
        ,   -2131354877
        ,   'this is a test string'
        ,   'another string'
        ),
        (
            NULL
        ,   0
        ,   NULL
        ,   30
        ,   NULL
        ,   '5'
        ,   NULL
        ,   '1952-12-31 23:59:59'
        ,   NULL
        ,   -87.01
        ,   NULL
        ,   -0.005
        ,   NULL
        ,   -85555.44
        ,   NULL
        ,   '1306aeae-06a8-4cd8-95d0-0fbccbaef523'
        ,   NULL
        ,   -215
        ,   NULL
        ,   -399999
        ,   NULL
        ,   -2131354877
        ,   NULL
        ,   'another string'
        )
    ";
    insertDataCommand.ExecuteNonQuery();
}

void PrintData(SqliteConnection connection)
{
    SqliteCommand selectCommand = connection.CreateCommand();
    selectCommand.CommandText =
    @"
        SELECT 
            BoolNullable        
        ,   BoolNotNullable     
        ,   ByteNullable        
        ,   ByteNotNullable     
        ,   CharNullable        
        ,   CharNotNullable     
        ,   DateTimeNullable    
        ,   DateTimeNotNullable 
        ,   DecimalNullable     
        ,   DecimalNotNullable  
        ,   DoubleNullable      
        ,   DoubleNotNullable   
        ,   FloatNullable       
        ,   FloatNotNullable    
        ,   GuidNullable        
        ,   GuidNotNullable     
        ,   ShortNullable       
        ,   ShortNotNullable    
        ,   IntNullable         
        ,   IntNotNullable      
        ,   LongNullable        
        ,   LongNotNullable     
        ,   StringNullable      
        ,   StringNotNullable   
        FROM
            DemoData
    ";

    using (SqliteDataReader reader = selectCommand.ExecuteReader())
	{
        int rowNumber = 1;
        while (reader.Read())
		{
            Console.WriteLine($"Row {rowNumber++} data:");

            Console.WriteLine("GetBooleanNullable".PadRight(30, ' ') + "| " + reader.GetBooleanNullable(0));
            Console.WriteLine("GetBooleanNullable".PadRight(30, ' ') + "| " + reader.GetBooleanNullable("BoolNotNullable"));
            Console.WriteLine("GetByteNullable".PadRight(30, ' ') + "| " + reader.GetByteNullable(2));
            Console.WriteLine("GetByteNullable".PadRight(30, ' ') + "| " + reader.GetByteNullable("ByteNotNullable"));
            Console.WriteLine("GetCharNullable".PadRight(30, ' ') + "| " + reader.GetCharNullable(4));
            Console.WriteLine("GetCharNullable".PadRight(30, ' ') + "| " + reader.GetCharNullable("CharNotNullable"));
            Console.WriteLine("GetDateTimeNullable".PadRight(30, ' ') + "| " + reader.GetDateTimeNullable(6));
            Console.WriteLine("GetDateTimeNullable".PadRight(30, ' ') + "| " + reader.GetDateTimeNullable("DateTimeNotNullable"));
            Console.WriteLine("GetDecimalNullable".PadRight(30, ' ') + "| " + reader.GetDecimalNullable(8));
            Console.WriteLine("GetDecimalNullable".PadRight(30, ' ') + "| " + reader.GetDecimalNullable("DecimalNotNullable"));
            Console.WriteLine("GetDoubleNullable".PadRight(30, ' ') + "| " + reader.GetDoubleNullable(10));
            Console.WriteLine("GetDoubleNullable".PadRight(30, ' ') + "| " + reader.GetDoubleNullable("DoubleNotNullable"));
            Console.WriteLine("GetFloatNullable".PadRight(30, ' ') + "| " + reader.GetFloatNullable(12));
            Console.WriteLine("GetFloatNullable".PadRight(30, ' ') + "| " + reader.GetFloatNullable("FloatNotNullable"));
            Console.WriteLine("GetGuidNullable".PadRight(30, ' ') + "| " + reader.GetGuidNullable(14));
            Console.WriteLine("GetGuidNullable".PadRight(30, ' ') + "| " + reader.GetGuidNullable("GuidNotNullable"));
            Console.WriteLine("GetInt16Nullable".PadRight(30, ' ') + "| " + reader.GetInt16Nullable(16));
            Console.WriteLine("GetInt16Nullable".PadRight(30, ' ') + "| " + reader.GetInt16Nullable("ShortNotNullable"));
            Console.WriteLine("GetInt32Nullable".PadRight(30, ' ') + "| " + reader.GetInt32Nullable(18));
            Console.WriteLine("GetInt32Nullable".PadRight(30, ' ') + "| " + reader.GetInt32Nullable("IntNotNullable"));
            Console.WriteLine("GetInt64Nullable".PadRight(30, ' ') + "| " + reader.GetInt64Nullable(20));
            Console.WriteLine("GetInt64Nullable".PadRight(30, ' ') + "| " + reader.GetInt64Nullable("LongNotNullable"));
            Console.WriteLine("GetStringNullable".PadRight(30, ' ') + "| " + reader.GetStringNullable(22));
            Console.WriteLine("GetStringNullable".PadRight(30, ' ') + "| " + reader.GetStringNullable("StringNotNullable"));

            Console.WriteLine();
        }
	}
}