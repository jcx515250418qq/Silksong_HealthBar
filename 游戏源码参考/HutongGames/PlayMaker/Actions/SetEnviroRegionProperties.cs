using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetEnviroRegionProperties : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[ObjectType(typeof(EnvironmentTypes))]
		public FsmEnum EnviroType;

		public FsmInt Priority;

		public override void Reset()
		{
			Target = null;
			EnviroType = new FsmEnum
			{
				UseVariable = true
			};
			Priority = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				EnviroRegion component = safe.GetComponent<EnviroRegion>();
				if ((bool)component)
				{
					if (!EnviroType.IsNone)
					{
						component.EnvironmentType = (EnvironmentTypes)(object)EnviroType.Value;
					}
					if (!Priority.IsNone)
					{
						component.Priority = Priority.Value;
					}
				}
			}
			Finish();
		}
	}
}
