using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public static class Code
    {
        public const string NbiDeck = "NDEC";
        public const string NbiSubstructure = "NSUB";
        public const string NbiSuperstructure = "NSUP";
        public const string NbiCulvert = "NCUL";
        public const string Bearing = "BEAR";
        public const string Joint = "JNT";
        public const string Overlay = "OLAY";
        public const string Paint = "PNT";
        public const string AllElements = "ALL";
        public const string Defect = "DFCT";
        public const string Deck = "DECK";
        public const string Slab = "SLAB";

        public const string DoNothing = "00";
        public const string DoNothingDesc = "DO NOTHING";
        public const string NewStructure = "01";
        public const string NewStructureDesc = "NEW STRUCTURE";
        public const string ReplaceStructure = "91";
        public const string ReplaceStructureDesc = "REPLACE - STRUCTURE";

        public const string RepairJoints = "04";
        public const string RepairDeck = "28";
        public const string ReplaceDeck = "06";
        public const string PaintComplete = "07";
        public const string ReplaceSuperstructure = "08";
        public const string ReplaceDeckPaintComplete = "80";
        public const string OverlayConcrete = "03";
        public const string OverlayHma = "21";
        public const string OverlayConcreteNewJoints = "58";
        public const string OverlayConcreteNewRailJoints = "20";
        public const string OverlayPma = "65";
        public const string OverlayThinPolymer = "77";
        public const string OverlayPolyesterPolymer = "92";
        public const string OverlayConcretePaint = "98";
        public const string OverlayThinPolymerNewJoints = "99";
        public const string RaiseStructure = "40";
        public const string WidenBridge = "02";
        public const string ReplaceDeckRaiseStructure = "93";
        public const string ReplaceDeckWidenBridge = "68";
        public const string ReplaceDeckOverlayThinPolymerPaintComplete = "95";
        public const string ReplaceDeckOverlayThinPolymer = "97";
        public const string OverlayThinPolymerRepairJoints = "96";

        public const string ElementsToCheck = "8000,510,8511,8512,8514,8515,12,38,8513,8902,8903,1080,2220,1130,1140,1150,1180,2350,3210,8516,3440,3220,2310,2360,9290,6000,8911,202,203,204,205,206,225,226,227,228,229,8400,207,208,210,211,212,213,215,216,217,218,219,220,231,233,234,235,236,1000";
        public const string ElementClassificationsToCheck = "BEAR,RAIL"; // Use HSIS element classification IDs

        public const int BareWearingSurface = 8000;
    }
}
