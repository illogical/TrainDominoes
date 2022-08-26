using Assets.Scripts.Game;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public MeshManager MeshManager = null;
    public LayoutManager LayoutManager = null;
    public TurnManager TurnManager = null;
    [Header("Events")]
    public SelectionEvent DominoClicked;
    public SelectionEvent PlayerDominoSelected;
    public SelectionEvent EngineDominoSelected;
    public SelectionEvent CreateTrackWithDomino;

    [HideInInspector]
    public DominoTracker DominoTracker = new DominoTracker();

    public int GetDominoCountPerPlayer(int playerCount)
    {
        // Up to 4 players take 15 dominoes each, 5 or 6 take 12 each, 7 or 8 take 10 each.
        if(playerCount <= 4) { return 15; }
        else if (playerCount <= 6) { return 12; }
        else return 10;
    }

    public GameObject GetNewPlayerDomino()
    {
        var dominoInfo = DominoTracker.GetDominoFromBonePile();
        return MeshManager.GetPlayerDomino(MeshManager.PlayerDominoPrefab, dominoInfo, LayoutManager.PlayerBottomCenter);
    }

    public GameObject GetNewEngineDomino()
    {
        var dominoInfo = DominoTracker.GetNextEngineAndCreateStation();
        return MeshManager.CreateEngineDomino(MeshManager.TableDominoPrefab, dominoInfo, Vector3.zero);
    }

    public void AddSelectedToNewTrack(int selectedDominoId)
    {
        //float trackSlideDuration = 0.3f;
        //int trackIndex = tracks.Count;
        //int selectedId = selectedObjectId.Value;
        //var currentObj = bottomGroup.PlayerObjects.GetObjectByKey(selectedId);
        //// positions the empty where the first object in the line will be placed
        //var trackLeftPosition = new Vector3(GetTrackStartXPosition(), GetTrackYPosition(trackIndex, tracks.Count + 1), 0);


        // TODO: should flip?
        if(!CompareDominoes(selectedDominoId, DominoTracker.GetEngineDominoID())) { return; }

        // add domino to track
        var newTrack = DominoTracker.Station.AddTrack(selectedDominoId);

        var startPosition = LayoutManager.GetEnginePosition(MeshManager.GetEngineDomino());
        // move domino to this position
        var currentDomino = MeshManager.GetDominoMeshById(selectedDominoId);

        // TODO: this needs to be done from a TargetRPC for the client to do it
        LayoutManager.PlaceDominoOnTrack(currentDomino, startPosition, newTrack.DominoIds.Count - 1,
            () => Debug.Log("Pretend both players can see this"));   // TODO: MeshManager to move/rotate domino

        // move empties to move the lines and animate the selected box moving to the track
        //StartCoroutine(AddNewBoxAndUpdateTrackPositions(currentObj, trackLeftPosition, trackSlideDuration,
        //    () => newTrack.AddToTrackAndParent(selectedId, ref currentObj)));

        // swap authority
        // TODO: destroy player's GameObject from MeshManager
        // TODO: create a new GameObject as "usedDomino" prefab to give it server authority and both players can see it
        //bottomGroup.PlayerObjects.Remove(selectedId);

        // TODO: deselect domino. Raise an event instead?
        //selectedObjectId = null;
    }

    public bool CompareDominoes(int playerSelectedDominoId, int trackDominoID)
    {
        var trackDomino = DominoTracker.GetDominoByID(trackDominoID);
        var selectedDomino = DominoTracker.GetDominoByID(playerSelectedDominoId);

        // take into account flipped track dominoes
        var trackScoreToCompare = trackDomino.Flipped ? trackDomino.BottomScore : trackDomino.TopScore;

        // TODO: fix this after the domino knows if it wants to be flipped
        //return trackScoreToCompare == selectedDomino.BottomScore
        //|| trackScoreToCompare == selectedDomino.TopScore;
        return trackDomino.TopScore == selectedDomino.BottomScore
        || trackDomino.BottomScore == selectedDomino.TopScore
        || trackDomino.TopScore == selectedDomino.TopScore
        || trackDomino.BottomScore == selectedDomino.BottomScore;
    }

    /// <summary>
    /// Always use TopScore of an unflipped destination domino and the BottomScore of an unflipped playerDomino.
    /// </summary>
    /// <param name="playerDomino">The player's selected domino from their hand.</param>
    /// <param name="destinationDomino">Another domino.</param>
    /// <returns></returns>
    public bool IsDominoFlipNeeded(DominoEntity playerDomino, DominoEntity destinationDomino)   // TODO: move this to mesh manager? Not sure I like how this works.
    {
        var destinationScore = destinationDomino.Flipped
            ? destinationDomino.BottomScore : destinationDomino.TopScore;

        // bottom score by default
        var playerScore = playerDomino.Flipped
            ? playerDomino.TopScore : playerDomino.BottomScore;

        return destinationScore != playerScore;
    }

    public void EndTurn(int callerNetId)
    {
        if (!TurnManager.IsPlayerTurn(callerNetId)) { return; }

        TurnManager.NextTurn();

        Debug.Log($"It is Player {TurnManager.GetCurrentPlayerId()}'s turn. It was {callerNetId}'s turn.");
    }
}
