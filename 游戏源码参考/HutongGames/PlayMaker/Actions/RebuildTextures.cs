using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Substance")]
	[Tooltip("Rebuilds all dirty textures. By default the rebuild is spread over multiple frames so it won't halt the game. Check Immediately to rebuild all textures in a single frame.")]
	[Obsolete("Built-in support for Substance Designer materials has been deprecated and will be removed in Unity 2018.1. To continue using Substance Designer materials in Unity 2018.1, you will need to install a suitable third-party external importer from the Asset Store.")]
	public class RebuildTextures : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Substance material.")]
		public FsmMaterial substanceMaterial;

		[RequiredField]
		[Tooltip("Rebuild now!")]
		public FsmBool immediately;

		[Tooltip("Repeat every frame. Useful if you're animating Substance parameters.")]
		public bool everyFrame;

		public override void Reset()
		{
			substanceMaterial = null;
			immediately = false;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoRebuildTextures();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoRebuildTextures();
		}

		private void DoRebuildTextures()
		{
		}
	}
}
