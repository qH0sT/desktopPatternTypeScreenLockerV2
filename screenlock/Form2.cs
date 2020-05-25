using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace screenlock
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            textBox1.Text = oku[0];
            pictureBox10.BackColor = Color.FromArgb(int.Parse(oku[1].Split(',')[0]),
                        int.Parse(oku[1].Split(',')[1]), int.Parse(oku[2].Split(',')[2]));

            pictureBox11.BackColor = Color.FromArgb(int.Parse(oku[2].Split(',')[0]),
                        int.Parse(oku[2].Split(',')[1]), int.Parse(oku[2].Split(',')[2]));
            releaseMouse();
            MouseDown += new MouseEventHandler(Form1_MouseDown);
            MouseUp += new MouseEventHandler(Form1_MouseUp);
            grafik = CreateGraphics();
            stift = new Pen(Color.Black, 4);
            foreach (Control cntrl in Controls)
            {
                if (cntrl is PictureBox)
                {
                    cntrl.MouseMove += new MouseEventHandler(pictureBoxMouseMove);
                    cntrl.MouseDown += new MouseEventHandler(pcbXDown);
                    cntrl.MouseUp += new MouseEventHandler(pcbXUP);
                }
            }
            aradaki.Add("pictureBox1pictureBox9", pictureBox6);
            aradaki.Add("pictureBox9pictureBox1", pictureBox6);
            aradaki.Add("pictureBox2pictureBox8", pictureBox5);
            aradaki.Add("pictureBox8pictureBox2", pictureBox5);
            aradaki.Add("pictureBox3pictureBox7", pictureBox4);
            aradaki.Add("pictureBox7pictureBox3", pictureBox4);
            aradaki.Add("pictureBox1pictureBox3", pictureBox2);
            aradaki.Add("pictureBox3pictureBox1", pictureBox2);
            aradaki.Add("pictureBox4pictureBox6", pictureBox5);
            aradaki.Add("pictureBox6pictureBox4", pictureBox5);
            aradaki.Add("pictureBox7pictureBox9", pictureBox8);
            aradaki.Add("pictureBox9pictureBox7", pictureBox8);
            aradaki.Add("pictureBox1pictureBox7", pictureBox5);
            aradaki.Add("pictureBox7pictureBox1", pictureBox5);
            aradaki.Add("pictureBox3pictureBox9", pictureBox5);
            aradaki.Add("pictureBox9pictureBox3", pictureBox5);
        }
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        bool tekrarla = false;
        Graphics grafik = default(Graphics);
        int sira = 0;
        Pen stift = default(Pen);
        bool hinunter = false;
        string PASSWORT = string.Empty;
        List<PictureBox> desenler = new List<PictureBox>();
        Dictionary<string, PictureBox> aradaki = new Dictionary<string, PictureBox>();
        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(PASSWORT);
            File.WriteAllText("data.key", Convert.ToBase64String(Encoding.UTF8.GetBytes(PASSWORT)));
            MessageBox.Show("YENİ ŞİFRE KAYDEDİLDİ.","THT SCREEN LOCKER",MessageBoxButtons.OK,MessageBoxIcon.Information);
            Close();
        }
        private void pictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (hinunter)
            {
                PictureBox baks = sender as PictureBox;
                if (desenler.Contains(baks) == false)
                {
                    desenler.Add(baks);
                    if (desenler.Count > 1)
                    {
                        PictureBox pcBxFirst = desenler[sira];
                        PictureBox pcBxSecond = desenler[sira + 1];
                        Point p1 = new Point(pcBxFirst.Location.X, pcBxFirst.Location.Y);
                        Point p2 = new Point(pcBxSecond.Location.X, pcBxSecond.Location.Y);
                        grafik.DrawLine(stift, p1, p2);
                        try
                        {
                            desenler.Add(aradaki[pcBxFirst.Name + pcBxSecond.Name]);
                            PictureBox pcTemp = desenler[desenler.Count - 2]; //Ana pc   p p p
                            desenler[desenler.Count - 1] = pcTemp;
                            desenler[desenler.Count - 2] = aradaki[pcBxFirst.Name + pcBxSecond.Name];
                            //label2.Text = desenler[desenler.Count - 1].Name;
                            if (tekrarla == false)
                            {
                                PASSWORT += pcBxFirst.Tag.ToString() + aradaki[pcBxFirst.Name + pcBxSecond.Name].Tag + aradaki[pcBxFirst.Name +
                                  pcBxSecond.Name].Tag + pcBxSecond.Tag.ToString();
                            }
                            else { temp += pcBxFirst.Tag.ToString() + aradaki[pcBxFirst.Name + pcBxSecond.Name].Tag + aradaki[pcBxFirst.Name +
                                  pcBxSecond.Name].Tag + pcBxSecond.Tag.ToString();
                            }
                            sira += 1;
                        }
                        catch (Exception)
                        {
                            //label2.Text = "HATA " + ex.Message;
                            //MessageBox.Show(pcBxFirst.Name + " " + pcBxSecond.Name);
                            if (tekrarla == false)
                            {
                                PASSWORT += pcBxFirst.Tag.ToString() + pcBxSecond.Tag.ToString();
                            }
                            else { temp += pcBxFirst.Tag.ToString() + pcBxSecond.Tag.ToString(); }
                        }
                        sira += 1;
                    }

                }
            }
        }
        private void pcbXDown(object sender, MouseEventArgs args)
        {
            hinunter = true;
        }
        private void pcbXUP(object sender, MouseEventArgs args)
        {
            if (desenler.Count > 0)
            {
                hinunter = false;
                if (tekrarla == false)
                {
                    grafik.Clear(SystemColors.Control);
                    tekrarla = true;
                    desenler.Clear();
                    label1.Text = "Şifrenizi doğrulayın";
                    sira = 0;
                }
                else
                {
                    if (string.IsNullOrEmpty(temp) == false && string.IsNullOrEmpty(PASSWORT) == false)
                    {
                        if (temp == PASSWORT)
                        {
                            button1.Enabled = true;
                            label1.Text = "Şifreler uyumlu.";
                        }
                        else { label1.Text = "Şifreler yanlış. Tekrar deneyin.";
                            temp = ""; PASSWORT = "";
                            grafik.Clear(SystemColors.Control);
                            tekrarla = false;
                            desenler.Clear();
                            sira = 0;
                        }
                    }
                }
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            hinunter = true;
        }
        string temp = "";
        private async void releaseMouse()
        {
            await Task.Run(async () => {
                while (true)
                {
                    try
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            ReleaseCapture();
                        });                      
                    }
                    catch (Exception) { }
                    await Task.Delay(250);
                }
            });
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (desenler.Count > 0)
            {
                hinunter = false;
                if (tekrarla == false)
                {
                    grafik.Clear(SystemColors.Control);
                    tekrarla = true;
                    desenler.Clear();
                    label1.Text = "Şifrenizi doğrulayın";
                    sira = 0;
                }
                else
                {
                    if (string.IsNullOrEmpty(temp) == false && string.IsNullOrEmpty(PASSWORT) == false)
                    {
                        if (temp == PASSWORT)
                        {
                            button1.Enabled = true;
                            label1.Text = "Şifreler uyumlu.";
                        }
                        else { label1.Text = "Şifreler yanlış. Tekrar deneyin.";
                            temp = ""; PASSWORT = "";
                            grafik.Clear(SystemColors.Control);
                            tekrarla = false;
                            desenler.Clear();
                            sira = 0;
                        }
                    }
                }
            }
        }
        string[] oku = File.ReadAllLines("data.base");
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Kilit ekranı duvar kağıdını seçin";
            op.Multiselect = false;
            op.Filter = "Resim Dosyaları |*.jpeg; *.jpg; *.png";
            if(op.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = op.FileName;
                oku[0] = op.FileName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            oku[0] = "...";
            textBox1.Text = "...";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if(cd.ShowDialog() == DialogResult.OK)
            {
                pictureBox10.BackColor = cd.Color;
                string argb = "";
                argb = cd.Color.R.ToString() + "," + cd.Color.G + "," + cd.Color.B;
                oku[1] = argb;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pictureBox11.BackColor = cd.Color;
                string argb = "";
                argb = cd.Color.R.ToString() + "," + cd.Color.G + "," + cd.Color.B;
                oku[2] = argb;
                
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            File.WriteAllLines("data.base", oku);
        }
    }
}
