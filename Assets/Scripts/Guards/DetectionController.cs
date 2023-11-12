using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum DetectionStatus
{
    NOTDETECTED,
    SUSPICIOUS,
    DETECTED,
}

public class DetectionController : MonoBehaviour
{
    [Header("References")]
    public GuardLogicManager logicManager;


    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    //public EnemyMovController m_Controller;

    public PlayerTriggerChecker triggerChecker;

    [Header("Visualization")]
    public SpriteRenderer detectionMeter;
    public SpriteRenderer detectedMeter;
    public float lerpSpeed = 20;
    public float showMeterSpeed;
    public Ease showMeterEase;
    public Ease hideMeterEase;
    public float showdetMeterSpeed;
    public float detectedShakeDuration = 0.9f;
    public float detectedShakeIntensity = 1f;
    private float initMeterScale;
    private float initdetMeterScale;
    private Tween detectionTween;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();


    public DetectionStatus m_Status = DetectionStatus.NOTDETECTED;
    public float DetectionDecayValue = -10f;
    public float DetectionIncreaseValue = 20f;
    public float DetectionMaxValue = 100f;
    private float detectionValue = 0f;

    private float detectedPercent;
    private int shader_ValueParam;

    UEventHandler eventHandler = new UEventHandler();


    private void Awake()
    {
        shader_ValueParam = Shader.PropertyToID("_Amount");

        initMeterScale = detectionMeter.transform.localScale.x;
        detectionMeter.transform.localScale = Vector3.zero;

        initdetMeterScale = detectedMeter.transform.localScale.x;
        detectedMeter.transform.localScale = Vector3.zero;
    }
    private void Start()
    {

        StartCoroutine("FindTargets", 0.2f);
        triggerChecker.OnTriggered.Subscribe(eventHandler, CatchedPlayer);

    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }

    private void Update()
    {
        var oldVal = detectionMeter.material.GetFloat(shader_ValueParam);
        var lerpVal = Mathf.Lerp(oldVal, detectedPercent, Time.deltaTime * lerpSpeed);
        detectionMeter.material.SetFloat(shader_ValueParam, lerpVal);
        detectedMeter.material.SetFloat(shader_ValueParam, lerpVal);
    }
    private IEnumerator FindTargets(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            //FindVisibleTargets();
            UpdateDetectionStatus();
        }
    }

    //private void FindVisibleTargets()
    //{
    //    visibleTargets.Clear();
    //    Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

    //    for (int i = 0; i < targetsInViewRadius.Length; i++)
    //    {
    //        Transform target = targetsInViewRadius[i].transform;
    //        Vector3 dirToTarget = (target.position - transform.position).normalized;
    //        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
    //        {
    //            float dstToTarget = Vector3.Distance(transform.position, target.position);


    //            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
    //            {
    //                visibleTargets.Add(target);
    //            }
    //        }
    //    }
    //}

    void UpdateDetectionStatus()
    {

        if (m_Status == DetectionStatus.NOTDETECTED || m_Status == DetectionStatus.SUSPICIOUS)
        {
            if (triggerChecker.hasObject)
            {
                detectionValue = Mathf.Clamp(detectionValue + DetectionIncreaseValue, 0f, 100f);


                if (detectionValue == 100)
                {
                    TrigerDetection();
                }
            }
            else
            {
                detectionValue = Mathf.Clamp(detectionValue + DetectionDecayValue, 0f, 100f);
            }

            if (m_Status == DetectionStatus.NOTDETECTED)
            {
                //If Started to get Suspicious
                if (detectedPercent > 0)
                {
                    TriggerSuspicion();
                }
            }
            else
            {
                //If lost suspicion
                if (detectedPercent <= 0)
                {
                    TriggerNotDetected();
                }
            }
        }
        else
        {
            if (visibleTargets.Count == 0)
            {
                detectionValue = Mathf.Clamp(detectionValue + DetectionDecayValue, 0f, 100f);

                if (detectionValue == 0)
                {
                    //SoundController.current.StopAlertSound();
                    TriggerNotDetected();
                }
            }
        }

        detectedPercent = detectionValue / 100;

    }

    private void TrigerDetection()
    {
        m_Status = DetectionStatus.DETECTED;
        //SoundController.current.PlayAlertSound();

        detectionMeter.enabled = false;
        detectedMeter.enabled = true;


        detectedMeter.transform.DOScale(initdetMeterScale, showdetMeterSpeed).SetEase(showMeterEase);
        detectedMeter.transform.DOShakePosition(detectedShakeDuration, detectedShakeIntensity);


        //bool p1 = false, p2 = false;
        //if (visibleTargets.Count >= 2)
        //{
        //    p1 = true;
        //    p2 = true;
        //}
        //else if (visibleTargets.Count > 0)
        //{
        //    p1 = visibleTargets[0].parent.name == PlayerManager.current.player1.name;
        //    p2 = visibleTargets[0].parent.name == PlayerManager.current.player2.name;
        //}
        //m_Controller.SetEnemyDetected(true, p1, p2);
    }

    private void TriggerSuspicion()
    {
        m_Status = DetectionStatus.SUSPICIOUS;

        detectionMeter.enabled = true;
        detectedMeter.enabled = false;

        //m_Controller.SetEnemySuspicious(true);
        //SoundController.current.PlayOneShotEvent("event:/Core Gameplay/Guard Suspect");
        detectedMeter.transform.localScale = Vector3.zero;

        if (detectedMeter != null)
        {
            detectionTween.Kill();
            detectionTween = null;

        }
        detectionTween = detectionMeter.transform.DOScale(initMeterScale, showMeterSpeed).SetEase(showMeterEase);
    }

    private void TriggerNotDetected()
    {
        m_Status = DetectionStatus.NOTDETECTED;
        //m_Controller.SetEnemySuspicious(false);
        //SoundController.current.PlayOneShotEvent("event:/Core Gameplay/Guard Relief");

        if (detectedMeter != null)
        {
            detectionTween.Kill();
            detectionTween = null;
        }

        detectionTween = detectionMeter.transform.DOScale(0, showMeterSpeed).SetEase(hideMeterEase);
    }

    private void CatchedPlayer(Transform playerController)
    {

        //PlayerManager.current.KnockPlayers(playerController, catchPosition);
        TriggerSuspicion();
    }

    public Vector3 DirFromAngle(float angle, bool isGlobal)
    {
        if (!isGlobal)
            angle += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
