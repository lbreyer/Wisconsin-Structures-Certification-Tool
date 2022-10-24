using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class Cai
    {
        public int Year { get; set; }
        public int CaiFormulaId { get; set; }
        public string Formula { get; set; }
        public string FormulaWithValues { get; set; }
        public NbiRating NbiRatings { get; set; }
        public List<Element> AllElements { get; set; }
        public List<Element> CaiElements { get; set; }
        public double CaiValue { get; set; }
        public WisamType.CaiBases Basis { get; set; }
        public string DebugInfo { get; set; }

        public Cai()
        { }

        public Cai(Cai caiToCopy)
        {
            this.Year = caiToCopy.Year;
            this.CaiFormulaId = caiToCopy.CaiFormulaId;
            this.NbiRatings = new NbiRating(caiToCopy.NbiRatings);
            this.CaiValue = caiToCopy.CaiValue;
            this.Basis = caiToCopy.Basis;

            this.Formula = caiToCopy.Formula;
            this.FormulaWithValues = caiToCopy.FormulaWithValues;
            this.NbiRatings = caiToCopy.NbiRatings;
            this.DebugInfo = caiToCopy.DebugInfo;


            this.AllElements = new List<Element>();
            this.CaiElements = new List<Element>();

            foreach (Element elem in caiToCopy.AllElements)
            {
                this.AllElements.Add(new Element(elem));
            }

            foreach (Element elem in caiToCopy.CaiElements)
            {
                this.CaiElements.Add(new Element(elem));
            }
        }

        public Cai(Cai caiToCopy, int calendarYear, int deteriorationYear)
        {
            this.Year = calendarYear;
            this.CaiFormulaId = caiToCopy.CaiFormulaId;
            this.NbiRatings = new NbiRating(caiToCopy.NbiRatings);
            this.CaiValue = caiToCopy.CaiValue;
            this.Basis = caiToCopy.Basis;
            this.AllElements = new List<Element>();
            this.CaiElements = new List<Element>();

            foreach (Element elem in caiToCopy.AllElements)
            {
                this.AllElements.Add(new Element(elem, deteriorationYear));
            }

            foreach (Element elem in caiToCopy.CaiElements)
            {
                this.CaiElements.Add(new Element(elem, deteriorationYear));
            }
        }
    }
}
