using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UIActions : MonoBehaviour
{
    public void OnDrawClick()
    {
        NetworkIdentity identity = NetworkClient.connection.identity;
        var dominoPlayer = identity.GetComponent<DominoPlayer>();

        // can call this from a Monobehaviour!
        dominoPlayer.CmdDealDominoes(12);

    }

    public void OnEndTurnClick()
    {
        NetworkIdentity identity = NetworkClient.connection.identity;
        var dominoPlayer = identity.GetComponent<DominoPlayer>();

        dominoPlayer.CmdEndTurn((int)NetworkClient.connection.identity.netId);
    }

}
