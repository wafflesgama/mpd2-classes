using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspiciousState : LogicMachineBehaviour<GuardLogicManager>
{
    [Header("Animation")]
    public float lerpSpeed = 20;
    public float showMeterSpeed = 1;
    public Ease showMeterEase = Ease.OutBack;
    public Ease hideMeterEase = Ease.OutQuad;

    [Header("Values")]
    public float DetectionDecayValue = -10f;
    public float DetectionIncreaseValue = 20f;
    public float DetectionMaxValue = 100f;

    public float initalDetectedValue = 20f;
    private float detectionValue = 0f;
    private float initMeterScale;
    private Tween detectionTween;

    private int shader_ValueParam;


    public override void OnAwake()
    {
        shader_ValueParam = Shader.PropertyToID("_Amount");

        initMeterScale = manager.detectionMeter.transform.localScale.x;
        manager.detectionMeter.transform.localScale = Vector3.zero;

    }

    public override void OnEnter()
    {
        detectionValue = initalDetectedValue;

        detectionTween?.Kill();
        detectionTween = null;

        manager.detectionMeter.enabled = true;
        detectionTween = manager.detectionMeter.transform.DOScale(initMeterScale, showMeterSpeed).SetEase(showMeterEase);
        manager.animationController.Look(true);

        manager.detectionMeter.material.SetFloat(shader_ValueParam, detectionValue / 100);
    }
    public override void OnUpdate()
    {
        if (manager.triggerChecker.hasObject)
        {
            detectionValue = Mathf.Clamp(detectionValue + DetectionIncreaseValue, 0f, 100f);


            if (detectionValue == 100)
                TriggerDetection();
        }
        else
        {
            detectionValue = Mathf.Clamp(detectionValue + DetectionDecayValue, 0f, 100f);

            if (detectionValue == 0)
                UntriggerSuspicion();
        }

        float oldValue = manager.detectionMeter.material.GetFloat(shader_ValueParam);
        float lerpValue = Mathf.Lerp(oldValue, detectionValue, Time.deltaTime * lerpSpeed);
        manager.detectionMeter.material.SetFloat(shader_ValueParam, lerpValue / 100);
    }

    public override void OnExit()
    {
        manager.animationController.Look(false);
        detectionTween?.Kill();
        detectionTween = null;

        detectionTween = manager.detectionMeter.transform.DOScale(0, showMeterSpeed).SetEase(hideMeterEase);
    }



    private void TriggerDetection()
    {
        logicAnimator.SetBool(manager.Param_Detected, true);
    }

    private void UntriggerSuspicion()
    {
        logicAnimator.SetBool(manager.Param_Suspicious, false);
    }




}
