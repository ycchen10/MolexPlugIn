using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;

namespace Basic
{
    public class PartUtils : ClassItem
    {
        /// <summary>
        /// 新建部件
        /// </summary>
        /// <param name="partName">部件全路径名</param>
        /// <returns>NXObject</returns>
        public static NXObject NewFile(string partName)
        {
            FileNew fileNew1 = theSession.Parts.FileNew();
            fileNew1.TemplateFileName = "molex-plain-1-mm-template.prt";
            fileNew1.UseBlankTemplate = false;
            fileNew1.ApplicationName = "ModelTemplate";
            fileNew1.Units = Part.Units.Millimeters;
            fileNew1.RelationType = "";
            fileNew1.UsesMasterModel = "No";
            fileNew1.TemplateType = FileNewTemplateType.Item;
            fileNew1.TemplatePresentationName = "Molex";
            fileNew1.ItemType = "";
            fileNew1.Specialization = "";
            fileNew1.SetCanCreateAltrep(false);
            fileNew1.NewFileName = partName;
            fileNew1.MasterFileName = "";
            fileNew1.MakeDisplayedPart = true;
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = fileNew1.Commit();
                return nXObject1;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("PartUtils.NewFile:" + ex.Message);
                return null;
            }
            finally
            {
                fileNew1.Destroy();
            }

        }
        /// <summary>
        /// 设置显示部件
        /// </summary>
        /// <param name="part"></param>
        public static void SetPartDisplay(Part part)
        {
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            NXOpen.PartLoadStatus partLoadStatus1;
            NXOpen.PartCollection.SdpsStatus status1;
            status1 = theSession.Parts.SetDisplay(part as Part, false, true, out partLoadStatus1);
            workPart = theSession.Parts.Work;
            displayPart = theSession.Parts.Display;
        }

        /// <summary>
        /// 设置显示部件
        /// </summary>
        /// <param name="part"></param>
        public static void SetPartWork(NXOpen.Assemblies.Component comp)
        {
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            NXOpen.PartLoadStatus partLoadStatus1 = null;
            try
            {
                theSession.Parts.SetWorkComponent(comp, NXOpen.PartCollection.RefsetOption.Entire, NXOpen.PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("PartUtils.SetPartWork" + ex.Message);
            }
            finally
            {
                workPart = theSession.Parts.Work;
                if (partLoadStatus1 != null)
                    partLoadStatus1.Dispose();
            }

        }

        /// <summary>
        /// 打开 部件
        /// </summary>
        /// <param name="path">地址</param>
        /// <returns></returns>
        public static Part OpenPartFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.BasePart basePart1;
            NXOpen.PartLoadStatus partLoadStatus1;
            basePart1 = theSession.Parts.OpenBaseDisplay(path, out partLoadStatus1);
            partLoadStatus1.Dispose();
            return basePart1 as Part;
        }
    }
}
