using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;

namespace Nop.Data.Initializers
{
    /// <summary>
    /// 创建表（如果表不存在）
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        private readonly string[] _tablesToValidate;
        private readonly string[] _customCommands;

        /// <summary>
        /// Ctor 实例化一个创建表的类对象
        /// </summary>
        /// <param name="tablesToValidate">A list of existing table names to validate; null to don't validate table names</param>
        /// <param name="customCommands">A list of custom commands to execute</param>
        public CreateTablesIfNotExist(string[] tablesToValidate, string [] customCommands)
        {
            this._tablesToValidate = tablesToValidate;
            this._customCommands = customCommands;
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="context"></param>
        public void InitializeDatabase(TContext context)
        {
            bool dbExists;

            //使代码块成为事务性代码
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                //检测数据库是否存在
                dbExists = context.Database.Exists();
            }

            if (dbExists)
            {
                bool createTables;
                if (_tablesToValidate != null && _tablesToValidate.Length > 0)
                {
                    //we have some table names to validate
                    //已存在的表
                    var existingTableNames = new List<string>(context.Database.SqlQuery<string>("SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'"));

                    createTables = !existingTableNames.Intersect(_tablesToValidate, StringComparer.InvariantCultureIgnoreCase).Any();
                }
                else
                {
                    //check whether tables are already created
                    int numberOfTables = 0;
                    foreach (var t1 in context.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' "))
                    {
                        numberOfTables = t1;
                    }

                    createTables = numberOfTables == 0;
                }

                if (createTables)
                {
                    //创建所有表
                    //create all tables
                    var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                    context.Database.ExecuteSqlCommand(dbCreationScript);

                    //Seed(context);
                    context.SaveChanges();

                    if (_customCommands != null && _customCommands.Length > 0)
                    {
                        foreach (var command in _customCommands)
                        {
                            context.Database.ExecuteSqlCommand(command);
                        }
                    }
                }
            }
            else
            {
                //数据库不存在
                throw new ApplicationException("No database instance");
            }
        }
    }
}
