using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 后处理类
    /// </summary>
    public class PartPost
    {
        public Part PostPart { get; private set; }

        public string Path { get; private set; }
        public PartPost(Part part)
        {
            this.PostPart = part;
            string partPart = part.FullPath;
            Path = System.IO.Path.GetDirectoryName(partPart) + "\\";
        }
        /// <summary>
        /// 后处理
        /// </summary>
        /// <param name="postName"></param>
        /// <param name="partPath"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        private bool Post(string postName, string post, params NXOpen.CAM.NCGroup[] groups)
        {
            Part workPart = Session.GetSession().Parts.Work;
            try
            {
                foreach (NCGroup np in groups)
                {
                    workPart.CAMSetup.PostprocessWithSetting(new CAMObject[] { np }, postName, post, NXOpen.CAM.CAMSetup.OutputUnits.PostDefined,
                           NXOpen.CAM.CAMSetup.PostprocessSettingsOutputWarning.PostDefined,
                           NXOpen.CAM.CAMSetup.PostprocessSettingsReviewTool.PostDefined);
                }
                return true;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("后处理错误" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取程序组最下半径
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public void GetGroupTool(List<NCGroup> groups, out double toolMinNumber, out double diaMin)
        {
            diaMin = 99999;
            toolMinNumber = 9999;
            foreach (NCGroup np in groups)
            {
                CAMObject obj = np.GetMembers()[0];
                NCGroup tool = (obj as NXOpen.CAM.Operation).GetParent(CAMSetup.View.MachineTool);
                ToolDataModel model = new ToolDataModel(tool);
                if (diaMin > model.ToolDia)
                    diaMin = model.ToolDia;
                if (toolMinNumber > model.ToolNumber)
                    toolMinNumber = model.ToolNumber;
            }

        }

        /// <summary>
        /// 获取电极后处理格式
        /// </summary>
        /// <returns></returns>
        public string[] GetElectrodePostName(List<NCGroup> groups)
        {
            List<string> postName = new List<string>();
            double diaMin, toolMinNumber;
            GetGroupTool(groups, out toolMinNumber, out diaMin);
            if (diaMin > 0.8 && toolMinNumber > 0)
            {
                postName.AddRange(new string[] { "HSM500", "DAKIN", "Mikron_Molex" });
            }
            if (diaMin > 0.8 && toolMinNumber == 0)
            {
                postName.AddRange(new string[] { "HSM500", "DAKIN", });
            }
            if (diaMin <= 0.8 && toolMinNumber > 0)
            {
                postName.AddRange(new string[] { "HSM500", "Mikron_Molex" });
            }
            if (diaMin <= 0.8 && toolMinNumber == 0)
            {
                postName.AddRange(new string[] { "HSM500" });
            }
            return postName.ToArray();
        }

        private bool PostHSM500(List<NCGroup> gps)
        {
            string path = Path + PostPart.Name + ".H";
            if (File.Exists(path))
                File.Delete(path);
            bool isok = Post("HSM500", path, gps.ToArray());
            return isok;
        }
        private bool PostDAKIN(List<NCGroup> gps)
        {
            string path = Path + PostPart.Name;
            if (File.Exists(path))
                File.Delete(path);
            bool isok = Post("DAKIN", path, gps.ToArray());

            return isok;
        }

        private bool PostMikron_Molex(NCGroup parent)
        {
            string path = Path + PostPart.Name + ".H";
            if (File.Exists(path))
                File.Delete(path);
            bool isok = Post("Mikron_Molex", path, parent);

            return isok;
        }
        private bool PostMAKINO(List<NCGroup> gps)
        {
            string path = Path + PostPart.Name;
            if (File.Exists(path))
                File.Delete(path);
            bool isok = Post("MAKINO", path, gps.ToArray());

            return isok;
        }

        public bool Post(string postName, params NCGroup[] gps)
        {

            bool isok = false;
            switch (postName)
            {
                case "DAKIN":
                    isok = PostDAKIN(gps.ToList());
                    break;
                case "HSM500":
                    isok = PostHSM500(gps.ToList());
                    break;
                case "Mikron_Molex":
                    isok = PostMikron_Molex(gps[0].GetParent());
                    break;
                case "MAKINO":
                    isok = PostMAKINO(gps.ToList());
                    break;
                default:
                    break;
            }
            return isok;
        }
        public List<NCGroup> GetGroup()
        {
            List<NCGroup> groups = new List<NCGroup>();
            NCGroup part = (NXOpen.CAM.NCGroup)PostPart.CAMSetup.CAMGroupCollection.FindObject("AAA");
            foreach (NCGroup np in part.GetMembers())
            {
                if (np.GetMembers().Length > 0)
                {
                    groups.Add(np);                 
                }
            }
            return groups;
        }
    }
}
