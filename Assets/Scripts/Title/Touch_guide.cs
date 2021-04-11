using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Touch_guide : MonoBehaviour
{
    float time = 0;
    bool isblur = true;
    private SpriteRenderer mRenderer;
    // Update is called once per frame
    private void OnEnable()
    {
        mRenderer = GetComponent<SpriteRenderer>();
        mRenderer.color = new Color(1, 1, 1, 0);
        mRenderer.DOFade(1, 2).SetLoops(-1, LoopType.Yoyo);
    }

}
