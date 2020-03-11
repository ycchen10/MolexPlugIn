using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    public class PlaneUtils : ClassItem
    {
        /// <summary>
        /// 以面数据创建平面
        /// </summary>
        /// <param name="planeFace"></param>
        /// <returns></returns>
        public static NXOpen.Plane CreatePlaneOfFace(Face face,bool flip)
        {
            Part workPart = theSession.Parts.Work;
            Point3d originPt;
            Vector3d normal;
            FaceUtils.AskFaceOriginAndNormal(face, out originPt, out normal);
            NXOpen.Plane plane1 = workPart.Planes.CreatePlane(originPt, normal, NXOpen.SmartObject.UpdateOption.WithinModeling);
            plane1.SetFlip(flip);
            return plane1;
        }



    }
}
