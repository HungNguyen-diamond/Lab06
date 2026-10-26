using Lab06_ThreeLayerMemory.DAL;
using Lab06_ThreeLayerMemory.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Lab06_ThreeLayerMemory.BUS
{
    public class KhoaBUS
    {
        private readonly KhoaDAL _dal = new KhoaDAL();
        public List<Khoa> GetAll() { return _dal.GetAll(); }
    }

    public class SinhVienBUS
    {
        public static readonly string[] XepLoaiList = { "Xuất sắc", "Giỏi", "Khá", "Trung bình", "Yếu" };

        private readonly SinhVienDAL _dal = new SinhVienDAL();
        private readonly BindingList<SinhVien> _ds;

        public SinhVienBUS() { _ds = new BindingList<SinhVien>(_dal.GetAll()); }

        public BindingList<SinhVien> DanhSach { get { return _ds; } }
        public string DuongDanCsv { get { return _dal.FilePath; } }

        private static void Validate(SinhVien sv)
        {
            if (string.IsNullOrWhiteSpace(sv.MaSV)) throw new ArgumentException("Thiếu mã sinh viên");
            if (string.IsNullOrWhiteSpace(sv.HoTen)) throw new ArgumentException("Thiếu họ tên");
            if (string.IsNullOrWhiteSpace(sv.TenKhoa)) throw new ArgumentException("Chưa chọn khoa");
            if (sv.DiemTB < 0 || sv.DiemTB > 10) throw new ArgumentException("Điểm phải trong khoảng [0, 10]");
            if (sv.NgaySinh > DateTime.Today) throw new ArgumentException("Ngày sinh không hợp lệ");
        }

        private int IndexOf(string ma)
        {
            for (int i = 0; i < _ds.Count; i++)
                if (_ds[i].MaSV.Equals(ma, StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }

       
        
        public void Add(SinhVien sv)
        {
            sv.MaSV = (sv.MaSV ?? "").Trim();
            sv.HoTen = (sv.HoTen ?? "").Trim();
            Validate(sv);
            if (IndexOf(sv.MaSV) >= 0) throw new InvalidOperationException("Trùng mã sinh viên");
            _dal.Add(sv);
            _ds.Add(sv);
        }

        public void Update(SinhVien sv)
        {
            sv.MaSV = (sv.MaSV ?? "").Trim();
            sv.HoTen = (sv.HoTen ?? "").Trim();
            Validate(sv);
            int i = IndexOf(sv.MaSV);
            if (i < 0) throw new InvalidOperationException("Không tìm thấy sinh viên có mã " + sv.MaSV);
            sv.MaSV = _ds[i].MaSV;       
            _dal.Update(sv);
            _ds[i] = sv;                
        }

        public void Delete(string maSV)
        {
            int i = IndexOf((maSV ?? "").Trim());
            if (i < 0) throw new InvalidOperationException("Không tìm thấy sinh viên có mã " + maSV);
            _dal.Delete(_ds[i].MaSV);
            _ds.RemoveAt(i);
        }




        public List<SinhVien> Search(string keyword)
        {
            keyword = (keyword ?? "").Trim();
            return _ds.Where(x => keyword == ""
                    || x.MaSV.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || x.HoTen.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || x.TenKhoa.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }





        public Dictionary<string, List<SinhVien>> NhomTheoKhoa()
        {
            return _ds.GroupBy(x => x.TenKhoa)
                      .OrderBy(g => g.Key)
                      .ToDictionary(g => g.Key, g => g.OrderBy(x => x.MaSV).ToList());
        }

       
        
        
        public static string XepLoai(double diem)
        {
            if (diem >= 9) return XepLoaiList[0];
            if (diem >= 8) return XepLoaiList[1];
            if (diem >= 7) return XepLoaiList[2];
            if (diem >= 5) return XepLoaiList[3];
            return XepLoaiList[4];
        }




        public void XuatCsv(string path) { _dal.SaveTo(path); }

        public void NapLaiTuCsv()
        {
            _dal.Reload();
            _ds.Clear();
            foreach (var sv in _dal.GetAll()) _ds.Add(sv);
        }
    }

    public class SanPhamBUS
    {
        public static readonly string[] TruongSapXep = { "Mã", "Tên", "Danh mục", "Số lượng", "Đơn giá", "Thành tiền" };

        private readonly SanPhamDAL _dal = new SanPhamDAL();

        public event EventHandler DuLieuThayDoi;
        private void BaoThayDoi() { DuLieuThayDoi?.Invoke(this, EventArgs.Empty); }

        public List<string> GetDanhMuc() { return _dal.GetDanhMuc(); }
        public List<SanPham> GetAll() { return _dal.GetAll(); }

        private SanPham Tim(string ma)
        {
            string k = (ma ?? "").Trim();
            return _dal.GetAll().FirstOrDefault(x => x.MaSP.Equals(k, StringComparison.OrdinalIgnoreCase));
        }

        private void Validate(SanPham sp)
        {
            if (string.IsNullOrWhiteSpace(sp.MaSP)) throw new ArgumentException("Thiếu mã sản phẩm");
            if (string.IsNullOrWhiteSpace(sp.TenSP)) throw new ArgumentException("Thiếu tên sản phẩm");
            if (string.IsNullOrWhiteSpace(sp.DanhMuc) || !_dal.GetDanhMuc().Contains(sp.DanhMuc))
                throw new ArgumentException("Chưa chọn danh mục hợp lệ");
            if (sp.SoLuong < 0) throw new ArgumentException("Số lượng không được âm");
            if (sp.DonGia < 0) throw new ArgumentException("Đơn giá không được âm");
        }





        public List<SanPham> Query(string tuKhoa, string truongSapXep, bool tang)
        {
            tuKhoa = (tuKhoa ?? "").Trim();
            IEnumerable<SanPham> q = _dal.GetAll().Where(x => tuKhoa == ""
                || x.MaSP.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)
                || x.TenSP.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)
                || x.DanhMuc.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase));

            Func<SanPham, object> key;
            switch (truongSapXep)
            {
                case "Tên": key = x => x.TenSP; break;
                case "Danh mục": key = x => x.DanhMuc; break;
                case "Số lượng": key = x => x.SoLuong; break;
                case "Đơn giá": key = x => x.DonGia; break;
                case "Thành tiền": key = x => x.ThanhTien; break;
                default: key = x => x.MaSP; break;
            }
            return (tang ? q.OrderBy(key) : q.OrderByDescending(key)).ToList();
        }




        public decimal TongGiaTriTon(IEnumerable<SanPham> ds) { return ds.Sum(x => x.ThanhTien); }

        public void Add(SanPham sp)
        {
            sp.MaSP = (sp.MaSP ?? "").Trim();
            sp.TenSP = (sp.TenSP ?? "").Trim();
            Validate(sp);
            if (Tim(sp.MaSP) != null) throw new InvalidOperationException("Trùng mã sản phẩm");
            _dal.Add(sp);
            BaoThayDoi();
        }

        public void Update(SanPham sp)
        {
            sp.TenSP = (sp.TenSP ?? "").Trim();
            var goc = Tim(sp.MaSP);
            if (goc == null) throw new InvalidOperationException("Không tìm thấy sản phẩm có mã " + sp.MaSP);
            sp.MaSP = goc.MaSP;
            Validate(sp);
            _dal.Update(sp);
            BaoThayDoi();
        }

        public void Delete(string ma)
        {
            var goc = Tim(ma);
            if (goc == null) throw new InvalidOperationException("Không tìm thấy sản phẩm có mã " + ma);
            _dal.Delete(goc.MaSP);
            BaoThayDoi();
        }




        public void ThemDanhMuc(string ten)
        {
            ten = (ten ?? "").Trim();
            if (ten == "") throw new ArgumentException("Thiếu tên danh mục");
            if (_dal.GetDanhMuc().Any(x => x.Equals(ten, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Danh mục đã tồn tại");
            _dal.AddDanhMuc(ten);
            BaoThayDoi();
        }

        public void DoiTenDanhMuc(string cu, string moi)
        {
            moi = (moi ?? "").Trim();
            if (moi == "") throw new ArgumentException("Thiếu tên danh mục");
            if (_dal.GetDanhMuc().Any(x => x != cu && x.Equals(moi, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Danh mục đã tồn tại");
            _dal.RenameDanhMuc(cu, moi);
            BaoThayDoi();
        }

        public void XoaDanhMuc(string ten)
        {
            _dal.DeleteDanhMuc(ten);     
            BaoThayDoi();
        }

        public void ThemNhanh(string danhMuc, string tenSP)
        {
            Add(new SanPham(SinhMa(), tenSP, danhMuc, 0, 0));
        }

        public void DoiTenSanPham(string ma, string tenMoi)
        {
            tenMoi = (tenMoi ?? "").Trim();
            if (tenMoi == "") throw new ArgumentException("Thiếu tên sản phẩm");
            var sp = Tim(ma);
            if (sp == null) throw new InvalidOperationException("Không tìm thấy sản phẩm");
            sp.TenSP = tenMoi;
            _dal.Update(sp);
            BaoThayDoi();
        }

        private string SinhMa()
        {
            int max = 0;
            foreach (var sp in _dal.GetAll())
            {
                if (sp.MaSP.StartsWith("SP", StringComparison.OrdinalIgnoreCase)
                    && int.TryParse(sp.MaSP.Substring(2), out int n) && n > max) max = n;
            }
            return "SP" + (max + 1).ToString("D2");
        }
    }
}
