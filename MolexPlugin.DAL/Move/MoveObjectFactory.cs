using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace MolexPlugin.DAL
{
    public class MoveObjectFactory
    {
        public static void CreateMove(string moveName)
        {
            Part workPart = Session.GetSession().Parts.Work;
            DisplayableObject[] dispObj = workPart.ModelingViews.WorkView.AskVisibleObjects();
            List<NXObject> objs = new List<NXObject>();          
            foreach (DisplayableObject dis in dispObj)
            {
                if (!(dis is Face) && !(dis is Edge)&&!(dis is CoordinateSystem))
                    objs.Add(dis);
            }
            MoveObjectBasic move = new MoveObjectBasic(objs);
            switch (moveName)
            {
                case "MENU_MoveObjectMin":
                    move.MoveObjectMinCenterPoint();
                    break;
                case "MENU_MoveObjectMax":
                    move.MoveObjectMaxCenterPoint();
                    break;
                case "MENU_MoveObjectRotateX":
                    move.MoveObjectRotate(new Vector3d(1,0,0),90);
                    break;
                case "MENU_MoveObjectRotateY":
                    move.MoveObjectRotate(new Vector3d(0, 1, 0), 90);
                    break;
                case "MENU_MoveObjectRotateZ":
                    move.MoveObjectRotate(new Vector3d(0, 0, 1), 90);
                    break;
                default:
                    break;
            }


        }
    }
}
