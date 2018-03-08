using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Farm.Repositories
{
    public abstract class GenericRepository<T> : IDependency, IRepository<T> where T : class, new()
    {
        #region Implementation of IRepository<T>

        /// <summary>
        /// 根据主值查询单条数据
        /// </summary>
        /// <param name="pkValue">主键值</param>
        /// <returns>泛型实体</returns>
        public T FindById(object pkValue)
        {
            var entity = SugarBase.DB.Queryable<T>().InSingle(pkValue);
            return entity;
        }

        /// <summary>
        /// 查询所有数据(无分页,请慎用)
        /// </summary>
        /// <returns></returns>
        public List<T> FindAll()
        {
            var list = SugarBase.DB.Queryable<T>().ToList();
            return list;
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序</param>
        /// <returns>泛型实体集合</returns>
        public List<T> FindListByClause(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy=null, int orderByType=0)
        {
            var query = SugarBase.DB.Queryable<T>().Where(predicate);
            if (orderBy!=null)
            {
                query = query.OrderBy(orderBy,(SqlSugar.OrderByType)orderByType);
            }
            var entities = query.ToList();
            return entities;
        }

        /// <summary>
        /// 根据条件查询数据(带分页)
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序</param>
        /// <returns>泛型实体集合</returns>
        public List<T> FindListByClauseAndPage(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, ref int totalCount, Expression<Func<T, object>> orderBy = null, int orderByType = 0)
        {
            var query = SugarBase.DB.Queryable<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderBy != null)
            {
                query = query.OrderBy(orderBy, (SqlSugar.OrderByType)orderByType);
            }
            var sql = query.ToSql();
            var entities = query.ToPageList(pageIndex,pageSize,ref totalCount);
            return entities;
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public T FindByClause(Expression<Func<T, bool>> predicate)
        {
            var entity = SugarBase.DB.Queryable<T>().First(predicate);
            return entity;
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public long Insert(T entity)
        {
            //返回插入数据的标识字段值
            var i = SugarBase.DB.Insertable(entity).ExecuteReturnBigIdentity();
            return i;
        }

        /// <summary>
        /// 全量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            //这种方式会以主键为条件
            var i = SugarBase.DB.Updateable(entity).ExecuteCommand();
            return i > 0;
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(dynamic updateDynamicObject)
        {
            //这种方式会以主键为条件
            var i = SugarBase.DB.Updateable<T>(updateDynamicObject).ExecuteCommand();
            return i > 0;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            var i = SugarBase.DB.Deleteable(entity).ExecuteCommand();
            return i > 0;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where">过滤条件</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> @where)
        {
            var i = SugarBase.DB.Deleteable<T>(@where).ExecuteCommand();
            return i > 0;
        }

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById(object id)
        {
            var i = SugarBase.DB.Deleteable<T>(id).ExecuteCommand();
            return i > 0;
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteByIds<K>(K[] ids)
        {
            var i = SugarBase.DB.Deleteable<T>().In(ids).ExecuteCommand();
            return i > 0;
        }
        #endregion
    }
}
