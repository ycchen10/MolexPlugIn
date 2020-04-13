using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class ElectrodeCAM
    {
        private List<NCGroup> program = new List<NCGroup>();
        private List<NCGroup> tool = new List<NCGroup>();
        private List<NCGroup> geometry = new List<NCGroup>();
        private List<NCGroup> method = new List<NCGroup>();
        private Session theSession;
        /// <summary>
        /// 程序组
        /// </summary>
        public List<NCGroup> ProgramGroup
        {
            get
            {
                if (program.Count == 0)
                {
                    Part workPart = theSession.Parts.Work;
                    NCGroup pm = workPart.CAMSetup.GetRoot(CAMSetup.View.ProgramOrder);
                    foreach (NCGroup ng in pm.GetMembers())
                    {
                        if (ng.Name.Equals("AAA"))
                        {
                            foreach (NCGroup np in ng.GetMembers())
                            {
                                program.Add(np as NCGroup);
                            }
                        }
                    }

                }
                return program;
            }
        }
        /// <summary>
        /// 刀具
        /// </summary>
        public List<NCGroup> ToolGroup
        {
            get
            {
                if (tool.Count == 0)
                {
                    Part workPart = theSession.Parts.Work;
                    NCGroup toolGroup = workPart.CAMSetup.GetRoot(CAMSetup.View.MachineTool);
                    foreach (NCGroup np in toolGroup.GetMembers())
                    {
                        tool.Add(np as NCGroup);
                    }
                }
                return tool;
            }
        }
        /// <summary>
        /// 加工体
        /// </summary>
        public List<NCGroup> Geometry
        {
            get
            {
                if (geometry.Count == 0)
                {
                    Part workPart = theSession.Parts.Work;
                    NCGroup ge = workPart.CAMSetup.GetRoot(CAMSetup.View.Geometry);
                    foreach (NCGroup np in ge.GetMembers())
                    {
                        geometry.Add(np as NCGroup);
                        if (np.GetMembers().Length > 0)
                        {
                            foreach (NCGroup np2 in np.GetMembers())
                            {
                                geometry.Add(np2 as NCGroup);
                            }
                        }
                    }
                }
                return geometry;
            }

        }
        /// <summary>
        /// 加工方法
        /// </summary>
        public List<NCGroup> MethodGroup
        {
            get
            {
                if (method.Count == 0)
                {
                    Part workPart = theSession.Parts.Work;
                    NCGroup methodGroup = workPart.CAMSetup.GetRoot(CAMSetup.View.MachineMethod);
                    foreach (NCGroup np in methodGroup.GetMembers())
                    {
                        method.Add(np as NCGroup);
                       
                    }
                }
                return method;
            }
        }

        public List<ProgramModel> Operations { get; set; }

        public ElectrodeCAM()
        {
            theSession = Session.GetSession();
        }
        /// <summary>
        /// 查找刀具
        /// </summary>
        /// <param name="toolName"></param>
        /// <returns></returns>
        public NCGroup FindTool(string toolName)
        {
            return ToolGroup.Find(a => a.Name.Equals(toolName, StringComparison.CurrentCultureIgnoreCase));
        }
        /// <summary>
        /// 查找程序组
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public NCGroup FindProgram(string program)
        {
            return ProgramGroup.Find(a => a.Name.Equals(program, StringComparison.CurrentCultureIgnoreCase));
        }
        public NCGroup FindMethod(string method)
        {
            return MethodGroup.Find(a => a.Name.Equals(method, StringComparison.CurrentCultureIgnoreCase));
        }

        public NCGroup FindGeometry(string method)
        {
            return Geometry.Find(a => a.Name.Equals(method, StringComparison.CurrentCultureIgnoreCase));
        }
    }



}
