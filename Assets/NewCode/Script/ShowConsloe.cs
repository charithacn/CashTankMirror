using System;
using UnityEngine;
using UnityEngine.UI;

public class ShowConsole : MonoBehaviour
{
    public Text consoleText;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logText, string stackTrace, LogType logType)
    {
        string color;

        // Set color based on log type
        switch (logType)
        {
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                color = "<color=red>";
                break;
            case LogType.Warning:
                color = "<color=yellow>";
                break;
            default:
                color = "<color=white>";
                break;
        }

        // Append the log with color tags
        consoleText.text += color + logText + "</color>\n";
    }
}