using Practic2020.GameCore;
using Practic2020.Players;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MafiaGraphik
{
    public partial class MainWindow : Form
    {
        private int CountMesage { get; set; }
        public Game Game { get; set; }

        private GroupBox MadeMassage(string massage = "", string autor = "")
        {
            GroupBox group = new GroupBox();
            group.BackColor = System.Drawing.SystemColors.ActiveBorder;
            group.Location = new System.Drawing.Point(12, 50 * CountMesage);
            group.Name = "groupBox1";
            group.Size = new System.Drawing.Size(349, 50);
            group.TabIndex = 0;
            group.TabStop = false;
            group.Text = "groupBox1";

            Label massageText = new Label();
            massageText.AutoSize = true;
            massageText.MaximumSize = new System.Drawing.Size(349, 200);
            massageText.Location = new System.Drawing.Point(6, 16);
            massageText.Name = "label1";
            massageText.Size = new System.Drawing.Size(35, 13);
            massageText.TabIndex = 4;
            massageText.Text = massage;

            group.Controls.Add(massageText);
            return group;
        }

        public MainWindow(Game game)
        {
            Game = game;
            CountMesage = 0;
            Line = new Queue<MessangeWindow>();
            InitializeComponent();

        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Game.Log.OnSending += AddMasage;
            foreach (RoleCounter roleCounter in Game.RoleList)
            {
                GroupBox roleBox = new GroupBox();
                roleBox.Text = roleCounter.Role.Name;
                roleBox.Size = new Size(290, 0);
                roleBox.AutoSize = true;

                foreach (Player player in roleCounter.RoleMembers)
                {
                    PlayerContayner playerContayner = new PlayerContayner(player);
                    Point pointLocation = new Point(5, 15);
                    if (roleBox.Controls.Count != 0)
                    {
                        var last = roleBox.Controls[roleBox.Controls.Count - 1];
                        pointLocation.Y = last.Location.Y + last.Size.Height + 5;
                    }

                    playerContayner.Location = pointLocation;
                    roleBox.Controls.Add(playerContayner);
                }

                Point pointLocationRole = new Point(2, 0);
                if (panel2.Controls.Count != 0)
                {
                    var last = panel2.Controls[panel2.Controls.Count - 1];
                    pointLocationRole.Y = last.Location.Y + last.Size.Height + 5;
                }
                roleBox.Location = pointLocationRole;
                panel2.Controls.Add(roleBox);
            }
        }

        private void AddMasage(string Author, string Massage){
            MessangeWindow messange = new MessangeWindow(Author, Massage);
            Line.Enqueue(messange);
            this.timer1.Enabled = true;
        }

        private Queue<MessangeWindow> Line { get; set;}

        private void PrintMessange()
        {
            if(Line.Count != 0)
            {
                var messange = Line.Dequeue();
                Point pointLocation = new Point(5, 15);
                if (messange.Text == "ведущий")
                {
                    pointLocation = new Point(80, 15);
                }
                if (panel1.Controls.Count != 0)
                {
                    var last = panel1.Controls[panel1.Controls.Count - 1];
                    pointLocation.Y = last.Location.Y + last.Size.Height + 5;
                }
                messange.Location = pointLocation;
                panel1.Controls.Add(messange);
                panel1.ScrollControlIntoView(panel1.Controls[panel1.Controls.Count - 1]);
            }
        }



        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Game.Step();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            PrintMessange();
        }
    }

    class PlayerContayner : GroupBox
    {
        private Player Player { get; set;}
        public PlayerContayner(Player player):base()
        {
            Player = player;

            this.Size = new Size(280, 30);

            this.DoubleClick += new EventHandler(OpenIndoWindow);
            this.MouseEnter += new EventHandler(Darcenst);
            this.MouseLeave += new EventHandler(Lightnes);

            Label massageText = new Label();
            massageText.AutoSize = true;
            massageText.Name = "label1";
            massageText.Size = new System.Drawing.Size(280, 30);
            massageText.Location = new Point(10, 10);
            massageText.TabIndex = 4;
            massageText.Text = Player.Name;

            this.Controls.Add(massageText);
        }

        public void Darcenst(object sender, EventArgs e)
        {
            Color palit = ((PlayerContayner)sender).BackColor;
            ((PlayerContayner)sender).BackColor = Color.FromArgb(palit.R/2, palit.G/2, palit.B/2);
        }

        public void Lightnes(object sender, EventArgs e)
        {
            Color palit = ((PlayerContayner)sender).BackColor;
            ((PlayerContayner)sender).BackColor = Color.FromArgb(palit.R * 2, palit.G * 2, palit.B * 2);
        }

        public void OpenIndoWindow(object sender, EventArgs e)
        {
            var boop = new PlayerInfo(((PlayerContayner)sender).Player);
            boop.ShowDialog();
        }
    }

    class MessangeWindow : GroupBox
    {
        public MessangeWindow(string author, string message): base(){
            this.BackColor = Color.PaleGreen;
            this.Size = new Size(400, 50);
            this.Text = author;

            Label massageText = new Label();
            massageText.AutoSize = true;
            massageText.Name = "label1";
            if(author == "ведущий")
            {
                massageText.TextAlign = ContentAlignment.MiddleRight;
                this.BackColor = Color.LightGray;
            }
            massageText.Size = new System.Drawing.Size(280, 30);
            massageText.Location = new Point(10, 15);
            massageText.TabIndex = 4;
            massageText.Text = message;

            this.Controls.Add(massageText);
        }
    }


}
