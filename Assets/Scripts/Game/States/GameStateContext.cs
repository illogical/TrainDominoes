namespace Assets.Scripts.Game.States
{
    public class GameStateContext
    {
        // events that could happen during any state
        public GameStartedState GameStartedState = new GameStartedState();
        public PlayerTurnStartedState PlayerTurnStartedState = new PlayerTurnStartedState();

        public DominoPlayer Player { get; private set; }

        private GameStateBase currentState;


        public GameStateContext(DominoPlayer player)
        {
            Player = player;

            currentState = GameStartedState;

            currentState.EnterState(this);
        }

        public void Update() // TODO: don't forget to run this from GameSession.Update()
        {
            currentState.UpdateState(this);
        }

        public void SwitchState(GameStateBase state)
        {
            if (currentState == state)
            {
                // only switch states to a different state. May want to add another abstract function Reset as the place to reset local variables for that state
                return;
            }

            currentState.LeaveState(this);
            currentState = state;
            state.EnterState(this);
        }
    }
}