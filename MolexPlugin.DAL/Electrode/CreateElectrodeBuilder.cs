using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;
using System.Text.RegularExpressions;

namespace MolexPlugin.DAL
{
    public class CreateElectrodeBuilder
    {
        private ElectrodeInfo eleInfo;
        private CreateConditionModel model;
        private ElectrodePreveiw preveiw;
        private ElectrodeHeadInfo head;
        private AbstractElectrodeSetValue setValue;

        public CreateElectrodeBuilder(CreateConditionModel model)
        {
            this.model = model;
            head = new ElectrodeHeadInfo(model);
            preveiw = new ElectrodePreveiw(head);
            setValue = ElectrodeSetValueFactory.Create(head, model.VecName);

        }
        /// <summary>
        /// 预览
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xNumber"></param>
        /// <param name="y"></param>
        /// <param name="yNumber"></param>
        public void CreatePreveiw(double x, int xNumber, double y, int yNumber)
        {
            preveiw.UpdatePattern(x, xNumber, y, yNumber);
        }
        /// <summary>
        /// 删除预览
        /// </summary>
        public void DelePreveiw()
        {
            preveiw.DelePattern();
        }
        /// <summary>
        /// 获取设定值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xNumber"></param>
        /// <param name="y"></param>
        /// <param name="yNumber"></param>
        /// <param name="zDatum"></param>
        /// <returns></returns>
        public Point3d GetSetValue(double x, int xNumber, double y, int yNumber, bool zDatum)
        {
            return setValue.GetHeadSetValue(x, xNumber, y, yNumber, zDatum);
        }
        /// <summary>
        /// 回去备料
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xNumber"></param>
        /// <param name="y"></param>
        /// <param name="yNumber"></param>
        /// <param name="zDatum"></param>
        /// <returns></returns>
        public double[] GetPreparation(double x, int xNumber, double y, int yNumber, bool zDatum)
        {
            return setValue.GetPreparation(x, xNumber, y, yNumber, zDatum);
        }
        /// <summary>
        /// 获取电极名
        /// </summary>
        /// <returns></returns>
        public string GetEleName()
        {
            string name;
            List<ElectrodeModel> eles = this.model.Assemble.Electrodes;
            eles.Sort();
            MoldInfoModel moldInfo = this.model.Assemble.Asm.MoldInfo;
            if (eles == null || eles.Count == 0)
            {
                name = moldInfo.MoldNumber + "-" + moldInfo.WorkpieceNumber + "E1";
            }
            else
            {
                name = moldInfo.MoldNumber + "-" + moldInfo.WorkpieceNumber + "E" + (eles[eles.Count - 1].EleInfo.EleNumber + 1).ToString();
            }

            return name;
        }
        /// <summary>
        /// 获取电极号
        /// </summary>
        /// <param name="eleName"></param>
        /// <returns></returns>
        public int GetEleNumber(string eleName)
        {
            string name = eleName.Substring(eleName.LastIndexOf("E"));

            MatchCollection match = Regex.Matches(name, @"\d+");
            int result;
            if (match.Count != 0 && int.TryParse(match[0].Value, out result))
            {
                return result;
            }
            return 0;
        }

        public void CreateEle(ElectrodeInfo eleInfo)
        {
            this.eleInfo = eleInfo;
            Matrix4 workMat = this.model.Work.WorkMatr;
            double zHeight = setValue.GetZHeight(eleInfo.Extrudewith);
            Matrix4 mat = setValue.GetEleMatr(eleInfo.Preparation, GetCenter());
            ElectrodePart elePart = new ElectrodePart(eleInfo, this.model.Work.MoldInfo, this.head, mat);
           
            elePart.CreateElectrode(eleInfo.ZDatum, zHeight, Expression);
         
        }
        private void Expression()
        {
            setValue.CreateExp(eleInfo.PitchX, eleInfo.PitchXNum, eleInfo.PitchY, eleInfo.PitchYNum, eleInfo.ZDatum);
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
        /// <summary>
        /// 齿移动的增量
        /// </summary>
        /// <returns></returns>
        private Vector3d GetMove()
        {
            Vector3d temp = new Vector3d();
            temp.X = (eleInfo.PitchXNum - 1) * eleInfo.PitchX / 2;
            temp.Y = (eleInfo.PitchYNum - 1) * eleInfo.PitchY / 2;
            temp.Z = eleInfo.EleSetValue[2];
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
    }
}
