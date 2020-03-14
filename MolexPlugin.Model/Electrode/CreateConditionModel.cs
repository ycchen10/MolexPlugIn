using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace MolexPlugin.Model
{
    /// <summary>
    /// 创建电极的条件
    /// </summary>
    public class CreateConditionModel
    {
        public WorkModel Work { get; set; }
        public string VecName { get; set; }
        public List<Body> Bodys { get; set; }

        public AssembleModel Assemble { get; set; }
    }
}
