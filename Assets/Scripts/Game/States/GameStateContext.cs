using UnityEngine;

namespace Assets.Scripts.Game.States
{
    public class GameStateContext
    {
        // events that could happen during any state
        public GameStartedState GameStartedState;
        public PlayerTurnStartedState PlayerTurnStartedState;
        public PlayerSelectedPlayerDominoState PlayerSelectedPlayerDominoState;
        public PlayerHasTakenAction PlayerHasTakenAction;

        public DominoPlayer Player { get; private set; }
        public GameplayManager GameplayManager { get; private set; } 

        private GameStateBase currentState;


        public GameStateContext(DominoPlayer player, GameplayManager gameplayManager)
        {
            Player = player;            
            GameplayManager = gameplayManager;

            GameStartedState = new GameStartedState(this);
            PlayerTurnStartedState = new PlayerTurnStartedState(this);
            PlayerSelectedPlayerDominoState = new PlayerSelectedPlayerDominoState(this);
            PlayerHasTakenAction = new PlayerHasTakenAction(this);

            currentState = GameStartedState;

            currentState.EnterState();
        }

        public void Update()
        {
            currentState.UpdateState();
        }

        public void SwitchState(GameStateBase state)
        {
            currentState.LeaveState();
            currentState = state;

            LogStateChanged(state.Name);

            state.EnterState();
        }

        public void LogStateChanged(string stateName)
        {
            Debug.Log($"<b><color=green>STATE:</color> {stateName}</b>");
        }
    }
}