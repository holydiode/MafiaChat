using Practic2020.GameCore;
using System;
using System.Collections.Generic;
using Practic2020.Players;

namespace Practic2020.Roles
{
    /// <summary>
    /// Сторона конфликта
    /// </summary>
    public enum Side
    {
        Peacful,
        Mafia,
        Maniac,
        Red,
        Blue,
        Green,
        Teal,
        Brown
    }

    /// <summary>
    /// Абстрактный класс роли, задающий способ обращения с ролями в модели игры
    /// </summary>
    public class Role
    {
        /// <summary>
        /// являеться ли роль сркытой
        /// 

        /// 
        /// </summary>

        public bool IsHiden{ get; set; }
        /// <summary>
        /// Сторана конфликта к которой пренадлежит игрок
        /// </summary>
        public Side Side{ get; set; }
        /// <summary>
        /// название роли
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// игра в кторой находиться данная роль
        /// </summary>
        public Game Game { get; protected set; }

        /// <summary>
        /// выполнение условия победы для роли
        /// </summary>
        /// <returns></returns>
        public virtual bool WinCondition()
        {
            if (Game.RoleList[this].SombodyInGame()) { 
                foreach (Player player in Game.Players)
                {
                    if(player.IsLive && player.Role != this)
                    {
                        return false;
                    }
                }
                return true;
            }
            else{
                return false;
            }
        }
        /// <summary>
        /// Присоеденение роли к игре
        /// </summary>
        /// <param name="game">игра к которой присоеденяеться роль</param>
        public virtual void LinkGame(Game game)
        {
            Game = game;
        }
        public static bool operator == (Role a, Role b) {
            a = a ?? new Role();
            b = b ?? new Role();
            return   a.GetType() == b.GetType(); } 
        public static bool operator != (Role a, Role b)
        {
            a = a ?? new Role();
            b = b ?? new Role();
            return a.GetType() != b.GetType();
        }
        /// <summary>
        /// получить список ролей противоположных данной
        /// </summary>
        /// <returns>список ролей противоположных данной</returns>
        protected List<Role> AntagonistList()
        {
            List<Role> AntagonistList = new List<Role>();
            foreach(RoleCounter counter in Game.RoleList)
            {
                if( !this.IsFriend(counter.Role) && counter.HasPlayer() )
                {
                    AntagonistList.Add(counter.Role);
                }
            }
            return AntagonistList;
        }
        /// <summary>
        /// получить случайную противоположную роль
        /// </summary>
        /// <returns>случайная противоположная роль</returns>
        public Role RandomReverse()
        {
            Random random = new Random();
            List<Role> rolelist = this.AntagonistList();
            return rolelist[random.Next(rolelist.Count)];

        }
        /// <summary>
        /// узнать являеться ли заданная роль дружественной 
        /// </summary>
        /// <param name="role">заданная роль</param>
        /// <returns>являеться ли заданная роль дружественной </returns>
        public bool IsFriend(Role role)
        {
            return this.Side == role.Side;
        }
    }
    /// <summary>
    /// роль мирного игрока
    /// </summary>
    public class Peacfool : Role
    {
        public Peacfool()
        {
            this.Name = "мирные";
            this.Side = Side.Peacful;
            this.IsHiden = false;
        }

    }
}