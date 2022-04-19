using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Practic2020.Players;
using Practic2020.GameCore;

namespace MafiaGraphik
{
    public partial class ChearacterDisign : Form
    {
        private Game Game { get; set; }
        private Player Player { get; set; } 
        public int Maxpoint { get; set;}

        private bool NewPlayer { get; set; }
        public ChearacterDisign(Game game, Player player = null)
        {
            if(player == null)
            {
                NewPlayer = true;
                player = new Player();
            }
            Game = game;
            Player = player;
            Maxpoint = 80;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Player.Name = textBox1.Text;
            Player.Tric = trackBar1.Value;
            Player.Сandor = trackBar2.Value;
            Player.Conviction = trackBar3.Value;
            Player.Trust = trackBar4.Value;
            Player.Attac= trackBar5.Value;
            Player.Protect = trackBar6.Value;
            Player.MemoryAbility = trackBar7.Value;
            Player.Intelegence = trackBar8.Value;
            Player.Skepticizm = trackBar9.Value;
            Player.Agression = trackBar10.Value;

            Game.AddPlayer(Player, AddPlayerMode.soft);
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label24.Text = trackBar1.Value.ToString();
            label25.Text = trackBar2.Value.ToString();
            label26.Text = trackBar3.Value.ToString();
            label27.Text = trackBar4.Value.ToString();
            label28.Text = trackBar5.Value.ToString();
            label29.Text = trackBar6.Value.ToString();
            label30.Text = trackBar7.Value.ToString();

            Maxpoint = 80;
            Maxpoint -= trackBar1.Value;
            Maxpoint -= trackBar2.Value;
            Maxpoint -= trackBar3.Value;
            Maxpoint -= trackBar4.Value;
            Maxpoint -= trackBar5.Value;
            Maxpoint -= trackBar6.Value;
            Maxpoint -= trackBar7.Value;

            
            trackBar1.Maximum = Maxpoint - 20 < 0 ? trackBar1.Value + Maxpoint > 20 ? 20 : trackBar1.Value + Maxpoint : 20;
            trackBar2.Maximum = Maxpoint - 20 < 0 ? trackBar2.Value + Maxpoint > 20 ? 20 : trackBar2.Value + Maxpoint : 20;
            trackBar3.Maximum = Maxpoint - 20 < 0 ? trackBar3.Value + Maxpoint > 20 ? 20 : trackBar3.Value + Maxpoint : 20;
            trackBar4.Maximum = Maxpoint - 20 < 0 ? trackBar4.Value + Maxpoint > 20 ? 20 : trackBar4.Value + Maxpoint : 20;
            trackBar5.Maximum = Maxpoint - 20 < 0 ? trackBar5.Value + Maxpoint > 20 ? 20 : trackBar5.Value + Maxpoint : 20;
            trackBar6.Maximum = Maxpoint - 20 < 0 ? trackBar6.Value + Maxpoint > 20 ? 20 : trackBar6.Value + Maxpoint : 20;
            trackBar7.Maximum = Maxpoint - 20 < 0 ? trackBar7.Value + Maxpoint > 20 ? 20 : trackBar7.Value + Maxpoint : 20;

            label23.Text = Maxpoint.ToString();
        }

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            label31.Text = trackBar8.Value.ToString();
            label32.Text = trackBar9.Value.ToString();
            label33.Text = trackBar10.Value.ToString();
        }
    }
}
