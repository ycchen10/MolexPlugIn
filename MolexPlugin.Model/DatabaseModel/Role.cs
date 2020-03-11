using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role
    {
        /// <summary>
        /// 角色id
        /// </summary>     
        public int Id { get; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; }

        public Role()
        {

        }
        public Role(int id, string roleName)
        {
            this.Id = id;
            this.RoleName = roleName;
        }

    }
}
