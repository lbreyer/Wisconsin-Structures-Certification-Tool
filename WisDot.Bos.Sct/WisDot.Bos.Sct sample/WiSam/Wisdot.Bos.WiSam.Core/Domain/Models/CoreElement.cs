using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class CoreElement
    {
        public string StructureId { get; set; }
        public DateTime InspectionDate { get; set; }
        public int ElemNum { get; set; }
        public string ElemName { get; set; }
        public int EnvironmentNumber { get; set; }
        public string UnitOfMeasurement { get; set; }
        public int Cs1Quantity { get; set; }
        public int Cs2Quantity { get; set; }
        public int Cs3Quantity { get; set; }
        public int Cs4Quantity { get; set; }
        public int Cs5Quantity { get; set; }
        public int StateCount { get; set; }
        public int TotalQuantity { get; set; }
    }
}
