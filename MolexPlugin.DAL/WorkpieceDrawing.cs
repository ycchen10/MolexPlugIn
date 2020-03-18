using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Drawings;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class WorkpieceDrawing
    {
        private WorkpieceInfo workpiece;
        private WorkModel work;
        private Point originPoint;
        public WorkpieceDrawing(WorkModel work, WorkpieceInfo workpiece, Point originPoint)
        {
            this.work = work;
            this.workpiece = workpiece;
            this.originPoint = originPoint;
        }

        public void CreateView(double scale, Point3d originPt)
        {
            Point3d projectedPt = new Point3d();
            projectedPt.X = originPt.X;
            projectedPt.Y = originPt.Y - this.workpiece.DisPt.Y - 2 * this.workpiece.DisPt.Z - 20;
            BaseView topView = Basic.DrawingUtils.CreateView("TOP", originPt, scale, this.work.WorkMatr,this.workpiece.GetHideComp());
            BaseView proView = Basic.DrawingUtils.CreateProjectedView(topView, projectedPt, scale);
            TopDimension(topView, originPt);
            ProjectedDimension(proView, projectedPt);
        }
        private Point3d GetPoint(Point pt)
        {
            Point3d temp = pt.Coordinates;
            this.work.WorkMatr.ApplyPos(ref temp);
            return temp;
        }
        /// <summary>
        /// 主视图标注
        /// </summary>
        /// <param name="topView"></param>
        /// <param name="originPt"></param>
        private void TopDimension(BaseView topView, Point3d originPt)
        {
            string err = "";
            Point3d minPt = GetPoint(this.workpiece.Points[0]);
            Point3d maxPt = GetPoint(this.workpiece.Points[1]);
            if (!UMathUtils.IsEqual(minPt.X, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.workpiece.DisPt.X - 5.0;
                dimPt.Y = originPt.Y + this.workpiece.DisPt.Y + 5.0;
                Basic.DrawingUtils.DimensionHorizontal(topView, dimPt, this.originPoint, this.workpiece.Points[0], ref err);
            }
            if (!UMathUtils.IsEqual(minPt.Y, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.workpiece.DisPt.X - 5.0;
                dimPt.Y = originPt.Y - this.workpiece.DisPt.Y - 5.0;
                Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, this.workpiece.Points[0], ref err);
            }
            if (!UMathUtils.IsEqual(maxPt.X, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X + this.workpiece.DisPt.X + 5.0;
                dimPt.Y = originPt.Y + this.workpiece.DisPt.Y + 5.0;
                Basic.DrawingUtils.DimensionHorizontal(topView, dimPt, this.originPoint, this.workpiece.Points[1], ref err);
            }
            if (!UMathUtils.IsEqual(maxPt.Y, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X + this.workpiece.DisPt.X + 5.0;
                dimPt.Y = originPt.Y - this.workpiece.DisPt.Y - 5.0;
                Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, this.workpiece.Points[1], ref err);
            }

        }
        /// <summary>
        /// 投影视图标注
        /// </summary>
        /// <param name="topView"></param>
        /// <param name="originPt"></param>
        private void ProjectedDimension(BaseView topView, Point3d originPt)
        {
            string err = "";
            Point3d minPt = GetPoint(this.workpiece.Points[0]);
            Point3d maxPt = GetPoint(this.workpiece.Points[1]);
            if (UMathUtils.IsEqual(minPt.Z, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.workpiece.DisPt.X - 5.0;
                dimPt.Y = originPt.Y - this.workpiece.DisPt.Y - 5.0;
                Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, this.workpiece.Points[0], ref err);
            }
            if (UMathUtils.IsEqual(maxPt.Z, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.workpiece.DisPt.X - 5.0;
                dimPt.Y = originPt.Y + this.workpiece.DisPt.Y + 5.0;
                Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, this.workpiece.Points[1], ref err);
            }
        }

        public NXOpen.Assemblies.Component GetPartComp(Part parent, Part part)
        {
            Tag[] elePartOccsTag;
            NXOpen.UF.UFSession theUFSession = NXOpen.UF.UFSession.GetUFSession();
            theUFSession.Assem.AskOccsOfPart(parent.Tag, part.Tag, out elePartOccsTag);
            return NXOpen.Utilities.NXObjectManager.Get(elePartOccsTag[0]) as NXOpen.Assemblies.Component;
        }

    }
}
