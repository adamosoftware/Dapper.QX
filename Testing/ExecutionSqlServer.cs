﻿using AdamOneilSoftware;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlIntegration.Library;
using SqlIntegration.Library.Classes;
using SqlIntegration.Library.Extensions;
using SqlServer.LocalDb;
using SqlServer.LocalDb.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using Testing.Queries;

namespace Testing
{
    [TestClass]
    public class ExecutionSqlServer : ExecutionBase
    {
        [TestMethod]
        public void SampleTableQueries()
        {
            using (var cn = LocalDb.GetConnection("DapperQX", CreateSampleSchema))
            {

            }
        }

        private void CreateSampleSchema(SqlConnection connection)
        {
            var statements = new InitializeStatement[]
            {
                new InitializeStatement(
                    "dbo.SampleTable", "DROP TABLE %obj%", @"CREATE TABLE %obj% (
                        [FirstName] nvarchar(50) NOT NULL,
                        [Weight] decimal(5,2) NOT NULL,
                        [SomeDate] datetime NOT NULL,
                        [Id] int identity(1,1) PRIMARY KEY
                    )")
            };
            LocalDb.ExecuteInitializeStatements(connection, statements);
            
            var tdg = new TestDataGenerator();
            tdg.Generate<SampleTableResult>(1000, (result) =>
            {
                result.FirstName = tdg.Random(Source.FirstName);
                result.SomeDate = tdg.RandomInRange(-1000, 1000, (i) => DateTime.Today.AddDays(i));
                result.Weight = tdg.RandomInRange<decimal>(50, 150, (d) => d);
            }, (results) =>
            {
                var dataTable = results.ToDataTable();
                BulkInsert.ExecuteAsync(dataTable, connection, DbObject.Parse("dbo.SampleTable"), 50, new BulkInsertOptions()
                {
                    SkipIdentityColumn = "Id"
                }).Wait();
            });
        }

        protected override IDbConnection GetConnection()
        {
            //string connectionString = GetConnectionString("SqlServer")
            throw new NotImplementedException();
        }
    }
}
