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
    public class ElectrodeDrawingInfo
    {

        public ElectrodeDrawingModel DraModel { get; private set; }

        public AssembleModel Assemble { get; private set; }

        public Point3d CenterPt { get; private set; }

        public Point3d DisPt { get; private set; }

        public ElectrodeModel EleModel { get; private set; }
        private Session theSession;


        public ElectrodeDrawingInfo(AssembleModel assemble, int eleNumer)
        {
            theSession = Session.GetSession();
            this.Assemble = assemble;
            List<ElectrodeModel> eles = this.Assemble.Electrodes.Where(a => a.EleInfo.EleNumber == eleNumer).ToList();
            WorkModel work = null;
            foreach (WorkModel wm in this.Assemble.Works)
            {
                if (eles[0].WorkNumber == wm.WorkNumber)
                {
                    work = wm;
                    break;
                }
            }
            DraModel = new ElectrodeDrawingModel(work, eles);
            EleModel = GetEleModel();
        }

        public void GetBoundingBox()
        {
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            Matrix4 workMat = this.DraModel.Work.WorkMatr;
            Matrix4 invers = workMat.GetInversMatrix();
            List<Body> bodys = GetBodys();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(workMat, invers);//坐标
            BoundingBoxUtils.GetBoundingBoxInLocal(bodys.ToArray(), csys, workMat, ref centerPt, ref disPt);
            this.CenterPt = centerPt;
            this.DisPt = disPt;
            CreateLineAndOrinig();
        }
        private Part GetWorkpiecePart()
        {
            string workpieceName = this.DraModel.Work.MoldInfo.MoldNumber + "-" + this.DraModel.Work.MoldInfo.WorkpieceNumber + this.DraModel.Work.MoldInfo.EditionNumber;
            foreach (Part part in this.Assemble.Workpieces)
            {
                if (part.Name.Equals(workpieceName))
                    return part;
            }
            return null;
        }

        private List<Body> GetBodys()
        {
            Part workPart = theSession.Parts.Work;
            List<Body> bodys = new List<Body>();
            Part workpiece = GetWorkpiecePart();
            NXOpen.Assemblies.Component workComp = AssmbliesUtils.GetPartComp(workPart, workpiece);
            workComp.Unblank();
            bodys.Add(AssmbliesUtils.GetNXObjectOfOcc(workComp.Tag, workpiece.Bodies.ToArray()[0].Tag) as Body);
            foreach (ElectrodeModel model in this.DraModel.Eles)
            {
                NXOpen.Assemblies.Component eleComp = AssmbliesUtils.GetPartComp(workPart, model.PartTag);
                bodys.Add(AssmbliesUtils.GetNXObjectOfOcc(eleComp.Tag, model.PartTag.Bodies.ToArray()[0].Tag) as Body);
            }
            return bodys;
        }

        public NXOpen.Assemblies.Component[] GetHideComp()
        {
            Part workPart = theSession.Parts.Work;
            List<NXOpen.Assemblies.Component> hide = new List<NXOpen.Assemblies.Component>();
            Part workpiece = GetWorkpiecePart();
            foreach (NXOpen.Assemblies.Component workComp in workPart.ComponentAssembly.RootComponent.GetChildren())
            {
                workComp.Unblank();
                foreach (NXOpen.Assemblies.Component ct in workComp.GetChildren())
                {
                    ct.Unblank();
                    NXOpen.Assemblies.Component[] edm = ct.GetChildren();
                    if (edm.Length != 0)
                    {

                        foreach (NXOpen.Assemblies.Component workpieceComp in edm)
                        {
                            workpieceComp.Unblank();
                            if (!workpieceComp.Name.Equals(workpiece.Name))
                                hide.Add(workpieceComp);
                        }
                    }
                    else
                    {
                        if (!this.DraModel.Eles.Exists(a => a.AssembleName.Equals(ct.Name)))
                            hide.Add(ct);
                    }
                }
            }
            return hide.ToArray();
        }

        public NXOpen.Assemblies.Component[] GetEleHideComp()
        {
            Part workPart = theSession.Parts.Work;
            List<NXOpen.Assemblies.Component> hide = new List<NXOpen.Assemblies.Component>();
            Part workpiece = GetWorkpiecePart();
            foreach (NXOpen.Assemblies.Component workComp in workPart.ComponentAssembly.RootComponent.GetChildren())
            {
                workComp.Blank();
                foreach (NXOpen.Assemblies.Component ct in workComp.GetChildren())
                {
                    ct.Unblank();
                    NXOpen.Assemblies.Component[] edm = ct.GetChildren();
                    if (edm.Length != 0)
                    {
                        hide.Add(ct);
                        foreach (NXOpen.Assemblies.Component workpieceComp in edm)
                        {
                            workpieceComp.Unblank();
                            hide.Add(workpieceComp);
                        }
                    }
                    else
                    {
                        hide.Add(ct);
                    }
                }
            }

            NXOpen.Assemblies.Component eleComp = AssmbliesUtils.GetPartComp(workPart, EleModel.PartTag);
            hide.Remove(eleComp);
            return hide.ToArray();
        }

        public List<Face> EleFaceSort()
        {
            List<Face> faces = new List<Face>();
            faces.AddRange(EleModel.PartTag.Bodies.ToArray()[0].GetFaces());
            faces.Sort(delegate (Face a, Face b)
            {
                FaceData data1 = FaceUtils.AskFaceData(a);
                FaceData data2 = FaceUtils.AskFaceData(b);
                return data1.Point.Z.CompareTo((data2.Point.Z));
            });
            return faces;
        }
        public void GetEdge(Face face, out List<Edge> xEdge, out List<Edge> yEdge)
        {
            xEdge = new List<Edge>();
            yEdge = new List<Edge>();
            string str = "";
            NXOpen.Assemblies.Component ct = AssmbliesUtils.GetPartComp(this.DraModel.PartTag, this.EleModel.PartTag);
            foreach (Edge eg in face.GetEdges())
            {
                NXOpen.UF.UFEval.Line lineData;
                if (EdgeUtils.GetLineData(eg, out lineData, ref str))
                {
                    if (lineData.start[1] == lineData.end[1])
                    {

                        xEdge.Add(AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, eg.Tag) as Edge);
                    }
                    if (lineData.start[0] == lineData.end[0])
                        yEdge.Add(AssmbliesUtils.GetNXObjectOfOcc(ct.Tag, eg.Tag) as Edge);
                }
            }
        }
        private ElectrodeModel GetEleModel()
        {
            ElectrodeModel ele = null;
            foreach (ElectrodeModel model in this.DraModel.Eles)
            {
                if (model.EleInfo.Positioning == "")
                    ele = model;
            }
            return ele;
        }

        public void CreateLineAndOrinig()
        {
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            Matrix4 workMat = this.DraModel.Work.WorkMatr;
            Matrix4 invers = workMat.GetInversMatrix();
            Part workpiecePart = GetWorkpiecePart();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(workMat, invers);//坐标
            BoundingBoxUtils.GetBoundingBoxInLocal(workpiecePart.Bodies.ToArray(), csys, workMat, ref centerPt, ref disPt);
            this.DraModel.Work.CreatePointAndCenterLine(centerPt, disPt);
        }
    }
}
