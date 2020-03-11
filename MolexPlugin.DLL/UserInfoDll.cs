using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolexPlugin.Model;
using Basic;
using Dapper;

namespace MolexPlugin.DLL
{
   public class UserInfoDll:OperationData<UserInfo>
    {
        #region 删除
        public override int Delete(List<UserInfo> model)
        {
            int count = 0;
            try
            {
                using (Conn)
                {
                    foreach (UserInfo ui in model)
                    {
                        count += Conn.Execute("delete from userRole where UserId = @UserId", new { UserId = ui.UserId });
                    }
                    count += Conn.Execute("delete from userInfo where UserId = @UserId", model);
                }
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("UserInfoDll.Delete." + "删除数据:" + ex.Message);
            }
            return count;
        }

        public override int Delete(int id)
        {
            int cout = 0;
            try
            {
                using (Conn)
                {
                    cout = Conn.Execute("delete from userRole where UserId = @UserId", new { UserId = id });
                    cout += Conn.Execute("delete from userInfo where UserId = @UserId", new { UserId = id });
                }
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("UserInfoDll.Delete." + id.ToString() + "删除数据:" + ex.Message);
            }

            return cout;
        }

        public override int Delete(UserInfo model)
        {         
            return Delete(model.UserId);
        }
        #endregion
        #region 获取实体
        public override UserInfo GetEntity(int id)
        {
            string query = @"select * from userinfo where UserId = @Userid";
            string roleStr = @"select role.RoleId,role.RoleName from role inner join UserRole on UserRole.RoleId=Role.RoleId where UserRole.UserId=@UserId";
            UserInfo info = new UserInfo();
            List<Role> role = new List<Role>();
            try
            {
                using (Conn)
                {
                    info = Conn.Query<UserInfo>(query, new { UserId = id }).SingleOrDefault();
                    role = Conn.Query<Role>(roleStr, new { UserId = id }).ToList();
                    info.Role = role;
                }
                return info;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("UserInfoDll.GetEntity." + id.ToString() + "读取数据错误:" + ex.Message);
            }
            return null;
        }
        public UserInfo GetEntity(string userAccount)
        {
            string query = @"select * from userinfo where UserAccount = @UserAccount";
            string roleStr = @"select role.RoleId,role.RoleName from role inner join UserRole on UserRole.RoleId=Role.RoleId where UserRole.UserId=@UserId";
            UserInfo info = new UserInfo();
            List<Role> role = new List<Role>();
            try
            {
                using (Conn)
                {
                    info = Conn.Query<UserInfo>(query, new { UserAccount = userAccount }).SingleOrDefault();
                    role = Conn.Query<Role>(roleStr, new { UserId = info.UserId }).ToList();
                    info.Role = role;
                }
                return info;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("UserInfoDll.GetEntity." + userAccount + "读取数据错误:" + ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <returns></returns>
        public override List<UserInfo> GetList()
        {
            string query = @"select * from userinfo";
            string roleStr = @"select role.RoleId,role.RoleName from role inner join UserRole on UserRole.RoleId=Role.RoleId where UserRole.UserId=@UserId";
            List<UserInfo> info = new List<UserInfo>();
            List<Role> role = new List<Role>();
            try
            {
                using (Conn)
                {
                    info = Conn.Query<UserInfo>(query).ToList();
                    for (int i = 0; i < info.Count; i++)
                    {
                        role = Conn.Query<Role>(roleStr, new { UserId = info[i].UserId }).ToList();
                        info[i].Role = role;
                    }
                }
                return info;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("UserInfoDll.GetList." + "读取数据错误:" + ex.Message);
            }
            return null;
        }
        #endregion
        #region 插入数据
        public override int Insert(List<UserInfo> model)
        {
            int count = 0;
            foreach (UserInfo ui in model)
            {
                count += Insert(ui);
            }
            return count;
        }

        public override int Insert(UserInfo model)
        {
            int count = 0;
            string query1 = "insert into UserInfo(UserJob,UserAccount,UserName,UserExt,UserTime)VALUES(@userJob,@userAccount,@userName,@userExt,@userTime)";
            DynamicParameters para = new DynamicParameters();
            para.Add("UserJob", model.UserJob);
            para.Add("UserAccount", model.UserAccount);
            para.Add("UserName", model.UserName);
            para.Add("UserExt", model.UserExt);
            para.Add("UserTime", model.UserTime);
            try
            {
                using (Conn)
                {
                    count = Conn.Execute(query1, para);
                    UserInfo info = GetEntity(model.UserAccount);
                    string query2 = "insert into UserRole(UserId,RoleId)values(@userId,@roleId)";
                    DynamicParameters[] para2 = new DynamicParameters[model.Role.Count];
                    for (int i = 0; i < model.Role.Count; i++)
                    {
                        para2[i] = new DynamicParameters();
                        para2[i].Add("UserId", info.UserId);
                        para2[i].Add("RoleId", model.Role[i].Id);
                    }

                    count += Conn.Execute(query2, para2);
                }


            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("UserInfoDll.Insert." + model.UserName + "插入数据错误:" + ex.Message);
            }
            return count;

        }
        #endregion

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override int Update(UserInfo model)
        {
            string query = "update userInfo set UserId=@userId,UserJob=@userJob,UserAccount=@userAccount,UserName=@userName,UserExt=@userExt,UserTime=@userTime";
            DynamicParameters para = new DynamicParameters();
            para.Add("UserJob", model.UserJob);
            para.Add("UserAccount", model.UserAccount);
            para.Add("UserName", model.UserName);
            para.Add("UserExt", model.UserExt);
            para.Add("UserTime", model.UserTime);
            int count = 0;
            try
            {
                count = Conn.Execute(query, para);
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("UserInfoDll.Update." + model.UserName + "插入数据错误:" + ex.Message);
            }
            return count;
        }
    }
}
