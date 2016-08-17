using System;
using System.Collections.Generic;
using System.Linq;
using RSI.Models;

namespace RSI.Cashed
{
    public class AllCountries
    {
        private static readonly Lazy<AllCountries> Inst = new Lazy<AllCountries>();

        private List<Country> _countries;
        public static AllCountries Instance => Inst.Value;

        public IEnumerable<Country> Get()
        {
            if (_countries != null) return _countries;

            var db = new Entities();
            _countries = db.Country_Codes
                .Select(c => new Country {Code = c.A3_UN, Name = c.COUNTRY }).ToList();
            return _countries;
        }

        public string GetCodeByName(string name)
        {
            var result = _countries.First(c => c.Name.Equals(name)).Code;
            return result;
        }

        //TODO: make sure below is necessary otherwise - delete
        public string GetNameByCode(string code)
        {
            var result = _countries.First(c => c.Code.Equals(code)).Name;
            return result;
        }
    }
}