using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Boomlagoon.JSON;
using CoMZ2;
using LitJson;
using UnityEngine;

public class GameData : MonoBehaviour
{
	public enum DailyMissionStatus
	{
		Disenable,
		Free,
		CrystalEnable,
		CrystalDisenable
	}

	public enum GamePlayType
	{
		Normal,
		Coop
	}

	public enum GameSceneState
	{
		None,
		Shop,
		Gaming
	}

	private static GameData instance;

	public DateTime last_checked_date_now;

	public DateTime next_cd_date;

	public string cur_save_date = string.Empty;

	public string save_date = string.Empty;

	public int daily_mission_count;

	public int lottery_reset_count;

	public int lottery_count;

	public bool daily_mode_enable;

	public bool is_crazy_daily;

	public GameDataInt total_cash = new GameDataInt(0);

	public GameDataInt total_crystal = new GameDataInt(0);

	public GameDataInt total_voucher = new GameDataInt(0);

	public int tapjoyPoints;

	public int day_level;

	public AvatarType cur_avatar = AvatarType.None;

	public Dictionary<string, WeaponData> WeaponData_Set = new Dictionary<string, WeaponData>();

	public Dictionary<AvatarType, AvatarData> AvatarData_Set = new Dictionary<AvatarType, AvatarData>();

	public Dictionary<string, GameProb> GameStoryProbs_Set = new Dictionary<string, GameProb>();

	public Dictionary<string, GameProb> WeaponFragmentProbs_Set = new Dictionary<string, GameProb>();

	public Dictionary<string, GameProb> WeaponIntensifierProbs_Set = new Dictionary<string, GameProb>();

	public Dictionary<string, int> Enemy_Loading_Set = new Dictionary<string, int>();

	public Dictionary<string, string> Iap_failed_info = new Dictionary<string, string>();

	public int Iap_failed_info_count;

	public GameSceneState scene_state;

	public string loading_to_scene = "test_new1";

	public QuestInfo cur_quest_info;

	public Action reset_nist_time_error;

	public Action reset_nist_time_finish;

	public float sensitivity_ratio = 0.4f;

	public bool is_enter_tutorial = true;

	public bool show_ui_tutorial = true;

	public bool show_ui_tutorial_weapon = true;

	public bool is_daily_cd_crystal;

	protected string user_id = string.Empty;

	public string NickName = string.Empty;

	public int FightStrength;

	protected string game_version = "1.0";

	public int enter_shop_count;

	public string timeserver_url = "http://72.167.165.221:7600/gameapi/GameCommonNo.do?action=groovy&json=%7B%22cmd%22:%22GetServerTime%22%7D";

	public string statistics_url = "http://208.109.176.89:8081/gameapi/turboPlatform2.do?action=logAllInfo&json=";

	public string iap_check_url = "http://192.225.224.97:7600/gameapi/GameCommon.do?action=groovy&json=";

	public string redeem_get_url = "http://184.168.72.188:9218/gameapi/comzb.do?action=comzb/GetGiftPackage&json=";

	public string redeem_accept_url = "http://184.168.72.188:9218/gameapi/comzb.do?action=comzb/AcceptGiftPackage&json=";

	public float redeem_change_ratio = 0.2f;

	protected DateTime finally_save_date;

	public bool TRINITI_IAP_CEHCK;

	public List<UnlockInGame> UnlockList = new List<UnlockInGame>();

	public Dictionary<string, SkillData> Skill_Avatar_Set = new Dictionary<string, SkillData>();

	public bool showUpdatePoster;

	public GamePlayType cur_game_type;

	public CoopBossType cur_coop_boss = CoopBossType.E_NONE;

	public List<string> lottery_seat_state = new List<string>();

	public static GameData Instance
	{
		get
		{
			return instance;
		}
	}

	public string UserId
	{
		get
		{
			return user_id;
		}
	}

	private void Awake()
	{
		if (GameObject.Find("_AndroidPlatform") == null)
		{
			GameObject gameObject = new GameObject("_AndroidPlatform");
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			DevicePlugin.InitAndroidPlatform();
			gameObject.AddComponent<TrinitiAdAndroidPlugin>();
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		finally_save_date = GetCurDateTime();
		TRINITI_IAP_CEHCK = true;
	}

	private void Init()
	{
		daily_mission_count = 0;
		day_level = 1;
		total_cash = new GameDataInt(0);
		total_crystal = new GameDataInt(0);
		total_voucher = new GameDataInt(0);
		cur_avatar = AvatarType.Human;
		foreach (AvatarConfig value in GameConfig.Instance.AvatarConfig_Set.Values)
		{
			AvatarData avatarData = new AvatarData();
			avatarData.show_name = value.show_name;
			avatarData.avatar_name = value.avatar_name;
			avatarData.avatar_type = value.avatar_type;
			avatarData.config = value;
			avatarData.level = 1;
			avatarData.cur_exp = new GameDataInt(0);
			avatarData.exist_state = value.exist_state;
			avatarData.primary_equipment = "MP5";
			for (int i = 0; i < 2; i++)
			{
				if (i == 0)
				{
					avatarData.skill_list.Add(value.first_skill);
				}
				else
				{
					avatarData.skill_list.Add("null");
				}
			}
			AvatarData_Set[avatarData.avatar_type] = avatarData;
		}
		foreach (SkillConfig value2 in GameConfig.Instance.Skill_Avatar_Set.Values)
		{
			SkillData skillData = new SkillData();
			skillData.skill_name = value2.skill_name;
			skillData.level = 1;
			skillData.exist_state = value2.exist_state;
			skillData.config = value2;
			skillData.ResetData();
			Skill_Avatar_Set[skillData.skill_name] = skillData;
		}
		foreach (WeaponConfig value3 in GameConfig.Instance.WeaponConfig_Set.Values)
		{
			WeaponData weaponData = new WeaponData();
			weaponData.weapon_name = value3.weapon_name;
			weaponData.weapon_type = value3.wType;
			weaponData.is_secondary = value3.is_secondary;
			weaponData.owner = value3.owner;
			weaponData.config = value3;
			weaponData.total_bullet_count = value3.initBullet;
			weaponData.level = 1;
			weaponData.exist_state = value3.exist_state;
			WeaponData_Set[weaponData.weapon_name] = weaponData;
		}
		foreach (string value4 in GameConfig.Instance.Enemy_Loading_Set.Values)
		{
			Enemy_Loading_Set[value4] = 0;
		}
		for (int j = 0; j < 10; j++)
		{
			lottery_seat_state.Add("null");
		}
		cur_quest_info = new QuestInfo();
		cur_quest_info.mission_type = MissionType.Cleaner;
		is_enter_tutorial = true;
		game_version = "2.1.2";
		user_id = DevicePlugin.GetUUID();
		if (!LoadData())
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer && Screen.height >= 700)
			{
				sensitivity_ratio = 0.6f;
			}
			SaveData();
			UploadStatistics("FirstTime", new Hashtable());
			showUpdatePoster = true;
		}
		if (game_version != "2.1.2")
		{
			game_version = "2.1.2";
			OnGameDataVersionDifferent();
			showUpdatePoster = true;
		}
		foreach (WeaponData value5 in WeaponData_Set.Values)
		{
			if (value5.exist_state == WeaponExistState.Locked && value5.config.unlock_day >= 0 && value5.config.unlock_day < day_level)
			{
				value5.Unlock();
			}
			value5.ResetData();
		}
		foreach (AvatarData value6 in AvatarData_Set.Values)
		{
			if (value6.exist_state == AvatarExistState.Locked && value6.config.unlockDay < day_level)
			{
				value6.Unlock();
			}
			value6.ResetData();
		}
	}

	public static void CheckGameData()
	{
		if (!GameObject.Find("GameData"))
		{
			GameObject gameObject = Instantiate(Resources.Load<GameObject>("GameData"));
			gameObject.name = "GameData";
			Instance.Init();
			TapJoyScript.CreateTapjoyObj();
		}
	}

	public void SaveData()
	{
		Configure configure = new Configure();
		configure.AddSection("Save", string.Empty, string.Empty);
		configure.AddValueSingle("Save", "IAPResend_android", Iap_Resend.Get_IAP_android_list(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "Cash", total_cash.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "Crystal", total_crystal.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "Voucher", total_voucher.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "TapjoyPoints", tapjoyPoints.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "DayLevel", day_level.ToString(), string.Empty, string.Empty);
		int num = (int)cur_avatar;
		configure.AddValueSingle("Save", "AvatarType", num.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "SensitivityRatio", sensitivity_ratio.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "Version", game_version, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "EnterShopCount", enter_shop_count.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "TimeserverUrl", timeserver_url, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "StatisticsUrl", statistics_url, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "IapUrl", iap_check_url, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "RedeemGetUrl", redeem_get_url, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "RedeemAcceptUrl", redeem_accept_url, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "RedeemAwardRatio", redeem_change_ratio.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "NickName", NickName, string.Empty, string.Empty);
		if (is_enter_tutorial)
		{
			configure.AddValueSingle("Save", "EnterTutorial", "1", string.Empty, string.Empty);
		}
		else
		{
			configure.AddValueSingle("Save", "EnterTutorial", "0", string.Empty, string.Empty);
		}
		if (show_ui_tutorial)
		{
			configure.AddValueSingle("Save", "ShowUITutorial", "1", string.Empty, string.Empty);
		}
		else
		{
			configure.AddValueSingle("Save", "ShowUITutorial", "0", string.Empty, string.Empty);
		}
		if (show_ui_tutorial_weapon)
		{
			configure.AddValueSingle("Save", "ShowUITutorialWeapon", "1", string.Empty, string.Empty);
		}
		else
		{
			configure.AddValueSingle("Save", "ShowUITutorialWeapon", "0", string.Empty, string.Empty);
		}
		if (TRINITI_IAP_CEHCK)
		{
			configure.AddValueSingle("Save", "IapCheck", "1", string.Empty, string.Empty);
		}
		else
		{
			configure.AddValueSingle("Save", "IapCheck", "0", string.Empty, string.Empty);
		}
		finally_save_date = GetCurDateTime();
		configure.AddValueSingle("Save", "SaveDate", finally_save_date.ToString(), string.Empty, string.Empty);
		ArrayList arrayList = new ArrayList();
		foreach (WeaponData value in WeaponData_Set.Values)
		{
			StringLine stringLine = new StringLine();
			stringLine.AddString(value.weapon_name);
			int weapon_type = (int)value.weapon_type;
			stringLine.AddString(weapon_type.ToString());
			stringLine.AddString(value.level.ToString());
			stringLine.AddString(value.total_bullet_count.ToString());
			int exist_state = (int)value.exist_state;
			stringLine.AddString(exist_state.ToString());
			stringLine.AddString(value.damage_level.ToString());
			stringLine.AddString(value.frequency_level.ToString());
			stringLine.AddString(value.clip_level.ToString());
			stringLine.AddString(value.range_level.ToString());
			stringLine.AddString(value.stretch_level.ToString());
			arrayList.Add(stringLine.content);
		}
		configure.AddValueArray2("Save", "WeaponsData", arrayList, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "WeaponsDataCount", arrayList.Count.ToString(), string.Empty, string.Empty);
		ArrayList arrayList2 = new ArrayList();
		foreach (AvatarData value2 in AvatarData_Set.Values)
		{
			StringLine stringLine2 = new StringLine();
			stringLine2.AddString(value2.avatar_name);
			int avatar_type = (int)value2.avatar_type;
			stringLine2.AddString(avatar_type.ToString());
			stringLine2.AddString("null");
			stringLine2.AddString(value2.level.ToString());
			stringLine2.AddString(value2.armor_level.ToString());
			stringLine2.AddString(value2.cur_exp.ToString());
			int exist_state2 = (int)value2.exist_state;
			stringLine2.AddString(exist_state2.ToString());
			stringLine2.AddString(value2.primary_equipment);
			stringLine2.AddString(value2.hp_level.ToString());
			stringLine2.AddString(value2.damage_level.ToString());
			foreach (string item in value2.skill_list)
			{
				stringLine2.AddString(item);
			}
			arrayList2.Add(stringLine2.content);
		}
		configure.AddValueArray2("Save", "AvatarsData", arrayList2, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "AvatarsDataCount", arrayList2.Count.ToString(), string.Empty, string.Empty);
		ArrayList arrayList3 = new ArrayList();
		foreach (SkillData value3 in Skill_Avatar_Set.Values)
		{
			StringLine stringLine3 = new StringLine();
			stringLine3.AddString(value3.skill_name);
			stringLine3.AddString(value3.level.ToString());
			int exist_state3 = (int)value3.exist_state;
			stringLine3.AddString(exist_state3.ToString());
			arrayList3.Add(stringLine3.content);
		}
		configure.AddValueArray2("Save", "SkillAvatarData", arrayList3, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "SkillAvatarDataCount", arrayList3.Count.ToString(), string.Empty, string.Empty);
		ArrayList arrayList4 = new ArrayList();
		foreach (string key in GameStoryProbs_Set.Keys)
		{
			StringLine stringLine4 = new StringLine();
			stringLine4.AddString(key);
			stringLine4.AddString(GameStoryProbs_Set[key].count.ToString());
			arrayList4.Add(stringLine4.content);
		}
		configure.AddValueArray2("Save", "StoryProbData", arrayList4, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "StoryProbDataCount", arrayList4.Count.ToString(), string.Empty, string.Empty);
		ArrayList arrayList5 = new ArrayList();
		foreach (string key2 in WeaponFragmentProbs_Set.Keys)
		{
			StringLine stringLine5 = new StringLine();
			stringLine5.AddString(key2);
			stringLine5.AddString(WeaponFragmentProbs_Set[key2].count.ToString());
			arrayList5.Add(stringLine5.content);
		}
		configure.AddValueArray2("Save", "FragmentProbData", arrayList5, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "FragmentProbDataCount", arrayList5.Count.ToString(), string.Empty, string.Empty);
		ArrayList arrayList6 = new ArrayList();
		foreach (string key3 in WeaponIntensifierProbs_Set.Keys)
		{
			StringLine stringLine6 = new StringLine();
			stringLine6.AddString(key3);
			stringLine6.AddString(WeaponIntensifierProbs_Set[key3].count.ToString());
			arrayList6.Add(stringLine6.content);
		}
		configure.AddValueArray2("Save", "IntensifierProbData", arrayList6, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "IntensifierProbDataCount", arrayList6.Count.ToString(), string.Empty, string.Empty);
		ArrayList arrayList7 = new ArrayList();
		foreach (string key4 in Enemy_Loading_Set.Keys)
		{
			StringLine stringLine7 = new StringLine();
			stringLine7.AddString(key4);
			stringLine7.AddString(Enemy_Loading_Set[key4].ToString());
			arrayList7.Add(stringLine7.content);
		}
		configure.AddValueArray2("Save", "EnemyLoadData", arrayList7, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "EnemyLoadDataCount", arrayList7.Count.ToString(), string.Empty, string.Empty);
		StringLine stringLine8 = new StringLine();
		foreach (string item2 in lottery_seat_state)
		{
			stringLine8.AddString(item2);
		}
		configure.AddValueArray("Save", "LotterySeatData", stringLine8.content, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "LotterySeatCount", lottery_seat_state.Count.ToString(), string.Empty, string.Empty);
		ArrayList arrayList8 = new ArrayList();
		foreach (string key5 in GameConfig.Instance.Config_Version_Set.Keys)
		{
			StringLine stringLine9 = new StringLine();
			stringLine9.AddString(key5);
			stringLine9.AddString(GameConfig.Instance.Config_Version_Set[key5]);
			arrayList8.Add(stringLine9.content);
		}
		configure.AddValueArray2("Save", "ConfigVersion", arrayList8, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "ConfigVersionCount", arrayList8.Count.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "IapFailedCount", Iap_failed_info.Count.ToString(), string.Empty, string.Empty);
		if (Iap_failed_info.Count > 0)
		{
			int num2 = 1;
			foreach (string key6 in Iap_failed_info.Keys)
			{
				configure.AddValueSingle("Save", "IapFailedInfoTid" + num2, key6, string.Empty, string.Empty);
				string data = Iap_failed_info[key6];
				data = DataEncipher(data);
				Utils.FileWriteString(Utils.SavePath() + MD5Sample.GetMd5String("IapFailedInfoReceipt" + num2) + ".bytes", data);
				num2++;
			}
		}
		string data2 = configure.Save();
		data2 = DataEncipher(data2);
		Utils.FileWriteString(Utils.SavePath() + MD5Sample.GetMd5String("CoMZ2") + ".bytes", data2);
	}

	public void SaveDailyData()
	{
		Configure configure = new Configure();
		configure.AddSection("Save", string.Empty, string.Empty);
		configure.AddValueSingle("Save", "SavDate", cur_save_date, string.Empty, string.Empty);
		configure.AddValueSingle("Save", "DailyMissionCount", daily_mission_count.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "LotteryResetCount", lottery_reset_count.ToString(), string.Empty, string.Empty);
		configure.AddValueSingle("Save", "LotteryCount", lottery_count.ToString(), string.Empty, string.Empty);
		string data = configure.Save();
		data = DataEncipher(data);
		Utils.FileWriteString(Utils.SavePath() + MD5Sample.GetMd5String("CoMZ2Daily") + ".bytes", data);
	}

	public bool LoadData()
	{
		string content = string.Empty;
		if (Utils.FileReadString(Utils.SavePath() + MD5Sample.GetMd5String("CoMZ2") + ".bytes", ref content))
		{
			Configure configure = new Configure();
			content = DataDecrypt(content);
			configure.Load(content);
			total_cash = new GameDataInt(int.Parse(configure.GetSingle("Save", "Cash")));
			total_crystal = new GameDataInt(int.Parse(configure.GetSingle("Save", "Crystal")));
			total_voucher = new GameDataInt(int.Parse(configure.GetSingle("Save", "Voucher")));
			if (configure.GetSingle("Save", "TapjoyPoints") != null)
			{
				tapjoyPoints = int.Parse(configure.GetSingle("Save", "TapjoyPoints"));
			}
			day_level = int.Parse(configure.GetSingle("Save", "DayLevel"));
			cur_avatar = (AvatarType)int.Parse(configure.GetSingle("Save", "AvatarType"));
			sensitivity_ratio = float.Parse(configure.GetSingle("Save", "SensitivityRatio"));
			int num = int.Parse(configure.GetSingle("Save", "EnterTutorial"));
			is_enter_tutorial = ((num != 0) ? true : false);
			game_version = configure.GetSingle("Save", "Version");
			num = int.Parse(configure.GetSingle("Save", "ShowUITutorial"));
			show_ui_tutorial = ((num != 0) ? true : false);
			num = int.Parse(configure.GetSingle("Save", "ShowUITutorialWeapon"));
			show_ui_tutorial_weapon = ((num != 0) ? true : false);
			enter_shop_count = int.Parse(configure.GetSingle("Save", "EnterShopCount"));
			timeserver_url = configure.GetSingle("Save", "TimeserverUrl");
			statistics_url = configure.GetSingle("Save", "StatisticsUrl");
			iap_check_url = "http://192.225.224.97:7600/gameapi/GameCommon.do?action=groovy&json=";
			int num2 = int.Parse(configure.GetSingle("Save", "IapCheck"));
			TRINITI_IAP_CEHCK = ((num2 != 0) ? true : false);
			if (configure.GetSingle("Save", "RedeemGetUrl") != null)
			{
				redeem_get_url = configure.GetSingle("Save", "RedeemGetUrl");
			}
			if (configure.GetSingle("Save", "RedeemAcceptUrl") != null)
			{
				redeem_accept_url = configure.GetSingle("Save", "RedeemAcceptUrl");
			}
			if (configure.GetSingle("Save", "RedeemAwardRatio") != null)
			{
				redeem_change_ratio = float.Parse(configure.GetSingle("Save", "RedeemAwardRatio"));
			}
			if (configure.GetSingle("Save", "NickName") != null)
			{
				NickName = configure.GetSingle("Save", "NickName");
			}
			if (configure.GetSingle("Save", "IAPResend_android") != "false")
			{
				string single = configure.GetSingle("Save", "IAPResend_android");
				if (single != null)
				{
					string[] array = single.Split('|');
					string product_Id = array[0];
					string tid = array[1];
					string receipt = array[2];
					string action = array[3];
					string signature = array[4];
					IapCenter.Instance.SendIAPVerifyRequest_for_Android(product_Id, tid, receipt, action, signature);
				}
			}
			int num3 = 0;
			num3 = int.Parse(configure.GetSingle("Save", "WeaponsDataCount"));
			for (int i = 0; i < num3; i++)
			{
				string array2 = configure.GetArray2("Save", "WeaponsData", i, 0);
				WeaponData weaponData = null;
				if (WeaponData_Set.ContainsKey(array2))
				{
					weaponData = WeaponData_Set[array2];
					weaponData.level = int.Parse(configure.GetArray2("Save", "WeaponsData", i, 2));
					weaponData.total_bullet_count = int.Parse(configure.GetArray2("Save", "WeaponsData", i, 3));
					weaponData.exist_state = (WeaponExistState)int.Parse(configure.GetArray2("Save", "WeaponsData", i, 4));
					if (configure.GetArray2("Save", "WeaponsData", i, 5) != null)
					{
						weaponData.damage_level = int.Parse(configure.GetArray2("Save", "WeaponsData", i, 5));
					}
					else
					{
						weaponData.damage_level = weaponData.level;
					}
					if (configure.GetArray2("Save", "WeaponsData", i, 6) != null)
					{
						weaponData.frequency_level = int.Parse(configure.GetArray2("Save", "WeaponsData", i, 6));
					}
					else
					{
						weaponData.frequency_level = weaponData.level;
					}
					if (configure.GetArray2("Save", "WeaponsData", i, 7) != null)
					{
						weaponData.clip_level = int.Parse(configure.GetArray2("Save", "WeaponsData", i, 7));
					}
					else
					{
						weaponData.clip_level = weaponData.level;
					}
					if (configure.GetArray2("Save", "WeaponsData", i, 8) != null)
					{
						weaponData.range_level = int.Parse(configure.GetArray2("Save", "WeaponsData", i, 8));
					}
					else
					{
						weaponData.range_level = weaponData.level;
					}
					if (configure.GetArray2("Save", "WeaponsData", i, 9) != null)
					{
						weaponData.stretch_level = int.Parse(configure.GetArray2("Save", "WeaponsData", i, 9));
					}
					else
					{
						weaponData.stretch_level = weaponData.level;
					}
				}
			}
			num3 = int.Parse(configure.GetSingle("Save", "AvatarsDataCount"));
			for (int j = 0; j < num3; j++)
			{
				AvatarType key = (AvatarType)int.Parse(configure.GetArray2("Save", "AvatarsData", j, 1));
				AvatarData avatarData = null;
				if (!AvatarData_Set.ContainsKey(key))
				{
					continue;
				}
				avatarData = AvatarData_Set[key];
				avatarData.level = int.Parse(configure.GetArray2("Save", "AvatarsData", j, 3));
				avatarData.armor_level = int.Parse(configure.GetArray2("Save", "AvatarsData", j, 4));
				avatarData.cur_exp = new GameDataInt(int.Parse(configure.GetArray2("Save", "AvatarsData", j, 5)));
				avatarData.exist_state = (AvatarExistState)int.Parse(configure.GetArray2("Save", "AvatarsData", j, 6));
				avatarData.primary_equipment = configure.GetArray2("Save", "AvatarsData", j, 7);
				if (configure.GetArray2("Save", "AvatarsData", j, 8) != null)
				{
					avatarData.hp_level = int.Parse(configure.GetArray2("Save", "AvatarsData", j, 8));
				}
				else
				{
					avatarData.hp_level = avatarData.level;
				}
				if (configure.GetArray2("Save", "AvatarsData", j, 9) != null)
				{
					avatarData.damage_level = int.Parse(configure.GetArray2("Save", "AvatarsData", j, 9));
				}
				else
				{
					avatarData.damage_level = avatarData.level;
				}
				int num4 = 10;
				for (int k = 0; k < 2; k++)
				{
					if (configure.GetArray2("Save", "AvatarsData", j, num4) != null)
					{
						string array3 = configure.GetArray2("Save", "AvatarsData", j, num4);
						avatarData.skill_list[k] = array3;
						num4++;
					}
				}
			}
			if (configure.GetSingle("Save", "SkillAvatarDataCount") != null)
			{
				num3 = int.Parse(configure.GetSingle("Save", "SkillAvatarDataCount"));
				for (int l = 0; l < num3; l++)
				{
					string array4 = configure.GetArray2("Save", "SkillAvatarData", l, 0);
					if (Skill_Avatar_Set.ContainsKey(array4))
					{
						SkillData skillData = Skill_Avatar_Set[array4];
						skillData.level = int.Parse(configure.GetArray2("Save", "SkillAvatarData", l, 1));
						skillData.exist_state = (SkillExistState)int.Parse(configure.GetArray2("Save", "SkillAvatarData", l, 2));
						skillData.ResetData();
					}
				}
			}
			else
			{
				SkillData skillData2 = Skill_Avatar_Set["Whirlwind"];
				skillData2.level = WeaponData_Set["Chainsaw"].level;
				skillData2.ResetData();
				skillData2 = Skill_Avatar_Set["Enchant"];
				skillData2.level = WeaponData_Set["Medicine"].level;
				skillData2.ResetData();
				skillData2 = Skill_Avatar_Set["BaseballRobot"];
				skillData2.level = WeaponData_Set["Baseball"].level;
				skillData2.ResetData();
				skillData2 = Skill_Avatar_Set["Grenade"];
				skillData2.level = WeaponData_Set["Shield"].level;
				skillData2.ResetData();
				skillData2 = Skill_Avatar_Set["Scarecrow"];
				skillData2.level = WeaponData_Set["44Magnum"].level;
				skillData2.ResetData();
			}
			num3 = int.Parse(configure.GetSingle("Save", "StoryProbDataCount"));
			for (int m = 0; m < num3; m++)
			{
				string array5 = configure.GetArray2("Save", "StoryProbData", m, 0);
				int count = int.Parse(configure.GetArray2("Save", "StoryProbData", m, 1));
				GameProb gameProb = new GameProb();
				gameProb.prob_cfg = GameConfig.Instance.ProbsConfig_Set[array5];
				gameProb.count = count;
				GameStoryProbs_Set[array5] = gameProb;
			}
			num3 = int.Parse(configure.GetSingle("Save", "FragmentProbDataCount"));
			for (int n = 0; n < num3; n++)
			{
				string array6 = configure.GetArray2("Save", "FragmentProbData", n, 0);
				int count2 = int.Parse(configure.GetArray2("Save", "FragmentProbData", n, 1));
				if (GameConfig.Instance.ProbsConfig_Set.ContainsKey(array6))
				{
					GameProb gameProb2 = new GameProb();
					gameProb2.prob_cfg = GameConfig.Instance.ProbsConfig_Set[array6];
					gameProb2.count = count2;
					WeaponFragmentProbs_Set[array6] = gameProb2;
				}
			}
			num3 = int.Parse(configure.GetSingle("Save", "IntensifierProbDataCount"));
			for (int num5 = 0; num5 < num3; num5++)
			{
				string array7 = configure.GetArray2("Save", "IntensifierProbData", num5, 0);
				int count3 = int.Parse(configure.GetArray2("Save", "IntensifierProbData", num5, 1));
				GameProb gameProb3 = new GameProb();
				gameProb3.prob_cfg = GameConfig.Instance.ProbsConfig_Set[array7];
				gameProb3.count = count3;
				WeaponIntensifierProbs_Set[array7] = gameProb3;
			}
			num3 = int.Parse(configure.GetSingle("Save", "EnemyLoadDataCount"));
			for (int num6 = 0; num6 < num3; num6++)
			{
				string array8 = configure.GetArray2("Save", "EnemyLoadData", num6, 0);
				int value = int.Parse(configure.GetArray2("Save", "EnemyLoadData", num6, 1));
				Enemy_Loading_Set[array8] = value;
			}
			if (configure.GetSingle("Save", "LotterySeatCount") != null)
			{
				num3 = int.Parse(configure.GetSingle("Save", "LotterySeatCount"));
				for (int num7 = 0; num7 < num3; num7++)
				{
					string array9 = configure.GetArray("Save", "LotterySeatData", num7);
					lottery_seat_state[num7] = array9;
				}
			}
			num3 = int.Parse(configure.GetSingle("Save", "ConfigVersionCount"));
			for (int num8 = 0; num8 < num3; num8++)
			{
				string array10 = configure.GetArray2("Save", "ConfigVersion", num8, 0);
				string array11 = configure.GetArray2("Save", "ConfigVersion", num8, 1);
				GameConfig.Instance.Config_Version_Set[array10] = array11;
			}
			Iap_failed_info_count = int.Parse(configure.GetSingle("Save", "IapFailedCount"));
			if (Iap_failed_info_count > 0)
			{
				for (int num9 = 1; num9 < Iap_failed_info_count + 1; num9++)
				{
					string single2 = configure.GetSingle("Save", "IapFailedInfoTid" + num9);
					string content2 = string.Empty;
					if (Utils.FileReadString(Utils.SavePath() + MD5Sample.GetMd5String("IapFailedInfoReceipt" + num9) + ".bytes", ref content2))
					{
						content2 = DataDecrypt(content2);
					}
					Iap_failed_info[single2] = content2;
				}
			}
			return true;
		}
		return false;
	}

	public bool LoadDailyData()
	{
		string content = string.Empty;
		if (Utils.FileReadString(Utils.SavePath() + MD5Sample.GetMd5String("CoMZ2Daily") + ".bytes", ref content))
		{
			Configure configure = new Configure();
			content = DataDecrypt(content);
			configure.Load(content);
			save_date = configure.GetSingle("Save", "SavDate");
			daily_mission_count = int.Parse(configure.GetSingle("Save", "DailyMissionCount"));
			if (configure.GetSingle("Save", "LotteryResetCount") != null)
			{
				lottery_reset_count = int.Parse(configure.GetSingle("Save", "LotteryResetCount"));
			}
			if (configure.GetSingle("Save", "LotteryCount") != null)
			{
				lottery_count = int.Parse(configure.GetSingle("Save", "LotteryCount"));
			}
			if (save_date != cur_save_date)
			{
				Debug.Log("Reset DailyMissionCount!");
				save_date = cur_save_date;
				daily_mission_count = 0;
				lottery_reset_count = 0;
				lottery_count = 0;
				is_daily_cd_crystal = false;
				SaveDailyData();
			}
			return true;
		}
		return false;
	}

	public List<WeaponData> GetOwnerWeapons()
	{
		List<WeaponData> list = new List<WeaponData>();
		foreach (WeaponData value in WeaponData_Set.Values)
		{
			if (value.exist_state == WeaponExistState.Owned)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<WeaponData> GetCouldIntensifierWeapons()
	{
		List<WeaponData> list = new List<WeaponData>();
		foreach (WeaponData value in WeaponData_Set.Values)
		{
			if (value.exist_state == WeaponExistState.Owned)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public void DropIntensifierRandom(List<WeaponData> WeaponSet)
	{
		int num = 0;
		foreach (WeaponData item in WeaponSet)
		{
			num += item.intensifier_drop_weight;
		}
		int num2 = UnityEngine.Random.Range(1, num);
		int num3 = 0;
		foreach (WeaponData item2 in WeaponSet)
		{
			num3 += item2.intensifier_drop_weight;
			if (num2 <= num3)
			{
				Debug.Log("drop intensifier, weapon name:" + item2.weapon_name);
				break;
			}
		}
	}

	public bool CheckStoryProbCombine(int level)
	{
		List<GameProb> list = new List<GameProb>();
		foreach (GameProb value in GameStoryProbs_Set.Values)
		{
			GameStoryProbsCfg gameStoryProbsCfg = value.prob_cfg as GameStoryProbsCfg;
			if (gameStoryProbsCfg.level == level)
			{
				list.Add(value);
			}
		}
		int num = 0;
		foreach (GameProb item in list)
		{
			GameStoryProbsCfg gameStoryProbsCfg2 = item.prob_cfg as GameStoryProbsCfg;
			num += gameStoryProbsCfg2.weight * item.count;
		}
		if (num >= 10)
		{
			foreach (GameProb item2 in list)
			{
				GameStoryProbs_Set.Remove(item2.prob_cfg.prob_name);
			}
			return true;
		}
		return false;
	}

	public List<GameProb> GetWeaponFragmentProbs(string weapon_name)
	{
		List<GameProb> list = new List<GameProb>();
		foreach (GameProb value in WeaponFragmentProbs_Set.Values)
		{
			WeaponFragmentProbsCfg weaponFragmentProbsCfg = value.prob_cfg as WeaponFragmentProbsCfg;
			if (weaponFragmentProbsCfg.weapon_name == weapon_name)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public int[] GetWeaponFragmentProbsCountOrder(string weapon_name)
	{
		if (WeaponData_Set[weapon_name].config.combination_count == 0)
		{
			return null;
		}
		List<GameProb> weaponFragmentProbs = GetWeaponFragmentProbs(weapon_name);
		int[] array = new int[WeaponData_Set[weapon_name].config.combination_count];
		for (int i = 0; i < weaponFragmentProbs.Count; i++)
		{
			bool flag = false;
			foreach (GameProb item in weaponFragmentProbs)
			{
				WeaponFragmentProbsCfg weaponFragmentProbsCfg = item.prob_cfg as WeaponFragmentProbsCfg;
				int num = (int)(weaponFragmentProbsCfg.type - 1);
				if (num == i)
				{
					array[i] = item.count;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				array[i] = 0;
			}
		}
		return array;
	}

	public bool CheckFragmentProbCombine(string weapon_name)
	{
		if (WeaponData_Set[weapon_name].exist_state != 0)
		{
			Debug.Log("Weapon:" + weapon_name + "can not combine,it is already combined.");
			return false;
		}
		int[] weaponFragmentProbsCountOrder = GetWeaponFragmentProbsCountOrder(weapon_name);
		if (weaponFragmentProbsCountOrder == null)
		{
			return false;
		}
		bool result = true;
		for (int i = 0; i < weaponFragmentProbsCountOrder.Length; i++)
		{
			if (weaponFragmentProbsCountOrder[i] == 0)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public bool WeaponCombine(string weapon_name)
	{
		if (!CheckFragmentProbCombine(weapon_name))
		{
			return false;
		}
		List<GameProb> weaponFragmentProbs = GetWeaponFragmentProbs(weapon_name);
		foreach (GameProb item in weaponFragmentProbs)
		{
			WeaponFragmentProbs_Set.Remove(item.prob_cfg.prob_name);
		}
		Instance.WeaponData_Set[weapon_name].exist_state = WeaponExistState.Owned;
		return true;
	}

	public List<WeaponData> GetPrimaryWeapons()
	{
		List<WeaponData> list = new List<WeaponData>();
		foreach (WeaponData value in WeaponData_Set.Values)
		{
			if (!value.is_secondary)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<WeaponData> GetSecondaryWeaponsFor(AvatarType avatar_type)
	{
		List<WeaponData> list = new List<WeaponData>();
		foreach (WeaponData value in WeaponData_Set.Values)
		{
			if (value.is_secondary && value.owner == avatar_type)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public void ResetCurNistTime()
	{
		Debug.Log("ResetCurNistTime...");
		StartCoroutine(ResetCurServerTime());
	}

	public IEnumerator ResetCurServerTime()
	{
    	DateTime currentDateTime = DateTime.Now;
    	last_checked_date_now = currentDateTime;
    	next_cd_date = currentDateTime.AddDays(1.0);
    	next_cd_date = new DateTime(next_cd_date.Year, next_cd_date.Month, next_cd_date.Day, 0, 0, 0);
    	Debug.Log("last_checked_date_now:" + last_checked_date_now);
    	Debug.Log("next_cd_date:" + next_cd_date);
    	Debug.Log(string.Concat("cd:", next_cd_date - last_checked_date_now, " totalSec:", (next_cd_date - last_checked_date_now).TotalSeconds));
    	string date = (cur_save_date = last_checked_date_now.Year.ToString() + last_checked_date_now.Month + last_checked_date_now.Day);
    	daily_mode_enable = true;
    	if (!LoadDailyData())
    	{
    	    SaveDailyData();
    	}
    	if (reset_nist_time_finish != null)
    	{
    	    reset_nist_time_finish();
    	}
		yield break;
	}

	public void SetMapMissionList(ref List<QuestInfo> mInfos)
	{
		int key;
		QuestInfo questInfo;
		if (instance.day_level > GameConfig.Instance.Side_Quest_Order.Count)
		{
			key = (instance.day_level - 1) % GameConfig.Instance.Side_Quest_Order_Spare.Count + 1;
			questInfo = GameConfig.Instance.Side_Quest_Order_Spare[key];
		}
		else
		{
			key = instance.day_level;
			questInfo = GameConfig.Instance.Side_Quest_Order[key];
		}
		questInfo.SetQuestComment();
		if (questInfo.mission_type == MissionType.MainMission)
		{
			questInfo = GameConfig.Instance.Main_Quest_Order[key];
		}
		mInfos.Add(questInfo);
		List<string> list = new List<string>();
		list.Add("Church");
		list.Add("Depot");
		list.Add("Junkyard");
		questInfo = new QuestInfo();
		questInfo.mission_type = MissionType.Cleaner;
		questInfo.mission_day_type = MissionDayType.Daily;
		questInfo.SetQuestComment();
		questInfo.scene_name = list[UnityEngine.Random.Range(0, list.Count)];
		mInfos.Add(questInfo);
		questInfo = new QuestInfo();
		questInfo.mission_type = MissionType.Coop;
		questInfo.mission_day_type = MissionDayType.None;
		questInfo.SetQuestComment();
		questInfo.scene_name = list[UnityEngine.Random.Range(0, list.Count)];
		mInfos.Add(questInfo);
	}

	public void MapSceneQuestInfoWrite(QuestInfo info)
	{
		Debug.Log(string.Concat("info:", info.mission_day_type, " ff:", info.mission_type));
		Instance.cur_quest_info = info;
		Instance.loading_to_scene = Instance.cur_quest_info.scene_name;
		if (Instance.cur_quest_info.mission_day_type == MissionDayType.Daily)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("tCrystalNum", GetDailyMissionPrice(is_crazy_daily));
			Instance.UploadStatistics("tCrystal_DaiLy", hashtable);
			total_crystal -= GetDailyMissionPrice(is_crazy_daily);
			if (total_crystal < 0)
			{
				total_crystal = new GameDataInt(0);
				Debug.LogWarning("Enter daily mission not enough crystal.");
			}
			Instance.daily_mission_count++;
			Instance.SaveDailyData();
		}
	}

	public List<WeaponFragmentList> GetWeaponFragmentProbsState(string weapon_name)
	{
		WeaponFragmentList weaponFragmentList = null;
		List<WeaponFragmentList> list = new List<WeaponFragmentList>();
		List<GameProb> weaponFragmentProbs = GetWeaponFragmentProbs(weapon_name);
		List<WeaponFragmentProbsCfg> weaponFragmentProb = GameConfig.Instance.GetWeaponFragmentProb(weapon_name);
		foreach (WeaponFragmentProbsCfg item in weaponFragmentProb)
		{
			weaponFragmentList = new WeaponFragmentList(item.prob_name, 0, item.image_name);
			foreach (GameProb item2 in weaponFragmentProbs)
			{
				WeaponFragmentProbsCfg weaponFragmentProbsCfg = item2.prob_cfg as WeaponFragmentProbsCfg;
				if (weaponFragmentProbsCfg != null && item.prob_name == weaponFragmentProbsCfg.prob_name)
				{
					weaponFragmentList.count += item2.count;
				}
			}
			list.Add(weaponFragmentList);
		}
		return list;
	}

	public float GetSideEnemyStandardReward()
	{
		float paraA = GameConfig.Instance.Standary_Enemy_Info.ParaA;
		float paraB = GameConfig.Instance.Standary_Enemy_Info.ParaB;
		float paraC = GameConfig.Instance.Standary_Enemy_Info.ParaC;
		float paraK = GameConfig.Instance.Standary_Enemy_Info.ParaK;
		float num = Instance.day_level;
		return paraA * Mathf.Pow(num - paraB, paraK) + paraC;
	}

	public float GetSideEnemyStandardRewardTotal()
	{
		float sideEnemyStandardReward = GetSideEnemyStandardReward();
		return sideEnemyStandardReward * GameConfig.Instance.GetStandaryEnemyCount(Instance.day_level);
	}

	public int GetMissionRewardCash(MissionType mission_type, MissionDayType mission_day_type, int crazy_daily = 0)
	{
		float sideEnemyStandardRewardTotal = GetSideEnemyStandardRewardTotal();
		int result = 0;
		switch (mission_day_type)
		{
		case MissionDayType.Daily:
			switch (crazy_daily)
			{
			case 0:
				result = ((!Instance.is_crazy_daily) ? ((int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_a * sideEnemyStandardRewardTotal)) : ((int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_b * sideEnemyStandardRewardTotal)));
				break;
			case 1:
				result = (int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_a * sideEnemyStandardRewardTotal);
				break;
			case 2:
				result = (int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_b * sideEnemyStandardRewardTotal);
				break;
			}
			break;
		case MissionDayType.Tutorial:
			result = GameConfig.Instance.init_cash;
			break;
		case MissionDayType.Main:
			result = (int)GameConfig.Instance.Main_Quest_Difficulty_Set[Instance.day_level].finish_reward;
			break;
		default:
			result = (int)(GameConfig.Instance.Mission_Finish_Reward_Info.side_ratio * sideEnemyStandardRewardTotal);
			break;
		}
		return result;
	}

	public int GetMissionRewardCrystal(MissionType mission_type, MissionDayType mission_day_type)
	{
		int result = 0;
		switch (mission_day_type)
		{
		case MissionDayType.Main:
			result = 1;
			break;
		case MissionDayType.Tutorial:
			result = GameConfig.Instance.init_crystal;
			break;
		}
		return result;
	}

	public int GetMissionRewardVoucher(MissionType mission_type, MissionDayType mission_day_type, int crazy_daily = 0)
	{
		int result = 0;
		switch (mission_day_type)
		{
		case MissionDayType.Daily:
			switch (crazy_daily)
			{
			case 0:
				result = ((!Instance.is_crazy_daily) ? ((int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_voucher_base * GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_voucher_a)) : ((int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_voucher_base * GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_voucher_b)));
				break;
			case 1:
				result = (int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_voucher_base * GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_voucher_a);
				break;
			case 2:
				result = (int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_voucher_base * GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_voucher_b);
				break;
			}
			break;
		case MissionDayType.Tutorial:
			result = GameConfig.Instance.init_voucher;
			break;
		default:
			result = 1;
			break;
		case MissionDayType.Main:
			break;
		}
		return result;
	}

	public DailyMissionStatus EnableDailyMission()
	{
		if (daily_mode_enable && daily_mission_count < 1)
		{
			return DailyMissionStatus.Free;
		}
		if (daily_mode_enable && daily_mission_count < 5)
		{
			return DailyMissionStatus.CrystalEnable;
		}
		if (daily_mode_enable && daily_mission_count >= 5)
		{
			return DailyMissionStatus.CrystalDisenable;
		}
		return DailyMissionStatus.Disenable;
	}

	public bool EnableEnterDailyMission()
	{
		if ((EnableDailyMission() == DailyMissionStatus.Free || EnableDailyMission() == DailyMissionStatus.CrystalEnable) && total_crystal >= GetDailyMissionPrice(is_crazy_daily))
		{
			return true;
		}
		return false;
	}

	public int GetDailyMissionPrice(bool is_hard)
	{
		if (is_hard)
		{
			if (EnableDailyMission() == DailyMissionStatus.Free)
			{
				return GameConfig.Instance.daily_price_hard1;
			}
			if (EnableDailyMission() == DailyMissionStatus.CrystalEnable)
			{
				return GameConfig.Instance.daily_price_hard2;
			}
		}
		else
		{
			if (EnableDailyMission() == DailyMissionStatus.Free)
			{
				return GameConfig.Instance.daily_price_easy1;
			}
			if (EnableDailyMission() == DailyMissionStatus.CrystalEnable)
			{
				return GameConfig.Instance.daily_price_easy2;
			}
		}
		return 99999;
	}

	public int GetDailyCDPrice()
	{
		if (EnableDailyMission() == DailyMissionStatus.CrystalEnable)
		{
			return GameConfig.Instance.daily_price_cd;
		}
		return 99999;
	}

	public bool ResetDailyCD()
	{
		if (EnableDailyMission() == DailyMissionStatus.CrystalEnable && total_crystal >= GetDailyCDPrice())
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("tCrystalNum", GetDailyCDPrice());
			Instance.UploadStatistics("tCrystal_SpeedUp", hashtable);
			total_crystal -= GetDailyCDPrice();
			is_daily_cd_crystal = true;
			return true;
		}
		return false;
	}

	public static string Encipher(string data_encipher)
	{
		int num = 17;
		char[] array = data_encipher.ToCharArray();
		char[] array2 = data_encipher.ToCharArray();
		char[] array3 = new char[2] { '\0', '\0' };
		for (int i = 0; i < array.Length; i++)
		{
			char c = (array3[0] = array[i]);
			string s = new string(array3);
			int num2 = char.ConvertToUtf32(s, 0);
			num2 ^= num;
			array2[i] = char.ConvertFromUtf32(num2)[0];
		}
		return new string(array2);
	}

	public static bool IsHighEffect()
	{
		return true;
	}

	public void OnExchgCurrcy(GameCurrencyType src_type, GameCurrencyType des_type, int src_count, int des_count)
	{
		switch (src_type)
		{
		case GameCurrencyType.Crystal:
			total_crystal -= src_count;
			if (total_crystal <= 0)
			{
				Debug.LogWarning(string.Concat("ExchgCurrcy warning src type:", src_type, " count:", src_count));
				total_crystal = new GameDataInt(0);
			}
			break;
		case GameCurrencyType.Voucher:
			total_voucher -= src_count;
			if (total_voucher <= 0)
			{
				Debug.LogWarning(string.Concat("ExchgCurrcy warning src type:", src_type, " count:", src_count));
				total_voucher = new GameDataInt(0);
			}
			break;
		case GameCurrencyType.Cash:
			total_cash -= src_count;
			if (total_cash <= 0)
			{
				Debug.LogWarning(string.Concat("ExchgCurrcy warning src type:", src_type, " count:", src_count));
				total_cash = new GameDataInt(0);
			}
			break;
		}
		Hashtable hashtable = new Hashtable();
		switch (des_type)
		{
		case GameCurrencyType.Crystal:
			total_crystal += des_count;
			break;
		case GameCurrencyType.Voucher:
			total_voucher += des_count;
			hashtable.Add("tCrystalNum", src_count);
			hashtable.Add("GoldNum", des_count);
			Instance.UploadStatistics("tCrystal_Gold", hashtable);
			break;
		case GameCurrencyType.Cash:
			total_cash += des_count;
			hashtable.Add("tCrystalNum", src_count);
			hashtable.Add("VouchersNum", des_count);
			Instance.UploadStatistics("tCrystal_Vouchers", hashtable);
			break;
		}
	}

	public void CheckEnemyFirstShow(string enemy_name)
	{
		if (Enemy_Loading_Set.ContainsKey(enemy_name) && Enemy_Loading_Set[enemy_name] != 1)
		{
			Enemy_Loading_Set[enemy_name] = 1;
		}
	}

	public void UploadStatistics(string action_id, Hashtable action_data)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["gamename"] = "com.trinitigame.callofminizombies2";
		hashtable["uuid"] = user_id;
		hashtable["action"] = action_id;
		string text2 = (string)(hashtable["data"] = JsonMapper.ToJson(action_data));
		string s = JsonMapper.ToJson(hashtable);
		byte[] post_data = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		wwwClient.Instance.SendHttpRequest(statistics_url, post_data, OnStatisticsResponse, OnStatisticsRequestError, action_id);
	}

	public void OnStatisticsResponse(string action, byte[] response_data)
	{
		string jsonString = string.Empty;
		if (response_data == null)
		{
			Debug.LogError("OnStatisticsResponse:" + action + " response_data error");
			return;
		}
		byte[] array = XXTEAUtils.Decrypt(response_data, Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		if (array != null)
		{
			jsonString = Encoding.UTF8.GetString(array);
		}
		JSONObject jSONObject = JSONObject.Parse(jsonString);
		if (jSONObject.ContainsKey("code"))
		{
			string text = jSONObject["code"].ToString();
			if (!(text == "0"))
			{
			}
		}
	}

	public void OnStatisticsRequestError(string action, byte[] post_data)
	{
		Debug.Log("OnRequestError");
	}

	private void OnApplicationPause(bool pause)
	{
		Debug.Log("OnApplicationPause:" + pause);
		if (is_enter_tutorial)
		{
			return;
		}
		if (!pause)
		{
			DateTime curDateTime = GetCurDateTime();
			TimeSpan timeSpan = (curDateTime - finally_save_date).Duration();
			if (timeSpan >= TimeSpan.FromMinutes(120.0))
			{
				Debug.Log("Long time no see.");
				TAudioManager.instance.soundVolume = 1f;
				TAudioManager.instance.musicVolume = 1f;
				Time.timeScale = 1f;
				LoadingUIController.FinishedLoading();
				Application.LoadLevel("GameCover");
			}
			PushNotification.ReSetNotifications();
		}
		else
		{
			finally_save_date = GetCurDateTime();
			Hashtable hashtable = new Hashtable();
			hashtable.Add("count", 1);
			Instance.UploadStatistics("Logout", hashtable);
		}
	}

	public static string DataEncipher(string data)
	{
		if (!GameConfig.IsEditorMode())
		{
			string text = Encipher(data);
			return XXTEAUtils.Encrypt(text, GetImportContent());
		}
		return data;
	}

	public static string DataDecrypt(string data)
	{
		if (!GameConfig.IsEditorMode())
		{
			string data_encipher = XXTEAUtils.Decrypt(data, GetImportContent());
			return Encipher(data_encipher);
		}
		return data;
	}

	public static string GetImportContent()
	{
		return "in0yt@n5#f.o71[";
	}

	public static DateTime GetCurDateTime()
	{
		return new DateTime(MiscPlugin.GetIOSYear(), MiscPlugin.GetIOSMonth(), MiscPlugin.GetIOSDay(), MiscPlugin.GetIOSHour(), MiscPlugin.GetIOSMin(), MiscPlugin.GetIOSSec());
	}

	private void OnGameDataVersionDifferent()
	{
		Debug.Log("OnGameDataVersionDifferent...");
		GameConfig.Instance.force_update_local = true;
		GameConfig.Instance.Init();
		SaveData();
	}

	public void SaveIapFailedInfo(string Random, int Rat, string product_Id, string tid, string receipt)
	{
		Iap_failed_info[Random + "|" + Rat + "|" + product_Id + "|" + tid] = receipt;
		SaveData();
	}

	public void RemoveIapFailedInfo(string Random, int Rat, string product_Id, string tid, string receipt)
	{
		if (Iap_failed_info.ContainsKey(Random + "|" + Rat + "|" + product_Id + "|" + tid))
		{
			Iap_failed_info.Remove(Random + "|" + Rat + "|" + product_Id + "|" + tid);
		}
		SaveData();
	}

	public static bool CheckSaveDataVersion()
	{
		string content = string.Empty;
		if (Utils.FileReadString(Utils.SavePath() + MD5Sample.GetMd5String("CoMZ2") + ".bytes", ref content))
		{
			Configure configure = new Configure();
			content = DataDecrypt(content);
			configure.Load(content);
			string single = configure.GetSingle("Save", "Version");
			if (single != "2.1.2")
			{
				return true;
			}
		}
		return false;
	}
}
