using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;

namespace MolexPlugin.DAL
{
    public abstract class AbstractElectrodeSetValue
    {
        protected ElectrodeHeadInfo head;
       

        public AbstractElectrodeSetValue(ElectrodeHeadInfo head)
        {
            this.head = head;
           
        }
        /// <summary>
        /// 单齿设定值
        /// </summary>
        /// <returns></returns>
        public abstract Point3d GetSingleHeadSetValue();
        /// <summary>
        /// 获取电极设定值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xNumber"></param>
        /// <param name="y"></param>
        /// <param name="yNumber"></param>
        /// <returns></returns>
        public Point3d GetHeadSetValue(double x, int xNumber, double y, int yNumber,bool zDatum)
        {
            Point3d temp = GetSingleHeadSetValue();
            double x1 = temp.X + (xNumber - 1) * x / 2;
            double y1 = temp.Y + (yNumber - 1) * y / 2;
            if (zDatum)
            {
                double[] pre = GetPreparation(x, xNumber, y, yNumber,  zDatum);
                if (pre[0] >= pre[1])
                {
                    x1 = temp.X + (xNumber - 2) * x / 2;
                }
                else
                {
                    y1 = temp.Y + (yNumber - 2) * y / 2;
                }
            }
            return new Point3d(x1, y1, temp.Z);
        }

      
        /// <summary>
        /// 创建表达式
        /// </summary>
        public abstract void CreateExp(double x, int xNumber, double y, int yNumber, bool zDatum);       
        /// <summary>
        /// 获取电极矩阵
        /// </summary>
        /// <param name="pre">备料</param>
        /// <returns></returns>
        public abstract Matrix4 GetEleMatr(int[] pre,Point3d center);
        /// <summary>
        /// 获取备料
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xNumber"></param>
        /// <param name="y"></param>
        /// <param name="yNumber"></param>
        /// <returns></returns>
        public abstract double[] GetPreparation(double x, int xNumber, double y, int yNumber, bool zDatum);
        /// <summary>
        /// 获取电极高度
        /// </summary>
        /// <param name="exp">拉伸</param>
        /// <returns></returns>
        public abstract double GetZHeight(double exp);

        protected void CreateDefault(bool zDatum)
        {
            ExpressionUtils.SetAttrExp("PitchX", "PitchX", NXObject.AttributeType.Real);
            ExpressionUtils.SetAttrExp("PitchXNum", "PitchXNum", NXObject.AttributeType.Integer);
            ExpressionUtils.SetAttrExp("PitchY", "PitchY", NXObject.AttributeType.Real);
            ExpressionUtils.SetAttrExp("PitchYNum", "PitchYNum", NXObject.AttributeType.Integer);
            ExpressionUtils.SetAttrExp("PreparationX", "Preparation", NXObject.AttributeType.Integer, 0);
            ExpressionUtils.SetAttrExp("PreparationY", "Preparation", NXObject.AttributeType.Integer, 1);

            //ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("xNCopies"), "PitchXNum");
            //ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("yNCopies"), "PitchYNum");
            //ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("xPitchDistance"), "PitchX");
            //ExpressionUtils.EditExp(ExpressionUtils.GetExpByName("yPitchDistance"), "-PitchY");

            ExpressionUtils.CreateExp("xNCopies=PitchXNum", "Number");
            ExpressionUtils.CreateExp("yNCopies=PitchYNum", "Number");
            ExpressionUtils.CreateExp("xPitchDistance=PitchX", "Number");
            ExpressionUtils.CreateExp("yPitchDistance=-PitchY", "Number");

            if(zDatum)
            {
                ExpressionUtils.CreateExp("moveBoxX=0", "Number");
                ExpressionUtils.CreateExp("moveBoxY=0", "Number");
                ExpressionUtils.CreateExp("moveBoxZ=0", "Number");
            }
         

            ExpressionUtils.CreateExp("moveX=0", "Number");
            ExpressionUtils.CreateExp("moveY=0", "Number");
            ExpressionUtils.CreateExp("moveZ=0", "Number");

        }


    }
}
