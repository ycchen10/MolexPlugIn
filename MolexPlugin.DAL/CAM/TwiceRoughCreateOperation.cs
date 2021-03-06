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
    /// 二次开粗
    /// </summary>
    public class TwiceRoughCreateOperation : AbstractCreateOperation
    {
        public string ReferenceTool { get; private set; } = "";
        public TwiceRoughCreateOperation(int site, string tool) : base(site, tool)
        {

        }
        public override void CreateOperation(ElectrodeCAM eleCam, double inter)
        {
            this.Oper = ElectrodeOperationTemplate.CreateOperationOfCavityMilling(this.NameModel, eleCam);
            this.Oper.Create(this.NameModel.OperName);
            (this.Oper as CavityMillingModel).SetReferenceTool(eleCam.FindTool(this.ReferenceTool) as Tool);
            if ((0.05 - inter) > 0)
            {
                if (0.03 - inter > 0)
                    this.Oper.SetStock(0.05 - inter, 0.03 - inter);
                else
                    this.Oper.SetStock(0.05 - inter, 0);
            }
            else
            {
                this.Oper.SetStock(0, 0);
            }
        }

        /// <summary>
        /// 设置参考刀具
        /// </summary>
        /// <param name="toolName"></param>
        public void SetReferencetool(string toolName)
        {
            this.ReferenceTool = toolName;
        }

        public override void CreateOperationName()
        {
            string program = "O000" + this.Site.ToString();
            this.NameModel = ElectrodeCAMNameTemplate.AskOperationNameModelOfTwiceRough(program, this.ToolName);
        }

        public override AbstractCreateOperation CopyOperation()
        {
            TwiceRoughCreateOperation to = new TwiceRoughCreateOperation(this.Site, this.ToolName);
            to.CreateOperationName();
            to.SetReferencetool(this.ReferenceTool);
            return to;
        }
    }
}
