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

namespace Lab_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == '\b' || e.KeyChar == '-'))
                e.Handled = true;
            if (e.KeyChar == '.')
            {
                if ((sender as TextBox).Text.Contains("."))
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            if (e.KeyChar == '-')
            {
                if ((sender as TextBox).Text.Length == 0)
                    e.Handled = false;
                else

                    e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double UpBorder = 0, DownBorder = 0;
            int Tabs=0;
            try
            {
                UpBorder = double.Parse(textBoxLowerBound.Text);
                DownBorder = double.Parse(textBoxUpperBound.Text);
                Tabs = int.Parse(textBoxPoints.Text);

            }
            catch (Exception exception)
            {
                statusStrip1.Text = exception.Message;
                return;
            }
            if(radioButtonImplicit.Checked)
                FillDataTableImplicitFunction(UpBorder, DownBorder, Tabs);
            else if(radioButtonExplicit.Checked)
                FillDataTableExplicitFunction(UpBorder,DownBorder,Tabs);
            chart1.DataBind();

        }

        private void FillDataTableImplicitFunction(double upBorder, double downBorder, int tabs)
        {
            double step = (upBorder - downBorder) / (tabs - 1);
            double t,x,y;
            double max=0, min=0;
            double a = 0.1;
            dataTable.Rows.Clear();
            for (int i = 1; i < tabs+1; i++)
            {
                t=downBorder+i*step;
               
                
                    x = a / (Math.Pow(Math.Cos(t), 3));
                    y = a * Math.Pow(Math.Tan(t), 3);
               
                
                if (i==0)
                {
                    min = max = y;
                }
                if (min > y)
                    min = y;
                if (max < y)
                    max = y;
                DataRow dr= dataTable.NewRow();
                dr["t"] = t;
                dr["x"] = x;
                dr ["y"] = y;
                a += 0.00000001;
                dataTable.Rows.Add(dr);
            }
            labelMin.Text =Math.Round(min,2).ToString();
            labelMax.Text =Math.Round(max,2).ToString();

        }

        private void FillDataTableExplicitFunction(double upBorder, double downBorder,int tabs)
        {
            
            double x, y;
            double step = (upBorder - downBorder) / (tabs - 1);
            double max = 0, min = 0;
            dataTable.Rows.Clear();
            for (int i = 0; i < tabs; i++)
            {
                
                x = downBorder+i*step;
                y = Math.Acos((1-x)/(1-2*x));
                //y = Math.Acos(x);
                if (i == 0)
                {
                    min = max = y;
                }
                if (min > y)
                    min = y;
                if (max < y)
                    max = y;
                DataRow dr = dataTable.NewRow();
                
                dr["x"] = x;
                dr["y"] = y;
                dataTable.Rows.Add(dr);
            }

            labelMin.Text = Math.Round(min, 2).ToString();
            labelMax.Text = Math.Round(max, 2).ToString();
        }

        private void radioButtonExplicit_CheckedChanged(object sender, EventArgs e)
        {
            if((sender as RadioButton).Checked)
                columnT.Visible = false;
               
        }

        private void radioButtonImplicit_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
                columnT.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButtonExplicit.Checked = true;
            float fontSize;
            string fontName;
            XDocument xDocument = XDocument.Load("config.xml");
            BackColor = Color.FromName(xDocument.Root.Element("BackColor").Value);
            ForeColor = Color.FromName(xDocument.Root.Element("Font").Element("FontColor").Value);
            fontSize = float.Parse(xDocument.Root.Element("Font").Element("FontSize").Value);
            fontName = xDocument.Root.Element("Font").Element("FontType").Value;
            this.Font = new Font(new FontFamily(fontName), fontSize);
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true;
            fontDialog.Font = Font;
            fontDialog.Color = ForeColor;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                this.Font = fontDialog.Font;
                this.ForeColor = fontDialog.Color;
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                this.BackColor = colorDialog.Color;
        }

        private void defaultValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XDocument xDocument = XDocument.Load("config.xml");
            textBoxLowerBound.Text = xDocument.Root.Element("Data").Element("LowerBound").Value;
            textBoxUpperBound.Text = xDocument.Root.Element("Data").Element("UpperBound").Value;
            textBoxPoints.Text = xDocument.Root.Element("Data").Element("Points").Value;
        }

        private void Form1_FontChanged(object sender, EventArgs e)
        {
            foreach (Control item in Controls)
            {
                item.Font = Font;

                foreach (Control item2 in item.Controls)
                {
                    item2.Font = Font;
                }
            }
        }

        private void Form1_ForeColorChanged(object sender, EventArgs e)
        {
            foreach (Control item in Controls)
            {

                item.ForeColor = ForeColor;
                colorToolStripMenuItem.ForeColor = fontToolStripMenuItem.ForeColor = ForeColor;
                foreach (Control item2 in item.Controls)
                {

                    item2.ForeColor = ForeColor;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            XDocument xDocument = XDocument.Load("config.xml");
            xDocument.Root.Element("BackColor").Value = BackColor.Name;
            xDocument.Root.Element("Font").Element("FontColor").Value = ForeColor.Name;
            xDocument.Root.Element("Font").Element("FontType").Value = Font.FontFamily.Name;
            xDocument.Root.Element("Font").Element("FontSize").Value = Font.Size.ToString();
            xDocument.Root.Element("Data").Element("LowerBound").Value = textBoxLowerBound.Text;
            xDocument.Root.Element("Data").Element("UpperBound").Value = textBoxUpperBound.Text;
            xDocument.Root.Element("Data").Element("Points").Value = textBoxPoints.Text;

            xDocument.Save("config.xml");
        }
    }
}

