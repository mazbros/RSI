using System.Collections.Generic;

namespace RSI.Helpers
{
    /// <summary>
    /// Input parameter for GetGiltered method
    /// Country property has to be set in constructor - it is not optional and defaults to USA
    /// </summary>
    public class Filter
    {
        public List<int?> Rank { get; set; }
        public List<string> Specialty { get; set; }
        public List<string> State { get; set; }
        public List<string> Country { get; set; }
    }
}