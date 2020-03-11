using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.DLL;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class UserInfoSingleton
    {
        private  UserInfo user = null;
        private static UserInfoSingleton instance = null;

        public  UserInfo UserInfo
        {
            get { return user; }
        }
        private static object syncLocker = new object();
        private UserInfoSingleton()
        {
            string userName = Environment.UserName;//获取电脑用户名
            user = new UserInfoDll().GetEntity(userName);
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
    }
}
