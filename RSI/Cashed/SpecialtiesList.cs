using System;
using System.Collections.Generic;
using System.Linq;
using RSI.Models;

namespace RSI.Cashed
{
    public class SpecialtiesList
    {
        private static readonly Lazy<SpecialtiesList> Inst = new Lazy<SpecialtiesList>();
        public static SpecialtiesList Instance => Inst.Value;
        
        private List<string> _specialities;

        public List<string> Get()
        {
            if (_specialities != null) return _specialities;
            
            var doctors = DoctorsList.Instance.Get();
            _specialities = doctors.OrderBy(s => s.Specialty).Select(s => s.Specialty).Distinct().ToList();
            return _specialities;
        }
    }
}