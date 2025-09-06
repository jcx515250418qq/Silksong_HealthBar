using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Checks to see if a Game Object exists in the scene by Name and/or Tag.")]
	public class GameObjectExists : FsmStateAction
	{
		[Tooltip("The name of the GameObject to find. You can leave this empty if you specify a Tag.")]
		public FsmString objectName;

		[UIHint(UIHint.Tag)]
		[Tooltip("Find a GameObject with this tag. If Object Name is specified then both name and Tag must match.")]
		public FsmString withTag;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a boolean variable.")]
		public FsmBool result;

		public override void Reset()
		{
			objectName = "";
			withTag = "Untagged";
			result = null;
		}

		public override void OnEnter()
		{
			Finish();
			if (withTag.Value != "Untagged")
			{
				if (!string.IsNullOrEmpty(objectName.Value))
				{
					GameObject[] array = GameObject.FindGameObjectsWithTag(withTag.Value);
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].name == objectName.Value)
						{
							result.Value = true;
							return;
						}
					}
					result.Value = false;
				}
				else if (GameObject.FindGameObjectWithTag(withTag.Value) != null)
				{
					result.Value = true;
				}
				else
				{
					result.Value = false;
				}
			}
			else if (GameObject.Find(objectName.Value) == null)
			{
				result.Value = true;
			}
		}

		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(objectName.Value) && string.IsNullOrEmpty(withTag.Value))
			{
				return "Please specify Name, Tag, or both for the object you are looking for.";
			}
			return null;
		}
	}
}
