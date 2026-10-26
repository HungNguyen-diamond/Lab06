using Lab06_ThreeLayerMemory.BUS;
namespace Lab06_ThreeLayerMemory.GUI;

public class frmMain : Form
{
    // BUS được tạo một lần ở đây rồi truyền xuống các Form con
    private readonly SinhVienBUS _svBus = new SinhVienBUS();
    private readonly KhoaBUS _khoaBus = new KhoaBUS();
    private readonly SanPhamBUS _spBus = new SanPhamBUS();

    public frmMain()
    {
        Text = "Lab 6 - Mô hình 3 lớp (dữ liệu trong bộ nhớ)";
        IsMdiContainer = true;
        WindowState = FormWindowState.Maximized;

        var menu = new MenuStrip();
        var mnuSV = new ToolStripMenuItem("Sinh viên");
        var mnuSP = new ToolStripMenuItem("Sản phẩm");
        var mnuDM = new ToolStripMenuItem("Danh mục sản phẩm");
        var mnuThoat = new ToolStripMenuItem("Thoát");
        menu.Items.AddRange(new ToolStripItem[] { mnuSV, mnuSP, mnuDM, mnuThoat });
        MainMenuStrip = menu;
        Controls.Add(menu);

        mnuSV.Click += (s, e) => OpenChildForm(new frmSinhVien(_svBus, _khoaBus));
        mnuSP.Click += (s, e) => OpenChildForm(new frmSanPham(_spBus));
        mnuDM.Click += (s, e) => OpenChildForm(new frmDanhMuc(_spBus));
        mnuThoat.Click += (s, e) => Close();
    }

    private void OpenChildForm(Form child)
    {
        foreach (Form f in MdiChildren)
        {
            if (f.GetType() == child.GetType())
            {
                f.Activate();
                child.Dispose();
                return;
            }
        }
        child.MdiParent = this;
        child.Show();
    }
}