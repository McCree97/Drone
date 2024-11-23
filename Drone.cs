﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Drone : MonoBehaviour
{
    public string DroneID { get; set; }  
    public string DroneName { get; set; }  
    public float Temperature { get; set; }  

    public string Partition { get; private set; }
    
    public List<Drone> CommunicationLinks { get;  set; }
    public List<Drone> Neighbors = new List<Drone>();

    public void AddNeighbor(Drone neighbor)
    {
        if (!Neighbors.Contains(neighbor))
        {
            Neighbors.Add(neighbor);
        }
    }



    private static int droneCounter = 0;  
    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }
    public int BulletCount { get; set; }
    public int BatteryLevel { get; set; } 
    Collider2D agentCollider;
    public Collider2D AgentCollider { get { return agentCollider; } }

     public DroneCommunication CommunicationLink { get; set; }

    
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
        
        DroneID = "Drone_" + droneCounter.ToString();
        droneCounter++;  
        CommunicationLinks = new List<Drone>(); 
        BatteryLevel = 0;
        BulletCount = 0;
        
        Partition = partition;
    }
}