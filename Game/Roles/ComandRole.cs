using Practic2020.GameCore;
using Practic2020.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Practic2020.Roles
{
    /// <summary>
    /// Класс командной роли расширяет возможности обычной роли, добавляя возможность организации локальных голосований. Все игроки пренадлежащие к одно командной роли знают друг друга.
    /// </summary>
    class ComandRole : Role
    {
        /// <summary>
        /// голосование пренадлежащие к данной роли
        /// </summary>
        protected Vote Vote { get; set;}

        /// <summary>
        /// начать голосование данной роли
        /// </summary>
        public virtual void StratVote()
        {
            Vote.DoVote();
        }
        /// <summary>
        /// присоеденить роль к игре вместе с голосованием
        /// </summary>
        /// <param name="game">игра к которой присоеденяется роль</param>
        public override void LinkGame(Game game)
        {
            base.LinkGame(game);
            Vote.Game = game;
        }
    }
    /// <summary>
    /// Роль мфии
    /// <img src="https://media.istockphoto.com/photos/noir-movie-character-picture-id837345268?k=20&m=837345268&s=612x612&w=0&h=1tahuBSTIUCUbVcZhaxHMV5iLm-W1c_UBlz7VBAcNrc=" />
    /// </summary>
    class Mafia : ComandRole
    {
        public Mafia()
        {
            Vote = new MafiaVote(Game);
            IsHiden = true;
            Side = Side.Mafia;
            this.Name = "мафия";
        }
    }
    /// <summary>
    /// голосование мафии
    /// </summary>
    class MafiaVote: Vote
    {
        public MafiaVote(Game game) : base(game)
        {
            Name = "голосование мафии";
        }
        protected override Player GetVote(Player player)
        {
            Random random = new Random();
            List<Guess> guesses = player.Memory.TakeListVersion(roleFilter: a => !a.IsFriend(player.Role));

            for (int i = 0; i < guesses.Count - 1; i++)
            {
                int j = random.Next(i + 1, guesses.Count);
                Guess temp = guesses[j];
                guesses[j] = guesses[i];
                guesses[i] = temp;
            }

            guesses.Sort((a, b) => a.Chance.CompareTo(b.Chance) * -1);
            int current = 0;

            while (true)
            {
                if (random.NextDouble() - (float)(50 + player.Intelegence) / 100 < 0)
                {
                    return guesses[current].Player;
                }
                else
                {
                    current = (current + 1) % guesses.Count;
                }
            }
        }
        /// <summary>
        /// Убить игрока на утро следующего дня по резульату голосования.
        /// </summary>
        /// <param name="player"></param>
        protected override void Execution(Player player)
        {
            Game.MorningHepening.Add(new MorningAction(player, a =>
            {
                Game.Log.WriteStreang("Утром город узнает, что игрок " + a.Name + " был убит");
                Game.Log.MadeIvent("ведущий", player.Name + " был убит прошлой ночью");
                a.Kill();
            }));
        }
    }


}
