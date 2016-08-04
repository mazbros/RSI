using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using PagedList;
using RSI.Cashed;
using RSI.Helpers;
using RSI.Models;

namespace RSI.Controllers
{
    public class DoctorsController : Controller
    {
        private static List<Doctors> _doctorsResults = new List<Doctors>();
        private readonly Entities _db = new Entities();

        // GET: DoctorsViews
        public ActionResult Index(long? id, string sortOrder, int? page, string specialtyFilter, string rankFilter,
            string stateFilter, string country = "0")
        {
            if (!Request.IsAuthenticated)
            {
                ViewBag.Error = "Please login in order to use search feature.";
                return View();
            }

            ViewBag.DRID = id;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SpecialtyFilter = specialtyFilter;
            ViewBag.RankFilter = rankFilter;
            ViewBag.StateFilter = stateFilter;
            ViewBag.Country = country;

            ViewBag.DrIdSortParm = string.IsNullOrEmpty(sortOrder) ? "drid_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "First Name" ? "fn_desc" : "First Name";
            ViewBag.LastNameSortParm = sortOrder == "Last Name" ? "ln_desc" : "Last Name";
            ViewBag.RankSortParm = sortOrder == "Rank" ? "rank_desc" : "Rank";
            ViewBag.PublicationsSortParm = sortOrder == "Publications" ? "publications_desc" : "Publications";
            ViewBag.RecentDateSortParm = sortOrder == "Recent Date" ? "recentdate_desc" : "Recent Date";
            ViewBag.SpecialtySortParm = sortOrder == "Specialty" ? "specialty_desc" : "Specialty";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.CitySortParm = sortOrder == "City" ? "city_desc" : "City";
            ViewBag.StateSortParm = sortOrder == "State" ? "state_desc" : "State";

            var allDoctors = DoctorsList.Instance.Get();

            var specialties = SpecialtiesList.Instance.Get();
            var ranks = RanksList.Instance.Get();
            var states = StatesList.Instance.Get();
            var countries = CountriesList.Instance.GetNames();

            var doctors = allDoctors;

            int fs, fr, fst, fc;
            ApplyFilters(specialtyFilter, rankFilter, stateFilter, country, allDoctors, specialties, ranks, states, countries, out fs,
                out fr, out fst, out fc, ref doctors);

            ViewBag.Specialties = DropDownHelper.ToSelectListItems(specialties, fs);
            ViewBag.Ranks = DropDownHelper.ToSelectListItems(ranks, fr);
            ViewBag.States = DropDownHelper.ToSelectListItems(states, fst);
            ViewBag.Countries = DropDownHelper.ToSelectListItems(countries, fc);

            ViewBag.TotalRecords = doctors.Count();

            doctors = ApplySort(sortOrder, doctors);

            ViewBag.CurrentSort = sortOrder;

            const int pageSize = 20;
            var pageNumber = page ?? 1;

            ViewBag.PageNumber = pageNumber;

            _doctorsResults = doctors;

            return View(doctors.ToPagedList(pageNumber, pageSize));
        }

        // Sort order logic
        private List<Doctors> ApplySort(string sortOrder, List<Doctors> doctors)
        {
            switch (sortOrder)
            {
                case "drid_desc":
                    doctors = doctors.OrderByDescending(d => d.DRID).ToList();
                    break;
                case "fn_desc":
                    doctors = doctors.OrderByDescending(d => d.First_Name).ToList();
                    break;
                case "First Name":
                    doctors = doctors.OrderBy(d => d.First_Name).ToList();
                    break;
                case "ln_desc":
                    doctors = doctors.OrderByDescending(d => d.Last_Name).ToList();
                    break;
                case "Last Name":
                    doctors = doctors.OrderBy(d => d.Last_Name).ToList();
                    break;
                case "rank_desc":
                    doctors = doctors.OrderByDescending(d => d.Rank).ToList();
                    break;
                case "Rank":
                    doctors = doctors.OrderBy(d => d.Rank).ToList();
                    break;
                case "publications_desc":
                    doctors = doctors.OrderByDescending(d => d.Publications).ToList();
                    break;
                case "Publications":
                    doctors = doctors.OrderBy(d => d.Publications).ToList();
                    break;
                case "recentdate_desc":
                    doctors =
                        doctors.OrderByDescending(d => Convert.ToDateTime(d.RecentDate, CultureInfo.InvariantCulture))
                            .ToList();
                    break;
                case "Recent Date":
                    doctors =
                        doctors.OrderBy(d => Convert.ToDateTime(d.RecentDate, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "specialty_desc":
                    doctors = doctors.OrderByDescending(d => d.Specialty).ToList();
                    break;
                case "Specialty":
                    doctors = doctors.OrderBy(d => d.Specialty).ToList();
                    break;
                case "address_desc":
                    doctors = doctors.OrderByDescending(d => d.Address).ToList();
                    break;
                case "Address":
                    doctors = doctors.OrderBy(d => d.Address).ToList();
                    break;
                case "city_desc":
                    doctors = doctors.OrderByDescending(d => d.City).ToList();
                    break;
                case "City":
                    doctors = doctors.OrderBy(d => d.City).ToList();
                    break;
                case "state_desc":
                    doctors = doctors.OrderByDescending(d => d.State).ToList();
                    break;
                case "State":
                    doctors = doctors.OrderBy(d => d.State).ToList();
                    break;
                default:
                    doctors = doctors.OrderBy(d => d.DRID).ToList();
                    break;
            }
            return doctors;
        }

        // One filter at a time logic
        private void ApplyFilters(string specialtyFilter, string rankFilter, string stateFilter, string country,
            List<Doctors> allDoctors, IReadOnlyList<string> specialties, IReadOnlyList<string> ranks,
            IReadOnlyList<string> states, IReadOnlyList<string> countries, out int fs, out int fr, out int fst,
            out int fc, ref List<Doctors> doctors)
        {
            int c;
            fc = -1;
            if (!country.IsNullOrWhiteSpace())
            {
                int.TryParse(country, out fc);
                c = fc;
                var countryCode = AllCountries.Instance.GetCodeByName(countries[c]).ToString();
                doctors =
                    allDoctors.Where(d => d.Country.Equals(countryCode))
                        .Select(d => d)
                        .ToList();
            }
            else
            {
                c = 0;
                var countryCode = AllCountries.Instance.GetCodeByName(countries[c]).ToString();

                doctors =
                    allDoctors.Where(d => d.Country.Equals(countryCode))
                        .Select(d => d)
                        .ToList();
            }

            fs = -1;
            if (!specialtyFilter.IsNullOrWhiteSpace())
            {
                int.TryParse(specialtyFilter, out fs);
            }

            fr = -1;
            if (!rankFilter.IsNullOrWhiteSpace())
            {
                int.TryParse(rankFilter, out fr);
            }

            fst = -1;
            if (!stateFilter.IsNullOrWhiteSpace())
            {
                int.TryParse(stateFilter, out fst);
            }

            if (fs >= 0 && fr < 0 && fst < 0)
            {
                var i = fs;
                doctors = allDoctors.Where(d => d.Specialty.Equals(specialties[i])).Select(d => d).ToList();
                if (fc >= 0)
                {
                    c = fc;
                    doctors =
                        doctors.Where(d => d.Country.Equals(AllCountries.Instance.GetCodeByName(countries[c])))
                            .Select(d => d)
                            .ToList();
                }
            }
            if (fr >= 0 && fs < 0 && fst < 0)
            {
                var i = fr;
                doctors =
                    allDoctors.Where(
                        d =>
                            !ranks[i].IsNullOrWhiteSpace()
                                ? d.Rank.Equals(int.Parse(ranks[i]))
                                : d.Rank.ToString().Equals(ranks[i])).Select(d => d).ToList();
                if (fc >= 0)
                {
                    c = fc;
                    doctors =
                        doctors.Where(d => d.Country.Equals(AllCountries.Instance.GetCodeByName(countries[c])))
                            .Select(d => d)
                            .ToList();
                }
            }
            if (fst >= 0 && fr < 0 && fs < 0)
            {
                var i = fst;
                doctors = allDoctors.Where(d => d.State.Equals(states[i])).Select(d => d).ToList();
                if (fc >= 0)
                {
                    c = fc;
                    doctors =
                        doctors.Where(d => d.Country.Equals(AllCountries.Instance.GetCodeByName(countries[c])))
                            .Select(d => d)
                            .ToList();
                }
            }
            if (fs >= 0 && fr >= 0 && fst < 0)
            {
                var i = fs;
                var j = fr;
                doctors =
                    allDoctors.Where(
                        d =>
                            d.Specialty.Equals(specialties[i]) &&
                            (!ranks[j].IsNullOrWhiteSpace()
                                ? d.Rank.Equals(int.Parse(ranks[j]))
                                : d.Rank.ToString().Equals(ranks[j])))
                        .Select(d => d)
                        .ToList();
                if (fc >= 0)
                {
                    c = fc;
                    doctors =
                        doctors.Where(d => d.Country.Equals(AllCountries.Instance.GetCodeByName(countries[c])))
                            .Select(d => d)
                            .ToList();
                }
            }
            if (fr < 0 && fs >= 0 && fst >= 0)
            {
                var i = fs;
                var j = fst;
                doctors =
                    allDoctors.Where(d => d.Specialty.Equals(specialties[i]) && d.State.Equals(states[j]))
                        .Select(d => d)
                        .ToList();
                if (fc >= 0)
                {
                    c = fc;
                    doctors =
                        doctors.Where(d => d.Country.Equals(AllCountries.Instance.GetCodeByName(countries[c])))
                            .Select(d => d)
                            .ToList();
                }
            }
            if (fst >= 0 && fr >= 0 && fs < 0)
            {
                var i = fst;
                var j = fr;
                doctors =
                    allDoctors.Where(
                        d =>
                            d.State.Equals(states[i]) &&
                            (!ranks[j].IsNullOrWhiteSpace()
                                ? d.Rank.Equals(int.Parse(ranks[j]))
                                : d.Rank.ToString().Equals(ranks[j])))
                        .Select(d => d)
                        .ToList();
                if (fc >= 0)
                {
                    c = fc;
                    doctors =
                        doctors.Where(d => d.Country.Equals(AllCountries.Instance.GetCodeByName(countries[c])))
                            .Select(d => d)
                            .ToList();
                }
            }

            if (fs < 0 || fr < 0 || fst < 0 || fc < 0) return; // fs >= 0 && fr >= 0 && fst >= 0

            var x = fs;
            var z = fr;
            var y = fst;
            doctors =
                allDoctors.Where(
                    d =>
                        d.Specialty.Equals(specialties[x]) &&
                        (!ranks[z].IsNullOrWhiteSpace()
                            ? d.Rank.Equals(int.Parse(ranks[z]))
                            : d.Rank.ToString().Equals(ranks[z])) &&
                        d.State.Equals(states[y])).Select(d => d).ToList();
            if (fc >= 0)
            {
                c = fc;
                doctors =
                    doctors.Where(d => d.Country.Equals(AllCountries.Instance.GetCodeByName(countries[c])))
                        .Select(d => d)
                        .ToList();
            }
        }

        // GET: Doctors/Details/5
        public async Task<ActionResult> Details(long? id, string sortOrder, int? page, string specialtyFilter,
            string rankFilter, string stateFilter, string country)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.DRID = id;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.PageNumber = page;
            ViewBag.SpecialtyFilter = specialtyFilter;
            ViewBag.RankFilter = rankFilter;
            ViewBag.StateFilter = stateFilter;
            ViewBag.Country = country;

            Doctors consolidatedDoctorsView = await _db.Doctors.FindAsync(id);
            if (consolidatedDoctorsView == null)
            {
                return HttpNotFound();
            }
            return View(consolidatedDoctorsView);
        }

        // GET: Doctors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(
                Include =
                    "DRID,Rank,Publications,RecentDate,NPI,REVIEWER_ID,Specialty,First_Name,Last_Name,Address,City,State,Zipcode,Phone,Fax," +
                    "Email_Address,County,Company_Name,Latitude,Longitude,Timezone,Website,Gender,Credentials,Taxonomy_Code,Taxonomy_Classification," +
                    "Taxonomy_Specialization,License_Number,License_State,Medical_School,Residency_Training,Graduation_Year,Patients,Claims,Prescriptions,Country"
                )] Doctors consolidatedDoctorsView)
        {
            if (ModelState.IsValid)
            {
                _db.Doctors.Add(consolidatedDoctorsView);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(consolidatedDoctorsView);
        }

        // GET: Doctors/Edit/5
        public async Task<ActionResult> Edit(long? id, string sortOrder, int? page, string specialtyFilter,
            string rankFilter, string stateFilter, string country)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctors consolidatedDoctorsView = await _db.Doctors.FindAsync(id);
            if (consolidatedDoctorsView == null)
            {
                return HttpNotFound();
            }

            ViewBag.Specialties = DropDownHelper.ToSelectListItems(SpecialtiesList.Instance.Get());

            ViewBag.DRID = id;
            ViewBag.SortOrder = sortOrder;
            ViewBag.PageNumber = page;
            ViewBag.SpecialtyFilter = specialtyFilter;
            ViewBag.RankFilter = rankFilter;
            ViewBag.StateFilter = stateFilter;
            ViewBag.Country = country;

            return View(consolidatedDoctorsView);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(
                Include =
                    "DRID,Rank,Publications,RecentDate,NPI,REVIEWER_ID,Specialty,First_Name,Last_Name,Address,City,State,Zipcode,Phone,Fax," +
                    "Email_Address,County,Company_Name,Latitude,Longitude,Timezone,Website,Gender,Credentials,Taxonomy_Code,Taxonomy_Classification," +
                    "Taxonomy_Specialization,License_Number,License_State,Medical_School,Residency_Training,Graduation_Year,Patients,Claims,Prescriptions,Country"
                )] Doctors consolidatedDoctorsView,
            long? id, string sortOrder, int? page, string specialtyFilter, string rankFilter, string stateFilter, string country)
        {
            ViewBag.DRID = id;
            ViewBag.SortOrder = sortOrder;
            ViewBag.PageNumber = page;
            ViewBag.SpecialtyFilter = specialtyFilter;
            ViewBag.RankFilter = rankFilter;
            ViewBag.StateFilter = stateFilter;
            ViewBag.Country = country;

            if (ModelState.IsValid)
            {
                _db.Entry(consolidatedDoctorsView).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                DoctorsList.Reset();
                return RedirectToAction(
                    "Index",
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Doctors",
                            action = "Index",
                            id = ViewBag.DRID,
                            sortOrder = ViewBag.SortOrder,
                            page = ViewBag.PageNumber,
                            specialtyFilter = ViewBag.SpecialtyFilter,
                            rankFilter = ViewBag.RankFilter,
                            stateFilter = ViewBag.StateFilter,
                            country = ViewBag.Country
                        })
                    );
            }
            return View(consolidatedDoctorsView);
        }

        // GET: Doctors/Delete/5
        public async Task<ActionResult> Delete(long? id, string sortOrder, int? page, string specialtyFilter,
            string rankFilter, string stateFilter, string country)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctors consolidatedDoctorsView = await _db.Doctors.FindAsync(id);
            if (consolidatedDoctorsView == null)
            {
                return HttpNotFound();
            }
            return View(consolidatedDoctorsView);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Doctors consolidatedDoctorsView = await _db.Doctors.FindAsync(id);
            _db.Doctors.Remove(consolidatedDoctorsView);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // CSV file dump of sorted and filtered list
        public ActionResult CsvList(string sortOrder, int? page, string specialtyFilter, string rankFilter,
            string stateFilter, string country)
        {
            var timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return new CsvDownloader<Doctors>(_doctorsResults, $"RSI-Export_{timeStamp}.csv");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}