using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        transform.rotation = Quaternion.Inverse(Camera.main.transform.rotation)* Input.gyro.attitude;
    }
}
