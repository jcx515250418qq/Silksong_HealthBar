using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Gets a named color value from a game object's material.")]
	public class GetMaterialColor : FsmStateAction
	{
		[Tooltip("The GameObject that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		[UIHint(UIHint.NamedColor)]
		[Tooltip("The named color parameter in the shader.")]
		public FsmString namedColor;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the parameter value.")]
		public FsmColor color;

		public FsmEvent fail;

		public override void Reset()
		{
			gameObject = null;
			materialIndex = 0;
			material = null;
			namedColor = "_Color";
			color = null;
			fail = null;
		}

		public override void OnEnter()
		{
			DoGetMaterialColor();
			Finish();
		}

		private void DoGetMaterialColor()
		{
			if (color.IsNone)
			{
				return;
			}
			string text = namedColor.Value;
			if (text == "")
			{
				text = "_Color";
			}
			if (material.Value != null)
			{
				if (!material.Value.HasProperty(text))
				{
					base.Fsm.Event(fail);
				}
				else
				{
					color.Value = material.Value.GetColor(text);
				}
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget.GetComponent<Renderer>() == null)
			{
				LogError("Missing Renderer!");
			}
			else if (ownerDefaultTarget.GetComponent<Renderer>().material == null)
			{
				LogError("Missing Material!");
			}
			else if (materialIndex.Value == 0)
			{
				if (!ownerDefaultTarget.GetComponent<Renderer>().material.HasProperty(text))
				{
					base.Fsm.Event(fail);
				}
				else
				{
					color.Value = ownerDefaultTarget.GetComponent<Renderer>().material.GetColor(text);
				}
			}
			else if (ownerDefaultTarget.GetComponent<Renderer>().materials.Length > materialIndex.Value)
			{
				Material[] materials = ownerDefaultTarget.GetComponent<Renderer>().materials;
				if (!materials[materialIndex.Value].HasProperty(text))
				{
					base.Fsm.Event(fail);
				}
				else
				{
					color.Value = materials[materialIndex.Value].GetColor(text);
				}
			}
		}
	}
}
