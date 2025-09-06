using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class CheckHeroPerformanceRegionBase : FsmStateAction
	{
		private HeroPerformanceRegion.AffectedState affectedState;

		private float delay;

		private bool respondingToNoise;

		private bool subscribedNoiseRespond;

		private BlackThreadState blackThreadState;

		private EnemySingDuration enemySingDuration;

		protected bool hasBlackThreadState;

		protected abstract bool IsActive { get; }

		protected abstract Transform TargetTransform { get; }

		protected virtual float TargetRadius => 0f;

		protected abstract float NewDelay { get; }

		protected abstract bool UseNeedolinRange { get; }

		protected abstract bool IsNoiseResponder { get; }

		public override void OnEnter()
		{
			respondingToNoise = false;
			if (IsNoiseResponder)
			{
				NoiseMaker.NoiseCreatedInScene += OnNoiseCreated;
				subscribedNoiseRespond = true;
			}
			affectedState = HeroPerformanceRegion.AffectedState.None;
			delay = 0f;
			blackThreadState = TargetTransform.GetComponent<BlackThreadState>();
			hasBlackThreadState = blackThreadState != null;
			enemySingDuration = TargetTransform.GetComponent<EnemySingDuration>();
		}

		public override void OnExit()
		{
			if (subscribedNoiseRespond)
			{
				NoiseMaker.NoiseCreatedInScene -= OnNoiseCreated;
				subscribedNoiseRespond = true;
			}
			blackThreadState = null;
			hasBlackThreadState = false;
			enemySingDuration = null;
		}

		public override void OnUpdate()
		{
			DoAction(isDelayed: true);
			if (!(delay <= 0f))
			{
				delay -= Time.deltaTime;
				if (delay <= 0f)
				{
					SendEvents(affectedState);
				}
			}
		}

		protected void DoAction(bool isDelayed)
		{
			if (!IsActive)
			{
				return;
			}
			if (hasBlackThreadState && blackThreadState.IsInForcedSing)
			{
				this.affectedState = HeroPerformanceRegion.AffectedState.ActiveInner;
				delay = 0f;
			}
			else
			{
				HeroPerformanceRegion.AffectedState affectedState = this.affectedState;
				this.affectedState = ((UseNeedolinRange && TargetRadius > Mathf.Epsilon) ? HeroPerformanceRegion.GetAffectedStateWithRadius(TargetTransform, TargetRadius) : HeroPerformanceRegion.GetAffectedState(TargetTransform, !UseNeedolinRange));
				if (hasBlackThreadState && blackThreadState.IsVisiblyThreaded)
				{
					this.affectedState = HeroPerformanceRegion.AffectedState.None;
				}
				if (this.affectedState == HeroPerformanceRegion.AffectedState.ActiveInner && hasBlackThreadState && blackThreadState.IsVisiblyThreaded)
				{
					this.affectedState = HeroPerformanceRegion.AffectedState.ActiveOuter;
				}
				if (affectedState == HeroPerformanceRegion.AffectedState.None && this.affectedState != 0 && (bool)enemySingDuration && !enemySingDuration.CheckCanSing())
				{
					this.affectedState = HeroPerformanceRegion.AffectedState.None;
				}
				if (this.affectedState == HeroPerformanceRegion.AffectedState.None && respondingToNoise)
				{
					this.affectedState = HeroPerformanceRegion.AffectedState.ActiveOuter;
				}
				if ((this.affectedState != 0 && affectedState == HeroPerformanceRegion.AffectedState.None) || (this.affectedState == HeroPerformanceRegion.AffectedState.None && affectedState != 0) || (this.affectedState == HeroPerformanceRegion.AffectedState.ActiveInner && affectedState != HeroPerformanceRegion.AffectedState.ActiveInner))
				{
					delay = (isDelayed ? NewDelay : 0f);
				}
			}
			OnAffectedState(this.affectedState);
			if (!(delay > 0f))
			{
				respondingToNoise = false;
				SendEvents(this.affectedState);
			}
		}

		protected abstract void OnAffectedState(HeroPerformanceRegion.AffectedState affectedState);

		protected abstract void SendEvents(HeroPerformanceRegion.AffectedState affectedState);

		private void OnNoiseCreated(Vector2 noiseSourcePos, NoiseMaker.NoiseEventCheck isNoiseInRange, NoiseMaker.Intensities intensity, bool allowOffscreen)
		{
			if (!IsActive)
			{
				return;
			}
			Transform targetTransform = TargetTransform;
			if (!targetTransform)
			{
				return;
			}
			Vector2 worldPosition = ((TargetRadius > Mathf.Epsilon) ? HeroPerformanceRegion.GetPosInRadius(noiseSourcePos, targetTransform.position, TargetRadius) : ((Vector2)targetTransform.position));
			if (isNoiseInRange(worldPosition))
			{
				Vector2 vector = new Vector2(8.3f * ForceCameraAspect.CurrentViewportAspect, 8.3f);
				Vector2 vector2 = GameCameras.instance.mainCamera.transform.position;
				if (allowOffscreen || (!(Mathf.Abs(worldPosition.x - vector2.x) > vector.x + 6f) && !(Mathf.Abs(worldPosition.y - vector2.y) > vector.y + 6f)))
				{
					respondingToNoise = true;
				}
			}
		}
	}
}
