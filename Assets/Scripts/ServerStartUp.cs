using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ServerStartUp : MonoBehaviour
{
    const string _internalServerIP = "0.0.0.0";
    ushort _serverPort = 7777;

    void Start()
    {
        // bool server = false;
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dedicatedServer")
            {
                // server = true;
            }
            if (args[i] == "-port" && (i + 1 < args.Length))
            {
                _serverPort = (ushort)int.Parse(args[i + 1]);
            }
        }

#if SERVER
        if (server)
        {
            StartServer();
        }
#endif
    }

    void StartServer()
    {
        //NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData
        //    (_internalServerIP, _serverPort);
        //NetworkManager.Singleton.StartServer();
    }
}