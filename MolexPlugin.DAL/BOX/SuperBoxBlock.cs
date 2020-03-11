using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;

namespace MolexPlugin.DAL
{
    public class SuperBoxBlock : AbstractSuperBox
    {

        public SuperBoxBlock(double[] offset, List<NXObject> nxObjects) : base(offset, nxObjects)
        {

        }

        public override void CreateSuperBox()
        {
            if ((2 * this.disPt.X + this.Offset[0] + this.Offset[1]) <= 0 ||
               (2 * this.disPt.Y + this.Offset[2] + this.Offset[3]) <= 0 ||
               (2 * this.disPt.Z + this.Offset[4] + this.Offset[5]) <= 0)
                return;
            else
                base.ToolingBox = ToolingBoxFeature.CreateToolingBlockBox(this.Matr.GetMatrix3(), this.CenterPt, this.Offset, base.ToolingBox, base.selectionObj.ToArray());
        }


        public override void SetDimForFace(ref LinearDimension ld, Vector3d vec)
        {
            foreach (Face face in this.ToolingBox.GetBodies()[0].GetFaces())
            {
                FaceData fd = FaceUtils.AskFaceData(face);
                Vector3d temp = fd.Dir;
                this.Matr.ApplyVec(ref temp);
                double angle = UMathUtils.Angle(vec, temp);
                if (UMathUtils.IsEqual(angle, 0))
                {
                    ld.HandleOrientation = fd.Dir;
                    ld.HandleOrigin = fd.Point;
                }
            }
        }

        public override void Update(Matrix4 matr, double[] offset)
        {
            if ((2 * this.disPt.X + offset[0] + offset[1]) <= 0 ||
                (2 * this.disPt.Y + offset[2] + offset[3]) <= 0 ||
                (2 * this.disPt.Z + offset[4] + offset[5]) <= 0)
                return;
            this.Matr = matr;
            this.Offset = offset;
            CreateSuperBox();
        }
        public override void UpdateSpecify(UIBlock ui)
        {
            if (ui is SpecifyOrientation)
            {
                SpecifyOrientation so = ui as SpecifyOrientation;
                so.Origin = this.CenterPt;
                so.XAxis = this.Matr.GetXAxis();
                so.YAxis = this.Matr.GetYAxis();
            }
        }


    }
}
