using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Obsolete("Use VideoPlayer actions instead.")]
	[ActionCategory(ActionCategory.Movie)]
	[Tooltip("Sets the Game Object to use to play the audio source associated with a movie texture. Note: the Game Object must have an <a href=\"http://unity3d.com/support/documentation/Components/class-AudioSource.html\">AudioSource</a> component.")]
	public class MovieTextureAudioSettings : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The movie texture to set.")]
		public FsmObject movieTexture;

		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The Game Object to use to play audio. Should have an AudioSource component.")]
		public FsmGameObject gameObject;

		public override void Reset()
		{
			movieTexture = null;
			gameObject = null;
		}

		public override void OnEnter()
		{
			Finish();
		}

		public override string ErrorCheck()
		{
			return "MovieTexture is Obsolete. Use VideoPlayer actions instead.";
		}
	}
}
