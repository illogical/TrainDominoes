using UnityEngine;

namespace Assets.Scripts.Game.States
{
    /// <summary>
    /// The player's turn just began. A domino has not been selected yet.
    /// </summary>
    public class PlayerTurnStartedState : GameStateBase
    {
        public int? SelectedDominoId;
        public bool IsTrackDominoSelected;

        // TODO: how do we know if this player has a track yet?

        public override string Name => nameof(PlayerTurnStartedState);
        public override void EnterState(GameStateContext ctx)
        {
            SelectedDominoId = null;
            IsTrackDominoSelected = false;

            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.AddListener(SelectPlayerDomino);
        }

        public override void UpdateState(GameStateContext ctx)
        {
            if (!SelectedDominoId.HasValue)
            {
                return;
            }

            ctx.GameplayManager.DominoTracker.SetSelectedDomino(SelectedDominoId.Value);

            // TODO: ensure the clicked domino is the player's (rather than a table domino)
            ctx.Player.CmdSelectPlayerDomino(SelectedDominoId.Value, null);

            ctx.SwitchState(ctx.PlayerSelectedPlayerDominoState);

          
        }

        public override void LeaveState(GameStateContext gameStateContext)
        {
            gameStateContext.GameplayManager.PlayerDominoSelected?.OnEventRaised.RemoveListener(SelectPlayerDomino);
        }

        private void SelectPlayerDomino(int dominoId)
        {
            Debug.Log($"Domino {dominoId} selected in SelectPlayerDomino");
            SelectedDominoId = dominoId;
        }
    }
}