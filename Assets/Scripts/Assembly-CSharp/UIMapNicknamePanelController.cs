using System.Text.RegularExpressions;
using UnityEngine;

public class UIMapNicknamePanelController : UIShopPanelController
{
	public TUILabel nick_name;

	private TouchScreenKeyboard keyboard;

	private bool is_input_name;

	protected Regex myRex;

	private string input_str = string.Empty;

	public override void Show()
	{
		base.Show();
		if (GameConfig.IsEditorMode())
		{
			nick_name.Text = (input_str = "Test");
			return;
		}
		input_str = string.Empty;
		is_input_name = true;
		keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.ASCIICapable);
	}

	public override void Hide(bool isPopFromStack)
	{
		is_input_name = false;
		base.Hide(isPopFromStack);
	}

	private void Awake()
	{
		myRex = new Regex("^[A-Za-z0-9]+$");
	}

	private void Update()
	{
		if (is_input_name && keyboard != null)
		{
			nick_name.Text = (input_str = keyboard.text);
		}
		if (input_str.Length > 8)
		{
			nick_name.Text = (input_str = input_str.Substring(0, 8));
		}
	}

	private void OnOKButton(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			if (keyboard != null)
			{
				keyboard.active = false;
			}
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			is_input_name = false;
			Match match = myRex.Match(input_str);
			if (input_str.Length > 0 && match.Success)
			{
				GameData.Instance.NickName = input_str;
				GameData.Instance.SaveData();
				UIMapSceneController.Instance.Fade.FadeOut("UICoopHall");
			}
			else
			{
				GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, base.gameObject, "Invalid name. Please try again!", OnMsgOkButton, null);
			}
		}
	}

	private void OnEditButton(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			if (GameConfig.IsEditorMode())
			{
				nick_name.Text = (input_str = "Test");
				return;
			}
			nick_name.Text = (input_str = string.Empty);
			is_input_name = true;
			keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.ASCIICapable);
		}
	}

	private void OnMsgOkButton()
	{
		nick_name.Text = (input_str = string.Empty);
		is_input_name = true;
		keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.ASCIICapable);
	}
}
