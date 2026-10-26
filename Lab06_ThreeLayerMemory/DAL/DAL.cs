using Lab06_ThreeLayerMemory.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lab06_ThreeLayerMemory.DAL
{
    public class KhoaDAL
    {
        private readonly List<Khoa> _data = new()
    {
        new Khoa("CNTT", "Công nghệ thông tin"),
        new Khoa("KT", "Kinh tế"),
        new Khoa("DDT", "Điện - Điện tử"),
    };
        public List<Khoa> GetAll() { return new List<Khoa>(_data); }
    }

    public class SinhVienDAL
    {
        private readonly List<SinhVien> _data = new();
        private readonly string _filePath;
        public string FilePath => _filePath;



        public SinhVienDAL(string filePath = "")
        {
            _filePath = string.IsNullOrEmpty(filePath)
                ? Path.Combine(AppContext.BaseDirectory, "sinhvien.csv") : filePath;
            if (File.Exists(_filePath)) LoadFromCsv();
            if (_data.Count == 0)
            {
                SeedData();
                SaveTo(_filePath);
            }
        }

        private void Seed(string ma, string ten, int y, int m, int d, double diem, string khoa)
        {
            _data.Add(new SinhVien(ma, ten, new DateTime(y, m, d), diem, khoa));
        }

        private void SeedData()
        {
            Seed("SV01", "Nguyễn An", 2005, 3, 12, 8.2, "Công nghệ thông tin");
            Seed("SV02", "Trần Bình", 2005, 7, 21, 7.0, "Công nghệ thông tin");
            Seed("SV03", "Lê Cường", 2004, 11, 5, 9.1, "Công nghệ thông tin");
            Seed("SV04", "Phạm Dung", 2005, 1, 30, 6.5, "Kinh tế");
            Seed("SV05", "Hoàng Em", 2004, 9, 14, 7.8, "Kinh tế");
            Seed("SV06", "Vũ Giang", 2005, 5, 2, 4.8, "Kinh tế");
            Seed("SV07", "Đặng Hải", 2004, 12, 25, 8.6, "Điện - Điện tử");
            Seed("SV08", "Bùi Khánh", 2005, 8, 8, 5.9, "Điện - Điện tử");
            Seed("SV09", "Ngô Linh", 2004, 4, 17, 9.4, "Điện - Điện tử");
        }

        public List<SinhVien> GetAll() { return new List<SinhVien>(_data); }

        public void Add(SinhVien sv) { _data.Add(sv); SaveTo(_filePath); }

        public void Update(SinhVien sv)
        {
            int i = _data.FindIndex(x => x.MaSV == sv.MaSV);
            if (i >= 0) { _data[i] = sv; SaveTo(_filePath); }
        }

        public void Delete(string maSV)
        {
            _data.RemoveAll(x => x.MaSV == maSV);
            SaveTo(_filePath);
        }

        public void Reload()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException("Chưa có file CSV: " + _filePath);
            _data.Clear();
            LoadFromCsv();
        }

        public void SaveTo(string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("MaSV,HoTen,NgaySinh,DiemTB,TenKhoa");
            foreach (var sv in _data)
            {
                sb.AppendLine(string.Join(",",
                    Escape(sv.MaSV), Escape(sv.HoTen),
                    sv.NgaySinh.ToString("yyyy-MM-dd"),
                    sv.DiemTB.ToString(CultureInfo.InvariantCulture),
                    Escape(sv.TenKhoa)));
            }
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
        }

        private void LoadFromCsv()
        {
            foreach (string line in File.ReadAllLines(_filePath, Encoding.UTF8).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] f = ParseLine(line);
                if (f.Length < 5) continue;
                if (!DateTime.TryParseExact(f[2], "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime ngaySinh)) continue;
                if (!double.TryParse(f[3], NumberStyles.Float, CultureInfo.InvariantCulture,
                        out double diem)) continue;
                _data.Add(new SinhVien(f[0], f[1], ngaySinh, diem, f[4]));
            }
        }

        private static string Escape(string s)
        {
            if (s.Contains(',') || s.Contains('"'))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private static string[] ParseLine(string line)
        {
            var result = new List<string>();
            var cur = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"') { cur.Append('"'); i++; }
                        else inQuotes = false;
                    }
                    else cur.Append(c);
                }
                else if (c == '"') inQuotes = true;
                else if (c == ',') { result.Add(cur.ToString()); cur.Clear(); }
                else cur.Append(c);
            }
            result.Add(cur.ToString());
            return result.ToArray();
        }
    }

    public class SanPhamDAL
    {
        private readonly List<string> _danhMuc = new() { "Điện thoại", "Laptop", "Phụ kiện" };
        private readonly List<SanPham> _data = new()
    {
        new SanPham("SP01", "iPhone 15", "Điện thoại", 10, 22000000m),
        new SanPham("SP02", "Galaxy S24", "Điện thoại", 15, 19500000m),
        new SanPham("SP03", "Dell XPS 13", "Laptop", 5, 32000000m),
        new SanPham("SP04", "MacBook Air M2", "Laptop", 7, 28000000m),
        new SanPham("SP05", "Tai nghe Bluetooth", "Phụ kiện", 40, 650000m),
        new SanPham("SP06", "Sạc nhanh 65W", "Phụ kiện", 60, 450000m),
        new SanPham("SP07", "Chuột không dây", "Phụ kiện", 35, 250000m),
    };

        public List<string> GetDanhMuc() { return new List<string>(_danhMuc); }
        public List<SanPham> GetAll() { return new List<SanPham>(_data); }
        public void Add(SanPham sp) { _data.Add(sp); }

        public void Update(SanPham sp)
        {
            int i = _data.FindIndex(x => x.MaSP == sp.MaSP);
            if (i >= 0) _data[i] = sp;
        }

        public void Delete(string ma) { _data.RemoveAll(x => x.MaSP == ma); }
        public void AddDanhMuc(string ten) { _danhMuc.Add(ten); }

        public void RenameDanhMuc(string cu, string moi)
        {
            int i = _danhMuc.FindIndex(x => x == cu);
            if (i < 0) return;
            _danhMuc[i] = moi;
            foreach (var sp in _data.Where(x => x.DanhMuc == cu).ToList()) sp.DanhMuc = moi;
        }

        public void DeleteDanhMuc(string ten)
        {
            _danhMuc.RemoveAll(x => x == ten);
            _data.RemoveAll(x => x.DanhMuc == ten);
        }
    }
}
