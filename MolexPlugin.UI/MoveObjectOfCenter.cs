using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using MolexPlugin.DAL;

namespace MolexPlugin
{
    public class MoveObjectOfCenter
    {
        private string uiName;

        public MoveObjectOfCenter(string name)
        {
            this.uiName = name;
        }

        public void Create()
        {
            MoveObjectFactory.CreateMove(this.uiName);
        }

    }
}
