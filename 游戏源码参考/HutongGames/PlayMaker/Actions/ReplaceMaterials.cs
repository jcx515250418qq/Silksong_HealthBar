using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ReplaceMaterials : FsmStateAction
	{
		public FsmOwnerDefault Parent;

		[CompoundArray("Pairs", "From", "To")]
		public FsmMaterial[] From;

		public FsmMaterial[] To;

		private Renderer[] renderers;

		public override void Reset()
		{
			Parent = null;
			From = null;
			To = null;
		}

		public override void Awake()
		{
			GameObject safe = Parent.GetSafe(this);
			if ((bool)safe)
			{
				renderers = safe.GetComponentsInChildren<Renderer>(includeInactive: true);
			}
		}

		public override void OnEnter()
		{
			Renderer[] array = renderers;
			foreach (Renderer renderer in array)
			{
				for (int j = 0; j < From.Length; j++)
				{
					Material value = From[j].Value;
					Material value2 = To[j].Value;
					if (!(renderer.sharedMaterial != value))
					{
						renderer.sharedMaterial = value2;
						break;
					}
				}
			}
			Finish();
		}
	}
}
