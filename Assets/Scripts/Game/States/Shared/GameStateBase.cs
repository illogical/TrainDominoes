namespace Assets.Scripts.Game.States
{
    public abstract class GameStateBase
    {
        /// <summary>
        /// Used for logging state changes
        /// </summary>
        public abstract string Name { get; }
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
