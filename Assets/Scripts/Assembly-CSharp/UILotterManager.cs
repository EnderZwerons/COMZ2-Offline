using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILotterManager : MonoBehaviour
{
	public List<UILotterSeat> lotter_seat_set = new List<UILotterSeat>();

	protected int cur_seat_index;

	protected int tar_seat_index;

	protected float seat_change_interval = 0.05f;

	protected float cur_change_time;

	protected bool is_lotterying;

	protected int lottery_count;

	protected int lottery_round_count = 2;

	private string weapon_combine = string.Empty;

	private void Start()
	{
		for (int i = 0; i < lotter_seat_set.Count; i++)
		{
			lotter_seat_set[i].SetLightState(false);
		}
		InitLotterSeat();
	}

	private void Update()
	{
		if (is_lotterying)
		{
			cur_change_time += Time.deltaTime;
			if (cur_change_time > seat_change_interval)
			{
				UILotteryController.Instance.SceneAudio.PlayAudio("UI_Lucky01");
				cur_change_time = 0f;
				lottery_count++;
				cur_seat_index++;
				ResetSeatlightState();
				CheckLotteryFinished();
			}
		}
	}

	private void InitLotterSeat()
	{
		for (int i = 0; i < GameData.Instance.lottery_seat_state.Count; i++)
		{
			string text = GameData.Instance.lottery_seat_state[i];
			if (text != "null")
			{
				lotter_seat_set[i].SetLotteryAward(GameConfig.Instance.Lottery_AwardItem_Set[text]);
				int award_level = GameConfig.Instance.Lottery_AwardItem_Set[text].award_level;
				lotter_seat_set[i].award_level = award_level;
				lotter_seat_set[i].seat_lottery_weight = GameConfig.Instance.Lottery_Seat_Weight_Set[award_level];
			}
		}
	}

	public void StartLottery()
	{
		lottery_count = 0;
		cur_change_time = 0f;
		cur_seat_index = UnityEngine.Random.Range(0, lotter_seat_set.Count);
		int num = 0;
		foreach (UILotterSeat item in lotter_seat_set)
		{
			num += item.seat_lottery_weight;
		}
		int num2 = UnityEngine.Random.Range(0, num);
		Debug.Log("weight sum:" + num + " random_val:" + num2);
		int num3 = 0;
		for (int i = 0; i < lotter_seat_set.Count; i++)
		{
			num3 += lotter_seat_set[i].seat_lottery_weight;
			if (num2 < num3)
			{
				tar_seat_index = i;
				Debug.Log("tar_seat_index:" + tar_seat_index);
				break;
			}
		}
		ResetSeatlightState();
		is_lotterying = true;
		Debug.Log("StartLottery cur index:" + cur_seat_index + ", tar index:" + tar_seat_index);
		UILotteryController.Instance.EnableBlock(true);
		GameData.Instance.lottery_count++;
	}

	public void ResetSeatlightState()
	{
		cur_seat_index %= lotter_seat_set.Count;
		for (int i = 0; i < lotter_seat_set.Count; i++)
		{
			if (cur_seat_index == i)
			{
				lotter_seat_set[i].SetLightState(true);
			}
			else
			{
				lotter_seat_set[i].SetLightState(false);
			}
		}
	}

	public void CheckLotteryFinished()
	{
		if (lottery_count >= lotter_seat_set.Count * lottery_round_count && tar_seat_index == cur_seat_index)
		{
			is_lotterying = false;
			OnLotteryAward();
		}
	}

	public List<T> RandomSortList<T>(List<T> ListT)
	{
		System.Random random = new System.Random();
		List<T> list = new List<T>();
		foreach (T item in ListT)
		{
			list.Insert(random.Next(list.Count + 1), item);
		}
		return list;
	}

	public void ResetSeatLevel(bool is_crystal)
	{
		int num = ((!is_crystal) ? GameConfig.Instance.lottery_free_percent : GameConfig.Instance.lottery_crystal_percent);
		List<UILotterSeat> list = RandomSortList(lotter_seat_set);
		int num2 = 0;
		foreach (int key in GameConfig.Instance.Lottery_Seat_Count_Set.Keys)
		{
			for (int num3 = GameConfig.Instance.Lottery_Seat_Count_Set[key]; num3 > 0; num3--)
			{
				list[num2].award_level = key;
				list[num2].seat_lottery_weight = GameConfig.Instance.Lottery_Seat_Weight_Set[key];
				num2++;
			}
		}
		if (UnityEngine.Random.Range(0, 100) < num)
		{
			foreach (UILotterSeat item in lotter_seat_set)
			{
				if (item.award_level == 2)
				{
					item.award_level = 7;
					item.seat_lottery_weight = GameConfig.Instance.Lottery_Seat_Weight_Set[7];
					break;
				}
			}
		}
		List<GameAwardItem> list2 = null;
		Dictionary<int, List<GameAwardItem>> dictionary = new Dictionary<int, List<GameAwardItem>>();
		for (int i = 1; i < 8; i++)
		{
			list2 = GameConfig.Instance.GetLotteryAwardWithLevel(i);
			dictionary.Add(i, list2);
		}
		num2 = 0;
		foreach (UILotterSeat item2 in lotter_seat_set)
		{
			list2 = dictionary[item2.award_level];
			item2.SetLotteryAward(list2[UnityEngine.Random.Range(0, list2.Count)]);
			dictionary[item2.award_level].Remove(item2.lottery_award);
			GameData.Instance.lottery_seat_state[num2] = item2.lottery_award.award_name;
			num2++;
		}
		GameData.Instance.lottery_reset_count++;
		GameData.Instance.SaveData();
		GameData.Instance.SaveDailyData();
	}

	public void OnLotteryAward()
	{
		UILotteryController.Instance.SceneAudio.PlayAudio("UI_craft");
		Debug.Log("Lottery finish index:" + tar_seat_index);
		Debug.Log("Lottery Award:" + lotter_seat_set[tar_seat_index].lottery_award.ToString());
		int num = 0;
		int num2 = 0;
		bool flag = false;
		bool show_bk = false;
		bool flag2 = false;
		GameAwardItem lottery_award = lotter_seat_set[tar_seat_index].lottery_award;
		Hashtable hashtable = new Hashtable();
		hashtable.Add("AwardName", lottery_award.award_name);
		GameData.Instance.UploadStatistics("Lottery_Award", hashtable);
		if (lottery_award.award_type == GameAwardItem.AwardType.Weapon)
		{
			if (GameData.Instance.WeaponData_Set[lottery_award.award_name].exist_state == WeaponExistState.Owned)
			{
				flag = true;
			}
			else
			{
				GameData.Instance.WeaponData_Set[lottery_award.award_name].LotteryReward(true);
			}
			num = 1;
			num2 = GameConfig.Instance.WeaponConfig_Set[lottery_award.award_name].sell_price.GetIntVal();
		}
		else if (lottery_award.award_type == GameAwardItem.AwardType.WeaponFragment)
		{
			WeaponFragmentProbsCfg weaponFragmentProbsCfg = (WeaponFragmentProbsCfg)GameConfig.Instance.ProbsConfig_Set[lottery_award.award_name];
			if (GameData.Instance.WeaponData_Set[weaponFragmentProbsCfg.weapon_name].exist_state == WeaponExistState.Unlocked || GameData.Instance.WeaponData_Set[weaponFragmentProbsCfg.weapon_name].exist_state == WeaponExistState.Owned)
			{
				flag = true;
			}
			else if (GameData.Instance.WeaponData_Set[weaponFragmentProbsCfg.weapon_name].exist_state == WeaponExistState.Locked)
			{
				if (GameData.Instance.WeaponFragmentProbs_Set.ContainsKey(lottery_award.award_name))
				{
					if (GameData.Instance.WeaponFragmentProbs_Set[lottery_award.award_name].count >= 1)
					{
						flag = true;
					}
					else
					{
						GameData.Instance.WeaponFragmentProbs_Set[lottery_award.award_name].count = 1;
					}
				}
				else
				{
					GameProb gameProb = new GameProb();
					gameProb.prob_cfg = GameConfig.Instance.ProbsConfig_Set[lottery_award.award_name];
					gameProb.count = 1;
					GameData.Instance.WeaponFragmentProbs_Set.Add(lottery_award.award_name, gameProb);
				}
				if (!flag)
				{
					WeaponFragmentProbsCfg weaponFragmentProbsCfg2 = (WeaponFragmentProbsCfg)GameConfig.Instance.ProbsConfig_Set[lottery_award.award_name];
					if (GameData.Instance.CheckFragmentProbCombine(weaponFragmentProbsCfg2.weapon_name) && GameData.Instance.WeaponData_Set[weaponFragmentProbsCfg2.weapon_name].Unlock())
					{
						flag2 = true;
						weapon_combine = weaponFragmentProbsCfg2.weapon_name;
						Debug.Log("weapon:" + weaponFragmentProbsCfg2.weapon_name + " is unlocked, it enable combie.");
					}
				}
			}
			show_bk = true;
			num = 1;
			num2 = ((WeaponFragmentProbsCfg)GameConfig.Instance.ProbsConfig_Set[lottery_award.award_name]).sell_price.GetIntVal();
		}
		else
		{
			num = lottery_award.award_count;
			if (lottery_award.award_type == GameAwardItem.AwardType.Cash)
			{
				GameData.Instance.total_cash += num;
			}
			else if (lottery_award.award_type == GameAwardItem.AwardType.Crystal)
			{
				GameData.Instance.total_crystal += num;
			}
			else if (lottery_award.award_type == GameAwardItem.AwardType.Voucher)
			{
				GameData.Instance.total_voucher += num;
			}
		}
		if (flag)
		{
			AwardChangePanel.ShowAwardChangePanel(UILotteryController.Instance.TUIControls.gameObject, OnAwardOk, lottery_award.award_name, "Cash_s", num2, show_bk);
			GameData.Instance.total_cash += num2;
		}
		else if (flag2)
		{
			AwardGetPanel.ShowAwardGetPanel(UILotteryController.Instance.TUIControls.gameObject, OnFragmentCombine, lottery_award.award_name, num, show_bk);
		}
		else
		{
			AwardGetPanel.ShowAwardGetPanel(UILotteryController.Instance.TUIControls.gameObject, OnAwardOk, lottery_award.award_name, num, show_bk);
		}
		GameData.Instance.SaveData();
		GameData.Instance.SaveDailyData();
		UILotteryController.Instance.EnableBlock(false);
		UISceneController.Instance.MoneyController.UpdateInfo();
		UILotteryController.Instance.UpdateLotteryBar();
	}

	private void OnFragmentCombine()
	{
		AwardGetPanel.ShowAwardGetPanel(UILotteryController.Instance.TUIControls.gameObject, OnAwardOk, weapon_combine, 1);
	}

	private void OnAwardOk()
	{
		if (UILotteryController.Instance.lotter_count_award)
		{
			AwardGetPanel.ShowAwardGetPanel(UILotteryController.Instance.TUIControls.gameObject, null, "tCrystal_Cent499", GameConfig.Instance.lottery_count_award.GetIntVal());
			GameData.Instance.total_crystal += GameConfig.Instance.lottery_count_award.GetIntVal();
			UILotteryController.Instance.lotter_count_award = false;
			UISceneController.Instance.MoneyController.UpdateInfo();
		}
	}
}
