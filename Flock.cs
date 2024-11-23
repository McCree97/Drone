﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using TMPro;


public class Flock : MonoBehaviour
{
     public TMP_Text statusText;
    public Button destroyAllButton;
    public TMP_InputField droneInputField; 
    public Button selfDestructButton;
    public Button showLocationsButton;
    public Drone agentPrefab;
    List<Drone> agents = new List<Drone>();
    public FlockBehavior behavior;

    private DroneCommunication partition1Head;
    private DroneCommunication partition2Head;
    private Queue<string> logMessages = new Queue<string>(); // Queue to store log messages
    
    public TMP_InputField startDroneInputField;
    public TMP_InputField endDroneInputField;
    public Button showPathButton;
    public LineRenderer lineRenderer;

    
    

    [Range(10, 5000)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 2f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
    private float bulletCountUpdateInterval = 5f; 
    private float bulletCountTimer = 0f; 

void Start()
{
    foreach (Drone agent in agents)
    {
        agent.BulletCount = UnityEngine.Random.Range(1, 101); 
        UpdateDroneColor(agent); 
    }

    if (agentPrefab == null)
    {
        return; 
    }
    
     if (statusText != null)
        {
            statusText.text = "";
        }
    

    squareMaxSpeed = maxSpeed * maxSpeed;
    squareNeighborRadius = neighborRadius * neighborRadius;
    squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

    for (int i = 0; i < startingCount; i++)
    {
        Drone newAgent = Instantiate(
            agentPrefab,
            UnityEngine.Random.insideUnitCircle * startingCount * AgentDensity,
            Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(0f, 360f)),
            transform
        );

        newAgent.name = "Agent " + i;
        newAgent.DroneID = "Drone_" + i; 
        newAgent.Initialize(this);
        newAgent.BulletCount = UnityEngine.Random.Range(0, 100); 
        agents.Add(newAgent);

        if (i < startingCount / 2)
        {
            partition1Head = AddToCommunicationList(partition1Head, newAgent);
        }
        else
        {
            partition2Head = AddToCommunicationList(partition2Head, newAgent);
        }
    }
if (destroyAllButton != null)
    {
        destroyAllButton.onClick.AddListener(DestroyAllDrones);
    }
    selfDestructButton.onClick.AddListener(OnSelfDestructButtonClick);
    showLocationsButton.onClick.AddListener(DisplayAllDroneLocations); 
    showPathButton.onClick.AddListener(OnShowPathButtonClick);
   
}

private void OnSelfDestructButtonClick()
    {
        string droneIdToDestroy = droneInputField.text; 
        SelfDestructDrone(droneIdToDestroy);
    }


private DroneCommunication AddToCommunicationList(DroneCommunication head, Drone drone)
    {
        DroneCommunication newNode = new DroneCommunication(drone);
        if (head == null)
        {
            return newNode;
        }

        DroneCommunication current = head;
        while (current.Next != null)
        {
            current = current.Next;
        }
        current.Next = newNode;
        return head;
    }


    void BubbleSort(Drone[] arr, int n) 
    {
        int i, j;
        Drone temp;
        for (i = 0; i < n - 1; i++)  
        {
            for (j = 0; j < n - i - 1; j++)                               
            {
                if (arr[j].Temperature > arr[j + 1].Temperature) 
                {
                    temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }
            }
        }
    }

void Update()
{

{
        bulletCountTimer += Time.deltaTime; 

        if (bulletCountTimer >= bulletCountUpdateInterval)
        {
            bulletCountTimer = 0f; 

            foreach (Drone agent in agents)
            {
                agent.BulletCount = UnityEngine.Random.Range(1, 101); 
                UpdateDroneColor(agent); 
            }
        }

        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            
            string droneIdToDestroy = PromptUserForDroneID();
            SelfDestructDrone(droneIdToDestroy);
        }

        
        foreach (Drone agent in agents)
        {
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Transform> context = GetNearbyObjects(agent);

            stopwatch.Stop();
            

            Vector2 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;

            
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }
    
    

    bulletCountTimer += Time.deltaTime; 

    
    if (bulletCountTimer >= bulletCountUpdateInterval)
    {
        bulletCountTimer = 0f; 

        
        foreach (Drone agent in agents)
        {
            agent.BulletCount = UnityEngine.Random.Range(1, 101); 
            UpdateDroneColor(agent); 
        }
    }

    
    foreach (Drone agent in agents)
    {
        List<Transform> context = GetNearbyObjects(agent);
        Vector2 move = behavior.CalculateMove(agent, context, this);
        move *= driveFactor;

        if (move.sqrMagnitude > squareMaxSpeed)
        {
            move = move.normalized * maxSpeed;
        }
        agent.Move(move);
    }
}

private string PromptUserForDroneID()
    {
        UnityEngine.Debug.Log("Enter the Drone ID to self-destruct (e.g., Drone_0):");
        string droneId = ""; 
        return droneId; 
    }

private void SelfDestructDrone(string droneID)
    {
        Stopwatch stopwatch = new Stopwatch(); 
        stopwatch.Start(); 
        bool droneFound = false;
          foreach (Drone agent in agents)
    {
        
        if (agent.DroneID == droneID)
        {
            agent.gameObject.SetActive(false); 
            UnityEngine.Debug.Log(droneID + " has been self-destructed.");
            droneFound = true; 
            break; 
        }
    }

    stopwatch.Stop(); 

    if (droneFound)
    {
       
        UnityEngine.Debug.Log($"Time taken to self-destruct {droneID}: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
    }
    else
    {
        UnityEngine.Debug.Log("Drone with ID " + droneID + " not found.");
        
    }

       
    }


private void UpdateDroneColor(Drone agent)
{
    Renderer renderer = agent.GetComponent<Renderer>();
    if (agent.BulletCount < 50) 
    {
        renderer.material.color = Color.red;
    }
    else
    {
        renderer.material.color = Color.cyan;
    }
}


List<Transform> GetNearbyObjects(Drone agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }
   private void DestroyAllDrones()
{
    StartCoroutine(DestroyAllDronesCoroutine());
}

private IEnumerator DestroyAllDronesCoroutine()
{
    foreach (Drone agent in agents)
    {
        if (agent != null && agent.gameObject.activeSelf)
        {
            Renderer renderer = agent.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red; // Change color to red
            }
        }
    }

    yield return new WaitForSeconds(1f); // Wait for 1 second

    foreach (Drone agent in agents)
    {
        if (agent != null && agent.gameObject.activeSelf)
        {
            agent.gameObject.SetActive(false); // Deactivate the drone
        }
    }
     UnityEngine.Debug.Log("All drones has been Deactivated");

        // Update the status text
        if (statusText != null)
        {
            statusText.text = "All Drones Deactivated";
        }
    
    
}

public void SearchDroneInPartition(string droneName, int partitionNumber)
    {
        if (partitionNumber == 1 && partition1Head != null)
        {
            partition1Head.FindDrone(droneName);
        }
        else if (partitionNumber == 2 && partition2Head != null)
        {
            partition2Head.FindDrone(droneName);
        }
        else
        {
            UnityEngine.Debug.Log($"Partition {partitionNumber} does not exist or is empty.");
        }
    }

  public void DisplayAllDroneLocations()
{
    float partition1Time = 0f; 
    float partition2Time = 0f; 
    
    if (partition1Head != null)
    {
        Stopwatch partition1Stopwatch = new Stopwatch();
        partition1Stopwatch.Start(); 
        DroneCommunication current = partition1Head;
        int position = 0;

        UnityEngine.Debug.Log("Drone Locations in Partition 1:");
        while (current != null)
        {
            UnityEngine.Debug.Log($"Position {position}: {current.Drone.name}");
            current = current.Next;
            position++;
        }
        partition1Stopwatch.Stop(); 
        partition1Time = (float)partition1Stopwatch.Elapsed.TotalMilliseconds;
        UnityEngine.Debug.Log($"Time taken to display Partition 1: {partition1Time:F3} ms");
    }
    else
    {
        UnityEngine.Debug.Log("Partition 1 is empty.");
    }

    
    if (partition2Head != null)
    {
        Stopwatch partition2Stopwatch = new Stopwatch();
        partition2Stopwatch.Start(); 
        DroneCommunication current = partition2Head;
        int position = 0;

        UnityEngine.Debug.Log("Drone Locations in Partition 2:");
        while (current != null)
        {
            UnityEngine.Debug.Log($"Position {position}: {current.Drone.name}");
            current = current.Next;
            position++;
        }
        partition2Stopwatch.Stop(); 
        partition2Time = (float)partition2Stopwatch.Elapsed.TotalMilliseconds;
        UnityEngine.Debug.Log($"Time taken to display Partition 2: {partition2Time:F3} ms");
    }
    else
    {
        UnityEngine.Debug.Log("Partition 2 is empty.");
    }

    
    UnityEngine.Debug.Log($"Summary of times taken:");
    UnityEngine.Debug.Log($"Partition 1 time: {partition1Time:F3} ms");
    UnityEngine.Debug.Log($"Partition 2 time: {partition2Time:F3} ms");
}

 private void OnShowPathButtonClick()
    {
        string startDroneId = startDroneInputField.text;
        string endDroneId = endDroneInputField.text;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        List<Drone> path = FindShortestPathBFS(startDroneId, endDroneId);

        // Stop the stopwatch
        stopwatch.Stop();

        // Display the elapsed time
        UnityEngine.Debug.Log("Time taken to find the shortest path: " + stopwatch.ElapsedMilliseconds+ "ms");
       
        if (path != null && path.Count > 0)
        {
            // Log the shortest path to the console
            UnityEngine.Debug.Log("Shortest Path found:");
            foreach (Drone drone in path)
            {
                UnityEngine.Debug.Log(drone.DroneID);
            }
        }
        else
        {
            UnityEngine.Debug.Log("No path found between the drones.");
        }
    }

    // BFS Algorithm to find the shortest path between StartDrone and EndDrone
    private List<Drone> FindShortestPathBFS(string startDroneId, string endDroneId)
    {
        Drone startDrone = FindDroneById(startDroneId);
        Drone endDrone = FindDroneById(endDroneId);

        if (startDrone == null || endDrone == null)
        {
            UnityEngine.Debug.Log("One or both drones not found.");
            return null;
        }

        Queue<Drone> queue = new Queue<Drone>();
        Dictionary<Drone, Drone> previousDrone = new Dictionary<Drone, Drone>(); // Track the path

        queue.Enqueue(startDrone);
        previousDrone[startDrone] = null;

        while (queue.Count > 0)
        {
            Drone currentDrone = queue.Dequeue();

            if (currentDrone == endDrone)
            {
                // Reconstruct the path
                List<Drone> path = new List<Drone>();
                for (Drone drone = endDrone; drone != null; drone = previousDrone[drone])
                {
                    path.Add(drone);
                }
                path.Reverse(); // Reverse the path to show it from start to end
                return path;
            }

            foreach (Drone neighbor in GetNeighbors(currentDrone))
            {
                if (!previousDrone.ContainsKey(neighbor)) // Not visited yet
                {
                    queue.Enqueue(neighbor);
                    previousDrone[neighbor] = currentDrone;
                }
            }
        }

        return null; // No path found
    }



  private Drone FindDroneById(string droneId)
    {
        foreach (Drone agent in agents)
        {
            if (agent.DroneID == droneId)
            {
                return agent;
            }
        }
        return null;
    }



        private bool DFS(Drone currentDrone, Drone endDrone, HashSet<Drone> visited, List<Drone> path)
    {
        if (visited.Contains(currentDrone))
        {
            return false;
        }

        visited.Add(currentDrone);
        path.Add(currentDrone);

        if (currentDrone == endDrone)
        {
            return true;
        }

        // Explore neighbors (other drones) to continue DFS
        foreach (Drone neighbor in GetNeighbors(currentDrone))
        {
            if (DFS(neighbor, endDrone, visited, path))
            {
                return true;
            }
        }

        // If no path found, backtrack by removing the current drone from the path
        path.RemoveAt(path.Count - 1);
        return false;
    }

       private List<Drone> GetNeighbors(Drone drone)
    {
        List<Drone> neighbors = new List<Drone>();

        foreach (Drone otherDrone in agents)
        {
            if (otherDrone != drone && Vector2.Distance(drone.transform.position, otherDrone.transform.position) <= neighborRadius)
            {
                neighbors.Add(otherDrone);
            }
        }

        return neighbors;
    }

 private void DisplayPath(List<Drone> path)
{
    UnityEngine.Debug.Log("Path found:");

   

    for (int i = 0; i < path.Count; i++)
    {
        lineRenderer.SetPosition(i, path[i].transform.position);
    }

    // Log drone information
    foreach (Drone drone in path)
    {
        UnityEngine.Debug.Log($"Drone {drone.DroneID} at position {drone.transform.position}");
    }
}


    private List<Drone> GetNearbyDrones(Drone agent)
    {
        // Replace this logic with your actual neighbor connection logic
        List<Drone> nearbyDrones = new List<Drone>();
        // Add connected drones to nearbyDrones based on your structure
        return nearbyDrones;
    }

    
}