using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 选择线
    /// </summary>
    public class SelectionCurveRule:ClassItem,ISelectionRule
    {
        private List<Curve> curves = new List<Curve>();

        public SelectionCurveRule(List<Curve> curve)
        {
            this.curves = curve;
        }
        public SelectionIntentRule CreateSelectionRule()
        {
            Part workPart = Session.GetSession().Parts.Work;
            try
            {
                return workPart.ScRuleFactory.CreateRuleCurveDumb(curves.ToArray());
            }

            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.SelectionCurveRule.CreateSelectionRule:错误：" + ex.Message);
            }
            return null;
        }
    }
}
