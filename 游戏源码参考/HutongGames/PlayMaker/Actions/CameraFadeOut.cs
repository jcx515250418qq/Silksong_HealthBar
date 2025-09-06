using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Fade to a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
	public class CameraFadeOut : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Color to fade to. E.g., Fade to black.")]
		public FsmColor color;

		[RequiredField]
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Fade out time in seconds.")]
		public FsmFloat time;

		[Tooltip("Optional Event to send when finished.")]
		public FsmEvent finishEvent;

		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		private float startTime;

		private float currentTime;

		private Color colorLerp;

		public override void Reset()
		{
			color = Color.black;
			time = 1f;
			finishEvent = null;
		}

		public override void OnEnter()
		{
			startTime = FsmTime.RealtimeSinceStartup;
			currentTime = 0f;
			colorLerp = Color.clear;
		}

		public override void OnUpdate()
		{
			if (realTime)
			{
				currentTime = FsmTime.RealtimeSinceStartup - startTime;
			}
			else
			{
				currentTime += Time.deltaTime;
			}
			colorLerp = Color.Lerp(Color.clear, color.Value, currentTime / time.Value);
			if (currentTime > time.Value && finishEvent != null)
			{
				base.Fsm.Event(finishEvent);
			}
		}

		public override void OnGUI()
		{
			Color obj = GUI.color;
			GUI.color = colorLerp;
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = obj;
		}
	}
}
