using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Coin : MonoBehaviour
{
    public Transform model;
    public float rotationSpeed = 5;
    public VisualEffect vfx;

    private Collider col;
    private bool rotate = true;

    private void Awake()
    {
        rotate = true;
        col = GetComponent<Collider>();
    }

    [System.Obsolete]
    void Update()
    {
        if (!rotate)
            return;


        model.RotateAround(Vector3.up, Time.deltaTime * rotationSpeed);
    }

    public void Grabbed()
    {
        vfx.Play();
        rotationSpeed *= 2;

        col.enabled = false;
        model.DOScale(Vector3.zero, 1).SetEase(Ease.OutBack).OnComplete(() => { model.gameObject.SetActive(false); rotate = false; });
    }
}
