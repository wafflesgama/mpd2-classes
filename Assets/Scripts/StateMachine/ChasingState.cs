using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingState : LogicMachineBehaviour<GuardLogicManager>
{
    [Header("Movement")]
    public float speedMultiplier = 1.5f;

    [Header("Animation")]
    public float lerpSpeed = 20;
    public float showMeterSpeed = 1;
    public Ease showMeterEase = Ease.OutBack;
    public Ease hideMeterEase = Ease.OutQuad;
    public float detectedShakeDuration = 0.9f;
    public float detectedShakeIntensity = 1f;
    public int detectedShakeVibration = 20;

    [Header("Values")]
    public float DetectionDecayValue = -10f;
    public float DetectionIncreaseValue = 20f;
    public float DetectionMaxValue = 100f;

    public float initalDetectedValue = 100f;
    private float detectionValue = 0f;
    private float initMeterScale;
    private Tween detectionTween;

    private int shader_ValueParam;


    public override void OnAwake()
    {
        shader_ValueParam = Shader.PropertyToID("_Amount");

        initMeterScale = manager.detectedMeter.transform.localScale.x;
        manager.detectedMeter.transform.localScale = Vector3.zero;

    }

    public override void OnEnter()
    {
        detectionValue = initalDetectedValue;

        manager.agent.speed *= speedMultiplier;
        manager.vFXController.emitingDust = true;

        //manager.animationController.Shock(true);

        manager.detectedMeter.enabled = true;
        manager.detectedMeter.transform.DOScale(initMeterScale, showMeterSpeed).SetEase(showMeterEase);
        manager.detectedMeter.transform.DOShakePosition(detectedShakeDuration, detectedShakeIntensity, detectedShakeVibration);

        manager.agent.isStopped = false;

    }
    public override void OnUpdate()
    {
        manager.agent.SetDestination(manager.playerController.position);


        if (manager.triggerChecker.hasObject)
        {
            detectionValue = Mathf.Clamp(detectionValue + DetectionIncreaseValue, 0f, 100f);
        }
        else
        {
            detectionValue = Mathf.Clamp(detectionValue + DetectionDecayValue, 0f, 100f);

            if (detectionValue == 0)
                UnTriggerDetection();
        }

        float oldValue = manager.detectedMeter.material.GetFloat(shader_ValueParam);
        float lerpValue = Mathf.Lerp(oldValue, detectionValue, Time.deltaTime * lerpSpeed);
        manager.detectedMeter.material.SetFloat(shader_ValueParam, lerpValue / 100);
    }

    public override void OnExit()
    {
        manager.vFXController.emitingDust = false;
        manager.agent.speed /= speedMultiplier;

        manager.agent.isStopped = true;
        manager.detectedMeter.enabled = false;
    }

    private void UnTriggerDetection()
    {
        logicAnimator.SetBool(manager.Param_Detected, false);
    }




}
