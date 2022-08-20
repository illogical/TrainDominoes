using Assets.Scripts.Game;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public MeshManager MeshManager = null;
    public LayoutManager LayoutManager = null;
    public SelectionEvent DominoClicked;

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
        var dominoInfo = DominoTracker.GetNextEngine();
        return MeshManager.GetEngineDomino(MeshManager.TableDominoPrefab, dominoInfo, Vector3.zero);
    }
}
