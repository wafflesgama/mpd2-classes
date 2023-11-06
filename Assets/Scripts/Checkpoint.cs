using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Checkpoint : MonoBehaviour
{
    public VisualEffect vfx;
    public Animator animator;

    bool flag = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        if (flag) return;
        flag = true;

        animator.SetTrigger("Crossed");
        vfx.SendEvent("OnParty");

        CharacterMovController.current.UpdateCheckPoint();

        StartCoroutine(WaitToTurnFlagOn());

    }

    IEnumerator WaitToTurnFlagOn()
    {
        yield return new WaitForSeconds(12);

        flag = false;
    }
}
