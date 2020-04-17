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
    /// 清根
    /// </summary>
    public class FlowCutCreateOperation : AbstractCreateOperation
    {
        private List<Face> faces = new List<Face>();

        private string referencetoolName = "";
        public FlowCutCreateOperation(int site, string tool) : base(site, tool)
        {

        }
        public override void CreateOperation(ElectrodeCAM eleCam, double inter)
        {
            this.Oper = ElectrodeOperationTemplate.CreateOperationOfSurfaceContour(this.NameModel, eleCam);
            this.Oper.Create(this.NameModel.OperName);
            if (faces.Count > 0)
                (this.Oper as SurfaceContourModel).SetGeometry(faces.ToArray());
            if (referencetoolName != "")
            {

                (this.Oper as SurfaceContourModel).SetReferenceTool(eleCam.FindTool(referencetoolName) as Tool);
            }
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
        /// 设置参考刀具
        /// </summary>
        /// <param name="toolName"></param>
        public void SetReferencetool(string toolName)
        {
            this.referencetoolName = toolName;
        }
        public override void CreateOperationName()
        {
            string program = "O000" + this.Site.ToString();
            this.NameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfSurfaceContour(program, this.ToolName);
        }

        public override AbstractCreateOperation CopyOperation()
        {
            FlowCutCreateOperation fo = new FlowCutCreateOperation(this.Site, this.ToolName);
            fo.CreateOperationName();
            // fo.SetFaces(this.faces.ToArray());
            // fo.SetReferencetool(this.referencetoolName);
            return fo;
        }
    }
}
