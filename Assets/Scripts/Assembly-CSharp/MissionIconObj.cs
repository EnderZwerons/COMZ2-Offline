using CoMZ2;
using UnityEngine;

public class MissionIconObj : MonoBehaviour
{
	public TUIMeshSprite Background;

	public TUIMeshSprite Image;

	public GameObject Arrow;

	private QuestInfo questInfo;

	private Vector3 lastMapPointPosition;

	private Transform mapPoint;

	private void Update()
	{
		if (mapPoint != null && mapPoint.position != lastMapPointPosition)
		{
			PositionFromWorldToTUI(mapPoint.position);
		}
	}

	public void Init(QuestInfo info)
	{
		questInfo = info;
		TUIMeshSprite background = Background;
		int mission_day_type = (int)info.mission_day_type;
		background.texture = "mission_bk_" + mission_day_type.ToString("G");
		string text = "mission_";
		text = ((info.mission_type == MissionType.Boss) ? (text + "boss_") : ((info.mission_type == MissionType.Npc_Convoy) ? (text + "help_") : ((info.mission_type != MissionType.Npc_Resources) ? (text + "zombie_") : (text + "resource_"))));
		string text2 = text;
		int mission_day_type2 = (int)info.mission_day_type;
		text = text2 + mission_day_type2.ToString("G");
		if (info.mission_type == MissionType.Coop)
		{
			text = "Boss-raid";
		}
		Image.texture = text;
		if (info.mission_type == MissionType.Coop)
		{
			mapPoint = UIMapSceneController.Instance.coop_trans;
			Background.texture = string.Empty;
		}
		else
		{
			int index = Random.Range(0, UIMapSceneController.Instance.mission_icon_trans_list.Count);
			mapPoint = UIMapSceneController.Instance.mission_icon_trans_list[index];
			UIMapSceneController.Instance.mission_icon_trans_list.RemoveAt(index);
		}
		PositionFromWorldToTUI(mapPoint.position);
		UpdateArrow();
	}

	private void PositionFromWorldToTUI(Vector3 pVector3)
	{
		lastMapPointPosition = pVector3;
		Vector3 position = Camera.main.WorldToScreenPoint(pVector3);
		Vector3 vector = UIMapSceneController.Instance.TUICamera.GetComponent<Camera>().ScreenToWorldPoint(position);
		base.transform.position = new Vector3(vector.x, vector.y, base.transform.parent.position.z);
	}

	public void UpdateArrow()
	{
		Vector2 point = new Vector2(base.transform.position.x, base.transform.transform.position.y);
		if (!UIMapSceneController.Instance.ScreenRect.Contains(point))
		{
			Arrow.SetActive(true);
			float num = 57.29578f * Mathf.Atan2(point.y, point.x);
			Arrow.transform.rotation = Quaternion.Euler(0f, 0f, num + 90f);
			if (Mathf.Abs(point.x / point.y) > UIMapSceneController.Instance.TUICamera.GetComponent<Camera>().aspect)
			{
				float num2 = ((!(point.x > 0f)) ? ((0f - UIMapSceneController.Instance.ScreenRect.width) / 2f) : (UIMapSceneController.Instance.ScreenRect.width / 2f));
				float y = num2 * (point.y / point.x);
				Arrow.transform.position = new Vector3(num2, y, Arrow.transform.position.z);
			}
			else
			{
				float num3 = ((!(point.y > 0f)) ? ((0f - UIMapSceneController.Instance.ScreenRect.height) / 2f) : (UIMapSceneController.Instance.ScreenRect.height / 2f));
				float x = num3 * (point.x / point.y);
				Arrow.transform.position = new Vector3(x, num3, Arrow.transform.position.z);
			}
		}
		else
		{
			Arrow.SetActive(false);
		}
	}

	private void OnMissionButtonEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType != 3)
		{
			return;
		}
		UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
		if (questInfo.mission_type == MissionType.Coop)
		{
			if (GameData.Instance.NickName == string.Empty)
			{
				UIMapSceneController.Instance.NickNamePanel.Show();
			}
			else
			{
				UIMapSceneController.Instance.Fade.FadeOut("UICoopHall");
			}
		}
		else if (questInfo.mission_day_type != MissionDayType.Daily)
		{
			((UIMapMissionPanelController)UIMapSceneController.Instance.MissionPanel).QuestInfo = questInfo;
			UIMapSceneController.Instance.MissionPanel.Show();
		}
		else
		{
			((UIMapMissionDailyPanelController)UIMapSceneController.Instance.MissionDailyPanel).QuestInfo = questInfo;
			UIMapSceneController.Instance.ConnectServer();
		}
	}
}
