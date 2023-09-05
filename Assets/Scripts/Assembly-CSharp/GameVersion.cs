using System.Collections;
using System.IO;
using CoMZ2;
using UnityEngine;

public class GameVersion : MonoBehaviour
{
	private static GameVersion instance;

	protected string content = string.Empty;

	public bool is_test_config;

	public static GameVersion Instance
	{
		get
		{
			return instance;
		}
	}

	public static void CheckGameVersionInstance()
	{
		if (!GameObject.Find("GameVersion"))
		{
			GameObject gameObject = new GameObject("GameVersion");
			gameObject.transform.parent = null;
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.AddComponent<GameVersion>();
		}
	}

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
	}

	public void CheckRemoteGameVersion(OnServerVersion callback, OnServerVersionError callback_error)
	{
		if (GameDefine.LOAD_CONFIG_SAVE_PATH && GameConfig.IsEditorMode())
		{
			Utils.FileReadString(Utils.SavePath() + "CoMZ2_version.bytes", ref content);
			content = Encipher(content);
			Debug.Log(content);
			Configure configure = new Configure();
			configure.Load(content);
			string single = configure.GetSingle("CoMZ2", "Ver");
			string single2 = configure.GetSingle("CoMZ2", "TestVer");
			single = configure.GetSingle("CoMZ2", "VerAndroid");
			single2 = configure.GetSingle("CoMZ2", "TestVerAndroid");
			string single3 = configure.GetSingle("CoMZ2", "TimeserverUrl");
			string single4 = configure.GetSingle("CoMZ2", "StatisticsUrl");
			string text = "http://192.225.224.97:7600/gameapi/GameCommon.do?action=groovy&json=";
			string single5 = configure.GetSingle("CoMZ2", "RedeemGetUrl");
			string single6 = configure.GetSingle("CoMZ2", "RedeemAcceptUrl");
			if (GameData.Instance.timeserver_url != single3 || GameData.Instance.statistics_url != single4 || GameData.Instance.iap_check_url != text || GameData.Instance.redeem_get_url != single5 || GameData.Instance.redeem_accept_url != single6)
			{
				GameData.Instance.timeserver_url = single3;
				GameData.Instance.statistics_url = single4;
				GameData.Instance.iap_check_url = text;
				GameData.Instance.redeem_get_url = single5;
				GameData.Instance.redeem_accept_url = single6;
				GameData.Instance.SaveData();
			}
			bool flag = ((int.Parse(configure.GetSingle("CoMZ2", "IapCheck")) != 0) ? true : false);
			if (flag != GameData.Instance.TRINITI_IAP_CEHCK)
			{
				GameData.Instance.TRINITI_IAP_CEHCK = flag;
				GameData.Instance.SaveData();
			}
			float num = float.Parse(configure.GetSingle("CoMZ2", "RedeemAwardRatio"));
			if (num != GameData.Instance.redeem_change_ratio)
			{
				GameData.Instance.redeem_change_ratio = num;
				GameData.Instance.SaveData();
			}
			int num2 = 0;
			if (single == "2.1.2")
			{
				Debug.Log("to normal server.");
				is_test_config = false;
				num2 = int.Parse(configure.GetSingle("CoMZ2", "ConfigVersionCount"));
				for (int i = 0; i < num2; i++)
				{
					string array = configure.GetArray2("CoMZ2", "ConfigVersion", i, 0);
					string array2 = configure.GetArray2("CoMZ2", "ConfigVersion", i, 1);
					GameConfig.Instance.Remote_Config_Version_Set[array] = array2;
				}
				if (callback != null)
				{
					callback(true);
				}
			}
			else if (single2 == "2.1.2")
			{
				Debug.Log("to test server.");
				is_test_config = true;
				num2 = int.Parse(configure.GetSingle("CoMZ2", "ConfigVersionCountTest"));
				for (int j = 0; j < num2; j++)
				{
					string array3 = configure.GetArray2("CoMZ2", "ConfigVersionTest", j, 0);
					string array4 = configure.GetArray2("CoMZ2", "ConfigVersionTest", j, 1);
					GameConfig.Instance.Remote_Config_Version_Set[array3] = array4;
				}
				if (callback != null)
				{
					callback(true);
				}
			}
			else
			{
				Debug.Log("game version error.");
				if (callback != null)
				{
					callback(false);
				}
			}
		}
		else
		{
			StartCoroutine(CheckGameVersion(callback, callback_error));
		}
	}

	private IEnumerator CheckGameVersion(OnServerVersion callback, OnServerVersionError callback_error)
	{
		WWW www = new WWW("http://account.trinitigame.com/game/callofminizombies2/new_version/CoMZ2_version.bytes?rand=" + Random.Range(10, 99999));
		yield return www;
		if (www.error != null)
		{
			Debug.Log(www.error);
			if (callback_error != null)
			{
				callback_error();
			}
			yield break;
		}
		content = www.text;
		content = Encipher(content);
		Debug.Log(content);
		Configure cfg = new Configure();
		cfg.Load(content);
		string ver2 = cfg.GetSingle("CoMZ2", "Ver");
		string ver_test2 = cfg.GetSingle("CoMZ2", "TestVer");
		ver2 = cfg.GetSingle("CoMZ2", "VerAndroid");
		ver_test2 = cfg.GetSingle("CoMZ2", "TestVerAndroid");
		string timeserver_url = cfg.GetSingle("CoMZ2", "TimeserverUrl");
		string statistics_url = cfg.GetSingle("CoMZ2", "StatisticsUrl");
		string iap_url = "http://192.225.224.97:7600/gameapi/GameCommon.do?action=groovy&json=";
		string redeem_get_url = cfg.GetSingle("CoMZ2", "RedeemGetUrl");
		string redeem_accept_url = cfg.GetSingle("CoMZ2", "RedeemAcceptUrl");
		if (GameData.Instance.timeserver_url != timeserver_url || GameData.Instance.statistics_url != statistics_url || GameData.Instance.iap_check_url != iap_url || GameData.Instance.redeem_get_url != redeem_get_url || GameData.Instance.redeem_accept_url != redeem_accept_url)
		{
			GameData.Instance.timeserver_url = timeserver_url;
			GameData.Instance.statistics_url = statistics_url;
			GameData.Instance.iap_check_url = iap_url;
			GameData.Instance.redeem_get_url = redeem_get_url;
			GameData.Instance.redeem_accept_url = redeem_accept_url;
			GameData.Instance.SaveData();
		}
		bool check_tem = ((int.Parse(cfg.GetSingle("CoMZ2", "IapCheck")) != 0) ? true : false);
		if (check_tem != GameData.Instance.TRINITI_IAP_CEHCK)
		{
			GameData.Instance.TRINITI_IAP_CEHCK = check_tem;
			GameData.Instance.SaveData();
		}
		float redeem_change_ratio = float.Parse(cfg.GetSingle("CoMZ2", "RedeemAwardRatio"));
		if (redeem_change_ratio != GameData.Instance.redeem_change_ratio)
		{
			GameData.Instance.redeem_change_ratio = redeem_change_ratio;
			GameData.Instance.SaveData();
		}
		int count3 = 0;
		if (ver2 == "2.1.2")
		{
			Debug.Log("to normal server.");
			is_test_config = false;
			count3 = int.Parse(cfg.GetSingle("CoMZ2", "ConfigVersionCount"));
			for (int j = 0; j < count3; j++)
			{
				string file_name2 = cfg.GetArray2("CoMZ2", "ConfigVersion", j, 0);
				string file_version2 = cfg.GetArray2("CoMZ2", "ConfigVersion", j, 1);
				GameConfig.Instance.Remote_Config_Version_Set[file_name2] = file_version2;
			}
			if (callback != null)
			{
				callback(true);
			}
		}
		else if (ver_test2 == "2.1.2")
		{
			Debug.Log("to test server.");
			is_test_config = true;
			count3 = int.Parse(cfg.GetSingle("CoMZ2", "ConfigVersionCountTest"));
			for (int i = 0; i < count3; i++)
			{
				string file_name = cfg.GetArray2("CoMZ2", "ConfigVersionTest", i, 0);
				string file_version = cfg.GetArray2("CoMZ2", "ConfigVersionTest", i, 1);
				GameConfig.Instance.Remote_Config_Version_Set[file_name] = file_version;
			}
			if (callback != null)
			{
				callback(true);
			}
		}
		else
		{
			Debug.Log("game version error.");
			if (callback != null)
			{
				callback(false);
			}
		}
	}

	private string Encipher(string data_encipher)
	{
		int num = 30;
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

	public void OutputVersionCheckFile()
	{
		string data_encipher = Utils.LoadResourcesFileForText("CoMZ2_version");
		string text = Utils.SavePath();
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		data_encipher = Encipher(data_encipher);
		Utils.FileWriteString(text + "CoMZ2_version.bytes", data_encipher);
		Debug.Log("CoM2_version.bytes output is ok.");
	}
}
