using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DespawnCurrency : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			try
			{
				List<CurrencyObjectBase> list = new List<CurrencyObjectBase>();
				list.AddRange(UnityEngine.Object.FindObjectsByType<GeoControl>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
				list.AddRange(UnityEngine.Object.FindObjectsByType<ShellShard>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Recycle();
				}
			}
			catch (Exception)
			{
			}
			Finish();
		}
	}
}
