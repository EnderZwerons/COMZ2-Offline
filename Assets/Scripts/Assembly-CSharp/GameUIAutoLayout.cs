using UnityEngine;

public class GameUIAutoLayout : MonoBehaviour
{
	public bool left;

	public bool right;

	public bool top;

	public bool bottom;

	private void Start()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		if (num == 2048f || num2 == 2048f)
		{
		}
		float x = base.transform.position.x;
		float y = base.transform.position.y;
		float num3 = Screen.height;
		float num4 = num / num2;
		float num5 = 1.5f - num4;
		float num6 = (float)Screen.width + num5 * 640f;
		if (left)
		{
			x = base.transform.position.x - (num - num6) / 4f;
		}
		if (right)
		{
			x = base.transform.position.x + (num - num6) / 4f;
		}
		if (top)
		{
			y = base.transform.position.y + (num2 - num3) / 4f;
		}
		if (bottom)
		{
			y = base.transform.position.y - (num2 - num3) / 4f;
		}
		base.transform.position = new Vector3(x, y, base.transform.position.z);
	}

	private void Update()
	{
	}

	public void Reset()
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
		float x = base.transform.position.x;
		float y = base.transform.position.y;
		if (left)
		{
			x = base.transform.position.x - (num - 960f) / 4f;
		}
		if (right)
		{
			x = base.transform.position.x + (num - 960f) / 4f;
		}
		if (top)
		{
			y = base.transform.position.y + (num2 - 640f) / 4f;
		}
		if (bottom)
		{
			y = base.transform.position.y - (num2 - 640f) / 4f;
		}
		base.transform.position = new Vector3(x, y, base.transform.position.z);
	}
}
