using System;
using System.Collections.Generic;
using System.Linq;
using RSI.Models;

namespace RSI.Cashed
{
    public class StatesList
    {
        private static readonly Lazy<StatesList> Inst = new Lazy<StatesList>();
        public static StatesList Instance => Inst.Value;

        private List<string> _states;

        public List<string> Get()
        {
            if (_states != null) return _states;
        
            var doctors = DoctorsList.Instance.Get();
            _states = doctors.OrderBy(s => s.State).Select(s => s.State).Distinct().ToList();
            return _states;
        }
    }
}
