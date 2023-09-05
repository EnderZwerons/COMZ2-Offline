using System.Collections.Generic;
using UnityEngine;

public class EnemyMap : MonoBehaviour
{
	public Dictionary<EnemyType, GameObject> Enemy_Set = new Dictionary<EnemyType, GameObject>();

	private void Awake()
	{
	}

	public void ResetEnemyMapInfo(List<EnemyType> enemy_type_list)
	{
		GameObject gameObject = null;
		foreach (EnemyType item in enemy_type_list)
		{
			gameObject = Object.Instantiate(Resources.Load("Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[item].enemy_name)) as GameObject;
			Enemy_Set[item] = gameObject;
			switch (item)
			{
			case EnemyType.E_FATCOOK:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[EnemyType.E_BOOMER_TIMER].enemy_name)) as GameObject;
				Enemy_Set[EnemyType.E_BOOMER_TIMER] = gameObject;
				break;
			case EnemyType.E_HALLOWEEN:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[EnemyType.E_HALLOWEEN_SUB].enemy_name)) as GameObject;
				Enemy_Set[EnemyType.E_HALLOWEEN_SUB] = gameObject;
				break;
			}
		}
		foreach (EnemyType key in Enemy_Set.Keys)
		{
			GameObject gameObject2 = EnemyFactory.CreateEnemyObj(key);
			gameObject2.SetActive(false);
		}
	}
}
