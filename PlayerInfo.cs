using Practic2020.Players;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MafiaGraphik
{
    public partial class PlayerInfo : Form
    {
        private Player Player { get; set;}
        public PlayerInfo(Player player)
        {
            Player = player;
            InitializeComponent();
        }

        private void PlayerInfo_Load(object sender, EventArgs e)
        {
            label32.Text = Player.IsLive ? "жив" : "мертв";
            label1.Text = Player.Name + " (" + Player.Role.Name + ")" ;
            label24.Text = Player.Tric.ToString();
            label22.Text = Player.Сandor.ToString();
            label23.Text = Player.Conviction.ToString();
            label25.Text = Player.Trust.ToString();
            label26.Text = Player.Attac.ToString();
            label27.Text = Player.Protect.ToString();
            label28.Text = Player.MemoryAbility.ToString();
            label29.Text = Player.Smart.ToString();
            label30.Text = Player.Skepticizm.ToString();
            label31.Text = Player.Agression.ToString();

            foreach (Opinion opinion in Player.Memory.Opinions) {

                Point pointLocation= new Point(10, 20);
                if (panel1.Controls.Count != 0)
                {
                    var last = panel1.Controls[panel1.Controls.Count - 1];
                    pointLocation.Y = last.Location.Y + last.Size.Height + 15;
                }
                PlayerRoleGroup roleGroup = new PlayerRoleGroup(opinion);
                roleGroup.Location = pointLocation;
                panel1.Controls.Add(roleGroup);
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }

    public class PlayerRoleGroup : GroupBox
    {

        public PlayerRoleGroup(Opinion opinion):base()
        {
            this.Size = new Size(500, 70);
            this.Text = opinion.Player.Name;

            foreach (Guess guess in opinion.Guesses) {
                Point pointLocation = new Point(10, 15);
                if (this.Controls.Count != 0)
                {
                    var last = this.Controls[this.Controls.Count - 1];
                    pointLocation.X = last.Location.X + last.Size.Width + 10;
                }
                PlayerGuessGroup guessGroup = new PlayerGuessGroup(guess);
                guessGroup.Location = pointLocation;
                this.Controls.Add(guessGroup);
            }
        }
    }

    public class PlayerGuessGroup : GroupBox
    {
        public PlayerGuessGroup(Guess guess):base()
        {
            this.Size = new Size(80, 40);
            this.Text = guess.Role.Name;
            Label massageText = new Label();
            massageText.AutoSize = true;
            massageText.Name = "label1";
            massageText.Size = new System.Drawing.Size(50, 40);
            massageText.Location = new Point(10, 20);
            massageText.TabIndex = 4;
            massageText.Text = Math.Round((guess.Chance * 100), 2).ToString() + "%";
            this.Controls.Add(massageText);
        }
    }


}
