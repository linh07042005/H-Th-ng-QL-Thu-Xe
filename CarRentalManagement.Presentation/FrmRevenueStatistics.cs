
using System;
using System.Drawing;
using System.Windows.Forms;
using CarRentalManagement.BLL.Services;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.Presentation
{
    public class FrmRevenueStatistics : Form
    {
        private readonly User currentUser;
        private readonly RevenueService revenueService;

        private Color backgroundColor = Color.FromArgb(17, 18, 20);
        private Color panelColor = Color.FromArgb(28, 30, 33);
        private Color goldColor = Color.FromArgb(198, 166, 107);
        private Color textColor = Color.FromArgb(243, 239, 230);
        private Color mutedColor = Color.FromArgb(145, 145, 145);

        private Label lblTotalRevenue;
        private Label lblTotalInvoices;
        private Label lblPaidInvoices;
        private Label lblUnpaidInvoices;

        private ComboBox cboYear;
        private Panel chartPanel;

        public FrmRevenueStatistics(User user)
        {
            currentUser = user;

            // KIỂM TRA QUYỀN ADMIN
            if (currentUser == null ||
                currentUser.Role != "Admin")
            {
                MessageBox.Show(
                    "Bạn không có quyền truy cập chức năng thống kê doanh thu!\n" +
                    "Chỉ Admin mới được sử dụng chức năng này.",
                    "Từ chối truy cập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                this.Close();
                return;
            }

            revenueService = new RevenueService();

            InitializeForm();
            LoadRevenueData();
        }

        private void InitializeForm()
        {
            Text = "LINHXURY - Thống kê doanh thu";
            Size = new Size(1100, 700);
            MinimumSize = new Size(950, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = backgroundColor;
            ForeColor = textColor;
            Font = new Font("Segoe UI", 10);

            Label title = new Label
            {
                Text = "THỐNG KÊ DOANH THU",
                Font = new Font("Georgia", 24, FontStyle.Bold),
                ForeColor = goldColor,
                Location = new Point(30, 25),
                AutoSize = true
            };
            Controls.Add(title);

            Label subtitle = new Label
            {
                Text = "Theo dõi doanh thu và tình hình thanh toán của LINHXURY",
                ForeColor = mutedColor,
                Location = new Point(33, 75),
                AutoSize = true
            };
            Controls.Add(subtitle);

            // Bộ lọc năm
            Label lblYear = new Label
            {
                Text = "Chọn năm:",
                Location = new Point(790, 80),
                ForeColor = textColor,
                AutoSize = true
            };
            Controls.Add(lblYear);

            cboYear = new ComboBox
            {
                Location = new Point(875, 76),
                Size = new Size(140, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 42, 45),
                ForeColor = textColor,
                Font = new Font("Segoe UI", 10)
            };

            for (int year = DateTime.Now.Year; year >= 2020; year--)
            {
                cboYear.Items.Add(year);
            }

            cboYear.SelectedIndex = 0;

            // Chỉ đăng ký sự kiện sau khi đã chọn năm mặc định
            cboYear.SelectedIndexChanged += (s, e) =>
            {
                LoadRevenueData();
            };

            Controls.Add(cboYear);

            // Các thẻ thống kê
            lblTotalRevenue = AddCard(
                "TỔNG DOANH THU", "0 VNĐ",
                "Doanh thu đã thanh toán", 30, 130);

            lblTotalInvoices = AddCard(
                "SỐ HÓA ĐƠN", "0",
                "Tổng số hóa đơn", 285, 130);

            lblPaidInvoices = AddCard(
                "ĐÃ THANH TOÁN", "0",
                "Hóa đơn đã thanh toán", 540, 130);

            lblUnpaidInvoices = AddCard(
                "CHƯA THANH TOÁN", "0",
                "Hóa đơn chưa thanh toán", 795, 130);

            chartPanel = new Panel
            {
                Location = new Point(30, 320),
                Size = new Size(1020, 280),
                BackColor = panelColor
            };

            Controls.Add(chartPanel);

            Label account = new Label
            {
                Text = "Tài khoản: " + currentUser.FullName
                    + " | " + currentUser.Role,
                ForeColor = mutedColor,
                Location = new Point(32, 620),
                AutoSize = true
            };
            Controls.Add(account);
        }

        private Label AddCard(
            string title,
            string value,
            string description,
            int x,
            int y)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(230, 145),
                BackColor = panelColor
            };

            Panel accent = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(4, 145),
                BackColor = goldColor
            };

            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(18, 18),
                ForeColor = mutedColor,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                AutoSize = true
            };

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(18, 48),
                ForeColor = goldColor,
                Font = new Font("Georgia", 19, FontStyle.Bold),
                AutoSize = true
            };

            Label lblDescription = new Label
            {
                Text = description,
                Location = new Point(18, 105),
                ForeColor = mutedColor,
                Font = new Font("Segoe UI", 8),
                AutoSize = true
            };

            card.Controls.Add(accent);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblDescription);

            Controls.Add(card);

            return lblValue;
        }

        private void LoadRevenueData()
        {
            try
            {
                int selectedYear =
                    Convert.ToInt32(cboYear.SelectedItem);

                RevenueSummary summary =
                    revenueService.GetRevenueSummary(selectedYear);

                lblTotalRevenue.Text =
                    summary.TotalRevenue.ToString("N0") + " VNĐ";

                lblTotalInvoices.Text =
                    summary.TotalInvoices.ToString("N0");

                lblPaidInvoices.Text =
                    summary.PaidInvoices.ToString("N0");

                lblUnpaidInvoices.Text =
                    summary.UnpaidInvoices.ToString("N0");

                DrawMonthlyRevenue(summary, selectedYear);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi thống kê doanh thu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DrawMonthlyRevenue(
            RevenueSummary summary,
            int selectedYear)
        {
            chartPanel.Controls.Clear();

            Label chartTitle = new Label
            {
                Text = "DOANH THU THEO 12 THÁNG - NĂM " + selectedYear,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = goldColor,
                Location = new Point(22, 15),
                AutoSize = true
            };

            chartPanel.Controls.Add(chartTitle);

            decimal maxRevenue = 0;

            foreach (MonthlyRevenue item in summary.MonthlyRevenues)
            {
                if (item.Revenue > maxRevenue)
                    maxRevenue = item.Revenue;
            }

            int barWidth = 45;
            int gap = 30;
            int startX = 35;
            int baseY = 225;
            int maxBarHeight = 140;

            for (int month = 1; month <= 12; month++)
            {
                decimal revenue = 0;

                foreach (MonthlyRevenue item in summary.MonthlyRevenues)
                {
                    if (item.Month == "Tháng " + month)
                    {
                        revenue = item.Revenue;
                        break;
                    }
                }

                int barHeight = 0;

                if (maxRevenue > 0)
                {
                    barHeight = (int)(
                        revenue / maxRevenue * maxBarHeight);
                }

                int x = startX + (month - 1) * (barWidth + gap);

                Panel bar = new Panel
                {
                    Location = new Point(
                        x,
                        baseY - Math.Max(barHeight, 2)),
                    Size = new Size(
                        barWidth,
                        Math.Max(barHeight, 2)),
                    BackColor = goldColor
                };

                Label monthLabel = new Label
                {
                    Text = "T" + month,
                    Location = new Point(x - 2, baseY + 5),
                    Size = new Size(barWidth + 5, 22),
                    ForeColor = textColor,
                    Font = new Font("Segoe UI", 8),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                chartPanel.Controls.Add(bar);
                chartPanel.Controls.Add(monthLabel);
            }

            if (maxRevenue == 0)
            {
                Label empty = new Label
                {
                    Text = "Chưa có doanh thu trong năm " + selectedYear + ".",
                    ForeColor = mutedColor,
                    Location = new Point(22, 65),
                    AutoSize = true 
                };

                chartPanel.Controls.Add(empty);
            }
        }
    }
}
