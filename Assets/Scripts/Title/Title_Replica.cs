using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Title_Replica : MonoBehaviour
{
    [SerializeField]
    private GameObject mShadow;
    [SerializeField]
    private GameObject mLight;
    [SerializeField]
    private Ease mEase;
    void Start()
    {
        transform.position = new Vector3(0, 12);
        transform.DOMove(new Vector3(0, -5f), 1f).SetEase(mEase);
    }

    private void OnEnable()
    {
        transform.position = new Vector3(0, 12);
        transform.DOMove(new Vector3(0, -5f), 1f).SetEase(Ease.OutBounce).OnComplete(()=> { mLight.SetActive(true); });
        StartCoroutine(CR_Update());
    }

    public IEnumerator CR_Update()
    {
        while(gameObject.activeSelf)
        {
            float Distance = transform.position.y - mShadow.transform.position.y;
            mShadow.transform.localScale = Vector3.one - new Vector3(Distance, Distance) * 0.05f;
            yield return null;
        }
        yield break;
    }
}

