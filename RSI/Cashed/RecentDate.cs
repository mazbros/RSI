using System;
using System.Collections.Generic;
using System.Linq;

namespace RSI.Cashed
{
    public class RecentDate
    {
        private static readonly Lazy<RecentDate> Inst = new Lazy<RecentDate>();

        private List<string> _years;

        public static RecentDate Instance => Inst.Value;

        public List<string> Get()
        {
            if (_years != null) return _years;

            var doctors = DoctorsList.Instance.Get();

            _years =
                doctors.Select(s => DateTime.Parse(s.RecentDate).Year)
                    .Where(y => y >= 2000)
                    .Select(y => y.ToString())
                    .ToList()
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();

            return _years;
        }
    }
}