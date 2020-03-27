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
    public class ElectrodeBomBuilder
    {
        public List<ElectrodeModel> Model { get; private set; } = new List<ElectrodeModel>();
        private WorkModel work;
        private ElectrodeInfo info;
        public ElectrodeBomBuilder(ElectrodeInfo info, AssembleModel assemble)
        {
            this.info = info;
            foreach (ElectrodeModel model in assemble.Electrodes)
            {
                if (model.EleInfo.EleNumber == info.EleNumber)
                    Model.Add(model);
            }
            foreach (WorkModel wk in assemble.Works)
            {
                if (wk.WorkNumber == Model[0].WorkNumber)
                    work = wk;
            }
        }

        /// <summary>
        /// 获取改变的设定值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xNumber"></param>
        /// <param name="y"></param>
        /// <param name="yNumber"></param>
        /// <param name="eleInfo"></param>
        /// <returns></returns>
        public static Point3d GetSetValue(double x, int xNumber, double y, int yNumber, ElectrodeInfo eleInfo)
        {
            Point3d oldSet = new Point3d(eleInfo.EleSetValue[0], eleInfo.EleSetValue[1], eleInfo.EleSetValue[2]);
            if (eleInfo.PitchX != 0 && eleInfo.PitchXNum > 1)
            {
                oldSet.X = oldSet.X - eleInfo.PitchX * (eleInfo.PitchXNum - 1) / 2;
            }
            if (eleInfo.PitchY != 0 && eleInfo.PitchYNum > 1)
            {
                oldSet.Y = oldSet.Y - eleInfo.PitchY * (eleInfo.PitchYNum - 1) / 2;
            }
            Point3d newSet = new Point3d(eleInfo.EleSetValue[0], eleInfo.EleSetValue[1], eleInfo.EleSetValue[2]);
            if (x != 0 && xNumber > 1)
            {
                newSet.X = oldSet.X + x * (xNumber - 1) / 2;
            }
            if (y != 0 && yNumber > 1)
            {
                newSet.Y = oldSet.Y + y * (yNumber - 1) / 2;
            }
            return newSet;
        }

        public static int[] GetPreparation(double x, int xNumber, double y, int yNumber, string material, ElectrodeInfo eleInfo)
        {
            Point3d oldPt = new Point3d(eleInfo.Preparation[0], eleInfo.Preparation[1], eleInfo.Preparation[2]);
            if (eleInfo.PitchX != 0 && eleInfo.PitchXNum > 1)
                oldPt.X = oldPt.X - eleInfo.PitchX * (eleInfo.PitchXNum - 1);
            if (eleInfo.PitchY != 0 && eleInfo.PitchYNum > 1)
                oldPt.Y = oldPt.Y - eleInfo.PitchY * (eleInfo.PitchYNum - 1);
            Point3d newPt = new Point3d(oldPt.X, oldPt.Y, oldPt.Z);
            if (x != 0 && xNumber > 1)
            {
                newPt.X = oldPt.X + x * (xNumber - 1);
            }
            if (y != 0 && yNumber > 1)
            {
                newPt.Y = oldPt.Y + y * (yNumber - 1);
            }
            int[] temp = new int[3] { (int)newPt.X, (int)newPt.Y, (int)newPt.Z };
            EletrodePreparation pre;
            if (material.Equals("紫铜"))
            {
                pre = new EletrodePreparation("CuLength", "CuWidth");
            }
            else
            {
                pre = new EletrodePreparation("WuLength", "WuWidth");
            }
            pre.GetPreparation(ref temp);
            return temp;
        }

        private void AlterElectrode(ElectrodeInfo newEleInfo, ElectrodeModel model)
        {
            Part workPart = Session.GetSession().Parts.Work;
            double[] temp = new double[3];
            for (int i = 0; i < 3; i++)
            {
                temp[i] = AttributeUtils.GetAttrForDouble(model.PartTag, "EleSetValue", i);
            }
            Point3d oldPt = new Point3d(temp[0], temp[1], temp[2]);
            Point3d newPt = new Point3d(newEleInfo.EleSetValue[0], newEleInfo.EleSetValue[1], newEleInfo.EleSetValue[2]);
            newEleInfo.Positioning = model.EleInfo.Positioning;
            model.EleInfo = newEleInfo;
            newEleInfo.SetAttribute(model.PartTag);

            if (!UMathUtils.IsEqual(oldPt, newPt))
            {
                NXOpen.Assemblies.Component ct = AssmbliesUtils.GetPartComp(workPart, model.PartTag);
                PartUtils.SetPartWork(ct);
                Vector3d move = new Vector3d(0, 0, 0);
                move.X = newPt.X - oldPt.X;
                move.Y = newPt.Y - oldPt.Y;

                Body[] bodys = model.PartTag.Bodies.ToArray();
                if (bodys.Length > 1)
                {
                    AlterMove(model);
                    DeleteObject.UpdateObject();
                    CreateUnite(bodys);
                }
                PartUtils.SetPartWork(null);
                AssmbliesUtils.MoveCompPart(ct, move, this.work.WorkMatr);

            }
            DeleteObject.UpdateObject();
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="allBodys"></param>
        /// <param name="datBodyTag1"></param>
        /// <param name="datBodyTag2"></param>
        /// <returns></returns>
        private Tag CreateUnite(Body[] allBodys)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            Tag uniteTag = Tag.Null;
            if (allBodys.Length < 2)
                return uniteTag;
            theUFSession.Modl.UniteBodiesWithRetainedOptions(allBodys[0].Tag, allBodys[1].Tag, false, false, out uniteTag);
            for (int i = 2; i < allBodys.Length; i++)
            {
                Tag temp = Tag.Null;
                theUFSession.Modl.AskFeatBody(uniteTag, out temp);
                theUFSession.Modl.UniteBodiesWithRetainedOptions(allBodys[i].Tag, temp, false, false, out uniteTag);

            }
            return uniteTag;
        }

        public void Alter()
        {
            Part asm = Session.GetSession().Parts.Work;
            foreach (ElectrodeModel ele in Model)
            {
                AlterElectrode(info, ele);
            }
            AlterDrawing();
            PartUtils.SetPartDisplay(asm);
        }

        public void AlterMove(ElectrodeModel model)
        {
            List<Body> bodys = new List<Body>();
            NXOpen.Features.Feature moveFeature = null;
            NXOpen.Features.Feature patternFeature = null;
            NXOpen.Features.Feature linkedFeature = null;
            foreach (NXOpen.Features.Feature ft in model.PartTag.Features.ToArray())
            {
                if (ft.FeatureType.Equals("MOVE_OBJECT", StringComparison.CurrentCultureIgnoreCase))
                    moveFeature = ft;
                if (ft.FeatureType.Equals("Pattern Geometry", StringComparison.CurrentCultureIgnoreCase))
                    patternFeature = ft;
                if (ft.FeatureType.Equals("LINKED_BODY", StringComparison.CurrentCultureIgnoreCase))
                    linkedFeature = ft;
            }
            bodys.AddRange(((NXOpen.Features.BodyFeature)linkedFeature).GetBodies());
            bodys.AddRange(((NXOpen.Features.PatternGeometry)patternFeature).GetAssociatedBodies());

            MoveObject.CreateMoveObjToXYZ("moveX", "moveY", "moveZ", moveFeature as NXOpen.Features.MoveObject, bodys.ToArray());

        }

        public void AlterDrawing()
        {
            ElectrodeModel ele = null;
            foreach (ElectrodeModel em in Model)
            {
                if (em.EleInfo.Positioning == "")
                    ele = em;
            }
            string path = ele.WorkpieceDirectoryPath + ele.EleInfo.EleName + "_dwg.prt";
            if (File.Exists(path))
            {
                Part dwg = PartUtils.OpenPartFile(path);
                ele.EleInfo.SetAttribute(dwg);
                foreach (NXOpen.Drawings.DrawingSheet sh in dwg.DrawingSheets)
                {
                    Basic.DrawingUtils.UpdateViews(sh);
                }
            }
        }
    }
}
