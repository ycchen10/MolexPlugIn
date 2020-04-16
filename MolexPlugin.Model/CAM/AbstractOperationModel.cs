using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using NXOpen.CAM;

namespace MolexPlugin.Model
{
    public abstract class AbstractOperationModel : IDisplayObject
    {
        /// <summary>
        /// 刀路
        /// </summary>
        public NXOpen.CAM.Operation Oper { get; private set; }

        public NCGroupModel GroupModel { get; set; }
        /// <summary>
        /// 程序名
        /// </summary>
        public string OperName { get; set; }

        public Node Node { get; set; }

        string templateName;
        protected Part workPart;
        public AbstractOperationModel(NXOpen.CAM.Operation oper)
        {
            workPart = Session.GetSession().Parts.Work;
            this.Oper = oper;
        }
        public AbstractOperationModel(NCGroupModel model, string templateName)
        {
            this.GroupModel = model;
            workPart = Session.GetSession().Parts.Work;
            this.templateName = templateName;
        }

        /// <summary>
        /// 创建刀路
        /// </summary>
        /// <param name="templateName">模板名</param>
        /// <param name="templateOperName">刀路模板名</param>
        /// <param name="name">程序名</param>
        /// <param name="groupModel">父程序组</param>
        /// <returns></returns>
        protected void CreateOperation(string templateOperName, string name, NCGroupModel groupModel)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.CAM.Operation operation1;
            operation1 = workPart.CAMSetup.CAMOperationCollection.Create(groupModel.ProgramGroup, groupModel.MethodGroup, groupModel.ToolGroup,
                groupModel.GeometryGroup, templateName, templateOperName, NXOpen.CAM.OperationCollection.UseDefaultName.False, name);
            this.Oper = operation1;

        }
        /// <summary>
        /// 编辑刀路
        /// </summary>
        public abstract void Create(string name);
        /// <summary>
        /// 获取刀路数据
        /// </summary>
        public virtual OperationData GetOperationData()
        {
            OperationData data = new OperationData();
            data.Oper = this.Oper;
            data.OperName = this.Oper.Name.ToString();
            data.ToolNCGroup = this.Oper.GetParent(NXOpen.CAM.CAMSetup.View.MachineTool);
            data.OperGroup = this.Oper.GetParent(NXOpen.CAM.CAMSetup.View.ProgramOrder).Name;
            data.Tool = new ToolDataModel(data.ToolNCGroup);
            data.OperTime = this.Oper.GetToolpathTime() / 1440.0;
            data.OprtLength = this.Oper.GetToolpathLength();
            string path = PostOperation(this.Oper);
            GetPostData(data, path);
            return data;
        }
        /// <summary>
        /// 获取父项
        /// </summary>
        public void GetNCGroupMolde()
        {
            this.GroupModel = new NCGroupModel();
            this.GroupModel.ProgramGroup = this.Oper.GetParent(CAMSetup.View.ProgramOrder);
            this.GroupModel.GeometryGroup = this.Oper.GetParent(CAMSetup.View.Geometry);
            this.GroupModel.ToolGroup = this.Oper.GetParent(CAMSetup.View.MachineTool);
            this.GroupModel.MethodGroup = this.Oper.GetParent(CAMSetup.View.MachineMethod);
        }
        /// <summary>
        /// 设置起始点
        /// </summary>
        /// <param name="pt"></param>
        public abstract void SetRegionStartPoints(params Point3d[] pt);
        /// <summary>
        /// 设置余量
        /// </summary>
        /// <param name="stock"></param>
        public abstract void SetStock(double partStock, double floorStock);
        /// <summary>
        /// 后处理
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>z
        private string PostOperation(NXOpen.CAM.Operation op)
        {

            Part workPart = Session.GetSession().Parts.Work;
            string filePath = System.IO.Path.GetDirectoryName(workPart.FullPath);
            NXOpen.CAM.Operation[] ops = { op };
            string postPath = filePath + "\\" + op.Name + ".txt";
            if (File.Exists(postPath))
            {
                File.Delete(postPath);
            }
            workPart.CAMSetup.Postprocess(ops, "mill3ax", postPath, NXOpen.CAM.CAMSetup.OutputUnits.Metric);
            return postPath;
        }
        /// <summary>
        ///获取后处理数据
        /// </summary>
        /// <param name="path"></param>
        private void GetPostData(OperationData data, string path)
        {
            List<string> text = new List<string>();
            string line;
            if (File.Exists(path))
            {

                StreamReader sr = new StreamReader(path);
                while ((line = sr.ReadLine()) != null)
                {
                    text.Add(line);
                }
                sr.Close();
                File.Delete(path);
            }

            data.CutterCompenstation = text[10];
            data.Zmax = text[12];
            data.Zmin = text[11];

        }
        /// <summary>
        /// 移动程序到程序组下
        /// </summary>
        /// <param name="program"></param>
        public void MoveOpreationToProgram(NCGroup program)
        {
            Part workPart = Session.GetSession().Parts.Work;
            workPart.CAMSetup.MoveObjects(CAMSetup.View.ProgramOrder, new CAMObject[1] { this.Oper }, program, CAMSetup.Paste.Inside);
        }
        /// <summary>
        /// 移动程序到刀具下
        /// </summary>
        /// <param name="tool"></param>
        public void MoveOperationToTool(NCGroup tool)
        {
            Part workPart = Session.GetSession().Parts.Work;
            workPart.CAMSetup.MoveObjects(CAMSetup.View.MachineTool, new CAMObject[1] { this.Oper }, tool, CAMSetup.Paste.Inside);
        }
        /// <summary>
        /// 移动程序到加工体下
        /// </summary>
        /// <param name="geometry"></param>
        public void MoveOperationToGeometry(NCGroup geometry)
        {
            Part workPart = Session.GetSession().Parts.Work;
            workPart.CAMSetup.MoveObjects(CAMSetup.View.Geometry, new CAMObject[1] { this.Oper }, geometry, CAMSetup.Paste.Inside);
        }
        /// <summary>
        /// 移动程序到加工方法下
        /// </summary>
        /// <param name="method"></param>
        public void MoveOperationToMethod(NCGroup method)
        {
            Part workPart = Session.GetSession().Parts.Work;
            workPart.CAMSetup.MoveObjects(CAMSetup.View.MachineMethod, new CAMObject[1] { this.Oper }, method, CAMSetup.Paste.Inside);
        }
        public void Highlight(bool highlight)
        {
            Session theSession = Session.GetSession();
            if (highlight)
                theSession.CAMSession.PathDisplay.ShowToolPath(this.Oper);

        }
    }
}
