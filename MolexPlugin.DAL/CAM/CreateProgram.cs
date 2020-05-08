using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
using NXOpen.UF;
using System.Text.RegularExpressions;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建程序组
    /// </summary>
    public class CreateProgram
    {
        private List<NCGroup> program = new List<NCGroup>();

        private NCGroup parent;

        public CreateProgram(NCGroup parent)
        {
            this.parent = parent;
            GetProgram();
        }
        public List<NCGroup> Program
        {
            get
            {

                return program;
            }
        }

        private void GetProgram()
        {
            foreach (NCGroup ng in this.parent.GetMembers())
            {
                program.Add(ng as NCGroup);
            }
        }

        /// <summary>
        /// 修改程序组
        /// </summary>
        /// <param name="name"></param>
        public void AlterProgram(string name)
        {
            int temp = 1000000;
            int index = 1;
            foreach (NCGroup np in program)
            {
                np.SetName(temp.ToString());
                temp--;
            }
            if (Regex.IsMatch(name, "[0-9]+$"))
            {
                string[] tp = Regex.Split(name, "[0-9]+$");  //拆分  
                name = tp[0];
            }
            foreach (NCGroup np in program)
            {
                np.SetName(name + index.ToString());

                index++;
            }
        }
        public void AlterProgram()
        {
            UFSession theUFSession = UFSession.GetUFSession();
            int temp = 1000000;
            int index = 1;
            foreach (NCGroup np in program)
            {
                np.SetName(temp.ToString());
                temp--;
            }

            foreach (NCGroup np in program)
            {
                np.SetName("O000" + index.ToString());
                index++;
            }
            theUFSession.UiOnt.Refresh();
        }
        public void AddProgram(int cout, string name)
        {
            Part workPart = Session.GetSession().Parts.Work;
            int index = 0;
            if (program.Count != 0)
            {
                index = program.Count;
                name = program[index - 1].Name;
                if (Regex.IsMatch(name, "[0-9]+$"))
                {
                    string[] temp = Regex.Split(name, "[0-9]+$");  //拆分  
                    name = temp[0];
                }
            }
            else if (Regex.IsMatch(name, "[0-9]+$"))
            {
                string[] temp = Regex.Split(name, "[0-9]+$");  //拆分  
                name = temp[0];
                index = Int32.Parse(temp[1]);
            }
            if (cout > program.Count)
            {

                for (int i = 0; i < cout; i++)
                {
                    NCGroup nCGroup = workPart.CAMSetup.CAMGroupCollection.CreateProgram(parent, "mill_planar", "PROGRAM",
                        NXOpen.CAM.NCGroupCollection.UseDefaultName.False, name + (index + i + 1).ToString());

                }
            }
        }

        public void AddProgram(int cout)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Part workPart = Session.GetSession().Parts.Work;
            int index = 0;
            if (program.Count != 0)
            {
                index = program.Count;
                if (cout > index)
                    cout = cout - index;
                else
                    cout = 0;
            }
            for (int i = 0; i < cout; i++)
            {
                NCGroup nCGroup = workPart.CAMSetup.CAMGroupCollection.CreateProgram(parent, "mill_planar", "PROGRAM",
                    NXOpen.CAM.NCGroupCollection.UseDefaultName.False, "O000" + (index + i + 1).ToString());

            }

            theUFSession.UiOnt.Refresh();
        }

        private void Update(NCGroup group)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.CAM.ProgramOrderGroupBuilder programOrderGroupBuilder1;
            programOrderGroupBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateProgramOrderGroupBuilder(group);
            programOrderGroupBuilder1.Destroy();
        }
    }
}
