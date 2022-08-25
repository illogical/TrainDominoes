using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.States
{
    /// <summary>
    /// The player has selected a domino. 
    /// The player could select a different domino.
    /// The player could select the engine to attempt to add a new track.
    /// The player could select any domino on a track to attempt to add the domino to the end of the track.
    /// </summary>
    public class PlayerSelectedPlayerDominoState : GameStateBase
    {
        public int? ClickedDominoId;
        public int? NextSelectedDomino; // player, engine, or track domino 

        public PlayerSelectedPlayerDominoState(GameStateContext gameContext) : base(gameContext) { }

        public override string Name => nameof(PlayerSelectedPlayerDominoState);
        public override void EnterState()
        {
            NextSelectedDomino = null;

            ctx.GameplayManager.DominoClicked?.OnEventRaised.AddListener(DominoClicked);
            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.AddListener(SelectPlayerDomino);
            ctx.GameplayManager.EngineDominoSelected?.OnEventRaised.AddListener(SelectEngineDomino);
            ctx.GameplayManager.CreateTrackWithDomino?.OnEventRaised.AddListener(AddDominoToTrack);
        }

        public override void UpdateState()
        {
            //if (gameStateContext.GameplayManager.IsEngineDomino(NextSelectedDomino.Value) && gameStateContext.GameplayManager.CompareDominoes(playerSelectedDominoID.Value, NextSelectedDomino.Value))
            //{
            //    // player clicked the engine domino with a match
            //    var playerSelectedDomino = gameStateContext.GameplayManager.GetDominoByID(playerSelectedDominoID.Value);
            //    var engineDomino = gameStateContext.GameplayManager.GetDominoByID(NextSelectedDomino.Value);

            //    gameStateContext.GameplayManager.FlipPlayerDominoIfNeeded(playerSelectedDomino, engineDomino);
            //    gameStateContext.GameplayManager.AddDominoToNewTrack(playerSelectedDomino.ID);

            //    gameStateContext.SwitchState(gameStateContext.IdlePlayerModifiedTrackState);
            //    return;
            //}

            //var trackTrailDomino = gameStateContext.GameplayManager.GetTailDomino(trackIndex.Value);
            //if (gameStateContext.GameplayManager.CompareDominoes(playerSelectedDominoID.Value, trackTrailDomino.ID))
            //{
            //    // player clicked a domino on a track and the player's selected domino matches the last domino on that track
            //    var playerSelectedDomino = gameStateContext.GameplayManager.GetDominoByID(playerSelectedDominoID.Value);

            //    gameStateContext.GameplayManager.FlipPlayerDominoIfNeeded(playerSelectedDomino, trackTrailDomino);
            //    gameStateContext.GameplayManager.AddDominoToExistingTrack(playerSelectedDominoID.Value, trackIndex.Value);

            //    gameStateContext.SwitchState(gameStateContext.IdlePlayerModifiedTrackState);
            //    return;
            //}
        }

        public override void LeaveState()
        {
            ctx.GameplayManager.DominoClicked?.OnEventRaised.RemoveListener(DominoClicked);
            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.RemoveListener(SelectPlayerDomino);
            ctx.GameplayManager.EngineDominoSelected?.OnEventRaised.RemoveListener(SelectEngineDomino);
            ctx.GameplayManager.CreateTrackWithDomino?.OnEventRaised.RemoveListener(AddDominoToTrack);
        }

        private void DominoClicked(int dominoId)
        {
            ClickedDominoId = dominoId;
            ctx.Player.CmdDominoClicked(ClickedDominoId.Value);
        }

        private void SelectPlayerDomino(int dominoId)
        {
            ctx.Player.CmdSelectPlayerDomino(dominoId, ctx.GameplayManager.DominoTracker.SelectedDomino);
            ctx.GameplayManager.DominoTracker.SetSelectedDomino(dominoId);
            NextSelectedDomino = dominoId;
        }

        private void SelectEngineDomino(int dominoId)
        {
            if (ctx.GameplayManager.DominoTracker.SelectedDomino.HasValue)
            {
                ctx.Player.CmdEngineClicked(ctx.GameplayManager.DominoTracker.SelectedDomino.Value);
            }
        }

        private void AddDominoToTrack(int dominoId)
        {
            ctx.Player.CmdAddDominoToNewTrack(ctx.GameplayManager.DominoTracker.SelectedDomino.Value, NextSelectedDomino.Value);
            // the player can end their turn
            ctx.SwitchState(ctx.PlayerHasTakenAction);
        }
    }
}
