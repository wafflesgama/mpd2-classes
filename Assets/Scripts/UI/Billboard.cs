using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool invert;
    public bool orthographic = false;

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(Camera.main.transform);
        if (orthographic)
            transform.forward = Camera.main.transform.forward * (invert ? 1 : -1);
        else
            transform.forward = (Camera.main.transform.position - transform.position) * (invert ? -1 : 1);


    }
}
