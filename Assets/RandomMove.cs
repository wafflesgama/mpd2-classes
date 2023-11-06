using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour
{
    public Transform[] platforms;
    public float offSetY = 2;
    public float duration = 4;
    public Ease ease = Ease.InOutBack;
    private void Awake()
    {
        platforms = new Transform[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            platforms[i] = child;
            i++;
        }
    }
    void Start()
    {


        foreach (var platform in platforms)
        {
            platform.DOMoveY(platform.transform.position.y + offSetY, duration).SetEase(ease).SetLoops(100, LoopType.Yoyo).SetDelay(Random.Range(0, 0.9f));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
