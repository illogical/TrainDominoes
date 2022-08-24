using UnityEngine;

namespace Assets.Scripts.Game.States
{
    public class GameStartedState : GameStateBase
    {
        public GameStartedState(GameStateContext gameContext) : base(gameContext) { }

        public override string Name => nameof(GameStartedState);
        public override void EnterState()
        {
            ctx.Player.CmdDealDominoes(12);    // TODO: wondering if GameplayManager should contain the logic for determining how many dominoes to deal to each player
        }

        public override void UpdateState()
        {
            ctx.SwitchState(ctx.PlayerTurnStartedState);
        }

        public override void LeaveState()
        {

        }
    }
}