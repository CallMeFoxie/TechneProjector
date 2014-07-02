using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TechneProjector
{
    public partial class form : Form
    {

        public int[] getInts(String value)
        {
            int[] ret;

            String[] parts = value.Split(',');

            ret = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                ret[i] = Convert.ToInt32(parts[i]);
            }

            return ret;

        }

        private String input;
        private String output;

        public form()
        {
            InitializeComponent();
        }

        private void btnSelectInput_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Techne XML files|*.xml";
            ofd.ValidateNames = true;
            ofd.CheckFileExists = true;
            DialogResult d = ofd.ShowDialog();
            if (d == System.Windows.Forms.DialogResult.OK)
            {
                input = ofd.FileName;
                lblInput.Text = ofd.FileName;
            }
        }

        private void btnSelectOutput_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Bitmaps|*.bmp";
            sfd.ValidateNames = true;
            sfd.DefaultExt = ".bmp";
            DialogResult d = sfd.ShowDialog();
            if (d == System.Windows.Forms.DialogResult.OK)
            {
                output = sfd.FileName.Substring(0, sfd.FileName.Length - 4);
                lblOutput.Text = sfd.FileName;
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (input == "" || output == "")
            {
                MessageBox.Show("Select inputs and outputs first!");
                return;
            }

            try
            {
                XDocument xdoc = XDocument.Load(input);
                Bitmap bmp = null;
                Bitmap bmp2 = null;
                int multiplier = 10;


                var sizedata = from x in xdoc.Descendants("Models").Descendants("Model")
                               select x;

                foreach (var size in sizedata)
                {
                    int[] sizes = getInts(size.Element("TextureSize").Value);

                    bmp = new Bitmap(sizes[0], sizes[1]);
                    bmp2 = new Bitmap(sizes[0] * multiplier, sizes[1] * multiplier);
                }

                Graphics g = Graphics.FromImage(bmp);
                Graphics g2 = Graphics.FromImage(bmp2);

                var data = from lv1 in xdoc.Descendants("Models").Descendants("Model").Descendants("Shape")
                           select lv1;

                Font f = new Font(FontFamily.GenericMonospace, 8.0f);



                foreach (var d in data)
                {
                    int[] sizes = getInts(d.Element("Size").Value);
                    int[] offset = getInts(d.Element("TextureOffset").Value);
                    String label = d.Attribute("name").Value;

                    int realWidth = sizes[0] * 2 + sizes[1] * 2;
                    int realHeight = sizes[2];

                    //  ----- TOP BOT -------
                    // SID  FRONT   SID  BACK

                    int offsetLeft = sizes[2] + offset[0];
                    int offsetTop = sizes[2] + offset[1];

                    // draw top and bottom
                    g.FillRectangle(new SolidBrush(Color.DarkGreen), offsetLeft, offset[1], sizes[0], sizes[2]);
                    g2.FillRectangle(new SolidBrush(Color.DarkGreen), offsetLeft * multiplier, offset[1] * multiplier, sizes[0] * multiplier, sizes[2] * multiplier);
                    g2.DrawString(label + "\r\n" + "T", f, new SolidBrush(Color.White), offsetLeft * multiplier, offset[1] * multiplier);

                    g.FillRectangle(new SolidBrush(Color.Green), offsetLeft + sizes[0], offset[1], sizes[0], sizes[2]);
                    g2.FillRectangle(new SolidBrush(Color.Green), multiplier * (offsetLeft + sizes[0]), offset[1] * multiplier, sizes[0] * multiplier, sizes[2] * multiplier);
                    g2.DrawString(label + "\r\n" + "B", f, new SolidBrush(Color.White), multiplier * (offsetLeft + sizes[0]), offset[1] * multiplier);

                    // draw SIDE
                    g.FillRectangle(new SolidBrush(Color.DarkRed), offset[0], offsetTop, sizes[2], sizes[1]);
                    g2.FillRectangle(new SolidBrush(Color.DarkRed), multiplier * offset[0], multiplier * offsetTop, multiplier * sizes[2], multiplier * sizes[1]);
                    g2.DrawString(label + "\r\n" + "L", f, new SolidBrush(Color.White), multiplier * offset[0], multiplier * offsetTop);
                    // draw FRONT
                    g.FillRectangle(new SolidBrush(Color.DarkBlue), offset[0] + sizes[2], offsetTop, sizes[0], sizes[1]);
                    g2.FillRectangle(new SolidBrush(Color.DarkBlue), multiplier * (offset[0] + sizes[2]), multiplier * offsetTop, multiplier * sizes[0], multiplier * sizes[1]);
                    g2.DrawString(label + "\r\n" + "F", f, new SolidBrush(Color.White), multiplier * (offset[0] + sizes[2]), multiplier * offsetTop);
                    // draw SIDE
                    g.FillRectangle(new SolidBrush(Color.Red), offset[0] + sizes[2] + sizes[0], offsetTop, sizes[2], sizes[1]);
                    g2.FillRectangle(new SolidBrush(Color.Red), multiplier * (offset[0] + sizes[2] + sizes[0]), multiplier * offsetTop, multiplier * sizes[2], multiplier * sizes[1]);
                    g2.DrawString(label + "\r\n" + "R", f, new SolidBrush(Color.White), multiplier * (offset[0] + sizes[2] + sizes[0]), multiplier * offsetTop);
                    // draw BACK
                    g.FillRectangle(new SolidBrush(Color.Blue), offset[0] + sizes[2] * 2 + sizes[0], offsetTop, sizes[0], sizes[1]);
                    g2.FillRectangle(new SolidBrush(Color.Blue), multiplier * (offset[0] + sizes[2] * 2 + sizes[0]), multiplier * offsetTop, multiplier * sizes[0], multiplier * sizes[1]);
                    g2.DrawString(label + "\r\n" + "K", f, new SolidBrush(Color.White), multiplier * (offset[0] + sizes[2] * 2 + sizes[0]), multiplier * offsetTop);

                    //break;
                }

                bmp.Save(output + ".bmp");
                bmp2.Save(output + "_big.bmp");
                MessageBox.Show("Done.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while processing: \r\n" + ex.Message);
            }
        }
    }
}
