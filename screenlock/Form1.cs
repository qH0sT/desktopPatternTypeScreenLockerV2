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
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            // ODAMIN HAYALETİSİN, SESSİZLİĞİNE AŞIĞIM.
            InitializeComponent();
            label2.BackColor = Color.Transparent;
            label3.BackColor = Color.Transparent;
            releaseMouse();
            grafik = CreateGraphics();
            stift = new Pen(Color.FromArgb(int.Parse(oku[2].Split(',')[0]),
                        int.Parse(oku[2].Split(',')[1]), int.Parse(oku[2].Split(',')[2])), 4);
            foreach(Control cntrl in Controls)
            {
                if(cntrl is PictureBox)
                {
                    cntrl.BackColor = Color.FromArgb(int.Parse(oku[1].Split(',')[0]), 
                        int.Parse(oku[1].Split(',')[1]), int.Parse(oku[1].Split(',')[2]));
                    cntrl.MouseMove += new MouseEventHandler(pictureBoxMouseMove);
                    cntrl.MouseDown += new MouseEventHandler(pcbXDown);
                    cntrl.MouseUp += new MouseEventHandler(pcbXUP);
                }
            }
            if(oku[0] != "...") { BackgroundImage = Image.FromFile(oku[0]); BackgroundImageLayout = ImageLayout.Stretch;
            } else { BackgroundImage = null; }
           
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
        /*
         Üstteki WinAPI ekrana basarak desen çizmeyi simüle etmek için. Bunu eklemesek resim kutularının MouseMove olayı Mouse'un sol tuşu
         önceden Click edilipte hale Click olayı gerçekleşiyorken bu MouseMove eveti tetiklenmiyor. Yani mouse, sol tuşu
         basılı halde resim kutularının üzerine geldiğinde resim kutusu kontrollerinin MouseMove olayı tetiklenmiyor. Ben de araştırmalarım sonucu
         bu yöntemi buldum.
         */
        bool fren = false;
        Graphics grafik = default(Graphics);
        int sira = 0;
        Pen stift = default(Pen);
        bool hinunter = false;
        string PASSWORT = string.Empty;
        string[] oku = File.ReadAllLines("data.base");
        Dictionary<string, PictureBox> aradaki = new Dictionary<string, PictureBox>();
        List<PictureBox> desenler = new List<PictureBox>();
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
                         PASSWORT += pcBxFirst.Tag.ToString() + aradaki[pcBxFirst.Name + pcBxSecond.Name].Tag + aradaki[pcBxFirst.Name + 
                           pcBxSecond.Name].Tag + pcBxSecond.Tag.ToString();
                            sira += 1;
                        }
                        catch (Exception) {
                            //label2.Text = "HATA " + ex.Message;
                            //MessageBox.Show(pcBxFirst.Name + " " + pcBxSecond.Name);
                            PASSWORT += pcBxFirst.Tag.ToString() + pcBxSecond.Tag.ToString();
                        }
                        sira += 1;
                    }
                  
                }
            }
        }      
        private void pcbXDown(object sender, MouseEventArgs args)
        {
            label1.Visible = false;
            hinunter = true;
        }
        private void pcbXUP(object sender, MouseEventArgs args)
        {

            if (desenler.Count > 0)
            {
                hinunter = false;
                grafik.Clear(SystemColors.Control);
                if (oku[0] != "...")
                {
                    BackgroundImage = Image.FromFile(oku[0]); BackgroundImageLayout = ImageLayout.Stretch;
                }
                else { BackgroundImage = null; }
                sira = 0;
                desenler.Clear();
                if (Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText("data.key"))) == PASSWORT)
                {
                    PASSWORT = "";
                    fren = true;
                    Visible = false;
                    label1.Text = "..."; label1.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    label1.Visible = true;
                    label1.Text = "Hatalı desen çizdiniz."; label1.BackColor = Color.Transparent;
                    label1.ForeColor = Color.Red;
                    PASSWORT = "";
                }
            }
        }
      
      private async void releaseMouse()
        {
            await Task.Run(async() => { while (fren == false) {
                    try
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            ReleaseCapture();
                        });
                    }
                    catch (Exception) { }
                    await Task.Delay(250); } });
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            label1.Visible = false;
            hinunter = true;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            
            if (desenler.Count > 0)
            {
                hinunter = false;
                desenler.Clear();
                grafik.Clear(SystemColors.Control);
                if (oku[0] != "...")
                {
                    BackgroundImage = Image.FromFile(oku[0]); BackgroundImageLayout = ImageLayout.Stretch;
                }
                else { BackgroundImage = null; }
                sira = 0;
                if (Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText("data.key"))) == PASSWORT)
                {
                    fren = true;
                    PASSWORT = "";
                    Visible = false;
                    label1.Text = "..."; label1.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    label1.Visible = true;
                    label1.Text = "Hatalı desen çizdiniz."; label1.BackColor = Color.Transparent;
                    label1.ForeColor = Color.Red;
                    PASSWORT = "";
                }
            }
            
        }

        private void ayarlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form2().ShowDialog();
        }

        private void kilitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            oku = File.ReadAllLines("data.base");
            if(Visible == false)
            {
                if (oku[0] != "...")
                {
                    BackgroundImage = Image.FromFile(oku[0]); BackgroundImageLayout = ImageLayout.Stretch;
                }
                else { BackgroundImage = null; }
                stift.Color = Color.FromArgb(int.Parse(oku[2].Split(',')[0]),
                        int.Parse(oku[2].Split(',')[1]), int.Parse(oku[2].Split(',')[2]));
                foreach(Control cntrl in Controls)
                {
                    if(cntrl is PictureBox) { cntrl.BackColor =
                            Color.FromArgb(int.Parse(oku[1].Split(',')[0]),
                        int.Parse(oku[1].Split(',')[1]), int.Parse(oku[1].Split(',')[2]));
                    }
                }
                fren = false;
                releaseMouse();
                Visible = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = DateTime.Now.ToString("HH:mm:ss") + " " +  
              string.Format("{0}.{1}.{2}",  DateTime.Now.Date.Day,DateTime.Now.Month,DateTime.Now.Year);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
