namespace Game.Incidents
{
    public static class IncidentContextExtensions
    {
		public static string Link(this IIncidentContext context)
		{
			return string.Format("<u><link=\"{0}\">{1}</link></u>", context.ID, context.Name);
		}
	}
}