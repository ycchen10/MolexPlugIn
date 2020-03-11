using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 分析面斜率和半径
    /// </summary>
    public class AnalyzeFaceSlopeAndRadius : IComparable<AnalyzeFaceSlopeAndRadius>
    {
        private Session theSession;

        private UFSession theUFSession;
        private Part workPart;

        public Face face { get; private set; }
        /// <summary>
        /// 最大斜度
        /// </summary>
        public double MaxSlope { get; private set; } = -999999;
        /// <summary>
        /// 最小斜度
        /// </summary>
        public double MinSlope { get; private set; } = 999999;
        /// <summary>
        /// 最大半径
        /// </summary>
        public double MaxRadius { get; private set; } = -999999;
        /// <summary>
        /// 最小半径
        /// </summary>
        public double MinRadius { get; private set; } = -999999;
        /// <summary>
        /// 射线焦点个数
        /// </summary>
        public int ResultsNum { get; private set; } = 0;

        public FaceData FaceData { get; set; }

        public Vector3d Vec { get; set; }
        public AnalyzeFaceSlopeAndRadius(Face face)
        {
            theSession = Session.GetSession();
            theUFSession = UFSession.GetUFSession();
            this.workPart = theSession.Parts.Work;
            this.face = face;

        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(int color)
        {
            theUFSession.Obj.SetColor(this.face.Tag, color);
        }

        /// <summary>
        /// 分析面
        /// </summary>
        /// <param name="vec"></param>
        public void AnalyzeFace(Vector3d vec)
        {
            this.FaceData = FaceUtils.AskFaceData(this.face);
            this.ResultsNum = TraceARay.AskTraceARayForFaceData(face, vec);
            this.Vec = vec;
            if (this.face.SolidFaceType == Face.FaceType.Planar) //平面
            {
                this.MaxRadius = 0;
                this.MinRadius = 0;
                double angle = Math.Round(UMathUtils.Angle(vec, this.FaceData.Dir), 3);
                this.MaxSlope = angle;
                this.MinSlope = angle;
                return;
            }
            if (this.face.SolidFaceType == Face.FaceType.Cylindrical)
            {

                this.MaxRadius = Math.Round(this.FaceData.Radius, 3) * this.FaceData.IntNorm;
                this.MinRadius = Math.Round(this.FaceData.Radius, 3) * this.FaceData.IntNorm;
                AskFace(vec);
                return;
            }
            if (this.face.SolidFaceType == Face.FaceType.Conical)
            {
                this.MaxRadius = Math.Round(this.FaceData.Radius, 3) * this.FaceData.IntNorm;
                this.MinRadius = Math.Round(this.FaceData.RadData, 3) * this.FaceData.IntNorm;
                AskFace(vec);
                return;
            }
            else
            {
                double[] slope;
                double[] rad;
                FaceUtils.GetSweptSlope(this.face, vec, out slope, out rad);
                foreach (double temp in slope)
                {
                    if (this.MaxSlope > temp)
                        this.MaxSlope = Math.Round( temp,3);
                    if (this.MinSlope < temp)
                        this.MinSlope = Math.Round(temp, 3);
                }
                foreach (double temp in rad)
                {
                    if (this.MaxRadius > temp)
                        this.MaxRadius = Math.Round(temp, 3);
                    if (this.MinRadius < temp)
                        this.MinRadius = Math.Round(temp, 3);
                }
            }
        }
        /// <summary>
        /// 获取（圆锥，R面，球面）最大最小斜率
        /// </summary>
        /// <param name="vec"></param>
        private void AskFace(Vector3d vec)
        {
            Vector3d[] vecs = FaceUtils.AskFaceNormals(this.face);
            foreach (Vector3d temp in vecs)
            {
                double angle = Math.Round(UMathUtils.Angle(vec, temp), 3);
                if (this.MaxSlope <= angle)
                    this.MaxSlope = angle;
                if (this.MinSlope >= angle)
                    this.MinSlope = angle;
            }

        }

        public int CompareTo(AnalyzeFaceSlopeAndRadius other)
        {
            CoordinateSystem wcs = workPart.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(wcs.Origin, this.Vec);
            Point3d centerPt1 = UMathUtils.GetMiddle(this.FaceData.BoxMinCorner, this.FaceData.BoxMaxCorner);
            Point3d centerPt2 = UMathUtils.GetMiddle(other.FaceData.BoxMinCorner, other.FaceData.BoxMaxCorner);
            mat.ApplyPos(ref centerPt1);
            mat.ApplyPos(ref centerPt2);
            return centerPt2.Z.CompareTo(centerPt1.Z);
        }
    }
}
