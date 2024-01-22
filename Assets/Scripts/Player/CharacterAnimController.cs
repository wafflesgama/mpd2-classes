using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimController : MonoBehaviour
{
    public static CharacterAnimController current;
    public CharacterMovController movController;
    public NavMeshAgent agent;
    public PlayerInputHandler playerInputHandler;

    [Header("Model Rotation")]
    public float rotationSpeed = 5;

    public Vector3 playerDirection;
    public Quaternion playerRotation;

    private Animator animator;
    private UEventHandler eventHandler = new UEventHandler();

    private void Awake()
    {
        current = this;
        animator = GetComponent<Animator>();
        playerInputHandler.input_interact.Onpressed.Subscribe(eventHandler, Kick);
    }
    void Start()
    {
        if (movController != null)
        {
            movController.OnJump.Subscribe(eventHandler, Jump);
            movController.OnLand.Subscribe(eventHandler, Land);
        }
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }

    // Update is called once per frame
    void Update()
    {

        var vel = movController != null ? movController.rb.velocity : agent.velocity;
        vel.y = 0;


        var magnitude = vel.sqrMagnitude;
        animator.SetFloat("Speed", magnitude);

        vel.Normalize();

        if (magnitude > 0 && vel != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vel), rotationSpeed * Time.deltaTime);
            //transform.forward = Vector3.Lerp(transform.forward, vel, Time.deltaTime * rotationSpeed);
        }
        playerRotation = transform.rotation;
        playerDirection = Vector3.Scale(transform.forward, new Vector3(1, 0, 1));
        //Debug.DrawRay(transform.position, playerDirection);
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
    }

    void Land()
    {
        animator.SetTrigger("Land");
    }


    void Kick()
    {
        animator.SetTrigger("Kick");
    }

    public void Shock(bool on=true)
    {
        animator.SetBool("Shock",on);
    }

    public void Recover()
    {
        animator.SetTrigger("Recover");
    }

}
