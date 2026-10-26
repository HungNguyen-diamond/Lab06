using System.ComponentModel;
using Lab06_ThreeLayerMemory.BUS;
using Lab06_ThreeLayerMemory.Models;
namespace Lab06_ThreeLayerMemory.GUI;

public class frmSanPham : Form
{
    private readonly SanPhamBUS _bus;
    private bool _dangNap = false;   // chặn việc tự điền ô nhập khi đang nạp lại lưới

    private TextBox txtMaSP, txtTenSP, txtTim;
    private ComboBox cboDanhMuc, cboSapXep, cboThuTu;
    private NumericUpDown nudSoLuong, nudDonGia;
    private DataGridView dgvSanPham;
    private Label lblTong;

    public frmSanPham(SanPhamBUS bus)
    {
        _bus = bus;
        InitializeUI();
        _bus.DuLieuThayDoi += Bus_DuLieuThayDoi;
        FormClosed += (s, e) => _bus.DuLieuThayDoi -= Bus_DuLieuThayDoi;
    }

    private void InitializeUI()
    {
        Text = "Quản lý sản phẩm";
        ClientSize = new Size(860, 560);

        txtMaSP = UI.Txt(110, 15, 150);
        txtTenSP = UI.Txt(110, 50, 250);
        nudDonGia = new NumericUpDown
        {
            Left = 110,
            Top = 85,
            Width = 150,
            Minimum = 0,
            Maximum = 10000000000M,
            DecimalPlaces = 0,
            Increment = 1000,
            ThousandsSeparator = true
        };
        cboDanhMuc = new ComboBox { Left = 480, Top = 15, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        nudSoLuong = new NumericUpDown { Left = 480, Top = 50, Width = 100, Minimum = 0, Maximum = 1000000 };

        var btnThem = UI.Btn("Thêm", 20, 125);
        var btnSua = UI.Btn("Sửa", 125, 125);
        var btnXoa = UI.Btn("Xóa", 230, 125);
        var btnLamMoi = UI.Btn("Làm mới", 335, 125);

        txtTim = UI.Txt(110, 165, 200);
        cboSapXep = new ComboBox { Left = 425, Top = 165, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
        cboSapXep.Items.AddRange(SanPhamBUS.TruongSapXep);
        cboSapXep.SelectedIndex = 0;
        cboThuTu = new ComboBox { Left = 555, Top = 165, Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
        cboThuTu.Items.AddRange(new object[] { "Tăng dần", "Giảm dần" });
        cboThuTu.SelectedIndex = 0;

        dgvSanPham = new DataGridView
        {
            Left = 10,
            Top = 200,
            Width = 840,
            Height = 310,
            AutoGenerateColumns = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        var phai = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight };
        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSP", HeaderText = "Mã SP", FillWeight = 12 });
        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenSP", HeaderText = "Tên sản phẩm", FillWeight = 28 });
        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DanhMuc", HeaderText = "Danh mục", FillWeight = 16 });
        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoLuong", HeaderText = "Số lượng", FillWeight = 10, DefaultCellStyle = phai });
        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DonGia", HeaderText = "Đơn giá", FillWeight = 16, DefaultCellStyle = phai });
        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ThanhTien", HeaderText = "Thành tiền", FillWeight = 18, DefaultCellStyle = phai });

        lblTong = new Label
        {
            Left = 20,
            Top = 520,
            Width = 820,
            Height = 25,
            Font = new Font(Font, FontStyle.Bold),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };

        Controls.AddRange(new Control[] {
            UI.Lbl("Mã SP", 20, 15), txtMaSP,
            UI.Lbl("Tên SP", 20, 50), txtTenSP,
            UI.Lbl("Đơn giá", 20, 85), nudDonGia,
            UI.Lbl("Danh mục", 380, 15, 95), cboDanhMuc,
            UI.Lbl("Số lượng", 400, 50, 75), nudSoLuong,
            btnThem, btnSua, btnXoa, btnLamMoi,
            UI.Lbl("Tìm kiếm", 20, 165), txtTim,
            UI.Lbl("Sắp xếp theo", 320, 165, 105), cboSapXep, cboThuTu,
            dgvSanPham, lblTong });

        btnThem.Click += btnThem_Click;
        btnSua.Click += btnSua_Click;
        btnXoa.Click += btnXoa_Click;
        btnLamMoi.Click += (s, e) => ClearInput();
        txtTim.TextChanged += (s, e) => Reload();
        cboSapXep.SelectedIndexChanged += (s, e) => Reload();
        cboThuTu.SelectedIndexChanged += (s, e) => Reload();
        dgvSanPham.SelectionChanged += dgvSanPham_SelectionChanged;

        NapDanhMuc();
        Reload();
    }

    private void Bus_DuLieuThayDoi(object sender, EventArgs e)
    {
        NapDanhMuc();
        Reload();
    }

    private void NapDanhMuc()
    {
        string cu = cboDanhMuc.Text;
        cboDanhMuc.DataSource = _bus.GetDanhMuc();
        cboDanhMuc.SelectedIndex = cboDanhMuc.FindStringExact(cu);
    }

    // Tìm kiếm + sắp xếp + tính tổng đều do BUS làm; Form chỉ hiển thị
    private void Reload()
    {
        _dangNap = true;
        List<SanPham> ds = _bus.Query(txtTim.Text, cboSapXep.Text, cboThuTu.SelectedIndex == 0);
        dgvSanPham.DataSource = new BindingList<SanPham>(ds);
        lblTong.Text = "Tổng giá trị hàng tồn: " + _bus.TongGiaTriTon(ds).ToString("N0")
                       + " đ  (" + ds.Count + " sản phẩm đang hiển thị)";
        _dangNap = false;
    }

    private SanPham ReadInput()
    {
        return new SanPham(txtMaSP.Text, txtTenSP.Text, cboDanhMuc.Text,
                           (int)nudSoLuong.Value, nudDonGia.Value);
    }

    private void FillInput(SanPham sp)
    {
        txtMaSP.Text = sp.MaSP;
        txtTenSP.Text = sp.TenSP;
        cboDanhMuc.SelectedIndex = cboDanhMuc.FindStringExact(sp.DanhMuc);
        nudSoLuong.Value = Math.Min(nudSoLuong.Maximum, sp.SoLuong);
        nudDonGia.Value = Math.Min(nudDonGia.Maximum, sp.DonGia);
    }

    private void ClearInput()
    {
        txtMaSP.Clear();
        txtTenSP.Clear();
        cboDanhMuc.SelectedIndex = -1;
        nudSoLuong.Value = 0;
        nudDonGia.Value = 0;
        txtMaSP.Focus();
    }

    private void dgvSanPham_SelectionChanged(object sender, EventArgs e)
    {
        if (_dangNap) return;
        if (dgvSanPham.CurrentRow?.DataBoundItem is SanPham sp) FillInput(sp);
    }

    private void btnThem_Click(object sender, EventArgs e)
    {
        try { _bus.Add(ReadInput()); ClearInput(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void btnSua_Click(object sender, EventArgs e)
    {
        try { _bus.Update(ReadInput()); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void btnXoa_Click(object sender, EventArgs e)
    {
        string ma = txtMaSP.Text.Trim();
        if (ma == "") { MessageBox.Show("Hãy chọn sản phẩm cần xóa."); return; }
        if (MessageBox.Show("Xóa sản phẩm " + ma + "?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try { _bus.Delete(ma); ClearInput(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}