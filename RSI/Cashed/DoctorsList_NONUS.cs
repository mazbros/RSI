using System;
using System.Collections.Generic;
using System.Linq;
using RSI.Models;

namespace RSI.Cashed
{
    public class DoctorsList_NONUS
    {
        private static Lazy<DoctorsList_NONUS> _inst = new Lazy<DoctorsList_NONUS>();

        private List<Doctors_NONUS> _doctors;
        public static DoctorsList_NONUS Instance => _inst.Value;

        public List<Doctors_NONUS> Get()
        {
            if (_doctors != null) return _doctors;
            var db = new Entities();
            var doctors = db.Doctors_NONUS.Select(d => d).ToList();
            _doctors = doctors;
            return _doctors;
        }

        public static void Reset()
        {
            _inst = new Lazy<DoctorsList_NONUS>();
        }
    }
}