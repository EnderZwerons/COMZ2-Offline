using UnityEngine;

public class UIMapInputController : MonoBehaviour
{
	public TUICamera TUICamera;

	public Transform MapTransform;

	public Transform Left;

	public Transform Right;

	public Transform Up;

	public Transform Down;

	private void OnBackToShopEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UIMapSceneController.Instance.SceneAudio.PlayAudio("UI_back");
			UIMapSceneController.Instance.Fade.FadeOut("UIShop");
		}
	}

	private void OnBackToSceneEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UIMapSceneController.Instance.SceneAudio.PlayAudio("UI_back");
			if (UIMapSceneController.Instance.PanelStack.Count > 0)
			{
				UIMapSceneController.Instance.PanelStack.Peek().Hide(true);
			}
		}
	}

	private void OnMoveMapEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType != 2)
		{
			return;
		}
		MapTransform.localPosition += new Vector3(wparam, 0f, lparam) * 3f;
		float x = Mathf.Clamp(MapTransform.localPosition.x, Left.localPosition.x, Right.localPosition.x);
		float z = Mathf.Clamp(MapTransform.localPosition.z, Down.localPosition.z, Up.localPosition.z);
		MapTransform.localPosition = new Vector3(x, MapTransform.localPosition.y, z);
		foreach (Transform missionIcon in UIMapSceneController.Instance.MissionIcons)
		{
			missionIcon.GetComponent<MissionIconObj>().UpdateArrow();
		}
	}
}
