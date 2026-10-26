using System.Drawing.Drawing2D;
using Lab06_ThreeLayerMemory.BUS;
using Lab06_ThreeLayerMemory.Models;
namespace Lab06_ThreeLayerMemory.GUI;

public class frmSinhVien : Form
{
    private readonly SinhVienBUS _bus;
    private readonly KhoaBUS _khoaBus;

    private TextBox txtMaSV, txtHoTen, txtTim;
    private DateTimePicker dtpNgaySinh;
    private ComboBox cboKhoa;
    private NumericUpDown nudDiemTB;
    private DataGridView dgvSinhVien;
    private ListView lvSinhVien;
    private ImageList imgXepLoai;
    private TreeView tvKhoa;
    private Label lblChiTiet;

    public frmSinhVien(SinhVienBUS bus, KhoaBUS khoaBus)
    {
        _bus = bus;
        _khoaBus = khoaBus;
        InitializeUI();
    }

    private void InitializeUI()
    {
        Text = "Quản lý sinh viên";
        ClientSize = new Size(860, 600);

        // ----- Khu nhập liệu -----
        txtMaSV = UI.Txt(110, 15, 150);
        txtHoTen = UI.Txt(110, 50, 250);
        dtpNgaySinh = new DateTimePicker
        {
            Left = 110,
            Top = 85,
            Width = 150,
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy"
        };
        cboKhoa = new ComboBox { Left = 480, Top = 15, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        nudDiemTB = new NumericUpDown
        {
            Left = 480,
            Top = 50,
            Width = 80,
            Minimum = 0,
            Maximum = 10,
            DecimalPlaces = 2,
            Increment = 0.1M
        };

        var btnThem = UI.Btn("Thêm", 20, 125);
        var btnSua = UI.Btn("Sửa", 125, 125);
        var btnXoa = UI.Btn("Xóa", 230, 125);
        var btnLamMoi = UI.Btn("Làm mới", 335, 125);
        var btnXuatCsv = UI.Btn("Xuất CSV...", 440, 125, 110);
        var btnNapCsv = UI.Btn("Nạp lại CSV", 560, 125, 120);
        var lblCsv = new Label
        {
            Left = 20,
            Top = 163,
            Width = 820,
            Text = "Dữ liệu tự lưu tại: " + _bus.DuongDanCsv
        };

        Controls.AddRange(new Control[] {
            UI.Lbl("Mã SV", 20, 15), txtMaSV,
            UI.Lbl("Họ tên", 20, 50), txtHoTen,
            UI.Lbl("Ngày sinh", 20, 85), dtpNgaySinh,
            UI.Lbl("Khoa", 400, 15, 75), cboKhoa,
            UI.Lbl("Điểm TB", 400, 50, 75), nudDiemTB,
            btnThem, btnSua, btnXoa, btnLamMoi, btnXuatCsv, btnNapCsv, lblCsv });

        // ----- TabControl -----
        var tabs = new TabControl
        {
            Left = 10,
            Top = 190,
            Width = 840,
            Height = 400,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        var tabDgv = new TabPage("DataGridView");
        var tabLv = new TabPage("ListView");
        var tabTv = new TabPage("TreeView");
        tabs.TabPages.AddRange(new[] { tabDgv, tabLv, tabTv });
        Controls.Add(tabs);

        // ----- Bài 1: DataGridView, tắt AutoGenerateColumns để tự cấu hình cột -----
        dgvSinhVien = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSV", HeaderText = "Mã SV", FillWeight = 15 });
        dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoTen", HeaderText = "Họ tên", FillWeight = 30 });
        dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "NgaySinh",
            HeaderText = "Ngày sinh",
            FillWeight = 17,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
        });
        dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenKhoa", HeaderText = "Khoa", FillWeight = 28 });
        dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "DiemTB",
            HeaderText = "Điểm TB",
            FillWeight = 10,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        tabDgv.Controls.Add(dgvSinhVien);

        // ----- Bài 8: ImageList vẽ bằng code (mỗi xếp loại một màu) -----
        imgXepLoai = TaoImageList();

        // ----- Bài 3: ListView dạng Details -----
        txtTim = UI.Txt(100, 10, 300);
        lvSinhVien = new ListView
        {
            Left = 10,
            Top = 45,
            Width = 810,
            Height = 320,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            MultiSelect = false,
            HideSelection = false,
            SmallImageList = imgXepLoai,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        lvSinhVien.Columns.Add("Mã SV", 90);
        lvSinhVien.Columns.Add("Họ tên", 200);
        lvSinhVien.Columns.Add("Khoa", 200);
        lvSinhVien.Columns.Add("Điểm TB", 80, HorizontalAlignment.Right);
        lvSinhVien.Columns.Add("Xếp loại", 120);
        tabLv.Controls.AddRange(new Control[] { UI.Lbl("Tìm kiếm", 10, 10), txtTim, lvSinhVien });

        // ----- Bài 4: TreeView Khoa - Sinh viên -----
        tvKhoa = new TreeView
        {
            Left = 10,
            Top = 10,
            Width = 380,
            Height = 355,
            HideSelection = false,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
        };
        lblChiTiet = new Label
        {
            Left = 410,
            Top = 10,
            Width = 410,
            Height = 355,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10),
            Font = new Font(Font.FontFamily, 11f),
            Text = "Chọn một sinh viên trong cây để xem chi tiết.",
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        tabTv.Controls.AddRange(new Control[] { tvKhoa, lblChiTiet });

        // ----- Sự kiện -----
        btnThem.Click += btnThem_Click;
        btnSua.Click += btnSua_Click;
        btnXoa.Click += btnXoa_Click;
        btnLamMoi.Click += (s, e) => ClearInput();
        btnXuatCsv.Click += btnXuatCsv_Click;
        btnNapCsv.Click += btnNapCsv_Click;
        txtTim.TextChanged += (s, e) => LoadListView(_bus.Search(txtTim.Text));
        dgvSinhVien.SelectionChanged += dgvSinhVien_SelectionChanged;
        lvSinhVien.SelectedIndexChanged += lvSinhVien_SelectedIndexChanged;
        tvKhoa.AfterSelect += tvKhoa_AfterSelect;

        // ----- Gắn dữ liệu (làm sau cùng) -----
        cboKhoa.DataSource = _khoaBus.GetAll();
        cboKhoa.SelectedIndex = -1;
        dgvSinhVien.DataSource = _bus.DanhSach;
        RefreshViews();
    }

    // Bài 8: tạo ImageList gồm 5 chấm tròn màu, đúng thứ tự SinhVienBUS.XepLoaiList
    // (Xuất sắc, Giỏi, Khá, Trung bình, Yếu)
    private static ImageList TaoImageList()
    {
        var list = new ImageList
        {
            ColorDepth = ColorDepth.Depth32Bit,
            ImageSize = new Size(16, 16)
        };
        _ = list.Handle;   // ép tạo handle trước để ảnh được sao chép vào ImageList ngay

        Color[] mau = { Color.MediumPurple, Color.SeaGreen, Color.DodgerBlue, Color.Orange, Color.Crimson };
        foreach (Color m in mau)
        {
            var bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using var br = new SolidBrush(m);
                g.FillEllipse(br, 1, 1, 14, 14);
            }
            list.Images.Add(bmp);   // chỉ số 0..4 ứng với Xuất sắc..Yếu
        }
        return list;
    }

    // ===== Bài 3, 4: nạp lại ListView và TreeView =====
    private void LoadListView(IEnumerable<SinhVien> data)
    {
        lvSinhVien.BeginUpdate();
        lvSinhVien.Items.Clear();
        foreach (SinhVien sv in data)
        {
            string xepLoai = SinhVienBUS.XepLoai(sv.DiemTB);
            int idx = Array.IndexOf(SinhVienBUS.XepLoaiList, xepLoai);
            var item = new ListViewItem(sv.MaSV) { ImageIndex = idx, Tag = sv };
            item.SubItems.Add(sv.HoTen);
            item.SubItems.Add(sv.TenKhoa);
            item.SubItems.Add(sv.DiemTB.ToString("N2"));
            item.SubItems.Add(xepLoai);
            lvSinhVien.Items.Add(item);
        }
        lvSinhVien.EndUpdate();
    }

    private void LoadTree()
    {
        tvKhoa.BeginUpdate();
        tvKhoa.Nodes.Clear();
        foreach (var group in _bus.NhomTheoKhoa())
        {
            TreeNode khoaNode = new TreeNode(group.Key + " (" + group.Value.Count + ")");
            foreach (SinhVien sv in group.Value)
            {
                TreeNode svNode = new TreeNode(sv.MaSV + " - " + sv.HoTen);
                svNode.Tag = sv;
                khoaNode.Nodes.Add(svNode);
            }
            tvKhoa.Nodes.Add(khoaNode);
        }
        tvKhoa.ExpandAll();
        tvKhoa.EndUpdate();
    }

    private void RefreshViews()
    {
        LoadListView(_bus.Search(txtTim.Text));
        LoadTree();
    }

    // ===== Đồng bộ ô nhập với dòng đang chọn =====
    private void FillInput(SinhVien sv)
    {
        txtMaSV.Text = sv.MaSV;
        txtHoTen.Text = sv.HoTen;
        dtpNgaySinh.Value = sv.NgaySinh;
        cboKhoa.SelectedIndex = cboKhoa.FindStringExact(sv.TenKhoa);
        nudDiemTB.Value = (decimal)Math.Min(10, Math.Max(0, sv.DiemTB));
    }

    private SinhVien ReadInput()
    {
        string tenKhoa = cboKhoa.SelectedItem is Khoa k ? k.TenKhoa : "";
        return new SinhVien(txtMaSV.Text, txtHoTen.Text, dtpNgaySinh.Value.Date,
                            (double)nudDiemTB.Value, tenKhoa);
    }

    private void ClearInput()
    {
        txtMaSV.Clear();
        txtHoTen.Clear();
        dtpNgaySinh.Value = DateTime.Today;
        cboKhoa.SelectedIndex = -1;
        nudDiemTB.Value = 0;
        txtMaSV.Focus();
    }

    private void dgvSinhVien_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvSinhVien.CurrentRow?.DataBoundItem is SinhVien sv) FillInput(sv);
    }

    private void lvSinhVien_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lvSinhVien.SelectedItems.Count > 0 && lvSinhVien.SelectedItems[0].Tag is SinhVien sv)
            FillInput(sv);
    }

    // Bài 4: chọn node con thì hiển thị chi tiết (đối tượng nằm trong Tag)
    private void tvKhoa_AfterSelect(object sender, TreeViewEventArgs e)
    {
        if (e.Node == null) return;
        if (e.Node.Tag is SinhVien sv)
        {
            FillInput(sv);
            lblChiTiet.Text =
                "Mã SV: " + sv.MaSV + "\r\n\r\n" +
                "Họ tên: " + sv.HoTen + "\r\n\r\n" +
                "Ngày sinh: " + sv.NgaySinh.ToString("dd/MM/yyyy") + "\r\n\r\n" +
                "Khoa: " + sv.TenKhoa + "\r\n\r\n" +
                "Điểm TB: " + sv.DiemTB.ToString("N2") + "\r\n\r\n" +
                "Xếp loại: " + SinhVienBUS.XepLoai(sv.DiemTB);
        }
        else
        {
            lblChiTiet.Text = "Khoa: " + e.Node.Text + "\r\n\r\n(Chọn một sinh viên để xem chi tiết)";
        }
    }

    // ===== Bài 2: CRUD (Form chỉ gọi BUS) =====
    private void btnThem_Click(object sender, EventArgs e)
    {
        try
        {
            _bus.Add(ReadInput());
            RefreshViews();
            ClearInput();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void btnSua_Click(object sender, EventArgs e)
    {
        try
        {
            _bus.Update(ReadInput());
            RefreshViews();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void btnXoa_Click(object sender, EventArgs e)
    {
        string ma = txtMaSV.Text.Trim();
        if (ma == "")
        {
            MessageBox.Show("Hãy chọn sinh viên cần xóa.");
            return;
        }
        if (MessageBox.Show("Xóa sinh viên " + ma + "?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            _bus.Delete(ma);
            RefreshViews();
            ClearInput();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // ===== Bài 9: xuất / nạp lại CSV =====
    private void btnXuatCsv_Click(object sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "sinhvien_export.csv" };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            _bus.XuatCsv(dlg.FileName);
            MessageBox.Show("Đã xuất file:\r\n" + dlg.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void btnNapCsv_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show("Nạp lại dữ liệu từ file CSV? Thay đổi chưa lưu sẽ mất.", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            _bus.NapLaiTuCsv();
            RefreshViews();
            ClearInput();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}