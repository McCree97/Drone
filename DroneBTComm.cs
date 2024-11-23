using System.Collections.Generic;  
using UnityEngine;

public class DroneBTComm 
{
   public DroneBTNode root { get; private set; }
     public DroneBTComm()
    {
        root = null;
    }
    
    public void InsertDrone(Drone drone)
    {
        if (root == null)
        {
            root = new DroneBTNode(drone);  
        }
        else
        {
            InsertDroneRecursively(root, drone);
        }
    }

    private void InsertDroneRecursively(DroneBTNode node, Drone drone)
    {
        if (drone.DroneID.CompareTo(node.Drone.DroneID) < 0)  
        {
            if (node.Left == null)
            {
                node.Left = new DroneBTNode(drone);  
            }
            else
            {
                InsertDroneRecursively(node.Left, drone);  
            }
        }
        else  
        {
            if (node.Right == null)
            {
                node.Right = new DroneBTNode(drone);  
            }
            else
            {
                InsertDroneRecursively(node.Right, drone);  
            }
        }
    }

    
    private void InsertNode(DroneBTNode node, Drone drone)
    {
        if (drone.DroneID.CompareTo(node.Drone.DroneID) < 0)
        {
            if (node.Left == null)
                node.Left = new DroneBTNode(drone);
            else
                InsertNode(node.Left, drone);
        }
        else
        {
            if (node.Right == null)
                node.Right = new DroneBTNode(drone);
            else
                InsertNode(node.Right, drone);
        }
    }

    
    public List<DroneBTCommunication> GetAllDrones()
    {
        List<DroneBTCommunication> drones = new List<DroneBTCommunication>();
        TraverseInOrder(root, drones);
        return drones;
    }

    
    private void TraverseInOrder(DroneBTNode node, List<DroneBTCommunication> drones)
    {
        if (node != null)
        {
            TraverseInOrder(node.Left, drones);  
            drones.Add(new DroneBTCommunication(node.Drone));  
            TraverseInOrder(node.Right, drones);  
        }
    }
       public DroneBTCommunication FindDrone(string droneID, out string path)
    {
        path = "";  
        return FindDroneRecursively(root, droneID, ref path);
    }

    
       private DroneBTCommunication FindDroneRecursively(DroneBTNode node, string droneID, ref string path)
    {
        if (node == null)
        {
            return null; 
        }

        int compare = droneID.CompareTo(node.Drone.DroneID);

        if (compare < 0) 
        {
             path += $" -> {node.Drone.DroneID} (Left)";
            return FindDroneRecursively(node.Left, droneID, ref path);
        }
        else if (compare > 0)  
        {
            path += $" -> {node.Drone.DroneID} (Right)";
            return FindDroneRecursively(node.Right, droneID, ref path);
        }
        else  
        {
            path += $" -> {node.Drone.DroneID} (Found)";  
            return new DroneBTCommunication(node.Drone);  
        }
    }
}

