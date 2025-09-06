using System;

public interface IHitResponder
{
	public enum Response
	{
		None = 0,
		GenericHit = 1,
		DamageEnemy = 2,
		Invincible = 3
	}

	public struct HitResponse : IEquatable<HitResponse>
	{
		public Response response;

		public bool consumeCharges;

		public static HitResponse Default = new HitResponse
		{
			response = Response.None,
			consumeCharges = true
		};

		public HitResponse(Response response, bool consumeCharges = true)
		{
			this.response = response;
			this.consumeCharges = consumeCharges;
		}

		public static implicit operator HitResponse(Response response)
		{
			HitResponse result = default(HitResponse);
			result.response = response;
			return result;
		}

		public static implicit operator Response(HitResponse hitResponse)
		{
			return hitResponse.response;
		}

		public bool Equals(HitResponse other)
		{
			if (response == other.response)
			{
				return consumeCharges == other.consumeCharges;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is HitResponse other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine((int)response, consumeCharges);
		}
	}

	bool HitRecurseUpwards => true;

	int HitPriority => 0;

	HitResponse Hit(HitInstance damageInstance);
}
