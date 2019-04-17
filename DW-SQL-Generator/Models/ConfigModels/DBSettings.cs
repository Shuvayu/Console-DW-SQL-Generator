namespace DW_SQL_Generator.Models.ConfigModels
{
    public class DBSettings
    {

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string TableName { get; set; }

        public string SchemaName { get; set; }


        public bool IsNotValid() {

            return string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(DatabaseName) || string.IsNullOrEmpty(TableName) ? true : false;
        }
    }
}
