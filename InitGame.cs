using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Practic2020.GameCore;
using Practic2020.Players;
using Practic2020.Roles;

namespace MafiaGraphik
{
    public partial class InitGame : Form
    {
        private Game Game { get; set; }
            
        public InitGame(Game game)
        {
            Game = game;
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case (0):
                    Game.AddRole(new Mafia());
                    break;
                case (1):
                    Game.AddRole(new Peacfool());
                    break;
                case (2):
                    Game.AddRole(new Detective());
                    break;
                case (3):
                    Game.AddRole(new Manaic());
                    break;
            }

            if (checkBox1.Checked)
            {
                Game.AddPlayer(Player.Random("Игрок №" + Game.Players.Count), AddPlayerMode.soft);
                refresh();
            }
            else{
                var boop = new ChearacterDisign(Game);
                boop.ShowDialog();
                refresh();
            }
        }

        private void refresh()
        {
            dataGridView1.Rows.Clear();
            foreach(Player player in Game.Players)
            {
                dataGridView1.Rows.Add(player.Name, player.Role.Name);
            }
        }
        private void InitGame_Load(object sender, EventArgs e)
        {

            comboBox1.SelectedIndex = 0;

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Game.StratByStep();
            var boop = new MainWindow(Game);
            boop.ShowDialog();
        }
    }
}
