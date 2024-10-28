using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Drone : MonoBehaviour
{
    public string DroneID;
    public int Temperature { set; get; } = 0;
    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }
    public int BulletCount;
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
    }

    public void Move(Vector2 velocity)
    {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}