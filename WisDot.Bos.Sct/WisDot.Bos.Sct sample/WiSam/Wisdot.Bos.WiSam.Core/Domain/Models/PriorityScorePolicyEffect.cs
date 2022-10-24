using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class PriorityScorePolicyEffect
    {
        public int Year { get; set; }
        public float ScoreEffect { get; set; }
        public string MathOperation { get; set; }
        public string Policy { get; set; }
        public string PolicyCriteria { get; set; }

        public PriorityScorePolicyEffect()
        { }

        public PriorityScorePolicyEffect(int year, float scoreEffect, string mathOperation, string policy, string policyCriteria)
        {
            Year = year;
            ScoreEffect = scoreEffect;
            MathOperation = mathOperation;
            Policy = policy;
            PolicyCriteria = policyCriteria;
        }
    }
}
