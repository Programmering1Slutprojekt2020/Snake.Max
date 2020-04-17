using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace rörlig {
    public partial class Form1 : Form {

        static List<(int, int)> orm = new List<(int, int)>();
        static int riktning = 1;
        static bool pause = false;
        static Random r = new Random();
        static Point mat = new Point(2,2);
        static bool ätit = false;

        //variabler för special_mat
        static Point special_mat = new Point();
        static bool power = false;
        static int Tick_Count = 0;
        static int power_delay = 20;
        static bool power_enabled = false;
        
        public Form1() {
            InitializeComponent();
            init();
        }

        static void init() {
            orm.Add((200, 200));
            orm.Add((220, 200));
            orm.Add((240, 200));
            orm.Add((260, 200));
        }

        static void power_reset() {

            power = false;
            Tick_Count = 0;
            power_delay = r.Next(20, 200);


        }

        static void matKoordinater() {

            mat.X = r.Next(0, 30);
            mat.Y = r.Next(0, 30);

            //maten inte spawnar under ormen

            while (true) {

                for (int i = 0; i < orm.Count; i++) {

                    if (mat.X == orm.ElementAt(i).Item1 && mat.Y == orm.ElementAt(i).Item2) {

                        mat.X = r.Next(0, 30);
                        mat.Y = r.Next(0, 30);

                    }
                    else goto Brytning;
                }
            }
            Brytning:;
        }

        private void Panel1_Paint(object sender, PaintEventArgs e) {

            Graphics g = e.Graphics;
            Font style = new Font("Calibri", 20);
            SolidBrush bl = new SolidBrush(Color.Black);
            SolidBrush rd = new SolidBrush(Color.Red);
            SolidBrush gr = new SolidBrush(Color.LightGreen);

            g.FillRectangle(rd, mat.X * 20, mat.Y * 20, 20, 20); 

            if (power) g.FillRectangle(gr, special_mat.X * 20, special_mat.Y * 20, 20, 20);

            foreach ((int x, int y) in orm) { g.FillRectangle(bl, x, y, 20, 20); }
            
            if (pause) { g.DrawString("Paused", style, bl, panel1.Width/2, panel1.Height/2); }

            g.DrawString((orm.Count - 4).ToString(), style, bl, panel1.Width/2, 0);

        }



        private void Timer1_Tick(object sender, EventArgs e) {

            //orm rörelse

            if (riktning == 1) {
                orm.Insert(0, (orm.ElementAt(0).Item1 - 20, orm.ElementAt(0).Item2));
            }

            if (riktning == 2) {
                orm.Insert(0, (orm.ElementAt(0).Item1 + 20, orm.ElementAt(0).Item2));
            }

            if (riktning == 3) {
                orm.Insert(0, (orm.ElementAt(0).Item1, orm.ElementAt(0).Item2 + 20));
            }

            if (riktning == 4) {
                orm.Insert(0, (orm.ElementAt(0).Item1, orm.ElementAt(0).Item2 - 20));
            }

            if (orm.ElementAt(0).Item1 > panel1.Width) {
                orm.Insert(0, (0, orm.ElementAt(0).Item2)); orm.RemoveAt(1);
            }

            if (orm.ElementAt(0).Item2 > panel1.Height) {
                orm.Insert(0, (orm.ElementAt(0).Item1, 0)); orm.RemoveAt(1);
            }

            if (orm.ElementAt(0).Item1 < 0) {
                orm.Insert(0, (panel1.Width - 20, orm.ElementAt(0).Item2)); orm.RemoveAt(1);
            }

            if (orm.ElementAt(0).Item2 < 0) {
                orm.Insert(0, (orm.ElementAt(0).Item1, panel1.Height - 20)); orm.RemoveAt(1);
            }
            if (!ätit && riktning > 0) {
                orm.RemoveAt(orm.Count - 1);
            }
            else ätit = false;

            //orm äter

            if (orm.ElementAt(0).Item1 / 20 == mat.X && orm.ElementAt(0).Item2 / 20 == mat.Y) {

                ätit = true;

                matKoordinater();

            }

            if (orm.ElementAt(0).Item1 / 20 == special_mat.X && orm.ElementAt(0).Item2 / 20 == special_mat.Y && power) {

                timer1.Interval = 100;

                power_enabled = true;

                power_reset();


            }

            //orm dör

            for (int i = 1; i < orm.Count; i++) {

                if (orm.ElementAt(0).Item1 == orm.ElementAt(i).Item1 && orm.ElementAt(0).Item2 == orm.ElementAt(i).Item2) {

                    orm.Clear();
                    riktning = 1;
                    init();
                    matKoordinater();
                    break;

                }
            }

            //special mat

            Tick_Count++;

            if (Tick_Count >= power_delay) {

                //första tick + check för spawn

                if (!power) {

                    special_mat.X = r.Next(0, 30);
                    special_mat.Y = r.Next(0, 30);

                    while (true) {

                        for (int i = 0; i < orm.Count; i++) {

                            if ((special_mat.X == orm.ElementAt(i).Item1 && special_mat.Y == orm.ElementAt(i).Item2) || (special_mat.X == mat.X && special_mat.Y == mat.Y)) {

                                special_mat.X = r.Next(0, 30);
                                special_mat.Y = r.Next(0, 30);

                            }
                            else goto Bryt;
                        }
                    }

                Bryt:;

                }

                power = true;

            }

            if (Tick_Count >= power_delay + 20) power_reset();

            if (Tick_Count >= 30 && power_enabled) {

                timer1.Interval = 200;

                power_enabled = false;

            }
            
            panel1.Invalidate();

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {

            timer1.Start();

            if (e.KeyData == Keys.Left && riktning != 2) { riktning = 1; return; }
            if (e.KeyData == Keys.Right && riktning != 1) { riktning = 2; return; }
            if (e.KeyData == Keys.Down && riktning != 4) { riktning = 3; return; }
            if (e.KeyData == Keys.Up && riktning != 3) { riktning = 4; }
            if (e.KeyData == Keys.P) { timer1.Stop(); pause = true; panel1.Invalidate(); }
            if (e.KeyData == Keys.S) { timer1.Start(); pause = false; }

        }

        
    }
}
