using DeBangTool.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeBangTool
{
    public partial class 核销小工具 : Form
    {
        public 核销小工具()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = dialog.FileName;
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "请选择文件路径";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string foldPath = dialog.SelectedPath;
                    DirectoryInfo theFolder = new DirectoryInfo(foldPath);
                    textBox2.Text = theFolder.FullName;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请选择源文件地址");
                return;
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("请选择输出路径");
                return;
            }

            try
            {
                button3.Text = "计算中,请稍等....";
                var sucessData = new List<ColipuSouceModel>();
                Task.Factory.StartNew(() =>
                {
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    var errorList = new List<string>();
                    var errorDic = new Dictionary<string, List<string>>();
                    ExcelHelper excelHelper = new ExcelHelper(textBox1.Text);
                    var colipuCoolName = new string[6] { "DOrder", "ProductCode", "Price", "Quantity", "Time", "IsHeXiao" };
                    var colipuData = excelHelper.ExcelToList<ColipuSouceModel>(1, colipuCoolName).ToList();
                    var depponCoolName = new string[3] { "PorductCode", "Price", "Quantity" };
                    var depponData = excelHelper.ExcelToList<DepponSouceModel>(2, depponCoolName).ToList();
                    var colipuDateDic = colipuData.GroupBy(x => x.ProductCode).ToList();

                    for (int i = 0; i < depponData.Count; i++)
                    {
                        var SoucessData = colipuDateDic.Where(x => x.Key == depponData[i].PorductCode).ToList();

                        if (SoucessData.Count > 1)
                        {
                            MessageBox.Show("要查询的数据中有重复SKU");
                            return;
                        }
                        else if (SoucessData.Count == 0)
                        {
                            sucessData.Add(new ColipuSouceModel
                            {
                                ProductCode = depponData[i].PorductCode,
                                IsHeXiao = "科力普原始数据中不包含此商品编号",
                                Price = depponData[i].Price,
                                Quantity = depponData[i].Quantity,
                            });
                            continue;
                        }
                        else
                        {
                            Kernel kernel = new Kernel(SoucessData, depponData[i]);
                            var suData = kernel.Start().ToList();
                            sucessData.AddRange(suData);
                        }

                    }
                    stopwatch.Stop();
                    TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间
                    double seconds = timeSpan.TotalSeconds;  //  秒数
                                                             // MessageBox.Show(seconds.ToString());
                    Export(sucessData);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="colipuSouceModels"></param>
        private void Export(List<ColipuSouceModel> colipuSouceModels)
        {
            //输出Excel
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet tb = workbook.CreateSheet("核销结果");

            #region 表头              
            IRow row = tb.CreateRow(0);
            var cellValList = new List<string>()
                {
                    "出库单号",
                    "商品编号",
                    "金额",
                    "数量",
                    "签收日期",
                    "是否已核销",
                };
            for (int i = 0; i < cellValList.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(cellValList[i]);
            }
            #endregion

            #region 组装数据 
            var table = new DataTable("匹配结果");
            DataColumn column;
            column = new DataColumn
            {
                ColumnName = "Dorder",
                Caption = "出库单号"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                ColumnName = "ProductCode",
                Caption = "商品编码"
            };
            table.Columns.Add(column);

            column = new DataColumn
            {
                ColumnName = "Price",
                Caption = "价格"
            };
            table.Columns.Add(column);

            column = new DataColumn
            {
                ColumnName = "Quantity",
                Caption = "价格"
            };
            table.Columns.Add(column);

            column = new DataColumn
            {
                ColumnName = "Time",
                Caption = "签收时间"
            };
            table.Columns.Add(column);

            column = new DataColumn
            {
                ColumnName = "IsHeXiao",
                Caption = "是否核销"
            };
            table.Columns.Add(column);
            #endregion
            //colipuSouceModels
            foreach (var item in colipuSouceModels.OrderBy(x => x.ProductCode))
            {
                if (item != null)
                {
                    var tableRow = table.NewRow();
                    tableRow["Dorder"] = item.DOrder;
                    tableRow["ProductCode"] = item.ProductCode;
                    tableRow["Price"] = item.Price;
                    tableRow["Quantity"] = item.Quantity;
                    tableRow["Time"] = item.Time;
                    tableRow["IsHeXiao"] = item.IsHeXiao;
                    table.Rows.Add(tableRow);
                }

            }

            #region 数据  

            for (int i = 0; i < table.Rows.Count; i++)
            {
                IRow row1 = tb.CreateRow(i + 1);
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(table.Rows[i][j].ToString());
                }
            }


            using (FileStream fs = File.OpenWrite($"{textBox2.Text}/核销结果.xls")) //打开一个xls文件，如果没有则自行创建，如果存在myxls.xls文件则在创建是不要打开该文件！
            {
                workbook.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
                if (MessageBox.Show("操作成功！", "", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    Environment.Exit(0);
                    Close();
                }

            }

            #endregion
        }


    }
}
