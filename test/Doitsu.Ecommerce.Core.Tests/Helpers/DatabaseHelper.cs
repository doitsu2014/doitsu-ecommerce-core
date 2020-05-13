using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Optional;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Doitsu.Ecommerce.Core;
using Doitsu.Ecommerce.Core.Data;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public static class DatabaseHelper
    {
        public static async Task<List<string>> GetMatchingDatabasesAsync(string connectionString, string prefix)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = $"SELECT name FROM master.dbo.sysdatabases WHERE name LIKE '{prefix}-%'";
                var myCommand = new SqlCommand(command, connection);
                connection.Open();
                var result = new List<string>();
                using (var reader = await myCommand.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
                return result;
            };
        }

        public static void DeleteDatabase(string connectionString, string databaseName)
        {
            ExecuteNonQuery(connectionString, "DROP DATABASE [" + databaseName + "]");
            if (ExecuteRowCount(connectionString, "sys.databases", $"WHERE [Name] = '{databaseName}'") == 1)
                throw new InvalidOperationException($"Failed to deleted {databaseName}. Did you have SSMS open or something?");
        }

        public static void KillConnections(string connectionString, string databaseName)
        {
            var query = string.Format(@"
                DECLARE @killConnections varchar(8000) = '';  
                SELECT @killConnections = @killConnections + 'kill ' + CONVERT(varchar(5), session_id) + ';'
                FROM sys.dm_exec_sessions
                WHERE database_id  = db_id('{0}');
                EXEC(@killConnections);
            ", databaseName);

            ExecuteNonQuery(connectionString, query);
        }

        public static string GeneratePoolKeyConnectionString(string connectionStringValue, string poolKey = "")
        {
            var parts = connectionStringValue.Split(';').ToList();
            var database = parts.Single(p => p.StartsWith("Database="));
            parts.Remove(database);
            var newDatabase = $"{database}.{poolKey}";
            parts.Add(newDatabase);
            return parts.Aggregate((x, y) => $"{x};{y}");
        }

        public static Option<string, string> GetPoolConnectionString(string poolKey, IWebHost host)
        {
            return (poolKey, host).SomeNotNull()
                .WithException(string.Empty)
                .Filter(pk => !string.IsNullOrEmpty(pk.poolKey), "pool key is empty")
                .Filter(pk => pk.host != null, "host is null")
                .Map(pk =>
                {
                    var listConnectionString = new List<string>();
                    host.Services.GetService<IConfiguration>().GetSection("ConnectionPoolStrings").Bind(listConnectionString);

                    var connectionString = listConnectionString
                            .Select(x => new { PoolKey = x.Split("|")[0], PoolConnectionString = x.Split("|")[1] })
                            .FirstOrDefault(x => x.PoolKey == poolKey)
                            .PoolConnectionString;

                    return connectionString;
                });
        }

        [Obsolete]
        public static void TruncateAllTable(IWebHost host, string poolKey)
        {
            var dbContext = host.Services.GetService<EcommerceDbContext>();
            dbContext.Database.ExecuteSqlRaw(@"
                EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? NOCHECK CONSTRAINT ALL'  
                EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? DISABLE TRIGGER ALL'  
                DECLARE @deleteFromAllTable varchar(8000) = '';
                SELECT @deleteFromAllTable = Concat(@deleteFromAllTable, 'DELETE FROM ', TABLE_SCHEMA, '.',TABLE_NAME, ';') 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME NOT LIKE '__EFMigrationsHistory'
                EXEC(@deleteFromAllTable)
                EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? CHECK CONSTRAINT ALL' 
                EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? ENABLE TRIGGER ALL'
            ");
        }

        public static void ReseedAllTable(IWebHost host, string poolKey)
        {
            var dbContext = host.Services.GetService<EcommerceDbContext>();
            dbContext.Database.ExecuteSqlRaw(@"
                DECLARE @reseedTableQuery NVARCHAR(MAX) = '';
                SELECT 
                    @reseedTableQuery = CONCAT(@reseedTableQuery, '
                        IF NOT EXISTS (SELECT 1 FROM [' + TABLE_NAME + '])
                            BEGIN
                                DBCC CHECKIDENT ([' + TABLE_NAME + '], RESEED, 1);
                            END
                        ELSE
                            BEGIN
                                DELETE FROM [' + TABLE_NAME + ']
                                DBCC CHECKIDENT ([' + TABLE_NAME + '], RESEED, 0);
                            END
                    ')
                FROM 
                    INFORMATION_SCHEMA.TABLES
                WHERE 
                    OBJECTPROPERTY(OBJECT_ID(TABLE_NAME), 'TableHasIdentity') = 1
                    AND TABLE_TYPE = 'BASE TABLE'
                EXECUTE (@reseedTableQuery)
            ");
        }

        public static async Task MakeEmptyDatabase(IWebHost host, string poolKey)
        {
            var dbContext = host.Services.GetService<EcommerceDbContext>();
            await dbContext.Database.ExecuteSqlRawAsync(@"
                EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? NOCHECK CONSTRAINT ALL'  
                EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? DISABLE TRIGGER ALL'  
                DECLARE @deleteFromAllTable varchar(8000) = '';
                SELECT @deleteFromAllTable = Concat(@deleteFromAllTable, 'DELETE FROM ', TABLE_SCHEMA, '.',TABLE_NAME, ';') 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME NOT LIKE '__EFMigrationsHistory'
                EXEC(@deleteFromAllTable)
                EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? CHECK CONSTRAINT ALL' 
                EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? ENABLE TRIGGER ALL'
            ");
            await dbContext.Database.ExecuteSqlRawAsync(@"
                DECLARE @reseedTableQuery NVARCHAR(MAX) = '';
                SELECT 
                    @reseedTableQuery = CONCAT(@reseedTableQuery, '
                        IF NOT EXISTS (SELECT 1 FROM [' + TABLE_NAME + '])
                            BEGIN
                                DBCC CHECKIDENT ([' + TABLE_NAME + '], RESEED, 1);
                            END
                        ELSE
                            BEGIN
                                DELETE FROM [' + TABLE_NAME + ']
                                DBCC CHECKIDENT ([' + TABLE_NAME + '], RESEED, 0);
                            END
                    ')
                FROM 
                    INFORMATION_SCHEMA.TABLES
                WHERE 
                    OBJECTPROPERTY(OBJECT_ID(TABLE_NAME), 'TableHasIdentity') = 1
                    AND TABLE_TYPE = 'BASE TABLE'
                EXECUTE (@reseedTableQuery)
            ");
        }

        public static int ExecuteNonQuery(string connectionString, string command)
        {
            using (var myConn = new SqlConnection(connectionString))
            {
                var myCommand = new SqlCommand(command, myConn);
                myConn.Open();
                return myCommand.ExecuteNonQuery();
            }
        }

        public static int ExecuteRowCount(this string connectionString, string tableName, string whereClause = "")
        {
            using (var myConn = new SqlConnection(connectionString))
            {
                var command = "SELECT COUNT(*) FROM " + tableName + " " + whereClause;
                var myCommand = new SqlCommand(command, myConn);
                myConn.Open();
                return (int)myCommand.ExecuteScalar();
            }
        }
    }
}
