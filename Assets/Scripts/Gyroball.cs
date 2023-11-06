using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyroball : MonoBehaviour
{
    // Start is called before the first frame update

    public float Speed = 0.1f;
    public Rigidbody rb;
    public float amplitude = 1.2f;

    public AudioClip clip;
    void Start()
    {
        //clip=  Microphone.Start(Microphone.devices[0], true, 20, AudioSettings.outputSampleRate);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public float GetLoudnessFromAudioClip()
    //{

    //}


    private void FixedUpdate()
    {
        var forward = Camera.main.transform.forward;
        forward.y = 0;



        var direction = Quaternion.AngleAxis(90 * Input.gyro.attitude.z * amplitude, Vector3.up) * forward;

        Debug.Log(Input.gyro.attitude);

        Debug.DrawRay(transform.position, direction);

        rb.AddForce(direction * Speed);
    }

    protected void OnGUI()
    {
        GUI.skin.label.fontSize = Screen.width / 40;

        GUILayout.Label("Orientation: " + Screen.orientation);
        GUILayout.Label("input.gyro.attitude: " + Input.gyro.attitude);
        GUILayout.Label("iphone width/font: " + Screen.width + " : " + GUI.skin.label.fontSize);
    }
}
