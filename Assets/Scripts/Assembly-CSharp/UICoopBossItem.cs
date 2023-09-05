using UnityEngine;

public class UICoopBossItem : MonoBehaviour
{
	public TUIMeshSpriteSliced ui_board;

	public TUIMeshSprite ui_bk;

	public TUILabel ui_name;

	public CoopBossInfo boss_info;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetBossName(string name)
	{
		ui_name.Text = name;
	}

	public void SetItemChoiced(bool state)
	{
		if (state)
		{
			ui_board.texture = "Dikuang_dianji";
			ui_bk.texture = "touxiang_ditu_dianji";
		}
		else
		{
			ui_board.texture = "Dikuang";
			ui_bk.texture = "touxiang_ditu";
		}
	}
}
