using Practic2020.Players;
using Practic2020.Roles;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Practic2020.GameCore
{
    /// <summary>
    /// абстрактный класс события. Событие - некотороая сущность видоизменяющая память игрока заданным образом. 
    /// </summary>
    public class Happening
    {
        /// <summary>
        /// являеться ли событие правдой или ложью, влеяет на то, какая характеристика игрока будет использоваться для проверки влияния на игрока 
        /// </summary>
        public bool IsTrue { get; set; }
        /// <summary>
        /// являеться ли событие гарантированной истинной, чаще всего такие события происходят от лица видущего
        /// </summary>
        public bool IsAcsiom { get; set; }
        /// <summary>
        /// Вес события. То на сколько правдоподобно звучит информация.
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// игра, к кторой пренадлежит данное событие
        /// </summary>
        public Game Game { get; set; }
        /// <summary>
        /// Общий конструктор
        /// </summary>
        /// <param name="game">игра, к кторой пренадлежит данное событие</param>
        public Happening(Game game)
        {
            Game = game;
        }
        /// <summary>
        /// Влияние события на игрока. Абстрактный метод задающий способ видоизменения с памятью игрока.
        /// </summary>
        /// <param name="player">игрок, на кторого влияет событие</param>
        public virtual void ImpactOnPlayer(Player player) {
        }
    }
    /// <summary>
    /// Собыьтие описывающие роль одного игрока. Создаеться либо игрой, либо игроком. Если событие создаеться игроком, то в нем может говориться только о своей роли.
    /// </summary>
    public class OpenUp : Happening
    {
        /// <summary>
        /// Игрок, чья роль раскрываеться
        /// </summary>
        public Player Player { get; set; }
        /// <summary>
        /// Раскрываемая роль
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        /// Конструктор для создания события
        /// </summary>
        /// <param name="game">игра, к кторой пренадлежит данное событие</param>
        /// <param name="player">Игрок, чья роль раскрываеться</param>
        /// <param name="role">Раскрываемая роль</param>
        /// <param name="isTrue">являеться ли событие правдой</param>
        /// <param name="isAcsiom">являеться ли событие гарантированной истинной</param>
        /// <param name="wight">на сколько правдоподобно звучит информация</param>
        public OpenUp(Game game, Player player, Role role, bool isTrue, bool isAcsiom = false, int wight = 0):base(game)
        {
            Player = player;
            Role = role;
            IsAcsiom = isAcsiom;
            IsTrue = isTrue;
            Weight = wight;
            if (!IsAcsiom)
            {
                game.Log.WriteStreang("Ирок " + player.Name + "(" + player.Role.Name + ")" + " сказал что он " + Role.Name);
                Game.Log.MadeIvent(Player.Name, "я - " + Role.Name);

            }
        }
        /// <summary>
        /// Если игрок считает событие правдивым, то вероятноть заданной роли у заднного игрока увеличиться, иначе увеличиться вероятность случайной противоположнойроли
        /// </summary>
        /// <param name="player">игрок, на кторого влияет событие</param>
        public override void ImpactOnPlayer(Player player)
        {

            if (IsAcsiom)
            {
                player.Memory.AddClue(new Clue(this, important: 100, player: Player, role: Role, exactClue: true));
            }
            else if (player != Player)
            {
                Random random = new Random();
                int check = IsTrue ? 40 - (random.Next(20) + player.Trust) : random.Next(20) + player.Conviction;

                if (check < Weight)
                {
                    Game.Log.WriteStreang("    Ирок " + player.Name + "(" + player.Role.Name + ")" + "считает что игрок " + Player.Name + "(" + Player.Role.Name + ") сказал правду");
                    Game.Log.WriteStreang("        Ирок " + player.Name + "(" + player.Role.Name + ")" + "считает что игрок " + Player.Name + "(" + Player.Role.Name + ") возможно " + Role.Name);
                    Game.Log.MadeIvent(player.Name, "я в это верю");
                    player.Memory.AddClue(new Clue(this, important: 70, player: Player, role: Role, point: player.Skepticizm));
                }
                else
                {
                    Game.Log.WriteStreang("    Ирок " + player.Name + "(" + player.Role.Name + ")" + "считает что игрок " + Player.Name + "(" + Player.Role.Name + ") намеренно солгал");
                    Role randomIncerseRole = Role.RandomReverse();
                    Game.Log.WriteStreang("        Ирок " + player.Name + "(" + player.Role.Name + ")" + "считает что игрок " + Player.Name + "(" + Player.Role.Name + ") возможно " + randomIncerseRole.Name);
                    Game.Log.MadeIvent(player.Name, "я в это не верю");
                    player.Memory.AddClue(new Clue(this, important: 70, player: Player, role: randomIncerseRole, point: player.Skepticizm));
                }
            }
        }
    }

    /// <summary>
    /// событие моделирующие ситуацию при которой один из игроков, высказывает своё предположение по поводу другого игрока.
    /// </summary>
    public class Shame : Happening
    {
        /// <summary>
        /// Игрок высказывающий пердположение
        /// </summary>
        public Player Speaker { get; set; }
        /// <summary>
        /// игрок по поаоду которого высказываються предположение
        /// </summary>
        public Player Player { get; set; }
        /// <summary>
        /// Указывает на то, смог ли высказывающийся доказать свою провоту в споре
        /// </summary>
        public bool SpeakerIsWin { get; set;}
        /// <summary>
        /// Роль которую предпологает игрок
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        /// Создание события
        /// </summary>
        /// <param name="game"></param>
        /// <param name="speaker">Игрок высказывающий пердположение</param>
        /// <param name="player">Указывает на то, смог ли высказывающийся доказать свою провоту в споре</param>
        /// <param name="role">Роль которую предпологает игрок</param>
        /// <param name="power">на сколько убедительным был игрок в споре</param>
        /// <param name="weight">на сколько правдоподобно звучит информация</param>
        /// <param name="isTrue">было ли высказывание правдивым</param>
        public Shame(Game game, Player speaker, Player player, Role role, int power, int weight, bool isTrue):base(game)
        {
            Speaker = speaker;
            Player = player;
            Role = role;
            Weight = weight;
            SpeakerIsWin = player.ContrAttack(role, power);
            IsTrue = isTrue;

            Game.Log.WriteStreang("Ирок " + Speaker.Name + "(" + Speaker.Role.Name +")" + 
                " сказал что ирок " + Player.Name + "(" + Speaker.Role.Name + ")" + "- " + 
                Role.Name + "   (" + (IsTrue ? "сказал правду": "солгал") + ")" );

            Game.Log.MadeIvent(Speaker.Name, "я считаю что " + Player.Name + " - " + Role.Name);


            if (SpeakerIsWin) {
                Game.Log.WriteStreang("    Ирок " + Player.Name + "(" + Speaker.Role.Name + ") не смог оправергнуть это");
                Game.Log.MadeIvent(Player.Name, "я не могу ничего сказать в свою защиту");
            }
            else
            {
                Game.Log.WriteStreang("    Ирок " + Player.Name + "(" + Speaker.Role.Name + ") смог оправергнуть это");
                Game.Log.MadeIvent(Player.Name, "я с этим не согласен");
            }
        }
        /// <summary>
        /// Если игрок не поверит в высказывание, то он будет уверен в обратной версии
        /// Если игрок верит в высказывание но его смогли опровергнуть то ничего не происходит
        /// Если игрок верит в высказываение и его не могли опровегрнуть, то игрок соглашается с изночальной версией.
        /// </summary>
        /// <param name="player"></param>
        public override void ImpactOnPlayer(Player player)
        {
            if (player != Player && player != Speaker)
            {

                Random random = new Random();
                int check = IsTrue ? 40 - (random.Next(20) + player.Trust) : random.Next(20) + player.Conviction;

                if (check < Weight)
                {
                    if (SpeakerIsWin)
                    {
                        Game.Log.WriteStreang("    Ирок " + player.Name + "(" + player.Role.Name + ")" + "считает что игрок " + Speaker.Name + "(" + Speaker.Role.Name + ") сказал правду");
                        Game.Log.WriteStreang("        Ирок " + player.Name + "(" + player.Role.Name + ")" + "считает что игрок " + Player.Name + "(" + Player.Role.Name + ") возможно " + Role.Name);
                        Game.Log.MadeIvent(player.Name, "я в это верю");
                        player.Memory.AddClue(new Clue(this, important: 70, player: Player, role: Role, point: player.Skepticizm));
                    }
                    else
                    {

                    }
                }
                else
                {
                    Game.Log.WriteStreang("    Ирок " + player.Name + "(" + player.Role.Name + ")" + "считает что игрок " + Speaker.Name + "(" + Speaker.Role.Name + ") намеренно солгал");
                    Role randomIncerseRole = Role.RandomReverse();
                    Game.Log.WriteStreang("        Ирок " + player.Name + "(" + player.Role.Name + ")" + "считает что игрок " + Player.Name + "(" + Player.Role.Name + ") возможно " + randomIncerseRole.Name);
                    Game.Log.MadeIvent(player.Name, "я в это не верю");
                    player.Memory.AddClue(new Clue(this, important: 70, player: Player, role: randomIncerseRole, point: player.Skepticizm));
                }
            }
        }
    }
}


