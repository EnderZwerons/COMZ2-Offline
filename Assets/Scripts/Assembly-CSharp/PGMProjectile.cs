using CoMZ2;
using UnityEngine;

public class PGMProjectile : ProjectileController
{
	public NearestTargetInfo target_info;

	public Transform target_trans;

	public Vector3 targetPos = Vector3.zero;

	public float initAngel = 40f;

	public override void OnProjectileCollideEnter(GameObject obj)
	{
		MineProjectile.ChainBoom(centroid, explode_radius);
		if (obj.layer == PhysicsLayer.PLAYER)
		{
			ObjectController component = obj.GetComponent<ObjectController>();
			if (component != null)
			{
				component.OnHit(0f, null, object_controller, component.centroid, centroid - component.centroid);
			}
		}
		else if (obj.layer == PhysicsLayer.DYNAMIC_SCENE)
		{
			if (obj.GetComponent<Rigidbody>() == null)
			{
				obj.AddComponent<Rigidbody>();
			}
			Vector3 force = (obj.transform.position - base.transform.position).normalized * 100f;
			Rigidbody component2 = obj.GetComponent<Rigidbody>();
			component2.mass = 5f;
			component2.drag = 1.5f;
			component2.angularDrag = 1.5f;
			component2.AddForceAtPosition(force, obj.transform.position, ForceMode.Impulse);
			obj.layer = PhysicsLayer.WALL;
			obj.AddComponent<RemoveTimerScript>().life = 3f;
		}
		else if (obj.layer == PhysicsLayer.ANIMATION_SCENE)
		{
			GameObject gameObject = obj.transform.parent.gameObject;
			if (gameObject.GetComponent<Animation>() != null)
			{
				gameObject.GetComponent<Animation>()["Take 001"].clip.wrapMode = WrapMode.Once;
				gameObject.GetComponent<Animation>().Play("Take 001");
			}
		}
		PlayerController playerController = object_controller as PlayerController;
		bool flag = false;
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			if ((value.centroid - centroid).sqrMagnitude < explode_radius * explode_radius && !GameSceneController.CheckBlockBetween(centroid, value.centroid))
			{
				value.OnHit(damage, null, object_controller, value.centroid, centroid - value.centroid);
				flag = true;
			}
		}
		foreach (GameObject item in GameSceneController.Instance.wood_box_list)
		{
			WoodBoxController component3 = item.GetComponent<WoodBoxController>();
			if (component3 != null && (component3.centroid - centroid).sqrMagnitude < explode_radius * explode_radius && !GameSceneController.CheckBlockBetween(centroid, component3.centroid))
			{
				flag = true;
				component3.OnHit(damage, null, object_controller, component3.centroid, centroid - component3.centroid);
			}
		}
		if (flag && weapon_controller != null)
		{
			playerController.AddComboValue(weapon_controller.weapon_data.config.combo_base);
		}
		GameSceneController.Instance.boom_m_pool.GetComponent<ObjectPool>().CreateObject(centroid, Quaternion.identity);
		Object.DestroyObject(base.gameObject);
	}

	public override void OnProjectileCollideStay(GameObject obj)
	{
	}

	public override void UpdateTransform(float deltaTime)
	{
		if (target_info != null && target_trans != null)
		{
			targetPos = target_info.LockPosition;
			base.transform.LookAt(targetPos);
			initAngel -= deltaTime * 80f;
			if (initAngel <= 0f)
			{
				initAngel = 0f;
			}
			base.transform.rotation = Quaternion.AngleAxis(initAngel, -1f * base.transform.right) * base.transform.rotation;
			base.transform.Rotate(base.transform.forward, Time.time * 10f, Space.World);
			launch_dir = base.transform.forward;
			base.transform.Translate(fly_speed * launch_dir * deltaTime, Space.World);
			if (target_info.type == NearestTargetInfo.NearestTargetType.Enemy)
			{
				EnemyController enemyController = target_info.target_obj as EnemyController;
				if (enemyController != null && enemyController.Enemy_State.GetStateType() == EnemyStateType.Dead)
				{
					Object.DestroyObject(base.gameObject);
				}
			}
		}
		else
		{
			Object.DestroyObject(base.gameObject);
		}
	}
}
