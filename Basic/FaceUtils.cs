using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpenUI;
using NXOpen.UF;

namespace Basic
{
    /// <summary>
    /// 面数据
    /// </summary>
    public class FaceUtils
    {
        public static Face GetLeftFace(Body axis, ref string errorMsg)
        {
            if (axis == null)
            {
                errorMsg = "体为空";
            }
            List<FaceComparable> FcList = new List<FaceComparable>();
            Face[] faceArr = axis.GetFaces();
            foreach (Face face in faceArr)
            {
                if (face.SolidFaceType == Face.FaceType.Planar)
                {
                    FaceComparable fc = new FaceComparable(face);
                    FcList.Add(fc);
                }
            }
            FcList.Sort();
            return FcList[0].face;
        }

        /// <summary>
        /// 获取面的数据
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static FaceData AskFaceData(Face face)
        {
            NXOpen.UF.UFSession uf = NXOpen.UF.UFSession.GetUFSession();

            int type;
            double[] point = new double[3];
            double[] dir = new double[3];
            double[] box = new double[6];
            double radius;
            double rad_data;
            int norm_dir;

            uf.Modl.AskFaceData(face.Tag, out type, point, dir, box, out radius, out rad_data, out norm_dir);


            FaceData fd = new FaceData();
            fd.FaceType = type;

            //UVector dit = new UVector(dir[0], dir[1], dir[2]);
            //dit.Norm();

            fd.Dir = new Vector3d(dir[0], dir[1], dir[2]);

            fd.BoxMinCorner = new Point3d(box[0], box[1], box[2]);
            fd.BoxMaxCorner = new Point3d(box[3], box[4], box[5]);
            fd.Radius = radius;
            fd.RadData = rad_data;
            fd.IntNorm = norm_dir;
            fd.Point = new Point3d(point[0], point[1], point[2]);
            fd.Face = face;
            return fd;
        }

        /// <summary>
        /// 计算圆柱或圆锥面的最大直径
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static double GetFaceMaxDia(Face face)
        {
            double max = 0;
            if (face.SolidFaceType == Face.FaceType.Cylindrical || face.SolidFaceType == Face.FaceType.Conical || face.SolidFaceType == Face.FaceType.Planar)
            {
                foreach (Edge edge in face.GetEdges())
                {
                    if (edge.SolidEdgeType == Edge.EdgeType.Circular)
                    {
                        double radius = EdgeUtils.GetArcRadius(edge);

                        if (radius > max)
                            max = radius;
                    }
                }
            }

            return max;
        }

        /// <summary>
        /// 计算面的面积
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static double GetFaceArea(Face face)
        {
            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part workPart = theSession.Parts.Work;

            try
            {
                Unit unit1 = workPart.UnitCollection.FindObject("SquareMilliMeter");
                Unit unit2 = workPart.UnitCollection.FindObject("MilliMeter");
                double accuracy = workPart.Preferences.Modeling.AngleToleranceData;
                IParameterizedSurface[] objects1 = new IParameterizedSurface[1];
                objects1[0] = face as IParameterizedSurface;
                MeasureFaces measureface = workPart.MeasureManager.NewFaceProperties(unit1, unit2, accuracy, objects1);
                return measureface.Area;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("FaceUtils:GetFaceArea:" + ex.Message);
            }
            finally
            {
            }

            return 0;
        }


        /// <summary>
        /// 求面的法向
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Vector3d AskFaceNormal(Face face)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag faceTag = face.Tag; //输入面特征
            double[] uvs = new double[4];
            theUFSession.Modl.AskFaceUvMinmax(faceTag, uvs); //获得面u,v参数空间（u,v最小,最大值）
            double[] param = new double[2];                           //输入U,V方向值
            param[0] = (uvs[0] + uvs[1]) / 2;
            param[1] = (uvs[2] + uvs[3]) / 2;
            double[] point = new double[3];        //输出点坐标
            double[] u1 = new double[3];           //输出 输出一阶导数在U位置
            double[] v1 = new double[3];           //输出 输出一阶导数在V位置
            double[] u2 = new double[3];           //输出 输出二阶导数在U位置
            double[] v2 = new double[3];           //输出 输出二阶导数在V位置
            double[] unit_norm = new double[3];    //输出面上该点的矢量方向
            double[] radii = new double[2];        //输出，双半径，输出主曲率半径
            theUFSession.Modl.AskFaceProps(face.Tag, param, point, u1, v1, u2, v2, unit_norm, radii);
            Point3d point_1 = new Point3d(point[0], point[1], point[2]);
            Vector3d vec = new Vector3d(unit_norm[0], unit_norm[1], unit_norm[2]);
            return vec;
        }
        /// <summary>
        /// 求曲率相同曲面u,v方向的法向（如圆锥，R面，球面）
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Vector3d[] AskFaceNormals(Face face)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Vector3d[] vec = new Vector3d[5];
            Tag faceTag = face.Tag; //输入面特征
            double[] uvs = new double[4];
            theUFSession.Modl.AskFaceUvMinmax(faceTag, uvs); //获得面u,v参数空间（u,v最小,最大值）
            double[,] param = new double[5, 2]
            {
                { uvs[0], uvs[2], },
                { uvs[0], uvs[3], },
                { uvs[1], uvs[2], },
                { uvs[1], uvs[3], },
                { (uvs[0] + uvs[1]) / 2,(uvs[2] + uvs[3]) / 2, }
            };                           //输入U,V方向值

            for (int i = 0; i < 5; i++)
            {
                double[] temp = new double[] { param[i, 0], param[i, 1] };
                double[] point = new double[3];        //输出点坐标
                double[] u1 = new double[3];           //输出 输出一阶导数在U位置
                double[] v1 = new double[3];           //输出 输出一阶导数在V位置
                double[] u2 = new double[3];           //输出 输出二阶导数在U位置
                double[] v2 = new double[3];           //输出 输出二阶导数在V位置
                double[] unit_norm = new double[3];    //输出面上该点的矢量方向
                double[] radii = new double[2];        //输出，双半径，输出主曲率半径
                theUFSession.Modl.AskFaceProps(face.Tag, temp, point, u1, v1, u2, v2, unit_norm, radii);
                vec[i] = new Vector3d(unit_norm[0], unit_norm[1], unit_norm[2]);
            }
            return vec;
        }

        /// <summary>
        /// 求面的中点
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Point3d AskFaceOrigin(Face face)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag faceTag = face.Tag; //输入面特征
            double[] uvs = new double[4];
            theUFSession.Modl.AskFaceUvMinmax(faceTag, uvs); //获得面u,v参数空间（u,v最小,最大值）
            double[] param = new double[2];                           //输入U,V方向值
            param[0] = (uvs[0] + uvs[1]) / 2;
            param[1] = (uvs[2] + uvs[3]) / 2;
            double[] point = new double[3];        //输出点坐标
            double[] u1 = new double[3];           //输出 输出一阶导数在U位置
            double[] v1 = new double[3];           //输出 输出一阶导数在V位置
            double[] u2 = new double[3];           //输出 输出二阶导数在U位置
            double[] v2 = new double[3];           //输出 输出二阶导数在V位置
            double[] unit_norm = new double[3];    //输出面上该点的矢量方向
            double[] radii = new double[2];        //输出，双半径，输出主曲率半径
            theUFSession.Modl.AskFaceProps(face.Tag, param, point, u1, v1, u2, v2, unit_norm, radii);
            Point3d point_1 = new Point3d(point[0], point[1], point[2]);
            Vector3d vec = new Vector3d(unit_norm[0], unit_norm[1], unit_norm[2]);
            return point_1;
        }
        public static void AskFaceOriginAndNormal(Face face, out Point3d originPt, out Vector3d normal)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag faceTag = face.Tag; //输入面特征
            double[] uvs = new double[4];
            theUFSession.Modl.AskFaceUvMinmax(faceTag, uvs); //获得面u,v参数空间（u,v最小,最大值）
            double[] param = new double[2];                           //输入U,V方向值
            param[0] = (uvs[0] + uvs[1]) / 2;
            param[1] = (uvs[2] + uvs[3]) / 2;
            double[] point = new double[3];        //输出点坐标
            double[] u1 = new double[3];           //输出 输出一阶导数在U位置
            double[] v1 = new double[3];           //输出 输出一阶导数在V位置
            double[] u2 = new double[3];           //输出 输出二阶导数在U位置
            double[] v2 = new double[3];           //输出 输出二阶导数在V位置
            double[] unit_norm = new double[3];    //输出面上该点的矢量方向
            double[] radii = new double[2];        //输出，双半径，输出主曲率半径
            theUFSession.Modl.AskFaceProps(face.Tag, param, point, u1, v1, u2, v2, unit_norm, radii);
            originPt = new Point3d(point[0], point[1], point[2]);
            normal = new Vector3d(unit_norm[0], unit_norm[1], unit_norm[2]);

        }
        /// <summary>
        /// 求两个面的夹角
        /// </summary>
        /// <param name="face1"></param>
        /// <param name="face2"></param>
        /// <returns></returns>
        public static double Angle(Face face1, Face face2)
        {
            Vector3d vec1 = FaceUtils.AskFaceNormal(face1);
            Vector3d vec2 = FaceUtils.AskFaceNormal(face2);
            return UMathUtils.Angle(vec1, vec2);
        }
        /// <summary>
        /// 求曲面斜率
        /// </summary>
        /// <param name="sweptFace"></param>
        /// <param name="vec"></param>
        /// <param name="slope"></param>
        /// <param name="rad"></param>
        public static void GetSweptSlope(Face sweptFace, Vector3d vec, out double[] slope, out double[] rad)
        {
            rad = new double[2] { 99999, 99999 };
            slope = new double[2] { 99999, -99999 };

            double faceArae = GetFaceArea(sweptFace);
            int accuracy = Convert.ToInt32(faceArae) * 100;

            UFSession theUFSession = UFSession.GetUFSession();
            Tag faceTag = sweptFace.Tag; //输入面特征
            double[] uvs = new double[4];
            theUFSession.Modl.AskFaceUvMinmax(faceTag, uvs); //获得面u,v参数空间（u,v最小,最大值）
            double[] param = new double[2];                           //输入U,V方向值

            for (int i = 0; i < accuracy; i++)
            {
                param[0] = i * (uvs[1] - uvs[0]) / accuracy + uvs[0];
                param[1] = i * (uvs[3] - uvs[2]) / accuracy + uvs[2];

                double[] point = new double[3];        //输出点坐标
                double[] u1 = new double[3];           //输出 输出一阶导数在U位置
                double[] v1 = new double[3];           //输出 输出一阶导数在V位置
                double[] u2 = new double[3];           //输出 输出二阶导数在U位置
                double[] v2 = new double[3];           //输出 输出二阶导数在V位置
                double[] unit_norm = new double[3];    //输出面上该点的矢量方向
                double[] radii = new double[2];        //输出，双半径，输出主曲率半径
                theUFSession.Modl.AskFaceProps(sweptFace.Tag, param, point, u1, v1, u2, v2, unit_norm, radii);

                double angle = UMathUtils.Angle(vec, new Vector3d(unit_norm[0], unit_norm[1], unit_norm[2]));

                if (slope[0] >= angle)
                {
                    slope[0] = angle;
                }
                if (slope[1] < angle)
                {
                    slope[1] = angle;
                }

                if (rad[0] >= radii[0])
                {
                    rad[0] = radii[0];
                }

                if (rad[1] < radii[1])
                {
                    rad[1] = radii[1];
                }


            }


        }
    }

    /// <summary>
    /// 面的数据（AskFaceData 函数的封装）
    /// </summary>
    public class FaceData : IEquatable<FaceData>
    {
        /// <summary>
        /// 面类型
        /// </summary>
        public int FaceType;

        public Point3d Point { get; set; }
        public Vector3d Dir { get; set; }
        /// <summary>
        /// 最小点
        /// </summary>
        public Point3d BoxMinCorner { get; set; }
        /// <summary>
        /// 最大点
        /// </summary>
        public Point3d BoxMaxCorner { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius { get; set; }
        public double RadData { get; set; }
        public int IntNorm { get; set; }
        /// <summary>
        /// 面
        /// </summary>
        public Face Face { get; set; }
        public Point3d GetBoxCenter()
        {
            return new Point3d(0.5 * (BoxMinCorner.X + BoxMaxCorner.X), 0.5 * (BoxMinCorner.Y + BoxMaxCorner.Y), 0.5 * (BoxMinCorner.Z + BoxMaxCorner.Z));
        }

        public bool Equals(FaceData other)
        {
            double[] ptOnObj1 = new double[3];
            double[] ptOnObj2 = new double[3];

            double minDis = AnalysisUtils.AskMinimumDist(this.Face.Tag, other.Face.Tag, out ptOnObj1, out ptOnObj2);
            double anlge = UMathUtils.Angle(this.Dir, other.Dir);
            Point3d pt1 = new Point3d(ptOnObj1[0], ptOnObj1[1], ptOnObj1[2]);
            Point3d pt2 = new Point3d(ptOnObj2[0], ptOnObj2[1], ptOnObj2[2]);
            List<Edge> edges1 = GetPointOnEdge(this.Face, ptOnObj1);
            List<Edge> edges2 = GetPointOnEdge(other.Face, ptOnObj2);
            bool edgeBool = false;
            if (edges1.Count > 0 && edges2.Count > 0)
            {
                foreach (Edge ed in edges1)
                {
                    foreach (Edge ed1 in edges2)
                    {
                        if (ed.Tag == ed1.Tag)
                            edgeBool = true;
                    }
                }
            }
            if ((this.FaceType == other.FaceType) && UMathUtils.IsEqual(anlge, Math.PI) && (this.IntNorm == -other.IntNorm) &&
                UMathUtils.IsEqual(minDis, 0) && UMathUtils.IsEqual(pt1, pt2) && !edgeBool)
                return true;
            else
                return false;
        }
        private List<Edge> GetPointOnEdge(Face face, double[] pt)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            List<Edge> edges = new List<Edge>();
            foreach (Edge edge1 in this.Face.GetEdges()) //判断是否you
            {
                int ptStatus;
                theUFSession.Modl.AskPointContainment(pt, edge1.Tag, out ptStatus);
                if (ptStatus == 3)
                    edges.Add(edge1);
            }
            return edges;
        }
    }


    public class FaceComparable : IComparable<FaceComparable>
    {

        public Face face { get; set; }

        public FaceComparable(Face face)
        {
            this.face = face;
        }

        public int CompareTo(FaceComparable other)
        {
            Edge[] thisArray = this.face.GetEdges();
            Edge[] otherArray = other.face.GetEdges();

            NXOpen.UF.UFSession ufsession = NXOpen.UF.UFSession.GetUFSession();
            NXOpen.UF.UFEval.Arc arc;

            IntPtr eval;
            ufsession.Eval.Initialize(thisArray[0].Tag, out eval);
            ufsession.Eval.AskArc(eval, out arc);
            double[] thiscenter1 = arc.center;

            ufsession.Eval.Initialize(otherArray[0].Tag, out eval);
            ufsession.Eval.AskArc(eval, out arc);
            double[] othercenter1 = arc.center;

            if (thiscenter1[0] > othercenter1[0])
            {
                return 1;
            }
            else if (thiscenter1[0] < othercenter1[0])
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
