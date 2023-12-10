using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// [RequireComponent(typeof(PlayerInput))]  
public class InputManager : MonoBehaviour
{

    // public bool debugShow;

    public Button input_accelerate;
    public Button input_steer;

    public Button input_jump;
    public Button input_drift;


    #region Button Base stuff
    public delegate void ClickAction();
    public event ClickAction released;
    public class Button
    {
        public float value { get; set; }
        public event ClickAction Onpressed;
        public event ClickAction Onreleased;

        public void Pressed() => Onpressed?.Invoke();
        public void Released() => Onreleased?.Invoke();
    }
    #endregion

    PlayerInput playerInput;
    private void Awake()
    {
        input_accelerate = new Button();
        input_steer = new Button();
        input_jump = new Button();
        input_drift = new Button();
    }
private void Start() {
    playerInput=GetComponent<PlayerInput>();
    
    Debug.Log("Input manager is"+playerInput.ToString());
}
    private void OnAccelerate(InputValue inputValue) => SetButtonInfo(input_accelerate, inputValue);    
    private void OnSteer(InputValue inputValue) => SetButtonInfo(input_steer, inputValue);
    private void OnJump(InputValue inputValue) => SetButtonInfo(input_jump, inputValue);
    private void OnDrift(InputValue inputValue) => SetButtonInfo(input_drift, inputValue);

    void SetButtonInfo(Button button, InputValue inputValue)
    {
        float value = inputValue.Get<float>();
        
        var oldValue= button.value; // This is done to prevent OnPressed incorrect value reads  (if value was set before the invoke)
        button.value = value;

        if (value == 0)
            button.Released();
        else if(oldValue==0)
            button.Pressed();

    }
}
