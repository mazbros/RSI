using System;
using System.Collections.Generic;
using System.Linq;

namespace RSI.Cashed
{
    public class CountriesList
    {
        private static readonly Lazy<CountriesList> Inst = new Lazy<CountriesList>();

        private List<Country> _countries;
        private List<string> _countryNames;
        public static CountriesList Instance => Inst.Value;

        //TODO: make sure below is necessary otherwise - delete
        public List<Country> Get()
        {
            if (_countries != null) return _countries;

            var doctors = DoctorsList.Instance.Get();
            var countries = AllCountries.Instance.Get();
            _countries = doctors
                .OrderBy(d => d.Country)
                .Join(countries, d => d.Country, c => c.Code,
                    (d, c) => new Country {Name = c.Name, Code = c.Code}).Distinct().ToList();
            
            _countries.Remove(new Country { Name = "United States", Code = "USA" });
            _countries.Insert(0, new Country { Name = "United States", Code = "USA" });

            return _countries;
        }

        public List<string> GetNames()
        {
            if (_countryNames != null) return _countryNames;

            var doctors = DoctorsList.Instance.Get();
            var countries = AllCountries.Instance.Get();
            _countryNames = doctors
                .OrderBy(d => d.Country)
                .Join(countries, d => d.Country, c => c.Code,
                    (d, c) =>  c.Name).Distinct().ToList();

            _countryNames.Remove("United States");
            _countryNames.Insert(0, "United States");

            return _countryNames;
        }
    }
}