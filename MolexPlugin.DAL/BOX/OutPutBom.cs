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
using Basic;
using MolexPlugin.Model;


namespace MolexPlugin.DAL
{
    public class OutPutBom
    {

        public static void CreateBomExecl(AssembleModel assemble)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            MoldInfoModel mold = assemble.Asm.MoldInfo;
            string workpieceName = mold.MoldNumber + "-" + mold.WorkpieceNumber + mold.EditionNumber;
            string bomPath = assemble.Asm.WorkpieceDirectoryPath + workpieceName + "-Bom.xlsx";
            if (File.Exists(bomPath))
            {
                File.Delete(bomPath);
            }
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string excelTemplatePath = dllPath.Replace("application\\", "Cofigure\\ElectrodeBom_Template.xlsx");
            //string excelTemplatePath = @"C:\Users\ycchen10\OneDrive - kochind.com\Desktop\MolexPlugIn\Cofigure\ElectrodeBom_Template.xlsx";
            IWorkbook workbook = ExcelUtils.CreateExeclFile(excelTemplatePath);
            if (workbook == null)
            {
                return;
            }
            IFont font = workbook.CreateFont();
            font.FontName = "微软雅黑";
            font.FontHeightInPoints = 10;
            ICellStyle style = ExcelUtils.SetCellStyle(workbook, font);
            ISheet sheet = workbook.GetSheetAt(0);

            ExcelUtils.SetValue(sheet, style, 1, 1, mold.MoldNumber);
            ExcelUtils.SetValue(sheet, style, 2, 1, mold.WorkpieceNumber);
            ExcelUtils.SetValue(sheet, style, 3, 1, mold.EditionNumber);
            ExcelUtils.SetValue(sheet, style, 4, 1, mold.MoldType);
            ExcelUtils.SetValue(sheet, style, 5, 1, mold.CreatedDate);
            ExcelUtils.SetValue(sheet, style, 6, 1, mold.CreatorName);
            string workName = "";
            int row = 9;
            foreach (WorkModel wk in assemble.Works)
            {
                ExcelUtils.SetValue(sheet, style, row, 0, "WORK" + wk.WorkNumber);
                row++;
                List<ElectrodeModel> eles = assemble.Electrodes.Where(a => a.WorkNumber == wk.WorkNumber).ToList();
                eles.Sort();
                foreach (ElectrodeModel model in eles)
                {
                    SetRowData(sheet, style, row, model.EleInfo);
                    row++;
                }
            }

            FileStream fs = File.Create(bomPath);
            workbook.Write(fs);
            fs.Close();
            workbook.Close();
            NXOpen.UI.GetUI().NXMessageBox.Show("信息", NXMessageBox.DialogType.Information, "导出成功");
        }


        public static void SetRowData(ISheet sheet, ICellStyle style, int row, ElectrodeInfo info)
        {
            ExcelUtils.SetValue(sheet, style, row, 0, info.EleName);
            ExcelUtils.SetValue(sheet, style, row, 1, info.EleSetValue[0]);
            ExcelUtils.SetValue(sheet, style, row, 2, info.EleSetValue[1]);
            ExcelUtils.SetValue(sheet, style, row, 3, info.EleSetValue[2]);
            ExcelUtils.SetValue(sheet, style, row, 4, info.PitchX);
            ExcelUtils.SetValue(sheet, style, row, 5, info.PitchXNum);
            ExcelUtils.SetValue(sheet, style, row, 6, info.PitchY);
            ExcelUtils.SetValue(sheet, style, row, 7, info.PitchYNum);
            ExcelUtils.SetValue(sheet, style, row, 8, info.CrudeInter);
            ExcelUtils.SetValue(sheet, style, row, 9, info.CrudeNum);
            ExcelUtils.SetValue(sheet, style, row, 10, info.DuringInter);
            ExcelUtils.SetValue(sheet, style, row, 11, info.DuringNum);
            ExcelUtils.SetValue(sheet, style, row, 12, info.FineInter);
            ExcelUtils.SetValue(sheet, style, row, 13, info.FineNum);
            ExcelUtils.SetValue(sheet, style, row, 14, info.EleType);
            ExcelUtils.SetValue(sheet, style, row, 15, info.Ch);
            ExcelUtils.SetValue(sheet, style, row, 16, info.Material);
            ExcelUtils.SetValue(sheet, style, row, 17, info.Condition);
            ExcelUtils.SetValue(sheet, style, row, 18, info.Positioning);
            string size = info.Preparation[0] + "*" + info.Preparation[1] + "*" + info.Preparation[2];
            ExcelUtils.SetValue(sheet, style, row, 19, size);
            ExcelUtils.SetValue(sheet, style, row, 20, info.BorrowName);
            ExcelUtils.SetValue(sheet, style, row, 21, info.Remarks);
            ExcelUtils.SetValue(sheet, style, row, 22, info.Technology);
        }
    }
}
