using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager current;

    public CinemachineVirtualCamera focusCamera;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    public void FocusCamera(Transform focusPoint)
    {
        focusCamera.Priority = 11;
        focusCamera.LookAt = focusPoint;
    }


    public void UnfocusCamera()
    {
        focusCamera.Priority = 9;
    }

}
