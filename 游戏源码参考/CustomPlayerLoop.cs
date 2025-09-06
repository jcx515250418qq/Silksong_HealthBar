using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public static class CustomPlayerLoop
{
	public interface ILateFixedUpdate
	{
		bool isActiveAndEnabled { get; }

		void LateFixedUpdate();
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct LateFixedUpdate
	{
		public static PlayerLoopSystem Create(List<ILateFixedUpdate> updateList, List<ILateFixedUpdate> laterUpdateList)
		{
			PlayerLoopSystem result = default(PlayerLoopSystem);
			result.type = typeof(LateFixedUpdate);
			result.updateDelegate = delegate
			{
				Update(updateList);
				Update(laterUpdateList);
				FixedUpdateCycle++;
			};
			return result;
		}

		private static void Update(List<ILateFixedUpdate> list)
		{
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				ILateFixedUpdate lateFixedUpdate = list[i];
				if (lateFixedUpdate != null)
				{
					if (lateFixedUpdate.isActiveAndEnabled)
					{
						lateFixedUpdate.LateFixedUpdate();
					}
					list[num++] = lateFixedUpdate;
				}
			}
			if (num < list.Count)
			{
				list.RemoveRange(num, list.Count - num);
			}
		}
	}

	private static readonly List<ILateFixedUpdate> _lateFixedUpdateList = new List<ILateFixedUpdate>();

	private static readonly List<ILateFixedUpdate> _superLateFixedUpdateList = new List<ILateFixedUpdate>();

	public static int FixedUpdateCycle { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void SetupCustomPlayerLoop()
	{
		PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
		int index;
		PlayerLoopSystem directSubSystem = GetDirectSubSystem<FixedUpdate>(currentPlayerLoop, out index);
		List<PlayerLoopSystem> list = directSubSystem.subSystemList.ToList();
		list.Add(LateFixedUpdate.Create(_lateFixedUpdateList, _superLateFixedUpdateList));
		directSubSystem.subSystemList = list.ToArray();
		currentPlayerLoop.subSystemList[index] = directSubSystem;
		PlayerLoop.SetPlayerLoop(currentPlayerLoop);
	}

	private static PlayerLoopSystem GetDirectSubSystem<T>(PlayerLoopSystem def, out int index)
	{
		index = -1;
		if (def.subSystemList == null)
		{
			return default(PlayerLoopSystem);
		}
		for (int i = 0; i < def.subSystemList.Length; i++)
		{
			PlayerLoopSystem result = def.subSystemList[i];
			if (!(result.type != typeof(T)))
			{
				index = i;
				return result;
			}
		}
		return default(PlayerLoopSystem);
	}

	public static void RegisterLateFixedUpdate(ILateFixedUpdate obj)
	{
		_lateFixedUpdateList.Add(obj);
	}

	public static void UnregisterLateFixedUpdate(ILateFixedUpdate obj)
	{
		_lateFixedUpdateList.Remove(obj);
	}

	public static void RegisterSuperLateFixedUpdate(ILateFixedUpdate obj)
	{
		_superLateFixedUpdateList.Add(obj);
	}

	public static void UnregisterSuperLateFixedUpdate(ILateFixedUpdate obj)
	{
		_superLateFixedUpdateList.Remove(obj);
	}
}
