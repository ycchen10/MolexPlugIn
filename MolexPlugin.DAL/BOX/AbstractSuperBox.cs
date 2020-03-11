using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.BlockStyler;
using Basic;

namespace MolexPlugin.DAL
{
    public abstract class AbstractSuperBox
    {
        protected List<NXObject> selectionObj = new List<NXObject>();

        public double[] Offset { get; protected set; }
        public Point3d CenterPt { get; set; }

        public Point3d disPt;

        protected Part workPart;
        public Matrix4 Matr { get; set; }
        public NXOpen.Features.ToolingBox ToolingBox { get; protected set; } = null;

        public AbstractSuperBox(double[] offset, List<NXObject> nxObjects)
        {
            workPart = Session.GetSession().Parts.Work;
            this.Offset = offset;
            this.selectionObj = nxObjects;
            GetBoundingBox();
            this.Matr = GetMatrix();
        }

        /// <summary>
        /// 获取中心点
        /// </summary>
        protected void GetBoundingBox()
        {
            CoordinateSystem wcs = workPart.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToCsys(wcs, ref mat);
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            BoundingBoxUtils.GetBoundingBoxInLocal(selectionObj.ToArray(), null, mat, ref centerPt, ref disPt);
            this.CenterPt = centerPt;
            this.disPt = disPt;
        }
        /// <summary>
        /// 删除特征
        /// </summary>
        public void DeleToolingBoxFeatures()
        {
            DeleteObject.Delete(ToolingBox);
        }
        /// <summary>
        /// 获取矩阵
        /// </summary>
        /// <returns></returns>
        protected Matrix4 GetMatrix()
        {
            Vector3d xDir;
            Vector3d yDir;
            workPart.WCS.CoordinateSystem.GetDirections(out xDir, out yDir);
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(this.CenterPt, xDir, yDir);
            return mat;
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(int color)
        {

            UFSession theUFSession = UFSession.GetUFSession();
            if (this.ToolingBox != null)
                theUFSession.Obj.SetColor(this.ToolingBox.GetBodies()[0].Tag, color);
        }
        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="tl"></param>
        public void SetTranslucency(int tl)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            if (this.ToolingBox != null)
                theUFSession.Obj.SetTranslucency(this.ToolingBox.GetBodies()[0].Tag, tl);
        }
        /// <summary>
        /// 设置层
        /// </summary>
        /// <param name="layer"></param>
        public void SetLayer(int layer)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            if (this.ToolingBox != null)
                theUFSession.Obj.SetLayer(this.ToolingBox.GetBodies()[0].Tag, layer);
        }
        /// <summary>
        /// 创建特征
        /// </summary>
        public abstract void CreateSuperBox();

        /// <summary>
        /// 更新方位
        /// </summary>
        /// <param name="spec"></param>
        public abstract void UpdateSpecify(UIBlock ui);

        /// <summary>
        /// 关联线性向量到体面上
        /// </summary>
        /// <param name="ld">控件</param
        /// <param name="vec">向量</param>
        public abstract void SetDimForFace(ref LinearDimension ld, Vector3d vec);

        public abstract void Update(Matrix4 matr, double[] offset);


        public void Update(List<NXObject> nxobjects)
        {
            this.selectionObj = nxobjects;
            GetBoundingBox();
            this.Matr = GetMatrix();
            CreateSuperBox();
        }

    }
}
