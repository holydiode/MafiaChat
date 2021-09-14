using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using Practic2020.Players;
using Practic2020.Roles;

namespace Practic2020.GameCore
{
    public enum AddPlayerMode
    {
        /// <summary>
        /// Игрок будет добавленн без изменений
        /// </summary>
        clear,
        /// <summary>
        /// Если игрок не имеет роли, ему будет выброна одна из доступных
        /// </summary>
        soft,
        /// <summary>
        /// Если игроков с заданнной ролью слишком много, их количество будет рассширенно
        /// </summary>
        hard,
    }
    public class Game
    {
        /// <summary>
        /// список событий кторые должны произойти утром 
        /// </summary>
        public List<MorningAction> MorningHepening {get; set;}
        /// <summary>
        /// вывод игры
        /// </summary>
        public Log Log { get; set; }
        /// <summary>
        /// список игроков в игре
        /// </summary>
        public List<Player> Players { get; set; }
        /// <summary>
        /// список ролей и их количесвто в игре
        /// </summary>
        public RoleList RoleList { get; set; }
        /// <summary>
        /// Стадия обсуждения в игре, игроки пытаються сгенерировать события и начинают их обсуждать.
        /// </summary>
        private void DiscusingStadia() { 
            foreach (Player players in Players)
            {
                Happening happening = players.TryToMakeHapening();
                if (happening != null)
                {
                    DisscussHapening(happening);
                } 
            }
        }
        /// <summary>
        /// Просыпаються все игроки заданной роли
        /// </summary>
        /// <param name="role">роль игроков</param>
        private void Wakeup(Type role = null){
            if(role == null)
            {
                Log.WriteStreang("город просыпаеться");
                Log.MadeIvent("ведущий", "город просыпаеться");
                foreach(Player player in Players)
                {
                    player.IsSleep = false;
                }
            }
            else
            {
                foreach (Player player in RoleList[role])
                {
                    player.IsSleep = false;
                }
                
            }
        }
        /// <summary>
        /// Просыпаються все игроки заданной роли
        /// </summary>
        /// <param name="role">роль игроков</param>
        private void Wakeup(Role role)
        {
            Log.WriteStreang(role.Name + " просыпаеться");
            Log.MadeIvent("ведущий", role.Name + " просыпаеться");
            Wakeup(role.GetType());
        }
        /// <summary>
        /// Засыпают все игроки заданной роли
        /// </summary>
        /// <param name="role">роль игроков</param>
        private void Sleep(Type role = null)
        {
            if (role == null)
            {
                Log.MadeIvent("ведущий", "город засыпает");
                Log.WriteStreang("город засыпает");
                foreach (Player player in Players)
                {
                    player.IsSleep = true;
                }
            }
            else
            {

                foreach (Player player in RoleList[role])
                {
                    player.IsSleep = true;
                }

            }
        }
        /// <summary>
        /// Засыпают все игроки заданной роли
        /// </summary>
        /// <param name="role">роль игроков</param>
        private void Sleep(Role role)
        {
            Log.WriteStreang(role.Name + " засыпает");
            Log.MadeIvent("ведущий", role.Name + " засыпает");


            Sleep(role.GetType());
        }
        public Game()
        {
            MorningHepening = new List<MorningAction>();
            Players = new List<Player>();
            RoleList = new RoleList();
            Log = new Log();
        }
        /// <summary>
        /// Проверяет выйграла ли одна из сторон
        /// </summary>
        /// <returns>Выигравшая роль, null если не одна сторона не выйграла </returns>
        private Role SombodyWin()
        {
            foreach(RoleCounter counter in RoleList)
            {
                if (counter.Role.WinCondition())
                {
                    return counter.Role;
                }
            }
            return null;
        }
        /// <summary>
        /// подготовка игроков к игре
        /// </summary>
        private void InitPlayers()
        {
            this.MorningHepening.Clear();
            foreach(Player player in Players)
            {
                player.Rebirth();

                player.Memory = new Memory(this, player.MemoryAbility);
                if(player.Role is ComandRole)
                {
                    foreach(Player playerInComand in RoleList[player.Role])
                    {
                        Happening happening = new OpenUp(this, player: playerInComand, role: player.Role, isTrue: true, isAcsiom: true);
                        happening.ImpactOnPlayer(player);
                    }
                }
                else
                {
                    Happening happening = new OpenUp(this, player: player, role: player.Role, isTrue: true, isAcsiom: true);
                    happening.ImpactOnPlayer(player);
                }
            }
        }
        /// <summary>
        /// Начать игру
        /// </summary>
        public void StartPlay()
        {
            InitPlayers();
            bool check = true;
            while (check)
            {
                check = Step();
            }
        }
        /// <summary>
        /// Начать игру без начала первого дня
        /// </summary>
        public void StratByStep()
        {
            InitPlayers();
        }
        /// <summary>
        /// Начать следующий день
        /// </summary>
        /// <returns>Закончилась ли игра</returns>
        public bool Step() {
            Wakeup();
            foreach (MorningAction action in MorningHepening)
            {
                action.Do();
            }
            MorningHepening.Clear();

            if (SombodyWin() != null)
            {
            Log.MadeIvent("ведущий", "Выграли игроки игравшие за " + SombodyWin().Name);
                Log.WriteStreang("Выграли игроки игравшие за " + SombodyWin().Name);
                return false;
            }
            DiscusingStadia();
            MurderVote vote = new MurderVote(this, "за казнь мафии");
            vote.DoVote();
            if (SombodyWin() != null)
            {
                Log.MadeIvent("ведущий", "Выграли игроки игравшие за " + SombodyWin().Name);
                Log.WriteStreang("Выграла игроки игравшие за " + SombodyWin().Name);
                return false;
            }
            Sleep();
            foreach (RoleCounter counter in RoleList)
            {
                if (counter.SombodyInGame())
                {
                    if (counter.Role is SoloRole)
                    {
                        foreach(Player player in counter.RoleMembers)
                        {
                            player.IsSleep = false;
                            this.Log.MadeIvent("ведущий", counter.Role.Name + " просыпается");
                            SoloRole CurentRole = (SoloRole)counter.Role;
                            CurentRole.StartAbility();
                            player.IsSleep = true;
                            this.Log.MadeIvent("ведущий", counter.Role.Name + " засыпает");
                        }
                    }


                    if (counter.Role is ComandRole)
                    {
                        ComandRole CurentRole = (ComandRole)counter.Role;
                        Wakeup(CurentRole);
                        CurentRole.StratVote();
                        Sleep(CurentRole);
                    }
                }
            }
                return true;
            }
        /// <summary>
        /// добавить игрока
        /// </summary>
        /// <param name="player">добавляемый игрок</param>
        /// <param name="playerMode">способ добавления игрока</param>
        public void AddPlayer(Player player, AddPlayerMode playerMode = AddPlayerMode.clear)
        {
            player.Game = this;
            RoleList.AddPlayer(player, playerMode);
            Players.Add(player);
        }
        /// <summary>
        /// добавляет роль в игру
        /// </summary>
        /// <param name="role">роль</param>
        /// <param name="count">количество игроков, которые могут играть на данной роли</param>
        public void AddRole(Role role, int count = 1)
        {
            role.LinkGame(this);
            RoleList.AddRole(role, count);
        }
        /// <summary>
        /// обсудить событие среди всех игроков 
        /// </summary>
        /// <param name="happening">обсуждаемое событие</param>
        public void DisscussHapening(Happening happening)
        {
            foreach (Player react_players in Players)
            {
                if (react_players.IsActive)
                {
                    happening.ImpactOnPlayer(react_players);
                }
            }
        }
    }
}
