
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CarRentalManagement.Models.Entities;
using CarRentalManagement.BLL.Services;

namespace CarRentalManagement.Presentation
{
    public class FrmMain : Form
    {
        private User currentUser;
        private readonly DashboardService dashboardService;

        private Label lblTotalCars;
        private Label lblRentedCars;
        private Label lblTotalCustomers;
        private Label lblActiveContracts;
        private Label lblTotalRevenue;

        private Color backgroundColor = Color.FromArgb(17, 18, 20);
        private Color sidebarColor = Color.FromArgb(24, 25, 28);
        private Color cardColor = Color.FromArgb(31, 33, 37);
        private Color goldColor = Color.FromArgb(198, 166, 107);
        private Color mutedColor = Color.FromArgb(145, 145, 145);

        public FrmMain(User user)
        {
            currentUser = user;
            dashboardService = new DashboardService();

            InitializeMainForm();
        }

        private string GetRoleName()
        {
            if (currentUser.Role == "Admin")
                return "Quản trị viên";

            if (currentUser.Role == "Staff")
                return "Nhân viên";

            return currentUser.Role;
        }

        private void InitializeMainForm()
        {
            this.Text = "LINHXURY | Trang tổng quan";
            this.Size = new Size(1360, 820);
            this.MinimumSize = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = backgroundColor;
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.Sizable;

            CreateSidebar();
            CreateMainContent();
            LoadDashboardData();
        }

        private void CreateSidebar()
        {
            Panel sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 245;
            sidebar.BackColor = sidebarColor;

            Label brand = new Label();
            brand.Text = "LINHXURY";
            brand.Font = new Font("Georgia", 23, FontStyle.Bold);
            brand.ForeColor = goldColor;
            brand.Location = new Point(25, 30);
            brand.AutoSize = true;

            Label brandSub = new Label();
            brandSub.Text = "HỆ THỐNG QUẢN LÝ CHO THUÊ XE";
            brandSub.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            brandSub.ForeColor = mutedColor;
            brandSub.Location = new Point(28, 72);
            brandSub.AutoSize = true;

            Panel line = new Panel();
            line.BackColor = Color.FromArgb(75, 65, 48);
            line.Location = new Point(22, 112);
            line.Size = new Size(200, 1);

            Label menuTitle = new Label();
            menuTitle.Text = "DANH MỤC CHÍNH";
            menuTitle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            menuTitle.ForeColor = mutedColor;
            menuTitle.Location = new Point(27, 140);
            menuTitle.AutoSize = true;

            sidebar.Controls.Add(brand);
            sidebar.Controls.Add(brandSub);
            sidebar.Controls.Add(line);
            sidebar.Controls.Add(menuTitle);

            string[] menuItems =
            {
                "Tổng quan",
                "Quản lý xe",
                "Danh mục xe cho thuê",
                "Quản lý khách hàng",
                "Quản lý hợp đồng",
                "Giao - nhận xe",
                "Quản lý hóa đơn",
                "Thống kê doanh thu"
            };

            int y = 175;

            foreach (string selectedItem in menuItems)
            {
                // Nhân viên không được xem hóa đơn và thống kê doanh thu
                if (currentUser.Role == "Staff" &&
                    (selectedItem == "Quản lý hóa đơn" ||
                     selectedItem == "Thống kê doanh thu"))
                {
                    continue;
                }

                Button menuButton = new Button();

                menuButton.Text =
                    (selectedItem == "Tổng quan" ? "◆   " : "    ")
                    + selectedItem;

                menuButton.Location = new Point(15, y);
                menuButton.Size = new Size(215, 48);
                menuButton.FlatStyle = FlatStyle.Flat;
                menuButton.FlatAppearance.BorderSize = 0;
                menuButton.TextAlign = ContentAlignment.MiddleLeft;
                menuButton.Padding = new Padding(12, 0, 0, 0);
                menuButton.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                menuButton.Cursor = Cursors.Hand;

                if (selectedItem == "Tổng quan")
                {
                    menuButton.BackColor = Color.FromArgb(55, 48, 37);
                    menuButton.ForeColor = goldColor;
                }
                else
                {
                    menuButton.BackColor = sidebarColor;
                    menuButton.ForeColor = Color.FromArgb(210, 210, 210);
                }

                menuButton.Click += (sender, e) =>
                {
                    if (selectedItem == "Tổng quan")
                    {
                        LoadDashboardData();

                        MessageBox.Show(
                            "Đã cập nhật trang tổng quan LINHXURY.",
                            "LINHXURY",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else if (selectedItem == "Quản lý xe")
                    {
                        using (FrmCars form = new FrmCars())
                            form.ShowDialog();
                    }
                    else if (selectedItem == "Danh mục xe cho thuê")
                    {
                        using (FrmVehicleCatalog catalog =
                            new FrmVehicleCatalog())
                        {
                            if (catalog.ShowDialog() == DialogResult.OK &&
                                catalog.SelectedCarID > 0)
                            {
                                using (FrmContracts form =
                                    new FrmContracts(
                                        currentUser,
                                        catalog.SelectedCarID))
                                {
                                    form.ShowDialog();
                                }
                            }
                        }
                    }
                    else if (selectedItem == "Quản lý khách hàng")
                    {
                        using (FrmCustomers form = new FrmCustomers())
                            form.ShowDialog();
                    }
                    else if (selectedItem == "Quản lý hợp đồng")
                    {
                        using (FrmContracts form =
                            new FrmContracts(currentUser))
                        {
                            form.ShowDialog();
                        }
                    }
                    else if (selectedItem == "Giao - nhận xe")
                    {
                        using (FrmVehicleHandover form =
                            new FrmVehicleHandover(currentUser))
                        {
                            form.ShowDialog();
                        }
                    }
                    else if (selectedItem == "Quản lý hóa đơn")
                    {
                        using (FrmInvoices form =
                            new FrmInvoices(currentUser))
                        {
                            form.ShowDialog();
                        }
                    }
                    else if (selectedItem == "Thống kê doanh thu")
                    {
                        using (FrmRevenueStatistics form =
                            new FrmRevenueStatistics(currentUser))
                        {
                            form.ShowDialog();
                        }
                    }

                    if (selectedItem != "Tổng quan")
                        LoadDashboardData();
                };

                sidebar.Controls.Add(menuButton);
                y += 55;
            }

            Panel bottomLine = new Panel();
            bottomLine.BackColor = Color.FromArgb(75, 65, 48);
            bottomLine.Location = new Point(22, 650);
            bottomLine.Size = new Size(200, 1);

            Label accountTitle = new Label();
            accountTitle.Text = "TÀI KHOẢN HIỆN TẠI";
            accountTitle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            accountTitle.ForeColor = mutedColor;
            accountTitle.Location = new Point(27, 670);
            accountTitle.AutoSize = true;

            Label accountName = new Label();
            accountName.Text = currentUser.FullName;
            accountName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            accountName.ForeColor = Color.White;
            accountName.Location = new Point(27, 700);
            accountName.AutoSize = true;

            Label accountRole = new Label();
            accountRole.Text = "● " + GetRoleName();
            accountRole.Font = new Font("Segoe UI", 8);
            accountRole.ForeColor = goldColor;
            accountRole.Location = new Point(27, 727);
            accountRole.AutoSize = true;

            sidebar.Controls.Add(bottomLine);
            sidebar.Controls.Add(accountTitle);
            sidebar.Controls.Add(accountName);
            sidebar.Controls.Add(accountRole);

            this.Controls.Add(sidebar);
        }

        private void CreateMainContent()
        {
            Panel content = new Panel();
            content.Dock = DockStyle.Fill;
            content.BackColor = backgroundColor;
            content.AutoScroll = true;

            Label pageTitle = new Label();
            pageTitle.Text = "Tổng quan";
            pageTitle.Font = new Font("Georgia", 25, FontStyle.Bold);
            pageTitle.ForeColor = Color.White;
            pageTitle.Location = new Point(35, 28);
            pageTitle.AutoSize = true;

            Label dateLabel = new Label();
            dateLabel.Text = DateTime.Now.ToString(
                "dddd, dd/MM/yyyy",
                CultureInfo.GetCultureInfo("vi-VN")
            );
            dateLabel.Font = new Font("Segoe UI", 9);
            dateLabel.ForeColor = mutedColor;
            dateLabel.Location = new Point(38, 75);
            dateLabel.AutoSize = true;

            Label welcome = new Label();
            welcome.Text = "Xin chào, " + currentUser.FullName;
            welcome.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            welcome.ForeColor = goldColor;
            welcome.Location = new Point(38, 125);
            welcome.AutoSize = true;

            Label description = new Label();
            description.Text =
                "Dưới đây là thông tin tổng quan về hoạt động kinh doanh cho thuê xe.";
            description.Font = new Font("Segoe UI", 9);
            description.ForeColor = mutedColor;
            description.Location = new Point(38, 155);
            description.AutoSize = true;

            Button logoutButton = new Button();
            logoutButton.Text = "ĐĂNG XUẤT";
            logoutButton.Size = new Size(125, 38);
            logoutButton.Location = new Point(950, 35);
            logoutButton.BackColor = Color.FromArgb(45, 40, 32);
            logoutButton.ForeColor = goldColor;
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.FlatAppearance.BorderColor = goldColor;
            logoutButton.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            logoutButton.Cursor = Cursors.Hand;

            logoutButton.Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn đăng xuất?",
                    "LINHXURY",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    Application.Restart();
                }
            };

            content.Controls.Add(pageTitle);
            content.Controls.Add(dateLabel);
            content.Controls.Add(welcome);
            content.Controls.Add(description);
            content.Controls.Add(logoutButton);

            // CÁC THẺ THỐNG KÊ

            lblTotalCars = AddStatCard(
                content,
                "TỔNG SỐ XE",
                "0",
                "Tổng số xe trong hệ thống",
                38, 215
            );

            lblRentedCars = AddStatCard(
                content,
                "XE ĐANG CHO THUÊ",
                "0",
                "Số xe hiện đang được thuê",
                275, 215
            );

            lblTotalCustomers = AddStatCard(
                content,
                "KHÁCH HÀNG",
                "0",
                "Tổng số khách hàng",
                512, 215
            );

            lblActiveContracts = AddStatCard(
                content,
                "HỢP ĐỒNG HOẠT ĐỘNG",
                "0",
                "Số hợp đồng đang hiệu lực",
                749, 215
            );

            // THẺ DOANH THU

            Panel revenueCard = new Panel();
            revenueCard.Location = new Point(38, 395);
            revenueCard.Size = new Size(826, 135);
            revenueCard.BackColor = cardColor;

            Label revenueTitle = new Label();
            revenueTitle.Text = "TỔNG DOANH THU";
            revenueTitle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            revenueTitle.ForeColor = mutedColor;
            revenueTitle.Location = new Point(22, 20);
            revenueTitle.AutoSize = true;

            lblTotalRevenue = new Label();
            lblTotalRevenue.Text = "0 VNĐ";
            lblTotalRevenue.Font = new Font("Georgia", 27, FontStyle.Bold);
            lblTotalRevenue.ForeColor = goldColor;
            lblTotalRevenue.Location = new Point(22, 48);
            lblTotalRevenue.AutoSize = true;

            Label revenueNote = new Label();
            revenueNote.Text =
                "Tổng doanh thu từ các hóa đơn đã thanh toán.";
            revenueNote.Font = new Font("Segoe UI", 8);
            revenueNote.ForeColor = mutedColor;
            revenueNote.Location = new Point(25, 103);
            revenueNote.AutoSize = true;

            revenueCard.Controls.Add(revenueTitle);
            revenueCard.Controls.Add(lblTotalRevenue);
            revenueCard.Controls.Add(revenueNote);

            content.Controls.Add(revenueCard);

            // HOẠT ĐỘNG GẦN ĐÂY

            Panel activityCard = new Panel();
            activityCard.Location = new Point(38, 555);
            activityCard.Size = new Size(826, 155);
            activityCard.BackColor = cardColor;


            Label activityTitle = new Label();
            activityTitle.Text = "GIỚI THIỆU LINHXURY";
            activityTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            activityTitle.ForeColor = goldColor;
            activityTitle.Location = new Point(22, 18);
            activityTitle.AutoSize = true;

            Label activityText = new Label();
            activityText.Text =
            "Mỗi chuyến đi đều bắt đầu từ một trải nghiệm tốt. LINHXURY mang đến giải pháp\n" +
            "quản lý dịch vụ cho thuê xe hiện đại, nơi thông tin về xe, khách hàng và hành trình\n" +
            "thuê xe được kết nối trên cùng một hệ thống.\n" +
            "Từ quản lý phương tiện đến hợp đồng và giao nhận xe, mọi quy trình đều được\n" +
            "sắp xếp rõ ràng, giúp việc vận hành trở nên thuận tiện và chuyên nghiệp hơn.";
            activityText.Font = new Font("Segoe UI", 9);
            activityText.ForeColor = mutedColor;
            activityText.Location = new Point(24, 55);
            activityText.AutoSize = true;

            activityCard.Controls.Add(activityTitle);
            activityCard.Controls.Add(activityText);

            content.Controls.Add(activityCard);

            this.Controls.Add(content);
            content.BringToFront();
            this.Controls.SetChildIndex(content, 0);

        }

        private Label AddStatCard(
            Panel parent,
            string title,
            string value,
            string description,
            int x,
            int y)
        {
            Panel card = new Panel();
            card.Location = new Point(x, y);
            card.Size = new Size(210, 150);
            card.BackColor = cardColor;

            Panel accent = new Panel();
            accent.BackColor = goldColor;
            accent.Location = new Point(0, 0);
            accent.Size = new Size(4, 150);

            Label titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            titleLabel.ForeColor = mutedColor;
            titleLabel.Location = new Point(18, 20);
            titleLabel.AutoSize = true;

            Label valueLabel = new Label();
            valueLabel.Text = value;
            valueLabel.Font = new Font("Georgia", 27, FontStyle.Bold);
            valueLabel.ForeColor = goldColor;
            valueLabel.Location = new Point(18, 48);
            valueLabel.AutoSize = true;

            Label descriptionLabel = new Label();
            descriptionLabel.Text = description;
            descriptionLabel.Font = new Font("Segoe UI", 8);
            descriptionLabel.ForeColor = mutedColor;
            descriptionLabel.Location = new Point(18, 112);
            descriptionLabel.AutoSize = true;

            card.Controls.Add(accent);
            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(descriptionLabel);

            parent.Controls.Add(card);

            return valueLabel;
        }

        private void LoadDashboardData()
        {
            try
            {
                DashboardSummary summary =
                    dashboardService.GetSummary();

                lblTotalCars.Text =
                    summary.TotalCars.ToString("N0");

                lblRentedCars.Text =
                    summary.RentedCars.ToString("N0");

                lblTotalCustomers.Text =
                    summary.TotalCustomers.ToString("N0");

                lblActiveContracts.Text =
                    summary.ActiveContracts.ToString("N0");

                lblTotalRevenue.Text =
                    summary.TotalRevenue.ToString("N0") + " VNĐ";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi trang tổng quan",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
