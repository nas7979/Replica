using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MileagePopup : MonoBehaviour
{
	private void OnEnable()
	{
		StartCoroutine(CR_OpenWindow());
	}

	IEnumerator CR_OpenWindow()
	{
		transform.localScale = new Vector3(0.1f, 0.1f, 1);
		while (transform.localScale.x < 1)
		{
			transform.localScale += new Vector3(5f, 5f, 0) * Time.deltaTime;
			yield return null;
		}
		transform.localScale = new Vector3(1, 1, 1);
	}
}
