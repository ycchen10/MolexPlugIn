using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.Features;

namespace Basic
{
    /// <summary>
    /// 工件体特征
    /// </summary>
    public class ToolingBoxFeature : ClassItem
    {
        /// <summary>
        /// 创建方块盒
        /// </summary>
        /// <param name="matr">矩阵</param>
        /// <param name="centerPt">中心坐标</param>
        /// <param name="offset">偏置 offset[6]</param>
        /// <param name="toolingBoxFeature">方块盒特征</param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static ToolingBox CreateToolingBlockBox(Matrix3x3 matr, Point3d centerPt, double[] offset, ToolingBox toolingBoxFeature = null, params TaggedObject[] objs)
        {
            Part workPart = theSession.Parts.Work;
            SelectionRuleFactory rules = new SelectionRuleFactory(objs.ToList());
            //ToolingBox nullToolingBox = null;
            ToolingBoxBuilder toolingBoxBuilder = workPart.Features.ToolingFeatureCollection.CreateToolingBoxBuilder(toolingBoxFeature);
            toolingBoxBuilder.Type = ToolingBoxBuilder.Types.BoundedBlock;
            toolingBoxBuilder.OffsetPositiveX.Value = offset[0];
            toolingBoxBuilder.OffsetNegativeX.Value = offset[1];
            toolingBoxBuilder.OffsetPositiveY.Value = offset[2];
            toolingBoxBuilder.OffsetNegativeY.Value = offset[3];
            toolingBoxBuilder.OffsetPositiveZ.Value = offset[4];
            toolingBoxBuilder.OffsetNegativeZ.Value = offset[5];
            toolingBoxBuilder.SingleOffset = false;
            toolingBoxBuilder.SetBoxMatrixAndPosition(matr, centerPt);
            ScCollector scCollector = toolingBoxBuilder.BoundedObject;
            scCollector.ReplaceRules(rules.CreateSelectionRule().ToArray(), false);
            toolingBoxBuilder.CalculateBoxSize();
            NXOpen.Session.UndoMarkId markId = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start ToolingBox");
            try
            {
                return toolingBoxBuilder.CommitFeature() as ToolingBox;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.ToolingFeature.CreateToolingBox:错误：" + ex.Message);
                return null;
            }
            finally
            {
                toolingBoxBuilder.Destroy();
                theSession.UpdateManager.DoUpdate(markId);
                theSession.DeleteUndoMark(markId, "End ToolingBox");
            }

        }
        /// <summary>
        /// 创建圆柱特征
        /// </summary>
        /// <param name="zAxis">轴</param>
        /// <param name="centerPt">中心点</param>
        /// <param name="offset"></param>
        /// <param name="toolingBoxFeature"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static ToolingBox CreateToolingCylinder(Vector3d zAxis, Point3d centerPt, double[] offset, ToolingBox toolingBoxFeature = null, params TaggedObject[] objs)
        {
            Part workPart = theSession.Parts.Work;
            SelectionRuleFactory rules = new SelectionRuleFactory(objs.ToList());
            //ToolingBox nullToolingBox = null;
            ToolingBoxBuilder toolingBoxBuilder = workPart.Features.ToolingFeatureCollection.CreateToolingBoxBuilder(toolingBoxFeature);
            toolingBoxBuilder.Type = ToolingBoxBuilder.Types.BoundedCylinder;
            Direction dir = workPart.Directions.CreateDirection(centerPt, zAxis, SmartObject.UpdateOption.WithinModeling);
            toolingBoxBuilder.AxisVector = dir;
            toolingBoxBuilder.RadialOffset.Value = offset[2];
            toolingBoxBuilder.OffsetPositiveZ.Value = offset[0];
            toolingBoxBuilder.OffsetNegativeZ.Value = offset[1];
            // toolingBoxBuilder.SingleOffset = false;
            ScCollector scCollector = toolingBoxBuilder.BoundedObject;
            scCollector.ReplaceRules(rules.CreateSelectionRule().ToArray(), false);
            toolingBoxBuilder.CalculateBoxSize();

            NXObject[] selections = new NXObject[1];
            NXObject[] deselections = new NXObject[1];
            selections[0] = (NXObject)objs[0];
            toolingBoxBuilder.SetSelectedOccurrences(selections, deselections);
            toolingBoxBuilder.CalculateBoxSize();

            NXOpen.Session.UndoMarkId markId = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start ToolingBox");
            try
            {
                return toolingBoxBuilder.CommitFeature() as ToolingBox;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.ToolingFeature.CreateToolingBox:错误：" + ex.Message);
                return null;
            }
            finally
            {
                toolingBoxBuilder.Destroy();
                theSession.UpdateManager.DoUpdate(markId);
                theSession.DeleteUndoMark(markId, "End ToolingBox");
            }

        }
    }
}
