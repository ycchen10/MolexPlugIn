using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 选择点
    /// </summary>
    public class SelectionCurveFromPointRule : ClassItem,ISelectionRule
    {
        private List<Point> points = new List<Point>();

        public SelectionCurveFromPointRule(List<Point> point)
        {
            this.points = point;
        }
        public SelectionIntentRule CreateSelectionRule()
        {
            Part workPart = Session.GetSession().Parts.Work;
            try
            {
                return workPart.ScRuleFactory.CreateRuleCurveDumbFromPoints(points.ToArray());
            }

            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.SelectionFaceFromPointRule.CreateSelectionRule:错误：" + ex.Message);
            }
            return null;
        }
    }
}
