using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloodEffect : MonoBehaviour
{
    [SerializeField]
    private float mTime;
    [SerializeField]
    private float mDelay;
    private float mAlpha;
    private SpriteRenderer mRenderer;
    private void OnEnable()
    {
        StartCoroutine(CR_ActiveFalse(mDelay));
    }

    private IEnumerator CR_ActiveFalse(float _Delay)
    {
        mRenderer = gameObject.GetComponent<SpriteRenderer>();
        mRenderer.color = Color.white;
        yield return new WaitForSeconds(_Delay);
        mRenderer.DOFade(0, mTime).OnComplete(()=> 
        {
            gameObject.SetActive(false);
        });
            yield break;
    }
}
