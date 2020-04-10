using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.UF;

namespace MolexPlugin
{
    public class MooldePluginMain
    {

        public static int Main(string[] args)
        {
            #region  公共工具
            if (args[0] == "MENU_MoveObject")
            {
                MoveObject move = new MoveObject();
                move.Show();
            }
            if (args[0] == "MENU_MoveObjectMin")
            {
                MoveObjectOfCenter move = new MoveObjectOfCenter("MENU_MoveObjectMin");
                move.Create();

            }
            if (args[0] == "MENU_MoveObjectMax")
            {
                MoveObjectOfCenter move = new MoveObjectOfCenter("MENU_MoveObjectMax");
                move.Create();
            }
            if (args[0] == "MENU_MoveObjectRotateX")
            {
                MoveObjectOfCenter move = new MoveObjectOfCenter("MENU_MoveObjectRotateX");
                move.Create();
            }
            if (args[0] == "MENU_MoveObjectRotateY")
            {
                MoveObjectOfCenter move = new MoveObjectOfCenter("MENU_MoveObjectRotateY");
                move.Create();
            }
            if (args[0] == "MENU_MoveObjectRotateZ")
            {
                MoveObjectOfCenter move = new MoveObjectOfCenter("MENU_MoveObjectRotateZ");
                move.Create();
            }
            #endregion

            #region 电极设计
            if (args[0] == "MENU_SuperBox")
            {
                SuperBox superBox = new SuperBox();
                superBox.Show();
            }
            if (args[0] == "MENU_AnalyzeBodyAndFace")
            {
                AnalyzeBodyAndFace analyze = new AnalyzeBodyAndFace();
                analyze.Show();
            }

            if (args[0] == "MENU_AddEdmAsm")
            {
                AddEdmAsm add = new AddEdmAsm();
                add.Show();
            }
            if (args[0] == "MENU_AddWork")
            {
                AddWork add = new AddWork();
                add.Show();
            }
            if (args[0] == "MENU_EleStandardSeatZ+")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("Z+");
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatX+")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("X-");
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatY+")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("Y+");
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatX-")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("X+");
                form.Show();

            }
            if (args[0] == "MENU_EleStandardSeatY-")
            {
                EleStandardSeatCreateForm form = new EleStandardSeatCreateForm("Y-");
                form.Show();

            }
            if (args[0] == "MENU_DeleteEle")
            {

                DeleteEle delete = new DeleteEle();
                delete.Show();
            }
            if (args[0] == "MENU_PositionEle")
            {

                PositionEle posit = new PositionEle();
                posit.Show();
            }
            if (args[0] == "MENU_Interference")
            {
                Interference inter = new Interference();
                inter.Show();
            }
            if (args[0] == "MENU_WorkpieceDrawing")
            {
                new WorkpieceDrawingCreateForm().Show();
            }
            if (args[0] == "MENU_ElectrodeDrawing")
            {
                new ElectrodeDrawingCreateForm().Show();
            }
            if (args[0] == "MENU_Bom")
            {
                new BomCreateForm().Show();
            }
            #endregion
           // test.ces();
            if (args[0] == "MENU_EleProgram")
            {
                EleProgram mode = new EleProgram();
                mode.Show();
            }
            return 1;
        }
        /// <summary>
        /// 卸载函数
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static int GetUnloadOption(string arg)
        {
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
            return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }

    }
}
