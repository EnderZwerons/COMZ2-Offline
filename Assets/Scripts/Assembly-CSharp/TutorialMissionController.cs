using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class TutorialMissionController : MissionController
{
	private GuideController guideController;

	public override List<EnemyType> GetMissionEnemyTypeList()
	{
		List<EnemyType> list = new List<EnemyType>();
		list.Add(EnemyType.E_ZOMBIE);
		return list;
	}

	public override IEnumerator Start()
	{
		GameSceneController.Instance.game_main_panel.gameObject.SetActive(false);
		if (zombie_nest_array == null)
		{
			InitMissionController();
		}
		mission_type = MissionType.Tutorial;
		yield return 1;
		PlayerController player = GameSceneController.Instance.player_controller;
		while (player == null)
		{
			yield return 1;
			player = GameSceneController.Instance.player_controller;
		}
		yield return new WaitForSeconds(1f);
		GameObject guideObj = Object.Instantiate(Resources.Load("Prefab/GuideUI")) as GameObject;
		guideObj.transform.parent = GameObject.Find("TUI/TUIControls").transform;
		guideObj.transform.localPosition = new Vector3(0f, 0f, -100f);
		guideController = guideObj.GetComponent<GuideController>();
		guideController.Show(new MoveGuide(guideController));
		yield return 1;
		SpwanZombiesFromNest(EnemyType.E_ZOMBIE, zombie_nest_array[0]);
		yield return new WaitForSeconds(3f);
		Vector3 arrowPosition = Vector3.zero;
		while (!EnemyNearPlayer(ref arrowPosition))
		{
			yield return 0;
		}
		guideController.Show(new KillZombieGuide(guideController, arrowPosition));
		while (!CouldHuntBox())
		{
			yield return 0;
		}
		guideController.Show(new DestroyBoxGuide(guideController));
		while (!GameSceneController.Instance.tutorial_ui_over)
		{
			yield return 0;
		}
		yield return new WaitForSeconds(1f);
		MissionFinished();
	}

	private bool EnemyNearPlayer(ref Vector3 arrowPos)
	{
		if (GameSceneController.Instance.Enemy_Set.Count > 0)
		{
			foreach (int key in GameSceneController.Instance.Enemy_Set.Keys)
			{
				if (GameSceneController.Instance.Enemy_Set[key].SqrDistanceFromPlayer < 300f)
				{
					Vector3 position = GameSceneController.Instance.Enemy_Set[key].transform.position;
					Vector3 position2 = GameSceneController.Instance.main_camera.GetComponent<Camera>().WorldToScreenPoint(position);
					Vector3 vector = GameSceneController.Instance.tui_camera.GetComponent<Camera>().ScreenToWorldPoint(position2);
					arrowPos = new Vector3(vector.x, vector.y + 50f, -5f);
					return true;
				}
			}
		}
		return false;
	}

	private bool CouldHuntBox()
	{
		if (GameSceneController.Instance.Enemy_Set.Count == 0)
		{
			return true;
		}
		return false;
	}
}
