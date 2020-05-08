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
        public List<Face> Faces { get; private set; } = new List<Face>();
        private Face levelFace =null;
        private bool steep;
        public ZLevelMillingCreateOperation(int site, string tool) : base(site, tool)
        {

        }
        public override void CreateOperation(ElectrodeCAM eleCam, double inter)
        {
            this.Oper = ElectrodeOperationTemplate.CreateOperationOfZLevelMilling(this.NameModel, eleCam);
            this.Oper.Create(this.NameModel.OperName);
            if (Faces.Count > 0)
                (this.Oper as ZLevelMillingModel).SetGeometry(Faces.ToArray());
            if (levelFace != null)
                (this.Oper as ZLevelMillingModel).SetCutLevel(levelFace);
            if(steep)
                (this.Oper as ZLevelMillingModel).SetSteep();
            this.Oper.SetStock(-inter, -inter);
        }
        /// <summary>
        /// 设置边界
        /// </summary>
        /// <param name="floorPt"></param>
        /// <param name="conditions"></param>
        public void SetFaces(params Face[] faces)
        {
            this.Faces = faces.ToList();
        }
        /// <summary>
        /// 设置切削层底
        /// </summary>
        /// <param name="level"></param>
        public void SetCutLevel(Face level)
        {
            this.levelFace = level;
        }
        public override void CreateOperationName()
        {
            string program = "O000" + this.Site.ToString();
            this.NameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfZLevelMilling(program, this.ToolName);
        }

        public override AbstractCreateOperation CopyOperation()
        {
            ZLevelMillingCreateOperation zo = new ZLevelMillingCreateOperation(this.Site, this.ToolName);
            zo.CreateOperationName();
            // zo.SetFaces(this.faces.ToArray());
            //  zo.SetCutLevel(this.level);
            return zo;
        }
        /// <summary>
        /// 设置陡峭角
        /// </summary>
        /// <param name="steep"></param>
        public void SetSteep(bool steep)
        {
            this.steep = steep;
        }
    }
}
