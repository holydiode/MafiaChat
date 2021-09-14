using Practic2020.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Practic2020.GameCore
{
    /// <summary>
    /// класс счётчик - считает количество голосов отданых за каждую роль.
    /// </summary>
    class VoteCounter : IComparable<VoteCounter>
    {   
        /// <summary>
        /// игрок за которого голосовали
        /// </summary>
        public Player Player { get; set; }
        /// <summary>
        /// количество голосов за игрока
        /// </summary>
        private int Count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public VoteCounter(Player player) {
            Player = player;
        }
        public int CompareTo(VoteCounter voteCounter) {
            if(Count == voteCounter.Count)
            {
                return 0;
            }
            if(Count > voteCounter.Count)
            {
                return 1;
            }
            return -1;
        }
        //увеличить счетчтчик голосов на 1
        public void Up(int incr = 1)
        {
            Count += incr;
        }
    }
    /// <summary>
    /// абстрактный класс голосования, выполняющий роль сбора голосов игроков, и выполнения приговоров
    /// </summary>
    class Vote
    {
        /// <summary>
        /// Список всех голосов в голосовании
        /// </summary>
        private List<VoteCounter> Votes { get; set;}
        /// <summary>
        /// название голосования
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Игра в которй происходит голосование
        /// </summary>
        public Game Game { get; set;}
        /// <summary>
        /// Создание нового голосование
        /// </summary>
        /// <param name="game">Игра в которй происходит голосование</param>
        public Vote(Game game) {
            Game = game;
            Votes = new List<VoteCounter>();
        }
        /// <summary>
        /// наччать голосование
        /// </summary>
        public void DoVote()
        {
            Votes.Clear();
            Game.Log.WriteStreang("начало голосования " + this.Name);
            Game.Log.MadeIvent("ведущий", " голосование " +this.Name);

            foreach (Player player in Game.Players)
            {
                Votes.Add(new VoteCounter(player));
            }

            foreach (Player player in Game.Players)
            {
                if (this.PlayerInvoled(player))
                {
                    Player vote = GetVote(player);
                    Game.Log.WriteStreang("    Игрок " + player.Name + "(" + player.Role.Name + ")" + " проголосовал за игрока" + vote.Name + "(" + vote.Role.Name + ")") ;
                    Game.Log.MadeIvent(player.Name, " я голосую за " + vote.Name);

                    AddVote(vote);
                }
            }
            Execution(this.Result());
        }
        /// <summary>
        /// добавть голос
        /// </summary>
        /// <param name="player">игро за которог идёт голос</param>
        protected virtual void AddVote(Player player) {
            foreach (VoteCounter vote in Votes) { 
                if(vote.Player == player)
                {
                    vote.Up();
                }
            }
        }
        //Фильтр для игроков допущеных к голосованию
        protected virtual bool PlayerInvoled(Player player) {
            return player.IsActive;
        }
        /// <summary>
        /// Получить результат голосование
        /// </summary>
        /// <returns>игрок выбранный в результате голосования</returns>
        protected virtual Player Result() {
            Random random = new Random();

            for (int i = 0; i < Votes.Count - 1; i++)
            {
                int j = random.Next(i + 1, Votes.Count);
                VoteCounter temp = Votes[j];
                Votes[j] = Votes[i];
                Votes[i] = temp;
            }

            this.Votes.Sort();
            Game.Log.WriteStreang("    в результате голосования был выбран игрок" + this.Votes.Last().Player.Name + "(" + this.Votes.Last().Player.Role.Name + ")");
            Game.Log.MadeIvent("ведущий", "в результате голосования был выбран игрок " + this.Votes.Last().Player.Name);
            return this.Votes.Last().Player;
        }
        /// <summary>
        /// Исполнить приговор
        /// </summary>
        /// <param name="player">игрок над которым выполняется приговор</param>
        protected virtual void Execution(Player player)
        {
        }
        /// <summary>
        /// Получение голоса игрока
        /// </summary>
        /// <param name="player">игрок который голосует</param>
        /// <returns>Игрок за которого проголосовали</returns>
        protected virtual Player GetVote(Player player)
        {
            return null;
        }  
    }
    /// <summary>
    /// голосование за убийцу. выполняеться каждый день среди всех участников игры. 
    /// </summary>
    class MurderVote : Vote {
        /// <summary>
        /// Создание голосования за убийство оювеняемого
        /// </summary>
        /// <param name="game">Игра, которой пренадлежит голосование</param>
        /// <param name="name">название голосования</param>
        public MurderVote(Game game,string name) : base(game) {
            Name = name;
        }
        /// <summary>
        /// Получение голосования, игроком выбираеться другой игрок наиболее вероятно имеющий противоположную сторону  
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        protected override Player GetVote(Player player)
        {
            Random random = new Random();
            List<Guess> guesses = player.Memory.TakeListVersion(roleFilter: a => !a.IsFriend(player.Role));
            guesses = guesses.OrderBy(a => a.Chance * -1).ToList();

            int current = 0;

            while (true)
            {
                if(random.NextDouble() - (float)(50 + player.Smart) / 100 < 0)
                {
                    var boop = guesses.Where(a => a.Chance == guesses[current].Chance).ToList();
                    return boop[random.Next(boop.Count)].Player;
                }
                else
                {
                    current = (current + 1) % guesses.Count;
                }
            }
        }
        /// <summary>
        /// убийство обвененного игрока
        /// </summary>
        /// <param name="player">обвеняемый игрок</param>
        protected override void Execution(Player player)
        {
            player.Kill();
        }
    }
}
