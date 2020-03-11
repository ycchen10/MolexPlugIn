using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 最大边界盒
    /// </summary>
    public class BoundingBoxUtils:ClassItem
    {
        /// <summary>
        /// 最大外形
        /// </summary>
        /// <param name="nxobj"></param>
        /// <param name="cs">坐标</param>
        /// <param name="mat">坐标矩阵</param>
        /// <param name="centerPt">中心点</param>
        /// <param name="disPt">外形一半</param>
        public static void GetBoundingBoxInLocal(NXObject[] nxobj, CartesianCoordinateSystem cs, Matrix4 mat, ref Point3d centerPt, ref Point3d disPt)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            NXOpen.UF.UFSession theUFSession = NXOpen.UF.UFSession.GetUFSession();
            Tag temp=Tag.Null;
            if(cs!=null)
            {
                temp = cs.Tag;
            }
            else
            {
                cs = workPart.WCS.CoordinateSystem;
            }
            
            if(nxobj[0]==null)
            {
                LogMgr.WriteLog("BoundingBoxUtils.GetBoundingBoxInLocal 传入参数为空！");
                return;
            }
            double[] minCorner = new double[3];            
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];
            double[] box = new double[6];
            double[] max = new double[6];
            double[] center = new double[3];
            double[] centerAbs = new double[3];

            theUFSession.Modl.AskBoundingBoxExact(nxobj[0].Tag, temp, minCorner, directions, distances);  //查询最大边界盒
            mat.ApplyPos(ref minCorner[0], ref minCorner[1], ref minCorner[2]);
            box[0] = minCorner[0];
            box[1] = minCorner[1];
            box[2] = minCorner[2];
            box[3] = minCorner[0] + distances[0];
            box[4] = minCorner[1] + distances[1];
            box[5] = minCorner[2] + distances[2];
            if (nxobj.Length >= 2)
            {
                for (int i = 1; i < nxobj.Length; i++)
                {
                    theUFSession.Modl.AskBoundingBoxExact(nxobj[i].Tag, temp, minCorner, directions, distances);
                    mat.ApplyPos(ref minCorner[0], ref minCorner[1], ref minCorner[2]);
                    max[0] = minCorner[0];
                    max[1] = minCorner[1];
                    max[2] = minCorner[2];
                    max[3] = minCorner[0] + distances[0];
                    max[4] = minCorner[1] + distances[1];
                    max[5] = minCorner[2] + distances[2];
                    if (max[0] < box[0])
                    {
                        box[0] = max[0];
                    }
                    if (max[1] < box[1])
                    {
                        box[1] = max[1];
                    }
                    if (max[2] < box[2])
                    {
                        box[2] = max[2];
                    }
                    if (max[3] > box[3])
                    {
                        box[3] = max[3];
                    }
                    if (max[4] > box[4])
                    {
                        box[4] = max[4];
                    }
                    if (max[5] > box[5])
                    {
                        box[5] = max[5];
                    }
                }
            }
            centerPt.X = 0.5 * (box[0] + box[3]);
            centerPt.Y = 0.5 * (box[1] + box[4]);
            centerPt.Z = 0.5 * (box[2] + box[5]);

            disPt.X = 0.5 * (box[3] - box[0]);
            disPt.Y = 0.5 * (box[4] - box[1]);
            disPt.Z = 0.5 * (box[5] - box[2]);

        }
        /// <summary>
        /// 创建坐标系
        /// </summary>
        /// <param name="mat">矩阵</param>
        /// <param name="inversmat">逆矩阵</param>
        /// <returns></returns>
        public static CartesianCoordinateSystem CreateCoordinateSystem(Matrix4 mat, Matrix4 inversmat)
        {
            Point3d pt = new Point3d(0, 0, 0);

            inversmat.ApplyPos(ref pt);

            return CreateCoordinateSystem(pt, mat.GetXAxis(), mat.GetYAxis(), mat.GetZAxis());
        }

        public static CartesianCoordinateSystem CreateCoordinateSystem(Point3d ori, Vector3d xVec, Vector3d yVec, Vector3d zVec)
        {
            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part workPart = theSession.Parts.Work;

            Matrix3x3 mat = new Matrix3x3();
            mat.Xx = xVec.X;
            mat.Xy = xVec.Y;
            mat.Xz = xVec.Z;

            mat.Yx = yVec.X;
            mat.Yy = yVec.Y;
            mat.Yz = yVec.Z;

            //Vector3d zVec = UMathUtils.VectorCross(xVec, yVec);

            mat.Zx = zVec.X;
            mat.Zy = zVec.Y;
            mat.Zz = zVec.Z;

            try
            {
                return workPart.CoordinateSystems.CreateCoordinateSystem(ori, mat, true);
            }
            catch (Exception ex)
            {
                return workPart.CoordinateSystems.CreateCoordinateSystem(ori, xVec, yVec);
            }
        }

    }
}
