using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;

using System.Data.SqlServerCe;
using System.Reflection;

namespace KöTaf.Utils.Database
{
    public class Tools
    {
        public static IEnumerable<string> GetFileDatabaseTables(string databaseFile, string password = null)
        {
            if (string.IsNullOrEmpty(databaseFile) || !File.Exists(databaseFile))
            {
                throw new Exception("Database not exists");
            }

            IList<string> tableNames = new List<string>();

            SqlCeConnectionStringBuilder connectionString = new SqlCeConnectionStringBuilder();
            connectionString.DataSource = databaseFile;
            
            if (!string.IsNullOrEmpty(password))
                connectionString.Password = password;

            using (SqlCeConnection sqlCon = new SqlCeConnection(connectionString.ToString()))
            {
                string sqlStmt = "SELECT table_name FROM INFORMATION_SCHEMA.TABLES";
                using (SqlCeCommand cmd = new SqlCeCommand(commandText: sqlStmt, connection: sqlCon))
                {
                    sqlCon.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tableNames.Add((string)reader["table_name"]);
                    }
                    sqlCon.Close();
                }
            }

            return tableNames;
        }
    }
}
