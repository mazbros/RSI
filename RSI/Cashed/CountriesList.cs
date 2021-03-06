﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RSI.Cashed
{
    public class CountriesList
    {
        private static readonly Lazy<CountriesList> Inst = new Lazy<CountriesList>();

        private List<Country> _countries;
        private List<string> _countryNames;
        private List<string> _countryAbbreviations;
        public static CountriesList Instance => Inst.Value;

        public List<Country> Get()
        {
            if (_countries != null) return _countries;

            var doctors = DoctorsList.Instance.Get();
            var countries = AllCountries.Instance.Get();
            _countries = doctors
                .OrderBy(d => d.Country)
                .Join(countries, d => d.Country, c => c.Code,
                    (d, c) => new Country {Name = c.Name, Code = c.Code}).Distinct().ToList();

            _countries.Remove(new Country {Name = "United States", Code = "USA"});
            //_countries = _countries.OrderBy(c => c.Name).ToList();
            _countries.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCulture));
            _countries.Insert(0, new Country {Name = "United States", Code = "USA"});
            _countries.Insert(1, new Country {Name = "All Non-USA", Code = "ALL"});

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
                    (d, c) => c.Name).Distinct().ToList();

            _countryNames.Remove("United States");
            _countryNames.Sort();
            _countryNames.Insert(0, "United States");
            _countryNames.Insert(1, "All Non-USA");

            return _countryNames;
        }

        public List<string> GetAbbreviations()
        {
            if (_countryAbbreviations != null) return _countryAbbreviations;

            var doctors = DoctorsList.Instance.Get();
            var countries = AllCountries.Instance.Get();
            _countryAbbreviations = doctors
                .OrderBy(d => d.Country)
                .Join(countries, d => d.Country, c => c.Code,
                    (d, c) => c.Code).Distinct().ToList();

            _countryAbbreviations.Remove("USA");
            _countryAbbreviations.Sort();
            _countryAbbreviations.Insert(0, "USA");

            return _countryAbbreviations;
        }

        public string GetIndexByCode(string code)
        {
            var countries = GetNames();

            var results = countries.IndexOf(AllCountries.Instance.GetNameByCode(code)).ToString();

            return results;
        }

        public string GetCodeByIndex(string index)
        {
            var countries = GetNames();

            var results = countries[int.Parse(index)];

            return results;
        }
    }
}