using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace MolexPlugin.DLL
{
    /// <summary>
    /// 数据库连接类
    /// </summary>
    class ConnectionFactory
    {

        //private static readonly string connectionName = ConfigurationManager.AppSettings["ConnectionName"];
        //private static readonly string conStr = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
        private static readonly string connectionName = "Access";
        private static readonly string conStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source =C:\Users\ycchen10\OneDrive - kochind.com\Desktop\MolexPlugIn-12.0\Cofigure\MolexPlup-2020.1.7.accdb;Persist Security Info=True;Jet OLEDB:Database Password = chyuch@0802; User Id = admin";
        public static IDbConnection CreateConnection()
        {
            IDbConnection conn = null;
            switch (connectionName)
            {
                case "SQLServer":
                    conn = new SqlConnection(conStr);
                    break;
                case "Access":
                    conn = new OleDbConnection(conStr);
                    break;
                default:
                    conn = new SqlConnection(conStr);
                    break;
            }
            return conn;
        }
    }
}
