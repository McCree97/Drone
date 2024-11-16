﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Drone : MonoBehaviour
{
    public string DroneID { get; set; }  // Unique ID of the drone (used as the key)
    public string DroneName { get; set; }  // Name of the drone
    public float Temperature { get; set; }  // Example of a drone attribute

    public string Partition { get; private set; }

    private static int droneCounter = 0;  // Static counter to auto-assign unique drone IDs
    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }
    public int BulletCount { get; set; }
    public int BatteryLevel { get; set; } // New attribute
    Collider2D agentCollider;
    public Collider2D AgentCollider { get { return agentCollider; } }

     public DroneCommunication CommunicationLink { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        Temperature = (int) (Random.value * 100);
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
        CommunicationLink = null;
        BatteryLevel = UnityEngine.Random.Range(0, 100);
    }

    public void Move(Vector2 velocity)
    {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    public Drone(string partition)
    {
        // Automatically generate a unique droneID
        DroneID = "Drone_" + droneCounter.ToString();
        droneCounter++;  // Increment for the next drone

        Partition = partition;
    }
}