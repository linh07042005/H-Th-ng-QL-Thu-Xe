
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CarRentalManagement.BLL.Services;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.Presentation
{
    public class FrmContracts : Form
    {
        private readonly ContractService contractService;
        private readonly CustomerService customerService;
        private readonly CarService carService;
        private readonly User currentUser;

        private List<Contract> contracts = new List<Contract>();

        private int selectedContractID = 0;
        private int pendingSelectedCarID = 0;

        private TextBox txtContractNumber;
        private ComboBox cboCustomer;
        private ComboBox cboCar;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpReturn;
        private NumericUpDown numDailyRate;
        private NumericUpDown numDeposit;
        private ComboBox cboStatus;
        private TextBox txtNotes;
        private TextBox txtSearch;
        private DataGridView dgvContracts;
        private Label lblUser;

        private readonly Color black = Color.FromArgb(17, 18, 20);
        private readonly Color panelColor = Color.FromArgb(28, 30, 33);
        private readonly Color gold = Color.FromArgb(198, 166, 107);
        private readonly Color ivory = Color.FromArgb(243, 239, 230);
        private readonly Color gray = Color.FromArgb(133, 133, 133);

        public FrmContracts(User loggedInUser)
            : this(loggedInUser, 0)
        {
        }

        public FrmContracts(User loggedInUser, int selectedCarID)
        {
            if (loggedInUser == null)
                throw new ArgumentNullException("loggedInUser");

            currentUser = loggedInUser;

            contractService = new ContractService();
            customerService = new CustomerService();
            carService = new CarService();

            InitializeInterface();
            LoadComboBoxes();
            LoadContracts();

            pendingSelectedCarID = selectedCarID;

            // Chỉ chọn xe sau khi cửa sổ hợp đồng đã hiển thị.
            // Tránh việc dữ liệu hợp đồng cũ ghi đè xe vừa chọn.
            if (pendingSelectedCarID > 0)
            {
                Shown += (s, e) =>
                {
                    ClearForm();
                    SelectCarForContract(pendingSelectedCarID);
                };
            }
        }

        private void InitializeInterface()
        {
            Text = "LINHXURY | Quản lý hợp đồng";
            Size = new Size(1250, 820);
            MinimumSize = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = black;
            ForeColor = ivory;
            Font = new Font("Segoe UI", 9F);

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = black,
                Padding = new Padding(25, 15, 25, 10)
            };

            Label title = new Label
            {
                Text = "QUẢN LÝ HỢP ĐỒNG",
                ForeColor = gold,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 12)
            };

            Label subtitle = new Label
            {
                Text = "Quản lý hợp đồng thuê xe và thông tin khách hàng",
                ForeColor = gray,
                AutoSize = true,
                Location = new Point(28, 52)
            };

            lblUser = new Label
            {
                Text = "ĐANG ĐĂNG NHẬP: " + currentUser.FullName,
                ForeColor = ivory,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(900, 35)
            };

            header.Controls.Add(title);
            header.Controls.Add(subtitle);
            header.Controls.Add(lblUser);

            Panel content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = black,
                Padding = new Padding(22, 5, 22, 18)
            };

            Panel formPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 300,
                BackColor = panelColor,
                Padding = new Padding(18)
            };

            Label formTitle = new Label
            {
                Text = "THÔNG TIN THUÊ XE",
                ForeColor = gold,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(18, 12),
                AutoSize = true
            };

            formPanel.Controls.Add(formTitle);

            AddFieldLabel(formPanel, "Mã hợp đồng", 20, 55);
            txtContractNumber = CreateTextBox(20, 78, 250);
            formPanel.Controls.Add(txtContractNumber);

            AddFieldLabel(formPanel, "Khách hàng", 300, 55);
            cboCustomer = CreateComboBox(300, 78, 250);
            formPanel.Controls.Add(cboCustomer);

            AddFieldLabel(formPanel, "Xe cho thuê", 580, 55);
            cboCar = CreateComboBox(580, 78, 250);
            formPanel.Controls.Add(cboCar);

            AddFieldLabel(formPanel, "Ngày nhận xe", 20, 125);
            dtpStart = CreateDatePicker(20, 148, 250);
            formPanel.Controls.Add(dtpStart);

            AddFieldLabel(formPanel, "Ngày trả dự kiến", 300, 125);
            dtpReturn = CreateDatePicker(300, 148, 250);
            dtpReturn.Value = DateTime.Today.AddDays(1);
            formPanel.Controls.Add(dtpReturn);

            AddFieldLabel(formPanel, "Giá thuê/ngày (VNĐ)", 580, 125);
            numDailyRate = CreateMoneyInput(580, 148, 250);
            formPanel.Controls.Add(numDailyRate);

            AddFieldLabel(formPanel, "Tiền đặt cọc (VNĐ)", 20, 195);
            numDeposit = CreateMoneyInput(20, 218, 250);
            formPanel.Controls.Add(numDeposit);

            AddFieldLabel(formPanel, "Trạng thái hợp đồng", 300, 195);
            cboStatus = CreateComboBox(300, 218, 250);

            // Giá trị lưu database vẫn là tiếng Anh.
            // Người dùng nhìn thấy nội dung tiếng Việt.
            cboStatus.DataSource = new List<ContractStatusOption>
            {
                new ContractStatusOption
                {
                    Value = "Draft",
                    DisplayName = "Nháp"
                },
                new ContractStatusOption
                {
                    Value = "Active",
                    DisplayName = "Đang hiệu lực"
                },
                new ContractStatusOption
                {
                    Value = "Completed",
                    DisplayName = "Đã hoàn thành"
                },
                new ContractStatusOption
                {
                    Value = "Cancelled",
                    DisplayName = "Đã hủy"
                }
            };

            cboStatus.DisplayMember = "DisplayName";
            cboStatus.ValueMember = "Value";
            cboStatus.SelectedValue = "Draft";

            formPanel.Controls.Add(cboStatus);

            AddFieldLabel(formPanel, "Ghi chú", 580, 195);

            txtNotes = new TextBox
            {
                Location = new Point(580, 218),
                Size = new Size(500, 27),
                BackColor = black,
                ForeColor = ivory,
                BorderStyle = BorderStyle.FixedSingle
            };

            formPanel.Controls.Add(txtNotes);

            // BUTTON PANEL

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = black,
                Padding = new Padding(0, 10, 0, 5)
            };

            Button btnAdd = CreateButton(
                "＋  TẠO HỢP ĐỒNG", gold, black, 0);

            Button btnUpdate = CreateButton(
                "✎  CẬP NHẬT", panelColor, gold, 145);

            Button btnDelete = CreateButton(
                "XÓA", panelColor, Color.IndianRed, 290);

            Button btnClear = CreateButton(
                "LÀM MỚI", panelColor, ivory, 435);

            btnAdd.Click += (s, e) => AddContract();
            btnUpdate.Click += (s, e) => UpdateContract();
            btnDelete.Click += (s, e) => DeleteContract();
            btnClear.Click += (s, e) => ClearForm();

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnUpdate);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnClear);

            // SEARCH PANEL

            Panel searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = black,
                Padding = new Padding(0, 8, 0, 5)
            };

            Label searchLabel = new Label
            {
                Text = "TÌM KIẾM HỢP ĐỒNG",
                ForeColor = gold,
                AutoSize = true,
                Location = new Point(0, 17)
            };

            txtSearch = new TextBox
            {
                Location = new Point(190, 12),
                Size = new Size(350, 27),
                BackColor = panelColor,
                ForeColor = ivory,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtSearch.TextChanged += (s, e) => FilterContracts();

            searchPanel.Controls.Add(searchLabel);
            searchPanel.Controls.Add(txtSearch);

            // CONTRACT DATAGRIDVIEW

            dgvContracts = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = panelColor,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(55, 55, 55),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                AutoGenerateColumns = false
            };

            dgvContracts.ColumnHeadersDefaultCellStyle.BackColor = black;
            dgvContracts.ColumnHeadersDefaultCellStyle.ForeColor = gold;
            dgvContracts.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvContracts.ColumnHeadersHeight = 38;

            dgvContracts.DefaultCellStyle.BackColor = panelColor;
            dgvContracts.DefaultCellStyle.ForeColor = ivory;
            dgvContracts.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(75, 65, 45);

            dgvContracts.DefaultCellStyle.SelectionForeColor = ivory;
            dgvContracts.DefaultCellStyle.Padding = new Padding(5);
            dgvContracts.RowTemplate.Height = 34;

            AddGridColumn("ContractNumber", "Mã hợp đồng", 110);
            AddGridColumn("CustomerName", "Khách hàng", 140);
            AddGridColumn("LicensePlate", "Biển số xe", 100);
            AddGridColumn("StartDate", "Ngày nhận xe", 100);
            AddGridColumn("ExpectedReturnDate", "Ngày trả dự kiến", 110);
            AddGridColumn("DailyRate", "Giá thuê/ngày", 110);
            AddGridColumn("Deposit", "Tiền đặt cọc", 100);
            AddGridColumn("Status", "Trạng thái", 100);

            dgvContracts.SelectionChanged += DgvContracts_SelectionChanged;

            content.Controls.Add(dgvContracts);
            content.Controls.Add(searchPanel);
            content.Controls.Add(buttonPanel);
            content.Controls.Add(formPanel);

            Controls.Add(content);
            Controls.Add(header);
        }

        private void AddFieldLabel(
            Panel panel,
            string text,
            int x,
            int y)
        {
            panel.Controls.Add(new Label
            {
                Text = text,
                ForeColor = ivory,
                Location = new Point(x, y),
                AutoSize = true
            });
        }

        private TextBox CreateTextBox(int x, int y, int width)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 27),
                BackColor = black,
                ForeColor = ivory,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private ComboBox CreateComboBox(int x, int y, int width)
        {
            return new ComboBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 28),
                BackColor = black,
                ForeColor = ivory,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
        }

        private DateTimePicker CreateDatePicker(
            int x,
            int y,
            int width)
        {
            return new DateTimePicker
            {
                Location = new Point(x, y),
                Size = new Size(width, 27),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                CalendarMonthBackground = panelColor
            };
        }

        private NumericUpDown CreateMoneyInput(
            int x,
            int y,
            int width)
        {
            return new NumericUpDown
            {
                Location = new Point(x, y),
                Size = new Size(width, 27),
                Maximum = 10000000000M,
                Minimum = 0,
                Increment = 100000,
                ThousandsSeparator = true,
                DecimalPlaces = 0,
                BackColor = black,
                ForeColor = ivory
            };
        }

        private Button CreateButton(
            string text,
            Color back,
            Color fore,
            int x)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, 5),
                Size = new Size(130, 38),
                BackColor = back,
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderColor = gold;
            button.FlatAppearance.BorderSize = 1;

            return button;
        }

        private void AddGridColumn(
            string property,
            string header,
            int minWidth)
        {
            dgvContracts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = property,
                HeaderText = header,
                Name = property,
                MinimumWidth = minWidth,
                SortMode = DataGridViewColumnSortMode.Automatic
            });
        }

        // LOAD CUSTOMER AND CAR COMBOBOXES

        private void LoadComboBoxes()
        {
            try
            {
                cboCustomer.DataSource =
                    customerService.GetAllCustomers();

                cboCustomer.DisplayMember = "FullName";
                cboCustomer.ValueMember = "CustomerID";

                cboCar.DataSource =
                    carService.GetAllCars();

                cboCar.DisplayMember = "LicensePlate";
                cboCar.ValueMember = "CarID";

                cboCustomer.SelectedIndex = -1;
                cboCar.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu khách hàng hoặc xe.\n" +
                    ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // SELECT CAR FROM VEHICLE CATALOG

        private void SelectCarForContract(int carID)
        {
            try
            {
                List<Car> cars = carService.GetAllCars();

                Car selectedCar = cars
                    .FirstOrDefault(c => c.CarID == carID);

                if (selectedCar == null)
                {
                    MessageBox.Show(
                        "Không tìm thấy xe được chọn.",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (selectedCar.Status != "Available")
                {
                    MessageBox.Show(
                        "Xe này hiện không còn sẵn sàng để cho thuê.",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // Tự động chọn biển số xe.
                cboCar.SelectedValue = selectedCar.CarID;

                // Tự động điền giá thuê theo xe.
                numDailyRate.Value = Math.Min(
                    numDailyRate.Maximum,
                    Math.Max(
                        numDailyRate.Minimum,
                        selectedCar.DailyRate
                    )
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể chọn xe cho hợp đồng.\n" +
                    ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // LOAD CONTRACTS

        private void LoadContracts()
        {
            try
            {
                contracts = contractService.GetAllContracts();
                FilterContracts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách hợp đồng.\n" +
                    ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // FILTER CONTRACTS

        private void FilterContracts()
        {
            string keyword = txtSearch.Text.Trim().ToLowerInvariant();

            var rows = contracts
                .Where(c =>
                    (c.ContractNumber ?? "")
                    .ToLowerInvariant()
                    .Contains(keyword))
                .Select(c => new ContractGridRow
                {
                    ContractID = c.ContractID,
                    ContractNumber = c.ContractNumber,
                    CustomerName = GetCustomerName(c.CustomerID),
                    LicensePlate = GetLicensePlate(c.CarID),
                    StartDate = c.StartDate.ToString("dd/MM/yyyy"),
                    ExpectedReturnDate =
                        c.ExpectedReturnDate.ToString("dd/MM/yyyy"),
                    DailyRate = c.DailyRate.ToString("N0"),
                    Deposit = c.Deposit.ToString("N0"),
                    Status = GetVietnameseContractStatus(c.Status)
                })
                .ToList();

            dgvContracts.DataSource = rows;
        }

        private string GetCustomerName(int customerID)
        {
            foreach (Customer customer in customerService.GetAllCustomers())
            {
                if (customer.CustomerID == customerID)
                    return customer.FullName;
            }

            return "Không xác định";
        }

        private string GetLicensePlate(int carID)
        {
            foreach (Car car in carService.GetAllCars())
            {
                if (car.CarID == carID)
                    return car.LicensePlate;
            }

            return "Không xác định";
        }

        private string GetVietnameseContractStatus(string status)
        {
            switch (status)
            {
                case "Draft":
                    return "Nháp";

                case "Active":
                    return "Đang hiệu lực";

                case "Completed":
                    return "Đã hoàn thành";

                case "Cancelled":
                    return "Đã hủy";

                default:
                    return status;
            }
        }

        // ADD CONTRACT

        private void AddContract()
        {
            try
            {
                Contract contract = ReadForm();

                if (contracts.Any(c =>
                    (c.ContractNumber ?? "").Equals(
                        contract.ContractNumber,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show(
                        "Mã hợp đồng đã tồn tại.",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (contract.Status == "Active" &&
                    !IsCarAvailableForContract(contract))
                {
                    MessageBox.Show(
                        "Xe không khả dụng trong khoảng thời gian thuê đã chọn.",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                contractService.AddContract(contract);

                MessageBox.Show(
                    "Tạo hợp đồng thành công!",
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadContracts();
                LoadComboBoxes();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thể tạo hợp đồng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // UPDATE CONTRACT

        private void UpdateContract()
        {
            try
            {
                if (selectedContractID == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn hợp đồng cần cập nhật.");

                    return;
                }

                Contract contract = ReadForm();
                contract.ContractID = selectedContractID;

                if (contract.Status == "Active" &&
                    !IsCarAvailableForContract(contract))
                {
                    MessageBox.Show(
                        "Xe không khả dụng trong khoảng thời gian thuê đã chọn.",
                        "LINHXURY",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                contractService.UpdateContract(contract);

                MessageBox.Show(
                    "Cập nhật hợp đồng thành công!",
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadContracts();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thể cập nhật hợp đồng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // CHECK CAR AVAILABILITY

        private bool IsCarAvailableForContract(Contract contract)
        {
            Car car = carService.GetAllCars()
                .FirstOrDefault(c => c.CarID == contract.CarID);

            if (car == null || car.Status != "Available")
                return false;

            return !contracts.Any(c =>
                c.CarID == contract.CarID &&
                c.ContractID != contract.ContractID &&
                c.Status == "Active" &&
                contract.StartDate <= c.ExpectedReturnDate &&
                contract.ExpectedReturnDate >= c.StartDate);
        }

        // READ FORM DATA

        private Contract ReadForm()
        {
            if (string.IsNullOrWhiteSpace(txtContractNumber.Text))
                throw new Exception("Vui lòng nhập mã hợp đồng.");

            if (cboCustomer.SelectedValue == null)
                throw new Exception("Vui lòng chọn khách hàng.");

            if (cboCar.SelectedValue == null)
                throw new Exception("Vui lòng chọn xe cho thuê.");

            if (cboStatus.SelectedValue == null)
                throw new Exception("Vui lòng chọn trạng thái hợp đồng.");

            Contract contract = new Contract
            {
                ContractNumber = txtContractNumber.Text.Trim(),

                CustomerID = Convert.ToInt32(
                    cboCustomer.SelectedValue),

                CarID = Convert.ToInt32(
                    cboCar.SelectedValue),

                UserID = currentUser.UserID,

                StartDate = dtpStart.Value.Date,

                ExpectedReturnDate = dtpReturn.Value.Date,

                DailyRate = numDailyRate.Value,

                Deposit = numDeposit.Value,

                // Lưu trạng thái tiếng Anh vào database.
                Status = cboStatus.SelectedValue.ToString(),

                Notes = txtNotes.Text.Trim()
            };

            if (contract.StartDate >= contract.ExpectedReturnDate)
                throw new Exception(
                    "Ngày trả xe phải sau ngày nhận xe.");

            if (contract.DailyRate <= 0)
                throw new Exception(
                    "Giá thuê mỗi ngày phải lớn hơn 0.");

            return contract;
        }

        // DELETE CONTRACT

        private void DeleteContract()
        {
            try
            {
                if (selectedContractID == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn hợp đồng cần xóa.");

                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa hợp đồng này?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                contractService.DeleteContract(selectedContractID);

                MessageBox.Show(
                    "Xóa hợp đồng thành công!",
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadContracts();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thể xóa hợp đồng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // SELECT CONTRACT FROM GRID

        private void DgvContracts_SelectionChanged(
            object sender,
            EventArgs e)
        {
            // Không có dòng nào được chọn thì không ghi đè dữ liệu form.
            if (dgvContracts.SelectedRows.Count == 0)
                return;

            if (dgvContracts.CurrentRow == null ||
                dgvContracts.CurrentRow.DataBoundItem == null)
                return;

            ContractGridRow row =
                dgvContracts.CurrentRow.DataBoundItem as ContractGridRow;

            if (row == null)
                return;

            Contract contract = contracts
                .FirstOrDefault(c => c.ContractID == row.ContractID);

            if (contract == null)
                return;

            selectedContractID = contract.ContractID;

            txtContractNumber.Text = contract.ContractNumber;

            cboCustomer.SelectedValue = contract.CustomerID;
            cboCar.SelectedValue = contract.CarID;

            dtpStart.Value = contract.StartDate;
            dtpReturn.Value = contract.ExpectedReturnDate;

            numDailyRate.Value = Math.Min(
                numDailyRate.Maximum,
                Math.Max(
                    numDailyRate.Minimum,
                    contract.DailyRate
                )
            );

            numDeposit.Value = Math.Min(
                numDeposit.Maximum,
                Math.Max(
                    numDeposit.Minimum,
                    contract.Deposit
                )
            );

            cboStatus.SelectedValue = contract.Status;

            txtNotes.Text = contract.Notes ?? "";
        }

        // CLEAR FORM

        private void ClearForm()
        {
            selectedContractID = 0;

            txtContractNumber.Clear();

            cboCustomer.SelectedIndex = -1;
            cboCar.SelectedIndex = -1;

            dtpStart.Value = DateTime.Today;
            dtpReturn.Value = DateTime.Today.AddDays(1);

            numDailyRate.Value = 0;
            numDeposit.Value = 0;

            cboStatus.SelectedValue = "Draft";

            txtNotes.Clear();

            dgvContracts.ClearSelection();
        }

        // CONTRACT STATUS OPTION

        private class ContractStatusOption
        {
            public string Value { get; set; }

            public string DisplayName { get; set; }
        }

        // GRID ROW MODEL

        private class ContractGridRow
        {
            public int ContractID { get; set; }

            public string ContractNumber { get; set; }

            public string CustomerName { get; set; }

            public string LicensePlate { get; set; }

            public string StartDate { get; set; }

            public string ExpectedReturnDate { get; set; }

            public string DailyRate { get; set; }

            public string Deposit { get; set; }

            public string Status { get; set; }
        }
    }
}
