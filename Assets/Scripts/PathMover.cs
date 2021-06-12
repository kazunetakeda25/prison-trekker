using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathMover : MonoBehaviour
{

    private NavMeshAgent NavMeshAgent;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    // Start is called before the first frame update

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        FindObjectOfType<PathCreator>().OnNewPathCreated += SetPoints;
    }

    private void SetPoints(IEnumerable<Vector3> points) {

        pathPoints = new Queue<Vector3>(points);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePathing();
    }

    private void UpdatePathing()
    {
        if (ShouldSetDestination())
            NavMeshAgent.SetDestination(pathPoints.Dequeue());

    }

    private bool ShouldSetDestination() {

        if (pathPoints.Count == 0)
            return false;

        if (NavMeshAgent.hasPath == false || NavMeshAgent.remainingDistance < 0.5f)
            return true;

        return false;
    }
}
