using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;

namespace Basic
{
    public class AssmbliesUtils : ClassItem
    {
        /// <summary>
        /// 新建装配档文件
        /// </summary>
        /// <param name="partName">装配Part名字</param>
        /// <param name="partPath">Part档地址</param>
        /// <returns></returns>
        public static NXObject CreateNew(string partName, string partPath)
        {
            Part workPart = theSession.Parts.Work;
            FileNew fileNew1 = theSession.Parts.FileNew();
            fileNew1.TemplateFileName = "molex-plain-1-mm-template.prt";
            fileNew1.UseBlankTemplate = false;
            fileNew1.ApplicationName = "ModelTemplate";
            fileNew1.Units = NXOpen.Part.Units.Millimeters;
            fileNew1.RelationType = "";
            fileNew1.UsesMasterModel = "No";
            fileNew1.TemplateType = NXOpen.FileNewTemplateType.Item;
            fileNew1.TemplatePresentationName = "Molex";
            fileNew1.ItemType = "";
            fileNew1.Specialization = "";
            fileNew1.SetCanCreateAltrep(false);
            fileNew1.NewFileName = string.Concat(partPath);
            fileNew1.MasterFileName = "";
            fileNew1.MakeDisplayedPart = false;
            NXOpen.Assemblies.CreateNewComponentBuilder createNewComponentBuilder1;
            createNewComponentBuilder1 = workPart.AssemblyManager.CreateNewComponentBuilder();
            createNewComponentBuilder1.ReferenceSet = NXOpen.Assemblies.CreateNewComponentBuilder.ComponentReferenceSetType.EntirePartOnly;
            createNewComponentBuilder1.NewComponentName = partName;
            createNewComponentBuilder1.ComponentOrigin = NXOpen.Assemblies.CreateNewComponentBuilder.ComponentOriginType.Wcs;
            createNewComponentBuilder1.NewFile = fileNew1;
            bool validate = createNewComponentBuilder1.Validate();

            try
            {
                NXObject nXObject1 = createNewComponentBuilder1.Commit();
                return nXObject1;

            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AssmbliesUtils:CreateNew:" + ex.Message);
                return null;
            }
            finally
            {
                createNewComponentBuilder1.Destroy();
                Session.UndoMarkId markId = theSession.GetNewestUndoMark(Session.MarkVisibility.Visible);
                theSession.DeleteUndoMark(markId, "");
            }


        }
        /// <summary>
        /// 加载部件
        /// </summary>
        /// <param name="part">部件下面</param>
        /// <param name="partPath">部件地址</param>
        /// <param name="partName">部件名字</param>
        /// <param name="csys">加载坐标</param>
        /// <param name="basePoint">装配位置点</param>
        /// <returns>Component</returns>
        public static NXOpen.Assemblies.Component PartLoad(Part part, string partPath, string partName, Matrix4 matr, Point3d basePoint)
        {
            //  Part workPart = theSession.Parts.Work;            

            NXOpen.PartLoadStatus partLoadStatus1 = null;
            NXOpen.Assemblies.Component component1;
            try
            {
                component1 = part.ComponentAssembly.AddComponent(partPath, "None", partName, basePoint, matr.GetMatrix3(), -1, out partLoadStatus1, true);
                return component1;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AssmbliesUtils:PartLoad:" + ex.Message);
                return null;
            }
            finally
            {
                partLoadStatus1.Dispose();
            }

        }


        /// <summary>
        /// 链接到工作部件中(关联)
        /// </summary>
        /// <param name="seleObj">要链接的体</param>
        /// <returns></returns>
        public static NXOpen.Features.Feature WaveBodys(params NXObject[] seleObj)
        {

            Part workPart = theSession.Parts.Work;
            NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
            NXOpen.Features.WaveLinkBuilder waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullNXOpen_Features_Feature);
            NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
            extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;
            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;
            waveLinkBuilder1.Type = NXOpen.Features.WaveLinkBuilder.Types.BodyLink;
            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;
            extractFaceBuilder1.AngleTolerance = 45.0;
            extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;
            extractFaceBuilder1.Associative = false;   //关联
            extractFaceBuilder1.MakePositionIndependent = false;
            extractFaceBuilder1.FixAtCurrentTimestamp = false;

            extractFaceBuilder1.HideOriginal = false;

            extractFaceBuilder1.InheritDisplayProperties = false;

            NXOpen.ScCollector scCollector1;
            scCollector1 = extractFaceBuilder1.ExtractBodyCollector;

            extractFaceBuilder1.CopyThreads = false; //是否复制

            extractFaceBuilder1.FeatureOption = NXOpen.Features.ExtractFaceBuilder.FeatureOptionType.OneFeatureForAllBodies;
            Body[] seleBody = new Body[seleObj.Length];
            for (int i = 0; i < seleObj.Length; i++)
            {
                seleBody[i] = (Body)seleObj[i];
            }

            BodyDumbRule bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(seleBody, true);

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = bodyDumbRule1;
            scCollector1.ReplaceRules(rules1, false);
            try
            {
                return waveLinkBuilder1.CommitFeature();

            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AssmbliesUtils:WaveBodys:" + ex.Message);
                return null;
            }
            finally
            {
                extractFaceBuilder1.Destroy();
            }

        }

        /// <summary>
        /// 链接到工作部件中(关联)
        /// </summary>
        /// <param name="seleObj">要链接的face</param>
        /// <returns></returns>
        public static NXOpen.Features.Feature WaveFace(params Face[] seleObj)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
            NXOpen.Features.WaveLinkBuilder waveLinkBuilder1;
            waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullNXOpen_Features_Feature);
            NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
            extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;
            waveLinkBuilder1.Type = NXOpen.Features.WaveLinkBuilder.Types.FaceLink;
            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.SingleFace;
            extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;
            extractFaceBuilder1.Associative = true;

            extractFaceBuilder1.MakePositionIndependent = false;

            extractFaceBuilder1.FixAtCurrentTimestamp = false;

            extractFaceBuilder1.HideOriginal = false;

            extractFaceBuilder1.DeleteHoles = false;

            extractFaceBuilder1.InheritDisplayProperties = false;

            NXOpen.SelectDisplayableObjectList selectDisplayableObjectList1;
            selectDisplayableObjectList1 = extractFaceBuilder1.ObjectToExtract;
            extractFaceBuilder1.Associative = false;

            bool added1;
            added1 = selectDisplayableObjectList1.Add(seleObj);
            try
            {

                return waveLinkBuilder1.CommitFeature();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AssmbliesUtils:WaveFace:" + ex.Message);
                return null;
            }
            finally
            {

                waveLinkBuilder1.Destroy();
            }


        }

        /// <summary>
        /// 移动部件并复制部件
        /// </summary>
        /// <param name="compObj"></param>
        /// <param name="endPt"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static NXOpen.Assemblies.Component MoveCompCopyPart(NXOpen.Assemblies.Component compObj, Point3d endPt, Matrix4 mat)
        {
            Part workPart = theSession.Parts.Work;

            Matrix4 invers = mat.GetInversMatrix();
            invers.ApplyPos(ref endPt);

            NXOpen.Positioning.ComponentPositioner componentPositioner1;
            componentPositioner1 = workPart.ComponentAssembly.Positioner; //组件定位
            componentPositioner1.ClearNetwork();  //删除定位器
            NXOpen.Assemblies.Arrangement arrangement1 = (NXOpen.Assemblies.Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");  //布局
            componentPositioner1.PrimaryArrangement = arrangement1; //主要布局
            componentPositioner1.BeginMoveComponent();//开始移动组件
            bool allowInterpartPositioning1;
            allowInterpartPositioning1 = theSession.Preferences.Assemblies.InterpartPositioning; //首选项的部件间的定位
            NXOpen.Positioning.Network network1;
            network1 = componentPositioner1.EstablishNetwork(); //建立
            NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
            componentNetwork1.MoveObjectsState = true; //移动对象状态
            NXOpen.Assemblies.Component nullNXOpen_Assemblies_Component = null;
            componentNetwork1.DisplayComponent = nullNXOpen_Assemblies_Component; //显示组件
            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing; //现有安排模式
            componentNetwork1.RemoveAllConstraints(); //删除约束
            NXOpen.NXObject[] movableObjects1 = new NXOpen.NXObject[1];
            movableObjects1[0] = compObj;
            NXOpen.Assemblies.Component[] components1 = new NXOpen.Assemblies.Component[1];
            components1[0] = compObj;
            NXOpen.Assemblies.Component[] newComponents1 = workPart.ComponentAssembly.CopyComponents(components1);

            componentNetwork1.SetMovingGroup(newComponents1); //设置移动组件
            componentNetwork1.Solve(); //解除约束
            bool loaded1;
            loaded1 = componentNetwork1.IsReferencedGeometryLoaded(); //参考几何加载
            componentNetwork1.BeginDrag();  //操作即将开始

            NXOpen.Vector3d translation1 = new NXOpen.Vector3d(endPt.X, endPt.Y, endPt.Z);
            componentNetwork1.DragByTranslation(translation1); //移动
            componentNetwork1.EndDrag(); //操作结束
            componentNetwork1.ResetDisplay(); //返回到模型上
            componentNetwork1.ApplyToModel();//应该到当前模型
            componentNetwork1.Solve(); //解除约束
            componentPositioner1.ClearNetwork();  //清空
            int nErrs1;
            nErrs1 = theSession.UpdateManager.AddToDeleteList(componentNetwork1); //更新

            componentPositioner1.ClearNetwork();
            componentPositioner1.DeleteNonPersistentConstraints();
            componentPositioner1.EndMoveComponent();

            NXOpen.Assemblies.Arrangement nullNXOpen_Assemblies_Arrangement = null;
            componentPositioner1.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
            return newComponents1[0];

        }

        /// <summary>
        /// 移动部件
        /// </summary>
        /// <param name="compObj"></param>
        /// <param name="endPt"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static void MoveCompPart(NXOpen.Assemblies.Component compObj, Vector3d endPt, Matrix4 mat)
        {
            Part workPart = theSession.Parts.Work;

            Matrix4 invers = mat.GetInversMatrix();

            invers.ApplyVec(ref endPt);

            NXOpen.Positioning.ComponentPositioner componentPositioner1;
            componentPositioner1 = workPart.ComponentAssembly.Positioner; //组件定位
            componentPositioner1.ClearNetwork();  //删除定位器
            NXOpen.Assemblies.Arrangement arrangement1 = (NXOpen.Assemblies.Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");  //布局
            componentPositioner1.PrimaryArrangement = arrangement1; //主要布局
            componentPositioner1.BeginMoveComponent();//开始移动组件
            bool allowInterpartPositioning1;
            allowInterpartPositioning1 = theSession.Preferences.Assemblies.InterpartPositioning; //首选项的部件间的定位
            NXOpen.Positioning.Network network1;
            network1 = componentPositioner1.EstablishNetwork(); //建立
            NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
            componentNetwork1.MoveObjectsState = true; //移动对象状态
            NXOpen.Assemblies.Component nullNXOpen_Assemblies_Component = null;
            componentNetwork1.DisplayComponent = nullNXOpen_Assemblies_Component; //显示组件
            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing; //现有安排模式
            componentNetwork1.RemoveAllConstraints(); //删除约束

            NXOpen.Assemblies.Component[] components1 = new NXOpen.Assemblies.Component[1];
            components1[0] = compObj;

            componentNetwork1.SetMovingGroup(components1); //设置移动组件
            componentNetwork1.Solve(); //解除约束
            bool loaded1;
            loaded1 = componentNetwork1.IsReferencedGeometryLoaded(); //参考几何加载
            componentNetwork1.BeginDrag();  //操作即将开始

            NXOpen.Vector3d translation1 = endPt;
            componentNetwork1.DragByTranslation(translation1); //移动
            componentNetwork1.EndDrag(); //操作结束
            componentNetwork1.ResetDisplay(); //返回到模型上
            componentNetwork1.ApplyToModel();//应该到当前模型
            componentNetwork1.Solve(); //解除约束
            componentPositioner1.ClearNetwork();  //清空
            int nErrs1;
            nErrs1 = theSession.UpdateManager.AddToDeleteList(componentNetwork1); //更新

            componentPositioner1.ClearNetwork();
            componentPositioner1.DeleteNonPersistentConstraints();
            componentPositioner1.EndMoveComponent();

            NXOpen.Assemblies.Arrangement nullNXOpen_Assemblies_Arrangement = null;
            componentPositioner1.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;


        }

        /// <summary>
        /// 删除部件
        /// </summary>
        /// <param name="comp">部件</param>
        public static void DeleteComponent(params NXOpen.Assemblies.Component[] comp)
        {

            theSession.UpdateManager.ClearErrorList();
            NXOpen.Session.UndoMarkId markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Delete");
            try
            {
                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddToDeleteList(comp);
                bool notifyOnDelete2;
                notifyOnDelete2 = theSession.Preferences.Modeling.NotifyOnDelete;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AssmbliesUtils:DeleteComponent:" + ex.Message);
            }
            finally
            {
                int nErrs2;
                nErrs2 = theSession.UpdateManager.DoUpdate(markId2);
            }

        }

        /// <summary>
        /// 创建唯一
        /// </summary>
        /// <param name="component"></param>
        /// <param name="str"></param>
        public static NXObject MakeUnique(NXOpen.Assemblies.Component component, string newFileName)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Assemblies.MakeUniquePartBuilder makeUniquePartBuilder1;
            makeUniquePartBuilder1 = workPart.AssemblyManager.CreateMakeUniquePartBuilder();
            bool added1;
            added1 = makeUniquePartBuilder1.SelectedComponents.Add(component);
            Tag partTag = theUFSession.Assem.AskPrototypeOfOcc(component.Tag);

            try
            {
                NXOpen.Part part1 = (Part)NXObjectManager.Get(partTag);
                part1.SetMakeUniqueName(newFileName);
                NXOpen.NXObject nXObject1;
                nXObject1 = makeUniquePartBuilder1.Commit();
                return nXObject1;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AssmbliesUtils:MakeUnique:         " + ex.Message);
                return null;
            }
            finally
            {
                makeUniquePartBuilder1.Destroy();
            }



        }
    }
}
