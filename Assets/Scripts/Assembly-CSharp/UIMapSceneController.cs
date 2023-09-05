using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMapSceneController : UISceneController
{
	public TUICamera TUICamera;

	public GameObject BackToShopButton;

	public UIShopPanelController MissionPanel;

	public UIShopPanelController MissionDailyPanel;

	public UIShopPanelController NickNamePanel;

	public Transform MissionIcons;

	public Transform coop_trans;

	public List<Transform> mission_icon_trans_list;

	public new static UIMapSceneController Instance
	{
		get
		{
			return (UIMapSceneController)UISceneController.instance;
		}
	}

	public Rect ScreenRect { get; set; }

	private void Awake()
	{
		UISceneController.instance = this;
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		MenuAudioController.CheckGameMenuAudio();
		int num = 2;
		if (!TUI.IsRetina())
		{
			num = 1;
		}
		else if (TUI.IsDoubleHD())
		{
			num = 4;
		}
		ScreenRect = new Rect(0f - (float)(Screen.width / num / 2), 0f - (float)(Screen.height / num / 2), Screen.width / num, Screen.height / num);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Map_Mission_Tag");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			mission_icon_trans_list.Add(gameObject.transform);
		}
	}

	private IEnumerator Start()
	{
		GameObject.Find("LabelDay").GetComponent<TUILabel>().Text = "Day " + GameData.Instance.day_level;
		while (!GameConfig.Instance.Load_finished)
		{
			yield return 1;
		}
		List<QuestInfo> missionQuest = new List<QuestInfo>();
		GameData.Instance.SetMapMissionList(ref missionQuest);
		foreach (QuestInfo info in missionQuest)
		{
			GameObject icon = Object.Instantiate(Resources.Load("Prefab/missionIcon")) as GameObject;
			icon.transform.parent = MissionIcons;
			icon.GetComponent<MissionIconObj>().Init(info);
			yield return 1;
		}
		GameData.Instance.reset_nist_time_finish = OnResetServerTimeFinish;
		GameData.Instance.reset_nist_time_error = OnResetSeverTimeError;
		GameData.Instance.cur_game_type = GameData.GamePlayType.Normal;
		GameData.Instance.is_crazy_daily = false;
		if (TapJoyScript.Instance != null)
		{
			TapJoyScript.Instance.points_add_call_back = OnTapJoyPointsAdd;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GameData.Instance.reset_nist_time_finish = null;
		GameData.Instance.reset_nist_time_error = null;
		if (TapJoyScript.Instance != null)
		{
			TapJoyScript.Instance.points_add_call_back = null;
		}
	}

	public void ConnectServer()
	{
		//IndicatorBlockController.ShowIndicator(TUIControls.gameObject, "Connecting to server...");
		StartCoroutine(GameData.Instance.ResetCurServerTime());
	}

	private void OnResetServerTimeFinish()
	{
		IndicatorBlockController.Hide();
		MissionDailyPanel.Show();
	}

	private void OnResetSeverTimeError()
	{
		OnResetServerTimeFinish();
		//IndicatorBlockController.Hide();
		//GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Unable to connect to the server! Please try again later.", null, null);
	}

	private void OnTapJoyPointsAdd()
	{
		MoneyController.UpdateInfo();
	}
}
