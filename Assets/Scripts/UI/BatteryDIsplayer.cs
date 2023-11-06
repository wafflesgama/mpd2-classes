using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BatteryDIsplayer : MonoBehaviour
{
    public static BatteryDIsplayer current;

    public RectTransform container;
    public float moveYShow = 5;
    public float moveDuration = .6f;
    public Ease moveEase = Ease.OutBack;

    public float shakeMinRatio = 0.1f;
    public float shakeStrength = 2;
    public float shakeSpeed = 0.1f;

    public Image fillImage;
    public Color lowBatteryColor;
    public Color highBatteryColor;

    public Vector3 initPos;

    Tween moveTween;
    bool showing;
    private void Awake()
    {
        current = this;

        showing = false;
        initPos = container.localPosition;
        container.localPosition = new Vector3(container.localPosition.x, container.localPosition.y - moveYShow, container.localPosition.z);
    }

    public void ShowBattery()
    {
        showing = true;
        moveTween= container.DOLocalMoveY(initPos.y, moveDuration).SetEase(moveEase);
    }


    // Update is called once per frame
    void Update()
    {
        if (!showing) return;

        var fuel = CharacterMovController.current.fuel;
        var maxFuel = CharacterMovController.current.maxFuel;
        var ratio = fuel / maxFuel;
        fillImage.fillAmount = ratio;

        fillImage.color = Color.Lerp(lowBatteryColor, highBatteryColor, ratio);

        if (CharacterMovController.current.isUsingJetpack && ratio <= shakeMinRatio)
            Shake();
        else if(!moveTween.active)
            container.localPosition = initPos;
    }

    void Shake()
    {
        var noiseVal = Mathf.PerlinNoise(Time.time * shakeSpeed, 0);
        container.localPosition = initPos + new Vector3(0, noiseVal * shakeStrength, 0);
    }
}
