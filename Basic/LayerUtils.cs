using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    public class LayerUtils
    {
        /// <summary>
        /// 移动到层
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="obj"></param>
        public static void MoveDisplayableObject(int layer, params NXObject[] obj)
        {
            Part workPart = Session.GetSession().Parts.Work;
            List<DisplayableObject> dis = new List<DisplayableObject>();
            foreach (NXObject nx in obj)
            {
                if (!(nx is Edge || nx is Face))
                    dis.Add(nx as DisplayableObject);
            }
            try
            {
                workPart.Layers.MoveDisplayableObjects(layer, dis.ToArray());
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("LayerUtils.MoveDisplayableObject            " + ex.Message);
            }

        }

        public static NXObject[] GetAllObjectsOnLayer(int layer)
        {
            Part workPart = Session.GetSession().Parts.Work;
            return workPart.Layers.GetAllObjectsOnLayer(layer);
        }
    }
}
