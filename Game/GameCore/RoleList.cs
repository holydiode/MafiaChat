using System;
using System.Collections.Generic;
using Practic2020.Players;
using Practic2020.Roles;

namespace Practic2020.GameCore
{
    /// <summary>
    /// Контейнер для зранение и удобного доступ ко всем счётчиком ролей в игрк.
    /// </summary>
    public class RoleList {
        /// <summary>
        /// Список ролей в игре
        /// </summary>
        public List<RoleCounter> List { get; set; }
        public RoleList()
        {
            List = new List<RoleCounter>();
        }
        /// <summary>
        /// можно ли дать новому игроку роль
        /// </summary>
        public bool HaveRoles {
            get {
                foreach (RoleCounter roleCounter in List)
                {
                    if (!roleCounter.IsFool)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Ввести роль в игру, если роль уже введина, количество игроков увеличиться на 1
        /// </summary>
        /// <param name="role"></param>
        public void AddRole(Role role, int count = 1)
        {
            bool Job = true;
            foreach (RoleCounter roleCounter in List)
            {
                if (role == roleCounter.Role)
                {
                    roleCounter.ExtendMaxPlayer(count);
                    Job = false;
                    break;
                }
            }
            if (Job)
            {
                List.Add(new RoleCounter(role));
                this[role].ExtendMaxPlayer(count - 1);
            }
        }
        public List<RoleCounter>.Enumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }
        /// <summary>
        /// возвращает случайную свободную роль, которая есть в игре на данный момент.
        /// </summary>
        /// <returns>случайная свободная роль</returns>
        public Role GetRandomPosibleRole()
        {
            Random random = new Random();
            while (true)
            {
                RoleCounter randomCounter = List[random.Next(0, List.Count)];
                if (!randomCounter.IsFool)
                {
                    return randomCounter.Role;
                }
            }
        }
        /// <summary>
        /// шаблон для фильтров ролей
        /// </summary>
        /// <param name="role">проверяемая роль</param>
        /// <returns>проходит ли роль через фильтр</returns>
        public delegate bool RoleFillter(Role role);
        /// <summary>
        /// Возвращает список всех ролей удовлетворяющий условию
        /// </summary>
        /// <param name="fillter">условие для ролей</param>
        /// <returns>список ролей удовлетворяющий условию</returns>
        public List<Role> GetRoleList(RoleFillter fillter = null)
        {
            if (fillter == null) fillter = a => true;
            List<Role> list = new List<Role>();
            foreach(RoleCounter roleCounter in List)
            {
                if (fillter(roleCounter.Role))
                {
                    list.Add(roleCounter.Role);
                }
            }
            return list;
        }
        /// <summary>
        /// Добавить новго игрока в учёт ролей
        /// </summary>
        /// <param name="player">добавляемый игрок</param>
        /// <param name="mode">способ добавления игрока</param>
        public void AddPlayer(Player player, AddPlayerMode mode = AddPlayerMode.soft)
        {
            if(player.Role == null)
            {
                if(mode == AddPlayerMode.soft)
                {
                    player.Role = this.GetRandomPosibleRole();
                }
                else
                {
                    throw new Exception("игрок не имеет роли");
                }
            }
            if (this[player.Role.GetType()].IsFool && mode == AddPlayerMode.hard)
            {
                this[player.Role.GetType()].ExtendMaxPlayer();
            }

            this[player.Role].ExtendRoleMembers(player);

        }
        /// <summary>
        /// возращает список игрорков некоторой роли
        /// </summary>
        /// <param name="role">роль игрока</param>
        /// <returns>список игроков с ролью role</returns>
        public RoleCounter this[Type role]
        {
            get
            {
                foreach(RoleCounter roleCounter in List)
                {
                    if(roleCounter.Role.GetType() == role)
                    {
                        return roleCounter;
                    }
                }
                throw new Exception("данной роли нет в игре");
            }
        }
        /// <summary>
        /// возращает список игрорков некоторой роли
        /// </summary>
        /// <param name="role">роль игрока</param>
        /// <returns>игроков с ролью role</returns>
        public RoleCounter this[Role role]
        {
            get
            {
                foreach (RoleCounter roleCounter in List)
                {
                    if (roleCounter.Role.GetType() == role.GetType())
                    {
                        return roleCounter;
                    }
                }
                throw new Exception("данной роли нет в игре");
            }
        }
    }
}
