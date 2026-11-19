
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CarRentalManagement.BLL.Services;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.Presentation
{
    public class FrmVehicleCatalog : Form
    {
        private readonly CarService carService;
        private List<Car> allCars = new List<Car>();

        private TextBox txtSearch;
        private ComboBox cboType;
        private ComboBox cboStatus;
        private FlowLayoutPanel flowCars;
        private Label lblResult;

        public int SelectedCarID { get; private set; }

        private readonly Color black = Color.FromArgb(17, 18, 20);
        private readonly Color panelColor = Color.FromArgb(28, 30, 33);
        private readonly Color gold = Color.FromArgb(198, 166, 107);
        private readonly Color ivory = Color.FromArgb(243, 239, 230);
        private readonly Color gray = Color.FromArgb(145, 145, 145);

        public FrmVehicleCatalog()
        {
            carService = new CarService();

            InitializeInterface();
            LoadCars();
        }

        private void InitializeInterface()
        {
            Text = "LINHXURY | Danh mục xe cho thuê";
            Size = new Size(1250, 820);
            MinimumSize = new Size(1050, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = black;
            ForeColor = ivory;
            Font = new Font("Segoe UI", 9F);

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 115,
                BackColor = black
            };

            Label title = new Label
            {
                Text = "DANH MỤC XE CHO THUÊ",
                Font = new Font("Georgia", 23F, FontStyle.Bold),
                ForeColor = gold,
                AutoSize = true,
                Location = new Point(25, 15)
            };

            Label subtitle = new Label
            {
                Text = "Khám phá và lựa chọn dòng xe phù hợp",
                ForeColor = gray,
                AutoSize = true,
                Location = new Point(29, 58)
            };

            header.Controls.Add(title);
            header.Controls.Add(subtitle);

            Panel filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = panelColor,
                Padding = new Padding(20, 15, 20, 10)
            };

            txtSearch = new TextBox
            {
                Location = new Point(20, 20),
                Size = new Size(300, 28),
                BackColor = black,
                ForeColor = ivory,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };

            txtSearch.TextChanged += (s, e) => FilterCars();

            cboType = new ComboBox
            {
                Location = new Point(340, 19),
                Size = new Size(220, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = black,
                ForeColor = ivory,
                FlatStyle = FlatStyle.Flat
            };

            cboType.Items.AddRange(new object[]
            {
                "Tất cả loại xe",
                "Siêu xe",
                "Hypercar",
                "Coupe",
                "Mui trần",
                "SUV",
                "Sedan",
                "Grand Tourer (GT)",
                "Xe điện (EV)"
            });

            cboType.SelectedIndex = 0;
            cboType.SelectedIndexChanged += (s, e) => FilterCars();

            cboStatus = new ComboBox
            {
                Location = new Point(580, 19),
                Size = new Size(190, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = black,
                ForeColor = ivory,
                FlatStyle = FlatStyle.Flat
            };

            cboStatus.Items.AddRange(new object[]
            {
                "Tất cả trạng thái",
                "Sẵn sàng",
                "Đang thuê",
                "Bảo dưỡng"
            });

            cboStatus.SelectedIndex = 0;
            cboStatus.SelectedIndexChanged += (s, e) => FilterCars();

            lblResult = new Label
            {
                Text = "0 xe",
                Location = new Point(800, 24),
                AutoSize = true,
                ForeColor = gold,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(cboType);
            filterPanel.Controls.Add(cboStatus);
            filterPanel.Controls.Add(lblResult);

            flowCars = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = black,
                AutoScroll = true,
                WrapContents = true,
                Padding = new Padding(18, 18, 18, 18)
            };

            Controls.Add(flowCars);
            Controls.Add(filterPanel);
            Controls.Add(header);
        }

        private void LoadCars()
        {
            try
            {
                allCars = carService.GetAllCars();
                FilterCars();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách xe.\n" + ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FilterCars()
        {
            if (flowCars == null)
                return;

            flowCars.Controls.Clear();

            string keyword = txtSearch.Text.Trim().ToLower();
            string selectedType = cboType.SelectedItem == null
                ? "Tất cả loại xe"
                : cboType.SelectedItem.ToString();

            string selectedStatus = cboStatus.SelectedItem == null
                ? "Tất cả trạng thái"
                : cboStatus.SelectedItem.ToString();

            List<Car> filteredCars = allCars.Where(car =>
            {
                string searchText =
                    (car.Brand ?? "") + " " +
                    (car.Model ?? "") + " " +
                    (car.LicensePlate ?? "");

                bool matchKeyword =
                    searchText.ToLower().Contains(keyword);

                bool matchType =
                    selectedType == "Tất cả loại xe" ||
                    GetVietnameseType(car.CarType) == selectedType;

                bool matchStatus =
                    selectedStatus == "Tất cả trạng thái" ||
                    GetVietnameseStatus(car.Status) == selectedStatus;

                return matchKeyword && matchType && matchStatus;
            }).ToList();

            lblResult.Text = filteredCars.Count + " xe";

            foreach (Car car in filteredCars)
                flowCars.Controls.Add(CreateCarCard(car));

            if (filteredCars.Count == 0)
            {
                Label empty = new Label
                {
                    Text = "Không tìm thấy xe phù hợp.",
                    ForeColor = gray,
                    Font = new Font("Segoe UI", 12F),
                    AutoSize = true,
                    Margin = new Padding(20)
                };

                flowCars.Controls.Add(empty);
            }
        }

        private Panel CreateCarCard(Car car)
        {
            Panel card = new Panel
            {
                Size = new Size(350, 430),
                BackColor = panelColor,
                Margin = new Padding(12)
            };

            PictureBox picture = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(330, 190),
                BackColor = Color.FromArgb(38, 40, 43),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            LoadCarImage(picture, car.ImagePath);

            Label brandModel = new Label
            {
                Text = (car.Brand ?? "") + " " + (car.Model ?? ""),
                Location = new Point(17, 210),
                Size = new Size(315, 30),
                ForeColor = ivory,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold)
            };

            Label type = new Label
            {
                Text = GetVietnameseType(car.CarType) +
                       "  |  " + car.ManufactureYear,
                Location = new Point(18, 245),
                Size = new Size(310, 23),
                ForeColor = gray
            };

            Label plate = new Label
            {
                Text = "Biển số: " + car.LicensePlate,
                Location = new Point(18, 272),
                Size = new Size(310, 23),
                ForeColor = gray
            };

            Label price = new Label
            {
                Text = car.DailyRate.ToString("N0") + " VNĐ / ngày",
                Location = new Point(18, 305),
                Size = new Size(315, 30),
                ForeColor = gold,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold)
            };

            Label status = new Label
            {
                Text = "Trạng thái: " + GetVietnameseStatus(car.Status),
                Location = new Point(18, 340),
                Size = new Size(310, 22),
                ForeColor = car.Status == "Available"
                    ? Color.LightGreen
                    : Color.Orange
            };

            Button choose = new Button
            {
                Text = car.Status == "Available"
                    ? "CHỌN XE"
                    : "XE CHƯA SẴN SÀNG",
                Location = new Point(18, 375),
                Size = new Size(310, 40),
                BackColor = car.Status == "Available"
                    ? gold
                    : Color.FromArgb(65, 65, 65),
                ForeColor = black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = car.Status == "Available"
            };

            choose.FlatAppearance.BorderSize = 0;

            choose.Click += (s, e) =>
            {
                SelectedCarID = car.CarID;
                DialogResult = DialogResult.OK;
                Close();
            };

            card.Controls.Add(picture);
            card.Controls.Add(brandModel);
            card.Controls.Add(type);
            card.Controls.Add(plate);
            card.Controls.Add(price);
            card.Controls.Add(status);
            card.Controls.Add(choose);

            return card;
        }

        private void LoadCarImage(PictureBox picture, string imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                    return;

                string fullPath = imagePath;

                if (!Path.IsPathRooted(fullPath))
                {
                    fullPath = Path.Combine(
                        Application.StartupPath,
                        imagePath);
                }

                if (!File.Exists(fullPath))
                    return;

                using (Image source = Image.FromFile(fullPath))
                {
                    picture.Image = new Bitmap(source);
                }
            }
            catch
            {
                picture.Image = null;
            }
        }

        private string GetVietnameseType(string type)
        {
            switch (type)
            {
                case "Supercar": return "Siêu xe";
                case "Hypercar": return "Hypercar";
                case "Coupe": return "Coupe";
                case "Convertible": return "Mui trần";
                case "SUV": return "SUV";
                case "Sedan": return "Sedan";
                case "Grand Tourer (GT)": return "Grand Tourer (GT)";
                case "Electric (EV)": return "Xe điện (EV)";
                default: return type ?? "";
            }
        }

        private string GetVietnameseStatus(string status)
        {
            switch (status)
            {
                case "Available": return "Sẵn sàng";
                case "Rented": return "Đang thuê";
                case "Maintenance": return "Bảo dưỡng";
                default: return status ?? "";
            }
        }
    }
}
