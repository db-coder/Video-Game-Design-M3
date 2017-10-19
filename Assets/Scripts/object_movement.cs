using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class object_movement : MonoBehaviour 
{
	// Use this for initialization
	private NavMeshAgent agent;
	public Transform[] waypoints;
	public GameObject explosion;
	public float waitTime = 1f;

	private Vector3 pos;
    protected float beginWaitTime = 0;
	void Start ()
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		pos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Time.timeSinceLevelLoad - beginWaitTime > waitTime)
		{
			agent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].position);
			beginWaitTime = Time.timeSinceLevelLoad;
		}
	}

	void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.gameObject.tag == "Player" || collision.transform.gameObject.tag == "grabbable")
        {
            if (collision.impulse.magnitude > 0f)
            {
                EventManager.TriggerEvent<ExplosionEvent, Vector3>(collision.contacts[0].point);
                Instantiate(explosion,collision.transform.position,Quaternion.identity);
                this.gameObject.transform.position = pos;
            }
        }		
    }
}
