
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using CarRentalManagement.BLL.Services;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.Presentation
{
    public class FrmVehicleHandover : Form
    {
        private readonly User currentUser;
        private readonly VehicleHandoverService handoverService;

        private ComboBox cboContract;
        private ComboBox cboType;
        private NumericUpDown numOdometer;
        private ComboBox cboFuel;
        private TextBox txtCondition;
        private TextBox txtNotes;
        private DateTimePicker dtpDate;
        private DataGridView dgvHandovers;

        private Color bg = Color.FromArgb(17, 18, 20);
        private Color card = Color.FromArgb(28, 30, 33);
        private Color gold = Color.FromArgb(198, 166, 107);
        private Color ivory = Color.FromArgb(243, 239, 230);

        public FrmVehicleHandover(User user)
        {
            currentUser = user;
            handoverService = new VehicleHandoverService();

            InitializeInterface();
            LoadContracts();
            LoadHandovers();
        }

        private void InitializeInterface()
        {
            Text = "LINHXURY | Quản lý giao nhận xe";
            Size = new Size(1150, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = bg;
            ForeColor = ivory;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "QUẢN LÝ GIAO - NHẬN XE";
            title.Font = new Font("Segoe UI", 23, FontStyle.Bold);
            title.ForeColor = gold;
            title.Location = new Point(30, 20);
            title.AutoSize = true;
            Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = "Theo dõi quá trình bàn giao và tiếp nhận phương tiện";
            subtitle.ForeColor = Color.Gray;
            subtitle.Location = new Point(34, 65);
            subtitle.AutoSize = true;
            Controls.Add(subtitle);

            Panel formPanel = new Panel();
            formPanel.BackColor = card;
            formPanel.Location = new Point(30, 105);
            formPanel.Size = new Size(1070, 300);
            Controls.Add(formPanel);

            AddLabel(formPanel, "Hợp đồng", 25, 20);
            cboContract = AddCombo(formPanel, 25, 48, 300);

            AddLabel(formPanel, "Loại bàn giao", 370, 20);
            cboType = AddCombo(formPanel, 370, 48, 250);
            cboType.Items.Add("Giao xe");
            cboType.Items.Add("Nhận xe");
            cboType.SelectedIndex = 0;

            AddLabel(formPanel, "Ngày bàn giao", 680, 20);
            dtpDate = new DateTimePicker();
            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpDate.ShowUpDown = true;
            dtpDate.Location = new Point(680, 48);
            dtpDate.Size = new Size(330, 32);
            StyleControl(dtpDate);
            formPanel.Controls.Add(dtpDate);

            AddLabel(formPanel, "Số km hiện tại", 25, 105);
            numOdometer = new NumericUpDown();
            numOdometer.Location = new Point(25, 133);
            numOdometer.Size = new Size(300, 32);
            numOdometer.Maximum = 10000000;
            numOdometer.BackColor = bg;
            numOdometer.ForeColor = ivory;
            formPanel.Controls.Add(numOdometer);

            AddLabel(formPanel, "Mức nhiên liệu (%)", 370, 105);
            cboFuel = AddCombo(formPanel, 370, 133, 250);

            for (int i = 0; i <= 100; i += 10)
                cboFuel.Items.Add(i.ToString());

            cboFuel.SelectedItem = "100";

            AddLabel(formPanel, "Tình trạng xe", 680, 105);
            txtCondition = AddText(formPanel, 680, 133, 330);

            AddLabel(formPanel, "Ghi chú", 25, 190);
            txtNotes = AddText(formPanel, 25, 218, 985);

            Button btnSave = AddButton(
                formPanel, "XÁC NHẬN", 25, 255, 150);

            Button btnClear = AddButton(
                formPanel, "LÀM MỚI", 195, 255, 130);

            btnSave.Click += BtnSave_Click;
            btnClear.Click += (s, e) => ClearForm();

            Label historyTitle = new Label();
            historyTitle.Text = "LỊCH SỬ GIAO - NHẬN XE";
            historyTitle.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            historyTitle.ForeColor = gold;
            historyTitle.Location = new Point(30, 425);
            historyTitle.AutoSize = true;
            Controls.Add(historyTitle);

            dgvHandovers = new DataGridView();
            dgvHandovers.Location = new Point(30, 465);
            dgvHandovers.Size = new Size(1070, 220);
            dgvHandovers.BackgroundColor = card;
            dgvHandovers.ForeColor = Color.Black;
            dgvHandovers.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvHandovers.ReadOnly = true;
            dgvHandovers.AllowUserToAddRows = false;
            dgvHandovers.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvHandovers.RowHeadersVisible = false;
            dgvHandovers.AllowUserToResizeRows = false;

            dgvHandovers.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(45, 40, 32);
            dgvHandovers.ColumnHeadersDefaultCellStyle.ForeColor = gold;
            dgvHandovers.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvHandovers.EnableHeadersVisualStyles = false;

            dgvHandovers.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(80, 65, 40);
            dgvHandovers.DefaultCellStyle.SelectionForeColor = Color.White;

            Controls.Add(dgvHandovers);
        }

        private void LoadContracts()
        {
            try
            {
                cboContract.Items.Clear();

                using (var conn =
                    CarRentalManagement.DAL.DbConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        SELECT c.ContractID,
                               c.ContractNumber,
                               cu.FullName,
                               ca.LicensePlate
                        FROM Contracts c
                        INNER JOIN Customers cu
                            ON c.CustomerID = cu.CustomerID
                        INNER JOIN Cars ca
                            ON c.CarID = ca.CarID
                        WHERE c.Status IN ('Draft', 'Active')
                        ORDER BY c.ContractID DESC";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cboContract.Items.Add(new ContractItem
                            {
                                ID = Convert.ToInt32(reader["ContractID"]),
                                Display = reader["ContractNumber"] + " - "
                                    + reader["FullName"] + " - "
                                    + reader["LicensePlate"]
                            });
                        }
                    }
                }

                cboContract.DisplayMember = "Display";
                cboContract.ValueMember = "ID";

                if (cboContract.Items.Count > 0)
                    cboContract.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không tải được danh sách hợp đồng: " + ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadHandovers()
        {
            try
            {
                dgvHandovers.DataSource =
                    handoverService.GetAllHandovers();

                if (dgvHandovers.Columns["HandoverID"] != null)
                    dgvHandovers.Columns["HandoverID"].Visible = false;

                if (dgvHandovers.Columns["ContractID"] != null)
                    dgvHandovers.Columns["ContractID"].Visible = false;

                if (dgvHandovers.Columns["UserID"] != null)
                    dgvHandovers.Columns["UserID"].Visible = false;

                // Việt hóa tên các cột trong lịch sử
                SetColumnHeader("ContractNumber", "Mã hợp đồng");
                SetColumnHeader("CustomerName", "Khách hàng");
                SetColumnHeader("LicensePlate", "Biển số xe");
                SetColumnHeader("HandoverType", "Loại bàn giao");
                SetColumnHeader("HandoverDate", "Ngày bàn giao");
                SetColumnHeader("Odometer", "Số km");
                SetColumnHeader("FuelLevel", "Nhiên liệu (%)");
                SetColumnHeader("VehicleCondition", "Tình trạng xe");
                SetColumnHeader("Notes", "Ghi chú");
                SetColumnHeader("EmployeeName", "Nhân viên thực hiện");

                if (dgvHandovers.Columns["HandoverDate"] != null)
                {
                    dgvHandovers.Columns["HandoverDate"]
                        .DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }

                if (dgvHandovers.Columns["Odometer"] != null)
                {
                    dgvHandovers.Columns["Odometer"]
                        .DefaultCellStyle.Format = "N0";
                }

                dgvHandovers.AutoResizeColumns(
                    DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải lịch sử giao nhận xe:\n" + ex.Message,
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SetColumnHeader(string columnName, string headerText)
        {
            if (dgvHandovers.Columns[columnName] != null)
            {
                dgvHandovers.Columns[columnName].HeaderText = headerText;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboContract.SelectedItem == null)
                    throw new Exception("Vui lòng chọn hợp đồng!");

                if (cboFuel.SelectedItem == null)
                    throw new Exception("Vui lòng chọn mức nhiên liệu!");

                if (string.IsNullOrWhiteSpace(txtCondition.Text))
                    throw new Exception("Vui lòng nhập tình trạng xe!");

                ContractItem selected =
                    cboContract.SelectedItem as ContractItem;

                VehicleHandover handover = new VehicleHandover();

                handover.ContractID = selected.ID;

                handover.HandoverType =
                    cboType.SelectedItem.ToString() == "Giao xe"
                        ? "Giao xe" : "Nhận xe";

                handover.HandoverDate = dtpDate.Value;
                handover.Odometer = (int)numOdometer.Value;
                handover.FuelLevel = cboFuel.SelectedItem.ToString();
                handover.VehicleCondition = txtCondition.Text.Trim();
                handover.Notes = txtNotes.Text.Trim();
                handover.UserID = currentUser.UserID;

                handoverService.RegisterHandover(handover);

                MessageBox.Show(
                    "Ghi nhận giao nhận xe thành công!",
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearForm();
                LoadContracts();
                LoadHandovers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void ClearForm()
        {
            if (cboType.Items.Count > 0)
                cboType.SelectedIndex = 0;

            numOdometer.Value = 0;
            cboFuel.SelectedItem = "100";
            txtCondition.Clear();
            txtNotes.Clear();
            dtpDate.Value = DateTime.Now;
        }

        private void AddLabel(
            Control parent, string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.ForeColor = gold;
            label.Location = new Point(x, y);
            label.AutoSize = true;
            parent.Controls.Add(label);
        }

        private ComboBox AddCombo(
            Control parent, int x, int y, int width)
        {
            ComboBox combo = new ComboBox();
            combo.Location = new Point(x, y);
            combo.Size = new Size(width, 32);
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.BackColor = bg;
            combo.ForeColor = ivory;
            parent.Controls.Add(combo);
            return combo;
        }

        private TextBox AddText(
            Control parent, int x, int y, int width)
        {
            TextBox box = new TextBox();
            box.Location = new Point(x, y);
            box.Size = new Size(width, 30);
            StyleControl(box);
            parent.Controls.Add(box);
            return box;
        }

        private void StyleControl(Control control)
        {
            control.BackColor = bg;
            control.ForeColor = ivory;
        }

        private Button AddButton(
            Control parent, string text,
            int x, int y, int width)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(width, 32);
            button.BackColor = gold;
            button.ForeColor = bg;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            parent.Controls.Add(button);
            return button;
        }

        private class ContractItem
        {
            public int ID { get; set; }
            public string Display { get; set; }
        }
    }
}
