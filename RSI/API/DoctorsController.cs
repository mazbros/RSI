using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using LinqKit;
using RSI.Cashed;
using RSI.Helpers;
using RSI.Models;

namespace RSI.API
{
    public class DoctorsController : ApiController
    {
        // GET api/<controller>
        /// <summary>
        ///     [GET] Gets all records (not useful, since there are other than USA countries, use <b>GetFiltered</b> instead for more control over what's returned)
        /// </summary>
        /// <returns>Complete list</returns>
        [HttpGet]
        public List<Doctors> GetAll()
        {
            return DoctorsList.Instance.Get();
        }

        // POST api/<controller>/<action>/<filter>
        /// <summary>
        ///     [POST] Takes list and filters it using a filter with several filter categories that may have multiple entries each
        /// </summary>
        /// <param name="filter">Filter with several categories that may have multiple entries each</param>
        /// <remarks>
        ///     <b>Country</b>: optional parameter eg. "USA", "CAN", etc., when not supplied, defaults to "USA"<br/>
        ///     There is a special case to filter and retrieve all non-USA countries: "ALL".<br/> 
        ///     It cannot be combined with any other country, but the others can, eg.: "USA", "CAN" will return USA and Canada combined
        /// </remarks>
        /// <remarks>
        ///     <b>Specialty</b>: can contain single or multiple specialties, if empty ignored
        /// </remarks>
        /// <remarks>
        ///     <b>State</b>: can contain single or multiple states, if empty ignored
        /// </remarks>
        /// <remarks>
        ///     <b>Rank</b>: can contain single or multiple ranks, if empty ignored
        /// </remarks>
        /// <returns>Returns filtered list</returns>
        [HttpPost]
        public List<Doctors> GetFiltered(Filter filter)
        {
            var doctors = DoctorsList.Instance.Get();

            if (filter == null)
                filter = new Filter();
            if ((filter.Country == null) || (filter.Country.Count == 0))
                filter.Country = new List<string> {"USA"};
            if ((filter.Specialty == null) || (filter.Specialty.Count == 0))
                filter.Specialty = new List<string>();
            if ((filter.State == null) || (filter.State.Count == 0))
                filter.State = new List<string>();
            if ((filter.Rank == null) || (filter.Rank.Count == 0))
                filter.Rank = new List<int?>();

            var predicateSpecialty = PredicateBuilder.New<Doctors>(false);
            var predicateState = PredicateBuilder.New<Doctors>(false);
            var predicateRank = PredicateBuilder.New<Doctors>(false);
            var predicateCountry = PredicateBuilder.New<Doctors>(false);

            predicateSpecialty = filter.Specialty != null
                ? filter.Specialty.Count != 0
                    ? filter.Specialty.Aggregate(predicateSpecialty,
                        (current, temp) => current.Or(d => d.Specialty == temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateState = filter.State != null
                ? filter.State.Count != 0
                    ? filter.State.Aggregate(predicateState,
                        (current, temp) => current.Or(d => d.State == temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateRank = filter.Rank != null
                ? filter.Rank.Count != 0
                    ? filter.Rank.Aggregate(predicateRank,
                        (current, temp) => current.Or(d => d.Rank == temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateCountry = filter.Country != null
                ? filter.Country.Count != 0
                    ? filter.Country[0].Equals("ALL")
                        ? filter.Country.Aggregate(predicateCountry,
                            (current, temp) => current.Or(d => d.Country != "USA"))
                        : filter.Country.Aggregate(predicateCountry,
                            (current, temp) => current.Or(d => d.Country == temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            return
                doctors.AsQueryable()
                    .Where(predicateSpecialty)
                    .Where(predicateState)
                    .Where(predicateRank)
                    .Where(predicateCountry)
                    .ToList();
        }

        // POST api/<controller>
        /// <summary>
        ///     [POST] Sorting list by any field in any direction
        /// </summary>
        /// <param name="sorter">Sort options</param>
        /// <remarks>
        ///     Sorter has two properties:
        /// </remarks>
        /// <remarks>
        ///     <b>Field</b> - field to sort on
        /// </remarks>
        /// <remarks>
        ///     <b>Sort</b> - direction of sort
        /// </remarks>
        /// <returns>Sorted list</returns>
        [HttpPost]
        public List<Doctors> GetSorted(Sorter sorter)
        {
            var result = DoctorsList.Instance.Get();

            if (typeof(Doctors).GetProperties().Any(p => p.Name.Equals(sorter.Field)))
            {
                var pi = typeof(Doctors).GetProperty(sorter.Field);
                result = sorter.Order == "desc"
                    ? result.OrderByDescending(x => pi.GetValue(x, null)).ToList()
                    : result.OrderBy(x => pi.GetValue(x, null)).ToList();
            }
            return result;
        }

        // GET api/<controller>/5
        /// <summary>
        ///     [GET] Gets single record
        /// </summary>
        /// <param name="id"> ID of the record to find</param>
        /// <returns>Single record of type Doctors</returns>
        [HttpGet]
        public async Task<Doctors> GetById(int id)
        {
            var db = new Entities();
            return await db.Doctors.FindAsync(id);
        }

        // GET api/<controller>
        /// <summary>
        ///     [GET] Distinct list of all available ranks
        /// </summary>
        /// <returns>List</returns>
        [HttpGet]
        public List<string> GetRanks()
        {
            return RanksList.Instance.Get();
        }

        // GET api/<controller>
        /// <summary>
        ///     [GET] Distinct list of all available specialties
        /// </summary>
        /// <returns>List</returns>
        [HttpGet]
        public List<string> GetSpecialties()
        {
            return SpecialtiesList.Instance.ForApi;
        }

        // GET api/<controller>
        /// <summary>
        ///     [GET] Distinct list of available states
        /// </summary>
        /// <returns>List</returns>
        [HttpGet]
        public List<string> GetStates()
        {
            return StatesList.Instance.Get();
        }

        //{
        //public void Put(int id, [FromBody] string value)

        //// PUT api/<controller>/5
        //}
        //{
        //public void Post([FromBody] string value)

        //// POST api/<controller>
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}