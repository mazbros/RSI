using System;
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

        public List<Doctors> TestFilter(List<string> filter, List<Doctors> doctors)
        {
            var predicate = PredicateBuilder.False<Doctors>();
            
            foreach (var fltr in filter)
            {
                var temp = fltr;
                predicate = predicate.Or(d => d.Specialty == temp);
            }
            return doctors.AsQueryable().Where(predicate).ToList();
        }
        // GET api/<controller>
        /// <summary>
        /// Gets all records
        /// </summary>
        /// <returns>Complete list</returns>
        public List<Doctors> Get()
        {
            return DoctorsList.Instance.Get();
        }

        // GET api/<controller>
        /// <summary>
        /// Takes list and filters it using a filter with several filter categories that may have multiple entries each
        /// </summary>
        /// <param name="filter">Filter with several categories that may have multiple entries each</param>
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
            
            return  doctors.AsQueryable().Where(predicateSpecialty).Where(predicateState).Where(predicateRank).ToList();
        }

        // GET api/<controller>
        /// <summary>
        /// Override to return sorted list, takes complete list or filtered portion
        /// </summary>
        /// <param name="sortField">Field to sort on</param>
        /// <param name="sortOrder">Direction of sort (asc, desc)</param>
        /// <param name="doctors">Complete or filtered list</param>
        /// <returns>Sorted list</returns>
        public List<Doctors> Get(string sortField, string sortOrder, List<Doctors> doctors )
        {
            List<Doctors> results = null;

            if (sortOrder == "desc")
            {
                switch (sortField)
                {
                    case "DRID":
                            results = doctors.OrderByDescending(d => d.DRID).ToList();
                        break;
                    case "First Name":
                        results = doctors.OrderByDescending(d => d.First_Name).ToList();
                        break;
                    case "Last Name":
                        results = doctors.OrderByDescending(d => d.Last_Name).ToList();
                        break;
                    case "Rank":
                        results = doctors.OrderByDescending(d => d.Rank).ToList();
                        break;
                    case "Publications":
                        results = doctors.OrderByDescending(d => d.Publications).ToList();
                        break;
                    case "Recent Date":
                        results = doctors.OrderByDescending(d => Convert.ToDateTime(d.RecentDate)).ToList();
                        break;
                    case "Specialty":
                        results = doctors.OrderByDescending(d => d.Specialty).ToList();
                        break;
                    case "Address":
                        results = doctors.OrderByDescending(d => d.Address).ToList();
                        break;
                    case "City":
                        results = doctors.OrderByDescending(d => d.City).ToList();
                        break;
                    case "State":
                        results = doctors.OrderByDescending(d => d.State).ToList();
                        break;
                }
            }
            else
            {
                switch (sortField)
                {
                    default:
                        results = doctors.OrderBy(d => d.DRID).ToList();
                        break;
                    case "First Name":
                        results = doctors.OrderBy(d => d.First_Name).ToList();
                        break;
                    case "Last Name":
                        results = doctors.OrderBy(d => d.Last_Name).ToList();
                        break;
                    case "Rank":
                        results = doctors.OrderBy(d => d.Rank).ToList();
                        break;
                    case "Publications":
                        results = doctors.OrderBy(d => d.Publications).ToList();
                        break;
                    case "Recent Date":
                        results = doctors.OrderBy(d => Convert.ToDateTime(d.RecentDate)).ToList();
                        break;
                    case "Specialty":
                        results = doctors.OrderBy(d => d.Specialty).ToList();
                        break;
                    case "Address":
                        results = doctors.OrderBy(d => d.Address).ToList();
                        break;
                    case "City":
                        results = doctors.OrderBy(d => d.City).ToList();
                        break;
                    case "State":
                        results = doctors.OrderBy(d => d.State).ToList();
                        break;
                }
            }

            return results;
        }

        // GET api/<controller>/5
        /// <summary>
        /// Gets single record
        /// </summary>
        /// <param name="id"> ID of the records</param>
        /// <returns>Single record</returns>
        public async Task<Doctors> GetById(int id)
        {
            var db = new Entities();
            return await db.Doctors.FindAsync(id);
        }

        // GET api/<controller>
        /// <summary>
        /// Distinct list of all available ranks
        /// </summary>
        /// <returns>List</returns>
        public List<string> GetRanks()
        {
            return RanksList.Instance.Get();
        }

        // GET api/<controller>
        /// <summary>
        /// Distinct list of all available specialties
        /// </summary>
        /// <returns>List</returns>
        public List<string> GetSpecialties()
        {
            return SpecialtiesList.Instance.Get();
        }

        // GET api/<controller>
        /// <summary>
        /// Distinct list of available states
        /// </summary>
        /// <returns>List</returns>
        public List<string> GetStates()
        {
            return StatesList.Instance.Get();
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}