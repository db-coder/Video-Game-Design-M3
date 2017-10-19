using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(AINavSteeringController))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIDemoController : MonoBehaviour
{
    public Transform[] waypointSetA;
    public Transform waypointThrow;

    public enum State
    {
        A,
        B,
        D,
        E
    }

    public State state = State.A;
    public Text state_AI;

    public float waitTime = 3f;
    public float futureTLimit = 5f;
    public GameObject mover;
    public Rigidbody rbody;
    public GameObject ballPrefab;

    private Animator anim;  
    private Rigidbody rbody_player;
    private Transform leftHand;
    private Rigidbody rbody_ball;
    private GameObject heldBall = null;
    private Vector3 moverVelocity;
    private int i;

    protected float beginWaitTime;

    AINavSteeringController aiSteer;
    NavMeshAgent agent;


    // Use this for initialization
    void Start()
    {
        aiSteer = GetComponent<AINavSteeringController>();
        agent = GetComponent<NavMeshAgent>();
        moverVelocity = rbody.velocity;

        anim = GetComponent<Animator>();
        rbody_player = GetComponent<Rigidbody>();
        leftHand = this.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand");

        Debug.Log("NavMesh:avoidancePredictionTime(default): " + NavMesh.avoidancePredictionTime);

        //NavMesh.avoidancePredictionTime = 4f;

        aiSteer.Init();
        aiSteer.waypointLoop = false;
        aiSteer.stopAtNextWaypoint = false;

        int s = Random.Range(0,3);
        switch(s)
        {
            case 0: transitionToStateA();
                    break;
            case 1: transitionToStateB();
                    break;
            case 2: transitionToStateE();
                    break;
        }
    }

    void transitionToStateA()
    {
        state_AI.text = "Current State: Path Planning";
        state = State.A;
        i = Random.Range(0,waypointSetA.Length);
        aiSteer.setWaypoint(waypointSetA[i]);
        aiSteer.useNavMeshPathPlanning = true;
        beginWaitTime = Time.timeSinceLevelLoad;
    }

    void transitionToStateB()
    {
        state_AI.text = "Current State: Reach Vantage Point";
        state = State.B;
        aiSteer.clearWaypoints();
        aiSteer.setWaypoint(waypointThrow);
        aiSteer.useNavMeshPathPlanning = true;

        if (aiSteer.waypointsComplete())
            transitionToStateD();
    }

    void transitionToStateD()
    {
        state_AI.text = "Current State: Throw";
        state = State.D;

        aiSteer.setFacingPoint(mover.transform.position);

        anim.SetTrigger("Throw");
        if (ballPrefab != null)
        {
          heldBall = Instantiate(ballPrefab, leftHand.position + new Vector3(0.05f, -0.1f, 0), Quaternion.identity);
          rbody_ball = heldBall.GetComponent<Rigidbody>();
          heldBall.transform.parent = leftHand;
          rbody_ball.isKinematic = true;
        }
        beginWaitTime = Time.timeSinceLevelLoad;
    }

    void transitionToStateE()
    {
        state_AI.text = "Current State: Chase GameObject";
        state = State.E;
        //dist between moving object and NPC 
        float dist = (mover.transform.position - transform.position).magnitude;
        float futureT = 0.1f * dist;
         
        futureT = Mathf.Min(futureT, futureTLimit); //limit on how far ahead to look

        //extrapolate assuming constant Vel and the futureT intercept estimate
        Vector3 futureMoverPos = mover.transform.position + moverVelocity * futureT;

        //update the target waypoint
        aiSteer.setWaypoint(futureMoverPos);
        beginWaitTime = Time.timeSinceLevelLoad;
    }

    void ExecuteThrowCallback()
    {
        heldBall.transform.parent = null;
        rbody_ball.isKinematic = false;

        //predictive code
        float ballSpeed = 15.0f;
        float sphereSpeed = moverVelocity.magnitude;
        Vector3 sphereDir = moverVelocity;
        sphereDir.Normalize();

        Vector3 sphereToBall = heldBall.transform.position - mover.transform.position;
        float sphereToBallDist = sphereToBall.magnitude;
        Vector3 sphereToBallDir = sphereToBall;
        sphereToBallDir.Normalize();

        float angle = Vector3.Dot(sphereToBallDir,sphereDir);

        float a = ballSpeed*ballSpeed - sphereSpeed*sphereSpeed;
        float b = 2*sphereToBallDist*sphereSpeed*angle;
        float c = -sphereToBallDist*sphereToBallDist;
        float disc = b*b - 4*a*c;
        float t0 = (-b + Mathf.Sqrt(disc))/(2*a);
        float t1 = (-b - Mathf.Sqrt(disc))/(2*a);

        float t = Mathf.Min(t0,t1);
        if(t0*t1 < 0)
            t = Mathf.Max(t0,t1);
        Vector3 launchV = moverVelocity - sphereToBall/t;
        // Vector3 launchV = force*(mover.transform.position - transform.position);
        EventManager.TriggerEvent<ThrowEvent, Vector3>(heldBall.transform.position);
        rbody_ball.AddForce(launchV, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            transitionToStateB();
        }
        else if(state == State.B)
        {
            if(aiSteer.waypointsComplete())
                transitionToStateD();
        }
        else if(state == State.E)
        {
            if(Mathf.Abs(Vector3.Distance(transform.position,mover.transform.position)) > 1.0)
                transitionToStateE();
            else
                transitionToStateA();
        }
		else if(Input.GetKeyDown(KeyCode.Space))
        {
            transitionToStateE();
        }
        else if (state != State.B && Time.timeSinceLevelLoad - beginWaitTime > waitTime)
        {
            if (aiSteer.waypointsComplete())
            {
                if(state == State.A)
                    EventManager.TriggerEvent<PlayerLandsEvent, Vector3>(waypointSetA[i].transform.position);
                int s = Random.Range(0,3);
                switch(s)
                {
                    case 0: transitionToStateA();
                            break;
                    case 1: transitionToStateB();
                            break;
                    case 2: transitionToStateE();
                            break;
                }
            }
        }
    }
}