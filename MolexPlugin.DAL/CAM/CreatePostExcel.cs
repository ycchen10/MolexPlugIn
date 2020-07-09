using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using NXOpen;
using NXOpen.CAM;
using Basic;
using MolexPlugin.Model;
using NPOI.SS.Util;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建后处理程序单
    /// </summary>
    public class CreatePostExcel
    {
        private List<NCGroup> groups = new List<NCGroup>();
        private Part part;

        public CreatePostExcel(List<NCGroup> group, Part part)
        {
            this.groups = group;
            this.part = part;
        }

        public void CreateExcel()
        {
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string excelTemplatePath = dllPath.Replace("application\\", "Cofigure\\ShopDoc_Template-test.xlsx");
            string path = part.FullPath;
            path = System.IO.Path.GetDirectoryName(path) + "\\" + part.Name + ".xlsx";

            IWorkbook workbook = ExcelUtils.CreateExeclFile(excelTemplatePath);
            if (workbook == null)
            {
                return;
            }
            IFont font = workbook.CreateFont();
            font.FontName = "微软雅黑";
            font.FontHeightInPoints = 8;
            ICellStyle style = ExcelUtils.SetCellStyle(workbook, font);
            ICellStyle styleDate = ExcelUtils.SetCellStyle(workbook, font);
            styleDate.DataFormat = 21;
            ISheet sheet = workbook.GetSheetAt(0);

            if (PostPartInfo.IsPartInfo(part))
            {
                SetMoldInfoToExcel(new MoldInfoModel(part), sheet, style);
            }
            if (PostPartInfo.IsPartElectrode(part))
            {
                SetEleMoldInfoToExcel(new PostElectrodenfo(part), sheet, style);
            }
            string name = AttributeUtils.GetAttrForString(part, "CAMUser");

            if (name.Equals(""))
            {
                name = Environment.UserName;
            }

            SetUser(name, sheet, style);
            SetRowData(sheet, style, styleDate);
            FileStream fs = File.Create(path);
            workbook.Write(fs);
            fs.Close();
            workbook.Close();
        }
        /// <summary>
        /// 设置模具
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sheet"></param>
        /// <param name="style"></param>
        private void SetMoldInfoToExcel(MoldInfoModel model, ISheet sheet, ICellStyle style)
        {
            ExcelUtils.SetValue(sheet, style, 3, 0, model.MoldNumber);
            ExcelUtils.SetValue(sheet, style, 3, 2, model.WorkpieceNumber);
            ExcelUtils.SetValue(sheet, style, 3, 5, model.EditionNumber);

        }
        /// <summary>
        /// 设置电极
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sheet"></param>
        /// <param name="style"></param>
        private void SetEleMoldInfoToExcel(PostElectrodenfo eleInfo, ISheet sheet, ICellStyle style)
        {
            ElectrodeModel model = eleInfo.GetElectrodeModel();
            ExcelUtils.SetValue(sheet, style, 3, 0, model.MoldInfo.MoldNumber);
            ExcelUtils.SetValue(sheet, style, 2, 2, "电极名");
            ExcelUtils.SetValue(sheet, style, 3, 2, model.EleInfo.EleName);
            ExcelUtils.SetValue(sheet, style, 3, 5, "A");

        }
        private void SetUser(string name, ISheet sheet, ICellStyle style)
        {
            ExcelUtils.SetValue(sheet, style, 2, 9, name);
            ExcelUtils.SetValue(sheet, style, 3, 9, DateTime.Now.ToShortDateString());
            ExcelUtils.SetValue(sheet, style, 4, 2, part.FullPath);
        }
        private void SetEleInfo(ISheet sheet, ICellStyle style, PostElectrodenfo eleInfo)
        {

            ElectrodeModel model = eleInfo.GetElectrodeModel();
            ExcelUtils.SetValue(sheet, style, 3, 0, model.MoldInfo.MoldNumber);
            ExcelUtils.SetValue(sheet, style, 3, 2, model.EleInfo.EleName);
            if (model.EleInfo.CrudeInter != 0)
                ExcelUtils.SetValue(sheet, style, 3, 12, model.EleInfo.CrudeInter);
            if (model.EleInfo.DuringInter != 0)
                ExcelUtils.SetValue(sheet, style, 3, 14, model.EleInfo.DuringInter);
            if (model.EleInfo.FineInter != 0)
                ExcelUtils.SetValue(sheet, style, 3, 16, model.EleInfo.FineInter);
            if (eleInfo.GetIsInter())
            {
                ExcelUtils.SetValue(sheet, style, 4, 16, "已扣");
            }
            else
                ExcelUtils.SetValue(sheet, style, 4, 16, "未扣");
        }

        private void SetRowData(ISheet sheet, ICellStyle style, ICellStyle styleDate)
        {
            int row = 6;
            foreach (NCGroup np in groups)
            {

                int oldRow = row;
                ProgramNcGroupModel model = new ProgramNcGroupModel(np);
                List<OperationData> datas = model.GetOperationData();
                List<OperationData> other = model.GetOperationFiltrationOrher(datas);
                if (row > 20)
                {
                    IRow iRow = sheet.GetRow(row);
                    ExcelUtils.InsertRow(sheet, row, other.Count, iRow);
                }
                foreach (OperationData data in other)
                {
                    ExcelUtils.SetValue(sheet, style, row, 2, data.OperName);//程序名
                    if (!datas[0].CutterCompenstation.Equals(""))
                        ExcelUtils.SetValue(sheet, style, row, 5, datas[0].CutterCompenstation); //刀半径补偿号
                    ExcelUtils.SetValue(sheet, style, row, 6, data.Stepover);//部距
                    ExcelUtils.SetValue(sheet, style, row, 7, data.Depth);//下刀量

                    ExcelUtils.SetValue(sheet, style, row, 8, data.Feed);//转速
                    ExcelUtils.SetValue(sheet, style, row, 9, data.Speed);//进给
                    string stock = data.SideStock.ToString("f3") + "/" + data.FloorStock.ToString("f3");
                    ExcelUtils.SetValue(sheet, style, row, 10, stock);//余量
                    row++;
                }
                if (oldRow + 1 < row)
                {
                    sheet.AddMergedRegion(new CellRangeAddress(oldRow, row - 1, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(oldRow, row - 1, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(oldRow, row - 1, 3, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(oldRow, row - 1, 4, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(oldRow, row - 1, 11, 11));
                    sheet.AddMergedRegion(new CellRangeAddress(oldRow, row - 1, 12, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(oldRow, row - 1, 13, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(oldRow, row - 1, 14, 14));
                    sheet.AddMergedRegion(new CellRangeAddress(oldRow, row - 1, 15, 15));
                }

                ExcelUtils.SetValue(sheet, style, oldRow, 0, np.Name);
                ExcelUtils.SetValue(sheet, style, oldRow, 1, datas[0].Tool.ToolName);
                if (other[0].Tool.ToolNumber != 0)
                {
                    ExcelUtils.SetValue(sheet, style, oldRow, 3, "T" + other[0].Tool.ToolNumber.ToString()); //刀号             
                    ExcelUtils.SetValue(sheet, style, oldRow, 4, "H" + other[0].Tool.ToolNumber.ToString()); //刀长号
                }
                double zMin, zMax;
                model.GetOperationZMinAndZMax(other, out zMin, out zMax);
                ExcelUtils.SetValue(sheet, style, oldRow, 11, Math.Ceiling(Math.Abs(zMin))); //伸出长              
                ExcelUtils.SetValue(sheet, style, oldRow, 12, Math.Ceiling(Math.Abs(zMin))); //首下长            
                ExcelUtils.SetValue(sheet, style, oldRow, 13, zMax.ToString("f3")); //最小值            
                ExcelUtils.SetValue(sheet, style, oldRow, 14, zMin.ToString("f3")); //最大值              
                ExcelUtils.SetValue(sheet, styleDate, oldRow, 15, model.GetProgramTime(datas)); //时间                                                                                              

            }

            sheet.ForceFormulaRecalculation = true; //刷新表格
        }


    }
}
