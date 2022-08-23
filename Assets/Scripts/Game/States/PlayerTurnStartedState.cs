using UnityEngine;

namespace Assets.Scripts.Game.States
{
    /// <summary>
    /// The player's turn just began. A domino has not been selected yet.
    /// </summary>
    public class PlayerTurnStartedState : GameStateBase
    {
        public bool DominoSelecting;
        public int? ClickedDominoId;
        public int? SelectedPlayerDominoId;

        // TODO: how do we know if this player has a track yet?

        public override string Name => nameof(PlayerTurnStartedState);
        public override void EnterState(GameStateContext ctx)
        {
            ClickedDominoId = null;

            DominoSelecting = false;

            ctx.GameplayManager.DominoClicked?.OnEventRaised.AddListener(DominoClicked);
            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.AddListener(SelectPlayerDomino);
        }

        public override void UpdateState(GameStateContext ctx)
        {
            if (ClickedDominoId.HasValue && DominoSelecting)
            {
                ctx.Player.CmdDominoClicked(ClickedDominoId.Value);
                DominoSelecting = false;
                return;
            }

            if (!SelectedPlayerDominoId.HasValue)
            {
                return;
            }

            ctx.GameplayManager.DominoTracker.SetSelectedDomino(SelectedPlayerDominoId.Value);

            // TODO: ensure the clicked domino is the player's (rather than a table domino)
            ctx.Player.CmdSelectPlayerDomino(SelectedPlayerDominoId.Value, null);

            ctx.SwitchState(ctx.PlayerSelectedPlayerDominoState);

          
        }

        public override void LeaveState(GameStateContext ctx)
        {
            ctx.GameplayManager.DominoClicked?.OnEventRaised.RemoveListener(DominoClicked);
            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.RemoveListener(SelectPlayerDomino);
        }

        private void DominoClicked(int dominoId)
        {
            DominoSelecting = true;
            ClickedDominoId = dominoId;
        }

        private void SelectPlayerDomino(int dominoId)
        {
            Debug.Log($"Domino {dominoId} selected in SelectPlayerDomino");
            SelectedPlayerDominoId = dominoId;
        }
    }
}