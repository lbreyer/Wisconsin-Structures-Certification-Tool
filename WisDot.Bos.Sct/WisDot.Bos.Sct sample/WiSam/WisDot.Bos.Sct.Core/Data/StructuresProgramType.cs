using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Data
{
    public class StructuresProgramType
    {
        public enum WorkConceptStatus
        {
            Eligible = 0,
            Quasicertified = 1,
            Unapproved = 2,
            Precertified = 3,
            Certified = 4,
            Proposed = 8,
            Evaluate = 9,
            Fiips = 10,
        }

        public enum ProjectStatus
        {
            New = 0,
            Unapproved = 2,
            Precertified = 3,
            Certified = 4,
            Deleted = 5,
            PendingPrecertification = 6,
            PendingCertification = 7,
            InPrecertification = 8,
            InCertification = 9,
            Fiips = 10,
            PendingSubmittal = 11,
            QuasiCertified = 12,
            Rejected = 13,
        }

        public enum ProposedWorkConceptJustification
        {
            //None = 0,
            Structural = 10,
            ExpansionCapacityIncrease = 20,
            ProximityToOtherWork = 30,
            Other = 100,
        }

        public enum WorkConceptChangeJustification
        {
            Structural = 0,
            ProximityToOtherWork = 1,
            LaneClosureRestriction = 2,
            ExpansionDevelopment = 3,
            LccaSecondaryWorkConcepts = 4,
            LccaSharedTrafficControlCosts = 5,
            LccaSharedMobilizationCosts = 6,
            LccaOther = 7,
            Other = 50,
        }

        public enum PrecertificatioReviewDecision
        {
            None = 0,
            Accept = 1,
            Reject = 2,
            AutoAccept = 3,
        }

        public enum ProjectUserAction
        {
            CreateProject = 0,
            SavedProject = 1,
            SubmittedProjectForPrecertification = 2,
            SubmittedProjectForCertification = 3,
            WithdrawCertification = 4,
            SubmittedProjectForRejection = 5,
            Reviewing = 10,
            Reviewed = 20,
            BosAcceptedPrecertification = 30,
            BosRejectedPrecertification = 31,
            BosPrecertified = 32,
            BosCertified = 40,
            BosRejectedCertification = 41,
            Deactivated = 50,
            Activated = 60,
            Precertification = 70,
            Certification = 80,
            UndoPrecertificationLiaisonAssignment = 90,
            UndoCertificationLiaisonAssignment = 100,
            BosTransitionallyCertified = 110,
            RequestRecertification = 120,
            GrantRecertification = 130,
            RejectRecertification = 140,
            DeletedProject = 150,
            DeletedWorkConcept = 160
        }

        public enum ObjectType
        {
            WorkConcept = 0,
            StructuresProject = 1,
            FiipsProject = 2,
            UserMarker = 3,
        }
    }
}
