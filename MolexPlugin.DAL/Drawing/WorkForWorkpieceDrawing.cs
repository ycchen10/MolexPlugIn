using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class WorkForWorkpieceDrawing
    {
        private AssembleModel assemble;
        private WorkModel work;
        private string dllPath;
        private string workpieceTablePath;
        private string plistPath;
        private string workpieceDrawTemplate;
        private Point originPoint;
        public WorkForWorkpieceDrawing(AssembleModel assemble, int workNumber)
        {
            this.assemble = assemble;
            foreach (WorkModel wm in this.assemble.Works)
            {
                if (wm.WorkNumber == workNumber)
                    this.work = wm;
            }
            this.GetTemplate();
        }

        private void GetTemplate()
        {
            dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            workpieceTablePath = dllPath.Replace("application\\", "part\\workpieceTable.prt");
            plistPath = dllPath.Replace("application\\", "part\\eleTable.prt");
            workpieceDrawTemplate = dllPath.Replace("application\\", "part\\EDM.prt");
        }
        public bool CreateDrawing()
        {
            Part workPart = Session.GetSession().Parts.Work;
            if (!workPart.Name.Equals(this.work.AssembleName))
                PartUtils.SetPartDisplay(this.work.PartTag);
            string workpiece = this.work.MoldInfo.MoldNumber + "-" + this.work.MoldInfo.WorkpieceNumber + this.work.MoldInfo.EditionNumber;
            WorkpieceInfo info = null;
            List<WorkpieceInfo> otherPart = new List<WorkpieceInfo>();
            foreach (Part part in this.assemble.Workpieces)
            {
                if (workpiece.Equals(part.Name))
                    info = new WorkpieceInfo(part, work);
                else
                    otherPart.Add(new WorkpieceInfo(part, this.work));
            }
            if (info == null)
                return false;

            double scale = GetScale(info);
            CreatePointAndCenterLine(info.CenterPt, info.DisPt);
            HostWorkpieceDrawing(info.workpiece, info, scale);
            if (otherPart.Count > 0)
            {
                OtherWorkpieceDrawing(otherPart, info, scale);
            }
            return true;
        }
        /// <summary>
        /// 主工件图
        /// </summary>
        /// <param name="workpiecePart"></param>
        private void HostWorkpieceDrawing(Part workpiecePart, WorkpieceInfo info, double scale)
        {

            NXOpen.Drawings.DrawingSheet sheet = Basic.DrawingUtils.DrawingSheetByName(workpiecePart.Name);
            if (sheet != null)
            {
                DeleteObject.Delete(sheet);
                DeleteObject.UpdateObject();
            }
            sheet = Basic.DrawingUtils.DrawingSheet(workpieceDrawTemplate, 297, 420, workpiecePart.Name);
            this.CreateView(scale, this.GetFirstPoint(info, scale), info);
            double[] plistOrigin = { 204, 70, 0 };
            Basic.DrawingUtils.CreatePlist(plistPath, plistOrigin);
            Basic.DrawingUtils.UpdateViews(sheet);


        }

        private void OtherWorkpieceDrawing(List<WorkpieceInfo> otherPart, WorkpieceInfo hostInfo, double scale)
        {
            int count = (int)Math.Floor(340 / (2 * hostInfo.DisPt.X * scale + 40));
            for (int i = 0; i < otherPart.Count; i++)
            {
                List<WorkpieceInfo> infos = new List<WorkpieceInfo>();
                for (int k = 0; k < count; k++)
                {
                    if (k + i < otherPart.Count)
                    {
                        infos.Add(otherPart[k + i]);
                        i = k + i;
                    }

                    else
                    {
                        i = k + i;
                        break;
                    }
                }
                OtherWorkpieceView(infos, scale);
            }
        }
        /// <summary>
        /// 画中心点和中心线
        /// </summary>
        /// <param name="disPt"></param>
        private void CreatePointAndCenterLine(Point3d centerPt, Point3d disPt)
        {

            Point3d temp = new Point3d(0, 0, 0);
            Point3d minX = new Point3d(centerPt.X - disPt.X - 10, 0, 0);
            Point3d maxX = new Point3d(centerPt.X + disPt.X + 10, 0, 0);
            Point3d minY = new Point3d(0, centerPt.Y - disPt.Y - 10, 0);
            Point3d maxY = new Point3d(0, centerPt.Y + disPt.Y + 10, 0);

            Matrix4 inver = this.work.WorkMatr.GetInversMatrix();
            inver.ApplyPos(ref temp);
            inver.ApplyPos(ref minX);
            inver.ApplyPos(ref maxX);
            inver.ApplyPos(ref minY);
            inver.ApplyPos(ref maxY);

            this.originPoint = PointUtils.CreatePointFeature(temp);

            Line line1 = this.work.PartTag.Curves.CreateLine(minX, maxX);
            Line line2 = this.work.PartTag.Curves.CreateLine(minY, maxY);
            SetLineObj(20, new Line[2] { line1, line2 });
        }

        private void CreateView(double scale, Point3d originPt, WorkpieceInfo info)
        {
            WorkpieceDrawing dra = new WorkpieceDrawing(work, info, originPoint);
            dra.CreateView(scale, originPt, this.workpieceTablePath);
        }
        /// <summary>
        /// 设置中心线
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="line"></param>
        private void SetLineObj(int layer, params Line[] line)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            foreach (Line temp in line)
            {
                theUFSession.Obj.SetColor(temp.Tag, 186);
                theUFSession.Obj.SetLayer(temp.Tag, layer);
                theUFSession.Obj.SetFont(temp.Tag, 7);
            }

        }
        private void SetOrigin(int layer, params Point[] pt)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            foreach (Point i in pt)
            {
                theUFSession.Obj.SetColor(i.Tag, 186);
                theUFSession.Obj.SetLayer(i.Tag, 20);
                theUFSession.Obj.SetName(i.Tag, "CenterPoint");
            }
        }
        /// <summary>
        /// 获取比例
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private double GetScale(WorkpieceInfo info)
        {
            double x = 120 / (info.DisPt.X * 2);
            double y = 160 / (info.DisPt.Y * 2 + info.DisPt.Z * 2);
            if (x > y)
            {
                if (y > 1)
                    return Math.Floor(y);
                else
                    return Math.Round(y, 1);
            }
            else
            {
                if (x > 1)
                    return Math.Floor(x);
                else
                    return Math.Round(x, 1);
            }
        }
        /// <summary>
        /// 获取第一个设定点
        /// </summary>
        /// <param name="info"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private Point3d GetFirstPoint(WorkpieceInfo info, double scale)
        {
            Point3d pt = new Point3d(0, 0, 0);
            pt.X = 50 + (info.DisPt.X) * scale;
            pt.Y = 240 - (info.DisPt.Y) * scale;
            return pt;
        }

        private void OtherWorkpieceView(List<WorkpieceInfo> infos, double scale)
        {

            Point3d firstPt = GetFirstPoint(infos[0], scale);
            double length = 0;
            foreach (WorkpieceInfo wk in infos)
            {

                length += 2 * wk.DisPt.X * scale;
            }
            NXOpen.Drawings.DrawingSheet sheet = Basic.DrawingUtils.DrawingSheetByName(infos[0].workpiece.Name);
            if (sheet != null)
            {
                DeleteObject.Delete(sheet);
                DeleteObject.UpdateObject();
            }
            int k = 0;
            if (infos.Count == 1)
            {
                k = 0;
            }
            else
            {
                k = (int)Math.Floor(300 - length) / (infos.Count - 1);
            }
            sheet = Basic.DrawingUtils.DrawingSheet(workpieceDrawTemplate, 297, 420, infos[0].workpiece.Name);
            for (int i = 0; i < infos.Count; i++)
            {
                Point3d temp;
                if (i == 0)
                    temp = firstPt;
                else
                {
                    double x = infos[i - 1].DisPt.X * scale + infos[i].DisPt.X * scale + i * k;
                    temp = new Point3d(firstPt.X + x, firstPt.Y, firstPt.Z);
                }

                CreateView(scale, temp, infos[i]);
            }
            Basic.DrawingUtils.UpdateViews(sheet);
        }

    }
}
