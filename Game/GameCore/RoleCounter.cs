using System;
using System.Collections.Generic;
using Practic2020.Players;
using Practic2020.Roles;

namespace Practic2020.GameCore
{
    /// <summary>
    /// Класс счётчик объеденяющий в себе информацию о количестве свободных мест и действующих игроках для заданной роли
    /// </summary>
    public class RoleCounter
    {
        /// <summary>
        /// роль к которой пренадлежит счётчик
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        /// максимум игроков данной роли которое можит быть в игре
        /// </summary>
        public int MaxCount { get; set; }
        /// <summary>
        /// Все игроки пренадлежащие к данной роли
        /// </summary>
        public List<Player> RoleMembers { get; set; }
        /// <summary>
        /// количество игроков в данной игре
        /// </summary>
        public int InGamePlayers{ get { return RoleMembers.Count; } }
        /// <summary>
        /// Есть ли действующие игроки данной роли в игре
        /// </summary>
        public bool SombodyInGame() {
            foreach (Player player in RoleMembers)
            {
                if (player.IsLive)
                {
                    return true;
                }
            }
            return false;
        }
        public RoleCounter(Role role)
        {
            Role = role;
            MaxCount = 1;
            RoleMembers = new List<Player>();
        }

        /// <summary>
        /// может ли игрок имаеть данную роль
        /// </summary>
        public bool IsFool {
            get {
                return MaxCount == InGamePlayers;
            }
        }
        /// <summary>
        /// добавить нового игрока данной роли
        /// </summary>
        /// <param name="player">доавляемый игрок</param>
        public void ExtendRoleMembers(Player player)
        {
            if (this.IsFool)
            {
                throw new Exception("Достигнутот максимум игроков данной роли");
            }
            RoleMembers.Add(player);
        }
        /// <summary>
        /// Расширрить количество игроков данной роли
        /// </summary>
        /// <param name="addition">количество новых мест</param>
        public void ExtendMaxPlayer(int addition = 1) {
            MaxCount += addition;
        }
        /// <summary>
        /// Имеются ли жевые игроки данной роли
        /// </summary>
        /// <returns></returns>
        public bool HasPlayer()
        {
            foreach(Player player in RoleMembers)
            {
                if (player.IsLive)
                {
                    return true;
                }
            }
            return false;
        }
        public List<Player>.Enumerator GetEnumerator()
        {
            return RoleMembers.GetEnumerator();
        }
    }
}
