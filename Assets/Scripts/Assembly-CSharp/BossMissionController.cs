using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class BossMissionController : MissionController
{
	private float last_check_time;

	private float check_rate = 1f;

	protected EnemyController boss;

	public EnemyController MissionBoss
	{
		get
		{
			return boss;
		}
	}

	public override List<EnemyType> GetMissionEnemyTypeList()
	{
		List<EnemyWaveInfo> list = null;
		foreach (int key in GameConfig.Instance.EnemyWaveInfo_Boss_Set.Keys)
		{
			if (GameSceneController.Instance.DayLevel <= key)
			{
				list = GameConfig.Instance.EnemyWaveInfo_Boss_Set[key].wave_info_list;
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
		list2.Add(GameData.Instance.cur_quest_info.boss_type);
		return list2;
	}

	public override IEnumerator Start()
	{
		InitMissionController();
		mission_type = MissionType.Boss;
		CaculateDifficulty();
		yield return 1;
		PlayerController player = GameSceneController.Instance.player_controller;
		while (player == null)
		{
			yield return 1;
			player = GameSceneController.Instance.player_controller;
		}
		string frame_name = GameConfig.Instance.EnemyConfig_Set[GameData.Instance.cur_quest_info.boss_type].enemy_name + "_icon_s";
		GameSceneController.Instance.game_main_panel.boss_panel.SetIcon(frame_name);
		GameSceneController.Instance.game_main_panel.boss_panel.SetContent(0 + " / " + 1);
		List<EnemyWaveInfo> EnemyWaveInfo_Set = null;
		foreach (int level in GameConfig.Instance.EnemyWaveInfo_Boss_Set.Keys)
		{
			if (GameSceneController.Instance.DayLevel <= level)
			{
				EnemyWaveInfo_Set = GameConfig.Instance.EnemyWaveInfo_Boss_Set[level].wave_info_list;
				break;
			}
		}
		while (!GameSceneController.Instance.enable_boss_spawn)
		{
			yield return 1;
		}
		boss = SpwanBossFromNest(GameData.Instance.cur_quest_info.boss_type, zombie_boss_array[Random.Range(0, zombie_boss_array.Length)]);
		boss.IsBoss = true;
		while (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
		{
			yield return 1;
		}
		GameSceneController.Instance.player_controller.UpdateWeaponUIShow();
		yield return new WaitForSeconds(1f);
		while (boss != null && boss.enemy_data.cur_hp > 0f)
		{
			int index = Random.Range(0, EnemyWaveInfo_Set.Count);
			EnemyWaveInfo wave = EnemyWaveInfo_Set[index];
			foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
			{
				if (boss == null || boss.enemy_data.cur_hp <= 0f)
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
					if (boss == null || boss.enemy_data.cur_hp <= 0f)
					{
						break;
					}
					switch (From)
					{
					case SpawnFromType.Grave:
					{
						GameObject grave = FindClosedGrave(player.transform.position);
						SpwanZombiesFromGrave(EType, grave);
						yield return new WaitForSeconds(0.3f);
						break;
					}
					case SpawnFromType.Nest:
						SpwanZombiesFromNest(EType, zombie_nest_array[Random.Range(0, zombie_nest_array.Length)]);
						yield return new WaitForSeconds(0.3f);
						break;
					}
				}
				yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Boss.line_interval);
			}
			yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Boss.wave_interval);
		}
		MissionFinished();
	}

	public override void Update()
	{
		base.Update();
		if (!is_mission_finished)
		{
			last_check_time += Time.deltaTime;
			if (last_check_time >= check_rate)
			{
				last_check_time = 0f;
			}
		}
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
		return EnemyFactory.CreateEnemyGetEnemyController(type, nest.transform.position, nest.transform.rotation);
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

	public void SpawnBoss()
	{
	}
}
