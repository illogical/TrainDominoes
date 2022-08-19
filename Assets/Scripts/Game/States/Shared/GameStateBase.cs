namespace Assets.Scripts.Game.States
{
    public abstract class GameStateBase
    {
        /// <summary>
        /// Setup current state
        /// </summary>
        /// <param name="gameStateContext"></param>
        public abstract void EnterState(GameStateContext gameStateContext);
        /// <summary>
        /// Main game loop during this state
        /// </summary>
        /// <param name="gameStateContext"></param>
        public abstract void UpdateState(GameStateContext gameStateContext);
        /// <summary>
        /// Cleanup before switching to the next state
        /// </summary>
        /// <param name="gameStateContext"></param>
        public abstract void LeaveState(GameStateContext gameStateContext);
    }
}
