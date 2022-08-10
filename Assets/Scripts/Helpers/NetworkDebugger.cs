using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Text;

public static class NetworkDebugger
{
    public static void OutputAuthority(NetworkBehaviour b, string caller, bool showServerStatus = false)
    {
        StringBuilder output = new StringBuilder($"<b>{caller}</b> from ");

        if (b.isServer && b.isClient)
        {
            output.Append("<color=magenta>server</color> AND <color=green>client</color>");
        }
        else if(b.isServer)
        {
            output.Append("<color=magenta>server</color>");
        }
        else if (b.isClient)
        {
            output.Append("<color=green>client</color>");
        }

        if (b.hasAuthority)
        {
            output.AppendLine("<color=cyan> has authority</color>");
        }
        else
        {
            output.AppendLine(string.Empty);
        }

        if (showServerStatus)
        {
            output.AppendLine(NetworkServer.active ? "Server is <b>active</b>" : "Server is <b>NOT</b> active");
        }

        Debug.Log(output.ToString());
    }
}
