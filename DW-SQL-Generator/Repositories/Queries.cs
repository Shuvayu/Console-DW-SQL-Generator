namespace DW_SQL_Generator.Repositories
{
    public class Queries
    {
        public const string getColumnNamesAndTypes = @"USE [{0}]
                                                SELECT COLUMN_NAME,DATA_TYPE
                                                FROM INFORMATION_SCHEMA.COLUMNS
                                                WHERE TABLE_NAME = @TableName AND TABLE_SCHEMA=@SchemaName";
    }
}
