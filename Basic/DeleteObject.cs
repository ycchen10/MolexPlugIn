using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    /// <summary>
    /// 删除
    /// </summary>
    public class DeleteObject : ClassItem
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="nxObject"></param>
        /// <returns></returns>
        public static bool Delete(params NXObject[] nxObject)
        {
            Session.UndoMarkId mark = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Delete");
            int dt = theSession.UpdateManager.AddToDeleteList(nxObject);
            try
            {
                int nErrs = theSession.UpdateManager.DoUpdate(mark);
                return dt != 0;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DeleteObject.UpdateObject: 删除失败！" + ex.Message);
                return false;
            }
            finally
            {
                theSession.DeleteUndoMark(mark, null);
            }


        }
        /// <summary>
        ///更新
        /// </summary>
        public static void UpdateObject()
        {
            Part workPart = theSession.Parts.Work;
            theSession.Preferences.Modeling.UpdatePending = false;
            NXOpen.Session.UndoMarkId markId;
            markId = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "NX update");
            try
            {
                theSession.UpdateManager.DoUpdate(markId);
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DeleteObject.UpdateObject:更新失败！" + ex.Message);
            }
            finally
            {
                theSession.DeleteUndoMark(markId, "NX update");
            }

        }

        public static void DeleteParms(params NXObject[] obj)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Features.RemoveParametersBuilder removeParametersBuilder1;
            removeParametersBuilder1 = workPart.Features.CreateRemoveParametersBuilder();
            bool added1 = removeParametersBuilder1.Objects.Add(obj);
            try
            {
                NXObject nXObject1;
                nXObject1 = removeParametersBuilder1.Commit();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DeleteObject.DeleteParms:删除参数失败！" + ex.Message);
            }
            finally
            {
                removeParametersBuilder1.Destroy();
            }

        }
    }
}
