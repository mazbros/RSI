using System;
using System.Collections.Generic;
using System.Linq;

namespace RSI.Cashed
{
    public class SpecialtiesList_NONUS
    {
        private static readonly Lazy<SpecialtiesList_NONUS> Inst = new Lazy<SpecialtiesList_NONUS>();

        private List<string> _specialities;
        public static SpecialtiesList_NONUS Instance => Inst.Value;

        public List<string> Get()
        {
            if (_specialities != null) return _specialities;

            var doctors = DoctorsList_NONUS.Instance.Get();
            _specialities = doctors.OrderBy(s => s.Specialty).Select(s => s.Specialty).Distinct().ToList();
            return _specialities;
        }
    }
}