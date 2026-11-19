using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using CarRentalManagement.BLL.Services;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.Presentation
{
    public class FrmCars : Form
    {
        private readonly CarService carService = new CarService();

        private List<Car> allCars = new List<Car>();
        private int selectedCarID = 0;

        private readonly Color backgroundColor = Color.FromArgb(17, 18, 20);
        private readonly Color cardColor = Color.FromArgb(31, 33, 37);
        private readonly Color goldColor = Color.FromArgb(198, 166, 107);
        private readonly Color mutedColor = Color.FromArgb(155, 155, 155);

        private TextBox txtLicensePlate;
        private TextBox txtBrand;
        private TextBox txtModel;
        private TextBox txtYear;
        private TextBox txtDailyRate;
        private TextBox txtDescription;
        private TextBox txtSearch;
        private TextBox txtImagePath;

        private ComboBox cboCarType;
        private ComboBox cboStatus;

        private DataGridView dgvCars;

        private PictureBox picCar;
        private Button btnChooseImage;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;

        public FrmCars()
        {
            InitializeForm();
            LoadCars();
        }

        private void InitializeForm()
        {
            Text = "LINHXURY | Quản lý xe";
            Size = new Size(1250, 850);
            MinimumSize = new Size(1050, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = backgroundColor;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9);

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 4;
            root.Padding = new Padding(20);
            root.BackColor = backgroundColor;

            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 260));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));

            CreateHeader(root);
            CreateInputPanel(root);
            CreateDataGrid(root);
            CreateButtons(root);

            Controls.Add(root);
        }

        private void CreateHeader(TableLayoutPanel root)
        {
            Panel header = new Panel();
            header.Dock = DockStyle.Fill;
            header.BackColor = backgroundColor;

            Label title = new Label();
            title.Text = "QUẢN LÝ XE";
            title.Font = new Font("Georgia", 22, FontStyle.Bold);
            title.ForeColor = goldColor;
            title.Location = new Point(5, 5);
            title.AutoSize = true;

            Label subtitle = new Label();
            subtitle.Text = "Quản lý danh sách xe cho thuê của LINHXURY";
            subtitle.ForeColor = mutedColor;
            subtitle.Location = new Point(8, 48);
            subtitle.AutoSize = true;

            Label searchLabel = new Label();
            searchLabel.Text = "TÌM KIẾM";
            searchLabel.ForeColor = goldColor;
            searchLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            searchLabel.Location = new Point(700, 10);
            searchLabel.AutoSize = true;

            txtSearch = new TextBox();
            txtSearch.Location = new Point(700, 35);
            txtSearch.Size = new Size(470, 30);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.BackColor = cardColor;
            txtSearch.ForeColor = Color.White;
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.TextChanged += (s, e) => FilterCars();

            header.Controls.Add(title);
            header.Controls.Add(subtitle);
            header.Controls.Add(searchLabel);
            header.Controls.Add(txtSearch);

            root.Controls.Add(header, 0, 0);
        }

        private void CreateInputPanel(TableLayoutPanel root)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = cardColor;

            txtLicensePlate = AddTextField(panel, "BIỂN SỐ", 15, 10, 190);
            txtBrand = AddTextField(panel, "HÃNG XE", 235, 10, 180);
            txtModel = AddTextField(panel, "DÒNG XE", 445, 10, 180);
            txtYear = AddTextField(panel, "NĂM SX", 655, 10, 130);

            cboCarType = AddComboField(
                panel, "LOẠI XE", 815, 10, 330,
                new string[]
                {
                    "Siêu xe hiệu năng cực cao",
                    "Siêu xe",
                    "Coupe",
                    "Xe mui trần",
                    "SUV",
                    "Sedan",
                    "Xe Grand Tourer (GT)",
                    "Xe điện (EV)"
                });

            txtDailyRate = AddTextField(
                panel, "GIÁ THUÊ / NGÀY (VNĐ)", 15, 90, 190);

            cboStatus = AddComboField(
                panel, "TRẠNG THÁI", 235, 90, 180,
                new string[]
                {
                    "Available", "Rented", "Maintenance"
                });

            txtDescription = AddTextField(
                panel, "MÔ TẢ", 445, 90, 700);

            Label lblImage = new Label();
            lblImage.Text = "HÌNH ẢNH XE";
            lblImage.ForeColor = goldColor;
            lblImage.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblImage.Location = new Point(15, 170);
            lblImage.AutoSize = true;

            txtImagePath = new TextBox();
            txtImagePath.Location = new Point(15, 195);
            txtImagePath.Size = new Size(500, 30);
            txtImagePath.ReadOnly = true;
            txtImagePath.BackColor = Color.FromArgb(43, 45, 49);
            txtImagePath.ForeColor = Color.White;
            txtImagePath.BorderStyle = BorderStyle.FixedSingle;

            btnChooseImage = CreateButton(
                "CHỌN ẢNH",
                goldColor,
                Color.Black);

            btnChooseImage.Location = new Point(530, 192);
            btnChooseImage.Size = new Size(145, 35);
            btnChooseImage.Click += BtnChooseImage_Click;

            picCar = new PictureBox();
            picCar.Location = new Point(700, 165);
            picCar.Size = new Size(130, 85);
            picCar.BackColor = Color.FromArgb(43, 45, 49);
            picCar.SizeMode = PictureBoxSizeMode.Zoom;
            picCar.BorderStyle = BorderStyle.FixedSingle;

            panel.Controls.Add(lblImage);
            panel.Controls.Add(txtImagePath);
            panel.Controls.Add(btnChooseImage);
            panel.Controls.Add(picCar);

            root.Controls.Add(panel, 0, 1);
        }

        private TextBox AddTextField(
            Panel parent,
            string labelText,
            int x,
            int y,
            int width)
        {
            Label label = new Label();
            label.Text = labelText;
            label.ForeColor = goldColor;
            label.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            label.Location = new Point(x, y);
            label.AutoSize = true;

            TextBox textBox = new TextBox();
            textBox.Location = new Point(x, y + 25);
            textBox.Size = new Size(width, 30);
            textBox.Font = new Font("Segoe UI", 9);
            textBox.BackColor = Color.FromArgb(43, 45, 49);
            textBox.ForeColor = Color.White;
            textBox.BorderStyle = BorderStyle.FixedSingle;

            parent.Controls.Add(label);
            parent.Controls.Add(textBox);

            return textBox;
        }

        private ComboBox AddComboField(
            Panel parent,
            string labelText,
            int x,
            int y,
            int width,
            string[] items)
        {
            Label label = new Label();
            label.Text = labelText;
            label.ForeColor = goldColor;
            label.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            label.Location = new Point(x, y);
            label.AutoSize = true;

            ComboBox combo = new ComboBox();
            combo.Location = new Point(x, y + 25);
            combo.Size = new Size(width, 30);
            combo.Font = new Font("Segoe UI", 9);
            combo.BackColor = Color.FromArgb(43, 45, 49);
            combo.ForeColor = Color.White;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;

            combo.Items.AddRange(items);

            parent.Controls.Add(label);
            parent.Controls.Add(combo);

            return combo;
        }

        private void CreateDataGrid(TableLayoutPanel root)
        {
            dgvCars = new DataGridView();

            dgvCars.Dock = DockStyle.Fill;
            dgvCars.BackgroundColor = cardColor;
            dgvCars.BorderStyle = BorderStyle.None;
            dgvCars.GridColor = Color.FromArgb(55, 55, 55);

            dgvCars.AutoGenerateColumns = false;
            dgvCars.AllowUserToAddRows = false;
            dgvCars.AllowUserToDeleteRows = false;
            dgvCars.ReadOnly = true;
            dgvCars.MultiSelect = false;
            dgvCars.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCars.RowHeadersVisible = false;
            dgvCars.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvCars.EnableHeadersVisualStyles = false;
            dgvCars.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(45, 40, 32);
            dgvCars.ColumnHeadersDefaultCellStyle.ForeColor = goldColor;
            dgvCars.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvCars.ColumnHeadersHeight = 42;

            dgvCars.DefaultCellStyle.BackColor = cardColor;
            dgvCars.DefaultCellStyle.ForeColor = Color.White;
            dgvCars.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(75, 62, 42);
            dgvCars.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvCars.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvCars.RowTemplate.Height = 35;

            AddColumn("CarID", "MÃ XE", "CarID", 45);
            AddColumn("LicensePlate", "BIỂN SỐ", "LicensePlate", 110);
            AddColumn("Brand", "HÃNG XE", "Brand", 90);
            AddColumn("Model", "DÒNG XE", "Model", 100);
            AddColumn("ManufactureYear", "NĂM SX", "ManufactureYear", 65);
            AddColumn("CarType", "LOẠI XE", "CarType", 85);
            AddColumn("DailyRate", "GIÁ THUÊ / NGÀY", "DailyRate", 120);
            AddColumn("Status", "TRẠNG THÁI", "Status", 90);
            AddColumn("Description", "MÔ TẢ", "Description", 150);

            dgvCars.SelectionChanged += DgvCars_SelectionChanged;
            dgvCars.CellFormatting += DgvCars_CellFormatting;

            root.Controls.Add(dgvCars, 0, 2);
        }

        private void DgvCars_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null)
                return;

            string columnName = dgvCars.Columns[e.ColumnIndex].Name;
            string value = e.Value.ToString();

            if (columnName == "CarType")
            {
                e.Value = GetCarTypeDisplay(value);
                e.FormattingApplied = true;
            }
            else if (columnName == "Status")
            {
                switch (value)
                {
                    case "Available":
                        e.Value = "Sẵn sàng";
                        break;
                    case "Rented":
                        e.Value = "Đang thuê";
                        break;
                    case "Maintenance":
                        e.Value = "Bảo dưỡng";
                        break;
                }

                e.FormattingApplied = true;
            }
        }

        private void AddColumn(
            string name,
            string header,
            string property,
            int minimumWidth)
        {
            DataGridViewTextBoxColumn column =
                new DataGridViewTextBoxColumn();

            column.Name = name;
            column.HeaderText = header;
            column.DataPropertyName = property;
            column.MinimumWidth = minimumWidth;
            column.SortMode = DataGridViewColumnSortMode.Automatic;

            if (name == "DailyRate")
                column.DefaultCellStyle.Format = "N0";

            dgvCars.Columns.Add(column);
        }

        private void CreateButtons(TableLayoutPanel root)
        {
            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = backgroundColor;
            panel.FlowDirection = FlowDirection.LeftToRight;
            panel.Padding = new Padding(0, 12, 0, 0);

            btnAdd = CreateButton("THÊM XE", goldColor, Color.Black);
            btnUpdate = CreateButton(
                "CẬP NHẬT",
                Color.FromArgb(65, 75, 85),
                Color.White);
            btnDelete = CreateButton(
                "XÓA XE",
                Color.FromArgb(120, 48, 48),
                Color.White);
            btnClear = CreateButton(
                "LÀM MỚI",
                Color.FromArgb(55, 57, 62),
                Color.White);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearForm();

            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnUpdate);
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnClear);

            root.Controls.Add(panel, 0, 3);
        }

        private Button CreateButton(
            string text,
            Color backColor,
            Color foreColor)
        {
            Button button = new Button();

            button.Text = text;
            button.Size = new Size(130, 40);
            button.Margin = new Padding(0, 0, 12, 0);
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            button.Cursor = Cursors.Hand;

            return button;
        }
        // Chuyển tên loại xe tiếng Việt trên giao diện
        // sang giá trị tiếng Anh lưu trong SQL.
        private string GetCarTypeValue(string displayName)
        {
            switch (displayName)
            {
                case "Siêu xe hiệu năng cực cao":
                    return "Hypercar";

                case "Siêu xe":
                    return "Supercar";

                case "Coupe":
                    return "Coupe";

                case "Xe mui trần":
                    return "Convertible";

                case "SUV":
                    return "SUV";

                case "Sedan":
                    return "Sedan";

                case "Xe Grand Tourer (GT)":
                    return "Grand Tourer (GT)";

                case "Xe điện (EV)":
                    return "Electric (EV)";

                default:
                    return displayName;
            }
        }

        // Chuyển giá trị loại xe trong SQL
        // sang tên tiếng Việt trên giao diện.
        private string GetCarTypeDisplay(string carType)
        {
            switch (carType)
            {
                case "Hypercar":
                    return "Siêu xe hiệu năng cực cao";

                case "Supercar":
                    return "Siêu xe";

                case "Coupe":
                    return "Coupe";

                case "Convertible":
                    return "Xe mui trần";

                case "SUV":
                    return "SUV";

                case "Sedan":
                    return "Sedan";

                case "Grand Tourer (GT)":
                    return "Xe Grand Tourer (GT)";

                case "Electric (EV)":
                    return "Xe điện (EV)";

                default:
                    return carType;
            }
        }

        // Lấy đường dẫn ảnh thực tế từ đường dẫn tương đối
        // hoặc đường dẫn cũ dạng tuyệt đối.
        private string GetImageFullPath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return "";

            if (Path.IsPathRooted(imagePath))
                return imagePath;

            return Path.Combine(
                Application.StartupPath,
                imagePath);
        }

        // CHỌN ẢNH TỪ MÁY TÍNH
        private void BtnChooseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn hình ảnh xe";
                dialog.Filter =
                    "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Image image = Image.FromFile(dialog.FileName))
                        {
                            Image preview = new Bitmap(image);

                            if (picCar.Image != null)
                            {
                                picCar.Image.Dispose();
                                picCar.Image = null;
                            }

                            picCar.Image = preview;
                        }

                        string imageFolder = Path.Combine(
                            Application.StartupPath,
                            "Assets",
                            "CarImages");

                        if (!Directory.Exists(imageFolder))
                            Directory.CreateDirectory(imageFolder);

                        string extension =
                            Path.GetExtension(dialog.FileName);

                        string fileName =
                            Guid.NewGuid().ToString("N") + extension;

                        string destinationPath =
                            Path.Combine(imageFolder, fileName);

                        File.Copy(
                            dialog.FileName,
                            destinationPath,
                            true);

                        // Chỉ lưu đường dẫn tương đối vào database.
                        txtImagePath.Text = Path.Combine(
                            "Assets",
                            "CarImages",
                            fileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Không thể tải hình ảnh!\n" + ex.Message,
                            "LINHXURY",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
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
                    "Không thể tải danh sách xe.\n\nChi tiết: " +
                    ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FilterCars()
        {
            if (dgvCars == null)
                return;

            string keyword = txtSearch.Text.Trim().ToLower();

            List<Car> filteredCars = allCars
                .Where(c =>
                    (c.LicensePlate ?? "").ToLower().Contains(keyword) ||
                    (c.Brand ?? "").ToLower().Contains(keyword) ||
                    (c.Model ?? "").ToLower().Contains(keyword) ||
                    (c.CarType ?? "").ToLower().Contains(keyword) ||
                    GetCarTypeDisplay(c.CarType ?? "")
                        .ToLower().Contains(keyword) ||
                    (c.Status ?? "").ToLower().Contains(keyword))
                .ToList();

            dgvCars.DataSource = null;
            dgvCars.DataSource = filteredCars;
        }

        private void DgvCars_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (dgvCars.CurrentRow == null ||
                dgvCars.CurrentRow.DataBoundItem == null)
                return;

            Car car =
                dgvCars.CurrentRow.DataBoundItem as Car;

            if (car == null)
                return;

            selectedCarID = car.CarID;

            txtLicensePlate.Text = car.LicensePlate;
            txtBrand.Text = car.Brand;
            txtModel.Text = car.Model;
            txtYear.Text = car.ManufactureYear.ToString();
            txtDailyRate.Text = car.DailyRate.ToString("0.##");

            cboCarType.SelectedItem =
                GetCarTypeDisplay(car.CarType);

            cboStatus.Text = car.Status;
            txtDescription.Text = car.Description;
            txtImagePath.Text = car.ImagePath ?? "";

            if (picCar.Image != null)
            {
                picCar.Image.Dispose();
                picCar.Image = null;
            }

            string fullImagePath =
                GetImageFullPath(car.ImagePath);

            if (!string.IsNullOrWhiteSpace(fullImagePath) &&
                File.Exists(fullImagePath))
            {
                try
                {
                    using (Image image = Image.FromFile(fullImagePath))
                    {
                        picCar.Image = new Bitmap(image);
                    }
                }
                catch
                {
                    picCar.Image = null;
                }
            }
        }

        private bool GetCarFromForm(out Car car)
        {
            car = null;

            if (string.IsNullOrWhiteSpace(txtLicensePlate.Text) ||
                string.IsNullOrWhiteSpace(txtBrand.Text) ||
                string.IsNullOrWhiteSpace(txtModel.Text) ||
                string.IsNullOrWhiteSpace(txtYear.Text) ||
                string.IsNullOrWhiteSpace(txtDailyRate.Text) ||
                string.IsNullOrWhiteSpace(cboCarType.Text) ||
                string.IsNullOrWhiteSpace(cboStatus.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ các thông tin bắt buộc!",
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            int year;

            if (!int.TryParse(txtYear.Text.Trim(), out year) ||
                year < 1900 ||
                year > DateTime.Now.Year + 1)
            {
                MessageBox.Show("Năm sản xuất không hợp lệ!");
                txtYear.Focus();
                return false;
            }

            decimal dailyRate;

            if (!decimal.TryParse(
                txtDailyRate.Text.Trim(),
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out dailyRate) ||
                dailyRate <= 0)
            {
                MessageBox.Show(
                    "Giá thuê phải là số hợp lệ lớn hơn 0!");

                txtDailyRate.Focus();
                return false;
            }

            car = new Car
            {
                CarID = selectedCarID,
                LicensePlate = txtLicensePlate.Text.Trim(),
                Brand = txtBrand.Text.Trim(),
                Model = txtModel.Text.Trim(),
                ManufactureYear = year,

                CarType = GetCarTypeValue(
                    cboCarType.Text.Trim()),

                DailyRate = dailyRate,
                Status = cboStatus.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                ImagePath = txtImagePath.Text.Trim()
            };

            return true;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GetCarFromForm(out Car car))
                    return;

                bool result = carService.AddCar(car);

                if (result)
                {
                    MessageBox.Show(
                        "Thêm xe mới thành công!",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadCars();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm xe.\n\n" + ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedCarID <= 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn xe cần cập nhật!");

                    return;
                }

                if (!GetCarFromForm(out Car car))
                    return;

                bool result = carService.UpdateCar(car);

                if (result)
                {
                    MessageBox.Show(
                        "Cập nhật xe thành công!",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadCars();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật xe.\n\n" + ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCarID <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn xe cần xóa!");

                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa xe này?",
                "XÁC NHẬN XÓA",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                bool result =
                    carService.DeleteCar(selectedCarID);

                if (result)
                {
                    MessageBox.Show(
                        "Xóa xe thành công!",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadCars();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        "Không tìm thấy xe cần xóa!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa xe. Xe có thể đang được sử dụng trong hợp đồng.\n\n" +
                    ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            selectedCarID = 0;

            txtLicensePlate.Clear();
            txtBrand.Clear();
            txtModel.Clear();
            txtYear.Clear();
            txtDailyRate.Clear();
            txtDescription.Clear();
            txtImagePath.Clear();

            cboCarType.SelectedIndex = -1;
            cboCarType.Text = "";

            cboStatus.SelectedIndex = -1;
            cboStatus.Text = "Available";

            if (picCar.Image != null)
            {
                picCar.Image.Dispose();
                picCar.Image = null;
            }

            dgvCars.ClearSelection();
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            if (picCar != null && picCar.Image != null)
            {
                picCar.Image.Dispose();
                picCar.Image = null;
            }

            base.OnFormClosed(e);
        }
    }
}