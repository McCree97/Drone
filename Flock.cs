﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using TMPro;


public class Flock : MonoBehaviour
{
    public TMP_InputField droneInputField; 
    public Button selfDestructButton;
    public Button showLocationsButton;
    public Drone agentPrefab;
    List<Drone> agents = new List<Drone>();
    public FlockBehavior behavior;

    private DroneCommunication partition1Head;
    private DroneCommunication partition2Head;
    private Queue<string> logMessages = new Queue<string>(); // Queue to store log messages
    

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

    selfDestructButton.onClick.AddListener(OnSelfDestructButtonClick);
    showLocationsButton.onClick.AddListener(DisplayAllDroneLocations); 
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


}