using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class BossCoopMissionController : MissionController
{
	private float last_check_time;

	private float check_rate = 1f;

	private TNetObject tnetObj;

	public EnemyController boss;

	public EnemyController MissionBoss
	{
		get
		{
			return boss;
		}
	}

	public override List<EnemyType> GetMissionEnemyTypeList()
	{
		CoopBossCfg coopBossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
		List<EnemyWaveInfo> list = null;
		foreach (int key in GameConfig.Instance.EnemyWaveInfo_Boss_Coop_Set.Keys)
		{
			if (coopBossCfg.day_level <= key)
			{
				list = GameConfig.Instance.EnemyWaveInfo_Boss_Coop_Set[key].wave_info_list;
				break;
			}
		}
		List<EnemyType> list2 = new List<EnemyType>();
		foreach (EnemyWaveInfo item in list)
		{
			foreach (EnemySpawnInfo item2 in item.spawn_info_list)
			{
				if (!list2.Contains(item2.EType))
				{
					list2.Add(item2.EType);
				}
			}
		}
		list2.Add(coopBossCfg.boss_type);
		return list2;
	}

	public override IEnumerator Start()
	{
		InitMissionController();
		mission_type = MissionType.Boss;
		CaculateDifficulty();
		yield return 1;
		if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		{
			tnetObj = TNetConnection.Connection;
		}
		PlayerController player = GameSceneController.Instance.player_controller;
		while (player == null)
		{
			yield return 1;
			player = GameSceneController.Instance.player_controller;
		}
		CoopBossCfg boss_cfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
		string frame_name = boss_cfg.boss_name + "_icon_s";
		GameSceneController.Instance.game_main_panel.boss_panel.SetIcon(frame_name);
		GameSceneController.Instance.game_main_panel.boss_panel.SetContent(0 + " / " + 1);
		yield return new WaitForSeconds(3f);
		//while (!TNetConnection.IsServer)
		//{
		//	yield return 3;
		//}
		while (GameSceneController.Instance.IsMissionFinished)
		{
			yield return 3;
		}
		while (GameSceneController.Instance.is_boss_dead)
		{
			yield return 3;
		}
		List<EnemyWaveInfo> EnemyWaveInfo_Set = null;
		foreach (int level in GameConfig.Instance.EnemyWaveInfo_Boss_Coop_Set.Keys)
		{
			if (boss_cfg.day_level <= level)
			{
				EnemyWaveInfo_Set = GameConfig.Instance.EnemyWaveInfo_Boss_Coop_Set[level].wave_info_list;
				break;
			}
		}
		GameSceneController.Instance.player_controller.UpdateWeaponUIShow();
		yield return new WaitForSeconds(1f);
		int count_b = 4;
		while (GameSceneController.Instance.GetCurEnemyIndex() < count_b)
		{
			int index = Random.Range(0, EnemyWaveInfo_Set.Count);
			EnemyWaveInfo wave = EnemyWaveInfo_Set[index];
			foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
			{
				if (GameSceneController.Instance.GetCurEnemyIndex() >= count_b)
				{
					break;
				}
				EnemyType EType = spawn_info.EType;
				int Count = spawn_info.Count;
				SpawnFromType From = spawn_info.From;
				for (int i = 0; i < Count; i++)
				{
					while (GameSceneController.Instance.Enemy_Set.Count >= 8)
					{
						yield return new WaitForSeconds(1f);
					}
					while (GameSceneController.Instance.is_play_cg)
					{
						yield return new WaitForSeconds(1f);
					}
					if (GameSceneController.Instance.GetCurEnemyIndex() >= count_b)
					{
						break;
					}
					switch (From)
					{
					case SpawnFromType.Grave:
					{
						GameObject grave = FindClosedGrave(player.transform.position);
						SpwanZombiesFromGrave(EType, grave);
						yield return new WaitForSeconds(1f);
						break;
					}
					case SpawnFromType.Nest:
						SpwanZombiesFromNest(EType, zombie_nest_array[Random.Range(0, zombie_nest_array.Length)]);
						yield return new WaitForSeconds(1f);
						break;
					}
				}
			}
		}
		if (boss == null)
		{
			boss = SpwanBossFromNest(boss_cfg.boss_type, zombie_boss_array[Random.Range(0, zombie_boss_array.Length)]);
			boss.SetEnemyBeCoopBoss(boss_cfg);
			GameSceneCoopController.Instance.OnBossBirthCameraShow(boss);
		}
		while (boss != null && boss.enemy_data.cur_hp > 0f)
		{
			int index2 = Random.Range(0, EnemyWaveInfo_Set.Count);
			EnemyWaveInfo wave2 = EnemyWaveInfo_Set[index2];
			foreach (EnemySpawnInfo spawn_info2 in wave2.spawn_info_list)
			{
				if (boss == null || boss.enemy_data.cur_hp <= 0f)
				{
					break;
				}
				EnemyType EType2 = spawn_info2.EType;
				int Count2 = spawn_info2.Count;
				SpawnFromType From2 = spawn_info2.From;
				for (int j = 0; j < Count2; j++)
				{
					while (GameSceneController.Instance.Enemy_Set.Count >= 8)
					{
						yield return new WaitForSeconds(1f);
					}
					while (GameSceneController.Instance.is_play_cg)
					{
						yield return new WaitForSeconds(1f);
					}
					if (boss == null || boss.enemy_data.cur_hp <= 0f)
					{
						break;
					}
					switch (From2)
					{
					case SpawnFromType.Grave:
					{
						GameObject grave2 = FindClosedGrave(player.transform.position);
						SpwanZombiesFromGrave(EType2, grave2);
						yield return new WaitForSeconds(0.3f);
						break;
					}
					case SpawnFromType.Nest:
						SpwanZombiesFromNest(EType2, zombie_nest_array[Random.Range(0, zombie_nest_array.Length)]);
						yield return new WaitForSeconds(0.3f);
						break;
					}
				}
				yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Boss_Coop.line_interval);
			}
			yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Boss_Coop.wave_interval);
		}
		MissionFinished();
	}

	public override void Update()
	{
	}

	public override void CaculateDifficulty()
	{
	}

	public EnemyController SpwanBossFromNest(EnemyType type, GameObject nest)
	{
		if (nest == null)
		{
			Debug.LogError("Spwan zombie from nest, nest is null.");
			return null;
		}
		EnemyController enemyController = EnemyFactory.CreateEnemyGetEnemyController(type, nest.transform.position, nest.transform.rotation);
		if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && TNetConnection.IsServer && tnetObj != null)
		{
			SFSArray sFSArray = new SFSArray();
			sFSArray.AddShort((short)type);
			sFSArray.AddShort((short)enemyController.EnemyID);
			sFSArray.AddFloat(nest.transform.position.x);
			sFSArray.AddFloat(nest.transform.position.y);
			sFSArray.AddFloat(nest.transform.position.z);
			sFSArray.AddFloat(nest.transform.rotation.eulerAngles.x);
			sFSArray.AddFloat(nest.transform.rotation.eulerAngles.y);
			sFSArray.AddFloat(nest.transform.rotation.eulerAngles.z);
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutSFSArray("spawnBoss", sFSArray);
			tnetObj.Send(new BroadcastMessageRequest(sFSObject));
			Debug.Log("send spawnBoss info...");
		}
		return enemyController;
	}

	public override float SpwanZombiesFromNest(EnemyType type, GameObject nest)
	{
		if (nest == null)
		{
			Debug.LogError("Spwan zombie from nest, nest is null.");
			return 0f;
		}
		EnemyController enemyController = EnemyFactory.CreateEnemyGetEnemyController(type, nest.transform.position, nest.transform.rotation);
		if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && TNetConnection.IsServer && tnetObj != null)
		{
			SFSArray sFSArray = new SFSArray();
			sFSArray.AddShort((short)type);
			sFSArray.AddShort((short)enemyController.EnemyID);
			sFSArray.AddFloat(nest.transform.position.x);
			sFSArray.AddFloat(nest.transform.position.y);
			sFSArray.AddFloat(nest.transform.position.z);
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutSFSArray("spawnEnemy", sFSArray);
			tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		}
		if (type != EnemyType.E_FATCOOK && type != EnemyType.E_HAOKE_A && type != EnemyType.E_HAOKE_B && GameData.Instance.cur_quest_info.mission_type != MissionType.Tutorial)
		{
			GameSceneController.Instance.ground_stone_pool.GetComponent<ObjectPool>().CreateObject(nest.transform.position, Quaternion.identity);
		}
		return GameConfig.Instance.EnemyConfig_Set[type].missionWeight;
	}

	public IEnumerator FatcookSummon()
	{
		GameObject grave;
		if (boss != null)
		{
			grave = FindClosedGrave(boss.transform.position);
		}
		else
		{
			PlayerController player = GameSceneController.Instance.player_controller;
			grave = FindClosedGrave(player.transform.position);
		}
		SpwanZombiesFromGrave(EnemyType.E_BOOMER_TIMER, grave);
		yield return 1;
		SpwanZombiesFromGrave(EnemyType.E_BOOMER_TIMER, grave);
		yield return 1;
		SpwanZombiesFromGrave(EnemyType.E_BOOMER_TIMER, grave);
		yield return 1;
		SpwanZombiesFromGrave(EnemyType.E_BOOMER_TIMER, grave);
		yield return 1;
	}

	public IEnumerator HalloweenSummon()
	{
		GameObject grave;
		if (boss != null)
		{
			grave = FindClosedGrave(boss.transform.position);
		}
		else
		{
			PlayerController player = GameSceneController.Instance.player_controller;
			grave = FindClosedGrave(player.transform.position);
		}
		SpwanZombiesFromGrave(EnemyType.E_HALLOWEEN_SUB, grave);
		yield return 1;
		SpwanZombiesFromGrave(EnemyType.E_HALLOWEEN_SUB, grave);
		yield return 1;
	}

	public override float SpwanZombiesFromGrave(EnemyType type, GameObject grave)
	{
		if (grave == null)
		{
			Debug.LogError("Spwan zombie from grave, grave is null.");
			return 0f;
		}
		float x = Random.Range((0f - grave.transform.localScale.x) / 2f, grave.transform.localScale.x / 2f);
		float z = Random.Range((0f - grave.transform.localScale.z) / 2f, grave.transform.localScale.z / 2f);
		Vector3 vector = grave.transform.position + new Vector3(x, 0f, z);
		EnemyController enemyController = EnemyFactory.CreateEnemyGetEnemyController(type, vector, Quaternion.identity);
		if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && TNetConnection.IsServer && tnetObj != null)
		{
			SFSArray sFSArray = new SFSArray();
			sFSArray.AddShort((short)type);
			sFSArray.AddShort((short)enemyController.EnemyID);
			sFSArray.AddFloat(vector.x);
			sFSArray.AddFloat(vector.y);
			sFSArray.AddFloat(vector.z);
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutSFSArray("spawnEnemy", sFSArray);
			tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		}
		if (type != EnemyType.E_FATCOOK && type != EnemyType.E_HAOKE_A && type != EnemyType.E_HAOKE_B && GameData.Instance.cur_quest_info.mission_type != MissionType.Tutorial)
		{
			GameSceneController.Instance.ground_stone_pool.GetComponent<ObjectPool>().CreateObject(vector, Quaternion.identity);
		}
		return GameConfig.Instance.EnemyConfig_Set[type].missionWeight;
	}
}
