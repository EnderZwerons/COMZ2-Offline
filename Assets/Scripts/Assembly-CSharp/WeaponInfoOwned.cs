using CoMZ2;
using UnityEngine;

public class WeaponInfoOwned : MonoBehaviour
{
	public TUIButtonClick EquipButton;

	public WeaponProperty[] Properties;

	public WeaponProperty Ammo;

	private WeaponData weaponData;

	private bool showUpgrade;

	public void UpdateInfo(WeaponData data)
	{
		weaponData = data;
		showUpgrade = weaponData.weapon_name == UIShopSceneController.Instance.CurrentAvatar.avatarData.primary_equipment;
		if (showUpgrade)
		{
			EquipButton.gameObject.SetActive(false);
			Ammo.gameObject.SetActive(true);
			UpdateAmmo();
		}
		else
		{
			EquipButton.gameObject.SetActive(true);
			Ammo.gameObject.SetActive(false);
		}
		CheckWeaponProperty();
	}

	private void CheckWeaponProperty()
	{
		WeaponProperty[] properties = Properties;
		foreach (WeaponProperty weaponProperty in properties)
		{
			weaponProperty.Clear();
		}
		int num = 0;
		if (weaponData.damage_level >= weaponData.config.max_level || Mathf.Abs(weaponData.damage_val_next - weaponData.damage_val) > 1E-05f)
		{
			Properties[num].Show(WeaponPropertyType.Damage);
			UpdateProperty(num);
			num++;
		}
		if (weaponData.frequency_level >= weaponData.config.max_level || Mathf.Abs(weaponData.frequency_val_next - weaponData.frequency_val) > 1E-05f)
		{
			Properties[num].Show(WeaponPropertyType.Frequency);
			UpdateProperty(num);
			num++;
		}
		if (weaponData.stretch_level >= weaponData.config.max_level || Mathf.Abs(weaponData.stretch_max_next - weaponData.stretch_max) > 1E-05f)
		{
			Properties[num].Show(WeaponPropertyType.Accuracy);
			UpdateProperty(num);
			num++;
		}
		if (weaponData.clip_level >= weaponData.config.max_level || (float)Mathf.Abs(weaponData.clip_capacity_next - weaponData.clip_capacity) > 1E-05f)
		{
			Properties[num].Show(WeaponPropertyType.Reload);
			UpdateProperty(num);
			num++;
		}
		if (weaponData.range_level >= weaponData.config.max_level || Mathf.Abs(weaponData.range_val_next - weaponData.range_val) > 1E-05f)
		{
			Properties[num].Show(WeaponPropertyType.Range);
			UpdateProperty(num);
			num++;
		}
	}

	private void UpdateProperty(int propertyIdx)
	{
		WeaponProperty weaponProperty = Properties[propertyIdx];
		bool flag = false;
		string text = string.Empty;
		switch (weaponProperty.Type)
		{
		case WeaponPropertyType.Damage:
			weaponProperty.Name.Text = "Damage (LV " + weaponData.damage_level + ")";
			weaponProperty.Value.Text = weaponData.damage_val.ToString("N2");
			flag = weaponData.damage_level >= weaponData.config.max_level;
			weaponProperty.UpgradeValue.Text = ((!flag) ? ("+" + (weaponData.damage_val_next - weaponData.damage_val).ToString("N2")) : string.Empty);
			text = weaponData.UpgradeDamagePrice.ToString("G");
			break;
		case WeaponPropertyType.Frequency:
			weaponProperty.Name.Text = "Rate of Fire (LV " + weaponData.frequency_level + ")";
			weaponProperty.Value.Text = weaponData.frequency_val.ToString("N3");
			flag = weaponData.frequency_level >= weaponData.config.max_level;
			weaponProperty.UpgradeValue.Text = ((!flag) ? (weaponData.frequency_val_next - weaponData.frequency_val).ToString("N3") : string.Empty);
			text = weaponData.UpgradeFrequencyPrice.ToString("G");
			break;
		case WeaponPropertyType.Accuracy:
			weaponProperty.Name.Text = "Accuracy (LV " + weaponData.stretch_level + ")";
			weaponProperty.Value.Text = (100f - weaponData.stretch_max).ToString("N2") + "%";
			flag = weaponData.stretch_level >= weaponData.config.max_level;
			weaponProperty.UpgradeValue.Text = ((!flag) ? ("+" + (weaponData.stretch_max - weaponData.stretch_max_next).ToString("N2") + "%") : string.Empty);
			text = weaponData.UpgradeStretchPrice.ToString("G");
			break;
		case WeaponPropertyType.Reload:
			weaponProperty.Name.Text = "Magazine Capacity (LV " + weaponData.clip_level + ")";
			weaponProperty.Value.Text = weaponData.clip_capacity.ToString("N0");
			flag = weaponData.clip_level >= weaponData.config.max_level;
			weaponProperty.UpgradeValue.Text = ((!flag) ? ("+" + (weaponData.clip_capacity_next - weaponData.clip_capacity).ToString("N0")) : string.Empty);
			text = weaponData.UpgradeClipPrice.ToString("G");
			break;
		case WeaponPropertyType.Range:
			weaponProperty.Name.Text = "Range (LV " + weaponData.range_level + ")";
			weaponProperty.Value.Text = weaponData.range_val.ToString("N2");
			flag = weaponData.range_level >= weaponData.config.max_level;
			weaponProperty.UpgradeValue.Text = ((!flag) ? ("+" + (weaponData.range_val_next - weaponData.range_val).ToString("N2")) : string.Empty);
			text = weaponData.UpgradeRangePrice.ToString("G");
			break;
		}
		if (showUpgrade)
		{
			weaponProperty.UpgradeButton.gameObject.SetActive(true);
			if (flag)
			{
				weaponProperty.UpgradeButton.Disable(true);
				return;
			}
			weaponProperty.UpgradeButton.Disable(false);
			weaponProperty.UpgradeButton.gameObject.SetActive(true);
			weaponProperty.UpgradeButton.m_NormalLabelObj.GetComponent<TUILabel>().Text = text;
			weaponProperty.UpgradeButton.m_PressLabelObj.GetComponent<TUILabel>().Text = text;
		}
		else
		{
			weaponProperty.UpgradeButton.gameObject.SetActive(false);
		}
	}

	private void UpdateAmmo()
	{
		Ammo.Value.Text = weaponData.total_bullet_count.ToString("G");
		if (weaponData.total_bullet_count + weaponData.config.buy_bullet_count <= 9999)
		{
			Ammo.UpgradeValue.Text = "+" + weaponData.config.buy_bullet_count.ToString("G");
			Ammo.UpgradeButton.Disable(false);
			Ammo.UpgradeButton.m_NormalLabelObj.GetComponent<TUILabel>().Text = weaponData.config.bulletShopPrice.ToString();
			Ammo.UpgradeButton.m_PressLabelObj.GetComponent<TUILabel>().Text = weaponData.config.bulletShopPrice.ToString();
		}
		else
		{
			Ammo.UpgradeValue.Text = string.Empty;
			Ammo.UpgradeButton.Disable(true);
		}
	}

	public bool UpgradeProperty(int index, ref int extraCash)
	{
		WeaponPropertyType type = Properties[index].Type;
		bool flag = false;
		switch (type)
		{
		case WeaponPropertyType.Damage:
			flag = weaponData.UpgradeDamage();
			if (!flag)
			{
				extraCash = weaponData.UpgradeDamagePrice - GameData.Instance.total_cash.GetIntVal();
			}
			break;
		case WeaponPropertyType.Frequency:
			flag = weaponData.UpgradeFrequency();
			if (!flag)
			{
				extraCash = weaponData.UpgradeFrequencyPrice - GameData.Instance.total_cash.GetIntVal();
			}
			break;
		case WeaponPropertyType.Accuracy:
			flag = weaponData.UpgradeStretch();
			if (!flag)
			{
				extraCash = weaponData.UpgradeStretchPrice - GameData.Instance.total_cash.GetIntVal();
			}
			break;
		case WeaponPropertyType.Reload:
			flag = weaponData.UpgradeClip();
			if (!flag)
			{
				extraCash = weaponData.UpgradeClipPrice - GameData.Instance.total_cash.GetIntVal();
			}
			break;
		case WeaponPropertyType.Range:
			flag = weaponData.UpgradeRange();
			if (!flag)
			{
				extraCash = weaponData.UpgradeRangePrice - GameData.Instance.total_cash.GetIntVal();
			}
			break;
		}
		if (flag)
		{
			UpdateProperty(index);
			return true;
		}
		return false;
	}

	public bool BuyAmmo()
	{
		if (weaponData.BuyBulletShop())
		{
			UpdateAmmo();
			return true;
		}
		return false;
	}
}
