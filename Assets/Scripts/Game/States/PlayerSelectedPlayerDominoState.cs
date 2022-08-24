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
        public bool DominoSelecting;
        public bool EngineDominoSelecting;
        public bool NewTrackAdding;
        public int? ClickedDominoId;
        public int? NextSelectedDomino; // player, engine, or track domino 


        public override string Name => nameof(PlayerSelectedPlayerDominoState);
        public override void EnterState(GameStateContext ctx)
        {
            DominoSelecting = false;
            EngineDominoSelecting = false;
            NewTrackAdding = false;

            NextSelectedDomino = null; // TODO: check if dominoManager.SelectedPlayerDomino is null

            ctx.GameplayManager.DominoClicked?.OnEventRaised.AddListener(DominoClicked);
            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.AddListener(SelectPlayerDomino);
            ctx.GameplayManager.EngineDominoSelected?.OnEventRaised.AddListener(SelectEngineDomino);
            ctx.GameplayManager.CreateTrackWithDomino?.OnEventRaised.AddListener(AddDominoToTrack);
        }

        public override void UpdateState(GameStateContext ctx)
        {
            if (ClickedDominoId.HasValue && DominoSelecting)
            {
                ctx.Player.CmdDominoClicked(ClickedDominoId.Value);
                DominoSelecting = false;
                return;
            }

            if (!NextSelectedDomino.HasValue)
            {
                return;
            }

            int? previouslySelectedDominoId = ctx.GameplayManager.DominoTracker.SelectedDomino;

            if (NewTrackAdding)
            {
                ctx.Player.CmdAddDominoToNewTrack(previouslySelectedDominoId.Value, NextSelectedDomino.Value);
                NewTrackAdding = false;
                // the player can end their turn
                ctx.SwitchState(ctx.PlayerHasTakenAction);
                return;
            }

            if (previouslySelectedDominoId.HasValue && EngineDominoSelecting)
            {
                ctx.Player.CmdEngineClicked(previouslySelectedDominoId.Value);
                EngineDominoSelecting = false;
                return;
            }
            else if(EngineDominoSelecting) // player clicked a domino that was not in hand
            {
                Debug.Log("Choose your domino first");
                EngineDominoSelecting = false;
                return;
            }

            
            ctx.GameplayManager.DominoTracker.SetSelectedDomino(NextSelectedDomino.Value);

            // TODO: ensure the clicked domino is the player's (rather than a table domino)
            ctx.Player.CmdSelectPlayerDomino(NextSelectedDomino.Value, previouslySelectedDominoId);
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

        public override void LeaveState(GameStateContext ctx)
        {
            ctx.GameplayManager.DominoClicked?.OnEventRaised.RemoveListener(DominoClicked);
            ctx.GameplayManager.PlayerDominoSelected?.OnEventRaised.RemoveListener(SelectPlayerDomino);
            ctx.GameplayManager.EngineDominoSelected?.OnEventRaised.RemoveListener(SelectEngineDomino);
            ctx.GameplayManager.CreateTrackWithDomino?.OnEventRaised.RemoveListener(AddDominoToTrack);
        }

        private void DominoClicked(int dominoId)
        {
            DominoSelecting = true;
            ClickedDominoId = dominoId;
        }

        private void SelectPlayerDomino(int dominoId)
        {
            NextSelectedDomino = dominoId;
        }

        private void SelectEngineDomino(int dominoId)
        {
            EngineDominoSelecting = true;
            NextSelectedDomino = dominoId;
        }

        private void AddDominoToTrack(int dominoId)
        {
            NewTrackAdding = true;
        }
    }
}
