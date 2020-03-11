using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 选择边
    /// </summary>
    public class SelectionEdgeRule : ClassItem,ISelectionRule
    {
        private List<Edge> edges = new List<Edge>();

        public SelectionEdgeRule(List<Edge> edge)
        {
            this.edges = edge;
        }
        public SelectionIntentRule CreateSelectionRule()
        {
            Part workPart = Session.GetSession().Parts.Work;
            try
            {
                return workPart.ScRuleFactory.CreateRuleEdgeDumb(edges.ToArray());
            }

            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.SelectionEdgeRule.CreateSelectionRule:错误：" + ex.Message);
            }
            return null;
        }
    }
}
