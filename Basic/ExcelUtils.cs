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

namespace Basic
{
    public class ExcelUtils
    {
        public static IWorkbook CreateExeclFile(string templatePath)
        {
            if (!File.Exists(templatePath))
            {
                NXOpen.UI.GetUI().NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "没找到模板");
                return null;
            }
            FileStream fs1 = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
            IWorkbook workBook = null;
            IFormulaEvaluator formulaEvaluator = null;
            if (templatePath.IndexOf(".xlsx") > 0 || templatePath.IndexOf(".xlsm") > 0) // 2007版本
            {
                workBook = new XSSFWorkbook(fs1);
                formulaEvaluator = new XSSFFormulaEvaluator(workBook);
            }
            else if (templatePath.IndexOf(".xls") > 0) // 2003版本
            {
                workBook = new HSSFWorkbook(fs1);
                formulaEvaluator = new HSSFFormulaEvaluator(workBook);
            }
            return workBook;

        }
        /// <summary>
        /// 写入单元格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="style">样式</param>
        /// <param name="rowNum">行数</param>
        /// <param name="cellIndex">列数</param>
        /// <param name="val">输入值</param>
        /// <returns></returns>
        public static bool SetValue(ISheet sheet, ICellStyle style, int rowNum, int cellIndex, object val)
        {
            IRow row = null;
            ICell cell = null;
            row = sheet.GetRow(rowNum);
            if (row == null)
            {
                row = sheet.CreateRow(rowNum);
                cell = row.CreateCell(cellIndex);
            }
            else
            {
                cell = row.GetCell(cellIndex);
                if (cell == null)
                {
                    cell = row.CreateCell(cellIndex);
                }
            }

            if (cell == null)
                return false;

            if (val is int || val is double)
            {
                cell.SetCellType(CellType.Numeric);
                if (val is int)
                {
                    double cal1 = Convert.ToDouble(val);
                    cell.SetCellValue(cal1);
                }
                else
                    cell.SetCellValue((double)val);

            }
            else if (val is string)
            {

                if (val == null || val.ToString() == "")
                    return false;

                cell.SetCellType(CellType.String);
                cell.SetCellValue((string)val);
            }
            else if (val is DateTime)
            {
                if (val == null)
                    return false;
                cell.SetCellType(CellType.Blank);
                cell.SetCellValue((DateTime)val);

            }

            cell.CellStyle = style;

            return true;
        }
        /// <summary>
        /// 写入单元格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="font">字体</param>
        /// <param name="rowNum">行数</param>
        /// <param name="cellIndex">列数</param>
        /// <param name="val">输入值</param>
        /// <returns></returns>
        public static bool SetValue(ISheet sheet, IFont font, int rowNum, int cellIndex, object val)
        {
            IRow row = null;
            ICell cell = null;
            row = sheet.GetRow(rowNum);
            if (row == null)
            {
                row = sheet.CreateRow(rowNum);
                cell = row.CreateCell(cellIndex);
            }
            else
            {
                cell = row.GetCell(cellIndex);
                if (cell == null)
                {
                    cell = row.CreateCell(cellIndex);
                }
            }

            if (cell == null)
                return false;

            if (val is int || val is double)
            {
                cell.SetCellType(CellType.Numeric);
                if (val is int)
                {
                    double cal1 = Convert.ToDouble(val);
                    cell.SetCellValue(cal1);
                }
                else
                    cell.SetCellValue((double)val);

            }
            else if (val is string)
            {

                if (val == null || val.ToString() == "")
                    return false;

                cell.SetCellType(CellType.String);
                cell.SetCellValue((string)val);
            }
            else if (val is DateTime)
            {
                if (val == null)
                    return false;
                cell.SetCellType(CellType.Blank);
                cell.SetCellValue((DateTime)val);
                cell.CellStyle.DataFormat = 21;
            }

            ICellStyle style = cell.CellStyle;
            style.SetFont(font);
            return true;
        }
        /// <summary>
        /// 设置单元格字体
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static ICellStyle SetCellStyle(IWorkbook workbook, IFont font)
        {
            ICellStyle style = workbook.CreateCellStyle();
            //ICellStyle style = workbook.GetCellStyleAt(1);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.WrapText = true;
            style.SetFont(font);
            return style;
        }
        /// <summary>
        /// 查入行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="row">插入行</param>
        /// <param name="rowNumber">插入行数</param>
        /// <param name="irow">插入行格式</param>
        public static void InsertRow(ISheet sheet, int row, int rowNumber, IRow irow)
        {
            sheet.ShiftRows(row, sheet.LastRowNum, rowNumber, true, false);
            for (int i = row; i < row + rowNumber; i++)
            {
                IRow targetRow = null;
                ICell sourceCell = null;
                ICell targetCell = null;
                targetRow = sheet.CreateRow(i);
                for (int m = irow.FirstCellNum; m < irow.LastCellNum; m++)
                {
                    sourceCell = irow.GetCell(m);
                    if (sourceCell == null)
                    {
                        continue;
                    }
                    targetCell = targetRow.CreateCell(m);
                    targetCell.CellStyle = sourceCell.CellStyle;
                    targetCell.SetCellType(sourceCell.CellType);
                }

            }

        }

        public static void SetSetCellFormula(ISheet sheet, int rowNum, int cellIndex, string val)
        {
            IRow row = null;
            ICell cell = null;
            row = sheet.GetRow(rowNum);
            if (row == null)
            {
                row = sheet.CreateRow(rowNum);
                cell = row.CreateCell(cellIndex);
            }
            else
            {
                cell = row.GetCell(cellIndex);
                if (cell == null)
                {
                    cell = row.CreateCell(cellIndex);
                }
            }
            cell.SetCellFormula(val);
            sheet.ForceFormulaRecalculation = true;
        }

    }
}
