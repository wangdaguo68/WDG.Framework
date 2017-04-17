using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using EntityState = System.Data.Entity.EntityState;

namespace WDG.Framework.DAL
{
    public class DbContextBase : DbContext, IDisposable
    {
        public DbContextBase(string connectionString)
        {
            base.Database.Connection.ConnectionString = connectionString;
            base.Configuration.LazyLoadingEnabled = false;
            base.Configuration.ProxyCreationEnabled = false;
        }

        public void Delete<T>(T entity) where T : class
        {
            base.Entry<T>(entity).State = EntityState.Deleted;
        }

        /// <summary>
        ///     Time:2015.11.11
        ///     Author:王达国
        ///     泛型根据实体列表去删除
        /// </summary>
        /// <param name="db"></param>
        /// <param name="modelEntity"></param>
        public  void Delete<T>(T[] modelEntity) where T:class
        {
            foreach (var model in modelEntity)
            {
                base.Entry(model).State = EntityState.Deleted;
            }
        }

        public T Find<T>(params object[] keyValues) where T : class
        {
            return this.Set<T>().Find(keyValues);
        }

        public List<T> FindAll<T>(Expression<Func<T, bool>> conditions = null) where T : class
        {
            if (conditions == null)
            {
                return this.Set<T>().ToList<T>();
            }
            return this.Set<T>().Where<T>(conditions).ToList<T>();
        }


        public void Insert<T>(params T[] models) where T : class
        {
            foreach (var model in models)
            {
                this.Set<T>().Add(model);
            }
        }
        public T Insert<T>(T entity) where T : class
        {
            this.Set<T>().Add(entity);
            return entity;
        }

        public T Update<T>(T entity) where T : class
        {
            this.Set<T>().Attach(entity);
            base.Entry<T>(entity).State = EntityState.Modified;
            return entity;
        }

        /// <summary>
        ///     Content:泛型 +反射 实现动态Update
        ///     Time:2015.11.11
        ///     Author:王达国
        /// </summary>
        /// <param name="db"></param>
        /// <param name="modelEntity"></param>
        public  void Update<T>(params T[] modelEntity) where T: class
        {
            foreach (var model in modelEntity)
            {
                this.Set<T>().Attach(model);
                base.Entry<T>(model).State = EntityState.Modified;
            }
        }

        /// <summary>
        ///     Time:2015.11.11
        ///     Author:王达国
        ///     执行存储过程返回DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parms"></param>
        /// <param name="procName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public  DataTable ExecProcToDataTable<T>( SqlParameter[] parms, string procName)
        {
            var sql = procName + " ";
            for (var i = 0; i < parms.Length; i++)
            {
                if (i == 0)
                {
                    sql += parms[i].ParameterName;
                }
                else
                {
                    sql += ", " + parms[i].ParameterName;
                }
            }
            var result = Database.SqlQuery<T>(sql, parms).ToList();
            var dt = ListToDataTable<T>(result);
            return dt;
        }

        /// <summary>
        ///     Time:2015.11.11
        ///     Author:王达国
        ///     执行存储过程返回Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parms"></param>
        /// <param name="procName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public  T ExecProcToModel<T>( SqlParameter[] parms, string procName)
        {
            var sql = procName + " ";
            for (var i = 0; i < parms.Length; i++)
            {
                if (i == 0)
                {
                    sql += parms[i].ParameterName;
                }
                else
                {
                    sql += ", " + parms[i].ParameterName;
                }
            }
            return Database.SqlQuery<T>(sql, parms).ToList().FirstOrDefault();
        }

        /// <summary>
        ///     Time:2015.11.11
        ///     Author:王达国
        ///     执行存储过程返回Model集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parms"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        public  List<T> ExecProcToModelList<T>(SqlParameter[] parms, string procName)
        {
            var sql = procName + " ";
            for (var i = 0; i < parms.Length; i++)
            {
                if (i == 0)
                {
                    sql += parms[i].ParameterName;
                }
                else
                {
                    sql += ", " + parms[i].ParameterName;
                }
            }
            return Database.SqlQuery<T>(sql, parms).ToList();
        }

        /// <summary>
        ///     Author:王达国
        ///     Time:2015.10.28
        ///     Content:将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public  DataTable ListToDataTable<T>(IList list)
        {
            var result = new DataTable(typeof(T).Name);
            result = CreateData<T>();
            if (list.Count > 0)
            {
                var propertys = list[0].GetType().GetProperties();
                for (var i = 0; i < list.Count; i++)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                    {
                        var obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>
        ///     Time:2015.11.11
        ///     Author:王达国
        ///     根据实体类得到表结构
        /// </summary>
        /// <returns></returns>
        private static DataTable CreateData<T>()
        {
            var dataTable = new DataTable(typeof(T).Name);
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                dataTable.Columns.Add(new DataColumn(propertyInfo.Name));
            }
            return dataTable;
        }

        public List<T> GetSearchList<T>(Expression<Func<T, bool>> where) where T : class
        {
            var item = this.Set<T>().Where(where).ToList();
            return item;
        }

        /// <summary>
        ///     Content:根据条件查询动态排序且分页，字段和排序类型均可变
        ///     Time:2016.03.03
        ///     Author:王达国
        /// </summary>
        /// <param name="db">数据库db</param>
        /// <param name="where"></param>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderByExpression">排序字段和排序类型实体</param>
        /// <returns></returns>
        public  IList DynamicOrder<T>( Expression<Func<T, bool>> where, int pageIndex, int pageSize,
            params OrderModelField[] orderByExpression) where T : class
        {
            var query = base.Set<T>().Where(where);
            //创建表达式变量参数
            var parameter = Expression.Parameter(typeof(T), "o");
            if (orderByExpression == null || orderByExpression.Length <= 0)
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            foreach (var t in orderByExpression)
            {
                //根据属性名获取属性
                PropertyInfo property;
                if (t.PropertyName == null || t.OrderType == null)
                {
                    property = typeof(T).GetProperties()[0];
                }
                else
                {
                    property = typeof(T).GetProperty(t.PropertyName);
                }
                //创建一个访问属性的表达式
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var orderName = t.OrderType == "desc" ? "OrderByDescending" : "OrderBy";
                var resultExp = Expression.Call(typeof(Queryable), orderName,
                    new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
                query = query.Provider.CreateQuery<T>(resultExp);
            }
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        ///     Content:根据结果集动态排序且分页，字段和排序类型均可变
        ///     Time:2016.03.03
        ///     Author:王达国
        /// </summary>
        /// <param name="query">结果集</param>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">页码</param>
        /// <param name="orderByExpression">排序字段和排序类型实体</param>
        /// <returns></returns>
        public static IList DynamicOrder<T>(IQueryable<T> query, int pageIndex, int pageSize,
            params OrderModelField[] orderByExpression)
        {
            //创建表达式变量参数
            var parameter = Expression.Parameter(typeof(T), "o");
            if (orderByExpression == null || orderByExpression.Length <= 0)
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            foreach (var t in orderByExpression)
            {
                //根据属性名获取属性
                PropertyInfo property;
                if (t.PropertyName == null || t.OrderType == null)
                {
                    property = typeof(T).GetProperties()[0];
                }
                else
                {
                    property = typeof(T).GetProperty(t.PropertyName);
                }
                //创建一个访问属性的表达式
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var orderName = t.OrderType == "desc" ? "OrderByDescending" : "OrderBy";
                var resultExp = Expression.Call(typeof(Queryable), orderName,
                    new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
                query = query.Provider.CreateQuery<T>(resultExp);
            }
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        ///     Time:2015.11.11
        ///     Author:王达国
        ///     执行sql脚本
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public  int ExecSqlNoQuery(string sql)
        {
            return Database.ExecuteSqlCommand(sql);
        }

        /// <summary>
        ///     Time:2015.11.11
        ///     Author:王达国
        ///     执行sql脚本并查询返回List
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public  List<T> ExecSqlQuery<T>( string sql)
        {
            return Database.SqlQuery(typeof(T), sql).Cast<T>().ToList();
        }

        /// <summary>
        ///     Time:2015.11.11
        ///     Author:王达国
        ///     将查询结果单个实体以Json字符串形式返回
        /// </summary>
        /// <param name="where"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public  string GetSearchToJson<T>(Expression<Func<T, bool>> where) where T:class
        {
            var obj = base.Set<T>().Where(where).FirstOrDefault();
            var jsSer = new JavaScriptSerializer();
            return jsSer.Serialize(obj);
        }

        /// <summary>
        /// </summary>
        public struct OrderModelField
        {
            /// <summary>
            ///     属性名称
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            ///     是否降序
            /// </summary>
            public string OrderType { get; set; }
        }
    }
}
