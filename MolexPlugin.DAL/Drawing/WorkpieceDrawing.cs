using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
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

        public void CreateView(double scale, Point3d originPt, string tablePath)
        {
            Point3d projectedPt = new Point3d();
            projectedPt.X = originPt.X;
            projectedPt.Y = originPt.Y - this.workpiece.DisPt.Y * scale - this.workpiece.DisPt.Z * scale - 30;
            double[] tableOrigin = new double[3] { originPt.X - 35, projectedPt.Y - this.workpiece.DisPt.Z * scale - 15, 0 };
            DraftingView topView = Basic.DrawingUtils.CreateView("TOP", originPt, scale, this.work.WorkMatr, this.workpiece.GetHideComp());
            DraftingView proView = Basic.DrawingUtils.CreateProjectedView(topView, projectedPt, scale);


            SetViewVisible(topView);
            SetViewVisible(proView);
            TopDimension(topView, originPt, scale);
            ProjectedDimension(proView, projectedPt, scale);
            SetTable(tablePath, tableOrigin);
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
        private void TopDimension(DraftingView topView, Point3d originPt, double scale)
        {
            string err = "";
            Point[] pointComp = this.workpiece.GetPointOcc();
            Point3d minPt = GetPoint(pointComp[0]);
            Point3d maxPt = GetPoint(pointComp[1]);
            if (!UMathUtils.IsEqual(minPt.X, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.workpiece.DisPt.X * scale - 10.0;
                dimPt.Y = originPt.Y + this.workpiece.DisPt.Y * scale + 10.0;
                Basic.DrawingUtils.DimensionHorizontal(topView, dimPt, this.originPoint, pointComp[0], ref err);
            }
            if (!UMathUtils.IsEqual(minPt.Y, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.workpiece.DisPt.X * scale - 10.0;
                dimPt.Y = originPt.Y - this.workpiece.DisPt.Y * scale - 10.0;
                Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, pointComp[0], ref err);
            }
            if (!UMathUtils.IsEqual(maxPt.X, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X + this.workpiece.DisPt.X * scale + 10.0;
                dimPt.Y = originPt.Y + this.workpiece.DisPt.Y * scale + 10.0;
                Basic.DrawingUtils.DimensionHorizontal(topView, dimPt, this.originPoint, pointComp[1], ref err);
            }
            if (!UMathUtils.IsEqual(maxPt.Y, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X + this.workpiece.DisPt.X * scale + 10.0;
                dimPt.Y = originPt.Y - this.workpiece.DisPt.Y * scale - 10.0;
                Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, pointComp[1], ref err);
            }

        }
        /// <summary>
        /// 投影视图标注
        /// </summary>
        /// <param name="topView"></param>
        /// <param name="originPt"></param>
        private void ProjectedDimension(DraftingView topView, Point3d originPt, double scale)
        {
            string err = "";
            Point[] pointComp = this.workpiece.GetPointOcc();
            Point3d minPt = GetPoint(pointComp[0]);
            Point3d maxPt = GetPoint(pointComp[1]);
            if (!UMathUtils.IsEqual(minPt.Z, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X - this.workpiece.DisPt.X * scale - 10.0;
                dimPt.Y = originPt.Y - this.workpiece.DisPt.Z * scale - 10.0;
                Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, pointComp[0], ref err);
            }
            if (!UMathUtils.IsEqual(maxPt.Z, 0))
            {
                Point3d dimPt = new Point3d(0, 0, 0);
                dimPt.X = originPt.X + this.workpiece.DisPt.X * scale + 10.0;
                dimPt.Y = originPt.Y + this.workpiece.DisPt.Z * scale + 10.0;
                Basic.DrawingUtils.DimensionVertical(topView, dimPt, this.originPoint, pointComp[1], ref err);
            }
        }

        private void SetViewVisible(DraftingView view)
        {
            Basic.DrawingUtils.SetLayerHidden(view);
            Basic.DrawingUtils.SetLayerVisible(new int[2] { 1, 20 }, view);
            //  Basic.DrawingUtils.ShowComponent(view, this.workpiece.workpieceComp);
        }
        /// <summary>
        /// 获取最小设定值
        /// </summary>
        /// <returns></returns>
        private Point3d GetMinSetValue()
        {
            Point[] pointComp = this.workpiece.GetPointOcc();
            Point3d min = new Point3d();
            Point3d minPt = pointComp[0].Coordinates;
            Point3d maxPt = pointComp[1].Coordinates;
            this.work.WorkMatr.ApplyPos(ref minPt);
            this.work.WorkMatr.ApplyPos(ref maxPt);
            if (Math.Abs(minPt.X) > Math.Abs(maxPt.X))
                min.X = maxPt.X;
            else
                min.X = minPt.X;
            if (Math.Abs(minPt.Y) > Math.Abs(maxPt.Y))
                min.Y = maxPt.Y;
            else
                min.Y = minPt.Y;
            if (Math.Abs(minPt.Z) > Math.Abs(maxPt.Z))
                min.Z = maxPt.Z;
            else
                min.Z = minPt.Z;
            return min;
        }
        /// <summary>
        /// 设置表格注释
        /// </summary>
        /// <param name="tablePath"></param>
        /// <param name="origin"></param>
        private void SetTable(string tablePath, double[] origin)
        {
            Tag rowTag = Tag.Null;
            Tag oldrowTag = Tag.Null;
            Tag[] columnsTags = new Tag[5];
            Tag[] cellTag = new Tag[5];
            Tag[] oldcellTag = new Tag[5];
            UFSession theUFSession = UFSession.GetUFSession();
            Point3d min = GetMinSetValue();
            Tag tableTag = Basic.DrawingUtils.CreateTable(tablePath, origin, 1);
            theUFSession.Tabnot.AskNthRow(tableTag, 0, out oldrowTag);
            theUFSession.Tabnot.AskNthRow(tableTag, 1, out rowTag);
            for (int i = 0; i < 5; i++)
            {
                UFTabnot.CellPrefs cellPrefs = new UFTabnot.CellPrefs();
                theUFSession.Tabnot.AskNthColumn(tableTag, i, out columnsTags[i]);
                theUFSession.Tabnot.AskCellAtRowCol(oldrowTag, columnsTags[i], out oldcellTag[i]);
                theUFSession.Tabnot.AskCellPrefs(oldcellTag[i], out cellPrefs);
                theUFSession.Tabnot.AskCellAtRowCol(rowTag, columnsTags[i], out cellTag[i]);
                theUFSession.Tabnot.SetCellPrefs(cellTag[i], ref cellPrefs);
            }
            theUFSession.Tabnot.SetCellText(cellTag[0], (1).ToString());
            theUFSession.Tabnot.SetCellText(cellTag[1], workpiece.workpiece.Name);
            theUFSession.Tabnot.SetCellText(cellTag[2], Math.Round(-min.X, 3).ToString());
            theUFSession.Tabnot.SetCellText(cellTag[3], Math.Round(-min.Y, 3).ToString());
            theUFSession.Tabnot.SetCellText(cellTag[4], Math.Round(min.Z, 3).ToString());
        }

        private void SetViewSite(DraftingView topView, DraftingView proView, Point3d origin)
        {
            DrawingOperation topOper = new DrawingOperation(topView);
            DrawingOperation proOper = new DrawingOperation(proView);



        }
    }
}

