using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
public class DroneSearchUI : MonoBehaviour
{
    public TMP_InputField droneInputField;  
    public TMP_InputField inputDFS;  
    public TMP_Text resultText;  
    public TMP_InputField inputStartDroneID;   // Start drone input field
    public TMP_InputField inputEndDroneID;     // End drone input field
    public Button showPathButton;              // Button to trigger path finding
    
    public TMP_Text displayPathText;
    public TMP_Text displayDFS;  
    public Button DFSButton;
    public Button searchButton;  
    public Button searchDroneButton;  
    public TMP_InputField searchInputField;  
    public TMP_Text displayText;  
    public DroneNetworkCommunication partition1Network;
    public DroneNetworkCommunication partition2Network;


    public Button searchBatteryLevelButton;  
    public TMP_InputField batteryLevelInputField;  

    public DroneBTComm partition1Comm;  
    public DroneBTComm partition2Comm;  
    public int totalDroneCount = 75;  

    private void Start()
    {
        
        partition1Comm = new DroneBTComm();
        partition2Comm = new DroneBTComm();
        partition1Network = new DroneNetworkCommunication();
        partition2Network = new DroneNetworkCommunication();
        DFSButton.onClick.AddListener(OnDFSButtonClick);
        CreateAndInsertDrones();

        // Establish communication links in both partitions
        partition1Network.EstablishLinks();
        partition2Network.EstablishLinks();
        
       showPathButton.onClick.AddListener(OnShowPathButtonClick);
        // Display the communication links for each partition
      
        
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
      private void OnDFSButtonClick()
    {
        string searchID = inputDFS.text;

        if (string.IsNullOrEmpty(searchID))
        {
            displayDFS.text = "Please enter a Drone ID to search.";
            return;
        }

        // Perform DFS in Partition 1
        string result = partition1Network.DFS(searchID);
        if (result.Contains("not found"))
        {
            // If not found in Partition 1, search in Partition 2
            result = partition2Network.DFS(searchID);
        }

        displayDFS.text = result;
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
                partition1Network.AddDrone(newDrone);
            }
            else
            {
                partition2Comm.InsertDrone(newDrone);  
                partition2Network.AddDrone(newDrone);
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
     private void OnShowPathButtonClick()
{
    string startDroneID = inputStartDroneID.text;
    string endDroneID = inputEndDroneID.text;

    if (string.IsNullOrEmpty(startDroneID) || string.IsNullOrEmpty(endDroneID))
    {
        displayPathText.text = "Please enter both start and end drone IDs.";
        return;
    }

    string pathResult = partition1Network.FindShortestPath(startDroneID, endDroneID);
    if (pathResult.Contains("not found"))
    {
        pathResult = partition2Network.FindShortestPath(startDroneID, endDroneID);
    }

    displayPathText.text = pathResult;
}


    private DroneNetworkCommunication GetNetworkForDrone(string droneID)
    {
        // Check if the drone belongs to partition 1 or partition 2 network
        DroneNetworkCommunication droneNetwork = partition1Network.GetDroneByID(droneID);
        if (droneNetwork != null)
            return partition1Network;

        droneNetwork = partition2Network.GetDroneByID(droneID);
        if (droneNetwork != null)
            return partition2Network;

        return null;
    }

    public List<string> FindShortestPath(DroneNetworkCommunication network, string startID, string endID)
    {
        // Perform DFS to find the shortest path from startID to endID
        Stack<string> pathStack = new Stack<string>();
        HashSet<string> visited = new HashSet<string>();
        List<string> shortestPath = new List<string>();

        if (DFS(network, startID, endID, visited, pathStack, ref shortestPath))
        {
            return shortestPath;
        }
        return new List<string>();
    }

    private bool DFS(DroneNetworkCommunication network, string currentDroneID, string targetDroneID,
                    HashSet<string> visited, Stack<string> pathStack, ref List<string> shortestPath)
    {
        // Mark the current drone as visited
        visited.Add(currentDroneID);
        pathStack.Push(currentDroneID);

        // If we've found the target drone, update the shortest path
        if (currentDroneID == targetDroneID)
        {
            shortestPath = new List<string>(pathStack);
            return true;
        }

        // Iterate through connected drones and continue DFS
        DroneNetworkCommunication currentDrone = network.GetDroneByID(currentDroneID);
        foreach (var connectedDrone in currentDrone.GetConnectedDrones)
{
    if (!visited.Contains(connectedDrone.Drone.DroneID))  // Access DroneID from the Drone object
    {
        if (DFS(network, connectedDrone.Drone.DroneID, targetDroneID, visited, pathStack, ref shortestPath))
            return true;
    }
}

        // Backtrack if no path is found
        pathStack.Pop();
        return false;
    }
}
