using Practic2020.GameCore;
using Practic2020.Roles;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;

namespace Practic2020.Players
{
    /// <summary>
    /// шаблон для указания способа нахождения лучшего предположения
    /// </summary>
    /// <param name="guesses">список предположений, которые среди которых надо найти наиболее подходящее</param>
    /// <returns>наиболее подходящее предположение</returns>
    public delegate Guess BestVersion(List<Guess> guesses);
    /// <summary>
    /// шаблон для фильтра ролей
    /// </summary>
    /// <param name="player">роль</param>
    /// <returns>проходит ли роль через фильтр</returns>
    public delegate bool RoleFilter(Role role);
    /// <summary>
    /// шаблон для фильтра догадок
    /// </summary>
    /// <param name="guess">догадка</param>
    /// <returns>проходит ли догадка через фильтр</returns>
    public delegate bool GuessFilter(Guess guess);
    /// <summary>
    /// шаблон для фильтра игроков
    /// </summary>
    /// <param name="player">игрок</param>
    /// <returns>проходит ли игрок через фильтр</returns>
    public delegate bool PlayerFilter(Player player);

    /// <summary>
    /// догадка - упрощённая модель базового суждения игрока. Содержит информацию о том с какой вероятностью определённый игро имеет определённую роль.
    /// </summary>
    public class Guess : IComparable<Guess>
    {
        /// <summary>
        /// Игрок, к которому относиться догадка
        /// </summary>
        public Player Player { get; set; }
        /// <summary>
        /// Педположительная роль игрока
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        /// Обсолютное значение уверенности игрока в этой дагадке
        /// </summary>
        public int Points { get; set; }
        /// <summary>
        /// Веротность того что догадка окажеться верной. Относительное значение уверенности
        /// </summary>
        public float Chance { get; set; }
        /// <summary>
        /// Является ли догадка гарантировнно верной
        /// </summary>
        public bool IsStatic{ get; set;}
        /// <summary>
        /// Создание догадки
        /// </summary>
        /// <param name="role">Игрок, к которому относиться догадка</param>
        /// <param name="player">Педположительная роль игрока</param>
        public Guess(Role role, Player player) {
            Player = player;
            Role = role;
            IsStatic = false;
            Chance = 0;
            Points = 0;
        }
        /// <summary>
        /// Добавить еденицы уверенности к догадке
        /// </summary>
        /// <param name="point">количесвто едениц уверенности</param>
        /// <returns></returns>
        public bool AddPoint(int point)
        {
            if (!this.IsStatic)
            {
                Points += point;
            }
            return !IsStatic;
        }
        /// <summary>
        /// Пересчитать относительную вероятность догадки
        /// </summary>
        /// <param name="GlobalPoint">Сумма вероятности всех дагадок длы данного игрока</param>
        public void CountChance(int GlobalPoint)
        {
            Chance = ((float)Points) / GlobalPoint;
        }
        /// <summary>
        /// Сбросить значение до исходного
        /// </summary>
        public void Reset()
        {
            Points = Player.Game.RoleList[Role].MaxCount * 10;
        }
        /// <summary>
        /// Сделать догадку доставерной
        /// </summary>
        /// <param name="result">true - догадка правдивая, false догадка ложная</param>
        public void MadeStatic( bool result = true)
        {
            this.IsStatic = true;
            if (result)
            {
                Points = 1;
                Chance = 1;
            }
            else
            {
                Points = 0;
                Chance = 0;
            }
        }
        public int CompareTo(Guess other)
        {
            if(this.Chance < other.Chance)
            {
                return -1;
            }
            else if(this.Chance > other.Chance){
                return 1;
            }
            else
            {
                return 0;
            }

        }
    }
    /// <summary>
    /// Мнение - модель комплексного суждения, котроая содержит все возможные догадки по отношению к определённому игроку. По сути содержит в себе информацию о том с каим шансом игрок пренадлежит к каждой роли.
    /// </summary>
    public class Opinion
    {
        /// <summary>
        /// Создание мнения
        /// </summary>
        /// <param name="player">игрок по отношению к которому создаеться суждение</param>
        public Opinion(Player player)
        {
            Player = player;
            Guesses = new List<Guess>();
            foreach(RoleCounter roleCounter in Player.Game.RoleList)
            {
                Guesses.Add(new Guess(roleCounter.Role, Player));
            }
            Reset();
        }
        /// <summary>
        /// Список из всех возможных догадок для данного игрока
        /// </summary>
        public List<Guess> Guesses {get;set;}
        /// <summary>
        /// Сумма значений уверенности всех догадок
        /// </summary>
        public int GlobalPoint { get; set;}
        /// <summary>
        /// игрок по отношению к которому создаеться суждение
        /// </summary>
        public Player Player { get; set; }
        /// <summary>
        /// Пересчитать вероятности для всех догадок
        /// </summary>
        public void Balance() {
            GlobalPoint = 0;
            foreach (Guess guess in Guesses)
            {
                GlobalPoint += guess.Points;
            }
            foreach (Guess guess in Guesses)
            {
                guess.CountChance(GlobalPoint);
            }
        }
        /// <summary>
        /// Сделать одну ис догадок достоверно веной
        /// </summary>
        /// <param name="role"></param>
        public void MadeExact(Role role)
        {
            foreach(Guess guess in Guesses)
            {
                if(guess.Role == role)
                {
                    guess.MadeStatic();
                    GlobalPoint = guess.Points;
                }
                else
                {
                    guess.MadeStatic(false);
                }
            }
        }
        /// <summary>
        /// Сбросить все догадки к исходному значению
        /// </summary>
        public void Reset()
        {
            GlobalPoint = Player.Game.Players.Count * 10; 
            foreach (Guess guess in Guesses)
            {
                guess.Reset();
            }
            this.Balance();
        }
        /// <summary>
        /// Получение догадки с заданной ролью
        /// </summary>
        /// <param name="role">заданная роль</param>
        /// <returns>догадка с заданной ролью</returns>
        public Guess this[Role role]
        {
            get
            {
                foreach (Guess guess in Guesses)
                {
                    if (guess.Role == role)
                    {
                        return guess;
                    }
                }
                throw new Exception("данной роли нет в игре");
            }
        }
        /// <summary>
        /// Получить лучшую догадку удовлетворяющую 
        /// </summary>
        /// <param name="bestVersion">способ получения лучшей версии</param>
        /// <param name="roleFilter">фильтр по роли</param>
        /// <param name="clueFilter">фильтр по догадки</param>
        /// <returns></returns>
        public Guess TakeBastVersion(BestVersion bestVersion = null, RoleFilter roleFilter = null, GuessFilter clueFilter = null)
        {
            if (bestVersion == null) bestVersion = a => a.Max();
            if (roleFilter == null) roleFilter = a => true;
            if (clueFilter == null) clueFilter = a => true;
            return bestVersion( Guesses.FindAll(a => roleFilter(a.Role) && clueFilter(a)));
        }
    }
    /// <summary>
    /// модель памяти игрока, к которой он обращаеться для принятия решений
    /// </summary>
    public class Memory
    {
        /// <summary>
        /// создание памяти игрока
        /// </summary>
        /// <param name="game">Игра в которой находиться игрок</param>
        /// <param name="memoryAbility">Количество событий, которые может запомнить игрок</param>
        public Memory(Game game, int memoryAbility)
        {
            this.Size = game.Players.Count * 2 + memoryAbility;
            Opinions = new List<Opinion>();
            Clues = new List<Clue>();

            foreach (Player player in game.Players)
            {
                Opinions.Add(new Opinion(player));
            }
            Game = game;
        }
        /// <summary>
        /// Игра в которой находиться игрок
        /// </summary>
        private Game Game { get; set; }
        /// <summary>
        /// Список мнений о всех игроках
        /// </summary>
        public List<Opinion> Opinions { get; set; }
        /// <summary>
        /// Список всех воспоминаний, которые имеет игрок в данный момент
        /// </summary>
        public List<Clue> Clues { get; set; }
        /// <summary>
        /// Поулчить список всех лучших версий удовлетворяющих условиям
        /// </summary>
        /// <param name="bestVersion">способ получения лучшей версии</param>
        /// <param name="roleFilter">фильтр ролей</param>
        /// <param name="clueFilter">фильтр улик</param>
        /// <param name="playerFilter">фильтр игроков</param>
        /// <returns>список всех лучших версий удовлетворяющих условиям</returns>
        public List<Guess> TakeListVersion(BestVersion bestVersion = null, RoleFilter roleFilter = null, GuessFilter clueFilter = null, PlayerFilter playerFilter = null)
        {
            List<Guess> guesses = new List<Guess>();
            if(playerFilter == null) playerFilter = a => true; 
            foreach (Opinion opinion in Opinions)
            {
                if (opinion.Player.IsLive && playerFilter(opinion.Player))
                {
                    Guess guess = opinion.TakeBastVersion(bestVersion, roleFilter, clueFilter);
                    if (guess != null)
                    {
                        guesses.Add(guess);
                    }
                }
            }
            return guesses;
        }
        /// <summary>
        /// Количесвто событий, которые игрок может запомнить
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Добавить новое воспоминание
        /// </summary>
        /// <param name="clue">добавляемое воспомниние</param>
        public void AddClue(Clue clue)
        {
            if (Clues.Count == Size)
            {
                this.ForgetOne();
            }

            Clues.Add(clue);
            if (clue.ExactClue)
            {
                this[clue.Player].MadeExact(clue.Role);
            }
            else
            {
                if (clue.Player != null && clue.Role != null)
                {
                    if (this[clue.Player][clue.Role].AddPoint(clue.Point))
                    {
                        this[clue.Player].GlobalPoint += clue.Point;
                    }
                }
            }
            Analize();
        }
        /// <summary>
        /// забыть наименее востребованное воспоминие
        /// </summary>
        public void ForgetOne()
        {
            Clues.Sort();
            Clue clue = Clues[0];

            if (clue.Player != null && clue.Role != null)
            {
                if (clue.Player != null && clue.Role != null)
                {
                    if (this[clue.Player][clue.Role].AddPoint(clue.Point * -1))
                    {
                        this[clue.Player].GlobalPoint -= clue.Point;
                    }

                }
            }
            Clues.Remove(clue);
        }
        /// <summary>
        /// Запустить череду рассуждений, во время которых методом исключения игрок попытаеться узнать несколько ролей
        /// </summary>
        private void Analize()
        {
            bool triger = true;
            while (triger)
            {
                triger = AnalizeStep();
            }
            foreach(Opinion opinion in Opinions)
            {
                opinion.Balance();
            }
        }
        /// <summary>
        /// Запустить следующий шаг рассуждйний
        /// </summary>
        /// <returns>привели ли рассуждения к результату</returns>
        private bool AnalizeStep()
        {
            bool work = false;

            foreach (RoleCounter roleCounter in Game.RoleList)
            {
                int maxCount = roleCounter.MaxCount;
                int cutentCount = 0;
                List<Guess> guesses = new List<Guess>();

                foreach (Player player in Game.Players)
                {
                    if (this[player][roleCounter.Role].Chance == 1)
                    {
                        cutentCount++;
                    }
                    else if (this[player][roleCounter.Role].Chance != 0)
                    {
                        guesses.Add(this[player][roleCounter.Role]);
                    }
                }

                if (maxCount == cutentCount && guesses.Count != 0)
                {
                    foreach (Guess guess in guesses)
                    {
                        guess.MadeStatic(false);
                    }
                }


                cutentCount = 0;
                List<Opinion> opinions = new List<Opinion>();

                foreach (Player player in Game.Players)
                {
                    if (this[player][roleCounter.Role].Chance == 0)
                    {
                        cutentCount++;
                    }
                    else if (this[player][roleCounter.Role].Chance != 1)
                    {
                        opinions.Add(this[player]);
                    }
                }

                if (maxCount == Game.Players.Count - cutentCount && opinions.Count != 0)
                {
                    foreach (Opinion opinion in opinions)
                    {
                        opinion.MadeExact(roleCounter.Role);
                    }
                }
            }
            return work;

        }
        /// <summary>
        /// Получить все догадки для заданного игрока 
        /// </summary>
        /// <param name="player">заданный игрок</param>
        /// <returns>догадки для заданного игрока </returns>
        public Opinion this[Player player]
        {
            get
            {
                foreach (Opinion opinion in this.Opinions)
                {
                    if (opinion.Player == player)
                    {
                        return opinion;
                    }
                }
                throw new Exception("данного инрока нет в игре");
            }
        }
    }
    /// <summary>
    /// Воспоминия о событиях в игре
    /// </summary>
    public class Clue : IComparable<Clue>
    {
        /// <summary>
        /// Создание новго воспоминания
        /// </summary>
        /// <param name="happening">к какому событию относиться воспоминание</param>
        /// <param name="important">важность воспоминания</param>
        /// <param name="point">количество уверенности добавляемое к одной из догадок</param>
        /// <param name="player">игрок к которому относиться воспомниние</param>
        /// <param name="role">роль игрока, которую подтверждает данное воспомнинание</param>
        /// <param name="exactClue">являеться ли воспомниание достоверно верным</param>
        public Clue(Happening happening, int important = 0, int point = 0,  Player player = null, Role role = null, bool exactClue = false)
        {
            Happenings = new List<Happening>();
            Happenings.Add(happening);
            Player = player;
            Point = point;
            Role = role;
            Important = important;
            ExactClue = exactClue;
        }
        /// <summary>
        /// к каким событиям относиться воспоминание
        /// </summary>
        public List<Happening> Happenings { get; set; }
        /// <summary>
        /// игрок к которому относиться воспомниние
        /// </summary>
        public Player Player { get; set; }
        /// <summary>
        /// роль игрока, которую подтверждает данное воспомнинание
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        /// количество уверенности добавляемое к одной из догадок
        /// </summary>
        public int Point { get; set; }
        /// <summary>
        /// важность воспоминания
        /// </summary>
        public int Important { get; set; }
        /// <summary>
        /// являеться ли воспомниание достоверно верным
        /// </summary>
        public bool ExactClue { get; set;}
        public int CompareTo(Clue other)
        {
            return this.Important.CompareTo(other.Important);
            
        }
    }

}