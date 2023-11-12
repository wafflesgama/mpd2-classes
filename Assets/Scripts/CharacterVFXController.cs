using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CharacterVFXController : MonoBehaviour
{
    public static CharacterVFXController current;

    public CharacterMovController mov;

    [Header("Dust Trail")]
    public VisualEffect dustVfx;
    public bool emitingDust = false;
    public int dustRate;

    public float minSpeed;
    public AnimationCurve dustCurve;
    int dustRateVar, dustJumpVar, dustLandVar;
    bool jumpFlag = true;

    [Header("Knock")]
    public VisualEffect knockVfx;
    int aliveVar;

    public float jetpackDustAmount = 10;

    [Header("Hit")]
    public VisualEffect hitVfx;
    int hitVar, hitEvent;

    UEventHandler eventHandler = new UEventHandler();


    private void Awake()
    {
        current = this;
        dustRateVar = Shader.PropertyToID("Rate");
        dustJumpVar = Shader.PropertyToID("OnJump");
        dustLandVar = Shader.PropertyToID("OnLand");
        aliveVar = Shader.PropertyToID("Alive");
        hitVar = Shader.PropertyToID("Pos");
        hitEvent = Shader.PropertyToID("OnHit");
    }
    void Start()
    {
        if (mov != null)
        {
            mov.OnJump.Subscribe(eventHandler, Jump);
            mov.OnLand.Subscribe(eventHandler, Land);
        }
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }


    private void Jump()
    {
        //if (!jumpFlag) return;

        dustVfx.SendEvent(dustJumpVar);
        //jumpFlag= false;
    }
    private void Land()
    {
        //jumpFlag= true;
        dustVfx.SendEvent(dustLandVar);
    }
    private void LateUpdate()
    {
        AnimateDust();
    }

    void AnimateDust()
    {
        if (!emitingDust) return;

        float dustAmount = dustRate;
        if (mov != null)
        {
            var show = mov.horizontalVelMag > minSpeed ? 1 : 0;
            var speedFraction = mov.horizontalVelMag / mov.goalVelocity;
            dustAmount = dustCurve.Evaluate(speedFraction) * dustRate * show;
            dustAmount = mov.isGrounded ? dustAmount : 0;
        }

        dustVfx.SetFloat(dustRateVar, dustAmount);
    }

    [ContextMenu("Shock")]
    public void Shock(bool on = true)
    {
        if (on)
            hitVfx.SendEvent("OnShock");
        else
            hitVfx.SendEvent("OnShockStop");
    }

    [ContextMenu("Recover")]
    public void Recover()
    {
        knockVfx.SetBool(aliveVar, false);
        knockVfx.Stop();
    }
}
