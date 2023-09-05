using UnityEngine;

namespace CoMZ2
{
	public class HalloweenReplicationState : EnemyState
	{
		protected HalloweenController halloween;

		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(halloween.gameObject, halloween.ANI_REPLICATION_01, 1f))
			{
				halloween.SetState(halloween.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (halloween == null)
			{
				halloween = m_enemy as HalloweenController;
			}
			halloween.SetPathCatchState(false);
			AnimationUtil.CrossAnimate(halloween.gameObject, halloween.ANI_REPLICATION_01, WrapMode.ClampForever);
			halloween.replication_enable = false;
			halloween.OnReplicationCast();
		}

		public override void OnExitState()
		{
		}
	}
}
