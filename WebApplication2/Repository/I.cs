using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Repository
{
    public interface IDbContext : IDisposable
    {
        Database Database { get; }
        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities</returns>
        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new();
        IList<ResultEntity> ExecuteStoredProcedureDataTable<ResultEntity>(Func<DbDataReader, ResultEntity> objConvert, string commandText, params object[] parameters) where ResultEntity : class;
        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);

        /// <summary>
        /// 提供一个更加底层的函数。能过直接获得数据DataReader对象。用来解决某些特殊情况绕过Entity转化
        /// </summary>
        /// <param name="commandText">SQL的文本内容</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        DataTable ExcuteSQLQuery(string commandText, params object[] parameters);

        IList<TEntity> ExcuteSQLQueryList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new();

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters);

        void ExecuteStoredProcedure(string commandText, params object[] parameters);

        IList<ResultEntity> ExecuteStoredProcedure<ResultEntity>(Func<DbDataReader, ResultEntity> objConvert, string commandText, params object[] parameters) where ResultEntity : class;
    }
    public interface IUnitOfWork : IDisposable
    {
        IDbContext Context { get; }
        DbContextTransaction Transacton { get; }
        void BeginTransaction();
        void Flush();
        void Commit();
        void RollBack();
    }
}