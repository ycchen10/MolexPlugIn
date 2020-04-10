using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.DLL;
using MolexPlugin.Model;
using System.Runtime.Serialization.Formatters.Binary;

namespace MolexPlugin.DAL
{
    public class UserInfoSingleton
    {
        private UserInfo user = null;
        private static UserInfoSingleton instance = null;

        public UserInfo UserInfo
        {
            get { return user; }
        }
        private static object syncLocker = new object();
        private UserInfoSingleton()
        {
            string userAccount = Environment.UserName;//获取电脑用户名
                                                      //  user = new UserInfoDll().GetEntity(userAccount);
            List<UserInfo> users = Deserialize();
            user = users.Find(a => a.UserAccount == userAccount);
        }
        public static UserInfoSingleton GetInstance()
        {
            if (instance == null)
            {
                lock (syncLocker)
                {
                    if (instance == null)
                        instance = new UserInfoSingleton();
                }
            }
            return instance;
        }
        /// <summary>
        /// 序列化
        /// </summary>
        public static void Serialize()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string userPath = dllPath.Replace("application\\", "Cofigure\\SerializeUser.dat");
            if (File.Exists(userPath))
                File.Delete(userPath);
            List<UserInfo> users = new UserInfoDll().GetList();
            FileStream fs = new FileStream(userPath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, users);
            fs.Close();
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> Deserialize()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string userPath = dllPath.Replace("application\\", "Cofigure\\SerializeUser.dat");
            if (File.Exists(userPath))
            {
                FileStream fs = new FileStream(userPath, FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(fs) as List<UserInfo>;
            }
            return null;
        }
    }
}
