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
    /// 草绘
    /// </summary>
    public class SketchUtils : ClassItem
    {
        /// <summary>
        /// 创建草绘环境
        /// </summary>
        /// <param name="mat">矩阵</param>
        /// <param name="name">名字</param>
        /// <returns></returns>
        public static Tag CreateShetch(double z, string name)
        {
            Tag shetchTag = Tag.Null;
            string sketch = "SKETCH_001";
            int option = 2;

            double[] matrix = { 1, 0, 0, 0, 1, 0, 0, 0, z };
            Tag[] obj = new Tag[2];
            int[] reference = new int[2];
            int planeDir = 1;
            theUFSession.Sket.InitializeSketch(ref name, out shetchTag);
            theUFSession.Sket.CreateSketch(sketch, option, matrix, obj, reference, planeDir, out shetchTag);
            Session.UndoMarkId markId = theSession.GetNewestUndoMark(Session.MarkVisibility.Visible);
            theSession.DeleteUndoMark(markId, "");
            return shetchTag;
        }

        /// <summary>
        /// 添加到草绘中
        /// </summary>
        /// <param name="addObj"></param>
        /// <param name="shetchTag"></param>
        public static void AddShetch(Tag shetchTag, params Tag[] addObj)
        {
            theUFSession.Sket.AddObjects(shetchTag, addObj.Length, addObj);
        }
        public static void AddShetch(Tag shetchTag, params NXObject[] addObj)
        {
            List<Tag> temp = new List<Tag>();
            foreach (NXObject obj in addObj)
            {
                temp.Add(obj.Tag);
            }
            theUFSession.Sket.AddObjects(shetchTag, addObj.Length, temp.ToArray());
        }
        /// <summary>
        /// 创建尺寸约束
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="dimOrigin"></param>
        /// <param name="method"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Expression CreateDimForLine(Line line1, Line line2, double[] dimOrigin, NXOpen.Annotations.DimensionMeasurementBuilder.MeasurementMethod method, InferSnapType.SnapType type)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Annotations.Dimension nullNXOpen_Annotations_Dimension = null;
            NXOpen.SketchLinearDimensionBuilder sketchLinearDimensionBuilder1;
            sketchLinearDimensionBuilder1 = workPart.Sketches.CreateLinearDimensionBuilder(nullNXOpen_Annotations_Dimension);
            sketchLinearDimensionBuilder1.Origin.SetInferRelativeToGeometry(true); //如果未应用其他关联性，则在提交时推断关联几何关系
            sketchLinearDimensionBuilder1.Measurement.Method = method;  //测量方向
            sketchLinearDimensionBuilder1.Driving.DrivingMethod = NXOpen.Annotations.DrivingValueBuilder.DrivingValueMethod.Driving;//驱动方法
            sketchLinearDimensionBuilder1.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.XyPlane;   //平面类型
            NXOpen.Direction nullNXOpen_Direction = null;
            sketchLinearDimensionBuilder1.Measurement.Direction = nullNXOpen_Direction;  //测量方向
            NXOpen.View nullNXOpen_View = null;
            sketchLinearDimensionBuilder1.Measurement.DirectionView = nullNXOpen_View; //测量视图
            sketchLinearDimensionBuilder1.Style.DimensionStyle.NarrowDisplayType = NXOpen.Annotations.NarrowDisplayOption.None; //设置窄尺寸样式
            NXOpen.Point3d point2_3 = new NXOpen.Point3d(0.0, 0.0, 0.0);
            sketchLinearDimensionBuilder1.SecondAssociativity.SetValue(NXOpen.InferSnapType.SnapType.Mid, line2, workPart.ModelingViews.WorkView, line2.EndPoint, null, nullNXOpen_View, point2_3);

            NXOpen.Point3d point2_4 = new NXOpen.Point3d(0.0, 0.0, 0.0);
            sketchLinearDimensionBuilder1.FirstAssociativity.SetValue(type, line1, workPart.ModelingViews.WorkView, line1.EndPoint, null, nullNXOpen_View, point2_4);
            NXOpen.Point3d point3 = new NXOpen.Point3d(dimOrigin[0], dimOrigin[1], dimOrigin[2]);
            sketchLinearDimensionBuilder1.Origin.Origin.SetValue(null, nullNXOpen_View, point3);

            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = sketchLinearDimensionBuilder1.Commit();
                Expression exp = sketchLinearDimensionBuilder1.Driving.ExpressionValue;
                return exp;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("SketchUtils:CreateDimForLine:" + ex.Message);
                return null;
            }
            finally
            {
                sketchLinearDimensionBuilder1.Destroy();

            }

        }


        public static Expression CreateDim(NXObject obj1, NXObject obj2, Point3d dimOrigin, NXOpen.Annotations.DimensionMeasurementBuilder.MeasurementMethod method, InferSnapType.SnapType type)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Annotations.Dimension nullNXOpen_Annotations_Dimension = null;
            NXOpen.SketchLinearDimensionBuilder sketchLinearDimensionBuilder1;
            sketchLinearDimensionBuilder1 = workPart.Sketches.CreateLinearDimensionBuilder(nullNXOpen_Annotations_Dimension);
            sketchLinearDimensionBuilder1.Origin.SetInferRelativeToGeometry(true); //如果未应用其他关联性，则在提交时推断关联几何关系
            sketchLinearDimensionBuilder1.Measurement.Method = method;  //测量方向
            sketchLinearDimensionBuilder1.Driving.DrivingMethod = NXOpen.Annotations.DrivingValueBuilder.DrivingValueMethod.Driving;//驱动方法
            sketchLinearDimensionBuilder1.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.XyPlane;   //平面类型
            NXOpen.Direction nullNXOpen_Direction = null;
            sketchLinearDimensionBuilder1.Measurement.Direction = nullNXOpen_Direction;  //测量方向
            NXOpen.View nullNXOpen_View = null;
            sketchLinearDimensionBuilder1.Measurement.DirectionView = nullNXOpen_View; //测量视图
            sketchLinearDimensionBuilder1.Style.DimensionStyle.NarrowDisplayType = NXOpen.Annotations.NarrowDisplayOption.None; //设置窄尺寸样式
            Point3d point1 = new Point3d(0.0, 0.0, 0.0);
            sketchLinearDimensionBuilder1.SecondAssociativity.SetValue(type, obj2, null, point1, null, nullNXOpen_View, point1);
            sketchLinearDimensionBuilder1.FirstAssociativity.SetValue(type, obj1, null, point1, null, nullNXOpen_View, point1);
            sketchLinearDimensionBuilder1.Origin.Origin.SetValue(null, nullNXOpen_View, dimOrigin);
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = sketchLinearDimensionBuilder1.Commit();
                Expression exp = sketchLinearDimensionBuilder1.Driving.ExpressionValue;
                return exp;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("SketchUtils:CreateDimForLine:" + ex.Message);
                return null;
            }
            finally
            {
                sketchLinearDimensionBuilder1.Destroy();

            }

        }

        /// <summary>
        /// 创建位置约束
        /// </summary>
        /// <param name="obj1">要约束的对象</param>
        /// <param name="obj2">约束到的对象</param>
        /// <param name="type">约束类型</param>
        public static void CreateConstraint(NXObject obj2, SketchConstraintBuilder.Constraint type, params NXObject[] obj1)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.SketchConstraintBuilder sketchConstraintBuilder1;
            sketchConstraintBuilder1 = workPart.Sketches.CreateConstraintBuilder();
            sketchConstraintBuilder1.ConstraintType = type;
            sketchConstraintBuilder1.GeometryToConstrain.SetArray(obj1);
            sketchConstraintBuilder1.GeometryToConstrainTo.Value = obj2;
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = sketchConstraintBuilder1.Commit();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("SketchUtils:CreateConstraint:" + ex.Message);
            }
            finally
            {
                sketchConstraintBuilder1.Destroy();
            }

        }
        /// <summary>
        /// 设置固定
        /// </summary>
        /// <param name="objs"></param>
        public static void SetFixed(params NXObject[] objs)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.SketchConstraintBuilder sketchConstraintBuilder1;
            sketchConstraintBuilder1 = workPart.Sketches.CreateConstraintBuilder();


            bool added1;
            added1 = sketchConstraintBuilder1.GeometryToConstrain.Add(objs);

            sketchConstraintBuilder1.ConstraintType = NXOpen.SketchConstraintBuilder.Constraint.Fixed;
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = sketchConstraintBuilder1.Commit();
                NXOpen.NXObject[] objects1;
                objects1 = sketchConstraintBuilder1.GetCommittedObjects();
            }

            catch (Exception ex)
            {
                LogMgr.WriteLog("SketchUtils:SetFixed:" + ex.Message);
            }

            finally
            {
                sketchConstraintBuilder1.Destroy();
            }


        }
    }
}
