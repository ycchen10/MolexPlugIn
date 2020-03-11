using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;

namespace Basic
{

    public class UMathUtils
    {
        public const double FLOAT_TOL = 0.001;
        public const double FLOAT_TOLZ = 1;

        /// <summary>
        /// 判定两个数字是否相等
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        public static bool IsEqual(double x1, double x2)
        {
            return Math.Abs(x1 - x2) < FLOAT_TOL;
        }

        public static bool IsEqualZ(double x1, double x2)
        {
            return Math.Abs(x1 - x2) < FLOAT_TOLZ;
        }

        /// <summary>
        /// 判定两个点是否重合
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool IsEqual(Point3d p1, Point3d p2)
        {
            return GetDis(p1, p2) < FLOAT_TOL;
        }

        /// <summary>
        /// 判断两数之间
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xmin"></param>
        /// <param name="xmax"></param>
        /// <returns></returns>
        public static bool Between(double x, double xmin, double xmax)
        {
            return (x >= xmin && x <= xmax) || (x >= xmax && x <= xmin);
        }

        /// <summary>
        /// 模向量
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static double SelfDis(Vector3d p1)
        {
            return Math.Sqrt(p1.X * p1.X + p1.Y * p1.Y + p1.Z * p1.Z);
        }

        /// <summary>
        /// 获得两个点之间的距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDis(Point3d p1, Point3d p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y) + (p1.Z - p2.Z) * (p1.Z - p2.Z));
        }
        /// <summary>
        /// 获得两个点之间得中点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point3d GetMiddle(Point3d p1, Point3d p2)
        {
            return new Point3d((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2, (p1.Z + p2.Z) / 2);
        }
        /// <summary>
        /// 获得p1点关于P2点的对称点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point3d GetSymmetry(Point3d p1, Point3d p2)
        {
            return new Point3d(2 * p2.X - p1.X, 2 * p2.Y - p1.Y, 2 * p2.Z - p1.Z);
        }

        /// <summary>
        /// 获得两个点的矢量 p1->p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3d GetVector(NXOpen.Point3d p1, NXOpen.Point3d p2)
        {
            Vector3d vector = new Vector3d();
            vector.X = p2.X - p1.X;
            vector.Y = p2.Y - p1.Y;
            vector.Z = p2.Z - p1.Z;

            return GetNorm(vector);

        }

        /// <summary>
        ///正交
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3d GetNorm(NXOpen.Vector3d vec)
        {
            double dis = Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);

            Vector3d res = new Vector3d();
            res.X = vec.X / dis;
            res.Y = vec.Y / dis;
            res.Z = vec.Z / dis;

            return res;
        }


        /// <summary>
        /// 矢量点乘
        /// </summary>
        /// <param name="vec1">矢量1</param>
        /// <param name="vec2">矢量2</param>
        /// <returns></returns>
        public static double VectorDot(NXOpen.Vector3d vec1, NXOpen.Vector3d vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }


        /// <summary>
        /// 矢量差乘
        /// </summary>
        /// <param name="vec1">矢量1</param>
        /// <param name="vec2">矢量2</param>
        /// <returns>差乘矢量</returns>
        public static NXOpen.Vector3d VectorCross(NXOpen.Vector3d vec1, NXOpen.Vector3d vec2)
        {
            NXOpen.UF.UFSession ufSession = NXOpen.UF.UFSession.GetUFSession();
            double[] x = { vec1.X, vec1.Y, vec1.Z };
            double[] y = { vec2.X, vec2.Y, vec2.Z };
            double[] z = new double[3];
            ufSession.Vec3.Cross(x, y, z);
            NXOpen.Vector3d zVec = new NXOpen.Vector3d(z[0], z[1], z[2]);

            return zVec;
        }

        /// <summary>
        /// 矢量相减
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <param name="resVec"></param>
        public static void VectorSubstract(NXOpen.Vector3d vec1, NXOpen.Vector3d vec2, ref NXOpen.Vector3d resVec)
        {
            resVec.X = vec1.X - vec2.X;
            resVec.Y = vec1.Y - vec2.Y;
            resVec.Z = vec1.Z - vec2.Z;
        }

        /// <summary>
        /// 角度
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Angle(Vector3d p1, Vector3d p2)
        {
            double val = VectorDot(p1, p2) / (SelfDis(p1) * SelfDis(p2));
            if (val > 1)
                val = 1;
            if (val < -1)
                val = -1;

            return Math.Acos(val);
        }


        public static UVector ToUVector(Vector3d vec)
        {
            UVector uvec = new UVector();
            uvec.X = vec.X;
            uvec.Y = vec.Y;
            uvec.Z = vec.Z;

            return uvec;
        }

        public static UVector ToUVector(Point3d pos)
        {
            UVector uvec = new UVector();
            uvec.X = pos.X;
            uvec.Y = pos.Y;
            uvec.Z = pos.Z;

            return uvec;
        }

        /// <summary>
        /// 数字经纬度和度分秒经纬度转换 (Digital degree of latitude and longitude and vehicle to latitude and longitude conversion)
        /// </summary>
        /// <param name="digitalDegree">数字经纬度</param>
        /// <return>度分秒经纬度</return>
        static public string ConvertDigitalToDegrees(double digitalDegree)
        {
            const double num = 60;
            int degree = (int)digitalDegree;
            double tmp = (digitalDegree - degree) * num;
            int minute = (int)tmp;
            double second = Math.Round((tmp - minute) * num);

            string secStr = "";
            if (second != 0)
            {
                secStr = second + "″";
            }

            string minStr = "";
            if (minute != 0 || second != 0)
            {
                minStr = minute + "′";
            }

            return degree + "°" + minStr + secStr;
        }

        /// <summary>
        /// 度分秒经纬度(必须含有'°')和数字经纬度转换
        /// </summary>
        /// <param name="digitalDegree">度分秒经纬度</param>
        /// <return>数字经纬度</return>
        static public double ConvertDegreesToDigital(string degrees)
        {
            const double num = 60;
            double digitalDegree = 0.0;
            int d = degrees.IndexOf('°');           //度的符号对应的 Unicode 代码为：00B0[1]（六十进制），显示为°。
            if (d < 0)
            {
                return digitalDegree;
            }
            string degree = degrees.Substring(0, d);
            digitalDegree += Convert.ToDouble(degree);

            int m = degrees.IndexOf('′');           //分的符号对应的 Unicode 代码为：2032[1]（六十进制），显示为′。
            if (m < 0)
            {
                return digitalDegree;
            }
            string minute = degrees.Substring(d + 1, m - d - 1);
            digitalDegree += ((Convert.ToDouble(minute)) / num);

            int s = degrees.IndexOf('″');           //秒的符号对应的 Unicode 代码为：2033[1]（六十进制），显示为″。
            if (s < 0)
            {
                return digitalDegree;
            }
            string second = degrees.Substring(m + 1, s - m - 1);
            digitalDegree += (Convert.ToDouble(second) / (num * num));

            return digitalDegree;
        }


        public static double[] Point3dToArray(Point3d pt)
        {
            double[] pa = new double[3];
            pa[0] = pt.X;
            pa[1] = pt.Y;
            pa[2] = pt.Z;

            return pa;
        }
    }
}
