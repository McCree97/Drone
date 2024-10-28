using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleToUI : MonoBehaviour
{
    public TMP_Text consoleOutput; // Reference to the TextMeshPro text field on the UI
    
    private Queue<string> logMessages = new Queue<string>(); // Queue to store log messages
    private int maxMessages = 20; // Limit of messages to display at once

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog; // Subscribe to the log message event
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog; // Unsubscribe when not in use
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Add log message to the queue
        logMessages.Enqueue(logString);

        // If the queue exceeds the max, remove the oldest message
        if (logMessages.Count > maxMessages)
        {
            logMessages.Dequeue();
        }

        // Update the console output display
        consoleOutput.text = string.Join("\n", logMessages.ToArray());
    }
}