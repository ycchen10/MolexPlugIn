using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Features;
using Basic;
using MolexPlugin.Model;


namespace MolexPlugin.DAL
{
    public class ElectrodePreveiw
    {
        public ElectrodeHeadInfo HeadModel { get; private set; }

        private PatternGeometry patternFeat;
        public ElectrodePreveiw(ElectrodeHeadInfo head)
        {
            this.HeadModel = head;
        }
        /// <summary>
        /// 创建阵列
        /// </summary>
        private void CreatePattern(double x, int xNumber, double y, int yNumber)
        {
            if ((xNumber > 1 && Math.Abs(x) > 0) || (yNumber > 1 && Math.Abs(y) > 0))
            {
                CreateExpression(x, xNumber, y, yNumber);
                this.patternFeat = PatternUtils.CreatePattern("xNCopies", "xPitchDistance", "yNCopies",
                    "yPitchDistance", this.HeadModel.ConditionModel.Work.WorkMatr, this.HeadModel.ConditionModel.Bodys.ToArray());
            }

        }
        /// <summary>
        /// 删除表达式
        /// </summary>
        private void DeleExpression()
        {
            ExpressionUtils.DeteteExp("xPitchDistance");
            ExpressionUtils.DeteteExp("xNCopies");
            ExpressionUtils.DeteteExp("yPitchDistance");
            ExpressionUtils.DeteteExp("yNCopies");
        }

        private void CreateExpression(double x, int xNumber, double y, int yNumber)
        {
            ExpressionUtils.CreateExp("xNCopies=" + xNumber.ToString(), "Number");
            ExpressionUtils.CreateExp("yNCopies=" + yNumber.ToString(), "Number");
            ExpressionUtils.CreateExp("xPitchDistance=" + x.ToString(), "Number");
            ExpressionUtils.CreateExp("yPitchDistance=" + y.ToString(), "Number");
        }
        /// <summary>
        /// 更新阵列
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xNumber"></param>
        /// <param name="y"></param>
        /// <param name="yNumber"></param>
        public void UpdatePattern(double x, int xNumber, double y, int yNumber)
        {
            if (this.patternFeat == null)
            {
                CreatePattern(x, xNumber, y, yNumber);
                return;
            }

            ExpressionUtils.UpdateExp("xPitchDistance", x.ToString());
            ExpressionUtils.UpdateExp("xNCopies", xNumber.ToString());
            ExpressionUtils.UpdateExp("yPitchDistance", y.ToString());
            ExpressionUtils.UpdateExp("yNCopies", yNumber.ToString());
            DeleteObject.UpdateObject();

        }
        /// <summary>
        /// 删除阵列
        /// </summary>
        public void DelePattern()
        {
            if (patternFeat != null)
            {
                DeleteObject.Delete(this.patternFeat);
                DeleExpression();
            }
        }
    }
}
