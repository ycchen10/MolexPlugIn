using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public class ElectrodePart
    {
        private MoldInfoModel mold;
        private ElectrodeInfo eleInfo;
        private ElectrodeHeadModel head;
        private AssembleCollection coll;
        private ElectrodeAssembleModel eleModel;
        private string path;
        private Body[] waveBodys;
        private ElectrodeSketchBuilder builder;
        private Part work;
        public NXOpen.Assemblies.Component EleComp { get; private set; }
        public bool IsOk { get; private set; }
        public ElectrodePart(ElectrodeHeadModel head, ElectrodeInfo info, MoldInfoModel mold, Part work)
        {
            this.work = work;
            path = Path.GetDirectoryName(work.FullPath) + "\\";
            this.head = head;
            this.eleInfo = info;
            this.mold = mold;
            AssembleInstance inst = AssembleInstance.GetInstance();
            coll = inst.GetAssembleModle();
        }
        public void CreateEle()
        {
            CreateElePart();
            if (IsOk)
            {
                CreateBodyFeature();
                Sketch(eleModel);
                CreateExt();
                CreateSetPont(eleModel.Matr);
                coll.AddEle(eleModel);

            }

        }
        /// <summary>
        /// 创建Part档
        /// </summary>
        private void CreateElePart()
        {
            Matrix4 mat = head.GetEleMatr();
            Point3d eleOrigin = GetEleOrigin(mat, GetSetValue());
            Matrix4 workInvers = this.head.model.Work.Matr.GetInversMatrix();
            workInvers.ApplyPos(ref eleOrigin);
            eleModel = new ElectrodeAssembleModel(eleInfo, mold, mat, eleOrigin);
            this.EleComp = eleModel.CreateCompPart(path);
            IsOk = this.EleComp != null;
            if (EleComp != null)
            {

                PartUtils.SetPartWork(this.EleComp);
                NXOpen.Features.Feature feat = AssmbliesUtils.WaveBodys(head.model.Bodys.ToArray());
                this.waveBodys = (feat as NXOpen.Features.BodyFeature).GetBodies();
            }
        }
        /// <summary>
        /// 创建体特征
        /// </summary>
        private void CreateBodyFeature()
        {
            List<Body> bodys = new List<Body>();
            Body[] boxBody;
            double pull = GetEleZDatum();
            foreach (Face face in GetMaxFaceForWave())
            {
                PullFaceUtils.CreatePullFace(new Vector3d(0, 0, -1), pull, face);
            }

            NXOpen.Features.PatternGeometry patt = PatternUtils.CreatePattern(eleInfo.PitchXNum.ToString(), eleInfo.PitchX.ToString(),
                eleInfo.PitchYNum.ToString(), (eleInfo.PitchY).ToString(), eleModel.Matr, waveBodys); //创建阵列
            bodys.AddRange(patt.GetAssociatedBodies());
            bodys.AddRange(waveBodys);

            SetExpression();
            MoveObject.CreateMoveObjToXYZ("moveX", "moveY", "moveZ", null, bodys.ToArray());
            //if (eleInfo.ZDatum)
            //{
            //    Point3d center = new Point3d();
            //    Body body = GetMaxWaveBody(out center);
            //    NXOpen.Features.ToolingBox box = ToolingBoxFeature.CreateToolingCylinder(new Vector3d(0, 0, -1), center, new double[3] { 0, 0, 0 }, null, body);
            //    boxBody = box.GetBodies();
            //    MoveObject.CreateMoveObjToXYZ("moveBoxX", "moveBoxY", "moveBoxZ", null, boxBody); //移动Z向基准台
            //}


        }

        /// <summary>
        /// 设置草绘
        /// </summary>
        /// <param name="model"></param>
        private void Sketch(ElectrodeAssembleModel model)
        {
            double z = GetEleZDatum();
            builder = new ElectrodeSketchBuilder(model.EleInfo.Preparation[0], model.EleInfo.Preparation[1], -z);
            builder.CreateEleSketch();
        }

        private void CreateExt()
        {
            Body[] bodys = eleModel.PartTag.Bodies.ToArray();
            NXOpen.Features.Feature ext1 = ExtrudedUtils.CreateExtruded(new Vector3d(0, 0, -1), "0", "3", null, builder.LeiLine);
            NXOpen.Features.Feature ext2 = ExtrudedUtils.CreateExtruded(new Vector3d(0, 0, -1), "3", "20", null, builder.WaiLine);
            if (eleInfo.ZDatum)
            {
                NXOpen.Features.Feature ext3 = ExtrudedUtils.CreateExtruded(new Vector3d(0, 0, 1), "0", GetEleZDatum().ToString(), null, builder.Center);
                Body extBody3 = (ext3 as NXOpen.Features.BodyFeature).GetBodies()[0];
              //  MoveObject.CreateMoveObjToXYZ("moveBoxX", "moveBoxY", "moveBoxZ", null, extBody3); //移动Z向基准台
            }
            Body extBody1 = (ext1 as NXOpen.Features.BodyFeature).GetBodies()[0];
            Body extBody2 = (ext2 as NXOpen.Features.BodyFeature).GetBodies()[0];
            CreateChamfer(extBody1.Tag);
            //  CreateUnite(bodys, extBody1.Tag, extBody2.Tag);
        }
        /// <summary>
        /// 获取最低面
        /// </summary>
        /// <returns></returns>
        private List<Face> GetMaxFaceForWave()
        {
            List<Face> temp = new List<Face>();
            foreach (Body body in waveBodys)
            {
                Face maxFace = null;
                double zMax = 9999;
                double zMin = 9999;
                foreach (Face face in body.GetFaces())
                {
                    FaceData data = FaceUtils.AskFaceData(face);
                    if (zMax >= data.BoxMaxCorner.Z && zMin >= data.BoxMinCorner.Z)
                    {
                        zMin = data.BoxMinCorner.Z;
                        zMax = data.BoxMaxCorner.Z;
                        maxFace = face;
                    }
                }
                temp.Add(maxFace);
            }
            return temp;
        }

        /// <summary>
        /// 获取移动点
        /// </summary>
        /// <returns></returns>
        public Vector3d GetMovePoint()
        {
            Vector3d temp = new Vector3d(0, 0, 0);
            if (eleInfo.ZDatum)
            {
                if (eleInfo.Preparation[0] >= eleInfo.Preparation[1])
                {
                    temp.X = eleInfo.PitchX * eleInfo.PitchXNum / 2;
                    temp.Y = -eleInfo.PitchY * (eleInfo.PitchYNum - 1) / 2;
                }
                else
                {

                    temp.X = eleInfo.PitchX * (eleInfo.PitchXNum - 1) / 2;
                    temp.Y = -eleInfo.PitchY * eleInfo.PitchYNum / 2;
                }
            }
            else
            {

                temp.X = eleInfo.PitchX * (eleInfo.PitchXNum - 1) / 2;
                temp.Y = eleInfo.PitchY * (eleInfo.PitchYNum - 1) / 2;
            }
            return temp;
        }

        /// <summary>
        ///设置表达式
        /// </summary>
        public void SetExpression()
        {
            ExpressionUtils.SetAttrExp("PitchX", "PitchX", NXObject.AttributeType.Real);
            ExpressionUtils.SetAttrExp("PitchXNum", "PitchXNum", NXObject.AttributeType.Integer);
            ExpressionUtils.SetAttrExp("PitchY", "PitchY", NXObject.AttributeType.Real);
            ExpressionUtils.SetAttrExp("PitchYNum", "PitchYNum", NXObject.AttributeType.Integer);

            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("xNCopies"), "PitchXNum");

            ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("yNCopies"), "PitchYNum");

            //if (eleInfo.ZDatum)
            //{
            //    if (eleInfo.Preparation[0] >= eleInfo.Preparation[1])
            //    {
            //        ExpressionUtils.CreateExp("moveX=-(xNCopies)*xPitchDistance/2", "Number");
            //        ExpressionUtils.CreateExp("moveY=(yNCopies-1)*yPitchDistance/2", "Number");

            //        ExpressionUtils.CreateExp("moveBoxX=(xNCopies)*xPitchDistance", "Number");
            //        ExpressionUtils.CreateExp("moveBoxY=0", "Number");
            //    }
            //    else
            //    {
            //        ExpressionUtils.CreateExp("moveX=-(xNCopies-1)*xPitchDistance/2", "Number");
            //        ExpressionUtils.CreateExp("moveY=(yNCopies)*yPitchDistance/2", "Number");

            //        ExpressionUtils.CreateExp("moveBoxX=0", "Number");
            //        ExpressionUtils.CreateExp("moveBoxY=-(yNCopies)*yPitchDistance", "Number");
            //    }
            //}
            //else
            //{
            //    ExpressionUtils.CreateExp("moveX=-(xNCopies-1)*xPitchDistance/2", "Number");
            //    ExpressionUtils.CreateExp("moveY=(yNCopies-1)*yPitchDistance/2", "Number");
            //}

            //ExpressionUtils.CreateExp("moveZ=0", "Number");
            //ExpressionUtils.CreateExp("moveBoxZ=0", "Number");

            SetMoveExp();

            ExpressionUtils.SetAttrExp("PreparationX", "Preparation", NXObject.AttributeType.Integer, 0);
            ExpressionUtils.SetAttrExp("PreparationY", "Preparation", NXObject.AttributeType.Integer, 1);
        }

        public void SetMoveExp()
        {
            double angleX = UMathUtils.Angle(this.head.model.Work.Matr.GetXAxis(), this.eleModel.Matr.GetXAxis());
            double angleY = UMathUtils.Angle(this.head.model.Work.Matr.GetYAxis(), this.eleModel.Matr.GetYAxis());
            Expression moveXExp = ExpressionUtils.CreateExp("moveX=0", "Number");
            Expression moveYExp = ExpressionUtils.CreateExp("moveY=0", "Number");
            Expression moveBoxXExp = ExpressionUtils.CreateExp("moveBoxX=0", "Number");
            Expression moveBoxYExp = ExpressionUtils.CreateExp("moveBoxY=0", "Number");
            ExpressionUtils.CreateExp("moveZ=0", "Number");
            ExpressionUtils.CreateExp("moveBoxZ=0", "Number");
            if (UMathUtils.IsEqual(angleX, 0))
            {

                ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("xPitchDistance"), "PitchX");
                if (eleInfo.Preparation[0] >= eleInfo.Preparation[1] && eleInfo.ZDatum)
                {
                    ExpressionUtils.EditExp(moveBoxXExp, "-(xNCopies)*xPitchDistance/2");
                    ExpressionUtils.EditExp(moveXExp, "-(xNCopies)*xPitchDistance/2");
                }
                else
                {
                    ExpressionUtils.EditExp(moveXExp, "-(xNCopies-1)*xPitchDistance/2");

                }
            }
            if (UMathUtils.IsEqual(angleX, Math.PI))
            {
                ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("xPitchDistance"), "-PitchX");
                if (eleInfo.Preparation[0] >= eleInfo.Preparation[1] && eleInfo.ZDatum)
                {
                    ExpressionUtils.EditExp(moveBoxXExp, "(xNCopies)*xPitchDistance/2");
                    ExpressionUtils.EditExp(moveXExp, "(xNCopies)*xPitchDistance/2");
                }
                else
                {
                    ExpressionUtils.EditExp(moveXExp, "(xNCopies-1)*xPitchDistance/2");

                }
            }
            if (UMathUtils.IsEqual(angleY, 0))
            {
                ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("yPitchDistance"), "PitchY");
                if (eleInfo.Preparation[0] < eleInfo.Preparation[1] && eleInfo.ZDatum)
                {
                    ExpressionUtils.EditExp(moveBoxYExp, "-(yNCopies)*yPitchDistance/2");
                    ExpressionUtils.EditExp(moveYExp, "-(yNCopies)*yPitchDistance/2");
                }
                else
                {
                    ExpressionUtils.EditExp(moveYExp, "-(yNCopies-1)*yPitchDistance/2");

                }
            }
            if (UMathUtils.IsEqual(angleY, Math.PI))
            {
                ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("yPitchDistance"), "-PitchY");
                if (eleInfo.Preparation[0] < eleInfo.Preparation[1] && eleInfo.ZDatum)
                {
                    ExpressionUtils.EditExp(moveBoxYExp, "(yNCopies)*yPitchDistance/2");
                    ExpressionUtils.EditExp(moveYExp, "(yNCopies)*yPitchDistance/2");
                }
                else
                {
                    ExpressionUtils.EditExp(moveYExp, "(yNCopies-1)*yPitchDistance/2");

                }
            }

        }
        /// <summary>
        /// 获取最大体
        /// </summary>
        /// <returns></returns>
        private Body GetMaxWaveBody(out Point3d center)
        {
            Body temp = null;
            double max = -99999;
            center = new Point3d();
            foreach (Body body in this.waveBodys)
            {
                double k = 0;
                Point3d centerPt = new Point3d();
                Point3d disPt = new Point3d();
                NXObject[] obj = new NXObject[1] { body };
                Matrix4 mat = new Matrix4();
                mat.Identity();
                BoundingBoxUtils.GetBoundingBoxInLocal(obj, null, mat, ref centerPt, ref disPt);
                if (disPt.X >= disPt.Y)
                    k = disPt.X;
                else
                    k = disPt.Y;
                if (max < k)
                {
                    max = k;
                    temp = body;
                    center = centerPt;
                }
            }
            return temp;
        }

        /// <summary>
        /// 创建倒角
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private Tag CreateChamfer(Tag body)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag[] edgesTag;
            Tag chamferTag = Tag.Null;
            double[] point1 = new double[3];
            double[] point2 = new double[3];
            int connt = 0;
            theUFSession.Modl.AskBodyEdges(body, out edgesTag);
            for (int i = 0; i < edgesTag.Length; i++)
            {
                theUFSession.Modl.AskEdgeVerts(edgesTag[i], point1, point2, out connt);
                if (point1[0] == point2[0] && point1[1] == point2[1] && point1[0] > 0 && point1[1] > 0)
                {
                    Tag[] obj = new Tag[1];
                    obj[0] = edgesTag[i];
                    theUFSession.Modl.CreateChamfer(1, "2.0", "2.0", "45.0", obj, out chamferTag);
                    break;
                }
            }
            return chamferTag;
        }

        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="allBodys"></param>
        /// <param name="datBodyTag1"></param>
        /// <param name="datBodyTag2"></param>
        /// <returns></returns>
        private Tag CreateUnite(Body[] allBodys, Tag datBodyTag1, Tag datBodyTag2)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag uniteTag = Tag.Null;
            theUFSession.Modl.UniteBodiesWithRetainedOptions(datBodyTag1, datBodyTag2, false, false, out uniteTag);
            for (int i = 0; i < allBodys.Length; i++)
            {
                Tag temp = Tag.Null;
                theUFSession.Modl.AskFeatBody(uniteTag, out temp);
                theUFSession.Modl.UniteBodiesWithRetainedOptions(allBodys[i].Tag, temp, false, false, out uniteTag);

            }
            return uniteTag;
        }
        /// <summary>
        /// 获取单齿设定值
        /// </summary>
        /// <returns></returns>
        private Point3d GetSetValue()
        {
            Point3d setPt = new Point3d(eleInfo.EleSetValue[0], eleInfo.EleSetValue[1], eleInfo.EleSetValue[2]);
            double x, y, z;
            z = setPt.Z; ;
            if (eleInfo.ZDatum)
            {
                x = Math.Round(setPt.X - (eleInfo.PitchXNum) * eleInfo.PitchX / 2, 4);
                y = Math.Round(setPt.Y - (eleInfo.PitchYNum) * eleInfo.PitchY / 2, 4);

                if (x >= y)
                {
                    y = Math.Round(setPt.Y - (eleInfo.PitchYNum - 1) * eleInfo.PitchY / 2, 4);
                }
                else
                {
                    x = Math.Round(setPt.X - (eleInfo.PitchXNum - 1) * eleInfo.PitchX / 2, 4);
                }
            }
            else
            {
                x = Math.Round(setPt.X - (eleInfo.PitchXNum - 1) * eleInfo.PitchX / 2, 4);
                y = Math.Round(setPt.Y - (eleInfo.PitchYNum - 1) * eleInfo.PitchY / 2, 4);

            }
            return new Point3d(x, y, z);
        }
        /// <summary>
        /// 获取电极圆心位置
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="setPt"></param>
        /// <returns></returns>
        private Point3d GetEleOrigin(Matrix4 mat, Point3d setPt)
        {
            double angle = UMathUtils.Angle(mat.GetZAxis(), head.model.Work.Matr.GetZAxis());
            if (UMathUtils.IsEqual(angle, Math.PI))
            {
                return setPt;
            }
            else
            {
                double anleX = UMathUtils.Angle(mat.GetZAxis(), head.model.Work.Matr.GetXAxis());
                double anleY = UMathUtils.Angle(mat.GetZAxis(), head.model.Work.Matr.GetYAxis());
                if (UMathUtils.IsEqual(anleX, Math.PI) || UMathUtils.IsEqual(anleX, 0))
                {
                    return new Point3d(setPt.X, setPt.Y, setPt.Z + eleInfo.Preparation[0] / 2 - 0.7);
                }
                if (UMathUtils.IsEqual(anleY, Math.PI) || UMathUtils.IsEqual(anleY, 0))
                {
                    return new Point3d(setPt.X, setPt.Y, setPt.Z + eleInfo.Preparation[1] / 2 - 0.7);
                }
            }
            return setPt;
        }
        /// <summary>
        /// 获取Z向基准台高度
        /// </summary>
        /// <returns></returns>
        private double GetEleZDatum()
        {
            double z = 0;
            double angle = UMathUtils.Angle(eleModel.Matr.GetZAxis(), head.model.Work.Matr.GetZAxis());
            if (UMathUtils.IsEqual(angle, Math.PI))
            {
                z = eleInfo.Extrudewith + Math.Abs(eleInfo.EleSetValue[2]);
            }
            else
            {
                double anleX = UMathUtils.Angle(eleModel.Matr.GetZAxis(), head.model.Work.Matr.GetXAxis());
                double anleY = UMathUtils.Angle(eleModel.Matr.GetZAxis(), head.model.Work.Matr.GetYAxis());
                if (UMathUtils.IsEqual(anleX, Math.PI) || UMathUtils.IsEqual(anleX, 0))
                {
                    z = this.head.DisPt.X * 2;
                }
                if (UMathUtils.IsEqual(anleY, Math.PI) || UMathUtils.IsEqual(anleY, 0))
                {
                    z = this.head.DisPt.Y * 2;
                }
            }
            return z;
        }
        /// <summary>
        /// 创建设定点
        /// </summary>
        /// <param name="mat"></param>
        private void CreateSetPont(Matrix4 mat)
        {
            double[] setPt = new double[3];
            Tag point = Tag.Null;
            double angle = UMathUtils.Angle(mat.GetZAxis(), head.model.Work.Matr.GetZAxis());
            if (UMathUtils.IsEqual(angle, Math.PI))
            {
                setPt = new double[3] { 0, 0, 0 };
            }
            else
            {
                double anleX = UMathUtils.Angle(mat.GetZAxis(), head.model.Work.Matr.GetXAxis());
                double anleY = UMathUtils.Angle(mat.GetZAxis(), head.model.Work.Matr.GetYAxis());
                if (UMathUtils.IsEqual(anleX, 0))
                {

                    setPt = new double[3] { 0 + (eleInfo.Preparation[0] / 2 - 0.7), 0, 0 };
                }
                if (UMathUtils.IsEqual(anleX, Math.PI))
                {

                    setPt = new double[3] { 0 - (eleInfo.Preparation[0] / 2 - 0.7), 0, 0 };
                }
                if (UMathUtils.IsEqual(anleY, 0))
                {
                    setPt = new double[3] { 0, 0 + (eleInfo.Preparation[1] / 2 - 0.7), 0 };
                }
                if (UMathUtils.IsEqual(anleY, Math.PI))
                {
                    setPt = new double[3] { 0, 0 - (eleInfo.Preparation[1] / 2 - 0.7), 0 };
                }
            }
            UFSession theUFsession = UFSession.GetUFSession();
            theUFsession.Curve.CreatePoint(setPt, out point);
            theUFsession.Obj.SetColor(point, 186);
            theUFsession.Obj.SetName(point, "centerPoint");
        }
        private void SetWCS(Matrix4 mat)
        {
            Point3d origin = new Point3d(0, 0, 0);
            Matrix4 invers = mat.GetInversMatrix();
            invers.ApplyPos(ref origin);
            CsysUtils.SetWcsOfCenteAndMatr(origin, mat.GetMatrix3());
        }
    }


}


