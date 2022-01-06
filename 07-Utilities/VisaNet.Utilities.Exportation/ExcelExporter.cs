using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using VisaNet.Utilities.Exportation.ExtensionMethods;

namespace VisaNet.Utilities.Exportation
{
    public static class ExcelExporter
    {
        /// <summary>
        /// Retorna un byte[] con un xlsx y los datos en una hoja
        /// </summary>
        /// <param name="Sheet name"></param>
        /// <param name="Data"></param>
        /// <param name="Headers"></param>
        /// <returns>Si los cabezales no son nulos, estos son el nombre de las propiedades de los datos</returns>
        public static byte[] ExcelExport(string sheetName, IEnumerable<object> data, string[] headers)
        {
            var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add(sheetName);
            int colCount;

            #region Headers
            //Si tengo headers particulares, los uso
            if (headers != null)
            {
                for (var i = 1; i <= headers.Count(); i++)
                {
                    ws.Cells[1, i].Value = headers[i - 1];
                    ws.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                    ws.Cells[1, i].Style.Font.Color.SetColor(Color.White);
                    ws.Cells[1, i].Style.Font.Bold = true;
                }
                colCount = headers.Count();
            }
            //Sino, los headers son los nombres de las propiesdes de la data
            else
            {
                var i = 1;
                foreach (var propertyInfo in data.ElementAt(0).GetType().GetProperties())
                {
                    ws.Cells[1, i].Value = propertyInfo.Name;
                    ws.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                    ws.Cells[1, i].Style.Font.Color.SetColor(Color.White);
                    ws.Cells[1, i].Style.Font.Bold = true;
                    ws.Column(i).Style.ShrinkToFit = true;
                    i++;
                }
                colCount = i;
            }
            #endregion

            #region Data
            var index = 2;
            foreach (var obj in data)
            {
                var propCount = 1;

                if (!(obj is ExpandoObject))
                {
                    foreach (var propertyInfo in obj.GetType().GetProperties())
                    {
                        var value = propertyInfo.GetValue(obj);

                        if (value is bool)
                        {
                            ws.Cells[index, propCount].Value = ((bool)value).CustomToString();
                        }
                        else if (value is DateTime)
                        {
                            ws.Cells[index, propCount].Value = ((DateTime)value).ToShortDateString();
                        }
                        else if (value is double || value is decimal)
                        {
                            ws.Cells[index, propCount].Value = Convert.ToDouble(value).SignificantDigits(2);
                        }
                        else if (value is int || value is short)
                        {
                            ws.Cells[index, propCount].Value = Convert.ToDouble(value).SignificantDigits(2);
                        }
                        else
                        {
                            ws.Cells[index, propCount].Value = (value != null ? value.ToString() : string.Empty);
                        }

                        ws.Cells[index, propCount].Style.WrapText = true;
                        ws.Cells[index, propCount].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        propCount++;
                    }
                }
                else
                {
                    foreach (var valor in ((IDictionary<string, object>)obj))
                    {
                        if (valor.Value is bool)
                        {
                            ws.Cells[index, propCount].Value = ((bool)valor.Value).CustomToString();
                        }
                        else if (valor.Value is DateTime)
                        {
                            ws.Cells[index, propCount].Value = ((DateTime)valor.Value).ToShortDateString();
                        }
                        else if (valor.Value is double || valor.Value is decimal)
                        {
                            ws.Cells[index, propCount].Value = Convert.ToDouble(valor.Value).SignificantDigits(2);
                        }
                        else if (valor.Value is int || valor.Value is short)
                        {
                            ws.Cells[index, propCount].Value = Convert.ToDouble(valor.Value).SignificantDigits(2);
                        }
                        else
                        {
                            ws.Cells[index, propCount].Value = (valor.Value != null ? valor.Value.ToString() : string.Empty);
                        }

                        ws.Cells[index, propCount].Style.WrapText = true;
                        ws.Cells[index, propCount].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        propCount++;
                    }
                }

                index++;
            }
            #endregion

            for (int i = 1; i <= colCount; i++)
            {
                ws.Column(i).AutoFit(30);
            }
            return pck.GetAsByteArray();
        }

        public static byte[] ExcelExportTransactions(string sheetName, IEnumerable<object> data, string[] headers, string[] filtersHeaders, string[] filtersData)
        {
            var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add(sheetName);
            int colCount;

            #region Filters
            for (var i = 1; i <= filtersHeaders.Count(); i++)
            {
                ws.Cells[1, i].Value = filtersHeaders[i - 1];
                ws.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                ws.Cells[1, i].Style.Font.Color.SetColor(Color.White);
                ws.Cells[1, i].Style.Font.Bold = true;

                ws.Cells[2, i].Value = filtersData[i - 1];
                ws.Cells[2, i].Style.WrapText = true;
                ws.Cells[2, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
            #endregion

            #region Headers
            //Si tengo headers particulares, los uso
            if (headers != null)
            {
                for (var i = 1; i <= headers.Count(); i++)
                {
                    ws.Cells[5, i].Value = headers[i - 1];
                    ws.Cells[5, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[5, i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                    ws.Cells[5, i].Style.Font.Color.SetColor(Color.White);
                    ws.Cells[5, i].Style.Font.Bold = true;
                }
                colCount = headers.Count();
            }
            //Sino, los headers son los nombres de las propiesdes de la data
            else
            {
                var i = 1;
                foreach (var propertyInfo in data.ElementAt(0).GetType().GetProperties())
                {
                    ws.Cells[5, i].Value = propertyInfo.Name;
                    ws.Cells[5, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[5, i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                    ws.Cells[5, i].Style.Font.Color.SetColor(Color.White);
                    ws.Cells[5, i].Style.Font.Bold = true;
                    ws.Column(i).Style.ShrinkToFit = true;
                    i++;
                }
                colCount = i;
            }
            #endregion

            #region Data
            var index = 6;
            foreach (var obj in data)
            {
                var propCount = 1;

                if (!(obj is ExpandoObject))
                {
                    foreach (var propertyInfo in obj.GetType().GetProperties())
                    {
                        var value = propertyInfo.GetValue(obj);

                        if (value is bool)
                        {
                            ws.Cells[index, propCount].Value = ((bool)value).CustomToString();
                        }
                        else if (value is DateTime)
                        {
                            ws.Cells[index, propCount].Value = ((DateTime)value).ToShortDateString();
                        }
                        else if (value is double || value is decimal)
                        {
                            ws.Cells[index, propCount].Value = Convert.ToDouble(value).SignificantDigits(2);
                        }
                        else if (value is int || value is short)
                        {
                            ws.Cells[index, propCount].Value = Convert.ToDouble(value).SignificantDigits(2);
                        }
                        else
                        {
                            ws.Cells[index, propCount].Value = (value != null ? value.ToString() : string.Empty);
                        }

                        ws.Cells[index, propCount].Style.WrapText = true;
                        ws.Cells[index, propCount].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        propCount++;
                    }
                }
                else
                {
                    foreach (var valor in ((IDictionary<string, object>)obj))
                    {
                        if (valor.Value is bool)
                        {
                            ws.Cells[index, propCount].Value = ((bool)valor.Value).CustomToString();
                        }
                        else if (valor.Value is DateTime)
                        {
                            ws.Cells[index, propCount].Value = ((DateTime)valor.Value).ToShortDateString();
                        }
                        else if (valor.Value is double || valor.Value is decimal)
                        {
                            ws.Cells[index, propCount].Value = Convert.ToDouble(valor.Value).SignificantDigits(2);
                        }
                        else if (valor.Value is int || valor.Value is short)
                        {
                            ws.Cells[index, propCount].Value = Convert.ToDouble(valor.Value).SignificantDigits(2);
                        }
                        else
                        {
                            ws.Cells[index, propCount].Value = (valor.Value != null ? valor.Value.ToString() : string.Empty);
                        }

                        ws.Cells[index, propCount].Style.WrapText = true;
                        ws.Cells[index, propCount].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        propCount++;
                    }
                }

                index++;
            }
            #endregion

            for (int i = 1; i <= colCount; i++)
            {
                ws.Column(i).AutoFit(30);
            }
            return pck.GetAsByteArray();
        }
    }
}
