
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CarRentalManagement.BLL.Services;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.Presentation
{
    public class FrmCustomers : Form
    {
        private readonly CustomerService customerService =
            new CustomerService();

        private List<Customer> allCustomers =
            new List<Customer>();

        private int selectedCustomerID = 0;

        private readonly Color backgroundColor =
            Color.FromArgb(17, 18, 20);

        private readonly Color cardColor =
            Color.FromArgb(31, 33, 37);

        private readonly Color goldColor =
            Color.FromArgb(198, 166, 107);

        private readonly Color mutedColor =
            Color.FromArgb(155, 155, 155);

        private TextBox txtFullName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtCitizenID;
        private TextBox txtDriverLicense;
        private TextBox txtAddress;
        private TextBox txtSearch;

        private DataGridView dgvCustomers;

        public FrmCustomers()
        {
            InitializeForm();
            LoadCustomers();
        }

        // KHỞI TẠO GIAO DIỆN
        private void InitializeForm()
        {
            Text = "LINHXURY | Quản lý khách hàng";
            Size = new Size(1250, 800);
            MinimumSize = new Size(1050, 700);
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

            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 85));

            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 175));

            root.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 65));

            CreateHeader(root);
            CreateInputPanel(root);
            CreateDataGrid(root);
            CreateButtons(root);

            Controls.Add(root);
        }

        // PHẦN TIÊU ĐỀ VÀ TÌM KIẾM
        private void CreateHeader(TableLayoutPanel root)
        {
            Panel header = new Panel();

            header.Dock = DockStyle.Fill;
            header.BackColor = backgroundColor;

            Label title = new Label();

            title.Text = "QUẢN LÝ KHÁCH HÀNG";
            title.Font = new Font(
                "Georgia", 22, FontStyle.Bold);
            title.ForeColor = goldColor;
            title.Location = new Point(5, 5);
            title.AutoSize = true;

            Label subtitle = new Label();

            subtitle.Text =
                "Quản lý thông tin và hồ sơ khách hàng";
            subtitle.ForeColor = mutedColor;
            subtitle.Location = new Point(8, 48);
            subtitle.AutoSize = true;

            Label searchLabel = new Label();

            searchLabel.Text = "TÌM KIẾM";
            searchLabel.ForeColor = goldColor;
            searchLabel.Font = new Font(
                "Segoe UI", 8, FontStyle.Bold);
            searchLabel.Location = new Point(700, 10);
            searchLabel.AutoSize = true;

            txtSearch = new TextBox();

            txtSearch.Location = new Point(700, 35);
            txtSearch.Size = new Size(470, 30);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.BackColor = cardColor;
            txtSearch.ForeColor = Color.White;
            txtSearch.BorderStyle = BorderStyle.FixedSingle;

            txtSearch.TextChanged +=
                (s, e) => FilterCustomers();

            header.Controls.Add(title);
            header.Controls.Add(subtitle);
            header.Controls.Add(searchLabel);
            header.Controls.Add(txtSearch);

            root.Controls.Add(header, 0, 0);
        }

        // KHU VỰC NHẬP THÔNG TIN KHÁCH HÀNG
        private void CreateInputPanel(TableLayoutPanel root)
        {
            Panel panel = new Panel();

            panel.Dock = DockStyle.Fill;
            panel.BackColor = cardColor;

            txtFullName = AddTextField(
                panel, "HỌ VÀ TÊN", 15, 10, 190);

            txtPhone = AddTextField(
                panel, "SỐ ĐIỆN THOẠI", 235, 10, 180);

            txtEmail = AddTextField(
                panel, "EMAIL", 445, 10, 210);

            txtCitizenID = AddTextField(
                panel, "SỐ CCCD", 685, 10, 190);

            txtDriverLicense = AddTextField(
                panel, "SỐ GIẤY PHÉP LÁI XE", 905, 10, 240);

            txtAddress = AddTextField(
                panel, "ĐỊA CHỈ", 15, 90, 1130);

            root.Controls.Add(panel, 0, 1);
        }

        // TẠO Ô NHẬP LIỆU
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
            label.Font = new Font(
                "Segoe UI", 8, FontStyle.Bold);
            label.Location = new Point(x, y);
            label.AutoSize = true;

            TextBox textBox = new TextBox();

            textBox.Location = new Point(x, y + 25);
            textBox.Size = new Size(width, 30);
            textBox.Font = new Font("Segoe UI", 9);
            textBox.BackColor =
                Color.FromArgb(43, 45, 49);
            textBox.ForeColor = Color.White;
            textBox.BorderStyle = BorderStyle.FixedSingle;

            parent.Controls.Add(label);
            parent.Controls.Add(textBox);

            return textBox;
        }

        // BẢNG DANH SÁCH KHÁCH HÀNG
        private void CreateDataGrid(TableLayoutPanel root)
        {
            dgvCustomers = new DataGridView();

            dgvCustomers.Dock = DockStyle.Fill;
            dgvCustomers.BackgroundColor = cardColor;
            dgvCustomers.BorderStyle = BorderStyle.None;
            dgvCustomers.GridColor =
                Color.FromArgb(55, 55, 55);

            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.MultiSelect = false;

            dgvCustomers.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvCustomers.RowHeadersVisible = false;

            dgvCustomers.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvCustomers.EnableHeadersVisualStyles = false;

            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(45, 40, 32);

            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor =
                goldColor;

            dgvCustomers.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvCustomers.ColumnHeadersHeight = 42;

            dgvCustomers.DefaultCellStyle.BackColor = cardColor;
            dgvCustomers.DefaultCellStyle.ForeColor = Color.White;

            dgvCustomers.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(75, 62, 42);

            dgvCustomers.DefaultCellStyle.SelectionForeColor =
                Color.White;

            dgvCustomers.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);

            dgvCustomers.RowTemplate.Height = 35;

            AddColumn("CustomerID", "MÃ KHÁCH HÀNG",
                "CustomerID", 45);

            AddColumn("FullName", "HỌ VÀ TÊN",
                "FullName", 140);

            AddColumn("Phone", "SỐ ĐIỆN THOẠI",
                "Phone", 100);

            AddColumn("Email", "EMAIL",
                "Email", 150);

            AddColumn("CitizenID", "SỐ CCCD",
                "CitizenID", 110);

            AddColumn("DriverLicense", "GIẤY PHÉP LÁI XE",
                "DriverLicense", 110);

            AddColumn("Address", "ĐỊA CHỈ",
                "Address", 180);

            dgvCustomers.SelectionChanged +=
                DgvCustomers_SelectionChanged;

            root.Controls.Add(dgvCustomers, 0, 2);
        }

        // THÊM CỘT VÀO BẢNG
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

            column.SortMode =
                DataGridViewColumnSortMode.Automatic;

            dgvCustomers.Columns.Add(column);
        }

        // CÁC NÚT CHỨC NĂNG
        private void CreateButtons(TableLayoutPanel root)
        {
            FlowLayoutPanel panel = new FlowLayoutPanel();

            panel.Dock = DockStyle.Fill;
            panel.BackColor = backgroundColor;

            panel.FlowDirection =
                FlowDirection.LeftToRight;

            panel.Padding = new Padding(0, 12, 0, 0);

            Button btnAdd = CreateButton(
                "THÊM KHÁCH HÀNG",
                goldColor,
                Color.Black);

            Button btnUpdate = CreateButton(
                "CẬP NHẬT",
                Color.FromArgb(65, 75, 85),
                Color.White);

            Button btnDelete = CreateButton(
                "XÓA",
                Color.FromArgb(120, 48, 48),
                Color.White);

            Button btnClear = CreateButton(
                "LÀM MỚI",
                Color.FromArgb(55, 57, 62),
                Color.White);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            btnClear.Click +=
                (s, e) => ClearForm();

            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnUpdate);
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnClear);

            root.Controls.Add(panel, 0, 3);
        }

        // TẠO NÚT
        private Button CreateButton(
            string text,
            Color backColor,
            Color foreColor)
        {
            Button button = new Button();

            button.Text = text;
            button.Size = new Size(145, 40);
            button.Margin = new Padding(0, 0, 12, 0);
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;

            button.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);

            button.Cursor = Cursors.Hand;

            return button;
        }

        // TẢI DANH SÁCH KHÁCH HÀNG
        private void LoadCustomers()
        {
            try
            {
                allCustomers =
                    customerService.GetAllCustomers();

                FilterCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách khách hàng.\n\n" +
                    ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // TÌM KIẾM VÀ LỌC KHÁCH HÀNG
        private void FilterCustomers()
        {
            if (dgvCustomers == null)
                return;

            string keyword = txtSearch.Text.Trim()
                .ToLower();

            List<Customer> filteredCustomers =
                allCustomers.Where(c =>
                    (c.FullName ?? "").ToLower()
                        .Contains(keyword) ||

                    (c.Phone ?? "").ToLower()
                        .Contains(keyword) ||

                    (c.Email ?? "").ToLower()
                        .Contains(keyword) ||

                    (c.CitizenID ?? "").ToLower()
                        .Contains(keyword) ||

                    (c.DriverLicense ?? "").ToLower()
                        .Contains(keyword) ||

                    (c.Address ?? "").ToLower()
                        .Contains(keyword))
                .ToList();

            dgvCustomers.DataSource = null;
            dgvCustomers.DataSource = filteredCustomers;
        }

        // CHỌN KHÁCH HÀNG TRÊN BẢNG
        private void DgvCustomers_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null ||
                dgvCustomers.CurrentRow.DataBoundItem == null)
                return;

            Customer customer =
                dgvCustomers.CurrentRow.DataBoundItem
                as Customer;

            if (customer == null)
                return;

            selectedCustomerID = customer.CustomerID;

            txtFullName.Text = customer.FullName;
            txtPhone.Text = customer.Phone;
            txtEmail.Text = customer.Email;
            txtCitizenID.Text = customer.CitizenID;
            txtDriverLicense.Text = customer.DriverLicense;
            txtAddress.Text = customer.Address;
        }

        // LẤY THÔNG TIN KHÁCH HÀNG TỪ FORM
        private bool GetCustomerFromForm(
            out Customer customer)
        {
            customer = null;

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập họ và tên khách hàng!");

                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập số điện thoại!");

                txtPhone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCitizenID.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập số CCCD!");

                txtCitizenID.Focus();
                return false;
            }

            customer = new Customer
            {
                CustomerID = selectedCustomerID,
                FullName = txtFullName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                CitizenID = txtCitizenID.Text.Trim(),
                DriverLicense = txtDriverLicense.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };

            return true;
        }

        // THÊM KHÁCH HÀNG
        private void BtnAdd_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (!GetCustomerFromForm(out Customer customer))
                    return;

                bool result =
                    customerService.AddCustomer(customer);

                if (result)
                {
                    MessageBox.Show(
                        "Thêm khách hàng thành công!",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadCustomers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        "Không thể thêm khách hàng. Vui lòng kiểm tra lại thông tin!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm khách hàng.\n\n" +
                    ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // CẬP NHẬT KHÁCH HÀNG
        private void BtnUpdate_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (selectedCustomerID <= 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn khách hàng cần cập nhật!");

                    return;
                }

                if (!GetCustomerFromForm(out Customer customer))
                    return;

                bool result =
                    customerService.UpdateCustomer(customer);

                if (result)
                {
                    MessageBox.Show(
                        "Cập nhật thông tin khách hàng thành công!",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadCustomers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        "Không thể cập nhật khách hàng!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật khách hàng.\n\n" +
                    ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // XÓA KHÁCH HÀNG
        private void BtnDelete_Click(
            object sender,
            EventArgs e)
        {
            if (selectedCustomerID <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn khách hàng cần xóa!");

                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa khách hàng này?",
                "XÁC NHẬN XÓA",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                bool result =
                    customerService.DeleteCustomer(
                        selectedCustomerID);

                if (result)
                {
                    MessageBox.Show(
                        "Xóa khách hàng thành công!",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadCustomers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        "Không tìm thấy khách hàng cần xóa!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa khách hàng. Khách hàng có thể " +
                    "đang được sử dụng trong hợp đồng.\n\n" +
                    ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // LÀM MỚI FORM
        private void ClearForm()
        {
            selectedCustomerID = 0;

            txtFullName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtCitizenID.Clear();
            txtDriverLicense.Clear();
            txtAddress.Clear();

            dgvCustomers.ClearSelection();

            txtFullName.Focus();
        }
    }
}
