using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;


namespace MolexPlugin.Model
{
    public interface IDisplayObject
    {
        void Highlight(bool highlight);
        Node Node { get; set; }
    }
}
