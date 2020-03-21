using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.Annotations;
using NXOpen.Drawings;

namespace Basic
{
    public class DrawingUtils : ClassItem
    {
        /// <summary>
        /// 创建图纸模板
        /// </summary>
        /// <param name="sheetPath">模板地址</param>
        /// <param name="height">模板高</param>
        /// <param name="length">模板长</param>
        /// <param name="drawName">图纸名</param>
        /// <returns></returns>
        public static NXOpen.Drawings.DrawingSheet DrawingSheet(string sheetPath, double height, double length, string drawName)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Drawings.DrawingSheet nullNXOpen_Drawings_DrawingSheet = null;
            NXOpen.Drawings.DrawingSheetBuilder drawingSheetBuilder1;
            drawingSheetBuilder1 = workPart.DrawingSheets.DrawingSheetBuilder(nullNXOpen_Drawings_DrawingSheet);
            drawingSheetBuilder1.AutoStartViewCreation = true; //自动创建视图设置
            drawingSheetBuilder1.StandardMetricScale = NXOpen.Drawings.DrawingSheetBuilder.SheetStandardMetricScale.S11;     //设置图纸比例为1:1
            drawingSheetBuilder1.Height = height;
            drawingSheetBuilder1.Length = length;
            drawingSheetBuilder1.ScaleNumerator = 1.0; //自定义比例分子
            drawingSheetBuilder1.ScaleDenominator = 1.0; //自定义比例分母
            drawingSheetBuilder1.Units = NXOpen.Drawings.DrawingSheetBuilder.SheetUnits.Metric; //图纸单位为公制
            drawingSheetBuilder1.ProjectionAngle = NXOpen.Drawings.DrawingSheetBuilder.SheetProjectionAngle.Third; //投影视角为第三视角
            drawingSheetBuilder1.Number = "1"; //创建或编辑图纸的编号
            drawingSheetBuilder1.SecondaryNumber = ""; //创建或编辑图纸的次要编号
            drawingSheetBuilder1.Revision = "A"; //创建或编辑图纸的修订版
            drawingSheetBuilder1.MetricSheetTemplateLocation = sheetPath;   //模板地址
            drawingSheetBuilder1.Name = drawName;
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = drawingSheetBuilder1.Commit();
                nXObject1.SetName(drawName);
                return nXObject1 as NXOpen.Drawings.DrawingSheet;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils.DrawingSheet" + ex.Message);
                return null;
            }
            finally
            {
                drawingSheetBuilder1.Destroy();
            }
        }

        /// <summary>
        /// 创建装配明细表
        /// </summary>
        /// <param name="templatePath">模板地址</param>
        /// <param name="origin"></param>
        public static void CreatePlist(string templatePath, double[] origin)
        {
            Tag partsList = Tag.Null;
            Tag row = Tag.Null;
            try
            {
                theUFSession.Plist.CreateFromTemplate(templatePath, origin, out partsList);
                theUFSession.Tabnot.AskNthRow(partsList, 0, out row);
                theUFSession.Tabnot.RemoveRow(row);
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils.CreatePlist" + ex.Message);
            }

        }
        /// <summary>
        /// 创建表格注释
        /// </summary>
        /// <param name="templatePath">模板位置</param>
        /// <param name="origin">中心点</param>
        /// <param name="rows">要创建得行数</param>
        /// <returns></returns>
        public static Tag CreateTable(string templatePath, double[] origin, int rows)
        {

            Tag partsList = Tag.Null;
            theUFSession.Tabnot.CreateFromTemplate(templatePath, origin, out partsList);
            if (rows >= 2)
            {
                Tag rowTag;

                for (int i = 0; i < rows - 1; i++)
                {
                    theUFSession.Tabnot.CreateRow(5, out rowTag);
                    theUFSession.Tabnot.AddRow(partsList, rowTag, UFConstants.UF_TABNOT_APPEND);
                }

            }
            return partsList;
        }

        /// <summary>
        /// 创建主视图
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="point"></param>
        /// <param name="scaleNumerator"></param>
        /// <param name="mat"></param>
        /// <param name="hiddenComp"></param>
        /// <returns></returns>
        public static BaseView CreateView(string viewName, Point3d point, double scaleNumerator, Matrix4 mat, params NXOpen.Assemblies.Component[] hiddenComp)
        {
            Part workPart = theSession.Parts.Work;
            Point3d origin = new Point3d(0, 0, 0);
            Matrix4 inver = mat.GetInversMatrix();
            inver.ApplyPos(ref origin);
            Vector3d xVec = mat.GetXAxis();
            Vector3d zVec = mat.GetZAxis();
            Direction zDir = workPart.Directions.CreateDirection(origin, zVec, NXOpen.SmartObject.UpdateOption.AfterModeling);
            Direction xDir = workPart.Directions.CreateDirection(origin, xVec, NXOpen.SmartObject.UpdateOption.AfterModeling);
            NXOpen.Drawings.BaseView nullNXOpen_Drawings_BaseView = null;
            NXOpen.Drawings.BaseViewBuilder baseViewBuilder1;
            baseViewBuilder1 = workPart.DraftingViews.CreateBaseViewBuilder(nullNXOpen_Drawings_BaseView);
            NXOpen.ModelingView modelingView1 = (NXOpen.ModelingView)workPart.ModelingViews.FindObject(viewName);
            baseViewBuilder1.SelectModelView.SelectedView = modelingView1;
            baseViewBuilder1.Placement.Associative = false;
            baseViewBuilder1.SecondaryComponents.ObjectType = NXOpen.Drawings.DraftingComponentSelectionBuilder.Geometry.PrimaryGeometry;
            baseViewBuilder1.Style.ViewStyleBase.Part = workPart;
            baseViewBuilder1.Style.ViewStyleOrientation.Ovt.NormalDirection = zDir;
            baseViewBuilder1.Style.ViewStyleOrientation.Ovt.XDirection = xDir;
            baseViewBuilder1.HiddenObjects.Objects.Add(hiddenComp);
            baseViewBuilder1.Scale.Numerator = scaleNumerator;
            baseViewBuilder1.Scale.Denominator = 1.0;

            baseViewBuilder1.Placement.Placement.SetValue(null, workPart.Views.WorkView, point);

            try
            {

                return baseViewBuilder1.Commit() as BaseView;
            }

            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils:CreateView" + ex.Message);
                return null;
            }
            finally
            {
                baseViewBuilder1.Destroy();
            }

        }

        /// <summary>
        /// 创建主视图
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static DraftingView CreateView(string viewName, Point3d point, double scaleNumerator, Matrix4 mat)
        {
            Part workPart = theSession.Parts.Work;
            Point3d origin = new Point3d(0, 0, 0);
            Matrix4 inver = mat.GetInversMatrix();
            inver.ApplyPos(ref origin);
            Vector3d xVec = mat.GetXAxis();
            Vector3d zVec = mat.GetZAxis();
            Direction zDir = workPart.Directions.CreateDirection(origin, zVec, NXOpen.SmartObject.UpdateOption.AfterModeling);
            Direction xDir = workPart.Directions.CreateDirection(origin, xVec, NXOpen.SmartObject.UpdateOption.AfterModeling);

            NXOpen.Drawings.BaseView nullNXOpen_Drawings_BaseView = null;
            NXOpen.Drawings.BaseViewBuilder baseViewBuilder1;
            baseViewBuilder1 = workPart.DraftingViews.CreateBaseViewBuilder(nullNXOpen_Drawings_BaseView);
            NXOpen.ModelingView modelingView1 = (NXOpen.ModelingView)workPart.ModelingViews.FindObject(viewName);
            baseViewBuilder1.SelectModelView.SelectedView = modelingView1;
            baseViewBuilder1.Placement.Associative = false;
            baseViewBuilder1.SecondaryComponents.ObjectType = NXOpen.Drawings.DraftingComponentSelectionBuilder.Geometry.PrimaryGeometry;
            baseViewBuilder1.Style.ViewStyleBase.Part = workPart;
            baseViewBuilder1.Style.ViewStyleOrientation.Ovt.NormalDirection = zDir;
            baseViewBuilder1.Style.ViewStyleOrientation.Ovt.XDirection = xDir;
            baseViewBuilder1.Scale.Numerator = scaleNumerator;
            baseViewBuilder1.Scale.Denominator = 1.0;
            baseViewBuilder1.Placement.Placement.SetValue(null, workPart.Views.WorkView, point);
            try
            {
                return baseViewBuilder1.Commit() as DraftingView;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils:CreateView" + ex.Message);
                return null;
            }
            finally
            {
                baseViewBuilder1.Destroy();
            }

        }
        /// <summary>
        /// 创建投影视图
        /// </summary>
        /// <param name="baseView">主视图</param>
        /// <param name="scale">比例</param>
        /// <returns></returns>
        public static DraftingView CreateProjectedView(NXOpen.Drawings.DraftingView baseView, Point3d origin, double scale)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Drawings.ProjectedView nullNXOpen_Drawings_ProjectedView = null;
            NXOpen.Drawings.ProjectedViewBuilder projectedViewBuilder1;
            projectedViewBuilder1 = workPart.DraftingViews.CreateProjectedViewBuilder(nullNXOpen_Drawings_ProjectedView);
            projectedViewBuilder1.Placement.Associative = false; //放置是否联想
            projectedViewBuilder1.SecondaryComponents.ObjectType = NXOpen.Drawings.DraftingComponentSelectionBuilder.Geometry.PrimaryGeometry; //次要组件
            projectedViewBuilder1.Parent.View.Value = baseView;
            projectedViewBuilder1.Style.ViewStyleGeneral.Scale.Numerator = scale;
            projectedViewBuilder1.Style.ViewStyleBase.Arrangement.InheritArrangementFromParent = true;

            projectedViewBuilder1.Placement.Placement.SetValue(null, workPart.Views.WorkView, origin);
            try
            {
                return projectedViewBuilder1.Commit() as DraftingView;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils:CreateProjectedView" + ex.Message);
                return null;
            }
            finally
            {
                projectedViewBuilder1.Destroy();
            }
        }

        /// <summary>
        /// 更新视图
        /// </summary>
        public static void UpdateViews(NXOpen.Drawings.DrawingSheet sheet)
        {
            Part workPart = theSession.Parts.Work;
            List<NXOpen.Drawings.DraftingView> views = new List<DraftingView>();
            foreach (DraftingView dv in sheet.GetDraftingViews())
            {
                if (dv.IsOutOfDate)
                    views.Add(dv);
            }
            workPart.DraftingViews.UpdateViews(views.ToArray());
        }

        /// <summary>
        /// 横向标准
        /// </summary>
        /// <param name="view"></param>
        /// <param name="xMarginPos"></param>
        /// <param name="yMarginPos"></param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <param name="obj"></param>
        /// <param name="ordinateOriginDimension1"></param>
        /// <param name="isdogle"></param>
        /// <param name="tolerBasic"></param>
        /// <returns></returns>
        public static NXObject DimensionHorizontal(DraftingView view, double xMarginPos, double yMarginPos, double xPos, double yPos, NXObject obj, OrdinateOriginDimension ordinateOriginDimension1, bool isdogle, ref string errorMsg, bool tolerBasic = false)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;

            NXObject nullNXObject = null;
            Point3d origin2 = new Point3d(xMarginPos, yMarginPos, 0.0);
            OrdinateMargin ordinateMargin1;
            ordinateMargin1 = workPart.Annotations.OrdinateMargins.CreateInferredMargin(ordinateOriginDimension1, origin2, 13);


            DimensionData dimensionData2;
            dimensionData2 = workPart.Annotations.NewDimensionData();
            Associativity associativity3;
            associativity3 = workPart.Annotations.NewAssociativity();
            associativity3.FirstObject = ordinateOriginDimension1;
            associativity3.SecondObject = nullNXObject;
            associativity3.ObjectView = workPart.Views.WorkView;
            associativity3.PointOption = AssociativityPointOption.Control;
            associativity3.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint2 = new Point3d(0.0, 0.0, 0.0);
            associativity3.FirstDefinitionPoint = firstDefinitionPoint2;
            Point3d secondDefinitionPoint2 = new Point3d(0.0, 0.0, 0.0);
            associativity3.SecondDefinitionPoint = secondDefinitionPoint2;
            associativity3.Angle = 0.0;
            Point3d pickPoint2 = new Point3d(0.0, 0.0, 0.0);
            associativity3.PickPoint = pickPoint2;
            Associativity[] associativity4 = new Associativity[1];
            associativity4[0] = associativity3;
            dimensionData2.SetAssociativity(1, associativity4);

            Associativity associativity5;
            associativity5 = workPart.Annotations.NewAssociativity();
            associativity5.FirstObject = obj;
            associativity5.SecondObject = nullNXObject;
            associativity5.ObjectView = view;
            associativity5.PointOption = AssociativityPointOption.Control;
            associativity5.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint3 = new Point3d(0.0, 0.0, 0.0);
            associativity5.FirstDefinitionPoint = firstDefinitionPoint3;
            Point3d secondDefinitionPoint3 = new Point3d(0.0, 0.0, 0.0);
            associativity5.SecondDefinitionPoint = secondDefinitionPoint3;
            associativity5.Angle = 0.0;
            //	Point3d pickPoint3(-272.0, -147.259337018431, -100.945677475);
            Point3d pickPoint3 = new Point3d(0.0, 0.0, 0.0);
            associativity5.PickPoint = pickPoint3;
            Associativity[] associativity6 = new Associativity[1];
            associativity6[0] = associativity5;
            dimensionData2.SetAssociativity(2, associativity6);

            Associativity associativity7;
            associativity7 = workPart.Annotations.NewAssociativity();
            HorizontalOrdinateMargin horizontalOrdinateMargin1 = ordinateMargin1 as HorizontalOrdinateMargin;
            associativity7.FirstObject = horizontalOrdinateMargin1;
            associativity7.SecondObject = nullNXObject;
            View nullView = null;
            associativity7.ObjectView = nullView;
            associativity7.PointOption = AssociativityPointOption.Control;
            associativity7.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint4 = new Point3d(0.0, 0.0, 0.0);
            associativity7.FirstDefinitionPoint = firstDefinitionPoint4;
            Point3d secondDefinitionPoint4 = new Point3d(0.0, 0.0, 0.0);
            associativity7.SecondDefinitionPoint = secondDefinitionPoint4;
            associativity7.Angle = 0.0;
            Point3d pickPoint4 = new Point3d(0.0, 0.0, 0.0);
            associativity7.PickPoint = pickPoint4;
            Associativity[] associativity8 = new Associativity[1];
            associativity8[0] = associativity7;
            dimensionData2.SetAssociativity(3, associativity8);




            if (isdogle)
            {
                DimensionPreferences dimensionPreferences1;
                dimensionPreferences1 = workPart.Annotations.Preferences.GetDimensionPreferences();

                OrdinateDimensionPreferences ordinateDimensionPreferences1;
                ordinateDimensionPreferences1 = dimensionPreferences1.GetOrdinateDimensionPreferences();
                ordinateDimensionPreferences1.DoglegCreationOption = OrdinateDoglegCreationOption.Yes;
                dimensionPreferences1.SetOrdinateDimensionPreferences(ordinateDimensionPreferences1);
                dimensionData2.SetDimensionPreferences(dimensionPreferences1);
            }
            HorizontalOrdinateDimension horizontalOrdinateDimension1 = null;
            try
            {
                Point3d origin3 = new Point3d(xPos, yPos, 0.0);
                horizontalOrdinateDimension1 = workPart.Dimensions.CreateHorizontalOrdinateDimension(dimensionData2, origin3);

                //NXOpen.Annotations.LetteringPreferences letteringPreferences1;
                //letteringPreferences1 = horizontalOrdinateDimension1.GetLetteringPreferences();

                //NXOpen.Annotations.Lettering dimensionText1;
                //dimensionText1.Size = 18.0;
                //dimensionText1.CharacterSpaceFactor = 1.0;
                //dimensionText1.AspectRatio = 1.0;
                //dimensionText1.LineSpaceFactor = 1.0;
                //dimensionText1.Cfw.Color = 125;
                //dimensionText1.Cfw.Font = 2;
                //dimensionText1.Cfw.Width = NXOpen.Annotations.LineWidth.Thin;
                //dimensionText1.Italic = false;
                //letteringPreferences1.SetDimensionText(dimensionText1);
                //horizontalOrdinateDimension1.SetLetteringPreferences(letteringPreferences1);
                NXOpen.Annotations.LinearTolerance tolPre = horizontalOrdinateDimension1.GetTolerance();
                tolPre.PrimaryDimensionDecimalPlaces = 2;

                if (tolerBasic)
                    tolPre.ToleranceType = NXOpen.Annotations.ToleranceType.Basic;
                horizontalOrdinateDimension1.SetTolerance(tolPre);

                //自己修改的部分
                //
                //设置尺寸为无公差样式
                horizontalOrdinateDimension1.ToleranceType = ToleranceType.None;
                //



                return horizontalOrdinateDimension1;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                LogMgr.WriteLog("DrawingUtils:DimensionHorizontal" + ex.Message);
            }

            return horizontalOrdinateDimension1;
        }

        /// <summary>
        /// 水平标注
        /// </summary>
        /// <param name="view"></param>
        /// <param name="ori"></param>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public static Dimension DimensionHorizontal(DraftingView view, Point3d ori, NXObject obj1, NXObject obj2, ref string errorMsg)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;

            NXObject nullNXObject = null;
            DimensionData dimensionData2;
            dimensionData2 = workPart.Annotations.NewDimensionData();
            Associativity associativity5;
            associativity5 = workPart.Annotations.NewAssociativity();
            associativity5.FirstObject = obj1;
            associativity5.SecondObject = nullNXObject;
            associativity5.ObjectView = view;
            associativity5.PointOption = AssociativityPointOption.Control;
            associativity5.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint3 = new Point3d(0.0, 0.0, 0.0);
            associativity5.FirstDefinitionPoint = firstDefinitionPoint3;

            Point3d secondDefinitionPoint3 = new Point3d(0.0, 0.0, 0.0);
            associativity5.SecondDefinitionPoint = secondDefinitionPoint3;
            associativity5.Angle = 0.0;
            Point3d pickPoint3 = new Point3d(0, 0, 0);
            associativity5.PickPoint = pickPoint3;

            Associativity[] associativity6 = new Associativity[1];
            associativity6[0] = associativity5;
            dimensionData2.SetAssociativity(1, associativity6);

            Associativity associativity7;
            associativity7 = workPart.Annotations.NewAssociativity();
            associativity7.FirstObject = obj2;
            associativity7.SecondObject = nullNXObject;
            associativity7.ObjectView = view;
            associativity7.PointOption = AssociativityPointOption.Control;
            associativity7.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint4 = new Point3d(0.0, 0.0, 0.0);
            associativity7.FirstDefinitionPoint = firstDefinitionPoint4;
            Point3d secondDefinitionPoint4 = new Point3d(0.0, 0.0, 0.0);
            associativity7.SecondDefinitionPoint = secondDefinitionPoint4;
            associativity7.Angle = 0.0;
            Point3d pickPoint4 = new Point3d(0.0, 0.0, 0.0);
            associativity7.PickPoint = pickPoint4;

            Associativity[] associativity8 = new Associativity[1];
            associativity8[0] = associativity7;
            dimensionData2.SetAssociativity(2, associativity8);


            try
            {
                /* Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
                 HorizontalDimension horizontalDimension2;
                 horizontalDimension2 = workPart.Dimensions.CreateHorizontalDimension(dimensionData2, origin2);

                 NXOpen.Annotations.LinearTolerance tolPre = horizontalDimension2.GetTolerance();
                 tolPre.PrimaryDimensionDecimalPlaces = 2;
                 horizontalDimension2.SetTolerance(tolPre);

                 Point3d origin3 = new Point3d(ori.X, ori.Y, ori.Z);
                 horizontalDimension2.AnnotationOrigin = origin3;
                 horizontalDimension2.LeaderOrientation = LeaderOrientation.FromLeft;
                 * 
                 * */

                Point3d origin2 = new Point3d(ori.X, ori.Y, ori.Z);
                HorizontalDimension horizontalDimension2;
                horizontalDimension2 = workPart.Dimensions.CreateHorizontalDimension(dimensionData2, origin2);
                //NXOpen.Annotations.LinearTolerance tolPre = horizontalDimension2.GetTolerance();
                //tolPre.PrimaryDimensionDecimalPlaces = 2;
                //horizontalDimension2.SetTolerance(tolPre);//设置小数点后两位
                return horizontalDimension2;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                LogMgr.WriteLog("DrawingUtils:DimensionHorizontal" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 竖直标注
        /// </summary>
        /// <param name="view"></param>
        /// <param name="ori"></param>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public static Dimension DimensionVertical(DraftingView view, Point3d ori, NXObject obj1, NXObject obj2, ref string errorMsg)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;

            NXObject nullNXObject = null;
            DimensionData dimensionData2;
            dimensionData2 = workPart.Annotations.NewDimensionData();
            Associativity associativity5;
            associativity5 = workPart.Annotations.NewAssociativity();
            associativity5.FirstObject = obj1;
            associativity5.SecondObject = nullNXObject;
            associativity5.ObjectView = view;
            associativity5.PointOption = AssociativityPointOption.Control;
            associativity5.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint3 = new Point3d(0.0, 0.0, 0.0);
            associativity5.FirstDefinitionPoint = firstDefinitionPoint3;

            Point3d secondDefinitionPoint3 = new Point3d(0.0, 0.0, 0.0);
            associativity5.SecondDefinitionPoint = secondDefinitionPoint3;
            associativity5.Angle = 0.0;
            Point3d pickPoint3 = new Point3d(0, 0, 0);
            associativity5.PickPoint = pickPoint3;

            Associativity[] associativity6 = new Associativity[1];
            associativity6[0] = associativity5;
            dimensionData2.SetAssociativity(1, associativity6);

            Associativity associativity7;
            associativity7 = workPart.Annotations.NewAssociativity();
            associativity7.FirstObject = obj2;
            associativity7.SecondObject = nullNXObject;
            associativity7.ObjectView = view;
            associativity7.PointOption = AssociativityPointOption.Control;
            associativity7.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint4 = new Point3d(0.0, 0.0, 0.0);
            associativity7.FirstDefinitionPoint = firstDefinitionPoint4;
            Point3d secondDefinitionPoint4 = new Point3d(0.0, 0.0, 0.0);
            associativity7.SecondDefinitionPoint = secondDefinitionPoint4;
            associativity7.Angle = 0.0;
            Point3d pickPoint4 = new Point3d(0.0, 0.0, 0.0);
            associativity7.PickPoint = pickPoint4;

            Associativity[] associativity8 = new Associativity[1];
            associativity8[0] = associativity7;
            dimensionData2.SetAssociativity(2, associativity8);

            try
            {
                Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
                VerticalDimension verticalDimension2;
                verticalDimension2 = workPart.Dimensions.CreateVerticalDimension(dimensionData2, origin2);

                //NXOpen.Annotations.LinearTolerance tolPre = verticalDimension2.GetTolerance();
                //tolPre.PrimaryDimensionDecimalPlaces = 2;
                //verticalDimension2.SetTolerance(tolPre);  //设置小数点后两位

                Point3d origin3 = new Point3d(ori.X, ori.Y, ori.Z);
                verticalDimension2.AnnotationOrigin = origin3;
                verticalDimension2.LeaderOrientation = LeaderOrientation.FromLeft;
                return verticalDimension2;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                LogMgr.WriteLog("DrawingUtils:DimensionVertical" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 竖直标注
        /// </summary>
        /// <param name="view"></param>
        /// <param name="xMarginPos"></param>
        /// <param name="yMarginPos"></param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <param name="obj"></param>
        /// <param name="ordinateOriginDimension1"></param>
        /// <param name="isdogle"></param>
        /// <param name="tolerBasic"></param>
        /// <returns></returns>
        public static NXObject DimensionVertical(DraftingView view, double xMarginPos, double yMarginPos, double xPos, double yPos, NXObject obj, OrdinateOriginDimension ordinateOriginDimension1, bool isdogle, ref string errorMsg, bool tolerBasic = false)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;

            Point3d origin4 = new Point3d(xMarginPos, yMarginPos, 0.0);
            OrdinateMargin ordinateMargin2;
            ordinateMargin2 = workPart.Annotations.OrdinateMargins.CreateInferredMargin(ordinateOriginDimension1, origin4, 14);

            NXObject nullNXObject = null;
            DimensionData dimensionData3;
            dimensionData3 = workPart.Annotations.NewDimensionData();

            Associativity associativity9;
            associativity9 = workPart.Annotations.NewAssociativity();
            associativity9.FirstObject = ordinateOriginDimension1;
            associativity9.SecondObject = nullNXObject;
            associativity9.ObjectView = workPart.Views.WorkView;
            associativity9.PointOption = AssociativityPointOption.None;
            associativity9.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint5 = new Point3d(0.0, 0.0, 0.0);
            associativity9.FirstDefinitionPoint = firstDefinitionPoint5;
            Point3d secondDefinitionPoint5 = new Point3d(0.0, 0.0, 0.0);
            associativity9.SecondDefinitionPoint = secondDefinitionPoint5;
            associativity9.Angle = 0.0;
            Point3d pickPoint5 = new Point3d(0.0, 0.0, 0.0);
            associativity9.PickPoint = pickPoint5;

            Associativity[] associativity10 = new Associativity[1];
            associativity10[0] = associativity9;
            dimensionData3.SetAssociativity(1, associativity10);


            Associativity associativity11;
            associativity11 = workPart.Annotations.NewAssociativity();
            associativity11.FirstObject = obj;
            associativity11.SecondObject = nullNXObject;
            associativity11.ObjectView = view;
            associativity11.PointOption = AssociativityPointOption.Control;
            associativity11.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint6 = new Point3d(0.0, 0.0, 0.0);
            associativity11.FirstDefinitionPoint = firstDefinitionPoint6;
            Point3d secondDefinitionPoint6 = new Point3d(0.0, 0.0, 0.0);
            associativity11.SecondDefinitionPoint = secondDefinitionPoint6;
            associativity11.Angle = 0.0;
            Point3d pickPoint6 = new Point3d(0.0, 0.0, 0.0);
            associativity11.PickPoint = pickPoint6;
            Associativity[] associativity12 = new Associativity[1];
            associativity12[0] = associativity11;
            dimensionData3.SetAssociativity(2, associativity12);

            Associativity associativity13;
            associativity13 = workPart.Annotations.NewAssociativity();
            VerticalOrdinateMargin verticalOrdinateMargin1 = ordinateMargin2 as VerticalOrdinateMargin;
            associativity13.FirstObject = verticalOrdinateMargin1;
            associativity13.SecondObject = nullNXObject;
            View nullView = null;
            associativity13.ObjectView = nullView;
            associativity13.PointOption = AssociativityPointOption.None;
            associativity13.LineOption = AssociativityLineOption.None;
            Point3d firstDefinitionPoint7 = new Point3d(0.0, 0.0, 0.0);
            associativity13.FirstDefinitionPoint = firstDefinitionPoint7;
            Point3d secondDefinitionPoint7 = new Point3d(0.0, 0.0, 0.0);
            associativity13.SecondDefinitionPoint = secondDefinitionPoint7;
            associativity13.Angle = 0.0;
            Point3d pickPoint7 = new Point3d(0.0, 0.0, 0.0);
            associativity13.PickPoint = pickPoint7;
            Associativity[] associativity14 = new Associativity[1];
            associativity14[0] = associativity13;
            dimensionData3.SetAssociativity(3, associativity14);

            if (isdogle)
            {
                DimensionPreferences dimensionPreferences1;
                dimensionPreferences1 = workPart.Annotations.Preferences.GetDimensionPreferences();

                OrdinateDimensionPreferences ordinateDimensionPreferences1;
                ordinateDimensionPreferences1 = dimensionPreferences1.GetOrdinateDimensionPreferences();
                ordinateDimensionPreferences1.DoglegCreationOption = OrdinateDoglegCreationOption.Yes;
                dimensionPreferences1.SetOrdinateDimensionPreferences(ordinateDimensionPreferences1);
                dimensionData3.SetDimensionPreferences(dimensionPreferences1);
            }

            VerticalOrdinateDimension verticalOrdinateDimension1 = null;
            try
            {
                Point3d origin5 = new Point3d(xPos, yPos, 0.0);
                verticalOrdinateDimension1 = workPart.Dimensions.CreateVerticalOrdinateDimension(dimensionData3, origin5);

                //NXOpen.Annotations.LetteringPreferences letteringPreferences1;
                //letteringPreferences1 = verticalOrdinateDimension1.GetLetteringPreferences();

                //NXOpen.Annotations.Lettering dimensionText1;
                //dimensionText1.Size = 18.0;
                //dimensionText1.CharacterSpaceFactor = 1.0;
                //dimensionText1.AspectRatio = 1.0;
                //dimensionText1.LineSpaceFactor = 1.0;
                //dimensionText1.Cfw.Color = 125;
                //dimensionText1.Cfw.Font = 2;
                //dimensionText1.Cfw.Width = NXOpen.Annotations.LineWidth.Thin;
                //dimensionText1.Italic = false;
                //letteringPreferences1.SetDimensionText(dimensionText1);
                //verticalOrdinateDimension1.SetLetteringPreferences(letteringPreferences1);

                NXOpen.Annotations.LinearTolerance tolPre = verticalOrdinateDimension1.GetTolerance();
                tolPre.PrimaryDimensionDecimalPlaces = 2;
                if (tolerBasic)
                    tolPre.ToleranceType = NXOpen.Annotations.ToleranceType.Basic;
                verticalOrdinateDimension1.SetTolerance(tolPre);

                return verticalOrdinateDimension1;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                LogMgr.WriteLog("DrawingUtils:DimensionVertical" + ex.Message);
            }

            return verticalOrdinateDimension1;
        }
        /// <summary>
        /// 显示组件
        /// </summary>
        /// <param name="baseView"></param>
        /// <param name="comp"></param>
        public static void ShowComponent(NXOpen.Drawings.DraftingView baseView, params NXOpen.Assemblies.Component[] comp)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Assemblies.ShowComponentBuilder showComponentBuilder1;
            showComponentBuilder1 = workPart.AssemblyManager.CreateShowComponentBuilder();

            bool added1;
            added1 = showComponentBuilder1.Views.Add(baseView);

            NXOpen.SelectTaggedObjectList selectTaggedObjectList1;
            selectTaggedObjectList1 = showComponentBuilder1.Components;
            bool added2;
            added2 = selectTaggedObjectList1.Add(comp);
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = showComponentBuilder1.Commit();
            }

            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils:ShowComponent" + ex.Message);
            }
            finally
            {
                showComponentBuilder1.Destroy();
            }


        }
        /// <summary>
        /// 隐藏组件
        /// </summary>
        /// <param name="baseView"></param>
        /// <param name="comp"></param>
        public static void HideComponent(NXOpen.Drawings.DraftingView baseView, params NXOpen.Assemblies.Component[] comp)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Assemblies.HideComponentBuilder hideComponentBuilder1;
            hideComponentBuilder1 = workPart.AssemblyManager.CreateHideComponentBuilder();
            bool added1;
            added1 = hideComponentBuilder1.Components.Add(comp);
            bool added2;
            added2 = hideComponentBuilder1.Views.Add(baseView);
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = hideComponentBuilder1.Commit();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils:HideComponent" + ex.Message);
            }
            finally
            {
                hideComponentBuilder1.Destroy();
            }
        }

        /// <summary>
        /// 设置层为可见
        /// </summary>
        /// <param name="view"></param>
        /// <param name="layers"></param>
        public static void SetLayerVisible(int[] layers, params NXObject[] view)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Layer.StateInfo[] stateArray1 = new NXOpen.Layer.StateInfo[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                stateArray1[i] = new NXOpen.Layer.StateInfo(layers[i], NXOpen.Layer.State.Visible);
            }
            for (int i = 0; i < view.Length; i++)
            {
                workPart.Layers.SetObjectsVisibilityOnLayer((NXOpen.Drawings.DraftingView)view[i], stateArray1, true);
            }

        }

        /// <summary>
        /// 设置所层不可见
        /// </summary>
        /// <param name="view"></param>
        /// <param name="layers"></param>
        public static void SetLayerHidden(params NXObject[] view)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Layer.StateInfo[] stateArray1 = new NXOpen.Layer.StateInfo[256];
            for (int i = 0; i < 256; i++)
            {
                stateArray1[i] = new NXOpen.Layer.StateInfo(i, NXOpen.Layer.State.Hidden);
            }
            for (int i = 0; i < view.Length; i++)
            {
                workPart.Layers.SetObjectsVisibilityOnLayer((NXOpen.Drawings.DraftingView)view[i], stateArray1, true);
            }

        }

        public static NXOpen.Drawings.DrawingSheet DrawingSheetByName(string sheetName)
        {
            Part workPart = Session.GetSession().Parts.Work;
            DrawingSheet sheet = null;
            foreach (DrawingSheet st in workPart.DrawingSheets)
            {
                if (st.Name.Equals(sheetName))
                    sheet = st;
            }
            return sheet;
        }
        /// <summary>
        /// 添加尺寸注释
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="lineName"></param>
        public static void AppendedTextDim(Dimension dim, params string[] lineName)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Annotations.AppendedTextEditorBuilder appendedTextEditorBuilder1;
            appendedTextEditorBuilder1 = workPart.Dimensions.CreateAppendedTextEditorBuilder(dim);
            appendedTextEditorBuilder1.AppendedTextBuilder.SetBelow(lineName);
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = appendedTextEditorBuilder1.Commit();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils:AppendedTextDim" + ex.Message);
            }
            finally
            {
                appendedTextEditorBuilder1.Destroy();
            }
        }
        /// <summary>
        /// 标注公差
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="type">类型</param>
        /// <param name="upTolerance">上公差</param>
        /// <param name="lowerTolerance">下公差</param>
        public static void ToleranceDim(Dimension dim, NXOpen.Annotations.ToleranceType type, double upTolerance, double lowerTolerance)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.DisplayableObject[] objects1 = new NXOpen.DisplayableObject[1];
            objects1[0] = dim;
            NXOpen.Annotations.EditSettingsBuilder editSettingsBuilder1;
            editSettingsBuilder1 = workPart.SettingsManager.CreateAnnotationEditSettingsBuilder(objects1);

            NXOpen.Drafting.BaseEditSettingsBuilder[] editsettingsbuilders1 = new NXOpen.Drafting.BaseEditSettingsBuilder[1];
            editsettingsbuilders1[0] = editSettingsBuilder1;
            workPart.SettingsManager.ProcessForMultipleObjectsSettings(editsettingsbuilders1);
            editSettingsBuilder1.AnnotationStyle.DimensionStyle.ToleranceType = type;
            editSettingsBuilder1.AnnotationStyle.DimensionStyle.UpperToleranceMetric = upTolerance;
            editSettingsBuilder1.AnnotationStyle.DimensionStyle.LowerToleranceMetric = lowerTolerance;

            try
            {

                NXOpen.NXObject nXObject1;
                nXObject1 = editSettingsBuilder1.Commit();
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils:ToleranceDim" + ex.Message);
            }
            finally
            {
                editSettingsBuilder1.Destroy();

            }

        }

        public static NXObject SetNote(Point3d point, double size, params string[] text)
        {
            Part workPart = theSession.Parts.Work;
            NXOpen.Annotations.SimpleDraftingAid nullNXOpen_Annotations_SimpleDraftingAid = null;
            NXOpen.Annotations.DraftingNoteBuilder draftingNoteBuilder1;
            draftingNoteBuilder1 = workPart.Annotations.CreateDraftingNoteBuilder(nullNXOpen_Annotations_SimpleDraftingAid);
            int fontIndex1;
            fontIndex1 = workPart.Fonts.AddFont("SimSun", NXOpen.FontCollection.Type.Standard);
            draftingNoteBuilder1.Style.LetteringStyle.GeneralTextSize = size;
            draftingNoteBuilder1.Style.LetteringStyle.AppendedTextFont = fontIndex1;
            draftingNoteBuilder1.Style.LetteringStyle.GeneralTextColor = workPart.Colors.Find(6);
            draftingNoteBuilder1.Text.TextBlock.SetText(text);
            NXOpen.View nullNXOpen_View = null;
            draftingNoteBuilder1.Origin.Origin.SetValue(null, nullNXOpen_View, point);
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = draftingNoteBuilder1.Commit();
                return nXObject1;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("DrawingUtils:SetNote" + ex.Message);
                return null;
            }
            finally
            {
                draftingNoteBuilder1.Destroy();
            }


        }
    }
}
