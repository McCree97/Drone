using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics; // For Stopwatch
public class DroneNetworkCommunication
{
    public Drone Drone { get; set; }
    public DroneCommunication FirstDrone { get; private set; }
    public List<DroneNetworkCommunication> GetConnectedDrones { get; private set; }
    public DroneNetworkCommunication Next { get; set; }

    public DroneNetworkCommunication()
    {
        FirstDrone = null;
        GetConnectedDrones = new List<DroneNetworkCommunication>();  
    }

    public DroneNetworkCommunication(Drone drone)
    {
        Drone = drone;
        GetConnectedDrones = new List<DroneNetworkCommunication>();  
    }

    public void AddDrone(Drone newDrone)
    {
        if (FirstDrone == null)
        {
            FirstDrone = new DroneCommunication(newDrone);
        }
        else
        {
            DroneCommunication current = FirstDrone;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = new DroneCommunication(newDrone);
        }
    }

    // Establishes communication links between drones
    public void EstablishLinks()
    {
        DroneCommunication current = FirstDrone;
        while (current != null)
        {
            DroneCommunication other = FirstDrone;
            while (other != null)
            {
                if (current != other)
                {
                    current.Drone.CommunicationLinks.Add(other.Drone);
                }
                other = other.Next;
            }
            current = current.Next;
        }
    }

    // Depth-First Search (DFS) to search for a drone by its ID
    public string DFS(string targetDroneID)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();  // Start the stopwatch
        if (FirstDrone == null)
        {
              stopwatch.Stop();
            return $"Network is empty, no drones to search.";
        }

        HashSet<string> visited = new HashSet<string>();
        Stack<Drone> stack = new Stack<Drone>();
        stack.Push(FirstDrone.Drone);
        int position = 0;

        while (stack.Count > 0)
        {
            Drone currentDrone = stack.Pop();

            if (!visited.Contains(currentDrone.DroneID))
            {
                visited.Add(currentDrone.DroneID);
                if (currentDrone.DroneID == targetDroneID)
                {
                     stopwatch.Stop();
                    return $"Drone {targetDroneID} found at position {position} in the DFS traversal. Time taken: {stopwatch.Elapsed.TotalMilliseconds.ToString("F4")}ms";
                }

                foreach (var neighbor in currentDrone.CommunicationLinks)
                {
                    stack.Push(neighbor);
                }
            }

            position++;  // Increment position after processing a drone
        }

        return $"Drone {targetDroneID} not found in the network.";
    }

    // Find the shortest path between two drones using BFS
    public string FindShortestPath(string startDroneID, string endDroneID)
    {
        if (FirstDrone == null)
        {
            return $"Network is empty, no drones to search.";
        }

        Dictionary<string, string> parent = new Dictionary<string, string>();
        Queue<Drone> queue = new Queue<Drone>();
        HashSet<string> visited = new HashSet<string>();

        Drone startDrone = FindDroneByID(startDroneID);
        if (startDrone == null)
        {
            return $"Drone {startDroneID} not found in the network.";
        }

        queue.Enqueue(startDrone);
        visited.Add(startDrone.DroneID);
        parent[startDrone.DroneID] = null;

        while (queue.Count > 0)
        {
            Drone current = queue.Dequeue();

            if (current.DroneID == endDroneID)
            {
                return ConstructPath(endDroneID, parent);
            }

            foreach (var neighbor in current.CommunicationLinks)
            {
                if (!visited.Contains(neighbor.DroneID))
                {
                    visited.Add(neighbor.DroneID);
                    queue.Enqueue(neighbor);
                    parent[neighbor.DroneID] = current.DroneID;
                }
            }
        }

        return $"Path from {startDroneID} to {endDroneID} not found.";
    }

    // Construct the path from the start drone to the end drone
    private string ConstructPath(string endDroneID, Dictionary<string, string> parent)
    {
        List<string> path = new List<string>();
        string current = endDroneID;

        while (current != null)
        {
            path.Add(current);
            current = parent[current];
        }

        path.Reverse();
        return $"Shortest path: {string.Join(" -> ", path)}";
    }

    // Find a drone by its ID in the network
    private Drone FindDroneByID(string droneID)
    {
        DroneCommunication current = FirstDrone;
        while (current != null)
        {
            if (current.Drone.DroneID == droneID)
            {
                return current.Drone;
            }
            current = current.Next;
        }
        return null;
    }

    // Get a drone by its ID (used for more complex queries like DFS)
    public DroneNetworkCommunication GetDroneByID(string droneID)
    {
        UnityEngine.Debug.Log($"Searching for DroneID: {droneID}");  // Log the search ID

        if (Drone != null && Drone.DroneID == droneID)
        {
            UnityEngine.Debug.Log($"Drone found: {Drone.DroneID}");
            return this;  // Return the current drone if IDs match
        }

        // Log the connected drones
        if (GetConnectedDrones != null && GetConnectedDrones.Count > 0)
        {
            foreach (var connectedDrone in GetConnectedDrones)
            {
                var result = connectedDrone.GetDroneByID(droneID);
                if (result != null)
                {
                    return result;  // Return the found drone
                }
            }
        }

        UnityEngine.Debug.Log("Drone not found.");
        return null;  // Return null if drone is not found
    }

    // Add a connected drone to the network
    public void AddConnection(DroneNetworkCommunication drone)
{
    if (drone != null && !GetConnectedDrones.Contains(drone))
    {
        GetConnectedDrones.Add(drone);
    }
}

}