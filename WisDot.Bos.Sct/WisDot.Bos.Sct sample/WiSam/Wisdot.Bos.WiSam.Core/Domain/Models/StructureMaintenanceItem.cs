using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class StructureMaintenanceItem
    {
        public string StructureId { get; set; }
        public string ItemId { get; set; }
        public string ItemDescription { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public bool FromInspection { get; set; }
        public DateTime InspectionDate { get; set; }
        public bool UserEntered { get; set; }
        public string Recommender { get; set; }
        public string Priority { get; set; }
        public string EstimatedCost { get; set; }
        public DateTime EstimatedDate { get; set; }
        public string ItemComments { get; set; }
        //public GeoLocation GeoLocation { get; set; }
        public string Region { get; set; }
        public string County { get; set; }
        public string OwnerAgency { get; set; }
        public string MaintainingAgency { get; set; }
        public string Municipality { get; set; }
        public string FeatureOn { get; set; }
        public string LocationOn { get; set; }
        public string FeatureUnder { get; set; }
        public float OverallLength { get; set; }
        public int DeckArea { get; set; }
        public float RoadwayWidth { get; set; }
        public string ConstructionHistory { get; set; }
        public Inspection LastInspection { get; set; }

        public StructureMaintenanceItem()
        {
            Initialize();
        }

        public StructureMaintenanceItem(string structureId)
        {
            Initialize();
            this.StructureId = structureId;
        }

        private void Initialize()
        {
            this.StructureId = "";
            this.ItemDescription = "";
            this.Status = "";
            this.Recommender = "";
            this.Priority = "";
            this.EstimatedCost = "";
            this.ItemComments = "";
            this.Region = "";
            this.County = "";
            this.OwnerAgency = "";
            this.MaintainingAgency = "";
            this.FeatureOn = "";
            this.LocationOn = "";
            this.FeatureUnder = "";
            this.ConstructionHistory = "";
        }
    }
}
