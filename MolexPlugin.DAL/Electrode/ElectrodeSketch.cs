using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using Basic;


namespace MolexPlugin.DAL
{
    public class ElectrodeSketchBuilder
    {
        private Part workPart;
        private UFSession theUFSession;
        private double preparationLength;
        private double preparationWigth;
        private double zSetValue;

        public Line[] WaiLine { get; private set; } = new Line[4];
        public Line[] LeiLine { get; private set; } = new Line[4];

        public Line[] CenterLine { get; private set; } = new Line[2];

        public Curve Center { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">备料长度</param>
        /// <param name="wigth">备料宽度</param>
        /// <param name="z">z向高度</param>
        public ElectrodeSketchBuilder(double length, double wigth, double z)
        {
            workPart = Session.GetSession().Parts.Work;
            theUFSession = UFSession.GetUFSession();
            this.preparationLength = length;
            this.preparationWigth = wigth;
            this.zSetValue = z;
        }
        /// <summary>
        /// 创建长方体
        /// </summary>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private Line[] CreateRectangle(double length, double width, double z)
        {
            Line line1 = workPart.Curves.CreateLine(new Point3d(length / 2, width / 2, z), new Point3d(-length / 2, width / 2, z)); //横
            Line line2 = workPart.Curves.CreateLine(line1.StartPoint, new Point3d(length / 2, -width / 2, z)); //竖
            Line line3 = workPart.Curves.CreateLine(line2.EndPoint, new Point3d(-length / 2, -width / 2, z)); //横
            Line line4 = workPart.Curves.CreateLine(line3.EndPoint, line1.EndPoint); //竖
            return new Line[] { line1, line2, line3, line4 };
        }
        /// <summary>
        /// 创建中心线
        /// </summary>
        /// <param name="z"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        private Line[] CreateCenterLine(double z)
        {
            Line line1 = workPart.Curves.CreateLine(new Point3d(preparationLength / 2 + 5, 0, z), new Point3d(-preparationLength / 2 - 5, 0, z)); //横
            Line line2 = workPart.Curves.CreateLine(new Point3d(0, preparationWigth / 2 + 5, z), new Point3d(0, -preparationWigth / 2 - 5, z)); //竖


            return new Line[] { line1, line2 };

        }
        /// <summary>
        /// 设置中心线类型
        /// </summary>
        /// <param name="lineTag"></param>
        /// <param name="color"></param>
        /// <param name="layer"></param>
        /// <param name="font"></param>
        /// <param name="name"></param>
        private void SetLineObj(int layer, params Line[] line)
        {
            foreach (Line temp in line)
            {
                theUFSession.Obj.SetColor(temp.Tag, 186);
                theUFSession.Obj.SetLayer(temp.Tag, layer);
                theUFSession.Obj.SetFont(temp.Tag, 7);
            }

        }
        /// <summary>
        /// 设置线名字
        /// </summary>
        /// <param name="name"></param>
        /// <param name="line"></param>
        private void SetLineName(string name, params Line[] line)
        {
            foreach (Line temp in line)
            {
                theUFSession.Obj.SetName(temp.Tag, name);

            }
        }
        /// <summary>
        /// 创建草绘
        /// </summary>
        /// <param name="waiLine"></param>
        /// <param name="leiLine"></param>
        public void CreateEleSketch()
        {
            Tag sketchTag = SketchUtils.CreateShetch(zSetValue, "SKETCH_01");
            this.WaiLine = CreateRectangle(this.preparationLength, this.preparationWigth, this.zSetValue);
            this.LeiLine = CreateRectangle(this.preparationLength - 2, this.preparationWigth - 2, this.zSetValue);
            Line[] centerLine = CreateCenterLine(this.zSetValue);
            Line[] centerLine2 = CreateCenterLine(0);
            SetLineObj(254, centerLine);
            SetLineObj(1, centerLine2);
            SetLineName("XCenterLine", centerLine2[0]);
            SetLineName("YCenterLine", centerLine2[1]);
            SketchUtils.AddShetch(sketchTag, this.WaiLine);
            SketchUtils.AddShetch(sketchTag, this.LeiLine);
            SketchUtils.AddShetch(sketchTag, centerLine);

            theUFSession.Sket.UpdateSketch(sketchTag);
            this.Center = CreateCenter(zSetValue);
            SetSketchConstraint(centerLine);
            SetSketch(this.WaiLine, centerLine, 0);
            SetSketch(this.LeiLine, centerLine, 1.0);
            theUFSession.Obj.SetLayer(sketchTag, 254);
            theUFSession.Obj.SetLayer(Center.Tag, 254);
            DisplayableObject[] disp = workPart.Datums.ToArray();
            for (int i = 0; i < disp.Length; i++)
            {
                theUFSession.Obj.SetLayer(disp[i].Tag, 254);
            }

        }
        /// <summary>
        /// 创建尺寸约束
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="center"></param>
        /// <param name="i"></param>
        private void SetSketch(Line[] rectangle, Line[] center, double i)
        {
            Point3d dimOrigin = new Point3d(0, 0, 0);


            Expression ex1 = SketchUtils.CreateDim(center[0], rectangle[0], dimOrigin, NXOpen.Annotations.DimensionMeasurementBuilder.MeasurementMethod.Vertical, InferSnapType.SnapType.Origin);
            Expression ex2 = SketchUtils.CreateDim(center[0], rectangle[2], dimOrigin, NXOpen.Annotations.DimensionMeasurementBuilder.MeasurementMethod.Vertical, InferSnapType.SnapType.Origin);
            ex1.RightHandSide = "PreparationY/2-" + i.ToString();
            ex2.RightHandSide = "PreparationY/2-" + i.ToString();

            Expression ex3 = SketchUtils.CreateDim(center[1], rectangle[1], dimOrigin, NXOpen.Annotations.DimensionMeasurementBuilder.MeasurementMethod.Horizontal, InferSnapType.SnapType.Origin);
            Expression ex4 = SketchUtils.CreateDim(center[1], rectangle[3], dimOrigin, NXOpen.Annotations.DimensionMeasurementBuilder.MeasurementMethod.Horizontal, InferSnapType.SnapType.Origin);

            ex3.RightHandSide = "PreparationX/2-" + i.ToString();
            ex4.RightHandSide = "PreparationX/2-" + i.ToString();
        }
        /// <summary>
        /// 创建位置约束
        /// </summary>
        /// <param name="center"></param>
        private void SetSketchConstraint(Line[] center)
        {
            SketchUtils.CreateConstraint(center[0], SketchConstraintBuilder.Constraint.Parallel, new NXObject[] { this.WaiLine[0], this.WaiLine[2], this.LeiLine[0], this.LeiLine[2] });   //平行约束
            SketchUtils.CreateConstraint(center[1], SketchConstraintBuilder.Constraint.Parallel, new NXObject[] { this.WaiLine[1], this.WaiLine[3], this.LeiLine[1], this.LeiLine[3] });
            SketchUtils.SetFixed(center);
        }
        /// <summary>
        /// 画基准台圆
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>        
        private Curve CreateCenter(double z)
        {
            Session theSession = Session.GetSession();
            Vector3d vecX = new Vector3d(1, 0, 0);
            Vector3d vecY = new Vector3d(0, 1, 0);
            Arc arc1 = workPart.Curves.CreateArc(new Point3d(0, 0, z), vecX, vecY, 2.5, 0, Math.PI * 2);
            return arc1;
        }

    }
}
