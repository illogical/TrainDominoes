using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : NetworkBehaviour
{
    private DominoPlayer dominoPlayer;

    private void Start()
    {
        // TODO: this almost works. Major flaw though. The client-only loses track of the domino that the server/client has.
        //NetworkIdentity identity = NetworkClient.connection.identity;

        //dominoPlayer = identity.GetComponent<DominoPlayer>();
        //dominoPlayer.CmdDealDomino();
    }

    #region Server

    #endregion Server


    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    #endregion Client
}
