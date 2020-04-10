using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace Basic
{
    /// <summary>
    /// 射线
    /// </summary>
    public class TraceARay
    {
        /// <summary>
        /// 射线（不包括面自己）
        /// </summary>
        /// <param name="body">体</param>
        /// <param name="pt">面上的点</param>   
        /// <param name="vec">向量</param>
        /// <returns></returns>
        public static int AskTraceARay(Body body, Point3d pt, Vector3d vec)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag[] bodyTag = { body.Tag };
            UFModl.RayHitPointInfo[] info;
            double[] origin = { pt.X, pt.Y, pt.Z };
            double[] dir = { vec.X, vec.Y, vec.Z };
            double[] mat = new double[16];
            theUFSession.Mtx4.Identity(mat);
            int res = 0;
            int count = 0;
            theUFSession.Modl.TraceARay(1, bodyTag, origin, dir, mat, 0, out res, out info);
            return count;
        }
        /// <summary>
        /// 面上任意一点做射线
        /// </summary>
        /// <param name="face"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static int AskTraceARayForFaceData(Face face, Vector3d vec)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag[] bodyTag = { face.GetBody().Tag };
            UFModl.RayHitPointInfo[] info;
            Point3d originPt = GetFacePoint(face);
            double[] origin = { originPt.X, originPt.Y, originPt.Z };
            double[] dir = { vec.X, vec.Y, vec.Z };
            double[] mat = new double[16];
            theUFSession.Mtx4.Identity(mat);
            int res = 0;
            int count = 0;
            theUFSession.Modl.TraceARay(1, bodyTag, origin, dir, mat, 0, out res, out info);
            foreach (UFModl.RayHitPointInfo ray in info)
            {
                Point3d temp = new Point3d(ray.hit_point[0], ray.hit_point[1], ray.hit_point[2]);
                double dis = UMathUtils.GetDis(originPt, temp);
                if (ray.hit_face != face.Tag && !UMathUtils.IsEqual(dis, 0))
                {

                    int statusPt = 0;
                    theUFSession.Modl.AskPointContainment(ray.hit_point, face.Tag, out statusPt);                
                    if (statusPt != 3)
                    {
                        count++;
                    }

                }
            }
            return count;
        }
        /// <summary>
        /// 获取面上的点
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        private static Point3d GetFacePoint(Face face)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            int statusPt;
            double[] point;
            FaceData data = FaceUtils.AskFaceData(face);
            point = new double[3] { data.Point.X, data.Point.Y, data.Point.Z };
            theUFSession.Modl.AskPointContainment(point, face.Tag, out statusPt);
            if (statusPt == 1)
                return data.Point;
            else
            {
                double[] uvs = new double[4];
                theUFSession.Modl.AskFaceUvMinmax(face.Tag, uvs); //获得面u,v参数空间（u,v最小,最大值）
                double[] param = new double[2];                           //输入U,V方向值

                for (int i = 1; i < 6; i++)
                {
                    param[0] = i * (uvs[1] - uvs[0]) / 5 + uvs[0];
                    param[1] = i * (uvs[3] - uvs[2]) / 5 + uvs[2];
                    double[] point1 = new double[3];        //输出点坐标
                    double[] u1 = new double[3];           //输出 输出一阶导数在U位置
                    double[] v1 = new double[3];           //输出 输出一阶导数在V位置
                    double[] u2 = new double[3];           //输出 输出二阶导数在U位置
                    double[] v2 = new double[3];           //输出 输出二阶导数在V位置
                    double[] unit_norm = new double[3];    //输出面上该点的矢量方向
                    double[] radii = new double[2];        //输出，双半径，输出主曲率半径
                    theUFSession.Modl.AskFaceProps(face.Tag, param, point1, u1, v1, u2, v2, unit_norm, radii);
                    theUFSession.Modl.AskPointContainment(point1, face.Tag, out statusPt);
                    if (statusPt == 1)
                        return new Point3d(point1[0], point1[1], point1[2]);
                }

            }
            return new Point3d();
        }
    }
}
