using UnityEngine;

namespace Assets.Scripts.Game.States
{
    /// <summary>
    /// The player's turn just began. A domino has not been selected yet.
    /// </summary>
    public class PlayerTurnStartedState : GameStateBase
    {
        public int? SelectedPlayerDominoId;

        public PlayerTurnStartedState(GameStateContext gameContext) : base(gameContext) { }

        // TODO: how do we know if this player has a track yet?

        public override string Name => nameof(PlayerTurnStartedState);
        public override void EnterState()
        {
            SelectedPlayerDominoId = null;
            ctx.GameplayManager.DominoClicked?.OnEventRaised.AddListener(DominoClicked);
            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.AddListener(SelectPlayerDomino);
        }

        public override void UpdateState() { }

        public override void LeaveState()
        {
            ctx.GameplayManager.DominoClicked?.OnEventRaised.RemoveListener(DominoClicked);
            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.RemoveListener(SelectPlayerDomino);
        }

        private void DominoClicked(int dominoId)
        {
            ctx.Player.CmdDominoClicked(dominoId);
        }

        private void SelectPlayerDomino(int dominoId)
        {
            Debug.Log($"Domino {dominoId} selected in SelectPlayerDomino");
            SelectedPlayerDominoId = dominoId;

            ctx.GameplayManager.DominoTracker.SetSelectedDomino(SelectedPlayerDominoId.Value);
            ctx.Player.CmdSelectPlayerDomino(SelectedPlayerDominoId.Value, null);

            ctx.SwitchState(ctx.PlayerSelectedPlayerDominoState);
        }
    }
}