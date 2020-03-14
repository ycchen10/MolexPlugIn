using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 电极表达式
    /// </summary>
    public class ElectrodeExp
    {
        public static void SetAttrExp()
        {
            ExpressionUtils.SetAttrExp("PitchX", "PitchX", NXObject.AttributeType.Real);
            ExpressionUtils.SetAttrExp("PitchXNum", "PitchXNum", NXObject.AttributeType.Integer);
            ExpressionUtils.SetAttrExp("PitchY", "PitchY", NXObject.AttributeType.Real);
            ExpressionUtils.SetAttrExp("PitchYNum", "PitchYNum", NXObject.AttributeType.Integer);
            ExpressionUtils.SetAttrExp("PreparationX", "Preparation", NXObject.AttributeType.Integer, 0);
            ExpressionUtils.SetAttrExp("PreparationY", "Preparation", NXObject.AttributeType.Integer, 1);
            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("xNCopies"), "PitchXNum");
            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("yNCopies"), "PitchYNum");
            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("xPitchDistance"), "PitchX");
            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("yPitchDistance"), "-PitchY");
        }

        public static void SetMoveExp(double anlge, int[] pre, bool zDatum)
        {
            if (zDatum)
            {
                SetMoveExpForZDatum(pre, anlge);
            }
            else
            {
                SetMoveExpForNoDatum(anlge);
            }
        }
        private static void SetMoveExpForZDatum(int[] pre, double anlge)
        {
            Expression moveXExp = ExpressionUtils.CreateExp("moveX=0", "Number");
            Expression moveBoxXExp = ExpressionUtils.CreateExp("moveBoxX=0", "Number");
            Expression moveBoxYExp = ExpressionUtils.CreateExp("moveBoxY=0", "Number");
            Expression moveYExp = ExpressionUtils.CreateExp("moveY=0", "Number");
            ExpressionUtils.CreateExp("moveZ=0", "Number");
            ExpressionUtils.CreateExp("moveBoxZ=0", "Number");
            if (pre[0] > pre[1])
            {
                ExpressionUtils.EditExp(moveXExp, "-(xNCopies-2)*xPitchDistance/2");
                ExpressionUtils.EditExp(moveBoxXExp, "-(xNCopies)*xPitchDistance/2");
                if (UMathUtils.IsEqual(anlge, Math.PI))
                {
                    ExpressionUtils.EditExp(moveYExp, "(yNCopies-1)*yPitchDistance/2");
                }
                else
                {
                    ExpressionUtils.EditExp(moveYExp, "-(yNCopies-1)*yPitchDistance/2");
                }
            }
            else
            {
                ExpressionUtils.EditExp(moveXExp, "-(xNCopies-1)*xPitchDistance/2");
                if (UMathUtils.IsEqual(anlge, Math.PI))
                {
                    ExpressionUtils.EditExp(moveYExp, "(yNCopies-2)*yPitchDistance/2");
                    ExpressionUtils.EditExp(moveBoxYExp, "(yNCopies)*yPitchDistance/2");
                }
                else
                {
                    ExpressionUtils.EditExp(moveYExp, "-(yNCopies-2)*yPitchDistance/2");
                    ExpressionUtils.EditExp(moveBoxYExp, "-(yNCopies)*yPitchDistance/2");
                }
            }
        }

        private static void SetMoveExpForNoDatum(double anlge)
        {
            Expression moveXExp = ExpressionUtils.CreateExp("moveX=0", "Number");
            Expression moveYExp = ExpressionUtils.CreateExp("moveY=0", "Number");
            ExpressionUtils.CreateExp("moveZ=0", "Number");
            ExpressionUtils.EditExp(moveXExp, "-(xNCopies-1)*xPitchDistance/2");
            if (UMathUtils.IsEqual(anlge, Math.PI))
            {
                ExpressionUtils.EditExp(moveYExp, "(yNCopies-1)*yPitchDistance/2");
            }
            else
            {
                ExpressionUtils.EditExp(moveYExp, "-(yNCopies-1)*yPitchDistance/2");
            }

        }
    }
}
