using Practic2020.Players;

namespace Practic2020.GameCore
{
    /// <summary>
    /// Описание действия происходящих с игороком отсрочкой (на утро следующего дня)
    /// </summary>
    public class MorningAction
    {
        /// <summary>
        /// игрок с которым происходит действие
        /// </summary>
        Player Player { get; set;}
        /// <summary>
        /// Обший вид действия
        /// </summary>
        /// <param name="player"></param>
        public delegate void Action(Player player);
        /// <summary>
        /// действие происходлящие с игроком
        /// </summary>
        public Action action { get; private set; }
        /// <summary>
        /// Создание действия с отсрочкой
        /// </summary>
        /// <param name="player">игрок с которым происходит действие</param>
        /// <param name="morningAction">действие происходлящие с игроком</param>
        public MorningAction(Player player, Action morningAction)
        {
            Player = player;
            action = morningAction;
        }
        /// <summary>
        /// Активация предзаписанного действия
        /// </summary>
        public void Do()
        {
            action(Player);
        }
    }
}
