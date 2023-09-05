using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class MissionController : MonoBehaviour
{
	public MissionType mission_type;

	public GameObject[] zombie_nest_array;

	public GameObject[] zombie_grave_array;

	public GameObject[] zombie_boss_array;

	protected bool is_mission_finished;

	protected bool is_mission_paused;

	public float mission_total_time;

	public virtual IEnumerator Start()
	{
		yield return 1;
	}

	public virtual void Update()
	{
		mission_total_time += Time.deltaTime;
	}

	public virtual void CaculateDifficulty()
	{
	}

	public virtual void InitMissionController()
	{
		zombie_nest_array = GameObject.FindGameObjectsWithTag("Zombie_Nest");
		zombie_grave_array = GameObject.FindGameObjectsWithTag("Zombie_Grave");
		zombie_boss_array = GameObject.FindGameObjectsWithTag("Zombie_Boss");
	}

	public GameObject FindClosedGrave(Vector3 pos)
	{
		GameObject result = null;
		if (zombie_grave_array != null)
		{
			float num = 9999f;
			float num2 = 9999f;
			GameObject[] array = zombie_grave_array;
			foreach (GameObject gameObject in array)
			{
				num2 = (gameObject.transform.position - pos).sqrMagnitude;
				if (num2 < num)
				{
					result = gameObject;
					num = num2;
				}
			}
		}
		return result;
	}

	public virtual float SpwanZombiesFromGrave(EnemyType type, GameObject grave)
	{
		if (grave == null)
		{
			Debug.LogError("Spwan zombie from grave, grave is null.");
			return 0f;
		}
		float x = Random.Range((0f - grave.transform.localScale.x) / 2f, grave.transform.localScale.x / 2f);
		float z = Random.Range((0f - grave.transform.localScale.z) / 2f, grave.transform.localScale.z / 2f);
		Vector3 vector = grave.transform.position + new Vector3(x, 0f, z);
		if (type == EnemyType.E_CROW)
		{
			if (Application.loadedLevelName != "test_new1")
			{
				StartCoroutine(EnemyFactory.CreateEnemy(type, GameSceneController.Instance.way_points[0].transform.position, Quaternion.identity));
			}
		}
		else
		{
			StartCoroutine(EnemyFactory.CreateEnemy(type, vector, Quaternion.identity));
			if (type != EnemyType.E_FATCOOK && type != EnemyType.E_HAOKE_A && type != EnemyType.E_HAOKE_B && GameData.Instance.cur_quest_info.mission_type != MissionType.Tutorial)
			{
				GameSceneController.Instance.ground_stone_pool.GetComponent<ObjectPool>().CreateObject(vector, Quaternion.identity);
			}
		}
		return GameConfig.Instance.EnemyConfig_Set[type].missionWeight;
	}

	public virtual float SpwanZombiesFromNest(EnemyType type, GameObject nest)
	{
		if (nest == null)
		{
			Debug.LogError("Spwan zombie from nest, nest is null.");
			return 0f;
		}
		if (type == EnemyType.E_CROW)
		{
			if (Application.loadedLevelName != "test_new1")
			{
				StartCoroutine(EnemyFactory.CreateEnemy(type, GameSceneController.Instance.way_points[0].transform.position, Quaternion.identity));
			}
		}
		else
		{
			StartCoroutine(EnemyFactory.CreateEnemy(type, nest.transform.position, nest.transform.rotation));
		}
		return GameConfig.Instance.EnemyConfig_Set[type].missionWeight;
	}

	public virtual void MissionFinished()
	{
		is_mission_finished = true;
		GameSceneController.Instance.MissionControllerFinished();
	}

	public virtual void SetMissionPaused(bool state)
	{
		is_mission_paused = state;
	}

	public virtual List<EnemyType> GetMissionEnemyTypeList()
	{
		return null;
	}
}
