public class DroneBTCommunication
{
    public Drone Drone { get; set; }  
    public DroneBTCommunication Left { get; set; }  
    public DroneBTCommunication Right { get; set; } 
    public DroneBTCommunication Next { get; set; }

    public DroneBTCommunication(Drone drone)
    {
        Drone = drone;
        Left = null;
        Right = null;
        Next = null;
    }

    
    public void Insert(Drone newDrone)
    {
        if (newDrone.DroneID.CompareTo(Drone.DroneID) < 0)
        {
            
            if (Left == null)
                Left = new DroneBTCommunication(newDrone);
            else
                Left.Insert(newDrone);
        }
        else
        {
            
            if (Right == null)
                Right = new DroneBTCommunication(newDrone);
            else
                Right.Insert(newDrone);
        }
    }

    
 private DroneBTCommunication FindNode(DroneBTNode node, string droneID, ref string path)
{
    if (node == null)
    {
        return null;  
    }

    int compare = droneID.CompareTo(node.Drone.DroneID);

    if (compare < 0)  
    {
        path += " -> Left";  
        return FindNode(node.Left, droneID, ref path);  
    }
    else if (compare > 0)  
    {
        path += " -> Right";  
        return FindNode(node.Right, droneID, ref path);  
    }
    else  
    {
        path += " -> Found";  
        return new DroneBTCommunication(node.Drone);  
    }
}



    
    public void InOrderTraversal()
    {
        if (Left != null)
            Left.InOrderTraversal();  

        
        UnityEngine.Debug.Log($"Drone ID: {Drone.DroneID}, Name: {Drone.DroneName}, Temperature: {Drone.Temperature}");

        if (Right != null)
            Right.InOrderTraversal();  
    }
}