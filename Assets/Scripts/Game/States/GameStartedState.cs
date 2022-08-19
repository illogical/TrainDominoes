using UnityEngine;

namespace Assets.Scripts.Game.States
{
    public class GameStartedState : GameStateBase
    {
        public override void EnterState(GameStateContext gameStateContext)
        {
            //gameStateContext.GameplayManager.BeginGame();
            Debug.Log("STATE: PlayerTurnStartedState");
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