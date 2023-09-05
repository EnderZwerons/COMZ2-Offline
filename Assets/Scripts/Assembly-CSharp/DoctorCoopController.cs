using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class DoctorCoopController : PlayerCoopController
{
	public const string ANI_ENCHANT = "UpperBody_Avatar_Doctor_Skill01";

	public PlayerState ENCHANT_SKILL_STATE;

	public GameObject enchant_gun_ref;

	protected GameObject enchant_gun_obj;

	protected Transform enchant_gun_fire_ori;

	public GameObject enchant_bullet_ref;

	public GameObject enchant_eff_ref;

	protected override void Awake()
	{
		base.Awake();
		ENCHANT_SKILL_STATE = PlayerState.Create(PlayerStateType.Enchant, this, true);
		if (enchant_gun_ref != null)
		{
			enchant_gun_obj = Object.Instantiate(enchant_gun_ref) as GameObject;
			Transform parent = base.transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 Prop1");
			enchant_gun_obj.transform.parent = parent;
			enchant_gun_obj.transform.localPosition = Vector3.zero;
			enchant_gun_obj.transform.localRotation = Quaternion.identity;
			enchant_gun_fire_ori = enchant_gun_obj.transform.Find("fire_ori");
		}
		HideEnchantGun();
	}

	protected override void ResetRenderSet()
	{
		List<Transform> list = new List<Transform>();
		ShaderColorFlash shaderColorFlash = null;
		list.Add(base.transform.Find("Avatar_Doctor_H_Body"));
		list.Add(base.transform.Find("Avatar_Doctor_H_Head"));
		list.Add(base.transform.Find("Avatar_Doctor_H_Neck"));
		list.Add(base.transform.Find("Avatar_Doctor_H_01"));
		foreach (Transform item in list)
		{
			if (item != null && item.GetComponent<Renderer>() != null)
			{
				shaderColorFlash = item.gameObject.AddComponent<ShaderColorFlash>();
				shaderColorFlash.start_color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				shaderColorFlash.end_color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 100);
				shaderColorFlash.flash_interval = 0.3f;
				avatar_render_set.Add(item.GetComponent<Renderer>());
			}
		}
	}

	public void EnchantFire()
	{
		Debug.Log("EnchantFire...");
		Vector2 fireOffset = GameSceneController.Instance.SightBead.GetFireOffset();
		Vector3 sightScreenPos = GameSceneController.Instance.GetSightScreenPos();
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(sightScreenPos.x + fireOffset.x, sightScreenPos.y + fireOffset.y, 100f));
		Ray ray = new Ray(Camera.main.transform.position, vector - Camera.main.transform.position);
		RaycastHit raycastHit = default(RaycastHit);
		RaycastHit[] array = Physics.RaycastAll(ray, 1000f, (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.DYNAMIC_SCENE) | (1 << PhysicsLayer.ANIMATION_SCENE) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD) | (1 << PhysicsLayer.WOOD_BOX));
		float num = 1000000f;
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit2 = array2[i];
			Vector3 zero = Vector3.zero;
			if (((raycastHit2.collider.gameObject.layer == PhysicsLayer.ENEMY) ? base.transform.InverseTransformPoint(raycastHit2.transform.position) : ((raycastHit2.collider.gameObject.layer != PhysicsLayer.PLAYER) ? base.transform.InverseTransformPoint(raycastHit2.point) : base.transform.InverseTransformPoint(raycastHit2.collider.transform.position))).z > 0f)
			{
				float sqrMagnitude = (raycastHit2.point - base.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					raycastHit = raycastHit2;
					num = sqrMagnitude;
				}
			}
		}
		if (raycastHit.collider != null && raycastHit.collider.gameObject.layer == PhysicsLayer.ENEMY)
		{
			EnemyController component = raycastHit.collider.GetComponent<EnemyController>();
			if (component != null)
			{
				SkillEnchantController skillEnchantController = skill_set["Enchant"] as SkillEnchantController;
				if (skillEnchantController.EnableEnchantMonst(component.enemy_data.enemy_type))
				{
					GameObject gameObject = Object.Instantiate(enchant_eff_ref, component.centroid, Quaternion.identity) as GameObject;
					gameObject.transform.parent = component.gameObject.transform;
				}
				else
				{
					Debug.Log(string.Concat("Enchant monster:", component.enemy_data.enemy_type, " failed."));
				}
			}
		}
		if (enchant_gun_fire_ori.InverseTransformPoint(GameSceneController.Instance.main_camera.target.position).z > 1f)
		{
			GameObject gameObject2 = Object.Instantiate(enchant_bullet_ref, enchant_gun_fire_ori.position, enchant_gun_fire_ori.rotation) as GameObject;
			FireLineScript component2 = gameObject2.GetComponent<FireLineScript>();
			component2.Init(enchant_gun_fire_ori.position, vector);
		}
	}

	public void ShowEnchantGun()
	{
		enchant_gun_obj.SetActive(true);
	}

	public void HideEnchantGun()
	{
		enchant_gun_obj.SetActive(false);
	}
}
