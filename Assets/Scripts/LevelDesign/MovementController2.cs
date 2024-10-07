using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController2 : MonoBehaviour
{
    public PlayerInputHandler inputHandler;
    public CharacterController characterController;

    [Header("Gravity")]
    public float gravityScale = .5f;
    [Header("Move")]
    public float movementSpeed = 2;
    public float spintSpeed = 2;
    public float maxAccel = 120f;
    public float maxDeccel = 200f;
    private Vector3 lastMove;

    [Header("Jump")]
    public float jumpStrength = 4;
    public AnimationCurve jumpCurve = AnimationCurve.Constant(0, 1, 1);
    public int jumpTicks = 2;
    private int jumpTickCounter;

    [Header("Extra Jumps")]
    public int extraJumps = 1;
    private int extraJumpCounter;

    [Header("Crouch")]
    public float crouchHeight = 0.1f;
    public float crouchSpeed = 2;
    private float initalHeight;




    private UEventHandler eventHandler = new UEventHandler();
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        initalHeight = characterController.height;

        inputHandler.input_jump.Onpressed.Subscribe(eventHandler, Jump);

        inputHandler.input_crouch.Onpressed.Subscribe(eventHandler, Crouch);
        inputHandler.input_crouch.Onreleased.Subscribe(eventHandler, UnCrouch);

    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }

    void Update()
    {
        Move();
        Gravity();
    }

    void Move()
    {
        //if (!characterController.isGrounded)
        //    return;

        //Get input direction and transform it to the camera orientation
        Vector2 move = inputHandler.input_move.value;
        Vector3 transformedMove = inputHandler.playerCamera.transform.TransformDirection(new Vector3(move.x, 0, move.y));
        transformedMove.y = 0;

        Vector3 newMove = lastMove;

        if (characterController.isGrounded)
        {
            var baseSpeed = inputHandler.input_sprint.value > 0 ? spintSpeed : movementSpeed;

            if (inputHandler.input_crouch.value > 0)
                baseSpeed = crouchSpeed;

            var goalVelocity = baseSpeed * transformedMove;

            var currentHorVel = lastMove;
            currentHorVel.y = 0;

            var acceleration = (goalVelocity - currentHorVel) / Time.fixedDeltaTime;

            //Debug.Log($"goalVelocity  {goalVelocity}");
            //Debug.Log($"currentHorVel  {currentHorVel}");

            //Check if new direction is facing or against current velocity
            float dot = Vector3.Dot(transformedMove, currentHorVel);

            if (dot >= 0 && goalVelocity.magnitude > 0)
                acceleration = Vector3.ClampMagnitude(acceleration, maxAccel);  //If positive or zero apply acceleration clamp
            else
                acceleration = Vector3.ClampMagnitude(acceleration, maxDeccel);  //If negative apply decceleration clamp

            //Debug.Log($"acceleration  {acceleration}");

            var speed = currentHorVel + acceleration * Time.fixedDeltaTime;
            //Debug.Log($"speed  {speed}");

            newMove = speed;
        }


        characterController.Move(newMove * Time.deltaTime);

        lastMove = newMove;

    }

    void Gravity()
    {
        Vector3 jumpValue = Vector3.zero;

        if (jumpTickCounter > 0)
        {
            float diff = jumpTicks - jumpTickCounter;
            Debug.Log($"Diff  {diff}");
            float diffFactor = diff / ((float)jumpTicks);
            Debug.Log($"Diff factor {diffFactor}");

            var jumpFactor = jumpCurve.Evaluate(diffFactor);

            jumpValue = Vector3.up * jumpFactor * jumpStrength;

            jumpTickCounter--;
        }

        characterController.Move((jumpValue + Physics.gravity * gravityScale) * Time.deltaTime);
    }

    void Jump()
    {
        if (characterController.isGrounded)
        {
            extraJumpCounter = 0;
        }


        if (extraJumpCounter > extraJumps)
            return;

        extraJumpCounter++;

        jumpTickCounter = jumpTicks;

    }

    void Crouch()
    {
        Debug.Log("coruch");
        characterController.height = crouchHeight;
    }

    void UnCrouch()
    {
        Debug.Log("uncoruch");
        characterController.height = initalHeight;

    }
}
