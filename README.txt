LAB 6. DATAGRIDVIEW, LISTVIEW, TREEVIEW VÀ MÔ HÌNH 3 LỚP CHƯA KẾT NỐI CSDL
Học phần: Lập trình .NET (IT3212)

Sinh viên: Nguyễn Tiến Hùng
Mã sinh viên: 24000091
Lớp: Lập trình .NET-1-1-26(N03)

1. CÁCH CHẠY
- Công cụ: Visual Studio 2022, project Windows Forms App (C#, .NET 10).
- Mở file Lab06_ThreeLayerMemory.sln bằng Visual Studio.
- Chọn Build > Rebuild Solution, sau đó nhấn F5 để chạy.
- Giao diện được tạo bằng code trong từng Form nên không có file Designer.
- Form chính là MDI. Dùng menu để mở: Sinh viên, Sản phẩm, Danh mục sản phẩm, Thoát.

2. DỮ LIỆU THỬ
- Sinh viên (9 người, 3 khoa, mỗi khoa 3 sinh viên): SV01 đến SV09.
  Khoa: Công nghệ thông tin, Kinh tế, Điện - Điện tử.
- Sản phẩm (7 sản phẩm, 3 danh mục): SP01 đến SP07.
  Danh mục: Điện thoại, Laptop, Phụ kiện.
- Dữ liệu sinh viên được lưu vào file sinhvien.csv, nằm cạnh file chạy
  (thư mục bin\Debug\net10.0-windows\). Lần chạy đầu tiên chương trình tự tạo
  file này từ dữ liệu mẫu; các lần sau đọc lại từ file.
- Không mở sinhvien.csv bằng Excel khi chương trình đang chạy (Excel khóa file).

3. KIẾN TRÚC 3 LỚP (tách bằng namespace, dữ liệu chạy trong bộ nhớ)
- Models: lớp SinhVien, Khoa, SanPham.
- DAL: KhoaDAL, SinhVienDAL, SanPhamDAL. Chỉ giữ và lưu dữ liệu giả lập
  (List trong bộ nhớ, SinhVienDAL có thêm đọc/ghi CSV).
- BUS: KhoaBUS, SinhVienBUS, SanPhamBUS. Kiểm tra nghiệp vụ, tìm kiếm,
  sắp xếp, tính tổng, xếp loại. BUS gọi DAL.
- GUI: frmMain, frmSinhVien, frmSanPham, frmDanhMuc, lớp UI/InputBox.
  Form chỉ gọi BUS, không chứa logic tính toán hay lưu dữ liệu.
- Sang Lab 7 chỉ cần thay lớp DAL bằng DAL kết nối SQL Server, giao diện
  và BUS gần như giữ nguyên.

4. CẤU TRÚC MÃ NGUỒN
- Program.cs: điểm vào, mở frmMain
- Models/Models.cs: các lớp mô hình
- DAL/DAL.cs: các lớp DAL giả lập
- BUS/BUS.cs: các lớp BUS
- GUI/UI.cs: hàm tạo control dùng chung và hộp nhập liệu InputBox
- GUI/frmMain.cs: Form chính MDI, tạo các BUS và truyền xuống Form con
- GUI/frmSinhVien.cs: DataGridView, ListView, TreeView và CRUD sinh viên
- GUI/frmSanPham.cs: quản lý sản phẩm bằng DataGridView
- GUI/frmDanhMuc.cs: TreeView danh mục - sản phẩm với ContextMenuStrip

5. CÁC BÀI ĐÃ HOÀN THÀNH
Bài 1. Hiển thị BindingList trên DataGridView
  frmSinhVien (tab DataGridView) gắn BindingList<SinhVien> từ SinhVienBUS.
  Tắt AutoGenerateColumns, tự cấu hình cột và tiêu đề 5tiếng Việt,
  chọn nguyên dòng (FullRowSelect).

Bài 2. CRUD trong bộ nhớ
  Nút Thêm, Sửa, Xóa gọi SinhVienBUS. Kiểm tra: thiếu mã, thiếu họ tên,
  chưa chọn khoa, điểm ngoài [0,10], ngày sinh trong tương lai, trùng mã.
  Lưới tự cập nhật nhờ BindingList. Xóa có hỏi xác nhận.

Bài 3. Danh sách chi tiết bằng ListView
  Tab ListView dạng Details (mã, họ tên, khoa, điểm, xếp loại). Xóa Items
  trước khi nạp lại. Có ô tìm kiếm theo mã, họ tên hoặc khoa.

Bài 4. Cây Khoa - Sinh viên bằng TreeView
  Node gốc là khoa (kèm số sinh viên), node con là sinh viên (đối tượng lưu
  trong Tag). Chọn node con thì hiển thị chi tiết bên phải.

Bài 5. Tách mô hình GUI - BUS - DAL giả lập
  SinhVienDAL giữ danh sách; SinhVienBUS kiểm tra nghiệp vụ; Form chỉ gọi BUS.

Bài 6. Quản lý sản phẩm bằng DataGridView
  frmSanPham: thêm, sửa, xóa, tìm kiếm (mã, tên, danh mục), sắp xếp theo
  cột và chiều tăng/giảm, hiển thị tổng giá trị hàng tồn
  (tổng số lượng x đơn giá của các sản phẩm đang hiển thị).

Bài 7. TreeView quản lý danh mục
  frmDanhMuc: danh mục - sản phẩm. Nhấp chuột phải (ContextMenuStrip) để
  thêm danh mục, thêm sản phẩm, đổi tên, xóa node. Thay đổi cập nhật sang
  frmSanPham qua sự kiện DuLieuThayDoi của SanPhamBUS.

Bài 8. ListView có biểu tượng
  ImageList (vẽ bằng code) hiển thị chấm màu theo xếp loại học lực:
  Xuất sắc (tím), Giỏi (xanh lá), Khá (xanh dương), Trung bình (cam), Yếu (đỏ).

Bài 9. Xuất dữ liệu ra tệp văn bản
  SinhVienDAL ghi danh sách ra sinhvien.csv sau mỗi lần Thêm, Sửa, Xóa và
  đọc lại khi khởi động. Có nút Xuất CSV (chọn nơi lưu) và Nạp lại CSV.

Bài 10. Hoàn thiện kiến trúc 3 lớp
  Tách namespace Models, DAL, BUS, GUI. Form không chứa logic tính toán
  hay lưu dữ liệu.

6. GHI CHÚ
- Điểm và ngày được ghi CSV theo InvariantCulture (dấu chấm, yyyy-MM-dd)
  để không sai trên máy dùng định dạng số Việt Nam.
- Xóa danh mục sản phẩm sẽ xóa luôn các sản phẩm thuộc danh mục đó
  (có hỏi xác nhận).
- Dữ liệu sản phẩm và danh mục chỉ nằm trong bộ nhớ, mất khi đóng chương
  trình. Dữ liệu sinh viên được lưu vào CSV.
- Lab 7 sẽ thay DAL giả lập bằng DAL kết nối SQL Server (ADO.NET).