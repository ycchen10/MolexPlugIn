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
    public class SuperBoxCylinder : AbstractSuperBox
    {

        public SuperBoxCylinder(double[] offset, List<NXObject> nxObjects) : base(offset, nxObjects)
        {

        }

        public override void CreateSuperBox()
        {
            if (this.disPt.X > this.disPt.Y)
            {
                if ((this.disPt.X + this.Offset[2]) <= 0 && (2 * this.disPt.Z + this.Offset[0] + this.Offset[1]) <= 0)
                    return;
            }
            if (this.disPt.X < this.disPt.Y)
            {
                if ((this.disPt.Y + this.Offset[2]) <= 0 && (2 * this.disPt.Z + this.Offset[0] + this.Offset[1]) <= 0)
                    return;
            }
            else
                base.ToolingBox = ToolingBoxFeature.CreateToolingCylinder(this.Matr.GetZAxis(), this.CenterPt, this.Offset, ToolingBox, this.selectionObj.ToArray());
        }


        public override void SetDimForFace(ref LinearDimension ld, Vector3d vec)
        {
            foreach (Face face in this.ToolingBox.GetBodies()[0].GetFaces())
            {

                if (face.SolidFaceType == Face.FaceType.Cylindrical)
                {
                    Point3d originPt = new Point3d(0, 0, 0);
                    Vector3d normal = new Vector3d(0, 0, 0);
                    FaceUtils.AskFaceOriginAndNormal(face, out originPt, out normal);
                    double angle1 = UMathUtils.Angle(vec, new Vector3d(1, 1, 1));
                    if (UMathUtils.IsEqual(angle1, 0))
                    {
                        ld.HandleOrientation = normal;
                        ld.HandleOrigin = originPt;
                    }
                }
                else
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
        }

        public override void Update(Matrix4 matr, double[] offset)
        {
            if (this.disPt.X > this.disPt.Y)
            {
                if ((this.disPt.X + offset[2]) <= 0 && (2 * this.disPt.Z + offset[0] + offset[1]) <= 0)
                    return;
            }
            if (this.disPt.X < this.disPt.Y)
            {
                if ((this.disPt.Y + offset[2]) <= 0 && (2 * this.disPt.Z + offset[0] + offset[1]) <= 0)
                    return;
            }
            this.Matr = matr;
            this.Offset = offset;
            CreateSuperBox();
        }

        public override void UpdateSpecify(UIBlock ui)
        {
            if (ui is SpecifyVector)
            {
                SpecifyVector sv = ui as SpecifyVector;
                sv.Vector = this.Matr.GetZAxis();
                sv.Point = this.CenterPt;
            }
        }



    }
}
