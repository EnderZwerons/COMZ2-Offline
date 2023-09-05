using UnityEngine;

public class UIPromotionPanelController : UIShopPanelController
{
	public GameObject Promotion_Button;

	public void ShowButton()
	{
		Promotion_Button.SetActive(true);
	}

	public void HideButton()
	{
		Promotion_Button.SetActive(false);
	}

	private void OnCOMPromotionEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			UIShopSceneController.Instance.PromotionPanel.Show();
		}
	}

	private void OnCOMZombiesEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://youtu.be/-k1h5XA68Og?si=iJGj1Hoh9eXy-RyM");
		}
	}

	private void OnCOMInfinityEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.trinitigame.android.callofminiinfinity");
		}
	}

	private void OnCOMDinoHunterEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.trinitigame.android.callofminidinohunter");
		}
	}

	private void OnCOMBrawlersEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://youtu.be/hpEKtFcr-D4?si=OSy2E08_h6Ncjcdf");
		}
	}

	private void OnCOMDoubleShootEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://youtu.be/zFDuLE_B5Cw?si=XXNpzA0W-EhCSsMY");
		}
	}

	private void OnCOMSniperEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://youtu.be/ykk2i92S2Q8?si=8tDdnULtAg9jfSu3");
		}
	}
}
