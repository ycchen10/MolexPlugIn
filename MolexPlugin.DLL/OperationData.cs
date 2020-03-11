using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.DLL
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OperationData<T> where T : class
    {
        private IDbConnection _conn;
        public IDbConnection Conn
        {
            get { return _conn = ConnectionFactory.CreateConnection(); }
        }

        public abstract int Insert(T model);
        public abstract int Insert(List<T> model);

        abstract public int Update(T model);

        abstract public int Delete(T model);

        abstract public int Delete(int id);

        abstract public int Delete(List<T> model);

        abstract public List<T> GetList();

        abstract public T GetEntity(int id);



    }
}
