using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.States
{
    /// <summary>
    /// The player has selected a domino
    /// </summary>
    public class PlayerSelectedPlayerDominoState : GameStateBase
    {
        private int? NextSelectedDomino; // player, engine, or track domino 


        public override string Name => nameof(PlayerSelectedPlayerDominoState);
        public override void EnterState(GameStateContext ctx)
        {
            NextSelectedDomino = null; // TODO: check if dominoManager.SelectedPlayerDomino is null

            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.AddListener(SelectPlayerDomino);
        }

        public override void UpdateState(GameStateContext ctx)
        {
            if (!NextSelectedDomino.HasValue)
            {
                return;
            }

            int? lastSelectedDominoId = ctx.GameplayManager.DominoTracker.SelectedDomino;
            ctx.GameplayManager.DominoTracker.SetSelectedDomino(NextSelectedDomino.Value);

            // TODO: ensure the clicked domino is the player's (rather than a table domino)
            ctx.Player.CmdSelectPlayerDomino(NextSelectedDomino.Value, lastSelectedDominoId);
            ctx.SwitchState(ctx.PlayerSelectedPlayerDominoState);


            //var playerSelectedDominoID = gameStateContext.GameplayManager.GetSelectedDominoID();

            // same domino was selected again
            //if (NextSelectedDomino.Value == playerSelectedDominoID.Value)
            //{
            //    // TODO: same player domino that was select is selected again. Just deselect it and change state back to PlayerTurnStartedState (and maybe rename that to PlayerIdleState)
            //    return;
            //}

            // a different player domino was selected
            //if (gameStateContext.GameplayManager.IsPlayerDomino(NextSelectedDomino.Value))
            //{
            //    // TODO: deselect current domino and select new domino. Don't change states OR change to this state again? Should probably prevent changing a state to the same state over and over?
            //    gameStateContext.GameplayManager.SelectDomino(NextSelectedDomino.Value);
            //    NextSelectedDomino = null; // start this state over again
            //    return;
            //}

            // TODO: new state after a Domino is placed? Might want a PlayerIdleAfterSelection to have additional options for the player to modify their choices


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

            //var trackIndex = gameStateContext.GameplayManager.GetTrackIndexByDominoId(NextSelectedDomino.Value);
            //if (!trackIndex.HasValue)
            //{
            //    // not a domino from a track
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

        public override void LeaveState(GameStateContext gameStateContext)
        {
            gameStateContext.GameplayManager.PlayerDominoSelected?.OnEventRaised.RemoveListener(SelectPlayerDomino);
        }

        private void SelectPlayerDomino(int dominoId)
        {
            NextSelectedDomino = dominoId;
        }
    }
}
