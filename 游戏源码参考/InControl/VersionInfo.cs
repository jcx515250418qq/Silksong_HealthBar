using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct VersionInfo : IComparable<VersionInfo>
	{
		[SerializeField]
		private int major;

		[SerializeField]
		private int minor;

		[SerializeField]
		private int patch;

		[SerializeField]
		private int build;

		public static VersionInfo Min => new VersionInfo(int.MinValue, int.MinValue, int.MinValue, int.MinValue);

		public static VersionInfo Max => new VersionInfo(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);

		public VersionInfo Next => new VersionInfo(major, minor, patch, build + 1);

		public int Build => build;

		public VersionInfo(int major, int minor, int patch, int build)
		{
			this.major = major;
			this.minor = minor;
			this.patch = patch;
			this.build = build;
		}

		public static VersionInfo InControlVersion()
		{
			VersionInfo result = default(VersionInfo);
			result.major = 1;
			result.minor = 8;
			result.patch = 11;
			result.build = 9379;
			return result;
		}

		public static VersionInfo UnityVersion()
		{
			Match match = Regex.Match(Application.unityVersion, "^(\\d+)\\.(\\d+)\\.(\\d+)[a-zA-Z](\\d+)");
			VersionInfo result = default(VersionInfo);
			result.major = Convert.ToInt32(match.Groups[1].Value);
			result.minor = Convert.ToInt32(match.Groups[2].Value);
			result.patch = Convert.ToInt32(match.Groups[3].Value);
			result.build = Convert.ToInt32(match.Groups[4].Value);
			return result;
		}

		public int CompareTo(VersionInfo other)
		{
			if (major < other.major)
			{
				return -1;
			}
			if (major > other.major)
			{
				return 1;
			}
			if (minor < other.minor)
			{
				return -1;
			}
			if (minor > other.minor)
			{
				return 1;
			}
			if (patch < other.patch)
			{
				return -1;
			}
			if (patch > other.patch)
			{
				return 1;
			}
			if (build < other.build)
			{
				return -1;
			}
			if (build > other.build)
			{
				return 1;
			}
			return 0;
		}

		public static bool operator ==(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) == 0;
		}

		public static bool operator !=(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) != 0;
		}

		public static bool operator <=(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) <= 0;
		}

		public static bool operator >=(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) >= 0;
		}

		public static bool operator <(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) < 0;
		}

		public static bool operator >(VersionInfo a, VersionInfo b)
		{
			return a.CompareTo(b) > 0;
		}

		public override bool Equals(object other)
		{
			if (other is VersionInfo)
			{
				return this == (VersionInfo)other;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return major.GetHashCode() ^ minor.GetHashCode() ^ patch.GetHashCode() ^ build.GetHashCode();
		}

		public override string ToString()
		{
			if (build == 0)
			{
				return $"{major}.{minor}.{patch}";
			}
			return $"{major}.{minor}.{patch} build {build}";
		}

		public string ToShortString()
		{
			if (build == 0)
			{
				return $"{major}.{minor}.{patch}";
			}
			return $"{major}.{minor}.{patch}b{build}";
		}
	}
}
