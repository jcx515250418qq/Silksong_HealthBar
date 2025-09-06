using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Get a texture from a material on a GameObject")]
	public class GetMaterialTexture : ComponentAction<Renderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject the Material is applied to.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The index of the Material in the Materials array.")]
		public FsmInt materialIndex;

		[UIHint(UIHint.NamedTexture)]
		[Tooltip("The texture to get. See Unity Shader docs for names.")]
		public FsmString namedTexture;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Title("StoreTexture")]
		[Tooltip("Store the texture in a variable.")]
		public FsmTexture storedTexture;

		[Tooltip("Get the shared version of the texture.")]
		public bool getFromSharedMaterial;

		public override void Reset()
		{
			gameObject = null;
			materialIndex = 0;
			namedTexture = "_MainTex";
			storedTexture = null;
			getFromSharedMaterial = false;
		}

		public override void OnEnter()
		{
			DoGetMaterialTexture();
			Finish();
		}

		private void DoGetMaterialTexture()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				string text = namedTexture.Value;
				if (text == "")
				{
					text = "_MainTex";
				}
				if (materialIndex.Value == 0 && !getFromSharedMaterial)
				{
					storedTexture.Value = base.renderer.material.GetTexture(text);
				}
				else if (materialIndex.Value == 0 && getFromSharedMaterial)
				{
					storedTexture.Value = base.renderer.sharedMaterial.GetTexture(text);
				}
				else if (base.renderer.materials.Length > materialIndex.Value && !getFromSharedMaterial)
				{
					Material[] materials = base.renderer.materials;
					storedTexture.Value = base.renderer.materials[materialIndex.Value].GetTexture(text);
					base.renderer.materials = materials;
				}
				else if (base.renderer.materials.Length > materialIndex.Value && getFromSharedMaterial)
				{
					Material[] sharedMaterials = base.renderer.sharedMaterials;
					storedTexture.Value = base.renderer.sharedMaterials[materialIndex.Value].GetTexture(text);
					base.renderer.materials = sharedMaterials;
				}
			}
		}
	}
}
