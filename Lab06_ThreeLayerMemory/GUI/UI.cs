using System;
using System.Collections.Generic;
using System.Text;

namespace Lab06_ThreeLayerMemory.GUI
{
    internal static class UI
    {
        public static Label Lbl(string t, int x, int y, int w = 85) =>
            new Label { Text = t, Left = x, Top = y + 3, Width = w, AutoSize = false };
        public static TextBox Txt(int x, int y, int w = 200) =>
            new TextBox { Left = x, Top = y, Width = w };
        public static Button Btn(string t, int x, int y, int w = 100) =>
            new Button { Text = t, Left = x, Top = y, Width = w, Height = 28 };
    }

    // Hộp thoại nhập một chuỗi; trả null nếu người dùng bấm Hủy
    internal static class InputBox
    {
        public static string Show(IWin32Window owner, string title, string prompt, string defaultValue = "")
        {
            using var f = new Form
            {
                Text = title,
                ClientSize = new Size(380, 120),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };
            var lbl = new Label { Text = prompt, Left = 15, Top = 12, Width = 350 };
            var txt = new TextBox { Left = 15, Top = 38, Width = 350, Text = defaultValue };
            var ok = new Button { Text = "OK", Left = 195, Top = 75, Width = 80, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Hủy", Left = 285, Top = 75, Width = 80, DialogResult = DialogResult.Cancel };
            f.Controls.AddRange(new Control[] { lbl, txt, ok, cancel });
            f.AcceptButton = ok;
            f.CancelButton = cancel;
            return f.ShowDialog(owner) == DialogResult.OK ? txt.Text.Trim() : null;
        }
    }
}
