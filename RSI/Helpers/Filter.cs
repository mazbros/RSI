using System.Collections.Generic;

namespace RSI.Helpers
{
    /// <summary>
    /// Filter with several categories that may have multiple entries each
    /// </summary>
    public class Filter
    {
        public List<int?> Rank { get; set; }
        public List<string> Specialty { get; set; }
        public List<string> State { get; set; }
    }
}
