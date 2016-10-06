using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace RSI.Cashed
{
    public class SpecialtiesList
    {
        private static readonly Lazy<SpecialtiesList> Inst = new Lazy<SpecialtiesList>();

        private List<string> _multiSpecialtiesList;

        private List<string> _singleSpecialtiesList;

        private List<string> _specialities;
        public static SpecialtiesList Instance => Inst.Value;

        public List<string> ForApi
        {
            get
            {
                if (_specialities != null) return _specialities;

                var doctors = DoctorsList.Instance.Get();

                _specialities =
                    doctors.OrderBy(s => s.Specialty)
                        .Select(s => s.Specialty)
                        .Distinct()
                        .ToList();

                return _specialities;
            }
        }

        public List<string> Get()
        {
            if (_specialities != null) return _specialities;

            var doctors = DoctorsList.Instance.Get();

            _multiSpecialtiesList =
                doctors.Select(s => s.Taxonomy_Specialization)
                    .Where(s => !s.IsNullOrWhiteSpace() && s.Contains(","))
                    .Distinct()
                    .ToList();

            var tempList = new List<string>();
            foreach (var li in _multiSpecialtiesList)
            {
                var tL = li.Split(',');

                tempList.AddRange(tL);
            }

            _singleSpecialtiesList =
                doctors.Select(s => s.Taxonomy_Specialization)
                    .Where(s => !s.IsNullOrWhiteSpace() && !s.Contains(","))
                    .Distinct()
                    .ToList();

            tempList.AddRange(_singleSpecialtiesList);

            _specialities =
                doctors.OrderBy(s => s.Specialty)
                    .Select(s => s.Specialty)
                    .Union(tempList)
                    .OrderBy(s => s)
                    .Distinct()
                    .ToList();

            return _specialities;
        }
    }
}