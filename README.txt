 README – LINHXURY
 HỆ THỐNG QUẢN LÝ CHO THUÊ XE

 1. GIỚI THIỆU DỰ ÁN

LINHXURY là hệ thống quản lý cho thuê xe được xây dựng dưới dạng ứng dụng desktop, hỗ trợ quản lý hoạt động kinh doanh dịch vụ cho thuê xe. Hệ thống giúp người quản lý và nhân viên thực hiện các nghiệp vụ như quản lý xe, quản lý khách hàng, lập hợp đồng thuê xe, ghi nhận quá trình giao – nhận xe, lập hóa đơn và theo dõi doanh thu.

Phần mềm được phát triển nhằm hỗ trợ quá trình quản lý được thực hiện tập trung, hạn chế thao tác thủ công, giảm sai sót trong quá trình xử lý thông tin và nâng cao hiệu quả quản lý hoạt động cho thuê xe.

 2. CÔNG NGHỆ SỬ DỤNG

- Ngôn ngữ lập trình: C#.
- Nền tảng: .NET Framework 4.7.2.
- Giao diện: Windows Forms (WinForms).
- Hệ quản trị cơ sở dữ liệu: Microsoft SQL Server.
- Kết nối cơ sở dữ liệu: ADO.NET.
- Công cụ phát triển: Visual Studio.
- Thành phần hỗ trợ giao diện đăng nhập: Microsoft WebView2.

 3. CẤU TRÚC DỰ ÁN

Dự án được tổ chức theo mô hình phân tầng nhằm tách biệt giao diện, xử lý nghiệp vụ, truy xuất dữ liệu và các lớp đối tượng.

CarRentalManagement.Models
- Chứa các lớp đối tượng đại diện cho dữ liệu của hệ thống.
- Bao gồm các lớp như Car, Customer, Contract, Invoice, User và VehicleHandover.

CarRentalManagement.DAL
- Chịu trách nhiệm kết nối và truy xuất dữ liệu từ SQL Server.
- Thực hiện các thao tác thêm, sửa, xóa, tìm kiếm và lấy dữ liệu thông qua ADO.NET.

CarRentalManagement.BLL
- Chứa các lớp xử lý nghiệp vụ của hệ thống.
- Tiếp nhận yêu cầu từ Presentation, kiểm tra dữ liệu và phối hợp với DAL để xử lý nghiệp vụ.

CarRentalManagement.Presentation
- Chứa giao diện Windows Forms.
- Hiển thị các màn hình chức năng và tiếp nhận thao tác của người dùng.

 4. CÁC CHỨC NĂNG CHÍNH

4.1. Đăng nhập và phân quyền
- Đăng nhập bằng tài khoản được lưu trong cơ sở dữ liệu.
- Kiểm tra thông tin đăng nhập.
- Phân quyền theo vai trò Admin và Staff.
- Giới hạn quyền truy cập chức năng phù hợp với từng vai trò.

4.2. Quản lý xe
- Thêm thông tin xe mới.
- Cập nhật thông tin xe.
- Xóa xe theo điều kiện dữ liệu.
- Hiển thị danh sách xe.
- Tìm kiếm và lọc xe.
- Quản lý trạng thái xe.

4.3. Danh mục xe cho thuê
- Hiển thị danh sách xe dưới dạng thẻ thông tin.
- Hiển thị hình ảnh, tên xe, hãng xe, mẫu xe và giá thuê theo ngày.
- Hiển thị tình trạng xe.
- Lọc và sắp xếp danh sách xe.
- Hỗ trợ chọn xe để chuyển thông tin sang quá trình lập hợp đồng.

4.4. Quản lý khách hàng
- Thêm khách hàng mới.
- Cập nhật thông tin khách hàng.
- Xóa khách hàng khi đáp ứng điều kiện dữ liệu.
- Tìm kiếm và hiển thị thông tin khách hàng.

4.5. Quản lý hợp đồng
- Lập hợp đồng thuê xe.
- Lựa chọn khách hàng và xe thuê.
- Quản lý thời gian thuê và thông tin tiền cọc.
- Theo dõi trạng thái hợp đồng.
- Cập nhật và quản lý hợp đồng trong quá trình thuê xe.

4.6. Quản lý giao – nhận xe
- Ghi nhận thông tin giao xe cho khách hàng.
- Ghi nhận thông tin nhận lại xe.
- Lưu thời gian giao và nhận xe.
- Theo dõi tình trạng xe trong quá trình bàn giao.
- Cập nhật trạng thái xe theo nghiệp vụ.

4.7. Quản lý hóa đơn
- Lập hóa đơn dựa trên hợp đồng đủ điều kiện.
- Theo dõi thông tin thanh toán.
- Quản lý trạng thái hóa đơn.
- Hỗ trợ theo dõi khoản thu từ hoạt động cho thuê xe.

4.8. Thống kê doanh thu
- Tổng hợp doanh thu từ hóa đơn.
- Hiển thị các thông tin thống kê phục vụ quản lý.
- Hỗ trợ theo dõi tình hình kinh doanh của hệ thống.

 5. YÊU CẦU MÔI TRƯỜNG

Để chạy chương trình, máy tính cần đáp ứng các yêu cầu sau:

- Hệ điều hành Windows.
- Visual Studio có hỗ trợ .NET Framework 4.7.2.
- .NET Framework 4.7.2.
- Microsoft SQL Server hoặc SQL Server Express.
- SQL Server Management Studio (SSMS) để quản lý cơ sở dữ liệu.
- Microsoft Edge WebView2 Runtime để hỗ trợ giao diện đăng nhập có video nền.

 6. CẤU HÌNH CƠ SỞ DỮ LIỆU

Tên cơ sở dữ liệu: LINHXURY

Máy chủ SQL Server đang sử dụng trong môi trường phát triển:

LAPTOP-TIAT3U08\SQLEXPRESS

Chuỗi kết nối hiện tại:

Data Source=LAPTOP-TIAT3U08\SQLEXPRESS;Initial Catalog=LINHXURY;Integrated Security=True

Lưu ý: Khi chạy chương trình trên máy tính khác, cần thay đổi tên máy chủ SQL Server trong chuỗi kết nối cho phù hợp với môi trường cài đặt.

Cơ sở dữ liệu cần được tạo trước khi chạy ứng dụng. Các bảng dữ liệu phải có đầy đủ khóa chính, khóa ngoại và các ràng buộc cần thiết để đảm bảo tính toàn vẹn dữ liệu.

 7. HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY CHƯƠNG TRÌNH

Bước 1: Cài đặt môi trường
- Cài đặt Visual Studio.
- Cài đặt .NET Framework 4.7.2.
- Cài đặt SQL Server Express.
- Cài đặt SQL Server Management Studio.
- Cài đặt WebView2 Runtime nếu máy chưa có.

Bước 2: Chuẩn bị cơ sở dữ liệu
- Mở SQL Server Management Studio.
- Kết nối đến SQL Server.
- Tạo cơ sở dữ liệu có tên LINHXURY.
- Thực thi file SQL đi kèm dự án để tạo bảng và dữ liệu mẫu (sau khi đã bổ sung file Database.sql vào bộ hồ sơ).

Bước 3: Mở source code
- Giải nén thư mục source code.
- Mở file CarRentalManagement.sln bằng Visual Studio.
- Kiểm tra các project trong Solution Explorer.
- Đặt CarRentalManagement.Presentation làm Startup Project.

Bước 4: Kiểm tra chuỗi kết nối
- Mở file cấu hình kết nối cơ sở dữ liệu trong project DAL.
- Điều chỉnh tên máy chủ SQL Server nếu cần.
- Đảm bảo tên cơ sở dữ liệu là LINHXURY.

Bước 5: Chạy chương trình
- Chọn Build Solution để biên dịch toàn bộ dự án.
- Nếu không có lỗi, nhấn Start để chạy chương trình.
- Đăng nhập bằng tài khoản được cung cấp trong phần tài khoản demo.

 8. TÀI KHOẢN ĐĂNG NHẬP DEMO

Tài khoản nhân viên:

Tên đăng nhập: staff01

Mật khẩu: Staff@123

Vai trò: Staff

Tài khoản quản trị viên:

Tên đăng nhập: admin

Mật khẩu: Admin@123

Vai trò: Admin

Lưu ý: Tài khoản demo chỉ phục vụ mục đích kiểm thử và trình diễn phần mềm.

 9. CẤU TRÚC THƯ MỤC NỘP BÀI

MSSV_HoTen_LINHXURY

BaoCao
- BaoCao.docx
- BaoCao.pdf

SourceCode
- CarRentalManagement.sln
- CarRentalManagement.Models
- CarRentalManagement.DAL
- CarRentalManagement.BLL
- CarRentalManagement.Presentation
- Assets
- README.txt

Database
- LINHXURY.sql
- DuLieuMau.sql (nếu tách riêng dữ liệu mẫu)

Lưu ý: Thư mục Assets chứa hình ảnh xe và video nền sử dụng trong giao diện. Cần giữ nguyên thư mục này để chương trình hiển thị đầy đủ hình ảnh và video.

 10. LƯU Ý KHI SỬ DỤNG

- Cần khởi động SQL Server trước khi chạy ứng dụng.
- Cần đảm bảo chuỗi kết nối chính xác.
- Không tự ý thay đổi tên bảng hoặc tên cột trong cơ sở dữ liệu nếu chưa cập nhật các phần code liên quan.
- Nên sao lưu cơ sở dữ liệu trước khi thực hiện các thao tác chỉnh sửa lớn.
- Khi chuyển dự án sang máy tính khác, cần kiểm tra lại đường dẫn tài nguyên hình ảnh, video và các thư viện phụ thuộc.

 11. THÔNG TIN SINH VIÊN

Tên đề tài: Hệ thống quản lý cho thuê xe LINHXURY

Sinh viên thực hiện: Lương Ngọc Linh
Mã số sinh viên: 233404050188

Lớp: Đ23HT1
Giảng viên hướng dẫn: Lê Hữu Nhơn

Năm thực hiện: 2026

