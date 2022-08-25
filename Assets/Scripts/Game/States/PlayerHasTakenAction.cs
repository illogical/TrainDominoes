using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.States
{
    /// <summary>
    /// Player has taken the necessary action to be able to end their turn (or change their minds?) such as playing a domino or drawing one.
    /// </summary>
    public class PlayerHasTakenAction : GameStateBase
    {
        public PlayerHasTakenAction(GameStateContext gameContext) : base(gameContext) { }

        public override string Name => nameof(PlayerHasTakenAction);

        public override void EnterState()
        {
            // TODO: enable the End Turn button
            // TODO: if this is the first turn, should this change to a state to allow selection of additional dominoes?
        }

        public override void UpdateState()
        {

        }

        public override void LeaveState()
        {
            // TODO: disable the End Turn button
        }

    }
}
