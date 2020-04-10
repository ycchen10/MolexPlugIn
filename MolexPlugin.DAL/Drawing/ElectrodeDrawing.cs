using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using NXOpen.Drawings;
using Basic;
using MolexPlugin.Model;


namespace MolexPlugin.DAL
{
    public class ElectrodeDrawing
    {
        private ElectrodeDrawingInfo draInfo;
        private string eleTemplate;
        public ElectrodeDrawing(AssembleModel assemble, string eleName)
        {
            draInfo = new ElectrodeDrawingInfo(assemble, eleName);
            GetTemplate();
        }
        public void CreateDrawing()
        {
            Session.GetSession().ApplicationSwitchImmediate("UG_APP_MODELING");
            CreatDwgPart();
            CreateView();
            Session.GetSession().ApplicationSwitchImmediate("UG_APP_DRAFTING");
        }
        private void CreatDwgPart()
        {

            foreach (Part part in Session.GetSession().Parts)
            {
                if (part.Name.ToUpper().Equals(draInfo.DraModel.AssembleName.ToUpper()))
                    part.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
            }
            if (File.Exists(draInfo.DraModel.WorkpiecePath))
            {
                File.Delete(draInfo.DraModel.WorkpiecePath);
            }
            draInfo.DraModel.CreatePart();
            draInfo.DraModel.Load(this.draInfo.DraModel.PartTag);
            PartUtils.SetPartDisplay(draInfo.DraModel.PartTag);
        }

        private void GetTemplate()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            eleTemplate = dllPath.Replace("application\\", "part\\electrode.prt");
        }

        private void CreateView()
        {
            double scale = GetScale();
            NXOpen.Drawings.DrawingSheet sheet = Basic.DrawingUtils.DrawingSheet(eleTemplate, 297, 420, draInfo.DraModel.AssembleName);
            Point3d origin = GetViewOriginPt(scale);
            Point3d projectedPt = new Point3d(0, 0, 0);
            projectedPt.X = origin.X;
            projectedPt.Y = origin.Y - (draInfo.DisPt.Y + draInfo.DisPt.Z) * scale - 30;
            DraftingView topView = Basic.DrawingUtils.CreateView("TOP", origin, scale, draInfo.DraModel.Work.WorkMatr, draInfo.GetHideComp());
            DraftingView proView = Basic.DrawingUtils.CreateProjectedView(topView, projectedPt, scale);

            double eleScale = GetEleScale();
            int[] pre = draInfo.DraModel.Eles[0].EleInfo.Preparation;
            Point3d eleOrigin = GetEleOrigin(eleScale);
            Point3d projectedElePt1 = new Point3d(eleOrigin.X, eleOrigin.Y + (pre[1] * eleScale / 2 + pre[2] * eleScale / 2 + 30), 0);
            Point3d projectedElePt2 = new Point3d(projectedElePt1.X - (pre[0] * eleScale / 2 + pre[1] * eleScale / 2 + 30), projectedElePt1.Y, 0);

            DraftingView topEleView = Basic.DrawingUtils.CreateView("TOP", eleOrigin, eleScale, draInfo.DraModel.Eles[0].EleMatr, draInfo.GetEleHideComp());
            DraftingView proEleView1 = Basic.DrawingUtils.CreateProjectedView(topEleView, projectedElePt1, eleScale);
            DraftingView proEleView2 = Basic.DrawingUtils.CreateProjectedView(proEleView1, projectedElePt2, eleScale);

            Matrix4 mat = new Matrix4(draInfo.DraModel.Eles[0].EleMatr);
            mat.RolateWithX(-2 * Math.PI / 5);
            mat.RolateWithY(Math.PI / 10);
            DraftingView topEleView2 = Basic.DrawingUtils.CreateView("Trimetric", new Point3d(240, 120, 0), eleScale, mat, draInfo.GetEleHideComp());

            List<Point> elePoint = this.draInfo.DraModel.GetEleCompPint();
            Point workPoint = this.draInfo.DraModel.GetWorkCompPoint();
            TopDimension(topView, origin, scale, workPoint, elePoint);
            ProViewDimension(proView, projectedPt, scale, workPoint, elePoint);

            List<Face> face = this.draInfo.EleFaceSort();
            // EleTopDimension(topEleView, eleOrigin, eleScale, face[0]);
            EleProViewDimension(proEleView2, projectedElePt2, eleScale, face[10], elePoint[0]);
            SetViewVisible(new DraftingView[6] { topView, proView, topEleView, proEleView1, proEleView2, topEleView2 });

            if (this.draInfo.DraModel.EleInfo.IsPreparation)
            {
                Basic.DrawingUtils.SetNote(new Point3d(130, 20, 0), 6, "标准铜料");
            }
            else if (!this.draInfo.DraModel.EleInfo.IsPreparation)
            {
                Basic.DrawingUtils.SetNote(new Point3d(130, 20, 0), 6, "非标准铜料");
            }
            Basic.DrawingUtils.SetNote(new Point3d(130, 30, 0), 4, this.draInfo.EleModel.EleInfo.ElePresentation);
            Basic.DrawingUtils.UpdateViews(sheet);
        }

        private Point3d GetViewOriginPt(double scale)
        {

            double higth = 2 * draInfo.DisPt.Y * scale + 2 * draInfo.DisPt.Z * scale;
            double k = (220 - higth) / 2;
            Point3d temp = new Point3d(0, 0, 0);
            temp.X = 90;
            temp.Y = 230 - (draInfo.DisPt.Y * scale);
            return temp;
        }

        private void SetViewVisible(params NXOpen.Drawings.DraftingView[] views)
        {
            foreach (DraftingView dv in views)
            {
                Basic.DrawingUtils.SetLayerHidden(dv);
                Basic.DrawingUtils.SetLayerVisible(new int[2] { 1, 20 }, dv);
            }


        }

        private double GetEleScale()
        {
            int[] pre = this.draInfo.DraModel.EleInfo.Preparation;
            double x = 130.0 / (pre[0] + pre[1]);
            double y = 150.0 / (pre[1] + pre[2]);
            if (x > y)
            {
                if (y > 1)
                    return Math.Round(y, 1) - 0.4;
                else
                    return Math.Round(y, 1);
            }
            else
            {
                if (x > 1)
                    return Math.Round(x, 1) - 0.4;
                else
                    return Math.Round(x, 1);
            }
        }

        private Point3d GetEleOrigin(double scale)
        {
            int[] pre = this.draInfo.DraModel.EleInfo.Preparation;
            Point3d temp = new Point3d(0, 0, 0);
            temp.Y = 230 - (pre[2] * scale + pre[1] * scale / 2);
            temp.X = 270 + (pre[1] * scale + pre[0] * scale / 2);
            return temp;
        }

        private double GetScale()
        {
            this.draInfo.GetBoundingBox();
            double x = 130.0 / (draInfo.DisPt.X * 2) - 0.1;
            double y = 190.0 / (draInfo.DisPt.Y * 2 + draInfo.DisPt.Z * 2) - 0.1;
            if (x > y)
            {
                return Math.Round(y, 1);
            }
            else
            {
                return Math.Round(x, 1);
            }
        }

        private void TopDimension(DraftingView topView, Point3d originPt, double scale, Point workPoint, List<Point> elePoint)
        {
            string err = "";

            Matrix4 mat = this.draInfo.DraModel.Work.WorkMatr;
            PointSort(ref elePoint, mat, "X");
            for (int i = 0; i < elePoint.Count; i++)
            {
                Point3d dimPt = new Point3d(originPt.X + 10.0, originPt.Y + this.draInfo.DisPt.Y * scale + (10 * (i + 1)), 0);
                NXOpen.Annotations.Dimension dim = Basic.DrawingUtils.DimensionHorizontal(topView, dimPt, workPoint, elePoint[i], ref err);
                Basic.DrawingUtils.AppendedTextDim(dim, "EDM SETTING");
                SetDimColor(dim);
                //Point3d temp = (elePoint[i].Prototype as Point).Coordinates;
                //if (temp.X != 0)
                //{
                //    double crude = -(this.draInfo.DraModel.Eles[0].EleInfo.CrudeInter);
                //    double fine = -(this.draInfo.DraModel.Eles[0].EleInfo.FineInter);
                //    if (crude != 0 && fine != 0)
                //        Basic.DrawingUtils.ToleranceDim(dim, NXOpen.Annotations.ToleranceType.BilateralTwoLines, fine, crude);
                //    if (crude != 0 && fine == 0)
                //        Basic.DrawingUtils.ToleranceDim(dim, NXOpen.Annotations.ToleranceType.BilateralTwoLines, 0, crude);
                //    if (crude == 0 && fine != 0)
                //        Basic.DrawingUtils.ToleranceDim(dim, NXOpen.Annotations.ToleranceType.BilateralTwoLines, 0, fine);
                //}
            }
            PointSort(ref elePoint, mat, "Y");
            for (int i = 0; i < elePoint.Count; i++)
            {
                Point3d dimPt = new Point3d(originPt.X - (this.draInfo.DisPt.X * scale + 8 * (i + 1)), originPt.Y + 10, 0);
                NXOpen.Annotations.Dimension dim = Basic.DrawingUtils.DimensionVertical(topView, dimPt, workPoint, elePoint[i], ref err);
                Basic.DrawingUtils.AppendedTextDim(dim, "EDM SETTING");
                SetDimColor(dim);
                //Point3d temp = (elePoint[i].Prototype as Point).Coordinates;
                //if (temp.Y != 0)
                //{
                //    double crude = -(this.draInfo.DraModel.Eles[0].EleInfo.CrudeInter);
                //    double fine = -(this.draInfo.DraModel.Eles[0].EleInfo.FineInter);
                //    if (crude != 0 && fine != 0)
                //        Basic.DrawingUtils.ToleranceDim(dim, NXOpen.Annotations.ToleranceType.BilateralTwoLines, fine, crude);
                //    if (crude != 0 && fine == 0)
                //        Basic.DrawingUtils.ToleranceDim(dim, NXOpen.Annotations.ToleranceType.BilateralTwoLines, 0, crude);
                //    if (crude == 0 && fine != 0)
                //        Basic.DrawingUtils.ToleranceDim(dim, NXOpen.Annotations.ToleranceType.BilateralTwoLines, 0, fine);
                //}
            }

        }

        private void ProViewDimension(DraftingView topView, Point3d originPt, double scale, Point workPoint, List<Point> elePoint)
        {
            string err = "";
            Point3d dimPt = new Point3d(originPt.X - (this.draInfo.DisPt.X * scale + 10), originPt.Y + (this.draInfo.DisPt.X * scale), 0);
            NXOpen.Annotations.Dimension dim = Basic.DrawingUtils.DimensionVertical(topView, dimPt, workPoint, elePoint[0], ref err);
            Basic.DrawingUtils.AppendedTextDim(dim, "EDM SETTING");
            SetDimColor(dim);
        }

        private void EleTopDimension(DraftingView topView, Point3d originPt, double scale, Face face)
        {
            int[] pre = draInfo.DraModel.Eles[0].EleInfo.Preparation;
            List<Edge> xEdge = new List<Edge>();
            List<Edge> yEdge = new List<Edge>();
            string err = "";
            Point3d pt1 = new Point3d(originPt.X - (pre[0] * scale / 2 + 10), originPt.Y, 0);
            Point3d pt2 = new Point3d(originPt.X, originPt.Y - (pre[1] * scale / 2 + 10), 0);
            this.draInfo.GetEdge(face, out xEdge, out yEdge);

            NXOpen.Annotations.Dimension dim1 = Basic.DrawingUtils.DimensionVertical(topView, pt1, xEdge[0], xEdge[1], ref err);
            Basic.DrawingUtils.SetDimensionPrecision(dim1, 0);
            NXOpen.Annotations.Dimension dim2 = Basic.DrawingUtils.DimensionHorizontal(topView, pt2, yEdge[0], yEdge[1], ref err);
            Basic.DrawingUtils.SetDimensionPrecision(dim2, 0);
        }

        private void EleProViewDimension(DraftingView topView, Point3d originPt, double scale, Face face, Point elePoint)
        {
            string err = "";
            int[] pre = draInfo.DraModel.Eles[0].EleInfo.Preparation;
            List<Edge> xEdge = new List<Edge>();
            List<Edge> yEdge = new List<Edge>();
            this.draInfo.GetEdge(face, out xEdge, out yEdge);
            Point3d dimPt = new Point3d(originPt.X, originPt.Y - (pre[1] * scale / 2 + 10), 0);
            NXOpen.Annotations.Dimension dim = Basic.DrawingUtils.DimensionVertical(topView, dimPt, xEdge[0], elePoint, ref err);
            Basic.DrawingUtils.SetDimensionPrecision(dim, 1);
        }

        private void PointSort(ref List<Point> points, Matrix4 mat, string axisName)
        {
            points.Sort(delegate (Point a, Point b)
            {
                Point3d pt1 = a.Coordinates;
                Point3d pt2 = b.Coordinates;
                mat.ApplyPos(ref pt1);
                mat.ApplyPos(ref pt2);
                if (axisName == "X")
                    return pt1.X.CompareTo(pt2.X);
                if (axisName == "Y")
                    return pt1.Y.CompareTo(pt2.Y);
                if (axisName == "Z")
                    return pt1.Z.CompareTo(pt2.Z);
                return 1;
            });
        }

        /// <summary>
        /// 设置尺寸颜色
        /// </summary>
        /// <param name="dim"></param>
        private void SetDimColor(NXOpen.Annotations.Dimension dim)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Annotations.LinearDimensionBuilder linearDimensionBuilder1;
            linearDimensionBuilder1 = workPart.Dimensions.CreateLinearDimensionBuilder(dim);

            linearDimensionBuilder1.Style.LineArrowStyle.SecondArrowheadColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.FirstExtensionLineColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.SecondExtensionLineColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.FirstArrowheadColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.FirstArrowLineColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LineArrowStyle.SecondArrowLineColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LetteringStyle.DimensionTextColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LetteringStyle.AppendedTextColor = workPart.Colors.Find("Magenta");

            linearDimensionBuilder1.Style.LetteringStyle.ToleranceTextColor = workPart.Colors.Find("Magenta");

            NXOpen.NXObject nXObject1;
            nXObject1 = linearDimensionBuilder1.Commit();
            linearDimensionBuilder1.Destroy();
        }
    }
}
