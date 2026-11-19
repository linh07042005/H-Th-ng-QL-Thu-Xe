
using System;
using System.Windows.Forms;
using CarRentalManagement.BLL.Services;

namespace CarRentalManagement.Presentation
{
    public partial class FrmCreateAdmin : Form
    {
        public FrmCreateAdmin()
        {
            InitializeComponent();

            try
            {
                string passwordHash =
                    PasswordHelper.HashPassword("Admin@123");

                Clipboard.SetText(passwordHash);

                MessageBox.Show(
                    "ĐÃ TẠO MÃ BĂM VÀ COPY THÀNH CÔNG!\n\n" +
                    "Bà chỉ cần mở Notepad và nhấn Ctrl + V để lấy mã.",
                    "LINHXURY - Password Hash",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
