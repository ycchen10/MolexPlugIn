using System;
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
    /// 等高
    /// </summary>
    public class ZLevelMillingCreateOperation : AbstractCreateOperation
    {
        private List<Face> faces = new List<Face>();
        private double level = 0;
        public ZLevelMillingCreateOperation(int site, string tool) : base(site, tool)
        {

        }
        public override void CreateOperation(ElectrodeCAM eleCam, double inter)
        {
            this.Oper = ElectrodeOperationTemplate.CreateOperationOfZLevelMilling(this.NameModel, eleCam);
            if (faces.Count > 0)
                (this.Oper as ZLevelMillingModel).SetGeometry(faces.ToArray());
            if (level != 0)
                (this.Oper as ZLevelMillingModel).SetCutLevel(level);
            this.Oper.SetStock(-inter, -inter);
        }
        /// <summary>
        /// 设置边界
        /// </summary>
        /// <param name="floorPt"></param>
        /// <param name="conditions"></param>
        public void SetFaces(params Face[] faces)
        {
            this.faces = faces.ToList();
        }
        /// <summary>
        /// 设置切削层底
        /// </summary>
        /// <param name="level"></param>
        public void SetCutLevel(double level)
        {
            this.level = level;
        }
        public override void CreateOperationName()
        {
            string program = "O000" + this.Site.ToString();
            this.NameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfZLevelMilling(program, this.ToolName);
        }
    }
}
