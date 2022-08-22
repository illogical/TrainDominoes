using UnityEngine;

namespace Assets.Scripts.Game.States
{
    public class GameStartedState : GameStateBase
    {

        public override string Name => nameof(GameStartedState);
        public override void EnterState(GameStateContext gameStateContext)
        {
            Debug.Log("STATE: PlayerTurnStartedState");

            gameStateContext.Player.CmdDealDominoes(12);    // TODO: wondering if GameplayManager should contain the logic for determining how many dominoes to deal to each player
        }

        public override void UpdateState(GameStateContext gameStateContext)
        {
            gameStateContext.SwitchState(gameStateContext.PlayerTurnStartedState);
        }

        public override void LeaveState(GameStateContext gameStateContext)
        {

        }
    }
}