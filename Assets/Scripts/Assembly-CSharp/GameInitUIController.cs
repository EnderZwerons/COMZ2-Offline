using System.Collections;
using CoMZ2;
using UnityEngine;

public class GameInitUIController : MonoBehaviour
{
	private void Awake()
	{
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
	}

	private IEnumerator Start()
	{
		OpenClikPlugin.Initialize("A36F6C65-C1E3-47D4-AD07-AA8A6C90132C");
		yield return 1;
		yield return 1;
//		Handheld.PlayFullScreenMovie("GameStory.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
		yield return 1;
		PushNotification.ReSetNotifications();
		yield return 1;
		if (GameData.Instance.showUpdatePoster)
		{
			Application.LoadLevel("UpdatePoster");
			GameData.Instance.showUpdatePoster = false;
		}
		else if (GameData.Instance.is_enter_tutorial)
		{
			GameData.Instance.cur_quest_info.mission_type = MissionType.Tutorial;
			GameData.Instance.cur_quest_info.mission_day_type = MissionDayType.Tutorial;
			GameData.Instance.loading_to_scene = "GameTutorial";
			Application.LoadLevel("Loading");
		}
		else
		{
			Application.LoadLevel("GameCover");
		}
		yield return 1;
	}
}
