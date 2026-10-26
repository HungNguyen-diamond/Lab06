using System;
using System.Collections.Generic;
using System.Text;

namespace Lab06_ThreeLayerMemory.Models
{
    public class Khoa
    {
        public string MaKhoa { get; set; } = "";
        public string TenKhoa { get; set; } = "";
        public Khoa() { }
        public Khoa(string ma, string ten) { MaKhoa = ma; TenKhoa = ten; }
        public override string ToString() => TenKhoa;
    }

    public class SinhVien
    {
        public string MaSV { get; set; } = "";
        public string HoTen { get; set; } = "";
        public DateTime NgaySinh { get; set; } = DateTime.Today;
        public double DiemTB { get; set; }
        public string TenKhoa { get; set; } = "";

        public SinhVien() { }
        public SinhVien(string maSV, string hoTen, DateTime ngaySinh, double diemTB, string tenKhoa = "")
        {
            MaSV = maSV; HoTen = hoTen; NgaySinh = ngaySinh; DiemTB = diemTB; TenKhoa = tenKhoa;
        }
        public override string ToString() => MaSV + " - " + HoTen;
    }

    public class SanPham
    {
        public string MaSP { get; set; } = "";
        public string TenSP { get; set; } = "";
        public string DanhMuc { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;

        public SanPham() { }
        public SanPham(string ma, string ten, string danhMuc, int soLuong, decimal donGia)
        {
            MaSP = ma; TenSP = ten; DanhMuc = danhMuc; SoLuong = soLuong; DonGia = donGia;
        }
    }
}
