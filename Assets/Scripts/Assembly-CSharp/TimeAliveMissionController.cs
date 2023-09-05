using System;
using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class TimeAliveMissionController : MissionController
{
	public float mission_life_time = 50f;

	protected bool mission_started;

	private float last_update_info_time;

	private float update_info_rate = 0.3f;

	public override List<EnemyType> GetMissionEnemyTypeList()
	{
		List<EnemyWaveInfo> list = null;
		foreach (int key in GameConfig.Instance.EnemyWaveInfo_Normal_Set.Keys)
		{
			if (GameSceneController.Instance.DayLevel <= key)
			{
				list = GameConfig.Instance.EnemyWaveInfo_Normal_Set[key].wave_info_list;
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
		return list2;
	}

	public override IEnumerator Start()
	{
		InitMissionController();
		mission_type = MissionType.Time_ALive;
		CaculateDifficulty();
		yield return 1;
		PlayerController player = GameSceneController.Instance.player_controller;
		while (player == null)
		{
			yield return 1;
			player = GameSceneController.Instance.player_controller;
		}
		List<EnemyWaveInfo> EnemyWaveInfo_Set = null;
		foreach (int level in GameConfig.Instance.EnemyWaveInfo_Normal_Set.Keys)
		{
			if (GameSceneController.Instance.DayLevel <= level)
			{
				EnemyWaveInfo_Set = GameConfig.Instance.EnemyWaveInfo_Normal_Set[level].wave_info_list;
				break;
			}
		}
		while (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
		{
			yield return 1;
		}
		mission_started = true;
		yield return new WaitForSeconds(1f);
		while (mission_life_time > 0f)
		{
			int index = UnityEngine.Random.Range(0, EnemyWaveInfo_Set.Count);
			EnemyWaveInfo wave = EnemyWaveInfo_Set[index];
			foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
			{
				if (mission_life_time <= 0f)
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
					if (mission_life_time <= 0f)
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
						SpwanZombiesFromNest(EType, zombie_nest_array[UnityEngine.Random.Range(0, zombie_nest_array.Length)]);
						yield return new WaitForSeconds(0.3f);
						break;
					}
				}
				yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Normal.line_interval);
			}
			yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Normal.wave_interval);
		}
		Debug.Log("Mission Life Over~");
	}

	public override void Update()
	{
		base.Update();
		if (GameSceneController.Instance.GamePlayingState == PlayingState.CG || !mission_started || is_mission_finished || is_mission_paused)
		{
			return;
		}
		if (mission_life_time > 0f)
		{
			mission_life_time -= Time.deltaTime;
			if (Time.time - last_update_info_time >= update_info_rate)
			{
				last_update_info_time = Time.time;
				TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.FloorToInt(mission_life_time));
				GameSceneController.Instance.game_main_panel.time_alive_panel.SetContent(timeSpan.Minutes.ToString("d2") + ":" + timeSpan.Seconds.ToString("d2"));
			}
		}
		else
		{
			MissionFinished();
		}
	}

	public override void CaculateDifficulty()
	{
		mission_life_time = 90f;
	}
}
