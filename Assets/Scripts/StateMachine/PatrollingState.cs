using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingState : LogicMachineBehaviour<GuardLogicManager>
{
    public Vector2 waitTime = new Vector2(1, 3);
    int currentPatrolIndex;
    bool waiting;

    public override void OnAwake()
    {
        currentPatrolIndex = -1;
    }


    public override void OnEnter()
    {
        if (currentPatrolIndex == -1)
            SetPatrolPoint();
        else
            manager.agent.SetDestination(manager.patrolPoints[currentPatrolIndex].position);

        manager.agent.isStopped = false;
    }

    public override void OnUpdate()
    {

        if (manager.triggerChecker.hasObject)
        {
            logicAnimator.SetBool(manager.Param_Suspicious, true);
        }

        var distance = Vector3.Distance(manager.agent.transform.position, manager.agent.destination);
        if (!waiting && distance <= manager.agent.stoppingDistance)
        {

            WaitToSetNewPoint();
        }
    }

    private async void WaitToSetNewPoint()
    {
        waiting = true;
        var time = UnityEngine.Random.Range(waitTime.x, waitTime.y);

        await Task.Delay(TimeSpan.FromSeconds(time));

        if (!active) return;

        SetPatrolPoint();
        waiting = false;
    }


    public override void OnExit()
    {
        manager.agent.isStopped = true;
    }

    private void SetPatrolPoint()
    {
        Transform patrolPoint = null;

        //Next 
        currentPatrolIndex = currentPatrolIndex + 1 >= manager.patrolPoints.Length ? 0 : currentPatrolIndex + 1;
        patrolPoint = manager.patrolPoints[currentPatrolIndex];

        //Random
        //for (int i = 0; i < 50; i++)
        //{
        //    patrolPoint = manager.patrolPoints[Random.Range(0, manager.patrolPoints.Length)];

        //    if (patrolPoint != currentPatrolPoint) break;
        //}

        manager.agent.SetDestination(patrolPoint.position);

    }


}
