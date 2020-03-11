using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 移动
    /// </summary>
    public class MoveObject
    {
        /// <summary>
        /// 按坐标到坐标移动工件
        /// </summary>
        /// <param name="csys"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static NXObject MoveObjectOfCsys(CoordinateSystem csys, params NXObject[] objs)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            Point3d originAbs = new Point3d(0, 0, 0);
            Part workPart = Session.GetSession().Parts.Work;
            CoordinateSystem abs = workPart.CoordinateSystems.CreateCoordinateSystem(originAbs, mat.GetMatrix3(), false);
            NXOpen.Features.MoveObject nullMoveObject = null;
            NXOpen.Features.MoveObjectBuilder moveObjectBuilder = workPart.BaseFeatures.CreateMoveObjectBuilder(nullMoveObject);
            bool added = moveObjectBuilder.ObjectToMoveObject.Add(objs);
            moveObjectBuilder.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.CsysToCsys;
            moveObjectBuilder.TransformMotion.FromCsys = csys;
            moveObjectBuilder.TransformMotion.ToCsys = abs;
            try
            {
                return moveObjectBuilder.Commit();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.MoveObject.MoveObjectOfCsys:错误：" + ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 按点到点移动工件
        /// </summary>
        /// <param name="startPt"></param>
        /// <param name="endPt"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static NXObject MoveObjectOfPointToPoint(Point3d startPt, Point3d endPt, params NXObject[] objs)
        {
            Vector3d direction = UMathUtils.GetVector(endPt, startPt);
            double value = UMathUtils.GetDis(startPt, endPt);
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Features.MoveObject nullMoveObject = null;
            NXOpen.Features.MoveObjectBuilder moveObjectBuilder = workPart.BaseFeatures.CreateMoveObjectBuilder(nullMoveObject);
            bool added = moveObjectBuilder.ObjectToMoveObject.Add(objs);
            moveObjectBuilder.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.Distance;
            Direction distance = workPart.Directions.CreateDirection(startPt, direction, SmartObject.UpdateOption.WithinModeling);
            moveObjectBuilder.TransformMotion.DistanceVector = distance;
            moveObjectBuilder.TransformMotion.DistanceValue.Value = value;
            try
            {
                return moveObjectBuilder.Commit();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.MoveObject.MoveObjectOfCsys:错误：" + ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 绕轴旋转
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static NXObject MoveObjectOfRotate(Vector3d direction, double value, params NXObject[] objs)
        {

            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Features.MoveObject nullMoveObject = null;
            Point nullNXOpen_Point = null;
            NXOpen.Features.MoveObjectBuilder moveObjectBuilder = workPart.BaseFeatures.CreateMoveObjectBuilder(nullMoveObject);
            bool added = moveObjectBuilder.ObjectToMoveObject.Add(objs);
            moveObjectBuilder.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.Angle;
            Direction distance = workPart.Directions.CreateDirection(new Point3d(0, 0, 0), direction, SmartObject.UpdateOption.WithinModeling);
            Axis axis = workPart.Axes.CreateAxis(nullNXOpen_Point, distance, SmartObject.UpdateOption.WithinModeling);
            moveObjectBuilder.TransformMotion.DistanceVector = distance;
            moveObjectBuilder.TransformMotion.AngularAxis = axis;
            moveObjectBuilder.TransformMotion.Angle.Value = value;
            try
            {
                return moveObjectBuilder.Commit();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.MoveObject.MoveObjectOfCsys:错误：" + ex.Message);
            }
            return null;
        }

        // <summary>
        /// 按x，y，z 移动工件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="moveX"></param>
        /// <param name="moveY"></param>
        /// <param name="moveZ"></param>
        /// <returns></returns>
        public static NXObject CreateMoveObjToXYZ(string moveX, string moveY, string moveZ, NXOpen.Features.MoveObject moveFeater = null, params NXObject[] objs)
        {
            Part workPart = Session.GetSession().Parts.Work;

            NXOpen.Features.MoveObjectBuilder moveObjectBuilder1;
            moveObjectBuilder1 = workPart.BaseFeatures.CreateMoveObjectBuilder(moveFeater);

            moveObjectBuilder1.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.DeltaXyz; //增量xyz
            moveObjectBuilder1.TransformMotion.AlongCurveAngle.AlongCurve.IsPercentUsed = true;
            moveObjectBuilder1.TransformMotion.DeltaEnum = NXOpen.GeometricUtilities.ModlMotion.Delta.ReferenceAcsWorkPart;

            moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = moveX; //x增量

            moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = moveY; //y增量

            moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = moveZ;//增量
            moveObjectBuilder1.MoveParents = false;
            moveObjectBuilder1.Associative = true;
            bool added1;
            added1 = moveObjectBuilder1.ObjectToMoveObject.Add(objs);
            NXOpen.NXObject nXObject1 = null;
            try
            {

                nXObject1 = moveObjectBuilder1.Commit();

            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("MoveObject:CreateMoveObjToXYZ:      " + ex.Message);

            }
            finally
            {
                moveObjectBuilder1.Destroy();
            }
            return nXObject1;

        }

    }
}
