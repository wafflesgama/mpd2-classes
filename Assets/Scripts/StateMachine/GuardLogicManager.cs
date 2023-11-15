using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardLogicManager : LogicMachineManager
{
    [Header("References")]
    public Transform playerController;

    public NavMeshAgent agent;
    public CharacterAnimController animationController;
    public PlayerTriggerChecker triggerChecker;
    public CharacterVFXController vFXController;

    [Header("Logic Param Names")]
    public string Param_Patrol = "Patrol";
    public string Param_Suspicious = "Suspicious";
    public string Param_Detected = "Detected";

    [Header("Patrol")]
    public Transform[] patrolPoints;


    [Header("Suspicion")]
    public SpriteRenderer detectionMeter;
    public SpriteRenderer detectedMeter;

    public override void OnAwake()
    {
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
