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
            Part workpiecePart = null;
            foreach (Part part in this.assemble.Workpieces)
            {
                if (workpiece.Equals(part.Name))
                    workpiecePart = part;
            }
            if (workpiecePart == null)
                return false;
            WorkpieceInfo info = new WorkpieceInfo(workpiecePart, work);
            double scale = GetScale(info);
            CreatePointAndCenterLine(info.CenterPt, info.DisPt);
            Basic.DrawingUtils.DrawingSheet(workpieceDrawTemplate, 297, 420, workpiecePart.Name);
            this.CreateView(scale, this.GetFirstPoint(info, scale), info);
            return true;
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
            this.originPoint = this.work.PartTag.Points.CreatePoint(temp);
            Line line1 = this.work.PartTag.Curves.CreateLine(minX, maxX);
            Line line2 = this.work.PartTag.Curves.CreateLine(minY, maxY);
            SetLineObj(20, new Line[2] { line1, line2 });
        }



        private void CreateView(double scale, Point3d originPt, WorkpieceInfo info)
        {
            WorkpieceDrawing dra = new WorkpieceDrawing(work, info, originPoint);
            dra.CreateView(scale, originPt);
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
        /// <summary>
        /// 获取比例
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private double GetScale(WorkpieceInfo info)
        {
            double x = 120 / (info.DisPt.X * 2);
            double y = 180 / (info.DisPt.X * 2 + info.DisPt.Z * 2);
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

        private Point3d GetFirstPoint(WorkpieceInfo info, double scale)
        {
            Point3d pt = new Point3d(0, 0, 0);
            pt.X = 30 + (info.DisPt.X) * scale;
            pt.Y = 250 - (info.DisPt.Y) * scale;
            return pt;
        }
    }
}
