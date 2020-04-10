using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;

namespace MolexPlugin.Model
{
    public class PointToPointModel
    {
        NXOpen.CAM.Operation Oper;

        public PointToPointModel(NXOpen.CAM.Operation oper)
        {
            this.Oper = oper;
        }
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
    }
}
