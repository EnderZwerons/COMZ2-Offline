using System.Collections;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
	protected static EnemyFactory instance;

	public EnemyFactory Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject.CreatePrimitive(PrimitiveType.Cube);
			}
			return instance;
		}
	}

	public static IEnumerator CreateEnemy(EnemyType type, Vector3 pos, Quaternion rot)
	{
		GameObject enemy_ref = GameSceneController.Instance.enemy_ref_map.Enemy_Set[type];
		yield return 1;
		GameObject enemy = Object.Instantiate(enemy_ref.GetComponent<SinglePrefabReference>().Instance, pos, rot) as GameObject;
		yield return 1;
		EnemyController eController = Utility.AddEnemyComponent(enemy, GetEnemyTypeControllerName(type));
		EnemyData data = EnemyData.CreateData(GameConfig.Instance.EnemyConfig_Set[type]);
		eController.SetEnemyData(data);
		eController.EnemyID = GameSceneController.Instance.EnemyIndex;
		eController.Accessory = enemy_ref.GetComponent<SinglePrefabReference>().Accessory;
		eController.is_traped = false;
		enemy.name = "Enemy_" + eController.EnemyID;
		enemy.GetComponent<EnemyAnimationEvent>().SetController(eController);
		GameSceneController.Instance.OnEnemySpawn(data);
		yield return 1;
		GameSceneController.Instance.Enemy_Set.Add(eController.EnemyID, eController);
	}

	public static EnemyController CreateRemoteEnemy(EnemyType type, Vector3 pos, Quaternion rot, int enemy_id, bool is_boss = false)
	{
		GameObject gameObject = GameSceneController.Instance.enemy_ref_map.Enemy_Set[type];
		GameObject gameObject2 = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Instance, pos, rot) as GameObject;
		EnemyController enemyController = Utility.AddEnemyComponent(gameObject2, GetEnemyTypeControllerName(type));
		EnemyData enemyData = EnemyData.CreateData(GameConfig.Instance.EnemyConfig_Set[type]);
		enemyController.SetEnemyData(enemyData);
		enemyController.EnemyID = enemy_id;
		enemyController.Accessory = gameObject.GetComponent<SinglePrefabReference>().Accessory;
		enemyController.is_traped = false;
		gameObject2.name = "Enemy_" + enemyController.EnemyID;
		gameObject2.GetComponent<EnemyAnimationEvent>().SetController(enemyController);
		GameSceneController.Instance.OnEnemySpawn(enemyData);
		GameSceneController.Instance.Enemy_Set.Add(enemyController.EnemyID, enemyController);
		GameSceneController.Instance.EnemyIndex = enemy_id + 1;
		return enemyController;
	}

	public static EnemyController CreateEnemyGetEnemyController(EnemyType type, Vector3 pos, Quaternion rot)
	{
		GameObject gameObject = GameSceneController.Instance.enemy_ref_map.Enemy_Set[type];
		GameObject gameObject2 = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Instance, pos, rot) as GameObject;
		EnemyController enemyController = Utility.AddEnemyComponent(gameObject2, GetEnemyTypeControllerName(type));
		EnemyData enemyData = EnemyData.CreateData(GameConfig.Instance.EnemyConfig_Set[type]);
		enemyController.SetEnemyData(enemyData);
		enemyController.EnemyID = GameSceneController.Instance.EnemyIndex;
		enemyController.Accessory = gameObject.GetComponent<SinglePrefabReference>().Accessory;
		enemyController.is_traped = false;
		gameObject2.name = "Enemy_" + enemyController.EnemyID;
		gameObject2.GetComponent<EnemyAnimationEvent>().SetController(enemyController);
		GameSceneController.Instance.Enemy_Set.Add(enemyController.EnemyID, enemyController);
		GameSceneController.Instance.OnEnemySpawn(enemyData);
		return enemyController;
	}

	public static EnemyController CreateEnemyForTrap(EnemyType type, Vector3 pos)
	{
		GameObject gameObject = GameSceneController.Instance.enemy_ref_map.Enemy_Set[type];
		GameObject gameObject2 = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Instance, pos, Quaternion.identity) as GameObject;
		EnemyController enemyController = Utility.AddEnemyComponent(gameObject2, GetEnemyTypeControllerName(type));
		EnemyData enemyData = EnemyData.CreateData(GameConfig.Instance.EnemyConfig_Set[type]);
		enemyController.SetEnemyData(enemyData);
		enemyController.EnemyID = GameSceneController.Instance.EnemyIndex;
		enemyController.Accessory = gameObject.GetComponent<SinglePrefabReference>().Accessory;
		enemyController.is_traped = true;
		gameObject2.name = "Enemy_" + enemyController.EnemyID;
		gameObject2.GetComponent<EnemyAnimationEvent>().SetController(enemyController);
		GameSceneController.Instance.Enemy_Set.Add(enemyController.EnemyID, enemyController);
		return enemyController;
	}

	public static string GetEnemyTypeControllerName(EnemyType type)
	{
		string result = "EnemyController";
		switch (type)
		{
		case EnemyType.E_ZOMBIE:
		case EnemyType.E_ZOMBIE_E:
		case EnemyType.E_ZOMBIE_COMMIS:
		case EnemyType.E_ZOMBIE_COMMIS_E:
			result = "ZombieController";
			break;
		case EnemyType.E_NURSE:
		case EnemyType.E_NURSE_E:
			result = "NurseController";
			break;
		case EnemyType.E_BOOMER:
		case EnemyType.E_BOOMER_E:
			result = "BoomerController";
			break;
		case EnemyType.E_BOOMER_TIMER:
			result = "BoomerTimerController";
			break;
		case EnemyType.E_CROW:
			result = "CrowController";
			break;
		case EnemyType.E_CLOWN:
		case EnemyType.E_CLOWN_E:
			result = "ClownController";
			break;
		case EnemyType.E_FATCOOK:
			result = "FatCookController";
			break;
		case EnemyType.E_HAOKE_A:
		case EnemyType.E_HAOKE_B:
			result = "HaokeController";
			break;
		case EnemyType.E_WRESTLER:
			result = "WrestlerController";
			break;
		case EnemyType.E_HALLOWEEN:
			result = "HalloweenController";
			break;
		case EnemyType.E_HALLOWEEN_SUB:
			result = "HalloweenSubController";
			break;
		case EnemyType.E_SHARK:
			result = "SharkController";
			break;
		}
		return result;
	}

	public static GameObject CreateEnemyObj(EnemyType type)
	{
		GameObject gameObject = GameSceneController.Instance.enemy_ref_map.Enemy_Set[type];
		return Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Instance) as GameObject;
	}
}
