using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 选择体
    /// </summary>
    public class SelectionBodyRule:ClassItem,ISelectionRule
    {
        private List<Body> bodys = new List<Body>();

        public SelectionBodyRule(List<Body> body)
        {
            this.bodys = body;
        }
        public SelectionIntentRule CreateSelectionRule()
        {
            Part workPart = Session.GetSession().Parts.Work;
            try
            {
                return workPart.ScRuleFactory.CreateRuleBodyDumb(bodys.ToArray());
            }

            catch (Exception ex)
            {
                LogMgr.WriteLog("Basic.SelectionBodyRule.CreateSelectionRule:错误：" + ex.Message);
            }
            return null;
        }
    }
}
