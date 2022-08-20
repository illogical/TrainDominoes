namespace Assets.Scripts.Game.States
{
    public class GameStateContext
    {
        // events that could happen during any state
        public GameStartedState GameStartedState = new GameStartedState();
        public PlayerTurnStartedState PlayerTurnStartedState = new PlayerTurnStartedState();
        public PlayerSelectedPlayerDominoState PlayerSelectedPlayerDominoState = new PlayerSelectedPlayerDominoState();

        public DominoPlayer Player { get; private set; }
        public GameplayManager GameplayManager { get; private set; } 

        private GameStateBase currentState;


        public GameStateContext(DominoPlayer player, GameplayManager gameplayManager)
        {
            Player = player;

            currentState = GameStartedState;
            GameplayManager = gameplayManager;

            currentState.EnterState(this);
        }

        public void Update()
        {
            currentState.UpdateState(this);
        }

        public void SwitchState(GameStateBase state)
        {
            currentState.LeaveState(this);
            currentState = state;
            state.EnterState(this);
        }
    }
}