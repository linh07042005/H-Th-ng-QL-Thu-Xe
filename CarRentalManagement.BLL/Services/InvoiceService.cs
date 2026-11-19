
using System;
using System.Collections.Generic;
using CarRentalManagement.DAL.Repositories;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.BLL.Services
{
    public class InvoiceService
    {
        private readonly InvoiceRepository invoiceRepository;

        public InvoiceService()
        {
            invoiceRepository = new InvoiceRepository();
        }

        // Lấy danh sách hóa đơn
        public List<Invoice> GetAllInvoices()
        {
            return invoiceRepository.GetAll();
        }

        // Tính tổng tiền hóa đơn
        public decimal CalculateTotalAmount(
            decimal rentalFee,
            decimal extraFee,
            decimal depositDeduction)
        {
            if (rentalFee < 0)
                throw new Exception("Tiền thuê xe không được âm!");

            if (extraFee < 0)
                throw new Exception("Phụ phí không được âm!");

            if (depositDeduction < 0)
                throw new Exception("Tiền cọc khấu trừ không được âm!");

            if (rentalFee + extraFee < depositDeduction)
                throw new Exception(
                    "Tiền cọc khấu trừ không được lớn hơn tiền thuê và phụ phí!");

            return rentalFee + extraFee - depositDeduction;
        }

        // Thêm hóa đơn
        public void AddInvoice(Invoice invoice)
        {
            ValidateInvoice(invoice);

            invoice.TotalAmount = CalculateTotalAmount(
                invoice.RentalFee,
                invoice.ExtraFee,
                invoice.DepositDeduction
            );

            invoiceRepository.Add(invoice);
        }

        // Cập nhật trạng thái thanh toán
        public void UpdatePaymentStatus(int invoiceID, string paymentStatus)
        {
            if (invoiceID <= 0)
                throw new Exception("Mã hóa đơn không hợp lệ!");

            if (paymentStatus != "Chưa thanh toán" &&
                paymentStatus != "Đã thanh toán")
            {
                throw new Exception("Trạng thái thanh toán không hợp lệ!");
            }

            invoiceRepository.UpdatePaymentStatus(invoiceID, paymentStatus);
        }

        // Xóa hóa đơn - chỉ Admin được xóa hóa đơn chưa thanh toán
        public void DeleteInvoice(int invoiceID, User currentUser)
        {
            if (currentUser == null)
                throw new Exception("Không xác định được người dùng!");

            if (currentUser.Role != "Admin")
                throw new Exception("Chỉ Admin mới có quyền xóa hóa đơn!");

            if (invoiceID <= 0)
                throw new Exception("Mã hóa đơn không hợp lệ!");

            bool deleted = invoiceRepository.Delete(invoiceID);

            if (!deleted)
            {
                throw new Exception(
                    "Không thể xóa hóa đơn này! Hóa đơn có thể đã thanh toán hoặc không tồn tại.");
            }
        }

        // Kiểm tra thông tin hóa đơn
        private void ValidateInvoice(Invoice invoice)
        {
            if (invoice == null)
                throw new Exception("Thông tin hóa đơn không hợp lệ!");

            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
                throw new Exception("Vui lòng nhập mã hóa đơn!");

            if (invoice.ContractID <= 0)
                throw new Exception("Vui lòng chọn hợp đồng!");

            if (invoice.RentalFee < 0)
                throw new Exception("Tiền thuê xe không hợp lệ!");

            if (invoice.ExtraFee < 0)
                throw new Exception("Phụ phí không hợp lệ!");

            if (invoice.DepositDeduction < 0)
                throw new Exception("Tiền cọc khấu trừ không hợp lệ!");

            if (invoice.RentalFee + invoice.ExtraFee <
                invoice.DepositDeduction)
            {
                throw new Exception(
                    "Tiền cọc khấu trừ vượt quá tổng chi phí!");
            }

            if (invoice.PaymentStatus != "Chưa thanh toán" &&
                invoice.PaymentStatus != "Đã thanh toán")
            {
                throw new Exception("Trạng thái thanh toán không hợp lệ!");
            }
        }
    }
}
