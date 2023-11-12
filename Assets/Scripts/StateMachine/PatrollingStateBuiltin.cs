using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingStateBuiltin : StateMachineBehaviour
{
    bool init;
    public NavMeshAgent agent;
    public Transform[] patrolPoints;

    private Transform currentPatrolPoint;

    private void Init(Animator controller)
    {
        if (init) return;

        init = true;
        agent = controller.gameObject.GetComponentInChildren<NavMeshAgent>();

        var pointsContainer = controller.transform.Find("Patrol Points");


        patrolPoints = new Transform[pointsContainer.childCount];
        for (int i = 0; i < pointsContainer.childCount; i++)
        {
            patrolPoints[i] = pointsContainer.GetChild(i);
        }
    }

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Init(animator);

        SetPatrolPoint();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var distance = Vector3.Distance(agent.transform.position, agent.destination);

        if (distance <= agent.stoppingDistance)
        {
            animator.SetTrigger("Wait");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.isStopped = true;
    }



    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    private void SetPatrolPoint()
    {
        Transform patrolPoint = null;

        for (int i = 0; i < 50; i++)
        {
            patrolPoint = patrolPoints[Random.Range(0, patrolPoints.Length)];

            if (patrolPoint != currentPatrolPoint) break;
        }


        agent.SetDestination(patrolPoint.position);
        currentPatrolPoint = patrolPoint;
        agent.isStopped = false;
    }



}
