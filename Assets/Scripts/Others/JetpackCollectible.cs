using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackCollectible : MonoBehaviour
{

    private bool flag;

    private void Awake()
    {
        flag = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        if (flag) return;

        flag = true;
        CharacterMovController.current.UnlockJetpack();
        BatteryDIsplayer.current.ShowBattery();

        gameObject.SetActive(false);
    }
}
