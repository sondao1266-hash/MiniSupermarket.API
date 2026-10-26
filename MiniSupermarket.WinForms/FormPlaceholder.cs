using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public class FormPlaceholder : Form
    {
        public FormPlaceholder(string text)
        {
            Controls.Add(new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16f)
            });
        }
    }
}