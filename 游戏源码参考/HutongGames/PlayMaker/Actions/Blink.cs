using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Effects)]
	[Tooltip("Turns a Game Object on/off in a regular repeating pattern.")]
	public class Blink : ComponentAction<Renderer>
	{
		[RequiredField]
		[Tooltip("The Game Object to blink on/off.")]
		public FsmOwnerDefault gameObject;

		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time to stay off in seconds.")]
		public FsmFloat timeOff;

		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time to stay on in seconds.")]
		public FsmFloat timeOn;

		[Tooltip("Start in the on state.")]
		public FsmBool startOn;

		[Tooltip("Only effect the renderer, keeping other components active.")]
		public bool rendererOnly;

		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		private float startTime;

		private float timer;

		private bool blinkOn;

		public override void Reset()
		{
			gameObject = null;
			timeOff = 0.5f;
			timeOn = 0.5f;
			rendererOnly = true;
			startOn = false;
			realTime = false;
		}

		public override void OnEnter()
		{
			startTime = FsmTime.RealtimeSinceStartup;
			timer = 0f;
			UpdateBlinkState(startOn.Value);
		}

		public override void OnUpdate()
		{
			if (realTime)
			{
				timer = FsmTime.RealtimeSinceStartup - startTime;
			}
			else
			{
				timer += Time.deltaTime;
			}
			if (blinkOn && timer > timeOn.Value)
			{
				UpdateBlinkState(state: false);
			}
			if (!blinkOn && timer > timeOff.Value)
			{
				UpdateBlinkState(state: true);
			}
		}

		private void UpdateBlinkState(bool state)
		{
			GameObject gameObject = ((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
			if (gameObject == null)
			{
				return;
			}
			if (rendererOnly)
			{
				if (UpdateCache(gameObject))
				{
					base.renderer.enabled = state;
				}
			}
			else
			{
				gameObject.SetActive(state);
			}
			blinkOn = state;
			startTime = FsmTime.RealtimeSinceStartup;
			timer = 0f;
		}
	}
}
