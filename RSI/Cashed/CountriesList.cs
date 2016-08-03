using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSI.Cashed
{
    public class CountriesList
    {
        private static readonly Lazy<CountriesList> Inst = new Lazy<CountriesList>();

        private List<string> _countries;
        public static CountriesList Instance => Inst.Value;

        public List<string> Get()
        {
            if (_countries != null) return _countries;

            var doctors = DoctorsList_NONUS.Instance.Get();
            _countries = doctors.OrderBy(c => c.Country).Select(c => c.Country).Distinct().ToList();
            return _countries;
        }
    }
}