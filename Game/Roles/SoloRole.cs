using Practic2020.GameCore;
using Practic2020.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Practic2020.Roles
{
    /// <summary>
    /// Класс одиночной роли расширяет возможности стандартной роли, добавляя дополнительное действие ночью
    /// </summary>
    public class SoloRole : Role
    {
        /// <summary>
        /// дополнительное действие ночью
        /// </summary>
        public virtual void StartAbility()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// роль детектива
    /// </summary>
    public class Detective : SoloRole
    {
        public Detective()
        {
            this.IsHiden = true;
            this.Side = Side.Peacful;
            this.Name = "Детекив";
        }
        /// <summary>
        /// Узнать роль одного из игроков
        /// </summary>
        override public void StartAbility(){
            Player truePlayer = null;
            foreach(Player player in Game.Players)
            {
                if (!player.IsSleep)
                {
                    truePlayer = player;
                }
            }
            try { 
                Player somePlayer = truePlayer.Memory.TakeListVersion(clueFilter: a => a.IsStatic == false)?.Last().Player;
                if (somePlayer != null) {
                    Happening happening = new OpenUp(Game, somePlayer, somePlayer.Role, true, true);
                    happening.ImpactOnPlayer(truePlayer);
                    Game.Log.MadeIvent(truePlayer.Name, "теперь я знаю что игрок " + somePlayer.Name + " - " + somePlayer.Role.Name);
                }
            }
            catch
            {

            }
        }


    }

    /// <summary>
    /// Роль маньяка
    /// </summary>
    public class Manaic : SoloRole
    {
        public Manaic()
        {
            this.IsHiden = true;
            this.Side = Side.Maniac;
            this.Name = "маньяк";
        }
        /// <summary>
        /// Убить одного из игроков на утро следующего дня
        /// </summary>
        override public void StartAbility()
        {
            Player truePlayer = null;
            foreach (Player player in Game.Players)
            {
                if (!player.IsSleep)
                {
                    truePlayer = player;
                }
            }
            Player somePlayer = truePlayer.Memory.TakeListVersion(roleFilter: a => a.IsFriend(this) == false )?.Last().Player;
            if (somePlayer != null)
            {
                Game.MorningHepening.Add(new MorningAction(somePlayer, a => { a.Kill(); Game.Log.MadeIvent("ведущий", somePlayer.Name + " был убит прошлой ночью"); }));
            }
        }


    }


}
