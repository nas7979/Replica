using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FadeInFadeOut : MonoBehaviour
{
    [SerializeField]
    private float mTime;
    private float mAlpha;
    private SpriteRenderer mRenderer;

    private void Start()
    {
        mRenderer = gameObject.GetComponent<SpriteRenderer>();
		FadeIn(mTime);
    }

    public SpriteRenderer GetRenderer() {
        return mRenderer;
    }
    public void FadeIn(float _Time)
    {
		if (GameManager.GetRound() != 1)
		{
			mRenderer.color = Color.white;
		}
		else
		{
			mRenderer.color = Color.black;
		}
        mRenderer.DOFade(0f, _Time);
    }

    public void FadeOut(string _SceneName, float _Time)
    {
		mRenderer = gameObject.GetComponent<SpriteRenderer>();
        mRenderer.DOFade(1f, _Time).OnComplete(() => { SceneManager.LoadScene(_SceneName); });
    }
    
}