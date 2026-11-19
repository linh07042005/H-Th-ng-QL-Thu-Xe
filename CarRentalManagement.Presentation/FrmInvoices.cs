
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms;
using CarRentalManagement.BLL.Services;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.Presentation
{
    public class FrmInvoices : Form
    {
        private readonly User currentUser;
        private readonly InvoiceService invoiceService;

        private DataGridView dgvInvoices;
        private ComboBox cboContract;
        private NumericUpDown numExtraFee;
        private TextBox txtNotes;
        private Label lblContractInfo;

        private Color backgroundColor = Color.FromArgb(17, 18, 20);
        private Color panelColor = Color.FromArgb(28, 30, 33);
        private Color goldColor = Color.FromArgb(198, 166, 107);
        private Color textColor = Color.FromArgb(243, 239, 230);

        public FrmInvoices(User user)
        {
            currentUser = user;

            // KIỂM TRA QUYỀN TRUY CẬP
            if (currentUser == null ||
                currentUser.Role != "Admin")
            {
                MessageBox.Show(
                    "Bạn không có quyền truy cập chức năng quản lý hóa đơn!\n" +
                    "Chỉ Admin mới được sử dụng chức năng này.",
                    "Từ chối truy cập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                this.Close();
                return;
            }

            invoiceService = new InvoiceService();

            InitializeForm();
            LoadEligibleContracts();
            LoadInvoices();
        }

        private void InitializeForm()
        {
            this.Text = "LINHXURY - Quản lý hóa đơn";
            this.Size = new Size(1200, 760);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = backgroundColor;
            this.ForeColor = textColor;
            this.Font = new Font("Segoe UI", 10);
            this.MinimumSize = new Size(1000, 650);

            // TIÊU ĐỀ
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ HÓA ĐƠN",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = goldColor,
                Location = new Point(30, 20),
                Size = new Size(500, 45)
            };

            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Quản lý chi phí thuê xe và thanh toán",
                ForeColor = Color.Gray,
                Location = new Point(32, 65),
                Size = new Size(500, 25)
            };

            this.Controls.Add(lblSubtitle);

            // KHU VỰC TẠO HÓA ĐƠN
            Panel pnlCreate = new Panel
            {
                Location = new Point(30, 105),
                Size = new Size(1120, 190),
                BackColor = panelColor,
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Left |
                         AnchorStyles.Right
            };

            this.Controls.Add(pnlCreate);

            Label lblSection = new Label
            {
                Text = "TẠO HÓA ĐƠN MỚI",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = goldColor,
                Location = new Point(20, 15),
                Size = new Size(250, 30)
            };

            pnlCreate.Controls.Add(lblSection);

            // HỢP ĐỒNG
            AddLabel(
                pnlCreate,
                "Hợp đồng đã hoàn tất:",
                20,
                60
            );

            cboContract = new ComboBox
            {
                Location = new Point(190, 57),
                Size = new Size(500, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 42, 45),
                ForeColor = textColor,
                Font = new Font("Segoe UI", 10)
            };

            pnlCreate.Controls.Add(cboContract);

            // PHỤ PHÍ
            AddLabel(
                pnlCreate,
                "Phụ phí (VNĐ):",
                20,
                110
            );

            numExtraFee = new NumericUpDown
            {
                Location = new Point(190, 107),
                Size = new Size(200, 32),
                Maximum = 1000000000,
                Minimum = 0,
                DecimalPlaces = 0,
                Increment = 10000,
                ThousandsSeparator = true,
                BackColor = Color.FromArgb(40, 42, 45),
                ForeColor = textColor,
                TextAlign = HorizontalAlignment.Left
            };

            pnlCreate.Controls.Add(numExtraFee);

            // LABEL GHI CHÚ
            // SỬA LỖI: KHÔNG ĐỂ LABEL ĐÈ LÊN TEXTBOX
            Label lblNotes = new Label
            {
                Text = "Ghi chú:",
                Location = new Point(430, 110),
                Size = new Size(75, 30),
                ForeColor = textColor,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10)
            };

            pnlCreate.Controls.Add(lblNotes);

            // Ô GHI CHÚ
            txtNotes = new TextBox
            {
                Location = new Point(510, 107),
                Size = new Size(350, 32),
                BackColor = Color.FromArgb(40, 42, 45),
                ForeColor = textColor,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10),
                Multiline = false,
                Margin = new Padding(0),
                AutoSize = false
            };

            pnlCreate.Controls.Add(txtNotes);

            // NÚT TẠO HÓA ĐƠN
            Button btnCreate = CreateButton(
                "TẠO HÓA ĐƠN",
                880,
                55,
                200,
                45
            );

            btnCreate.Click += BtnCreate_Click;
            pnlCreate.Controls.Add(btnCreate);

            // NÚT LÀM MỚI HỢP ĐỒNG
            Button btnRefreshContracts = CreateButton(
                "LÀM MỚI HỢP ĐỒNG",
                880,
                110,
                200,
                40
            );

            btnRefreshContracts.Click +=
                (s, e) => LoadEligibleContracts();

            pnlCreate.Controls.Add(btnRefreshContracts);

            // THÔNG TIN HỢP ĐỒNG
            lblContractInfo = new Label
            {
                Location = new Point(20, 155),
                Size = new Size(850, 25),
                ForeColor = Color.LightGray,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9)
            };

            pnlCreate.Controls.Add(lblContractInfo);

            // HIỂN THỊ THÔNG TIN KHI CHỌN HỢP ĐỒNG
            cboContract.SelectedIndexChanged += (s, e) =>
            {
                if (cboContract.SelectedItem is ContractInvoiceItem item)
                {
                    lblContractInfo.Text =
                        "Khách hàng: " + item.CustomerName +
                        " | Biển số: " + item.LicensePlate +
                        " | Số ngày thuê: " + item.RentalDays +
                        " | Tiền thuê: " +
                        item.RentalFee.ToString("N0") + " VNĐ";
                }
                else
                {
                    lblContractInfo.Text = "";
                }
            };

            // TIÊU ĐỀ DANH SÁCH
            Label lblList = new Label
            {
                Text = "DANH SÁCH HÓA ĐƠN",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = goldColor,
                Location = new Point(30, 315),
                Size = new Size(350, 30)
            };

            this.Controls.Add(lblList);

            // DATAGRIDVIEW
            dgvInvoices = new DataGridView
            {
                Location = new Point(30, 355),
                Size = new Size(1120, 270),
                BackgroundColor = panelColor,
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right
            };

            dgvInvoices.EnableHeadersVisualStyles = false;

            dgvInvoices.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(40, 42, 45);

            dgvInvoices.ColumnHeadersDefaultCellStyle.ForeColor =
                goldColor;

            dgvInvoices.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvInvoices.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(95, 79, 52);

            dgvInvoices.DefaultCellStyle.SelectionForeColor =
                Color.White;

            dgvInvoices.RowTemplate.Height = 35;

            dgvInvoices.Columns.Add("InvoiceID", "Mã");
            dgvInvoices.Columns.Add("InvoiceNumber", "Số hóa đơn");
            dgvInvoices.Columns.Add("ContractNumber", "Hợp đồng");
            dgvInvoices.Columns.Add("CustomerName", "Khách hàng");
            dgvInvoices.Columns.Add("LicensePlate", "Biển số xe");
            dgvInvoices.Columns.Add("InvoiceDate", "Ngày lập");
            dgvInvoices.Columns.Add("RentalFee", "Tiền thuê");
            dgvInvoices.Columns.Add("ExtraFee", "Phụ phí");
            dgvInvoices.Columns.Add("DepositDeduction", "Khấu trừ cọc");
            dgvInvoices.Columns.Add("TotalAmount", "Tổng tiền");
            dgvInvoices.Columns.Add("PaymentStatus", "Thanh toán");
            dgvInvoices.Columns.Add("Notes", "Ghi chú");

            this.Controls.Add(dgvInvoices);

            // NÚT ĐÁNH DẤU ĐÃ THANH TOÁN
            Button btnPaid = CreateButton(
                "ĐÁNH DẤU ĐÃ THANH TOÁN",
                30,
                650,
                250,
                42
            );

            btnPaid.Anchor =
                AnchorStyles.Bottom | AnchorStyles.Left;

            btnPaid.Click += BtnPaid_Click;
            this.Controls.Add(btnPaid);

            // NÚT CHƯA THANH TOÁN
            Button btnUnpaid = CreateButton(
                "CHƯA THANH TOÁN",
                300,
                650,
                200,
                42
            );

            btnUnpaid.Anchor =
                AnchorStyles.Bottom | AnchorStyles.Left;

            btnUnpaid.Click += BtnUnpaid_Click;
            this.Controls.Add(btnUnpaid);

            // NÚT LÀM MỚI
            Button btnReload = CreateButton(
                "LÀM MỚI DANH SÁCH",
                520,
                650,
                220,
                42
            );

            btnReload.Anchor =
                AnchorStyles.Bottom | AnchorStyles.Left;

            btnReload.Click += (s, e) => LoadInvoices();
            this.Controls.Add(btnReload);

            // NÚT XÓA HÓA ĐƠN
            Button btnDelete = CreateButton(
                "XÓA HÓA ĐƠN",
                760,
                650,
                180,
                42
            );

            btnDelete.Anchor =
                AnchorStyles.Bottom | AnchorStyles.Left;

            btnDelete.BackColor =
                Color.FromArgb(160, 55, 55);

            btnDelete.ForeColor = Color.White;
            btnDelete.Click += BtnDelete_Click;

            // CHỈ ADMIN ĐƯỢC SỬ DỤNG NÚT XÓA
            btnDelete.Enabled =
                currentUser != null &&
                currentUser.Role == "Admin";

            this.Controls.Add(btnDelete);
        }

        // THÊM LABEL
        private void AddLabel(
            Control parent,
            string text,
            int x,
            int y)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(165, 30),
                ForeColor = textColor,
                BackColor = Color.Transparent
            };

            parent.Controls.Add(label);
        }

        // TẠO BUTTON
        private Button CreateButton(
            string text,
            int x,
            int y,
            int width,
            int height)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = goldColor,
                ForeColor = Color.FromArgb(17, 18, 20),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;

            return button;
        }

        // LẤY CÁC HỢP ĐỒNG ĐÃ NHẬN XE
        // VÀ CHƯA CÓ HÓA ĐƠN
        private void LoadEligibleContracts()
        {
            cboContract.Items.Clear();
            lblContractInfo.Text = "";

            try
            {
                using (SqlConnection conn =
                    CarRentalManagement.DAL.DbConnection.GetConnection())
                {
                    string query = @"
                        SELECT
                            c.ContractID,
                            c.ContractNumber,
                            cu.FullName AS CustomerName,
                            ca.LicensePlate,
                            c.StartDate,
                            h.HandoverDate AS ActualReturnDate,
                            c.DailyRate,
                            c.Deposit
                        FROM Contracts c
                        INNER JOIN Customers cu
                            ON c.CustomerID = cu.CustomerID
                        INNER JOIN Cars ca
                            ON c.CarID = ca.CarID
                        INNER JOIN VehicleHandover h
                            ON c.ContractID = h.ContractID
                        WHERE c.Status = N'Completed'
                          AND h.HandoverType = N'Nhận xe'
                          AND NOT EXISTS
                          (
                              SELECT 1
                              FROM Invoices i
                              WHERE i.ContractID = c.ContractID
                          )
                        ORDER BY c.ContractID DESC";

                    using (SqlCommand cmd =
                        new SqlCommand(query, conn))
                    {
                        conn.Open();

                        using (SqlDataReader reader =
                            cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime startDate =
                                    Convert.ToDateTime(
                                        reader["StartDate"]);

                                DateTime returnDate =
                                    Convert.ToDateTime(
                                        reader["ActualReturnDate"]);

                                int rentalDays = Math.Max(
                                    1,
                                    (returnDate.Date -
                                     startDate.Date).Days
                                );

                                decimal dailyRate =
                                    Convert.ToDecimal(
                                        reader["DailyRate"]);

                                ContractInvoiceItem item =
                                    new ContractInvoiceItem
                                    {
                                        ContractID =
                                            Convert.ToInt32(
                                                reader["ContractID"]),

                                        ContractNumber =
                                            reader["ContractNumber"].ToString(),

                                        CustomerName =
                                            reader["CustomerName"].ToString(),

                                        LicensePlate =
                                            reader["LicensePlate"].ToString(),

                                        RentalDays = rentalDays,
                                        DailyRate = dailyRate,

                                        Deposit =
                                            Convert.ToDecimal(
                                                reader["Deposit"]),

                                        RentalFee =
                                            rentalDays * dailyRate
                                    };

                                cboContract.Items.Add(item);
                            }
                        }
                    }
                }

                if (cboContract.Items.Count > 0)
                    cboContract.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải hợp đồng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // TẢI DANH SÁCH HÓA ĐƠN
        private void LoadInvoices()
        {
            try
            {
                List<Invoice> invoices =
                    invoiceService.GetAllInvoices();

                dgvInvoices.Rows.Clear();

                foreach (Invoice invoice in invoices)
                {
                    int rowIndex = dgvInvoices.Rows.Add(
                        invoice.InvoiceID,
                        invoice.InvoiceNumber,
                        invoice.ContractNumber,
                        invoice.CustomerName,
                        invoice.LicensePlate,
                        invoice.InvoiceDate.ToString("dd/MM/yyyy"),
                        invoice.RentalFee.ToString("N0"),
                        invoice.ExtraFee.ToString("N0"),
                        invoice.DepositDeduction.ToString("N0"),
                        invoice.TotalAmount.HasValue
                            ? invoice.TotalAmount.Value.ToString("N0")
                            : "0",
                        invoice.PaymentStatus,
                        string.IsNullOrWhiteSpace(invoice.Notes)
                            ? "Không có ghi chú"
                            : invoice.Notes
                    );

                    if (invoice.PaymentStatus == "Đã thanh toán")
                    {
                        dgvInvoices.Rows[rowIndex]
                            .Cells["PaymentStatus"]
                            .Style.ForeColor = Color.LightGreen;
                    }
                    else
                    {
                        dgvInvoices.Rows[rowIndex]
                            .Cells["PaymentStatus"]
                            .Style.ForeColor = Color.Orange;
                    }
                }

                dgvInvoices.Columns["Notes"].MinimumWidth = 150;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách hóa đơn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // TẠO HÓA ĐƠN
        private void BtnCreate_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                ContractInvoiceItem item =
                    cboContract.SelectedItem as ContractInvoiceItem;

                if (item == null)
                {
                    MessageBox.Show(
                        "Hiện không có hợp đồng nào đủ điều kiện tạo hóa đơn!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                decimal extraFee = numExtraFee.Value;

                decimal depositDeduction = Math.Min(
                    item.Deposit,
                    item.RentalFee + extraFee
                );

                Invoice invoice = new Invoice
                {
                    InvoiceNumber = "INV" +
                        DateTime.Now.ToString("yyyyMMddHHmmssfff"),

                    ContractID = item.ContractID,
                    InvoiceDate = DateTime.Now,
                    RentalFee = item.RentalFee,
                    ExtraFee = extraFee,
                    DepositDeduction = depositDeduction,
                    PaymentStatus = "Chưa thanh toán",
                    Notes = txtNotes.Text.Trim()
                };

                invoiceService.AddInvoice(invoice);

                MessageBox.Show(
                    "Tạo hóa đơn thành công!\n" +
                    "Số hóa đơn: " + invoice.InvoiceNumber +
                    "\nTổng tiền: " +
                    invoice.TotalAmount.Value.ToString("N0") +
                    " VNĐ",
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                numExtraFee.Value = 0;
                txtNotes.Clear();

                LoadInvoices();
                LoadEligibleContracts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tạo hóa đơn:\n\n" + ex.ToString(),
                    "Chi tiết lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ĐÁNH DẤU ĐÃ THANH TOÁN
        private void BtnPaid_Click(
            object sender,
            EventArgs e)
        {
            UpdateSelectedPaymentStatus("Đã thanh toán");
        }

        // ĐÁNH DẤU CHƯA THANH TOÁN
        private void BtnUnpaid_Click(
            object sender,
            EventArgs e)
        {
            UpdateSelectedPaymentStatus("Chưa thanh toán");
        }

        // CẬP NHẬT TRẠNG THÁI THANH TOÁN
        private void UpdateSelectedPaymentStatus(string status)
        {
            if (dgvInvoices.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn!");
                return;
            }

            try
            {
                int invoiceID = Convert.ToInt32(
                    dgvInvoices.CurrentRow
                        .Cells["InvoiceID"].Value);

                invoiceService.UpdatePaymentStatus(
                    invoiceID,
                    status);

                MessageBox.Show(
                    "Cập nhật trạng thái thanh toán thành công!",
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadInvoices();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật hóa đơn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // XÓA HÓA ĐƠN
        private void BtnDelete_Click(
            object sender,
            EventArgs e)
        {
            // KIỂM TRA QUYỀN ADMIN
            if (currentUser == null ||
                currentUser.Role != "Admin")
            {
                MessageBox.Show(
                    "Chỉ Admin mới có quyền xóa hóa đơn!",
                    "Từ chối truy cập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // KIỂM TRA ĐÃ CHỌN HÓA ĐƠN CHƯA
            if (dgvInvoices.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn hóa đơn cần xóa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            int invoiceID = Convert.ToInt32(
                dgvInvoices.CurrentRow
                    .Cells["InvoiceID"].Value);

            string invoiceNumber =
                dgvInvoices.CurrentRow
                    .Cells["InvoiceNumber"].Value.ToString();

            string paymentStatus =
                dgvInvoices.CurrentRow
                    .Cells["PaymentStatus"].Value.ToString();

            // KHÔNG CHO XÓA HÓA ĐƠN ĐÃ THANH TOÁN
            if (paymentStatus == "Đã thanh toán")
            {
                MessageBox.Show(
                    "Không thể xóa hóa đơn đã thanh toán!",
                    "Không thể xóa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // XÁC NHẬN TRƯỚC KHI XÓA
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa hóa đơn " +
                invoiceNumber + " không?\n\n" +
                "Hành động này không thể hoàn tác!",
                "Xác nhận xóa hóa đơn",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                invoiceService.DeleteInvoice(
                    invoiceID,
                    currentUser);

                MessageBox.Show(
                    "Xóa hóa đơn thành công!",
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadInvoices();

                // HỢP ĐỒNG VỪA XÓA HÓA ĐƠN
                // CÓ THỂ LẬP HÓA ĐƠN LẠI
                LoadEligibleContracts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa hóa đơn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // THÔNG TIN HỢP ĐỒNG TRÊN COMBOBOX
        private class ContractInvoiceItem
        {
            public int ContractID { get; set; }
            public string ContractNumber { get; set; }
            public string CustomerName { get; set; }
            public string LicensePlate { get; set; }
            public int RentalDays { get; set; }
            public decimal DailyRate { get; set; }
            public decimal Deposit { get; set; }
            public decimal RentalFee { get; set; }

            public override string ToString()
            {
                return ContractNumber + " - " +
                       CustomerName + " - " +
                       LicensePlate;
            }
        }
    }
}
