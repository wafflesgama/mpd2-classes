using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMoving : MonoBehaviour
{

    public float offSetX = 4;
    public float durationMov = 2.5f;
    public Ease easeMov = Ease.InOutQuad;
    public int loops = 4500;
    public float delay = 0;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay); 
        transform.DOLocalMoveX(transform.localPosition.x + offSetX, durationMov).SetEase(easeMov).SetLoops(loops, LoopType.Yoyo);
    }
}
