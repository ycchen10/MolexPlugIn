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
    public class EleConditionModel
    {
        public WorkAssembleModel Work { get; set; }
        public Vector3d Vec { get; set; }
        public List<Body> Bodys { get; set; }

    }
}
