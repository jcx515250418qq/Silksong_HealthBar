using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ProjectBenchmark
{
	public static bool IsRunning { get; private set; }

	public static void RunBenchmark(MonoBehaviour runner)
	{
		runner.StartCoroutine(BenchmarkRoutine(runner));
	}

	private static IEnumerator BenchmarkRoutine(MonoBehaviour runner)
	{
		HeroController hc = HeroController.instance;
		if (!hc)
		{
			yield break;
		}
		GameManager gm = GameManager.instance;
		HeroActions ia = ManagerSingleton<InputHandler>.Instance.inputActions;
		hc.AddInputBlocker(runner);
		IsRunning = true;
		CheatManager.InvincibilityStates initialInvincibility = CheatManager.Invincibility;
		CheatManager.Invincibility = CheatManager.InvincibilityStates.FullInvincible;
		try
		{
			Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap = SceneTeleportMap.GetTeleportMap();
			foreach (KeyValuePair<string, SceneTeleportMap.SceneInfo> item in teleportMap)
			{
				if (item.Key.IsAny(WorldInfo.MenuScenes) || item.Key.IsAny(WorldInfo.NonGameplayScenes) || GameManager.GetBaseSceneName(item.Key) != item.Key)
				{
					continue;
				}
				string text = item.Value.TransitionGates.FirstOrDefault((string g) => g.StartsWith("left") || g.StartsWith("right") == g.StartsWith("door"));
				if (string.IsNullOrEmpty(text))
				{
					text = item.Value.TransitionGates.FirstOrDefault();
					if (string.IsNullOrEmpty(text))
					{
						continue;
					}
				}
				EventRegister.SendEvent(EventRegisterEvents.FsmCancel);
				hc.AffectedByGravity(gravityApplies: false);
				gm.BeginSceneTransition(new GameManager.SceneLoadInfo
				{
					SceneName = item.Key,
					EntryGateName = text,
					AlwaysUnloadUnusedAssets = true,
					EntrySkip = true,
					PreventCameraFadeOut = true
				});
				yield return null;
				while (gm.IsInSceneTransition || !hc.isHeroInPosition || hc.cState.transitioning)
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.5f);
				if (ia.MenuCancel.IsPressed)
				{
					break;
				}
			}
		}
		finally
		{
			IsRunning = false;
			hc.RemoveInputBlocker(runner);
			CheatManager.Invincibility = initialInvincibility;
		}
	}
}
