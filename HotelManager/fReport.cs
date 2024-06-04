using HotelManager.DAO;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HotelManager
{
    public partial class fReport : Form
    {
        private int month = 1;
        private int year = 1990;
        public fReport()
        {
            InitializeComponent();
            dataGridReport.Font = new System.Drawing.Font("Segoe UI", 9.75F);
        }

        #region Load
        private void LoadFullReport(int month, int year)
        {
            this.month = month;
            this.year = year;
            DataTable table = GetFulReport(month, year);

            // Đổi tên cột RoomType thành Tên loại phòng
            table.Columns["RoomType"].ColumnName = "Tên loại phòng";

            // Kiểm tra dữ liệu
            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine($"Tên loại phòng: {row["Tên loại phòng"]}, Value: {row["value"]}, Rate: {row["rate"]}");
            }

            ChangePrice(table);

            // Đặt cột "Tên loại phòng" lên trước cột "tỉ lệ" (rate)
            table.Columns["Tên loại phòng"].SetOrdinal(0);

            BindingSource source = new BindingSource();
            source.DataSource = table;
            dataGridReport.DataSource = source;
            bindingReport.BindingSource = source;

            foreach (var item in source)
            {
                DataRowView rowView = item as DataRowView;
                if (rowView != null)
                {
                    Console.WriteLine($"Tên loại phòng: {rowView["Tên loại phòng"]}, Value: {rowView["value"]}, Rate: {rowView["rate_New"]}");
                }
            }

            DrawChart(source);
            GC.Collect();
        }

        #endregion

        #region Click
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadFullReport(int.Parse(comboBoxMonth.Text), (int)(numericYear.Value));
        }
        private void ToolStripLabel1_Click(object sender, EventArgs e)
        {
            saveReport.FileName = "Doanh thu tháng " + month + '-' + year;
            if (saveReport.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                bool check;
                try
                {
                    switch (saveReport.FilterIndex)
                    {
                        case 2:
                            check = ExportToExcel.Instance.Export(dataGridReport, saveReport.FileName, ModeExportToExcel.XLSX);
                            break;
                        case 3:
                            check = ExportToExcel.Instance.Export(dataGridReport, saveReport.FileName, ModeExportToExcel.PDF);
                            break;
                        default:
                            check = ExportToExcel.Instance.Export(dataGridReport, saveReport.FileName, ModeExportToExcel.XLS);
                            break;
                    }
                    if (check)
                        MessageBox.Show("Xuất thành công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Lỗi xuất thất bại", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    MessageBox.Show("Lỗi (Cần cài đặt Office)", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Data
        private DataTable GetFulReport(int month, int year)
        {
            return ReportDAO.Instance.LoadFullReport(month, year);
        }

        #endregion

        #region Chart
        private void DrawChart(BindingSource source)
        {
            chartReport.Series.Clear();
            Series series = new Series("Rate")
            {
                XValueMember = "Tên loại phòng",
                YValueMembers = "rate_New",
                ChartType = SeriesChartType.Pie
            };
            chartReport.Series.Add(series);

            chartReport.DataSource = source;
            chartReport.DataBind();

            if (chartReport.ChartAreas.Count == 0)
            {
                chartReport.ChartAreas.Add(new ChartArea());
            }

            chartReport.ChartAreas[0].AxisX.Title = "Tên loại phòng";
            chartReport.ChartAreas[0].AxisY.Title = "Tỉ lệ";

            foreach (DataPoint item in chartReport.Series[0].Points)
            {
                if (item.YValues[0] == 0)
                {
                    item.Label = " ";
                }
                else
                {
                    DataRowView row = source[(int)item.XValue] as DataRowView;
                    if (row != null)
                    {
                        string rateNew = row["rate_New"].ToString();
                        item.Label = rateNew;
                    }
                    else
                    {
                        item.Label = item.YValues[0].ToString("#0.##%");
                    }
                }
            }
        }

        #endregion

        #region Change Price
        private void ChangePrice(DataTable table)
        {
            table.Columns.Add("value_New", typeof(string));
            table.Columns.Add("rate_New", typeof(string));
            int sum = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                object value = table.Rows[i]["value"];
                int node = Convert.IsDBNull(value) ? 0 : Convert.ToInt32(value);
                table.Rows[i]["value_New"] = node.ToString("C0", CultureInfo.CreateSpecificCulture("vi-VN"));

                object rateValue = table.Rows[i]["rate"];
                double rate = 0.0;
                if (rateValue != null && double.TryParse(rateValue.ToString(), out rate))
                {
                    table.Rows[i]["rate_New"] = (rate / 100).ToString("#0.##%");
                }
                else
                {
                    table.Rows[i]["rate_New"] = "N/A";
                }

                sum += node;
            }
            table.Columns.Remove("value");
            DataRow row = table.NewRow();
            table.Columns["value_New"].ColumnName = "value";
            row["value"] = sum.ToString("C0", CultureInfo.CreateSpecificCulture("vi-VN"));
            table.Rows.Add(row);
        }

        #endregion

        #region Form
        private void FReport_Load(object sender, EventArgs e)
        {
            LoadFullReport(DateTime.Now.Month, DateTime.Now.Year);
            comboBoxMonth.Text = DateTime.Now.Month.ToString();
            numericYear.Value = DateTime.Now.Year;
        }
        #endregion

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chartReport_Click(object sender, EventArgs e)
        {
        }
    }
}
