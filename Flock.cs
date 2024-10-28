using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using TMPro;


public class Flock : MonoBehaviour
{
    public TMP_InputField droneInputField; // Reference to the Input Field
    public Button selfDestructButton;
    public Button showLocationsButton;
    public Drone agentPrefab;
    List<Drone> agents = new List<Drone>();
    public FlockBehavior behavior;

    private DroneCommunication partition1Head;
    private DroneCommunication partition2Head;

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

    // Start is called before the first frame update
    private float bulletCountUpdateInterval = 5f; // Time interval in seconds
private float bulletCountTimer = 0f; // Timer to track elapsed time
void Start()
{
    // Randomize BulletCount for each agent at the beginning
    foreach (Drone agent in agents)
    {
        agent.BulletCount = UnityEngine.Random.Range(1, 101); // Randomize BulletCount from the start
        UpdateDroneColor(agent); // Update color based on the initial BulletCount
    }

    if (agentPrefab == null)
    {
        return; // Exit the Start method if agentPrefab is null
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
        newAgent.DroneID = "Drone_" + i; // Set the unique ID
        newAgent.Initialize(this);
        newAgent.BulletCount = UnityEngine.Random.Range(0, 100); // Assign a random bullet count to each drone
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
    showLocationsButton.onClick.AddListener(DisplayAllDroneLocations); // Only set up listener here
}

private void OnSelfDestructButtonClick()
    {
        string droneIdToDestroy = droneInputField.text; // Get text from the input field
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


    void BubbleSort(Drone[] arr, int n) // O(N^2)
    {
        int i, j;
        Drone temp;
        //bool swapped;               // let n =10
        for (i = 0; i < n - 1; i++)  // i=0..9
        {
           // swapped = false;
            for (j = 0; j < n - i - 1; j++)   // i=0: j=0..9
                                              // i=1; j=0..8
                                              // i=2; j=0..7
                                              // i
            {
                if (arr[j].Temperature > arr[j + 1].Temperature) // check whether to swap
                {

                    // Swap arr[j] and arr[j+1]
                    temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                    //swapped = true;
                }
            }

            // If no two elements were
            // swapped by inner loop, then break
            //if (swapped == false)
            //    break;
        }
    }

    // Update is called once per frame

void Update()
{

{
        bulletCountTimer += Time.deltaTime; // Increment the timer by the time since last frame

        // Check if 5 seconds have passed
        if (bulletCountTimer >= bulletCountUpdateInterval)
        {
            bulletCountTimer = 0f; // Reset the timer

            // Randomize BulletCount for each agent every 5 seconds
            foreach (Drone agent in agents)
            {
                agent.BulletCount = UnityEngine.Random.Range(1, 101); // Re-randomize the BulletCount
                UpdateDroneColor(agent); // Update color based on the new BulletCount
            }
        }

        if (Input.GetKeyDown(KeyCode.Return)) // Press Enter to trigger the self-destruct
        {
            // Prompt user for drone ID
            string droneIdToDestroy = PromptUserForDroneID();
            SelfDestructDrone(droneIdToDestroy);
        }

        // Movement behavior (unchanged)
        foreach (Drone agent in agents)
        {
            // Measure partitioning time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Transform> context = GetNearbyObjects(agent);

            stopwatch.Stop();
            //UnityEngine.Debug.Log($"Partitioning time for {agent.name}: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");

            Vector2 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;

            // Limit the speed of the drone
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }
    
    

    bulletCountTimer += Time.deltaTime; // Increment the timer by the time since last frame

    // Check if 5 seconds have passed
    if (bulletCountTimer >= bulletCountUpdateInterval)
    {
        bulletCountTimer = 0f; // Reset the timer

        // Randomize BulletCount for each agent every 5 seconds
        foreach (Drone agent in agents)
        {
            agent.BulletCount = UnityEngine.Random.Range(1, 101); // Re-randomize the BulletCount
            UpdateDroneColor(agent); // Update color based on the new BulletCount
        }
    }

    // Movement behavior (unchanged)
    foreach (Drone agent in agents)
    {
        List<Transform> context = GetNearbyObjects(agent);
        Vector2 move = behavior.CalculateMove(agent, context, this);
        move *= driveFactor;

        // Limit the speed of the drone
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
        string droneId = ""; // Capture user input (in a real implementation, you'd need to manage this input)
        return droneId; // Replace with actual input handling
    }

    // Self-destruct logic
    private void SelfDestructDrone(string droneID)
    {
        Stopwatch stopwatch = new Stopwatch(); // Create a stopwatch to measure time
        stopwatch.Start(); 
        bool droneFound = false;
          foreach (Drone agent in agents)
    {
        if (agent.DroneID == droneID)
        {
            agent.gameObject.SetActive(false); // Deactivate the drone
            UnityEngine.Debug.Log(droneID + " has been self-destructed.");
            droneFound = true; // Set the flag to true if drone is found
            break; // Exit the loop once the drone is found and deactivated
        }
    }

    stopwatch.Stop(); // Stop timing

    if (droneFound)
    {
        // Display time taken for self-destruction
        UnityEngine.Debug.Log($"Time taken to self-destruct {droneID}: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
    }
    else
    {
        UnityEngine.Debug.Log("Drone with ID " + droneID + " not found.");
        
    }

       
    }

// Separate method to update the color based on BulletCount
private void UpdateDroneColor(Drone agent)
{
    Renderer renderer = agent.GetComponent<Renderer>();
    if (agent.BulletCount < 50) // Assuming 50 is the threshold
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
    float partition1Time = 0f; // To store elapsed time for Partition 1
    float partition2Time = 0f; // To store elapsed time for Partition 2
    
    // Display locations for Partition 1
    if (partition1Head != null)
    {
        Stopwatch partition1Stopwatch = new Stopwatch();
        partition1Stopwatch.Start(); // Start timing for Partition 1
        DroneCommunication current = partition1Head;
        int position = 0;

        UnityEngine.Debug.Log("Drone Locations in Partition 1:");
        while (current != null)
        {
            UnityEngine.Debug.Log($"Position {position}: {current.Drone.name}");
            current = current.Next;
            position++;
        }
        partition1Stopwatch.Stop(); // Stop timing for Partition 1
        partition1Time = (float)partition1Stopwatch.Elapsed.TotalMilliseconds;
        UnityEngine.Debug.Log($"Time taken to display Partition 1: {partition1Time:F3} ms");
    }
    else
    {
        UnityEngine.Debug.Log("Partition 1 is empty.");
    }

    // Display locations for Partition 2
    if (partition2Head != null)
    {
        Stopwatch partition2Stopwatch = new Stopwatch();
        partition2Stopwatch.Start(); // Start timing for Partition 2
        DroneCommunication current = partition2Head;
        int position = 0;

        UnityEngine.Debug.Log("Drone Locations in Partition 2:");
        while (current != null)
        {
            UnityEngine.Debug.Log($"Position {position}: {current.Drone.name}");
            current = current.Next;
            position++;
        }
        partition2Stopwatch.Stop(); // Stop timing for Partition 2
        partition2Time = (float)partition2Stopwatch.Elapsed.TotalMilliseconds;
        UnityEngine.Debug.Log($"Time taken to display Partition 2: {partition2Time:F3} ms");
    }
    else
    {
        UnityEngine.Debug.Log("Partition 2 is empty.");
    }

    // Display the separate timing results for each partition at the end
    UnityEngine.Debug.Log($"Summary of times taken:");
    UnityEngine.Debug.Log($"Partition 1 time: {partition1Time:F3} ms");
    UnityEngine.Debug.Log($"Partition 2 time: {partition2Time:F3} ms");
}


}