using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCommunication
{
    public Drone Drone { get; set; }
    public DroneCommunication Next { get; set; }

    public DroneCommunication(Drone drone)
    {
        Drone = drone;
        Next = null;
    }

     public void FindDrone(string droneName)
    {
        DroneCommunication current = this;
        int position = 0;

        while (current != null)
        {
            if (current.Drone.name == droneName)
            {
                UnityEngine.Debug.Log($"Drone {droneName} found at position {position} in the linked list.");
                return;
            }
            current = current.Next;
            position++;
        }

        UnityEngine.Debug.Log($"Drone {droneName} not found in the linked list.");
    }
}

