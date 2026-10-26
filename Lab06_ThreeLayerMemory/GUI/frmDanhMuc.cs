using Lab06_ThreeLayerMemory.BUS;
using Lab06_ThreeLayerMemory.Models;
namespace Lab06_ThreeLayerMemory.GUI;

public class frmDanhMuc : Form
{
    private readonly SanPhamBUS _bus;
    private TreeView tv;
    private ToolStripMenuItem mnuThemDM, mnuThemSP, mnuDoiTen, mnuXoa;

    public frmDanhMuc(SanPhamBUS bus)
    {
        _bus = bus;
        InitializeUI();
        _bus.DuLieuThayDoi += Bus_DuLieuThayDoi;
        FormClosed += (s, e) => _bus.DuLieuThayDoi -= Bus_DuLieuThayDoi;
    }

    private void InitializeUI()
    {
        Text = "Danh mục - Sản phẩm";
        ClientSize = new Size(480, 480);

        tv = new TreeView { Dock = DockStyle.Fill, HideSelection = false };
        var lbl = new Label
        {
            Dock = DockStyle.Top,
            Height = 30,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = "  Nhấp chuột phải vào node để thêm, đổi tên hoặc xóa."
        };
        Controls.Add(tv);     // Fill thêm trước, Top thêm sau
        Controls.Add(lbl);

        var ctx = new ContextMenuStrip();
        mnuThemDM = new ToolStripMenuItem("Thêm danh mục");
        mnuThemSP = new ToolStripMenuItem("Thêm sản phẩm");
        mnuDoiTen = new ToolStripMenuItem("Đổi tên");
        mnuXoa = new ToolStripMenuItem("Xóa");
        ctx.Items.AddRange(new ToolStripItem[] { mnuThemDM, mnuThemSP, new ToolStripSeparator(), mnuDoiTen, mnuXoa });
        tv.ContextMenuStrip = ctx;

        // Nhấp phải thì chọn node dưới con trỏ trước khi menu mở
        tv.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode n = tv.GetNodeAt(e.X, e.Y);
                if (n != null) tv.SelectedNode = n;
            }
        };
        ctx.Opening += (s, e) =>
        {
            TreeNode n = tv.SelectedNode;
            mnuThemDM.Enabled = true;
            mnuThemSP.Enabled = n != null && n.Level == 1;
            mnuDoiTen.Enabled = n != null && n.Level >= 1;
            mnuXoa.Enabled = n != null && n.Level >= 1;
        };

        mnuThemDM.Click += mnuThemDM_Click;
        mnuThemSP.Click += mnuThemSP_Click;
        mnuDoiTen.Click += mnuDoiTen_Click;
        mnuXoa.Click += mnuXoa_Click;

        LoadTree();
    }

    private void Bus_DuLieuThayDoi(object sender, EventArgs e) { LoadTree(); }

    private void LoadTree()
    {
        tv.BeginUpdate();
        tv.Nodes.Clear();
        TreeNode root = new TreeNode("Danh mục sản phẩm");
        List<SanPham> all = _bus.GetAll();
        foreach (string dm in _bus.GetDanhMuc())
        {
            TreeNode dmNode = new TreeNode(dm) { Tag = dm };         // Tag = tên danh mục
            foreach (SanPham sp in all.Where(x => x.DanhMuc == dm))
                dmNode.Nodes.Add(new TreeNode(sp.MaSP + " - " + sp.TenSP) { Tag = sp });   // Tag = SanPham
            root.Nodes.Add(dmNode);
        }
        tv.Nodes.Add(root);
        root.ExpandAll();
        tv.EndUpdate();
    }

    private void ShowError(Exception ex)
    {
        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void mnuThemDM_Click(object sender, EventArgs e)
    {
        string ten = InputBox.Show(this, "Thêm danh mục", "Tên danh mục mới:");
        if (ten == null) return;
        try { _bus.ThemDanhMuc(ten); }
        catch (Exception ex) { ShowError(ex); }
    }

    private void mnuThemSP_Click(object sender, EventArgs e)
    {
        if (tv.SelectedNode?.Tag is not string dm) return;
        string ten = InputBox.Show(this, "Thêm sản phẩm", "Tên sản phẩm mới trong \"" + dm + "\":");
        if (ten == null) return;
        try { _bus.ThemNhanh(dm, ten); }   // mã tự sinh; số lượng và đơn giá = 0, chỉnh ở Form Sản phẩm
        catch (Exception ex) { ShowError(ex); }
    }

    private void mnuDoiTen_Click(object sender, EventArgs e)
    {
        TreeNode n = tv.SelectedNode;
        if (n == null) return;
        try
        {
            if (n.Tag is string dm)
            {
                string moi = InputBox.Show(this, "Đổi tên danh mục", "Tên mới:", dm);
                if (moi != null) _bus.DoiTenDanhMuc(dm, moi);
            }
            else if (n.Tag is SanPham sp)
            {
                string moi = InputBox.Show(this, "Đổi tên sản phẩm", "Tên mới:", sp.TenSP);
                if (moi != null) _bus.DoiTenSanPham(sp.MaSP, moi);
            }
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void mnuXoa_Click(object sender, EventArgs e)
    {
        TreeNode n = tv.SelectedNode;
        if (n == null) return;
        try
        {
            if (n.Tag is string dm)
            {
                if (MessageBox.Show("Xóa danh mục \"" + dm + "\" và tất cả sản phẩm bên trong?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    _bus.XoaDanhMuc(dm);
            }
            else if (n.Tag is SanPham sp)
            {
                if (MessageBox.Show("Xóa sản phẩm \"" + sp.TenSP + "\"?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    _bus.Delete(sp.MaSP);
            }
        }
        catch (Exception ex) { ShowError(ex); }
    }
}