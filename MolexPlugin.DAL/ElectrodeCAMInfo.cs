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
        /// 平坦面
        /// </summary>
        public List<Face> FlatFaces { get; private set; } = new List<Face>();
        /// <summary>
        /// 陡峭面
        /// </summary>
        public List<Face> SteepFaces { get; private set; } = new List<Face>();
        /// <summary>
        /// 平面
        /// </summary>
        public List<Face> PlaneFace { get; private set; } = new List<Face>();
        /// <summary>
        /// 是否以扣间隙
        /// </summary>
        public bool IsInter { get; private set; }
        /// <summary>
        /// 最小距离
        /// </summary>
        public double MinDim { get; private set; }

        public double MinDia { get; private set; }

        public ElectrodeInfo Info { get; private set; }

        public ElectrodeCAMInfo(ElectrodeModel model, bool isInter)
        {
            this.Info = model.EleInfo;
            this.IsInter = isInter;
            AnalyzeElectrode analyze = new AnalyzeElectrode(model);
            this.MinDim = analyze.GetMinDis();
            AnalyzeBuilder builder = analyze.AnalyzeBody();
            this.BaseFace = builder.AnalyzeFaces[1].FaceData;
            this.BaseSubfaceFace = builder.AnalyzeFaces[0].FaceData;
            this.MinDim = 2 * builder.MinRadius;
            this.BasePlanarPlanarBoundary = new PlanarBoundary(this.BaseFace.Face);

        }
        /// <summary>
        /// 获取平坦面（0到40度）
        /// </summary>
        /// <param name="analyzeFace"></param>
        public List<Face> GetFlatFaces(List<AnalyzeFaceSlopeAndRadius> analyzeFace)
        {
            List<Face> flat = new List<Face>();
            foreach (AnalyzeFaceSlopeAndRadius ar in analyzeFace)
            {
                if (ar.MinSlope > 0 && ar.MaxSlope <= Math.Round(2 * Math.PI / 9, 3))
                    flat.Add(ar.face);
            }
            return flat;
        }
        /// <summary>
        /// 获取陡峭面
        /// </summary>
        /// <param name="analyzeFace"></param>
        public List<Face> GetSteepFaces(List<AnalyzeFaceSlopeAndRadius> analyzeFace)
        {
            List<Face> steep = new List<Face>();
            foreach (AnalyzeFaceSlopeAndRadius ar in analyzeFace)
            {
                if (ar.MinSlope < Math.Round(Math.PI / 2, 3) && ar.MaxSlope > Math.Round(2 * Math.PI / 9, 3))
                    steep.Add(ar.face);
            }
            return steep;
        }
        /// <summary>
        /// 获取平面边界
        /// </summary>
        /// <param name="analyzeFace"></param>
        /// <returns></returns>
        public List<PlanarBoundary> GetPlaneFaces(List<AnalyzeFaceSlopeAndRadius> analyzeFace)
        {
            List<Face> plane = new List<Face>();
            List<PlanarBoundary> boundary = new List<PlanarBoundary>();
            foreach (AnalyzeFaceSlopeAndRadius ar in analyzeFace)
            {
                if (UMathUtils.IsEqual(ar.MaxSlope, 0) && UMathUtils.IsEqual(ar.MinSlope, 0))
                    plane.Add(ar.face);
            }
            plane.Remove(this.BaseFace.Face);
            plane.Remove(this.BaseSubfaceFace.Face);
            foreach (Face face in plane)
            {
                boundary.Add(new PlanarBoundary(face));
            }
            return boundary;
        }
        /// <summary>
        /// 获取所有面
        /// </summary>
        /// <param name="analyzeFace"></param>
        /// <returns></returns>
        public List<Face> GetAllFaces(List<AnalyzeFaceSlopeAndRadius> analyzeFace)
        {
            List<Face> faces = new List<Face>();
            foreach (AnalyzeFaceSlopeAndRadius ar in analyzeFace)
            {
                faces.Add(ar.face);
            }
            faces.Remove(this.BaseFace.Face);
            faces.Remove(this.BaseSubfaceFace.Face);
            return faces;
        }
    }
}
