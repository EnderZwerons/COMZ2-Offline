using CoMZ2;

public class EnemyData
{
	public string enemy_name = string.Empty;

	public EnemyType enemy_type = EnemyType.E_NONE;

	public float move_speed;

	public float hp_capacity;

	public float cur_hp;

	public float damage_val;

	public float frequency_val;

	public float attack_range;

	public float view_range;

	public EnemyAttackPriority attack_priority;

	public int loot_cash;

	public int exp;

	public EnemyConfig config;

	public void InitData(EnemyConfig econfig)
	{
		config = econfig;
		enemy_type = econfig.enemy_type;
		move_speed = config.speed_val;
		attack_range = config.attack_range;
		view_range = config.view_range;
		frequency_val = config.attack_frequency;
		attack_priority = config.attack_priority;
		if (GameSceneController.Instance.mission_day_type == MissionDayType.Tutorial)
		{
			cur_hp = (hp_capacity = 15f);
		}
		else
		{
			cur_hp = (hp_capacity = GameSceneController.Instance.enemy_standard_hp * config.hp_ratio);
		}
		damage_val = GameSceneController.Instance.enemy_standard_dps * config.damage_ratio;
		loot_cash = (exp = (int)(GameSceneController.Instance.enemy_standard_reward * config.reward_ratio));
	}

	public static EnemyData CreateData(EnemyConfig config)
	{
		EnemyData enemyData = new EnemyData();
		enemyData.InitData(config);
		return enemyData;
	}

	public bool OnInjured(float damage)
	{
		if (damage > 0f)
		{
			cur_hp -= damage;
			if (cur_hp <= 0f)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
