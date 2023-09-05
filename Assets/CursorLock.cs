using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour
{
	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	void Update()
	{
		if (!Application.isMobilePlatform)
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				Screen.lockCursor = !Screen.lockCursor;
			}
		}
	}
}
