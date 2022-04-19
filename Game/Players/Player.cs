using System;
using System.Collections.Generic;
using System.Text;
using Practic2020.Roles;
using Practic2020.GameCore;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Practic2020.Players
{
    public class Player
    {
        public Game Game { get; set; }

        /// <summary>
        /// хитрость - уменее говорить ложь
        /// </summary>
        public int Tric { set; get; }

        /// <summary>
        /// искренность - уменее говроить правду
        /// </summary>
        public int Сandor { set; get; }

        /// <summary>
        /// проницательнсоть - способность верно распознать дожь
        /// </summary>
        public int Conviction { set; get; }

        /// <summary>
        /// доверие - способность верно распознать правду
        /// </summary>
        public int Trust { get; set; }

        /// <summary>
        /// убеждение - способность доказывать свою точку зрения 
        /// </summary>
        public int Attac { get; set; }

        /// <summary>
        /// защита - способность защищиаться в дискуссиях
        /// </summary>
        public int Protect { get; set; }

        /// <summary>
        /// самоуверенность - желание совершать действия во время обсуждений
        /// </summary>
        public int Agression { get; set; }

        /// <summary>
        /// скептицизм - желание верить происходящим событиям
        /// </summary>
        public int Skepticizm { get; set; }

        /// <summary>
        /// разумность - желание своершать действие приводящие к победе
        /// </summary>
        public int Intelegence { get; set; }

        /// <summary>
        /// память - количесство улик которые игрок может запомнить
        /// </summary>
        public int MemoryAbility { get; set; }

        /// <summary>
        /// память игрока
        /// </summary>
        public Memory Memory { get; set; }

        /// <summary>
        /// Жив ли этот игрок
        /// Мертвые игроки не учавствуют в голосовании и не создают и не реагируют на события.
        /// </summary>
        public bool IsLive { get; private set; }

        /// <summary>
        /// Спит ли этот игрок
        /// Спящие игроки не учавствуют в голосовании и не создают и не реагируют на события.
        /// </summary>
        public bool IsSleep { get; set; }

        /// <summary>
        /// можит ли данный игрок учавствовать в голосовании, создовать события 
        /// </summary>
        public bool IsActive { get { return IsLive && !IsSleep; } }

        /// <summary>
        /// Список ролей пенадлежащих игроку
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// память игрока
        /// </summary>
        public Memory PlayerMemory { get; set; }

        /// <summary>
        /// имя персонажа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// вероятность того, что игрок сделает что-либо
        /// увеличиваеться каждый ход или при получениии улики
        /// после совершения действия уменьшаеться
        /// </summary>
        public float Initiative { get; set; }

        /// <summary>
        /// создание игрока
        /// </summary>
        public Player() {
            this.Name = "";
            this.IsLive = true;
            this.IsSleep = false;
        }

        /// <summary>
        /// Игрок попытаеться предпринять участие в обсуждениях
        /// </summary>
        /// <returns>вернет событие если захочит что-то высказать, иначе вернет null</returns>
        public Happening TryToMakeHapening()
        {
            if (!this.IsActive) {
                return null;
            }
            Random random = new Random();
            if (random.NextDouble() <= this.Initiative + 0.3)
            {
                this.Initiative /= 2;
                return this.MakeHapening();
            }
            else
            {
                this.Initiative += (1 - this.Initiative) * this.Agression / 100;
            }
            return null;
        }
        /// <summary>
        /// создание случайного игрока
        /// </summary>
        /// <param name="name">имя игрока</param>
        /// <returns>случайный игрок</returns>
        public static Player Random(string name)
        {
            Random random = new Random();
            Player player = new Player();

            int[] statData = new int[6];
            int SumList = 0;

            for (int i = 0; i < 6; i++)
            {
                int randint = random.Next(20);
                randint = (SumList + randint > 80) ? 80 - SumList : randint;
                statData[i] = randint;
                SumList += randint;
            }

            for (int i = 0; i < 5; i++)
            {
                int j = random.Next(i + 1, 5);
                int temp = statData[j];
                statData[j] = statData[i];
                statData[i] = temp;
            }

            player.Tric = statData[0];
            player.Сandor = statData[1];
            player.Conviction = statData[2];
            player.Trust = statData[3];
            player.Attac = statData[4];
            player.Protect = statData[5];
            player.MemoryAbility = 80 - SumList;

            player.Name = name;

            player.Agression = random.Next(50);
            player.Skepticizm = random.Next(50);
            player.Intelegence = random.Next(50);

            return player;
        }
        /// <summary>
        /// Создать событие в котором игрок говорит о своей роли
        /// </summary>
        /// <returns>созданное событие</returns>
        private Happening MakeProtect()
        {
            Random random = new Random();
            double lieChance = Role.IsHiden ? 0.5 + (float)Intelegence / 100 : 0.5 - (float)Intelegence / 100;

            if (lieChance > random.NextDouble())
            {
                List<Role> roleList = Game.RoleList.GetRoleList(a => a.IsHiden == false);
                if (roleList.Count == 0)
                {
                    roleList = Game.RoleList.GetRoleList();
                }
                return new OpenUp(Game, this, roleList[random.Next(roleList.Count)], false, false, random.Next(20) + this.Tric);
            }
            else
            {
                return new OpenUp(Game, this, this.Role, true, false, random.Next(20) + this.Сandor);
            }
        }
        /// <summary>
        /// ответить на событие в котором кто-либо назвал роль этого игрока
        /// </summary>
        /// <param name="role">Роль которую приписали данному игроку</param>
        /// <param name="power">На сколько убедительно была присвоена роль</param>
        /// <returns></returns>
        public bool ContrAttack(Role role, int power)
        {
            Random random = new Random();
            if (random.Next(20) + this.Protect < power)
            {
                return true;
            }
            else
            {
                return ! role.IsHiden;
            }
        }
        /// <summary>
        /// Создать событие в котором игрок говорит о чужой роли
        /// </summary>
        /// <param name="version">догадка, на основе которой будет созданно событие</param>
        /// <returns></returns>
        private Happening MakeAttak(Guess version)
        {
            Random random = new Random();

            if (this.Role.IsFriend(version.Role))
            {
                if (version.Role.IsHiden)
                {
                    List<Role> roleList = Game.RoleList.GetRoleList(a => a.IsHiden == false);
                    if (roleList.Count == 0)
                    {
                        roleList = Game.RoleList.GetRoleList();
                    }
                    return new Shame(Game, this, version.Player,roleList[random.Next(roleList.Count)], random.Next(20) + this.Attac, random.Next(20) + this.Tric, true);
                }
                else
                {
                    return new Shame(Game, this, version.Player, version.Role, random.Next(20) + this.Attac, random.Next(20) + this.Сandor, true);
                }
            }
            else
            {
                List<Role> roleList = Game.RoleList.GetRoleList(a => a.IsFriend(version.Role) == false);
                if (roleList.Count == 0)
                {
                    roleList = Game.RoleList.GetRoleList();
                }
                return new Shame(Game, this, version.Player, roleList[random.Next(roleList.Count)], random.Next(20) + this.Attac, random.Next(20) + this.Tric, true);
            }
        }
        /// <summary>
        /// Создать событие для обсуждения игроками
        /// </summary>
        /// <returns>созданное событие</returns>
        private Happening MakeHapening()
        {
            Random random = new Random();
            List<Guess> versions = Memory.TakeListVersion(playerFilter: a => a != this);
            int numberVersion = versions.Count -1;
            while(random.NextDouble() * 100 > 50 + this.Intelegence ){
                numberVersion = numberVersion - 1 < 0 ? versions.Count - 1 : numberVersion - 1;
            }

            if(random.NextDouble() >= versions[numberVersion].Chance)
            {
                return this.MakeProtect();
            }
            else
            {
                int current = 0;
                while (true)
                {
                    if (random.NextDouble() - (float)(this.Intelegence) / 100 <= 0)
                    {
                        return this.MakeAttak(versions[current]);
                    }
                    else
                    {
                        current = (current + 1) % versions.Count;
                    }
                }
            }
        }
        /// <summary>
        /// убить игрка
        /// </summary>
        public void Kill()
        {
            this.IsLive = false;
            this.Game.DisscussHapening(new OpenUp(Game, this, Role, true, true ));
            Game.Log.MadeIvent("ведущий", this.Name + " выбывает из игры, его роль была " + this.Role.Name);
        }
        /// <summary>
        /// востановить игрока
        /// </summary>
        public void Rebirth()
        {
            this.IsLive = true;
        }
    }
}
