using System;
using System.Collections.Generic;
using System.Linq;

namespace RSI.Cashed
{
    public class CountriesList
    {
        private static readonly Lazy<CountriesList> Inst = new Lazy<CountriesList>();

        private List<CountryKVP> _countries;
        private List<string> _countryNames;
        public static CountriesList Instance => Inst.Value;

        public List<CountryKVP> Get()
        {
            if (_countries != null) return _countries;

            var doctors = DoctorsList.Instance.Get();
            var countries = AllCountries.Instance.Get();
            _countries = doctors
                .OrderBy(d => d.Country)
                .Join(countries, d => d.Country, c => c.A3_UN,
                    (d, c) => new CountryKVP {Country = c.Country, A3_UN = c.A3_UN}).Distinct().ToList();
            
            _countries.Remove(new CountryKVP { Country = "United States", A3_UN = "USA" });
            _countries.Insert(0, new CountryKVP { Country = "United States", A3_UN = "USA" });

            return _countries;
        }

        public List<string> GetNames()
        {
            if (_countryNames != null) return _countryNames;

            var doctors = DoctorsList.Instance.Get();
            var countries = AllCountries.Instance.Get();
            _countryNames = doctors
                .OrderBy(d => d.Country)
                .Join(countries, d => d.Country, c => c.A3_UN,
                    (d, c) =>  c.Country).Distinct().ToList();

            _countryNames.Remove("United States");
            _countryNames.Insert(0, "United States");

            return _countryNames;
        }
    }
}