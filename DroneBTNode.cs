using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBTNode
{
    public Drone Drone { get; private set; }
    public DroneBTNode Left { get; set; }
    public DroneBTNode Right { get; set; }

    public DroneBTNode(Drone drone)
    {
        Drone = drone;
        Left = null;
        Right = null;
    }
}
