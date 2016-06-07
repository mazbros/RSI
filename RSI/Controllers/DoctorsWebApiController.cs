using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using LinqKit;
using Microsoft.Ajax.Utilities;
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
        public List<Doctors> Get()
        {
            return DoctorsList.Instance.Get();
        }

        // GET api/<controller>
        public List<Doctors> GetFiltered(Filter filter, List<Doctors> doctors)
        {
            var predicate = PredicateBuilder.False<Doctors>();

            if (filter.Specialty.Count != 0 && 
                filter.State.IsNullOrWhiteSpace() &&
                filter.Rank == null)
            {
                predicate = filter.Specialty.Aggregate(predicate, (current, temp) => current.Or(d => d.Specialty == temp));

                doctors = doctors.AsQueryable().Where(predicate).ToList();
            }
            if (filter.Specialty.Count != 0 &&
                !filter.State.IsNullOrWhiteSpace() &&
                filter.Rank == null)
            {
                predicate = filter.Specialty.Aggregate(predicate, (current, temp) => current.Or(d => d.Specialty == temp));
                doctors = doctors.AsQueryable().Where(predicate).Where(d => d.State == filter.State).ToList();
            }
            if (filter.Specialty.Count != 0 &&
                !filter.State.IsNullOrWhiteSpace() &&
                filter.Rank != null)
            {
                predicate = filter.Specialty.Aggregate(predicate, (current, temp) => current.Or(d => d.Specialty == temp));
                doctors = doctors.AsQueryable().Where(predicate).Where(d => d.State == filter.State && d.Rank == filter.Rank).ToList();
            }
            if (filter.Specialty.Count() != 0 &&
                filter.State.IsNullOrWhiteSpace() &&
                filter.Rank != null)
            {
                doctors = doctors.AsQueryable().Where(predicate).Where(d => d.Rank == filter.Rank).ToList();
            }
            if (filter.Specialty.Count == 0 &&
                !filter.State.IsNullOrWhiteSpace() &&
                filter.Rank != null)
            {
                doctors = doctors.Where(d => d.State == filter.State && d.Rank == filter.Rank).ToList();
            }
            if (filter.Specialty.Count == 0 &&
                !filter.State.IsNullOrWhiteSpace() &&
                filter.Rank == null)
            {
                doctors = doctors.Where(d => d.State == filter.State).ToList();
            }
            if (filter.Specialty.Count == 0 &&
                filter.State.IsNullOrWhiteSpace() &&
                filter.Rank == null)
            {
                doctors = doctors.Where(d => d.Rank == filter.Rank).ToList();
            }

            return doctors;
        }

        // GET api/<controller>
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
        public async Task<Doctors> GetById(int id)
        {
            var db = new Entities();
            return await db.Doctors.FindAsync(id);
        }

        // GET api/<controller>
        public List<string> GetRanks()
        {
            return RanksList.Instance.Get();
        }

        // GET api/<controller>
        public List<string> GetSpecialties()
        {
            return SpecialtiesList.Instance.Get();
        }

        // GET api/<controller>
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