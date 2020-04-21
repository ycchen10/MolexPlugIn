﻿using System;
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
    /// 等宽
    /// </summary>
    public class SurfaceContourCreateOperation : AbstractCreateOperation
    {
        private List<Face> faces = new List<Face>();
        private bool steep;
        public SurfaceContourCreateOperation(int site, string tool) : base(site, tool)
        {

        }
        public override void CreateOperation(ElectrodeCAM eleCam, double inter)
        {
            this.Oper = ElectrodeOperationTemplate.CreateOperationOfSurfaceContour(this.NameModel, eleCam);
            this.Oper.Create(this.NameModel.OperName);
            if (faces.Count > 0)
                (this.Oper as SurfaceContourModel).SetGeometry(faces.ToArray());
            if (steep)
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
            this.faces = faces.ToList();
        }
        public override void CreateOperationName()
        {
            string program = "O000" + this.Site.ToString();
            this.NameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfSurfaceContour(program, this.ToolName);
        }

        public override AbstractCreateOperation CopyOperation()
        {
            SurfaceContourCreateOperation so = new SurfaceContourCreateOperation(this.Site, this.ToolName);
            so.CreateOperationName();
            // so.SetFaces(this.faces.ToArray());
            return so;
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
