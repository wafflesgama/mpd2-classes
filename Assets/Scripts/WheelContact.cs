using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelContact : MonoBehaviour
{
    bool isOnContact;

    public bool hasContact() => isOnContact;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            isOnContact = true;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
            isOnContact = false;

    }
}

