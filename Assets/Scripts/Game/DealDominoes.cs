using Mirror;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DealDominoes : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerGui = null;
    [SerializeField] private GameObject topPanel = null;

    void Start()
    {
        if(isServer)
        {
            RpcUpdateGuiTest();
        }
        else
        {
            CmdAddTopGui();
        }

        if (hasAuthority)
        {
            topPanel.SetActive(true);
            // TODO: get current player and display their name up top
            //((MyNetworkManager)NetworkManager.singleton).Players.Where(p => p.id);
            NetworkIdentity identity = NetworkClient.connection.identity;
            var dominoPlayer = identity.GetComponent<DominoPlayer>();
            playerGui.text = dominoPlayer.GetDisplayName();
        }
    }

    void Update()
    {
        
    }

    #region Server

    public override void OnStartServer()
    {
        RpcUpdateGuiTest();
    }

    [Command]
    public void CmdAddTopGui()
    {
        RpcUpdateGuiTest();
    }

    #endregion Server


    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
        RpcUpdateGuiTest();

    }

    [ClientRpc]
    public void RpcUpdateGuiTest()
    {
        if(hasAuthority)
        {
            topPanel.SetActive(true);
            // TODO: get current player and display their name up top
            //((MyNetworkManager)NetworkManager.singleton).Players.Where(p => p.id);
            NetworkIdentity identity = NetworkClient.connection.identity;
            var dominoPlayer = identity.GetComponent<DominoPlayer>();
            playerGui.text = dominoPlayer.GetDisplayName();
        }
    }

    #endregion Client
}
