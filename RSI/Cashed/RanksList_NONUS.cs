using System;
using System.Collections.Generic;
using System.Linq;

namespace RSI.Cashed
{
    public class RanksList_NONUS
    {
        private static readonly Lazy<RanksList_NONUS> Inst = new Lazy<RanksList_NONUS>();

        private List<string> _ranks;
        public static RanksList_NONUS Instance => Inst.Value;

        public List<string> Get()
        {
            if (_ranks != null) return _ranks;

            var doctors = DoctorsList_NONUS.Instance.Get();
            _ranks = doctors.OrderByDescending(r => r.Rank).Select(r => r.Rank.ToString()).Distinct().ToList();
            return _ranks;
        }
    }
}