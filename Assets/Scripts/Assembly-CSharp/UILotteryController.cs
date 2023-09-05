using System.Collections;
using UnityEngine;

public class UILotteryController : UISceneController
{
	public UILotterManager lotter_manager;

	public GameObject lottery_button;

	public GameObject lottery_free_button;

	public TUILabel lottery_price_p;

	public TUILabel lottery_price;

	public TUILabel reset_price;

	public GameObject block_bk;

	public UIMoneyController money_controller;

	public TUILabel lottery_bar_label;

	public TUIMeshSprite lottery_bar;

	public TUIRect lottery_bar_rect;

	private float lottery_bar_rect_width = 322f;

	public bool lotter_count_award;

	public new static UILotteryController Instance
	{
		get
		{
			return (UILotteryController)UISceneController.instance;
		}
	}

	private void Awake()
	{
		UISceneController.instance = this;
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		MenuAudioController.CheckGameMenuAudio();
		GameData.Instance.reset_nist_time_finish = OnResetServerTimeFinish;
		GameData.Instance.reset_nist_time_error = OnResetSeverTimeError;
	}

	private void Start()
	{
		//IndicatorBlockController.ShowIndicator(TUIControls.gameObject, "Connecting to server...");
		StartCoroutine(GameData.Instance.ResetCurServerTime());
		lottery_free_button.gameObject.SetActive(false);
		lottery_button.gameObject.SetActive(false);
		lottery_price_p.Text = GameConfig.Instance.lottery_price.GetIntVal().ToString();
		lottery_price.Text = GameConfig.Instance.lottery_price.GetIntVal().ToString();
		reset_price.Text = GameConfig.Instance.lottery_reset_price.GetIntVal().ToString();
		EnableBlock(false);
		if (TapJoyScript.Instance != null)
		{
			TapJoyScript.Instance.points_add_call_back = OnTapJoyPointsAdd;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		AwardChangePanel.DestroyPanel();
		AwardGetPanel.DestroyPanel();
		GameData.Instance.reset_nist_time_finish = null;
		GameData.Instance.reset_nist_time_error = null;
		GameMsgBoxController.DestroyMsgBox();
		if (TapJoyScript.Instance != null)
		{
			TapJoyScript.Instance.points_add_call_back = null;
		}
	}

	private void OnMsgBoxOkButton()
	{
		Debug.Log("OnMsgBoxOkButton");
		Fade.FadeOut("UIShop");
	}

	private void OnBackButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Debug.Log("OnBackButton");
			SceneAudio.PlayAudio("UI_back");
			Fade.FadeOut("UIShop");
		}
	}

	private void OnResetButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			if (GameData.Instance.total_crystal.GetIntVal() >= GameConfig.Instance.lottery_reset_price.GetIntVal())
			{
				GameData.Instance.total_crystal -= GameConfig.Instance.lottery_reset_price.GetIntVal();
				lotter_manager.ResetSeatLevel(true);
				UISceneController.Instance.MoneyController.UpdateInfo();
				Hashtable hashtable = new Hashtable();
				hashtable.Add("tCrystalNum", GameConfig.Instance.lottery_reset_price.GetIntVal());
				GameData.Instance.UploadStatistics("tCrystal_Use_Lottery_Reset", hashtable);
			}
			else
			{
				ShowIapPanel();
			}
		}
	}

	private void LotteryButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			if (GameData.Instance.total_crystal.GetIntVal() >= GameConfig.Instance.lottery_price.GetIntVal())
			{
				GameData.Instance.total_crystal -= GameConfig.Instance.lottery_price.GetIntVal();
				lotter_manager.StartLottery();
				UISceneController.Instance.MoneyController.UpdateInfo();
				Hashtable hashtable = new Hashtable();
				hashtable.Add("tCrystalNum", GameConfig.Instance.lottery_price.GetIntVal());
				GameData.Instance.UploadStatistics("tCrystal_Use_Lottery", hashtable);
			}
			else
			{
				ShowIapPanel();
			}
		}
	}

	private void LotteryFreeButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			lotter_manager.StartLottery();
			lottery_free_button.gameObject.SetActive(false);
			lottery_button.gameObject.SetActive(true);
		}
	}

	private void OnResetServerTimeFinish()
	{
		IndicatorBlockController.Hide();
		if (GameData.Instance.lottery_reset_count == 0)
		{
			Debug.Log("reset lottery seat for free.");
			lotter_manager.ResetSeatLevel(false);
		}
		if (GameData.Instance.lottery_count == 0)
		{
			lottery_free_button.gameObject.SetActive(true);
			lottery_button.gameObject.SetActive(false);
		}
		else
		{
			lottery_free_button.gameObject.SetActive(false);
			lottery_button.gameObject.SetActive(true);
		}
		UpdateLotteryBar();
	}

	private void OnResetSeverTimeError()
	{
		OnResetServerTimeFinish();
		//IndicatorBlockController.Hide();
		//GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Unable to connect to the server! Please try again later.", OnMsgBoxOkButton, null);
	}

	public void EnableBlock(bool state)
	{
		block_bk.SetActive(state);
	}

	public void ShowIapPanel()
	{
		money_controller.IapPanel.Show();
	}

	public void UpdateLotteryBar()
	{
		if (GameData.Instance.lottery_count >= GameConfig.Instance.lottery_award_count.GetIntVal() + 1)
		{
			GameData.Instance.lottery_count = 1;
			lotter_count_award = true;
		}
		int num = GameData.Instance.lottery_count - 1;
		if (num < 0)
		{
			num = 0;
		}
		float x = (float)num / (float)GameConfig.Instance.lottery_award_count.GetIntVal() * lottery_bar_rect_width;
		lottery_bar_rect.Size = new Vector2(x, 20f);
		lottery_bar_rect.NeedUpdate = true;
		lottery_bar.NeedUpdate = true;
		lottery_bar_label.Text = "Free Spins:" + num;
	}

	private void OnTapJoyPointsAdd()
	{
		MoneyController.UpdateInfo();
	}
}
