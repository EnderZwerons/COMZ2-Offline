using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarData
{
	public enum AvatarState
	{
		Normal = 1,
		Strong,
		Super
	}

	public string show_name = string.Empty;

	public string avatar_name = string.Empty;

	public AvatarType avatar_type = AvatarType.None;

	public string primary_equipment = "None";

	public float move_speed;

	public float hp_capacity;

	public float cur_hp;

	public float damage_val;

	public float armor_val;

	public float hp_capacity_next;

	public float damage_val_next;

	public float armor_val_next;

	public float reload_speed_val;

	public int level = 1;

	public GameDataInt cur_exp = new GameDataInt(0);

	public GameDataInt next_level_exp = new GameDataInt(0);

	public AvatarExistState exist_state;

	public AvatarConfig config;

	public int hp_level = 1;

	public int damage_level = 1;

	public int armor_level = 1;

	public List<string> skill_list = new List<string>();

	public int avatar_worth;

	public AvatarState avatar_state = AvatarState.Normal;

	public int UpgradePrice
	{
		get
		{
			if (level >= config.max_level)
			{
				return 0;
			}
			float paraA = GameConfig.Instance.Avatar_Up_Price_Info.ParaA;
			float paraB = GameConfig.Instance.Avatar_Up_Price_Info.ParaB;
			float paraK = GameConfig.Instance.Avatar_Up_Price_Info.ParaK;
			float f = level - 1;
			return (int)((paraA * Mathf.Pow(f, paraK) + paraB) * (float)config.up_price_ratio);
		}
	}

	public int UpgradeArmorPrice
	{
		get
		{
			return GetUpgradeArmorPrice(armor_level);
		}
	}

	public int UpgradeHpPrice
	{
		get
		{
			return GetUpgradeHpPrice(hp_level);
		}
	}

	public int UpgradeDamagePrice
	{
		get
		{
			return GetUpgradeDamagePrice(damage_level);
		}
	}

	public void ResetData()
	{
		float num = level - 1;
		move_speed = config.speed_conf.base_data + num * (config.speed_conf.max_data - config.speed_conf.base_data) / (float)(config.max_level - 1);
		num = damage_level - 1;
		damage_val = config.damage_conf.base_data + num * (config.damage_conf.max_data - config.damage_conf.base_data) / (float)(config.max_level - 1);
		int num2 = armor_level - 1;
		armor_val = config.armor_conf.base_data + (float)num2 * (config.armor_conf.max_data - config.armor_conf.base_data) / (float)(config.max_level - 1);
		float paraA = GameConfig.Instance.Avatar_Hp_Up_Info.ParaA;
		float paraB = GameConfig.Instance.Avatar_Hp_Up_Info.ParaB;
		float paraC = GameConfig.Instance.Avatar_Hp_Up_Info.ParaC;
		if (hp_level == 1)
		{
			cur_hp = (hp_capacity = config.hp_conf.base_data);
		}
		else
		{
			cur_hp = (hp_capacity = config.hp_conf.base_data + (paraA * Mathf.Pow(hp_level, 2f) + paraB * (float)hp_level + paraC) * config.hp_ratio);
		}
		reload_speed_val = config.reload_ratio;
		if (hp_level < config.max_level)
		{
			int num3 = hp_level + 1;
			hp_capacity_next = config.hp_conf.base_data + (paraA * Mathf.Pow(num3, 2f) + paraB * (float)num3 + paraC) * config.hp_ratio;
		}
		else
		{
			hp_capacity_next = hp_capacity;
		}
		if (damage_level < config.max_level)
		{
			num = damage_level - 1 + 1;
			damage_val_next = config.damage_conf.base_data + num * (config.damage_conf.max_data - config.damage_conf.base_data) / (float)(config.max_level - 1);
		}
		else
		{
			armor_val_next = armor_val;
		}
		if (armor_level < config.max_level)
		{
			num = armor_level - 1 + 1;
			armor_val_next = config.armor_conf.base_data + num * (config.armor_conf.max_data - config.armor_conf.base_data) / (float)(config.max_level - 1);
		}
		else
		{
			armor_val_next = armor_val;
		}
		avatar_worth = 0;
		for (int i = 1; i <= hp_level - 1; i++)
		{
			avatar_worth += GetUpgradeHpPrice(i);
		}
		for (int j = 1; j <= damage_level - 1; j++)
		{
			avatar_worth += GetUpgradeDamagePrice(j);
		}
		for (int k = 1; k <= armor_level - 1; k++)
		{
			avatar_worth += GetUpgradeArmorPrice(k);
		}
		if (avatar_worth >= config.avatar_worth_2)
		{
			avatar_state = AvatarState.Super;
		}
		else if (avatar_worth >= config.avatar_worth_1)
		{
			avatar_state = AvatarState.Strong;
		}
		else
		{
			avatar_state = AvatarState.Normal;
		}
	}

	public bool Upgrade()
	{
		if (level < config.max_level && GameData.Instance.total_voucher >= UpgradePrice)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("AvartarName", config.avatar_name);
			hashtable.Add("AvartarLevel", level + 1);
			hashtable.Add("AvartarPrice", UpgradePrice);
			GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Avartar", hashtable);
			GameData.Instance.total_voucher -= UpgradePrice;
			level++;
			hp_level++;
			damage_level++;
			ResetData();
			CheckUnlockSecondaryWeapon();
			return true;
		}
		return false;
	}

	public bool UpgradeArmor()
	{
		if (armor_level < config.max_level && GameData.Instance.total_voucher >= UpgradeArmorPrice)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("AvartarName", config.avatar_name);
			hashtable.Add("Level", armor_level + 1);
			hashtable.Add("Price", UpgradeArmorPrice);
			GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Avartar_Armor", hashtable);
			GameData.Instance.total_voucher -= UpgradeArmorPrice;
			armor_level++;
			ResetData();
			GameData.Instance.SaveData();
			return true;
		}
		return false;
	}

	public bool UpgradeHp()
	{
		if (hp_level < config.max_level && GameData.Instance.total_voucher >= UpgradeHpPrice)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("AvartarName", config.avatar_name);
			hashtable.Add("Level", hp_level + 1);
			hashtable.Add("Price", UpgradeHpPrice);
			GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Avartar_Hp", hashtable);
			GameData.Instance.total_voucher -= UpgradeHpPrice;
			hp_level++;
			ResetData();
			GameData.Instance.SaveData();
			return true;
		}
		return false;
	}

	public bool UpgradeDamage()
	{
		if (damage_level < config.max_level && GameData.Instance.total_voucher >= UpgradeDamagePrice)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("AvartarName", config.avatar_name);
			hashtable.Add("Level", damage_level + 1);
			hashtable.Add("Price", UpgradeDamagePrice);
			GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Avartar_Damage", hashtable);
			GameData.Instance.total_voucher -= UpgradeDamagePrice;
			damage_level++;
			ResetData();
			GameData.Instance.SaveData();
			return true;
		}
		return false;
	}

	public bool AddExp(int exp)
	{
		if (level >= config.max_level)
		{
			return false;
		}
		cur_exp += exp;
		if (cur_exp >= next_level_exp.GetIntVal())
		{
			cur_exp -= next_level_exp.GetIntVal();
			Upgrade();
			return true;
		}
		return false;
	}

	public bool EquipWeapon(WeaponData weapon)
	{
		if (weapon.exist_state != WeaponExistState.Owned)
		{
			return false;
		}
		if (!weapon.is_secondary)
		{
			primary_equipment = weapon.weapon_name;
			return true;
		}
		if (weapon.owner == avatar_type)
		{
			return true;
		}
		return false;
	}

	public void Injured(float damage)
	{
		float num = damage * (1f - armor_val / 100f);
		if (num < 0f)
		{
			num = 0f;
		}
		cur_hp -= num;
	}

	public void CheckUnlockSecondaryWeapon()
	{
		foreach (int key in config.Second_Weapon_Cfg.Keys)
		{
			if (key != level)
			{
			}
		}
	}

	public bool Unlock()
	{
		if (exist_state == AvatarExistState.Locked)
		{
			exist_state = AvatarExistState.Unlocked;
			return true;
		}
		return false;
	}

	public bool Buy()
	{
		if (exist_state == AvatarExistState.Unlocked)
		{
			if (config.is_voucher_avatar)
			{
				if (GameData.Instance.total_voucher >= config.price)
				{
					Hashtable hashtable = new Hashtable();
					hashtable.Add("AvartarName", config.avatar_name);
					hashtable.Add("AvartarPrice", config.price);
					GameData.Instance.UploadStatistics("Vouchers_Use_Buy_Avartar", hashtable);
					GameData.Instance.total_voucher -= config.price;
					exist_state = AvatarExistState.Owned;
					GameData.Instance.SaveData();
					return true;
				}
			}
			else if (GameData.Instance.total_cash >= config.price)
			{
				GameData.Instance.total_cash -= config.price;
				exist_state = AvatarExistState.Owned;
				GameData.Instance.SaveData();
				return true;
			}
		}
		return false;
	}

	public float GetVatarHpWithLv(int lv)
	{
		int num = lv - 1;
		return config.hp_conf.base_data + (float)num * (config.hp_conf.max_data - config.hp_conf.base_data) / (float)(config.max_level - 1);
	}

	public bool CrystalUnlock()
	{
		if (exist_state == AvatarExistState.Locked)
		{
			int intVal = config.crystal_unlock_price.GetIntVal();
			if (intVal > 0 && GameData.Instance.total_crystal >= intVal)
			{
				GameData.Instance.total_crystal -= intVal;
				exist_state = AvatarExistState.Owned;
				Hashtable hashtable = new Hashtable();
				hashtable.Add("AvartarName", config.avatar_name);
				hashtable.Add("AvartarPrice", config.price);
				GameData.Instance.UploadStatistics("tCrystal_Use_Buy_Avartar", hashtable);
				GameData.Instance.SaveData();
				return true;
			}
			return false;
		}
		return false;
	}

	private int GetUpgradeHpPrice(int level)
	{
		if (level >= config.max_level)
		{
			return 0;
		}
		float paraA = GameConfig.Instance.Avatar_Up_Hp_Price_Info.ParaA;
		float paraB = GameConfig.Instance.Avatar_Up_Hp_Price_Info.ParaB;
		float paraK = GameConfig.Instance.Avatar_Up_Hp_Price_Info.ParaK;
		float f = level - 1;
		return (int)((paraA * Mathf.Pow(f, paraK) + paraB) * config.up_hp_price_ratio);
	}

	private int GetUpgradeArmorPrice(int level)
	{
		if (level >= config.max_level)
		{
			return 0;
		}
		float paraA = GameConfig.Instance.Avatar_Up_Armor_Price_Info.ParaA;
		float paraB = GameConfig.Instance.Avatar_Up_Armor_Price_Info.ParaB;
		float paraK = GameConfig.Instance.Avatar_Up_Armor_Price_Info.ParaK;
		float f = level - 1;
		return (int)((paraA * Mathf.Pow(f, paraK) + paraB) * config.up_armor_price_ratio);
	}

	private int GetUpgradeDamagePrice(int level)
	{
		if (level >= config.max_level)
		{
			return 0;
		}
		float paraA = GameConfig.Instance.Avatar_Up_Damage_Price_Info.ParaA;
		float paraB = GameConfig.Instance.Avatar_Up_Damage_Price_Info.ParaB;
		float paraK = GameConfig.Instance.Avatar_Up_Damage_Price_Info.ParaK;
		float f = level - 1;
		return (int)((paraA * Mathf.Pow(f, paraK) + paraB) * config.up_damage_price_ratio);
	}
}
