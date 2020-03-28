using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 用户
    /// </summary>
    [Serializable]
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>    
        public int UserId { get; set; }
        /// <summary>
        /// 用户工号
        /// </summary> 
        public string UserJob { get; set; }
        /// <summary>
        /// 电脑账号
        /// </summary>  
        public string UserAccount { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>    
        public string UserName { get; set; }
        /// <summary>
        /// 分机号
        /// </summary>    
        public int UserExt { get; set; }
        /// <summary>
        /// 用户权限
        /// </summary>
        public List<Role> Role { get; set; } = new List<Role>();
        /// <summary>
        /// 最后使用时间
        /// </summary>
        public double UserTime { get; set; }

        public UserInfo()
        {

        }
        public UserInfo(string job, string computer, string name, int ext, double time, params Role[] role)
        {
            this.UserJob = job;
            this.UserAccount = computer;
            this.UserName = name;
            this.UserExt = ext;
            this.UserTime = time;
            this.Role.AddRange(role);
        }

    }
}
