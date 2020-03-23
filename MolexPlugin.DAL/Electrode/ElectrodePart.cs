using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;


namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建电极
    /// </summary>
    public class ElectrodePart
    {
        /// <summary>
        /// 电极矩阵
        /// </summary>
        private Matrix4 eleMatr;
        /// <summary>
        /// 电极信息
        /// </summary>
        private ElectrodeInfo eleInfo;
        /// <summary>
        /// 模具信息
        /// </summary>
        private MoldInfoModel moldInfo;
        /// <summary>
        /// 电极头
        /// </summary>
        private ElectrodeHeadInfo head;
        /// <summary>
        /// 创建特征
        /// </summary>
        private CreateElectrodePart elePart;

        private Body[] waveBodys;

        public ElectrodePart(ElectrodeInfo eleInfo, MoldInfoModel moldInfo, ElectrodeHeadInfo head, Matrix4 mat)
        {
            this.eleMatr = mat;
            this.eleInfo = eleInfo;
            this.moldInfo = moldInfo;
            this.head = head;
            elePart = new CreateElectrodePart(head.ConditionModel.Work.WorkpieceDirectoryPath, this.head.ConditionModel.Work.WorkNumber
                , eleInfo, moldInfo, mat);
        }
        public bool CreateElectrode(bool zDatum, double zHeight, Action exp)
        {
            NXOpen.Assemblies.Component eleComp = this.CreateElePart();
            if (eleComp != null)
            {
                exp.Invoke();
                this.CreateBodyFeature(zHeight);
                this.CreateSeat(zHeight, zDatum);
                CreateCenterPoint();
                PartUtils.SetPartWork(null);
                Vector3d vec = GetMove();
                AssmbliesUtils.MoveCompPart(eleComp, vec, head.ConditionModel.Work.WorkMatr); //移动装配
                MoveObjectLayer(100 + eleInfo.EleNumber, this.head.ConditionModel.Bodys.ToArray());
                return true;

            }
            return false;

        }
        /// <summary>
        /// 创建part档
        /// </summary>
        /// <returns></returns>
        private NXOpen.Assemblies.Component CreateElePart()
        {
            NXOpen.Assemblies.Component eleComp = elePart.Create();
            if (eleComp != null)
            {
                PartUtils.SetPartWork(eleComp);
                NXOpen.Features.Feature feat = AssmbliesUtils.WaveBodys(this.head.ConditionModel.Bodys.ToArray());
                this.waveBodys = (feat as NXOpen.Features.BodyFeature).GetBodies();

            }
            return eleComp;
        }
        /// <summary>
        /// 创建体特征
        /// </summary>
        private void CreateBodyFeature(double zHeight)
        {
            List<Body> allbody = new List<Body>();
            Matrix4 mat = new Matrix4();
            mat.Identity();
            foreach (Face face in GetMaxFaceForWave(waveBodys))
            {
                PullFaceUtils.CreatePullFace(new Vector3d(0, 0, -1), zHeight, face);
            }
            NXOpen.Features.PatternGeometry patt = PatternUtils.CreatePattern(" xNCopies", "xPitchDistance", "yNCopies", " yPitchDistance"
               , mat, waveBodys); //创建阵列(就是绝对坐标的矩阵)
            allbody.AddRange(patt.GetAssociatedBodies());
            allbody.AddRange(waveBodys);
            MoveObject.CreateMoveObjToXYZ("moveX", "moveY", "moveZ", null, allbody.ToArray());
        }
        /// <summary>
        /// 创建基准座
        /// </summary>
        private void CreateSeat(double zHeigth, bool zDatum)
        {
            List<Body> bodys = new List<Body>();
            bodys.AddRange(elePart.Model.PartTag.Bodies.ToArray());
            ElectrodeSketchBuilder builder = new ElectrodeSketchBuilder(eleInfo.Preparation[0], eleInfo.Preparation[1], -zHeigth);
            builder.CreateEleSketch();
            NXOpen.Features.Feature ext1 = ExtrudedUtils.CreateExtruded(new Vector3d(0, 0, -1), "0", "2", null, builder.LeiLine);
            NXOpen.Features.Feature ext2 = ExtrudedUtils.CreateExtruded(new Vector3d(0, 0, -1), "2", "20", null, builder.WaiLine);
            if (zDatum)
            {
                NXOpen.Features.Feature ext3 = ExtrudedUtils.CreateExtruded(new Vector3d(0, 0, 1), "0", zHeigth.ToString(), null, builder.Center);
                Body extBody3 = (ext3 as NXOpen.Features.BodyFeature).GetBodies()[0];
                MoveObject.CreateMoveObjToXYZ("moveBoxX", "moveBoxY", "moveBoxZ", null, extBody3);
                bodys.Add(extBody3);
            }
            Body extBody1 = (ext1 as NXOpen.Features.BodyFeature).GetBodies()[0];
            Body extBody2 = (ext2 as NXOpen.Features.BodyFeature).GetBodies()[0];
            CreateChamfer(extBody1.Tag);
            CreateUnite(bodys.ToArray(), extBody1.Tag, extBody2.Tag);
        }
        /// <summary>
        /// 创建中心点
        /// </summary>
        private void CreateCenterPoint()
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Part workPart = Session.GetSession().Parts.Work;
            Point3d center = GetCenter();
            Matrix4 inver = head.ConditionModel.Work.WorkMatr.GetInversMatrix();
            inver.ApplyPos(ref center);
            elePart.Model.EleMatr.ApplyPos(ref center);
            Point pt = PointUtils.CreatePointFeature(center);
            pt.Color = 186;
            pt.Layer = 254;
            theUFSession.Obj.SetName(pt.Tag, "SetValuePoint");
            
        }
        /// <summary>
        /// 获取最低面
        /// </summary>
        /// <returns></returns>
        private List<Face> GetMaxFaceForWave(Body[] bodys)
        {
            List<Face> temp = new List<Face>();
            foreach (Body body in bodys)
            {
                Face maxFace = null;
                double zMin = 9999;

                foreach (Face face in body.GetFaces())
                {
                    FaceData data = FaceUtils.AskFaceData(face);
                    Point3d center = UMathUtils.GetMiddle(data.BoxMaxCorner, data.BoxMinCorner);
                    if (zMin > center.Z)
                    {
                        zMin = center.Z;
                        maxFace = face;
                    }
                }
                temp.Add(maxFace);
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

        private Vector3d GetMove()
        {
            Vector3d temp = new Vector3d();
            temp.X = (eleInfo.PitchXNum - 1) * eleInfo.PitchX / 2;
            temp.Y = (eleInfo.PitchYNum - 1) * eleInfo.PitchY / 2;
            temp.Z = 0;
            if (eleInfo.ZDatum)
            {
                if (eleInfo.Preparation[0] >= eleInfo.Preparation[1])
                {
                    temp.X = (eleInfo.PitchXNum - 2) * eleInfo.PitchX / 2;
                }
                else
                {
                    temp.Y = (eleInfo.PitchYNum - 2) * eleInfo.PitchY / 2;
                }

            }
            return temp;
        }

        /// <summary>
        /// 移动电极头
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="objs"></param>
        private void MoveObjectLayer(int layer, params NXObject[] objs)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            foreach (NXObject obj in objs)
            {
                theUFSession.Obj.SetLayer(obj.Tag, layer);
            }

        }

        private Point3d GetCenter()
        {
            Point3d temp = new Point3d();
            Vector3d move = GetMove();
            temp.X = eleInfo.EleSetValue[0] - move.X;
            temp.Y = eleInfo.EleSetValue[1] - move.Y;
            temp.Z = eleInfo.EleSetValue[2];
            return temp;
        }

    }
}
