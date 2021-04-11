using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveFalseAuto : MonoBehaviour
{
    [SerializeField]
    private float mTime;
    private void OnEnable()
    {
        StartCoroutine(CR_ActiveFalse());
    }

    IEnumerator CR_ActiveFalse()
    {
        yield return new WaitForSeconds(mTime);
        gameObject.SetActive(false);
    }
}
