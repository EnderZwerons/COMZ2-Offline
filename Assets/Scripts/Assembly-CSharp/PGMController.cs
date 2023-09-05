using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class PGMController : WeaponController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	public GameObject fire_line_obj;

	public GameObject fire_smoke_obj;

	private float cur_auto_lock_time;

	private float auto_lock_time_interval = 0.4f;

	protected GameObject pgm_sight;

	public Dictionary<NearestTargetInfo, GameObject> auto_lock_target_dir = new Dictionary<NearestTargetInfo, GameObject>();

	private float explode_radius;

	private TNetObject tnetObj;

	private void Awake()
	{
		weapon_type = WeaponType.PGM;
	}

	private void Start()
	{
		fire_ori = base.transform.Find("fire_ori");
		if (fire_ori == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find fire_ori!"));
		}
		fire_line_obj = Object.Instantiate(Accessory[1]) as GameObject;
		fire_line_obj.transform.parent = fire_ori;
		fire_line_obj.transform.localPosition = Vector3.zero;
		fire_line_obj.transform.localRotation = Quaternion.identity;
		fire_line_obj.GetComponent<ParticleSystem>().Stop();
		fire_smoke_obj = Object.Instantiate(Accessory[2]) as GameObject;
		fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
		explode_radius = (float)weapon_data.config.Ex_conf["explodeRange"];
		pgm_sight = Accessory[3];
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		//{
		//	tnetObj = TNetConnection.Connection;
		//}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void ResetFireInterval()
	{
		base.ResetFireInterval();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		ANI_IDLE_RUN = "RPG_UpperBody_Run01";
		ANI_IDLE_SHOOT = "RPG_UpperBody_Idle01";
		ANI_SHOOT = "RPG_UpperBody_Shooting01";
		ANI_RELOAD = "RPG_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "RPG_UpperBody_Shiftweapon01";
		ANI_IDLE_DOWN = "RPG_LowerBody_Idle01";
		ANI_SHOOT_DOWN = "RPG_LowerBody_Shooting01";
		ANI_RELOAD_DOWN = "RPG_LowerBody_Reload01";
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		ResetReloadAniSpeed(player);
		ResetFireAniSpeed(player);
		render_obj = base.gameObject;
		base.SetWeaponAni(player, spine);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		render_obj = base.gameObject;
	}

	public override void FireUpdate(PlayerController player, float deltaTime)
	{
		if (GameSceneController.Instance.main_camera != null)
		{
			GameSceneController.Instance.main_camera.ZoomIn(deltaTime);
		}
		if (CouldMakeNextShoot())
		{
			AutoLock(player, deltaTime);
		}
	}

	public void AutoLock(PlayerController player, float deltaTime)
	{
		if (!weapon_data.EnableFire())
		{
			return;
		}
		cur_auto_lock_time += deltaTime;
		foreach (NearestTargetInfo key in auto_lock_target_dir.Keys)
		{
			if (key != null)
			{
				if (key.target_obj == null)
				{
					key.enabel = false;
					GameSceneController.Instance.pgm_sight_pool.GetComponent<ObjectPool>().DeleteObject(auto_lock_target_dir[key]);
				}
				else if (key.type == NearestTargetInfo.NearestTargetType.Enemy && ((EnemyController)key.target_obj).Enemy_State.GetStateType() == EnemyStateType.Dead)
				{
					key.target_obj = null;
					key.enabel = false;
					GameSceneController.Instance.pgm_sight_pool.GetComponent<ObjectPool>().DeleteObject(auto_lock_target_dir[key]);
				}
			}
		}
		foreach (NearestTargetInfo key2 in auto_lock_target_dir.Keys)
		{
			if (key2 != null && key2.enabel)
			{
				Vector2 vector = Camera.main.WorldToScreenPoint(key2.LockPosition);
				vector = (key2.screenPos = GameSceneController.Instance.tui_camera.GetComponent<Camera>().ScreenToWorldPoint(vector));
			}
		}
		if (cur_auto_lock_time >= auto_lock_time_interval)
		{
			cur_auto_lock_time = 0f;
			List<NearestTargetInfo> list = new List<NearestTargetInfo>();
			NearestTargetInfo nearestTargetInfo = null;
			foreach (NearestTargetInfo key3 in auto_lock_target_dir.Keys)
			{
				if (!key3.enabel)
				{
					list.Add(key3);
				}
				else if (key3 != null && key3.target_obj != null)
				{
					Vector3 position = Camera.main.WorldToScreenPoint(key3.LockPosition);
					Vector3 vector2 = GameSceneController.Instance.tui_camera.GetComponent<Camera>().ScreenToWorldPoint(position);
					if (position.z < 0f || !GameSceneController.Instance.PGM_Lock_Rect.PtInControl(new Vector2(vector2.x, vector2.y)) || GameSceneController.CheckBlockBetween(key3.LockPosition, Camera.main.transform.position))
					{
						list.Add(key3);
					}
				}
			}
			foreach (NearestTargetInfo item in list)
			{
				GameSceneController.Instance.pgm_sight_pool.GetComponent<ObjectPool>().DeleteObject(auto_lock_target_dir[item]);
				auto_lock_target_dir.Remove(item);
			}
			if (nearestTargetInfo == null)
			{
				float num = 99999f;
				foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
				{
					if (value.Enemy_State.GetStateType() == EnemyStateType.Dead || ContainTarget(value))
					{
						continue;
					}
					Vector3 position2 = Camera.main.WorldToScreenPoint(value.centroid);
					if (position2.z < 0f)
					{
						continue;
					}
					Vector3 vector3 = GameSceneController.Instance.tui_camera.GetComponent<Camera>().ScreenToWorldPoint(position2);
					if (GameSceneController.Instance.PGM_Lock_Rect.PtInControl(new Vector2(vector3.x, vector3.y)))
					{
						float magnitude = (value.centroid - Camera.main.transform.position).magnitude;
						if (magnitude < num && !GameSceneController.CheckBlockBetween(value.centroid, Camera.main.transform.position))
						{
							num = magnitude;
							nearestTargetInfo = new NearestTargetInfo();
							nearestTargetInfo.type = NearestTargetInfo.NearestTargetType.Enemy;
							nearestTargetInfo.transform = value.transform;
							nearestTargetInfo.screenPos = vector3;
							nearestTargetInfo.target_obj = value;
							nearestTargetInfo.enabel = true;
							GameObject gameObject = GameSceneController.Instance.pgm_sight_pool.GetComponent<ObjectPool>().CreateObject();
							gameObject.transform.parent = GameSceneController.Instance.game_main_panel.transform;
							gameObject.transform.localPosition = nearestTargetInfo.screenPos;
							auto_lock_target_dir.Add(nearestTargetInfo, gameObject);
							player.PlayPlayerAudio("AutoLock");
							break;
						}
					}
				}
				foreach (GameObject item2 in GameSceneController.Instance.wood_box_list)
				{
					WoodBoxController component = item2.GetComponent<WoodBoxController>();
					if (component == null || component.Broken || ContainTarget(component))
					{
						continue;
					}
					Vector3 position3 = Camera.main.WorldToScreenPoint(component.centroid);
					if (position3.z < 0f)
					{
						continue;
					}
					Vector3 vector4 = GameSceneController.Instance.tui_camera.GetComponent<Camera>().ScreenToWorldPoint(position3);
					if (GameSceneController.Instance.PGM_Lock_Rect.PtInControl(new Vector2(vector4.x, vector4.y)))
					{
						float magnitude2 = (component.centroid - Camera.main.transform.position).magnitude;
						if (magnitude2 < num && !GameSceneController.CheckBlockBetween(component.centroid, Camera.main.transform.position))
						{
							num = magnitude2;
							nearestTargetInfo = new NearestTargetInfo();
							nearestTargetInfo.type = NearestTargetInfo.NearestTargetType.Box;
							nearestTargetInfo.transform = component.transform;
							nearestTargetInfo.screenPos = vector4;
							nearestTargetInfo.target_obj = component;
							nearestTargetInfo.enabel = true;
							GameObject gameObject2 = GameSceneController.Instance.pgm_sight_pool.GetComponent<ObjectPool>().CreateObject();
							gameObject2.transform.parent = GameSceneController.Instance.game_main_panel.transform;
							gameObject2.transform.localPosition = nearestTargetInfo.screenPos;
							auto_lock_target_dir.Add(nearestTargetInfo, gameObject2);
							player.PlayPlayerAudio("AutoLock");
							break;
						}
					}
				}
			}
		}
		foreach (NearestTargetInfo key4 in auto_lock_target_dir.Keys)
		{
			if (key4 != null && key4.enabel)
			{
				auto_lock_target_dir[key4].transform.localPosition = new Vector3(key4.screenPos.x, key4.screenPos.y, 0f);
			}
		}
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		if (weapon_data.EnableFire() && AimedTarget())
		{
			base.Fire(player, deltaTime);
			fireable = true;
			AnimationUtil.Stop(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState));
			AnimationUtil.PlayAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.ClampForever);
			if (player.avatar_data.avatar_type == AvatarType.Cowboy)
			{
				AnimationUtil.Stop(player.CowboyCap);
				AnimationUtil.PlayAnimate(player.CowboyCap, "RPG_Shooting01", WrapMode.Once);
			}
		}
	}

	public override void CheckHit(ObjectController controller)
	{
		if (!fireable)
		{
			return;
		}
		fireable = false;
		fire_line_obj.GetComponent<ParticleSystem>().Stop();
		fire_line_obj.GetComponent<ParticleSystem>().Play();
		fire_smoke_obj.transform.position = base.transform.position;
		fire_smoke_obj.transform.rotation = base.transform.rotation;
		fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
		fire_smoke_obj.GetComponent<ParticleSystem>().Play();
		GameSceneController.Instance.SightBead.Stretch(weapon_data.config.recoil);
		SFSArray sFSArray = new SFSArray();
		foreach (NearestTargetInfo key in auto_lock_target_dir.Keys)
		{
			if (key != null && key.enabel)
			{
				if (!weapon_data.OnFire())
				{
					break;
				}
				Vector3 normalized = (key.LockPosition - fire_ori.position).normalized;
				GameObject gameObject = Object.Instantiate(Accessory[0], base.transform.position, Quaternion.LookRotation(normalized)) as GameObject;
				PGMProjectile component = gameObject.GetComponent<PGMProjectile>();
				component.launch_dir = normalized;
				component.fly_speed = 20f;
				component.explode_radius = explode_radius;
				component.life = 10f;
				component.damage = weapon_data.damage_val;
				component.object_controller = controller;
				component.weapon_controller = this;
				component.targetPos = key.LockPosition;
				component.target_trans = key.transform;
				component.target_info = key;
				//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
				//{
				//	SFSArray sFSArray2 = new SFSArray();
				//	sFSArray2.AddFloat(component.targetPos.x);
				//	sFSArray2.AddFloat(component.targetPos.y);
				//	sFSArray2.AddFloat(component.targetPos.z);
				//	sFSArray.AddSFSArray(sFSArray2);
				//}
			}
		}
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null && sFSArray.Size() > 0)
		//{
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutSFSArray("pgmFire", sFSArray);
		//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//}
		CleanLockTargets();
	}

	public override void GunOn()
	{
		base.GunOn();
	}

	public override void GunOff()
	{
		base.GunOff();
		CleanLockTargets();
	}

	protected bool ContainTarget(ObjectController target_obj)
	{
		bool result = false;
		foreach (NearestTargetInfo key in auto_lock_target_dir.Keys)
		{
			if (key != null && key.target_obj != null && target_obj == key.target_obj)
			{
				return true;
			}
		}
		return result;
	}

	protected bool AimedTarget()
	{
		bool result = false;
		foreach (NearestTargetInfo key in auto_lock_target_dir.Keys)
		{
			if (key != null && key.enabel)
			{
				return true;
			}
		}
		return result;
	}

	public override void OnFireRelease(PlayerController player)
	{
		Fire(player, Time.deltaTime);
	}

	public void CleanLockTargets()
	{
		auto_lock_target_dir.Clear();
		cur_auto_lock_time = auto_lock_time_interval;
		if (GameSceneController.Instance != null && GameSceneController.Instance.pgm_sight_pool != null)
		{
			GameSceneController.Instance.pgm_sight_pool.GetComponent<ObjectPool>().AutoDestructAll();
		}
	}
}
