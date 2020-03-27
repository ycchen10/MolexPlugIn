using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.CAM;


namespace Basic
{
    public class OperationUtils
    {
        /// <summary>
        /// 设置平面铣边界
        /// </summary>
        /// <param name="toolside"></param>
        /// <param name="pt"></param>
        /// <param name="boundary"></param>
        /// <param name="edges"></param>
        public static BoundarySetPlanarMill CreateBoundaryPlanarMill(BoundarySet.ToolSideTypes toolside, BoundarySet.BoundaryTypes types,
            Point3d pt, Boundary boundary, params Edge[] edges)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            Part workPart = Session.GetSession().Parts.Work;
            BoundarySetPlanarMill boundarySetPlanarMill;
            boundarySetPlanarMill = boundary.CreateBoundarySetPlanarMill();
            Vector3d normal = new NXOpen.Vector3d(0.0, 0.0, 1.0);
            Plane plane = workPart.Planes.CreatePlane(pt, normal, NXOpen.SmartObject.UpdateOption.AfterModeling);
            boundarySetPlanarMill.Plane = plane;
            boundarySetPlanarMill.ToolSide = toolside;
            boundarySetPlanarMill.PlaneType = NXOpen.CAM.BoundarySet.PlaneTypes.UserDefined;
            boundarySetPlanarMill.AppendCurves(edges, pt, mat.GetMatrix3());
            boundarySetPlanarMill.BoundaryType = types;
            return boundarySetPlanarMill;
        }
        /// <summary>
        /// 设置平面铣边界
        /// </summary>
        /// <param name="toolside"></param>
        /// <param name="pt"></param>
        /// <param name="boundary"></param>
        /// <param name="edges"></param>
        public static BoundaryMillingSet CreateBoundaryMillingSet(BoundarySet.ToolSideTypes toolside, BoundarySet.BoundaryTypes types,
            Point3d pt, Boundary boundary, params Edge[] edges)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            Part workPart = Session.GetSession().Parts.Work;
            BoundaryMillingSet boundarySet;
            boundarySet = boundary.CreateBoundaryMillingSet();
            Vector3d normal = new NXOpen.Vector3d(0.0, 0.0, 1.0);
            Plane plane = workPart.Planes.CreatePlane(pt, normal, NXOpen.SmartObject.UpdateOption.AfterModeling);
            boundarySet.Plane = plane;
            boundarySet.ToolSide = toolside;
            boundarySet.PlaneType = NXOpen.CAM.BoundarySet.PlaneTypes.UserDefined;
            boundarySet.AppendCurves(edges, pt, mat.GetMatrix3());
            boundarySet.BoundaryType = types;
            return boundarySet;
        }

    }
}
