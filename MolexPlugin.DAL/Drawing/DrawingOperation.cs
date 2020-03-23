using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.Drawings;

namespace MolexPlugin.DAL
{
    public class DrawingOperation
    {
        public Point3d OriginPt { get; private set; }

        public Point3d DisPt { get; private set; }

        public Point3d CenterPt { get; private set; }
        public DraftingView View { get; private set; }

        public DrawingOperation(DraftingView view)
        {
            this.View = view;
            this.OriginPt = view.GetDrawingReferencePoint();
            GetBorders(this.View);
        }

        public void GetBorders(DraftingView view)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            double[] borders = new double[4];
            theUFSession.Draw.AskViewBorders(view.Tag, borders);
            Point3d disPt = new Point3d(0, 0, 0);
            Point3d centerPt = new Point3d(0, 0, 0);
            disPt.X = (borders[2] - borders[0]) / 2;
            disPt.Y = (borders[3] - borders[1]) / 2;
            centerPt.X = (borders[0] + borders[2]) / 2;
            centerPt.Y = (borders[1] + borders[3]) / 2;
        }

    }
}
