using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;
using MolexPlugin.Model;
using Basic;


namespace MolexPlugin.DAL
{
    public class ElectrodeCAMInfo
    {
        /// <summary>
        /// 基准面边界
        /// </summary>
        public PlanarBoundary BasePlanarPlanarBoundary { get; private set; }
        /// <summary>
        /// 基准底面
        /// </summary>
        public FaceData BaseSubfaceFace { get; private set; }
        /// <summary>
        /// 基准面
        /// </summary>
        public FaceData BaseFace { get; private set; }
        /// <summary>
        /// 是否以扣间隙
        /// </summary>
        public bool IsInter { get; private set; }
        /// <summary>
        /// 最小距离
        /// </summary>
        public double MinDim { get; private set; }
        /// <summary>
        /// 最小半径
        /// </summary>
        public double MinDia { get; private set; }

        public ElectrodeInfo Info { get; private set; }

        private AnalyzeBuilder builder;

        public ElectrodeCAMInfo(ElectrodeModel model)
        {
            this.Info = model.EleInfo;
            AnalyzeElectrode analyze = new AnalyzeElectrode(model);
            this.MinDim = analyze.GetMinDis();
            builder = analyze.AnalyzeBody();
            this.BaseFace = builder.AnalyzeFaces[1].FaceData;
            this.BaseSubfaceFace = builder.AnalyzeFaces[0].FaceData;
            this.MinDia = 2 * GetBaseMinDia(BaseFace.Face);
            this.BasePlanarPlanarBoundary = new PlanarBoundary(this.BaseFace.Face);

        }
        /// <summary>
        /// 获取平坦面（0到45度）
        /// </summary>
        /// <param name="analyzeFace"></param>
        public List<Face> GetFlatFaces()
        {
            List<Face> flat = new List<Face>();
            foreach (AnalyzeFaceSlopeAndRadius ar in builder.AnalyzeFaces)
            {
                if (ar.MinSlope > 0 && ar.MaxSlope <= Math.Round(Math.PI / 4, 1))
                {
                   // ar.face.Color = 186;
                    flat.Add(ar.face);
                }
                  
            }
            return flat;
        }
        /// <summary>
        /// 获取陡峭面(45到90)
        /// </summary>
        /// <param name="analyzeFace"></param>
        public List<Face> GetSteepFaces()
        {
            List<Face> steep = new List<Face>();
            foreach (AnalyzeFaceSlopeAndRadius ar in builder.AnalyzeFaces)
            {
                if (ar.MinSlope < Math.Round(Math.PI / 2, 3) && ar.MaxSlope > Math.Round(Math.PI / 4, 1))
                {
                   // ar.face.Color = 100;
                    steep.Add(ar.face);
                }
                  
            }
            return steep;
        }
        /// <summary>
        /// 获取平面边界
        /// </summary>
        /// <param name="analyzeFace"></param>
        /// <returns></returns>
        public List<Face> GetPlaneFaces()
        {
            List<Face> plane = new List<Face>();
            foreach (AnalyzeFaceSlopeAndRadius ar in builder.AnalyzeFaces)
            {
                if (UMathUtils.IsEqual(ar.MaxSlope, 0) && UMathUtils.IsEqual(ar.MinSlope, 0) && UMathUtils.IsEqual(GetFaceIsHighst(ar.face), 0))
                    plane.Add(ar.face);
            }
            // plane.Remove(this.BaseFace.Face);
            // plane.Remove(this.BaseSubfaceFace.Face);
            return plane;
        }
        /// <summary>
        /// 获取所有面
        /// </summary>
        /// <param name="analyzeFace"></param>
        /// <returns></returns>
        public List<Face> GetAllFaces()
        {
            List<Face> faces = new List<Face>();
            foreach (AnalyzeFaceSlopeAndRadius ar in builder.AnalyzeFaces)
            {
                faces.Add(ar.face);
            }
            faces.Remove(this.BaseFace.Face);
            faces.Remove(this.BaseSubfaceFace.Face);
            return faces;
        }
        /// <summary>
        /// 获取基准面最小半径
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        private double GetBaseMinDia(Face face)
        {
            double min = 9999;
            foreach (Edge eg in face.GetEdges())
            {
                if (eg.SolidEdgeType == Edge.EdgeType.Circular)
                {
                    foreach (Face fe in eg.GetFaces())
                    {
                        AnalyzeFaceSlopeAndRadius ar = new AnalyzeFaceSlopeAndRadius(fe);
                        ar.AnalyzeFace(new Vector3d(0, 0, 1));
                        if (min >= ar.MinRadius)
                            min = ar.MinRadius;
                    }
                }
            }
            if (min == 0)
                min = 9999;
            return min;

        }
        /// <summary>
        /// 获取平面高度
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        private double GetFaceIsHighst(Face face)
        {
            FaceData faceData = FaceUtils.AskFaceData(face);
            double zMax = Math.Round(faceData.BoxMaxCorner.Z, 3);
            double faceZ = zMax;
            foreach (Edge eg in face.GetEdges())
            {
                foreach (Face fa in eg.GetFaces())
                {
                    if (fa.Tag != face.Tag)
                    {
                        FaceData data = FaceUtils.AskFaceData(fa);
                        double z = Math.Round(data.BoxMaxCorner.Z, 3);
                        if (z > zMax)
                            zMax = z;
                    }
                }
            }
            return zMax - faceZ;
        }
    }
}
