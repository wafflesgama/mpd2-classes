using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathbounds : MonoBehaviour
{
    public float delayToFadeIn = 0.5f;
    public float delayToEndShock = 2.5f;
    public bool isShock = false;
    static bool flag;
    private void Awake()
    {
        flag = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (flag) return;

        if (other.tag != "Player") return;

        flag = true;

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        if (isShock)
        {
            CharacterMovController.current.FreezePlayer();
            CharacterAnimController.current.Shock();
            CharacterVFXController.current.Shock();
            yield return new WaitForSeconds(delayToEndShock);
        }

        FadeScreenController.current.FadeOut();
        yield return new WaitForSeconds(delayToFadeIn);

        CharacterMovController.current.ResetPos();
        if (isShock)
        {
            //CharacterAnimController.current.Shock(false);
            CharacterVFXController.current.Shock(false);
            CharacterMovController.current.FreezePlayer(true);
        }

        yield return new WaitForSeconds(delayToFadeIn);

        FadeScreenController.current.FadeIn();
        flag = false;
    }
}
