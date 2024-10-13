
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


public class Flock : MonoBehaviour
{
    public Drone agentPrefab;
    List<Drone> agents = new List<Drone>();
    public FlockBehavior behavior;

    [Range(10, 5000)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
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

     // Randomize BulletCount for each agent at the beginning
    foreach (Drone agent in agents)
    
     if (agentPrefab == null)
    {
        UnityEngine.Debug.LogError("Agent Prefab is not assigned in the Inspector. Please assign a Drone prefab.");
        return; // Exit the Start method if agentPrefab is null
    }
    else
    {
        UnityEngine.Debug.Log("Agent Prefab is assigned: " + agentPrefab.name); // Log the name of the prefab
    }
    // Check if agentPrefab is assigned
    if (agentPrefab == null)
    {
        UnityEngine.Debug.LogError("Agent Prefab is not assigned in the Inspector. Please assign a Drone prefab.");
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
        newAgent.Initialize(this);
        newAgent.BulletCount = UnityEngine.Random.Range(1, 101); // Assign a random bullet count to each drone
        agents.Add(newAgent);
    }
}


    void BubbleSort(Drone[] arr, int n) // O(N^2)
    {
        int i, j;
        Drone temp;
        bool swapped;               // let n =10
        for (i = 0; i < n - 1; i++)  // i=0..9
        {
            swapped = false;
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
                    swapped = true;
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

        // Movement behavior (unchanged)
        foreach (Drone agent in agents)
        {
            // Measure partitioning time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Transform> context = GetNearbyObjects(agent);

            stopwatch.Stop();
            UnityEngine.Debug.Log($"Partitioning time for {agent.name}: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");

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
}