using UnityEngine;

public class GameUIAutoSize : MonoBehaviour
{
	private void Start()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		if (num == 2048f || num2 == 2048f)
		{
			num /= 2f;
			num2 /= 2f;
		}
		num = 1136f;
		num2 = 640f;
		float num3 = num / 960f;
		float num4 = num2 / 640f;
		base.transform.localScale = new Vector3(base.transform.localScale.x * num3, base.transform.localScale.y * num4, base.transform.localScale.z);
	}
}
