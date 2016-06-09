using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using LinqKit;
using RSI.Cashed;
using RSI.Helpers;
using RSI.Models;

namespace RSI.Controllers
{
    public class DoctorsWebApiController : ApiController
    {
        // GET api/<controller>
        /// <summary>
        ///     Gets all records
        /// </summary>
        /// <returns>Complete list</returns>
        public List<Doctors> Get()
        {
            return DoctorsList.Instance.Get();
        }

        // GET api/<controller>
        /// <summary>
        ///     Takes list and filters it using a filter with several filter categories that may have multiple entries each
        /// </summary>
        /// <param name="filter">Filter with several categories that may have multiple entries each</param>
        /// <remarks>
        ///     Specialty: can contain single or multiple specialties, if empty ignored
        /// </remarks>
        /// <remarks>
        ///     State: can contain single or multiple states, if empty ignored
        /// </remarks>
        /// <remarks>
        ///     Rank: can contain single or multiple ranks, if empty ignored
        /// </remarks>
        /// <param name="doctors">Initial list, may be already sorted, sort will remain intact</param>
        /// <returns>Returns filtered list</returns>
        public List<Doctors> GetFiltered(Filter filter, List<Doctors> doctors)
        {
            var predicateSpecialty = PredicateBuilder.False<Doctors>();
            var predicateState = PredicateBuilder.False<Doctors>();
            var predicateRank = PredicateBuilder.False<Doctors>();

            predicateSpecialty = filter.Specialty.Count != 0
                ? filter.Specialty.Aggregate(predicateSpecialty,
                    (current, temp) => current.Or(d => d.Specialty == temp))
                : PredicateBuilder.True<Doctors>();

            predicateState = filter.State.Count != 0
                ? filter.State.Aggregate(predicateState,
                    (current, temp) => current.Or(d => d.State == temp))
                : PredicateBuilder.True<Doctors>();

            predicateRank = filter.Rank.Count != 0
                ? filter.Rank.Aggregate(predicateRank,
                    (current, temp) => current.Or(d => d.Rank == temp))
                : PredicateBuilder.True<Doctors>();

            return doctors.AsQueryable().Where(predicateSpecialty).Where(predicateState).Where(predicateRank).ToList();
        }

        // GET api/<controller>
        /// <summary>
        ///     Override to return sorted list, takes complete list or filtered portion
        /// </summary>
        /// <param name="sorter">Sort options</param>
        /// <remarks>
        ///     Sorter has two properties: 
        /// </remarks>
        /// <remarks>
        ///         Field - filed to sort on
        /// </remarks>
        /// <remarks>
        ///         Sort - direction of sort
        /// </remarks>
        /// <param name="doctors">Complete or filtered list</param>
        /// <returns>Sorted list</returns>
        public List<Doctors> Get(Sorter sorter, List<Doctors> doctors)
        {
            var result = doctors;

            if (typeof(Doctors).GetProperties().Any(p => p.Name.Equals(sorter.Field)))
            {
                var pi = typeof(Doctors).GetProperty(sorter.Field);
                result = sorter.Order == "desc"
                    ? doctors.OrderByDescending(x => pi.GetValue(x, null)).ToList()
                    : doctors.OrderBy(x => pi.GetValue(x, null)).ToList();
            }

            return result;
        }

        // GET api/<controller>/5
        /// <summary>
        ///     Gets single record
        /// </summary>
        /// <param name="id"> ID of the records</param>
        /// <returns>Single record of type Doctors</returns>
        public async Task<Doctors> GetById(int id)
        {
            var db = new Entities();
            return await db.Doctors.FindAsync(id);
        }

        // GET api/<controller>
        /// <summary>
        ///     Distinct list of all available ranks
        /// </summary>
        /// <returns>List</returns>
        public List<string> GetRanks()
        {
            return RanksList.Instance.Get();
        }

        // GET api/<controller>
        /// <summary>
        ///     Distinct list of all available specialties
        /// </summary>
        /// <returns>List</returns>
        public List<string> GetSpecialties()
        {
            return SpecialtiesList.Instance.Get();
        }

        // GET api/<controller>
        /// <summary>
        ///     Distinct list of available states
        /// </summary>
        /// <returns>List</returns>
        public List<string> GetStates()
        {
            return StatesList.Instance.Get();
        }

        //// POST api/<controller>
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}