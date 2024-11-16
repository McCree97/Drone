using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleToUI : MonoBehaviour
{
    public TMP_Text consoleOutput; 
    public Button clearButton; 

    private Queue<string> logMessages = new Queue<string>(); 
    private int maxMessages = 20; 

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog; 
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog; 
    }

    private void Start()
    {
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearConsole); 
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        
        logMessages.Enqueue(logString);

        
        if (logMessages.Count > maxMessages)
        {
            logMessages.Dequeue();
        }

        
        consoleOutput.text = string.Join("\n", logMessages.ToArray());
    }

    
    public void ClearConsole()
    {
        logMessages.Clear(); 
        consoleOutput.text = "";
    }
}
