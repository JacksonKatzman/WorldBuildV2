using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	[Serializable, HideReferenceObjectPicker]
	public abstract class ContextTag : IContextTag
	{
		[ValueDropdown("GetTags", IsUniqueList = true, DropdownTitle = "Tags")]
		public string tag;
		public string Tag => tag;

		public bool Equals(ContextTag other)
		{
			if (GetType() == other.GetType())
			{
				return this.Tag.Equals(other.Tag);
			}

			return false;
		}

		public bool Equals(IContextTag other)
		{
			if (GetType() == other.GetType())
			{
				return this.Tag.Equals(other.Tag);
			}

			return false;
		}

		private IEnumerable<string> GetTags()
		{
			if(StaticContextTags.TagDictionary.TryGetValue(GetType(), out var values))
			{
				return values;
			}

			return new List<string>();
		}

		/*
		public static bool operator == (ContextTag left, ContextTag right)
		{
			if (left.GetType() == right.GetType())
			{
				return left.Tag.Equals(right.Tag);
			}

			return false;
		}
		public static bool operator != (ContextTag left, ContextTag right)
		{
			if (left.GetType() == right.GetType())
			{
				return !left.Tag.Equals(right.Tag);
			}

			return false;
		}
		*/
	}

	public static class StaticContextTags
	{
		public readonly static List<string> CharacterTags = new List<string> { "ASSASSIN", "THEIF", "SOULLESS" };
		public readonly static List<string> LandmarkTags = new List<string> { "NATURE", "MORBID", "STONE" };
		public readonly static List<string> FactionTags = new List<string> { "TAG1", "TAG2", "TAG3" };

		public readonly static Dictionary<Type, List<string>> TagDictionary = new Dictionary<Type, List<string>>
		{
			{ typeof(CharacterTag), CharacterTags }, { typeof(LandmarkTag), LandmarkTags }, { typeof(FactionTag), FactionTags }
		};
	}

	public class CharacterTag : ContextTag, IEquatable<CharacterTag>
	{
		public bool Equals(CharacterTag other)
		{
			if (GetType() == other.GetType())
			{
				return this.Tag.Equals(other.Tag);
			}

			return false;
		}
	}

	public class LandmarkTag : ContextTag, IEquatable<LandmarkTag>
	{
		public bool Equals(LandmarkTag other)
		{
			if (GetType() == other.GetType())
			{
				return this.Tag.Equals(other.Tag);
			}

			return false;
		}
	}

	public class FactionTag : ContextTag, IEquatable<FactionTag>
	{
		public bool Equals(FactionTag other)
		{
			if (GetType() == other.GetType())
			{
				return this.Tag.Equals(other.Tag);
			}

			return false;
		}
	}

	public interface IContextTag
	{
		public string Tag { get; }
	}

	public static class ContextTagExtensions
	{
		public static bool Equals(this IContextTag us, IContextTag other)
		{
			if(us.GetType() == other.GetType())
			{
				return us.Tag.Equals(other.Tag);
			}

			return false;
		}
	}
}