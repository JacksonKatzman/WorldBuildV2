using Game.Data;

namespace Game.Incidents
{
    public static class IncidentContextExtensions
    {
		public static string Link(this IIncidentContext context)
		{
			return string.Format("<u><link=\"{0}\">{1}</link></u>", context.ID, context.Name);
		}

		public static string Link(this Monster monster)
        {
			return string.Format("<u><link=\"{0}\">{1}</link></u>", monster.monsterData.Link(), monster.monsterData.monsterName.ToLower());
		}

		public static string Link(this IIncidentContext context, string customString)
        {
			return string.Format("<u><link=\"{0}\">{1}</link></u>", context.ID, customString);
		}
	}
}