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
   public class ControlEnumNameDll:OperationData<ControlEnum>
    {
        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override int Delete(List<ControlEnum> model)
        {
            int count = 0;
            try
            {
                using (Conn)
                {

                    count = Conn.Execute("delete from ControlEnum where ControlEnumId = @ControlEnumId", model);
                }
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ControlEnumNameDll.Delete." + "删除数据:" + ex.Message);
            }
            return count;
        }

        public override int Delete(int id)
        {
            int count = 0;
            try
            {
                using (Conn)
                {

                    count = Conn.Execute("delete from ControlEnum where ControlEnumId = @ControlEnumId", new { ControlEnumId = id });
                }
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ControlEnumNameDll.Delete." + "删除数据:" + ex.Message);
            }
            return count;
        }

        public override int Delete(ControlEnum model)
        {
            int count = 0;
            try
            {
                using (Conn)
                {

                    count = Conn.Execute("delete from ControlEnum where ControlEnumId = @ControlEnumId", new { ControlEnumId = model.ControlEnumId });
                }
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ControlEnumNameDll.Delete." + "删除数据:" + ex.Message);
            }
            return count;
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 以主键获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ControlEnum GetEntity(int id)
        {
            string query = @"select control.ControlType,controlEnum.ControlEnumId,controlEnum.EnumName" +
                           "from control inner join controlEnum on control.ControlId=controlEnum.ControlId where ControlEnumId = @ControlEnumId";
            try
            {
                using (Conn)
                {

                    return Conn.Query<ControlEnum>(query, new { ControlEnumId = id }).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ControlEnumNameDll.GetEntity." + id.ToString() + ":获取数据:" + ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <returns></returns>
        public override List<ControlEnum> GetList()
        {
            string query = @"select control.ControlType,controlEnum.ControlEnumId,controlEnum.EnumName" +
                          " from control inner join controlEnum on control.ControlId=controlEnum.ControlId ";
            try
            {
                using (Conn)
                {

                    return Conn.Query<ControlEnum>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ControlEnumNameDll.GetList." + "获取数据:" + ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 以控件名获取实体
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<ControlEnum> GetListOfControlName(string name)
        {
            string query = @"select control.ControlType,controlEnum.ControlEnumId,controlEnum.EnumName" +
                          "from control inner join controlEnum on control.ControlId=controlEnum.ControlId where ControlEnumId = @ControlEnumId";
            try
            {
                using (Conn)
                {
                    int controlId = Conn.Query<int>(@"select * from Control where ControlName=@ControlName", new { ControlName = name }).SingleOrDefault();
                    return Conn.Query<ControlEnum>(query, new { ControlId = controlId }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ControlEnumNameDll.GetListOfControlName." + "获取数据:" + ex.Message);
            }
            return null;
        }
        #endregion

        #region 插入数据
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override int Insert(List<ControlEnum> model)
        {
            string query = "insert into ControlEnum(ControlId,EnumName) Values(@ControlId,@EnumName)";
            try
            {
                using (Conn)
                {
                    return Conn.Execute(query, model);
                }

            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ControlEnumNameDll.Insert." + "插入数据:" + ex.Message);
            }
            return 0;
        }

        public override int Insert(ControlEnum model)
        {
            string query = "insert into ControlEnum(ControlId,EnumName) Values(@ControlId,@EnumName)";
            try
            {
                using (Conn)
                {
                    return Conn.Execute(query, model);
                }

            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ControlEnumNameDll.Insert." + model.EnumName + "插入数据:" + ex.Message);
            }
            return 0;
        }
        #endregion
        /// <summary>
        /// 跟新数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override int Update(ControlEnum model)
        {
            string query = "update Control set ControlId=@ControlId,EnumName=@EnumName ";
            try
            {
                using (Conn)
                {
                    return Conn.Execute(query, model);
                }

            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("ControlEnumNameDll.Update." + model.EnumName + "更新数据:" + ex.Message);
            }
            return 0;
        }
    }
}
