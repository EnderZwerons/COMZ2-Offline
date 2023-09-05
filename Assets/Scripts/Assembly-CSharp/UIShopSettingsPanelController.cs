using CoMZ2;
using UnityEngine;

public class UIShopSettingsPanelController : UIShopPanelController
{
	public TUIButtonPush MusicButton;

	public TUIButtonPush SoundButton;

	public override void Show()
	{
		base.Show();
		ChangeMask("Mask");
		UpdateMusicButtons();
	}

	public void UpdateMusicButtons()
	{
		MusicButton.m_bPressed = TAudioManager.instance.isMusicOn;
		MusicButton.Show();
		SoundButton.m_bPressed = TAudioManager.instance.isSoundOn;
		SoundButton.Show();
	}

	private void OnButtonSettingsEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			Show();
		}
	}

	private void OnButtonMusicEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		switch (eventType)
		{
		case 1:
			TAudioManager.instance.isMusicOn = true;
			break;
		case 2:
			TAudioManager.instance.isMusicOn = false;
			break;
		}
		MusicButton.m_bPressed = TAudioManager.instance.isMusicOn;
		MusicButton.Show();
	}

	private void OnButtonSoundEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		switch (eventType)
		{
		case 1:
			TAudioManager.instance.isSoundOn = true;
			break;
		case 2:
			TAudioManager.instance.isSoundOn = false;
			break;
		}
		SoundButton.m_bPressed = TAudioManager.instance.isSoundOn;
		SoundButton.Show();
	}

	private void OnButtonCreditEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			Transform transform = base.transform.parent.Find("CreditPanel");
			transform.GetComponent<UIShopPanelController>().Show();
			Transform transform2 = transform.Find("image");
			if (Mathf.Max(Screen.width, Screen.height) == 960)
			{
				float num = 0.8450704f;
				transform2.localScale = new Vector3(num, num, 1f);
			}
			else if (Mathf.Max(Screen.width, Screen.height) == 1024)
			{
				float x = 0.901408434f;
				transform2.localScale = new Vector3(x, 1f, 1f);
			}
		}
	}

	private void OnButtonReviewEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			Debug.LogError("review -------------  OnButtonReviewEvent");
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.trinitigame.android.callofminizombies2");
		}
	}

	private void OnButtonSupportEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			Application.OpenURL(GameDefine.GetSupportUrl());
		}
	}
}
