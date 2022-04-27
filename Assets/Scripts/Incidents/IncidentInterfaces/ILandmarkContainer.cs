using System.Collections.Generic;

namespace Game.Incidents
{
	public interface ILandmarkContainer
	{
		public List<ILandmark> Landmarks
		{
			get;
		}
	}
}