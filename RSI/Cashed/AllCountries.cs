using System;
using System.Collections.Generic;
using System.Linq;
using RSI.Models;

namespace RSI.Cashed
{
    public class AllCountries
    {
        private static readonly Lazy<AllCountries> Inst = new Lazy<AllCountries>();

        private List<CountryKVP> _countries;
        public static AllCountries Instance => Inst.Value;

        public List<CountryKVP> Get()
        {
            if (_countries != null) return _countries;

            var db = new Entities();
            _countries = db.Country_Codes
                .Select(c => new CountryKVP {A3_UN = c.A3_UN, Country = c.COUNTRY }).ToList();
            return _countries;
        }

        public string GetCodeByName(string name)
        {
            var result = _countries.First(c => c.Country.Equals(name)).A3_UN;
            return result;
        }

        public string GetNameByCode(string code)
        {
            var result = _countries.First(c => c.A3_UN.Equals(code)).Country;
            return result;
        }
    }
}