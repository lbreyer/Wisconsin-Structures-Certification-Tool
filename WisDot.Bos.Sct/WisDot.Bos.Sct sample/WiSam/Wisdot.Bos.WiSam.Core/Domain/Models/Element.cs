using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class Element
    {
        public string StructureId { get; set; }
        public DateTime InspectionDate { get; set; }
        public int ElemNum { get; set; }
        public string ElemName { get; set; }
        public int ParentElemNum { get; set; }
        public int Cs1Quantity { get; set; }
        public int Cs2Quantity { get; set; }
        public int Cs3Quantity { get; set; }
        public int Cs4Quantity { get; set; }
        public int TotalQuantity { get; set; }
        public string ElementClassificationCode { get; set; }
        public int DeteriorationYear { get; set; }
        public int EquivalentAge { get; set; }
        public bool InCai { get; set; }
        public int IselItemId { get; set; }
        public int MainIselId { get; set; }

        public Element(int elemNum)
        {
            ElemNum = elemNum;
            DeteriorationYear = -1;
            EquivalentAge = -1;
        }

        public Element()
        {
            DeteriorationYear = -1;
            EquivalentAge = -1;
        }

        public Element(Element elemToCopy)
        {
            this.StructureId = elemToCopy.StructureId;
            this.InspectionDate = elemToCopy.InspectionDate;
            this.ElemNum = elemToCopy.ElemNum;
            this.ElemName = elemToCopy.ElemName;
            this.ParentElemNum = elemToCopy.ParentElemNum;
            this.Cs1Quantity = elemToCopy.Cs1Quantity;
            this.Cs2Quantity = elemToCopy.Cs2Quantity;
            this.Cs3Quantity = elemToCopy.Cs3Quantity;
            this.Cs4Quantity = elemToCopy.Cs4Quantity;
            this.TotalQuantity = elemToCopy.TotalQuantity;
            this.ElementClassificationCode = elemToCopy.ElementClassificationCode;
            this.DeteriorationYear = elemToCopy.DeteriorationYear;
            this.InCai = elemToCopy.InCai;
            this.EquivalentAge = elemToCopy.EquivalentAge;
        }

        public Element(Element elemToCopy, int detYear) : this(elemToCopy)
        {
            this.DeteriorationYear = detYear;
        }
    }
}
