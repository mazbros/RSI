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

namespace RSI.API
{
    public class DoctorsController : ApiController
    {
        // GET api/<controller>
        /// <summary>
        ///     [GET] Gets all records (not useful, since there are other than USA countries, use <b>GetFiltered</b> instead for
        ///     more control over what's returned)
        /// </summary>
        /// <returns>Complete list</returns>
        [HttpGet]
        public List<Doctors> GetAll()
        {
            return DoctorsList.Instance.Get();
        }

        // POST api/<controller>/<action>/<filter>
        /// <summary>
        ///     [POST] Takes list and filters it using a filter (see below)
        /// </summary>
        /// <param name="filter">Filter with several categories that may have multiple or single entries each</param>
        /// <remarks>
        ///     <b>Country</b>: optional parameter eg. "USA", "CAN", etc., when not supplied, defaults to "USA"<br />
        ///     There is a special case to filter and retrieve all non-USA countries: "ALL".<br />
        ///     It cannot be combined with any other country, but the others can, eg.: "USA", "CAN" will return USA
        ///     and Canada combined
        /// </remarks>
        /// <remarks>
        ///     <b>Specialty</b>: can contain single or multiple specialties, if empty ignored. Returns records with
        ///     specialties listed under Specialty and Taxonomy Specialization, that contain supplied value(s)
        /// </remarks>
        /// <remarks>
        ///     <b>State</b>: can contain single or multiple states, if empty ignored
        /// </remarks>
        /// <remarks>
        ///     <b>MinRank</b>: minimum rank, if empty ignored. Returns anything greater or equal to supplied value
        /// </remarks>
        /// <remarks>
        ///     <b>MinPublications</b>: minimum number of publications, if empty ignored. Returns anything greater
        ///     or equal to supplied value
        /// </remarks>
        /// <remarks>
        ///     <b>MinPrescriptions</b>: minimum number of prescriptions, if empty ignored. Returns anything greater
        ///     or equal to supplied value
        /// </remarks>
        /// <remarks>
        ///     <b>MinPatients</b>: minimum number of patients, if empty ignored. Returns anything greater or equal
        ///     to supplied value
        /// </remarks>
        /// <remarks>
        ///     <b>MinClaims</b>: minimum number of claims, if empty ignored. Returns anything greater or equal to
        ///     supplied value
        /// </remarks>
        /// <remarks>
        ///     <b>OldestRecentYear</b>: minimum recent year of publication, if empty ignored. Returns anything
        ///     greater or equal to supplied value
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
            if ((filter.MinRank == null) || (filter.MinRank.Count == 0))
                filter.MinRank = new List<int?>();
            if ((filter.MinPublications == null) || (filter.MinPublications.Count == 0))
                filter.MinPublications = new List<int?>();
            if ((filter.MinPrescriptions == null) || (filter.MinPrescriptions.Count == 0))
                filter.MinPrescriptions = new List<int?>();
            if ((filter.MinPatients == null) || (filter.MinPatients.Count == 0))
                filter.MinPatients = new List<int?>();
            if ((filter.MinClaims == null) || (filter.MinClaims.Count == 0))
                filter.MinClaims = new List<int?>();
            if ((filter.OldestRecentYear == null) || (filter.OldestRecentYear.Count == 0))
                filter.OldestRecentYear = new List<string>();

            if ((filter.FirstNameStartsWith == null) || (filter.FirstNameStartsWith.Count == 0))
                filter.FirstNameStartsWith = new List<string>();
            if ((filter.LastNameStartsWith == null) || (filter.LastNameStartsWith.Count == 0))
                filter.LastNameStartsWith = new List<string>();

            var predicateCountry = PredicateBuilder.New<Doctors>(false);
            var predicateSpecialty = PredicateBuilder.New<Doctors>(false);
            var predicateState = PredicateBuilder.New<Doctors>(false);
            var predicateRank = PredicateBuilder.New<Doctors>(false);
            var predicatePublications = PredicateBuilder.New<Doctors>(false);
            var predicatePrescriptions = PredicateBuilder.New<Doctors>(false);
            var predicatePatients = PredicateBuilder.New<Doctors>(false);
            var predicateClaims = PredicateBuilder.New<Doctors>(false);
            var predicateOldestRecentYear = PredicateBuilder.New<Doctors>(false);

            var predicateFirstNameStartsWith = PredicateBuilder.New<Doctors>(false);
            var predicateLastNameStartsWith = PredicateBuilder.New<Doctors>(false);

            predicateCountry = filter.Country != null
                ? filter.Country.Count != 0
                    ? filter.Country[0].Equals("ALL")
                        ? filter.Country.Aggregate(predicateCountry,
                            (current, temp) => current.Or(d => d.Country != "USA"))
                        : filter.Country.Aggregate(predicateCountry,
                            (current, temp) => current.Or(d => d.Country == temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateSpecialty = filter.Specialty != null
                ? filter.Specialty.Count != 0
                    ? filter.Specialty.Aggregate(predicateSpecialty,
                        (current, temp) => current.Or(d => (d.Specialty == temp)
                                                           || (!d.Taxonomy_Specialization.IsNullOrWhiteSpace()
                                                               && d.Taxonomy_Specialization.Contains(temp))))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateState = filter.State != null
                ? filter.State.Count != 0
                    ? filter.State.Aggregate(predicateState,
                        (current, temp) => current.Or(d => d.State == temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateRank = filter.MinRank != null
                ? filter.MinRank.Count != 0
                    ? filter.MinRank.Aggregate(predicateRank,
                        (current, temp) => current.Or(d => d.Rank >= temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicatePublications = filter.MinPublications != null
                ? filter.MinPublications.Count != 0
                    ? filter.MinPublications.Aggregate(predicatePublications,
                        (current, temp) => current.Or(d => d.Publications >= temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicatePrescriptions = filter.MinPrescriptions != null
                ? filter.MinPrescriptions.Count != 0
                    ? filter.MinPrescriptions.Aggregate(predicatePrescriptions,
                        (current, temp) => current.Or(d => d.Prescriptions >= temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicatePatients = filter.MinPatients != null
                ? filter.MinPatients.Count != 0
                    ? filter.MinPatients.Aggregate(predicatePatients,
                        (current, temp) => current.Or(d => d.Patients >= temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateClaims = filter.MinClaims != null
                ? filter.MinClaims.Count != 0
                    ? filter.MinClaims.Aggregate(predicateClaims,
                        (current, temp) => current.Or(d => d.Claims >= temp))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateOldestRecentYear = filter.OldestRecentYear != null
                ? filter.OldestRecentYear.Count != 0
                    ? filter.OldestRecentYear.Aggregate(predicateOldestRecentYear,
                        (current, temp) => current.Or(d => DateTime.Parse(d.RecentDate).Year >= int.Parse(temp)))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateFirstNameStartsWith = filter.FirstNameStartsWith != null
                ? filter.FirstNameStartsWith.Count != 0
                    ? filter.FirstNameStartsWith.Aggregate(predicateFirstNameStartsWith,
                        (current, temp) => current.Or(d => d.First_Name.StartsWith(temp)))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            predicateLastNameStartsWith = filter.LastNameStartsWith != null
                ? filter.LastNameStartsWith.Count != 0
                    ? filter.LastNameStartsWith.Aggregate(predicateLastNameStartsWith,
                        (current, temp) => current.Or(d => d.Last_Name.StartsWith(temp)))
                    : PredicateBuilder.New<Doctors>(true)
                : PredicateBuilder.New<Doctors>(true);

            return
                doctors.AsQueryable()
                    .Where(predicateCountry)
                    .Where(predicateSpecialty)
                    .Where(predicateState)
                    .Where(predicateRank)
                    .Where(predicatePublications)
                    .Where(predicatePrescriptions)
                    .Where(predicatePatients)
                    .Where(predicateClaims)
                    .Where(predicateOldestRecentYear)
                    .Where(predicateFirstNameStartsWith)
                    .Where(predicateLastNameStartsWith)
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
        /// <returns>List<string></string></returns>
        [HttpGet]
        public List<string> GetRanks()
        {
            return RanksList.Instance.Get();
        }

        // GET api/<controller>
        /// <summary>
        ///     [GET] Distinct list of all available specialties
        /// </summary>
        /// <returns>List<string></string></returns>
        [HttpGet]
        public List<string> GetSpecialties()
        {
            return SpecialtiesList.Instance.ForApi;
        }

        // GET api/<controller>
        /// <summary>
        ///     [GET] Distinct list of available states
        /// </summary>
        /// <returns>List<string></string></returns>
        [HttpGet]
        public List<string> GetStates()
        {
            return StatesList.Instance.Get();
        }

        // GAET api/<controller>
        /// <summary>
        ///     [GET] Distinct list of countries codes abbreviations
        /// </summary>
        /// <returns>List<string></string></returns>
        [HttpGet]
        public List<string> GetCountries()
        {
            return CountriesList.Instance.GetAbbreviations();
        }
    }
}