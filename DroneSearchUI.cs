using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
public class DroneSearchUI : MonoBehaviour
{
    public TMP_InputField droneInputField;  
    public TMP_Text resultText;  
    public Button searchButton;  
    public Button searchDroneButton;  
    public TMP_InputField searchInputField;  
    public TMP_Text displayText;  

    public Button searchBatteryLevelButton;  
    public TMP_InputField batteryLevelInputField;  

    public DroneBTComm partition1Comm;  
    public DroneBTComm partition2Comm;  
    public int totalDroneCount = 75;  

    private void Start()
    {
        
        partition1Comm = new DroneBTComm();
        partition2Comm = new DroneBTComm();

        
        CreateAndInsertDrones();

        
        if (searchButton != null)
        {
            searchButton.onClick.AddListener(OnSearchButtonClick);
        }
        if (searchDroneButton != null)
        {
            searchDroneButton.onClick.AddListener(OnSearchDroneClick);
        }

        
        if (searchBatteryLevelButton != null)
        {
            searchBatteryLevelButton.onClick.AddListener(OnSearchByBatteryLevelClick);
        }
    }

    private void CreateAndInsertDrones()
    {
        for (int i = 0; i < totalDroneCount; i++)
        {
            string droneID = $"Drone_{i + 1}";

            
            Drone newDrone = new Drone(droneID)
            {
                BulletCount = Random.Range(1, 101),  
                BatteryLevel = Random.Range(0, 101)  
            };

            
            UnityEngine.Debug.Log($"Created {newDrone.DroneID} - BatteryLevel: {newDrone.BatteryLevel}, BulletCount: {newDrone.BulletCount}");

            
            if (newDrone.BulletCount > 50)
            {
                partition1Comm.InsertDrone(newDrone); 
            }
            else
            {
                partition2Comm.InsertDrone(newDrone);  
            }
        }
    }

 private void OnSearchButtonClick()
{
    string droneID = droneInputField.text;  
    string searchPath1 = "";  
    string searchPath2 = "";  
    Stopwatch stopwatch = new Stopwatch();

    
    stopwatch.Start();
    DroneBTCommunication droneInPartition1 = partition1Comm.FindDrone(droneID, out searchPath1);
    stopwatch.Stop();
    double partition1SearchTime = stopwatch.Elapsed.TotalMilliseconds;

    
    stopwatch.Restart();
    DroneBTCommunication droneInPartition2 = partition2Comm.FindDrone(droneID, out searchPath2);
    stopwatch.Stop();
    double partition2SearchTime = stopwatch.Elapsed.TotalMilliseconds;

    
    if (droneInPartition1 != null)
    {
        resultText.text = $" {droneID} found in Partition 1. BatteryLevel: {droneInPartition1.Drone.BatteryLevel}. Path: {searchPath1}. Time: {partition1SearchTime} ms";
    }
    else if (droneInPartition2 != null)
    {
        resultText.text = $" {droneID} found in Partition 2. BatteryLevel: {droneInPartition2.Drone.BatteryLevel}. Path: {searchPath2}. Time: {partition2SearchTime} ms";
    }
    else
    {
        resultText.text = $" {droneID} not found in either partition.";
    }
}



    private void OnSearchDroneClick()
    {
        string searchID = searchInputField.text;  
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        if (string.IsNullOrEmpty(searchID))
        {
            UnityEngine.Debug.Log("Please enter a Drone ID to search.");
            displayText.text += "Please enter a Drone ID to search.\n";
            return;
        }

        
        DroneBTCommunication droneInPartition1 = partition1Comm.FindDrone(searchID, out string searchPath1);
        DroneBTCommunication droneInPartition2 = partition2Comm.FindDrone(searchID, out string searchPath2);
        
        stopwatch.Stop();
        double elapsedTime = stopwatch.Elapsed.TotalSeconds;

        if (droneInPartition1 != null)
        {
            string message = $"Drone {searchID} found in Partition 1. BatteryLevel: {droneInPartition1.Drone.BatteryLevel}. Path: {searchPath1}";
            UnityEngine.Debug.Log(message);
            displayText.text += message + "\n";
        }
        else if (droneInPartition2 != null)
        {
            string message = $"Drone {searchID} found in Partition 2. BatteryLevel: {droneInPartition2.Drone.BatteryLevel}. Path: {searchPath2}";
            UnityEngine.Debug.Log(message);
            displayText.text += message + "\n";
        }
        else
        {
            string message = $"Drone {searchID} not found in either partition.";
            UnityEngine.Debug.Log(message);
            displayText.text += message + "\n";
        }
    }

    private void OnSearchByBatteryLevelClick()
{
    string batteryLevelText = batteryLevelInputField.text;  

    
    if (string.IsNullOrEmpty(batteryLevelText) || !int.TryParse(batteryLevelText, out int batteryLevel))
    {
        displayText.text = "Please enter a valid BatteryLevel to search.\n";
        return;
    }

    Stopwatch stopwatch = new Stopwatch();  
    stopwatch.Start();

    List<string> matchingDrones = new List<string>();

    
    List<DroneBTCommunication> allDrones = new List<DroneBTCommunication>();
    allDrones.AddRange(partition1Comm.GetAllDrones());
    allDrones.AddRange(partition2Comm.GetAllDrones());

    foreach (var droneComm in allDrones)
    {
        if (droneComm.Drone.BatteryLevel == batteryLevel)
        {
            matchingDrones.Add(droneComm.Drone.DroneID);
        }
    }

    stopwatch.Stop();  
    double elapsedTime = stopwatch.Elapsed.TotalMilliseconds; 

    
    if (matchingDrones.Count > 0)
    {
        displayText.text = $"Drones with BatteryLevel {batteryLevel}: {string.Join(", ", matchingDrones)}. Time taken: {elapsedTime} ms";
    }
    else
    {
        displayText.text = $"No drones found with BatteryLevel {batteryLevel}. Time taken: {elapsedTime} ms";
    }
}

}
