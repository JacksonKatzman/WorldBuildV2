using Game.Debug;
using Game.Enums;
using Game.Simulation;
using Game.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class Organization : IncidentContext, IFactionAffiliated, IOrganizationAffiliated
	{
		public Faction AffiliatedFaction { get; set; }
		public int TotalPositions { get; private set; }
		public int MaxPositionsFilledInSimulation { get; private set; }

		public OrganizationType OrganizationType => template.organizationType;
		public List<Race> racesAllowedToHoldOffice;
		public SubOrganization primaryOrganization;
		public List<ISentient> Leaders => GetLeaders();

		public Organization AffiliatedOrganization
		{
			get
			{
				return this;
			}
			set
			{
				OutputLogger.LogError("Cannot set AffiliatedOrganization of an Organization!");
			}
		}

		public OrganizationTemplate template;

		public Organization() { }

		private Organization(Faction faction, Race majorityStartingRace, ISentient creator = null)
		{
			AffiliatedFaction = faction;
			racesAllowedToHoldOffice = new List<Race> { majorityStartingRace };

			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
			EventManager.Instance.AddEventHandler<AffiliatedFactionChangedEvent>(OnFactionChangeEvent);
		}

		public Organization(OrganizationTemplate template, Faction faction, Race majorityStartingRace, ISentient creator = null) : this(faction, majorityStartingRace, creator)
		{
			this.template = template;
			Setup();
		}

		public Organization(Faction faction, Race majorityStartingRace, OrganizationType organizationType, Character creator = null) : this(faction, majorityStartingRace, creator)
		{
			template = SimRandom.RandomEntryFromList(majorityStartingRace.racePreset.organizationTemplates);
			Setup();
		}

		private void Setup()
		{
			primaryOrganization = new SubOrganization(template.subOrg);
			primaryOrganization.Initialize(this, 0);

			var totalPositions = 0;
			GetTotalPositionCount(ref totalPositions);
			TotalPositions = totalPositions;
			MaxPositionsFilledInSimulation = template.maxPositionsFilledInSimulation;
			TryFillNextPosition(out var organizationPosition);
		}

		public void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			if (gameEvent.context.GetType() == typeof(Character))
			{
				var contains = Contains((Character)gameEvent.context, out var position);
				if(contains)
				{
					position.HandleSuccession();
				}
			}
		}

		public void OnFactionChangeEvent(AffiliatedFactionChangedEvent gameEvent)
		{
			if (gameEvent.affiliate.GetType() == typeof(Character) && Contains((Character)gameEvent.affiliate, out var position))
			{
				position.HandleSuccession();
			}
		}

		public bool Contains(ISentient sentient, out IOrganizationPosition position)
		{
			return primaryOrganization.Contains(sentient, out position);
		}

		public bool TryFillNextPosition(out IOrganizationPosition organizationPosition)
		{
			var filledPositions = 0;
			GetFilledPositionCount(ref filledPositions);
			if(filledPositions < MaxPositionsFilledInSimulation && WorldExtensions.CanFillAdditionalOrganizationPosition(World.CurrentWorld, Age, filledPositions))
			{
				primaryOrganization.TryFillNextPosition(out organizationPosition);
				return true;
			}
			else
			{
				organizationPosition = null;
				return false;
			}
		}

		public override void UpdateContext()
		{
			Age += 1;
			TryFillNextPosition(out var organizationPosition);
		}

		public override void DeployContext()
		{
			
		}

		public override void CheckForDeath()
		{

		}

		public override void Die()
		{
			EventManager.Instance.Dispatch(new RemoveContextEvent(this, GetType()));
		}

		public override void LoadContextProperties()
		{
			AffiliatedFaction = SaveUtilities.ConvertIDToContext<Faction>(contextIDLoadBuffers["AffiliatedFaction"][0]);

			contextIDLoadBuffers.Clear();
		}

		private List<ISentient> GetLeaders()
		{
			var topTier = primaryOrganization.tiers[0];
			var sentients = new List<ISentient>();
			topTier.GetSentients(ref sentients);
			return sentients;
		}

		private void GetTotalPositionCount(ref int total)
		{
			primaryOrganization.GetPositionCount(ref total, false);
		}

		private void GetFilledPositionCount(ref int filled)
		{
			primaryOrganization.GetPositionCount(ref filled, true);
		}
	}
}