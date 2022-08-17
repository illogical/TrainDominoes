using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UIActions : NetworkBehaviour
{
    public void OnDrawClick()
    {
        // this had worked before trying to run this same command from a GameSession object
        NetworkIdentity identity = NetworkClient.connection.identity;
        var dominoPlayer = identity.GetComponent<DominoPlayer>();
        //dominoPlayer.CmdAddPlayerDomino();

        // TODO: how to instead call GameSession? Keep in mind that the clients do not have authority over GameSession (it exists in the sceen prior to game start)
        dominoPlayer.CmdAddPlayerDominoes(12);

    }

    public void OnEndTurnClick()
    {
        NetworkIdentity identity = NetworkClient.connection.identity;
        var dominoPlayer = identity.GetComponent<DominoPlayer>();

        dominoPlayer.CmdEndTurn((int)NetworkClient.connection.identity.netId);
    }

    #region Server

    #endregion Server


    #region Client

    #endregion Client
}
