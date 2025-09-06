using GlobalEnums;
using GlobalSettings;

public static class HeroSlabCapture
{
	public static void ApplyCaptured()
	{
		HeroController instance = HeroController.instance;
		CurrencyManager.TempStoreCurrency();
		instance.MaxHealth();
		ToolCrest cloaklessCrest = Gameplay.CloaklessCrest;
		BindOrbHudFrame.ForceNextInstant = true;
		ToolItemManager.AutoEquip(cloaklessCrest, markTemp: false);
		BindOrbHudFrame.ForceNextInstant = false;
		GameManager.instance.SetDeathRespawnSimple("Caged Respawn Marker", 0, respawnFacingRight: false);
		PlayerData instance2 = PlayerData.instance;
		instance2.respawnScene = "Slab_03";
		instance2.mapZone = MapZone.THE_SLAB;
		DeliveryQuestItem.BreakAllNoEffects();
	}
}
