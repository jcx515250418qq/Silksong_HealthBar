using System;

[Serializable]
public struct HeroItemsState
{
	public bool IsRecorded;

	public int Health;

	public int Silk;

	public int Rosaries;

	public int ShellShards;

	public bool DoFullHeal;

	public static HeroItemsState Record(HeroController hc)
	{
		PlayerData playerData = hc.playerData;
		HeroItemsState result = default(HeroItemsState);
		result.IsRecorded = true;
		result.Health = playerData.health;
		result.Silk = playerData.silk;
		result.Rosaries = playerData.geo;
		result.ShellShards = playerData.ShellShards;
		return result;
	}

	public void Apply(HeroController hc)
	{
		if (!IsRecorded)
		{
			return;
		}
		PlayerData playerData = hc.playerData;
		int num = Health - playerData.health;
		playerData.health = Health;
		playerData.silk = Silk;
		playerData.geo = Rosaries;
		playerData.ShellShards = ShellShards;
		if (DoFullHeal)
		{
			int healthBlue = playerData.healthBlue;
			playerData.MaxHealth();
			playerData.healthBlue = healthBlue;
			num = playerData.health - Health;
		}
		IsRecorded = false;
		if (num <= 0)
		{
			if (num < 0)
			{
				EventRegister.SendEvent(EventRegisterEvents.HealthUpdate);
			}
		}
		else
		{
			EventRegister.SendEvent(EventRegisterEvents.HeroHealed);
		}
		if (hc != null)
		{
			hc.SuppressRefillSound(2);
		}
	}
}
