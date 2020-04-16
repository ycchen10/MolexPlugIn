using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 边界
    /// </summary>
    public class PlanarBoundary
    {
        private Face face;

        private FaceData faceData;
        public PlanarBoundary(Face face)
        {
            this.face = face;
            this.faceData = FaceUtils.AskFaceData(this.face);
        }
        /// <summary>
        /// 获取内边界
        /// </summary>
        /// <returns></returns>
        public List<BoundaryModel> GetHoleBoundary()
        {
            FaceLoopUtils.LoopList[] list = FaceLoopUtils.AskFaceLoops(face.Tag);
            List<BoundaryModel> models = new List<BoundaryModel>();
            foreach (FaceLoopUtils.LoopList loop in list)
            {
                if (loop.Type == 2)
                {
                    BoundaryModel model = new BoundaryModel();
                    List<Edge> edges = GetLoopToEdge(loop);
                    double zMax = GetLoopMaxOfZ(edges);
                    model.BouudaryPt = new Point3d(0, 0, zMax);
                    model.Edges = edges;
                    model.Types = NXOpen.CAM.BoundarySet.BoundaryTypes.Closed;
                    model.PlaneTypes = NXOpen.CAM.BoundarySet.PlaneTypes.UserDefined;
                    if (UMathUtils.IsEqual(zMax, faceData.BoxMaxCorner.Z))
                    {
                        model.ToolSide = NXOpen.CAM.BoundarySet.ToolSideTypes.InsideOrLeft;
                    }
                    else
                    {
                        model.ToolSide = NXOpen.CAM.BoundarySet.ToolSideTypes.OutsideOrRight;
                    }
                    models.Add(model);
                }
            }
            return models;
        }
        /// <summary>
        /// 获取外边界
        /// </summary>
        /// <param name="model">边界</param>
        /// <param name="blank">毛坯距离</param>
        public void GetPeripheralBoundary(out BoundaryModel model, out double blank)
        {
            blank = 0;
            model = new BoundaryModel();
            FaceLoopUtils.LoopList[] list = FaceLoopUtils.AskFaceLoops(face.Tag);
            foreach (FaceLoopUtils.LoopList loop in list)
            {
                if (loop.Type == 1)
                {
                    List<Edge> edges = GetLoopToEdge(loop);
                    double tempZ = GetLoopMaxOfZ(edges);
                    blank = Math.Round(tempZ - faceData.BoxMaxCorner.Z, 4);
                    model.BouudaryPt = new Point3d(0, 0, tempZ);
                    model.ToolSide = NXOpen.CAM.BoundarySet.ToolSideTypes.InsideOrLeft;
                    model.Types = NXOpen.CAM.BoundarySet.BoundaryTypes.Closed;
                    model.PlaneTypes = NXOpen.CAM.BoundarySet.PlaneTypes.Automatic;
                    model.Edges = edges;
                }
            }

        }
        /// <summary>
        /// Loop转换为边
        /// </summary>
        /// <param name="loop"></param>
        /// <returns></returns>
        private List<Edge> GetLoopToEdge(FaceLoopUtils.LoopList loop)
        {
            List<Edge> edges = new List<Edge>();
            foreach (Tag t in loop.EdgeList)
            {
                edges.Add(NXObjectManager.Get(t) as Edge);
            }
            return edges;
        }
        /// <summary>
        /// 获取loop最高z值
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        private double GetLoopMaxOfZ(List<Edge> edges)
        {
            double zMax = Math.Round(faceData.BoxMaxCorner.Z, 4);
            foreach (Edge e in edges)
            {
                foreach (Face fa in e.GetFaces())
                {
                    if (fa.Tag != face.Tag)
                    {
                        FaceData data = FaceUtils.AskFaceData(fa);
                        double z = Math.Round(data.BoxMaxCorner.Z, 4);
                        if (z > zMax)
                            zMax = z;
                    }
                }
            }
            return zMax;
        }
    }
}
