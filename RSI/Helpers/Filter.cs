using System.Collections.Generic;

namespace RSI.Helpers
{
    /// <summary>
    ///     Input parameter for GetGiltered method
    ///     //Country property has to be set in constructor - it is not optional and defaults to USA
    /// </summary>
    public class Filter
    {
        public List<string> Country { get; set; }
        public List<string> Specialty { get; set; }
        public List<string> State { get; set; }
        public List<int?> MinRank { get; set; }
        public List<int?> MinPublications { get; set; }
        public List<int?> MinPrescriptions { get; set; }
        public List<int?> MinPatients { get; set; }
        public List<int?> MinClaims { get; set; }
        public List<string> OldestRecentYear { get; set; }
        public List<string> FirstNameStartsWith { get; set; }
        public List<string> LastNameStartsWith { get; set; }
    }
}