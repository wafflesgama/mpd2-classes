using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum Axel
{
    Front, Rear
}

[Serializable]
public struct Wheel
{
    public GameObject model;
    public WheelCollider col;
    public WheelContact contact;
    public Axel axel;
    public bool leftSide;

}
public class VechicleController : MonoBehaviour
{
    #region  Public Vars

    [Header("Refs")]
    public InputManager inputManager;
    public TrailRenderer[] trailRenderers;

    [Header("Movement Controller")]

    [SerializeField]
    public List<Wheel> wheels;

    [Label("Center of Mass offset")]
    public Vector3 centerOfMass;


    [Label("Wheels on ground threshold")]
    [Range(0, 4)]
    public int groundSens = 2;

    public float acceleration = 20;

    public float maxMoveSpeed = 14;

    public float turnSensitivity = 1;

    public float maxSteerAngle = 45;
    public float decreaseFactorTurn = .2f;


    [Header("Jump")]
    public float jumpHeightMult = 1.5f;

    public float jumpAirbourneCompensation = 10;

    public float rotationSpeedAir = 2;

    public float driftJumpHeightMult = 1.5f;

    public float driftJumpTorque = 5;

    [Header("Drift")]
    public float driftFactorSide = 6.5f;
    public float driftFactorFront = 2;
    public float driftFollowForceFactor = .04f;
    public float driftForce;

    public float tireAnimAngle = 50;

    public float torqueKnockbackFactor = 1.5f;

    public float forwardKnockbackFactor = 100f;


    #endregion Public Vars
    //------------------------------------------------------------------------
    #region  Private Vars
    Rigidbody body;

    WheelFrictionCurve defautlFrictionFront;
    WheelFrictionCurve defautlFrictionSide;

    float standardDistance;

    float jumpTime = 0;

    Vector3 driftDirection;
    bool canSteer = true;
    bool canDriftCheck;
    Coroutine delayedDriftRoutine;
    float driftSpeedCap = float.MaxValue;
    Coroutine releaseDriftRoutine;

    #endregion Private Vars
    //------------------------------------------------------------------------
    #region  Debug Exposed Vars

    [ReadOnly]
    public bool isGrounded;


    [ReadOnly]
    [Label("Wheels grounded")]
    public float sens;

    [ReadOnly]
    public bool isDrifting;

    [ReadOnly]
    public int driftTimeCounter;

    [ReadOnly]
    public bool isInKnockBack;

    #endregion  Debug Exposed Vars

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = false;

        CalculateCenterOfMass();

        defautlFrictionFront = wheels[0].col.forwardFriction;
        defautlFrictionSide = wheels[0].col.sidewaysFriction;
        standardDistance = wheels[0].col.suspensionDistance;

        SetSubSteps();
        SubcribeToEvents();
    }

    void CalculateCenterOfMass()
    {
        float xPos = body.centerOfMass.x, zPos = body.centerOfMass.z;
        float yPos = (transform.TransformPoint(new Vector3(0, body.centerOfMass.y, 0))).y;

        foreach (var wheel in wheels)
        {
            xPos += wheel.col.transform.position.x;
            zPos += wheel.col.transform.position.z;
        }

        zPos /= wheels.Count;
        xPos /= wheels.Count;

        body.centerOfMass = transform.InverseTransformPoint(xPos, yPos, zPos);
        body.centerOfMass += centerOfMass;

    }

    void SetSubSteps()
    {
        foreach (var wheel in wheels)
        {
            wheel.col.ConfigureVehicleSubsteps(5, 15, 20);
        }
    }

    private void OnDestroy()
    {
        UnsubcribeToEvents();
    }

    void SubcribeToEvents()
    {
        inputManager.input_jump.Onpressed += Jump_Down;
        inputManager.input_jump.Onreleased += Jump_Up;

        inputManager.input_drift.Onpressed += Drift_Down;
        inputManager.input_drift.Onreleased += Drift_Up;
    }

    void UnsubcribeToEvents()
    {
        inputManager.input_jump.Onpressed -= Jump_Down;
        inputManager.input_jump.Onreleased -= Jump_Up;

        inputManager.input_drift.Onpressed -= Drift_Down;
        inputManager.input_drift.Onreleased -= Drift_Up;
    }

    void Update()
    {
        AnimateWheels();
        DriftCheck();
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                Vector3 newRot = new Vector3(-90 - (wheel.leftSide ? tireAnimAngle : -tireAnimAngle) * CalculateSteerValue(), wheel.leftSide ? 90 : -90, 0);
                Debug.Log("Left" + wheel.leftSide + "Rot: " + newRot);
                wheel.model.transform.localEulerAngles = newRot;
            }
            Quaternion rot;
            Vector3 pos;
            wheel.col.GetWorldPose(out pos, out rot);
            wheel.model.transform.position = pos;
            wheel.contact.transform.position = pos;

        }
    }

    void FixedUpdate()
    {
        //GroundCheck();
        //JumpAirbourne();
        //Airbourne();

        //MoveVechicle();
        //TurnVechicle();

        //DriftForce();
        //DriftJumpTorque();
    }


    void GroundCheck()
    {
        isGrounded = true;

        foreach (var wheel in wheels)
            if (wheel.contact.hasContact()) return;

        isGrounded = false;
    }

    void MoveVechicle()
    {

        var move = 1;
        if (body.velocity.magnitude > maxMoveSpeed)
            move = 0;

        foreach (var wheel in wheels)
        {
            wheel.col.motorTorque = -inputManager.input_accelerate.value * move * acceleration * 500 * Time.deltaTime;
        }
    }

    void TurnVechicle()
    {
        if (!canSteer) return;

        sens = 1 / (decreaseFactorTurn * body.velocity.magnitude + 1);

        foreach (var wheel in wheels)
        {
            if (wheel.axel is Axel.Front)
            {
                var steerAngle = CalculateSteerValue() * maxSteerAngle * turnSensitivity * sens * Time.deltaTime;

                wheel.col.steerAngle = Mathf.Lerp(wheel.col.steerAngle, steerAngle, Time.deltaTime);
            }
        }
    }

    float CalculateSteerValue()
    {
        if (!isDrifting) return -inputManager.input_steer.value;

        return -inputManager.input_steer.value;
    }

    void Setfriction()
    {
        foreach (var wheel in wheels)
        {
            wheel.col.sidewaysFriction = defautlFrictionSide;
            wheel.col.forwardFriction = defautlFrictionFront;
        }
    }

    #region DRIFT


    void DriftForce()
    {
        if (!isDrifting || !isGrounded) return;

        driftTimeCounter++;
        driftDirection = (driftDirection + (transform.forward.normalized * -driftFollowForceFactor)).normalized;

        if (body.velocity.magnitude < driftSpeedCap && body.velocity.magnitude < maxMoveSpeed)
            body.AddForce(driftDirection * body.mass * 10 * driftForce);
    }

    void DriftCheck()
    {
        if (!isDrifting || !canDriftCheck || isGrounded) return;
        canDriftCheck = false;
        Drift_Up();
    }

    void Drift_Down()
    {
        if (!isGrounded || isDrifting) return;

        foreach (var trailRenderer in trailRenderers)
        {
            trailRenderer.emitting = true;
        }

        driftSpeedCap = body.velocity.magnitude;
        isDrifting = true;
        DriftJump();
        Debug.LogWarning("Drift down");

        if (releaseDriftRoutine != null)
            StopCoroutine(releaseDriftRoutine);

        defautlFrictionSide.stiffness /= driftFactorSide;
        defautlFrictionFront.stiffness /= driftFactorFront;
        turnSensitivity *= driftFactorSide;
        Setfriction();
        driftDirection = -transform.forward.normalized;

    } 

    void Drift_Up()
    {
        if (!isDrifting) return;

        foreach (var trailRenderer in trailRenderers)
        {
            trailRenderer.emitting = false;
        }

        driftTimeCounter = 0;
        isDrifting = false;
        Debug.LogWarning("Drift up");

        defautlFrictionSide.stiffness *= driftFactorSide;
        defautlFrictionFront.stiffness *= driftFactorFront;
        turnSensitivity /= driftFactorSide;
        Setfriction();

        if (delayedDriftRoutine != null)
            StopCoroutine(delayedDriftRoutine);
        canDriftCheck = false;
    }

    void DriftJump()
    {
        var tempJumpHeightMult = jumpHeightMult;
        jumpHeightMult = driftJumpHeightMult;
        Jump_Down();
        jumpHeightMult = tempJumpHeightMult;
    }
    void DriftJumpTorque()
    {
        if (!isDrifting || isGrounded) return;
        if (CalculateSteerValue() == 0) return;
        body.AddTorque((CalculateSteerValue() > 0 ? transform.up : -transform.up) * driftJumpTorque, ForceMode.Acceleration);
    }

    #endregion

    #region JUMP
    void Jump_Down()
    {
        jumpTime = 1;
        foreach (var wheel in wheels)
        {
            wheel.col.suspensionDistance = standardDistance * jumpHeightMult;
        }
        StartCoroutine(JumpOffDelay());

        if (isGrounded)
            body.AddForce(Vector3.up * 100000);
    }

    IEnumerator JumpOffDelay()
    {
        yield return new WaitForSeconds(.1f);
        foreach (var wheel in wheels)
        {
            wheel.col.suspensionDistance = standardDistance;
        }
    }

    void Jump_Up()
    {
        jumpTime = 0;
    }

    void JumpAirbourne()
    {
        if (jumpTime > 0)
        {
            jumpTime += .1f;
            body.AddForce(Vector3.up * jumpAirbourneCompensation * jumpTime);
        }
    }


    void Airbourne()
    {
        if (isGrounded) return;

        body.MoveRotation(body.rotation * Quaternion.Euler(-inputManager.input_accelerate.value * rotationSpeedAir, CalculateSteerValue() * rotationSpeedAir, 0));
    }


    #endregion Jump


    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!isGrounded || isInKnockBack) return;

    //    isInKnockBack = true;
    //    canSteer = false;
    //    StartCoroutine(SlowUpTime());
    //    var collisionVector = -collision.contacts[0].normal;

    //    var dotVector = Vector3.Dot(collision.impulse, transform.forward);
    //    Debug.LogError("Collision is dotVector -> " + dotVector);
    //    Debug.LogError("Collision with force -> " + collision.impulse.magnitude + " angle front " + Vector3.Angle(transform.forward, collision.impulse) + "\n Angle right " + Vector3.Angle(transform.right, collisionVector) + " angle left " + Vector3.Angle(-transform.right, collisionVector));

    //    bool leftSide = Vector3.Angle(transform.right, collisionVector) > Vector3.Angle(-transform.right, collisionVector);
    //    Debug.LogError("Collision is force -> " + (leftSide ? "left" : "right"));
    //    collisionVector.y *= 0;
    //    var forwardAngleFactor = Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(transform.forward, collision.impulse)), 2);
    //    var sideAngleFactor = Mathf.Pow(Mathf.Sin(2 * Mathf.Deg2Rad * Vector3.Angle(transform.forward, collision.impulse)), 2);
    //    Debug.LogError("Collision  forwardAngleFactor -> " + forwardAngleFactor);
    //    body.AddForce(-collisionVector * forwardKnockbackFactor * forwardAngleFactor * collision.impulse.magnitude, ForceMode.Force);
    //    body.AddTorque(transform.up * (leftSide ? 1 : -1) * sideAngleFactor * collision.impulse.magnitude * torqueKnockbackFactor * body.mass);

    //}
    //IEnumerator SlowUpTime()
    //{
    //    yield return new WaitForSeconds(.5f);
    //    canSteer = true;
    //    Setfriction();
    //    isInKnockBack = false;
    //    // Time.timeScale=1;
    //}
}
