using System;
using System.Linq;
using UnityEngine;

namespace Assets.Ketchapp.Internal.Editor.EditorUtils.Dto
{
    [Serializable]
    public class UnityVersion : IComparable<UnityVersion>, IEquatable<UnityVersion>
    {
        public const char IdentifiersSeparator = '.';

        public uint Major { get; set; }
        public uint Minor { get; set; }
        public uint Patch { get; set; }
        public string PreRelease { get; set; }
        public string Build { get; set; }

        public static bool operator ==(UnityVersion left, UnityVersion right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UnityVersion left, UnityVersion right)
        {
            return !Equals(left, right);
        }

        public static bool operator >(UnityVersion left, UnityVersion right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(UnityVersion left, UnityVersion right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(UnityVersion left, UnityVersion right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(UnityVersion left, UnityVersion right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static implicit operator string(UnityVersion s)
        {
            return s.ToString();
        }

        public static implicit operator UnityVersion(string s)
        {
            return Parse(s);
        }

        public static UnityVersion Parse(string unityVersion)
        {
            return FromString(unityVersion);
        }

        public UnityVersion()
        {
            Minor = 1;
            PreRelease = string.Empty;
        }

        public int CompareTo(UnityVersion other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            if (ReferenceEquals(this, null))
            {
                return -1;
            }

            var majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0)
            {
                return majorComparison;
            }

            var minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0)
            {
                return minorComparison;
            }

            var patchComparison = Patch.CompareTo(other.Patch);
            return patchComparison != 0 ? patchComparison : ComparePreReleaseVersions(this, other);
        }

        public bool Equals(UnityVersion other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((UnityVersion)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Major}{IdentifiersSeparator}{Minor}{IdentifiersSeparator}{Patch}{PreRelease}{Build}";
        }

        #region Private Methods

        private static UnityVersion FromString(string semVerString)
        {
            var strings = semVerString.Split(IdentifiersSeparator);
            var preRelease = new string(strings[2].ToCharArray().Where(char.IsLetter).ToArray());
            var buildIndex = semVerString.IndexOf(preRelease, StringComparison.Ordinal);
            var build = buildIndex >= 0 ? semVerString.Substring(buildIndex + 1) : string.Empty;
            uint major = 0;
            if (strings.Length > 0)
            {
                uint.TryParse(strings[0], out major);
            }

            uint minor = 1;
            if (strings.Length > 1)
            {
                uint.TryParse(strings[1], out minor);
            }

            uint patch = 0;
            if (strings.Length > 0)
            {
                var patchString = new string(strings[2].ToCharArray().TakeWhile(c => !char.IsLetter(c)).ToArray());
                uint.TryParse(patchString, out patch);
            }

            var unityVersion = new UnityVersion
            {
                Major = major,
                Minor = minor,
                Patch = patch,
                PreRelease = preRelease,
                Build = build
            };

            return unityVersion;
        }

        private int ComparePreReleaseVersions(UnityVersion x, UnityVersion y)
        {
            if (!string.IsNullOrEmpty(x.PreRelease))
            {
                if (string.IsNullOrEmpty(y.PreRelease))
                {
                    return -1;
                }
            }
            else
            {
                return !string.IsNullOrEmpty(y) ? 1 : 0;
            }

            var xIdentifiers = x.PreRelease;
            var yIdentifiers = y.PreRelease;
            var length = Mathf.Min(xIdentifiers.Length, yIdentifiers.Length);
            for (var i = 0; i < length; i++)
            {
                var xIdentifier = xIdentifiers[i];
                var yIdentifier = yIdentifiers[i];
                if (Equals(xIdentifier, yIdentifier))
                {
                    continue;
                }

                return ComparePreReleaseIdentifiers(xIdentifier.ToString(), yIdentifier.ToString());
            }

            // Compare Build numbers
            int.TryParse(x.Build, out var xNumber);
            int.TryParse(y.Build, out var yNumber);
            return xNumber.CompareTo(yNumber);
        }

        private int ComparePreReleaseIdentifiers(string xIdentifier, string yIdentifier)
        {
            var lettersDifference = string.Compare(xIdentifier, yIdentifier, StringComparison.Ordinal);
            if (lettersDifference > 0)
            {
                lettersDifference = 1;
            }
            else if (lettersDifference < 0)
            {
                lettersDifference = -1;
            }

            return lettersDifference;
        }

        #endregion
    }
}