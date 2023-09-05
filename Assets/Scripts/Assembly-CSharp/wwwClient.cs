using System.Collections;
using UnityEngine;

public class wwwClient : MonoBehaviour
{
	private static wwwClient instance;

	public static wwwClient Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public static void CheckWwwClient()
	{
		if (!GameObject.Find("wwwClient"))
		{
			GameObject gameObject = new GameObject("wwwClient");
			gameObject.AddComponent<wwwClient>();
		}
	}

	public void SendHttpRequest(string url, byte[] post_data, OnRequestFinished on_request_finish, OnRequestError on_request_error, string action)
	{
		StartCoroutine(SendRequest(url, post_data, on_request_finish, on_request_error, action));
	}

	private IEnumerator SendRequest(string url, byte[] post_data, OnRequestFinished on_request_finish, OnRequestError on_request_error, string action)
	{
		WWW www2 = null;
		www2 = ((post_data != null) ? new WWW(url, post_data) : new WWW(url));
		yield return www2;
		if (www2.error != null)
		{
			Debug.Log(www2.error);
			if (on_request_error != null)
			{
				on_request_error(action, post_data);
			}
		}
		else
		{
			byte[] result_data = www2.bytes;
			if (on_request_finish != null)
			{
				on_request_finish(action, result_data);
			}
		}
	}
}
