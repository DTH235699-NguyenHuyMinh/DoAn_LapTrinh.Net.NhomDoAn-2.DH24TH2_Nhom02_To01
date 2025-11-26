using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DangNhap
{
    public class AutoScaler
    {
        private Dictionary<Control, Rectangle> originalBounds = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, float> originalFontSizes = new Dictionary<Control, float>();
        private Size originalFormSize;

        private Form targetForm;

        public AutoScaler(Form form)
        {
            targetForm = form;
            targetForm.Load += Form_Load;
            targetForm.Resize += Form_Resize;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            originalFormSize = targetForm.Size;
            SaveInitialBounds(targetForm);
        }

        private void SaveInitialBounds(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                originalBounds[c] = new Rectangle(c.Left, c.Top, c.Width, c.Height);
                originalFontSizes[c] = c.Font.Size;

                if (c.Controls.Count > 0)
                    SaveInitialBounds(c);
            }
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (originalFormSize.Width == 0 || originalFormSize.Height == 0)
                return;

            float xRatio = (float)targetForm.Width / originalFormSize.Width;
            float yRatio = (float)targetForm.Height / originalFormSize.Height;

            ResizeControls(targetForm, xRatio, yRatio);
        }

        private void ResizeControls(Control parent, float xRatio, float yRatio)
        {
            foreach (Control c in parent.Controls)
            {
                if (originalBounds.ContainsKey(c))
                {
                    Rectangle rect = originalBounds[c];

                    int newX = (int)(rect.X * xRatio);
                    int newY = (int)(rect.Y * yRatio);
                    int newW = (int)(rect.Width * xRatio);
                    int newH = (int)(rect.Height * yRatio);

                    c.SetBounds(newX, newY, newW, newH);

                    float newFontSize = originalFontSizes[c] * ((xRatio + yRatio) / 2f);
                    c.Font = new Font(c.Font.FontFamily, newFontSize, c.Font.Style);
                }

                if (c.Controls.Count > 0)
                    ResizeControls(c, xRatio, yRatio);
            }
        }
    }
}

