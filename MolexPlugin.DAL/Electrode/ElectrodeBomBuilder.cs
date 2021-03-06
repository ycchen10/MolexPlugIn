﻿using System;
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
                if (model.EleInfo.EleName == info.EleName)
                    Model.Add(model);
            }
            foreach (WorkModel wk in assemble.Works)
            {
                if (wk.WorkNumber == Model[0].WorkNumber)
                    work = wk;
            }
            EletrodePreparation pre;
            if (info.Material.Equals("紫铜"))
            {
                pre = new EletrodePreparation("CuLength", "CuWidth");
            }
            else
            {
                pre = new EletrodePreparation("WuLength", "WuWidth");
            }
            int[] temp = new int[2] { info.Preparation[0], info.Preparation[1] };
            info.IsPreparation = pre.IsPreCriterion(temp);
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
            // Part workPart = Session.GetSession().Parts.Work;
            PartUtils.SetPartDisplay(work.PartTag);
            double[] temp = new double[3];
            for (int i = 0; i < 3; i++)
            {
                temp[i] = AttributeUtils.GetAttrForDouble(model.PartTag, "EleSetValue", i);
            }
            Point3d oldPt = new Point3d(temp[0], temp[1], temp[2]);
            Point3d newPt = new Point3d(newEleInfo.EleSetValue[0], newEleInfo.EleSetValue[1], newEleInfo.EleSetValue[2]);
            Point3d dis = GetSingleToothDis(newEleInfo, model.EleInfo, oldPt);
            //  newEleInfo.Positioning = model.EleInfo.Positioning;
            string pos = model.EleInfo.Positioning;
           
            model.EleInfo = newEleInfo;
            newEleInfo.SetAttribute(model.PartTag);
            AttributeUtils.AttributeOperation("Positioning", pos, model.PartTag);
            if (!UMathUtils.IsEqual(oldPt, newPt))
            {
                NXOpen.Assemblies.Component ct = AssmbliesUtils.GetPartComp(work.PartTag, model.PartTag);
                PartUtils.SetPartWork(ct);
                Vector3d move = new Vector3d(0, 0, 0);
                move.X = newPt.X - oldPt.X - dis.X;
                move.Y = newPt.Y - oldPt.Y - dis.Y;

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
            AlterWorkDrawing();
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
            string dwgName = Model[0].EleInfo.EleName + "_dwg";
            string path = Model[0].WorkpieceDirectoryPath + Model[0].EleInfo.EleName + "_dwg.prt";
            Part dwg = null;
            foreach (Part part in Session.GetSession().Parts)
            {
                if (part.Name.ToUpper().Equals(dwgName.ToUpper()))
                {
                    dwg = part;
                    break;
                }

            }
            if (dwg == null)
            {
                if (File.Exists(path))
                {
                    dwg = PartUtils.OpenPartFile(path);
                }
            }
            if (dwg != null)
            {
                info.SetAttribute(dwg);
                string temp = info.Preparation[0].ToString() + "*" + info.Preparation[1].ToString() + "*" + info.Preparation[2].ToString();
                AttributeUtils.AttributeOperation("StrPre", temp, dwg);
                PartUtils.SetPartDisplay(dwg);
                foreach (NXOpen.Drawings.DrawingSheet sh in dwg.DrawingSheets)
                {

                    Basic.DrawingUtils.UpdateViews(sh);
                }
            }
        }
        /// <summary>
        /// 更新work零件明细表
        /// </summary>
        private void AlterWorkDrawing()
        {
            Part workPart = Session.GetSession().Parts.Work;
            UFSession theUFSession = UFSession.GetUFSession();
            if (!workPart.Equals(work.PartTag))
            {
                NXOpen.Assemblies.Component ct = AssmbliesUtils.GetPartComp(workPart, work.PartTag);
                PartUtils.SetPartWork(ct);
            }
            theUFSession.Plist.UpdateAllPlists();
            PartUtils.SetPartWork(null);
        }
        /// <summary>
        /// 获取单齿设定的距离
        /// </summary>
        /// <param name="newInfo"></param>
        /// <param name="oldInfo"></param>
        /// <returns></returns>
        private Point3d GetSingleToothDis(ElectrodeInfo newInfo, ElectrodeInfo oldInfo, Point3d oldPt)
        {
            Point3d newPt = new Point3d(newInfo.EleSetValue[0], newInfo.EleSetValue[1], newInfo.EleSetValue[2]);
            if (newInfo.PitchXNum > 0)
            {
                newPt.X = newPt.X - newInfo.PitchX * (newInfo.PitchXNum - 1) / 2;
            }
            if (newInfo.PitchYNum > 0)
            {
                newPt.Y = newPt.Y - newInfo.PitchY * (newInfo.PitchYNum - 1) / 2;
            }
            if (newInfo.PitchXNum > 0)
            {
                oldPt.X = oldPt.X - oldInfo.PitchX * (oldInfo.PitchXNum - 1) / 2;
            }
            if (newInfo.PitchYNum > 0)
            {
                oldPt.Y = oldPt.Y - oldInfo.PitchY * (oldInfo.PitchYNum - 1) / 2;
            }
            return new Point3d(newPt.X - oldPt.X, newPt.Y - oldPt.Y, newPt.Z - oldPt.Z);
        }
    }
}
