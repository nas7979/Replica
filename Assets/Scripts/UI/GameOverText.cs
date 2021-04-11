using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverText : MonoBehaviour
{

	private void Start()
	{
		StartCoroutine(CR_GameClearTextScaling());
	}

	void Update()
    {
        if(Input.GetMouseButtonDown(0))
		{
			SceneManager.LoadScene("Title");
		}
    }

	public IEnumerator CR_GameClearTextScaling()
	{
		transform.localScale = new Vector3(0.1f, 0.1f);
		while (transform.localScale.x < 1)
		{
			transform.localScale += new Vector3(5f, 5f) * Time.deltaTime;
			yield return null;
		}
		transform.localScale = new Vector3(1, 1);
	}
}
