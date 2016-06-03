using System;
using System.Collections.Generic;
using System.Linq;
using RSI.Models;

namespace RSI.Cashed
{
    public class DoctorsList
    {
        private static Lazy<DoctorsList> _inst = new Lazy<DoctorsList>();
        public static DoctorsList Instance => _inst.Value;

        private List<Doctors> _doctors;

        public List<Doctors> Get()
        {
            if (_doctors != null) return _doctors;
            var db = new Entities();
            var doctors = db.Doctors.Select(d => d).ToList();
            _doctors = doctors;
            return _doctors;
        }

        public static void Reset()
        {
            _inst = new Lazy<DoctorsList>();
        }
    }
}
