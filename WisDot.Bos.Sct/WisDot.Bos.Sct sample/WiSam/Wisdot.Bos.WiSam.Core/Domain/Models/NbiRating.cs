using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class NbiRating
    {
        public string StructureId { get; set; }
        public DateTime InspectionDate { get; set; }
        public string DeckRating { get; set; }
        public double DeckRatingVal { get; set; }
        public string SuperstructureRating { get; set; }
        public double SuperstructureRatingVal { get; set; }
        public string SubstructureRating { get; set; }
        public double SubstructureRatingVal { get; set; }
        public string CulvertRating { get; set; }
        public double CulvertRatingVal { get; set; }
        public int DeckDeteriorationYear { get; set; }
        public int SuperstructureDeteriorationYear { get; set; }
        public int SubstructureDeteriorationYear { get; set; }
        public int CulvertDeteriorationYear { get; set; }
        public string WaterwayRating { get; set; }
        public double WaterwayRatingVal { get; set; }
        public string InspectionTypeCode { get; set; }
        public string InspectionTypeDescription { get; set; }

        public NbiRating()
        {
            DeckRating = "N";
            SuperstructureRating = "N";
            SubstructureRating = "N";
            CulvertRating = "N";
            WaterwayRating = "N";
            DeckRatingVal = -1;
            SuperstructureRatingVal = -1;
            SubstructureRatingVal = -1;
            CulvertRatingVal = -1;
            WaterwayRatingVal = -1;
            DeckDeteriorationYear = -1;
            SuperstructureDeteriorationYear = -1;
            SubstructureDeteriorationYear = -1;
            CulvertDeteriorationYear = -1;
        }

        public NbiRating(NbiRating nbiToCopy)
        {
            this.StructureId = nbiToCopy.StructureId;
            this.InspectionDate = nbiToCopy.InspectionDate;
            this.DeckRating = nbiToCopy.DeckRating;
            this.SuperstructureRating = nbiToCopy.SuperstructureRating;
            this.SubstructureRating = nbiToCopy.SubstructureRating;
            this.CulvertRating = nbiToCopy.CulvertRating;
            this.DeckRatingVal = nbiToCopy.DeckRatingVal;
            this.SuperstructureRatingVal = nbiToCopy.SuperstructureRatingVal;
            this.SubstructureRatingVal = nbiToCopy.SubstructureRatingVal;
            this.CulvertRatingVal = nbiToCopy.CulvertRatingVal;
            this.DeckDeteriorationYear = nbiToCopy.DeckDeteriorationYear;
            this.SuperstructureDeteriorationYear = nbiToCopy.SuperstructureDeteriorationYear;
            this.SubstructureDeteriorationYear = nbiToCopy.SubstructureDeteriorationYear;
            this.CulvertDeteriorationYear = nbiToCopy.CulvertDeteriorationYear;
        }
    }
}
