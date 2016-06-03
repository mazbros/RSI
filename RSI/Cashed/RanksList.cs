using System;
using System.Collections.Generic;
using System.Linq;
using RSI.Models;

namespace RSI.Cashed
{
    public class RanksList
    {
        private static readonly Lazy<RanksList> Inst = new Lazy<RanksList>();
        public static RanksList Instance => Inst.Value;

        private List<string> _ranks;

        public List<string> Get()
        {
            if (_ranks != null) return _ranks;

            var doctors = DoctorsList.Instance.Get();
            _ranks = doctors.OrderByDescending(r => r.Rank).Select(r => r.Rank.ToString()).Distinct().ToList();
            return _ranks;
        }
    }
}
