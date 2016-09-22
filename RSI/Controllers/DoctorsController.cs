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
        private static List<DoctorsCanada> _doctorsResultsCanada = new List<DoctorsCanada>();
        private static List<DoctorsOther> _doctorsResultsOther = new List<DoctorsOther>();
        private readonly Entities _db = new Entities();

        // GET: DoctorsViews
        public ActionResult Index(long? id, string sortorder, int? page, string specialtyFilter, string rankFilter,
            string stateFilter, string countryId = "0", string yearFilter = "")
        {
            if (!Request.IsAuthenticated)
            {
                ViewBag.Error = "Please login in order to use search feature.";
                return View();
            }

            ViewBag.DRID = id;
            ViewBag.CurrentSort = sortorder;
            ViewBag.SpecialtyFilter = specialtyFilter;
            ViewBag.RankFilter = rankFilter;
            ViewBag.StateFilter = stateFilter;
            ViewBag.Country = countryId;
            ViewBag.YearFilter = yearFilter;

            ViewBag.DrIdSortParm = string.IsNullOrEmpty(sortorder) ? "drid_desc" : "";
            ViewBag.FirstNameSortParm = sortorder == "First Name" ? "fn_desc" : "First Name";
            ViewBag.LastNameSortParm = sortorder == "Last Name" ? "ln_desc" : "Last Name";
            ViewBag.RankSortParm = sortorder == "Rank" ? "rank_desc" : "Rank";
            ViewBag.PublicationsSortParm = sortorder == "Publications" ? "publications_desc" : "Publications";
            ViewBag.RecentDateSortParm = sortorder == "Recent Date" ? "recentdate_desc" : "Recent Date";
            ViewBag.SpecialtySortParm = sortorder == "Specialty" ? "specialty_desc" : "Specialty";
            ViewBag.AddressSortParm = sortorder == "Address" ? "address_desc" : "Address";
            ViewBag.CitySortParm = sortorder == "City" ? "city_desc" : "City";
            ViewBag.StateSortParm = sortorder == "State" ? "state_desc" : "State";

            var allDoctors = DoctorsList.Instance.Get();
            
            var specialties = SpecialtiesList.Instance.Get();
            var ranks = RanksList.Instance.Get();
            var states = StatesList.Instance.Get();
            var countries = CountriesList.Instance.GetNames();
            var years = RecentDate.Instance.Get();

            int fs, fr, fst, fc, fy;
            var doctors = ApplyFilters(specialtyFilter, rankFilter, stateFilter, yearFilter, countryId, allDoctors, specialties, ranks, states, countries, years, out fs,
                out fr, out fst, out fc, out fy);

            ViewBag.Specialties = DropDownHelper.ToSelectListItems(specialties, fs);
            ViewBag.Ranks = DropDownHelper.ToSelectListItems(ranks, fr);
            ViewBag.States = DropDownHelper.ToSelectListItems(states, fst);
            ViewBag.Countries = DropDownHelper.ToSelectListItems(countries, fc);
            ViewBag.Years = DropDownHelper.ToSelectListItems(years, fy);


            ViewBag.TotalRecords = doctors.Count;

            doctors = ApplySort(sortorder, doctors);

            ViewBag.CurrentSort = sortorder;

            const int pageSize = 20;
            var pageNumber = page ?? 1;

            ViewBag.PageNumber = pageNumber;

            if (countryId != "0")
            {
                if (countryId == CountriesList.Instance.GetIndexByCode("CAN"))
                {
                    _doctorsResultsCanada = DoctorsList.Instance.PrepareForCanada(doctors);
                }
                else
                {
                    _doctorsResultsOther = DoctorsList.Instance.PrepareForOther(doctors);
                }
            }
            else
            {
                // limiting USA to the doctors with publications since year 2000
                _doctorsResults = doctors.Where(d => DateTime.Parse(d.RecentDate).Year >= 2000).Select(d => d).ToList();
            }

            return View(doctors.ToPagedList(pageNumber, pageSize));
        }

        // Sort order logic
        private List<Doctors> ApplySort(string sortorder, List<Doctors> doctors)
        {
            switch (sortorder)
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
        private List<Doctors> ApplyFilters(string specialtyFilter, string rankFilter, string stateFilter, string yearFilter, string countryId,
            List<Doctors> allDoctors, IReadOnlyList<string> specialties, IReadOnlyList<string> ranks,
            IReadOnlyList<string> states, IReadOnlyList<string> countries, IReadOnlyList<string> years, out int fs, out int fr, out int fst,
            out int fc, out int fy)
        {
            List<Doctors> doctors;
            int c, ry;
            string countryCode, year = "";
            fc = -1;
            if (!countryId.IsNullOrWhiteSpace())
            {
                int.TryParse(countryId, out fc);
                c = fc;
                countryCode = AllCountries.Instance.GetCodeByName(countries[c]);
                if (countryCode != "USA")
                {
                    doctors =
                        allDoctors.Where(d => d.Country.Equals(countryCode))
                            .Select(d => d)
                            .ToList();
                }
                else
                {
                    doctors =
                     allDoctors.Where(d => d.Country.Equals(countryCode) && DateTime.Parse(d.RecentDate).Year >= 2000) // limiting USA doctors with publications since year 2000
                         .Select(d => d)
                         .ToList();
                }
            }
            else
            {
                c = 0;
                countryCode = AllCountries.Instance.GetCodeByName(countries[c]);
                doctors =
                    allDoctors.Where(d => d.Country.Equals(countryCode) && DateTime.Parse(d.RecentDate).Year >= 2000) // limiting USA doctors with publications since year 2000
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

            fy = -1;
            if (!yearFilter.IsNullOrWhiteSpace())
            {
                int.TryParse(yearFilter, out fy);
                ry = fy;
                year = years[ry];
                doctors =
                    doctors.Where(d => DateTime.Parse(d.RecentDate).Year.ToString().Equals(year))
                        .Select(d => d)
                        .ToList();
            }

            if (fs >= 0 && fr < 0 && fst < 0)
            {
                var i = fs;
                doctors =
                    allDoctors.Where(
                        d =>
                            d.Specialty.Equals(specialties[i]) ||
                            (!d.Taxonomy_Specialization.IsNullOrWhiteSpace() &&
                             d.Taxonomy_Specialization.Contains(specialties[i]))).Select(d => d).ToList();
                //TODO: filter to 2000
                if (fc >= 0)
                {
                    if (countryCode != "USA")
                    {
                        doctors =
                           doctors.Where(d => d.Country.Equals(countryCode))
                               .Select(d => d)
                               .ToList();
                    }
                    else
                    {
                        doctors =
                            doctors.Where(d => d.Country.Equals(countryCode) && DateTime.Parse(d.RecentDate).Year >= 2000)
                                .Select(d => d)
                                .ToList();
                    }
                }
                if (fy >= 0)
                {
                    doctors =
                        doctors.Where(d => DateTime.Parse(d.RecentDate).Year.ToString().Equals(year))
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
                    if (countryCode != "USA")
                    {
                        doctors =
                           doctors.Where(d => d.Country.Equals(countryCode))
                               .Select(d => d)
                               .ToList();
                    }
                    else
                    {
                        doctors =
                            doctors.Where(d => d.Country.Equals(countryCode) && DateTime.Parse(d.RecentDate).Year >= 2000)
                                .Select(d => d)
                                .ToList();
                    }
                }
                if (fy >= 0)
                {
                    doctors =
                        doctors.Where(d => DateTime.Parse(d.RecentDate).Year.ToString().Equals(year))
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
                    if (countryCode != "USA")
                    {
                        doctors =
                           doctors.Where(d => d.Country.Equals(countryCode))
                               .Select(d => d)
                               .ToList();
                    }
                    else
                    {
                        doctors =
                            doctors.Where(d => d.Country.Equals(countryCode) && DateTime.Parse(d.RecentDate).Year >= 2000)
                                .Select(d => d)
                                .ToList();
                    }
                }
                if (fy >= 0)
                {
                    doctors =
                        doctors.Where(d => DateTime.Parse(d.RecentDate).Year.ToString().Equals(year))
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
                    if (countryCode != "USA")
                    {
                        doctors =
                           doctors.Where(d => d.Country.Equals(countryCode))
                               .Select(d => d)
                               .ToList();
                    }
                    else
                    {
                        doctors =
                            doctors.Where(d => d.Country.Equals(countryCode) && DateTime.Parse(d.RecentDate).Year >= 2000)
                                .Select(d => d)
                                .ToList();
                    }
                }
                if (fy >= 0)
                {
                    doctors =
                        doctors.Where(d => DateTime.Parse(d.RecentDate).Year.ToString().Equals(year))
                            .Select(d => d)
                            .ToList();
                }
            }
            if (fr < 0 && fs >= 0 && fst >= 0)
            {
                var i = fs;
                var j = fst;
                doctors =
                    allDoctors.Where(d => (d.Specialty.Equals(specialties[i]) || d.Taxonomy_Specialization.Contains(specialties[i])) && d.State.Equals(states[j]))
                        .Select(d => d)
                        .ToList();
                if (fc >= 0)
                {
                    if (countryCode != "USA")
                    {
                        doctors =
                           doctors.Where(d => d.Country.Equals(countryCode))
                               .Select(d => d)
                               .ToList();
                    }
                    else
                    {
                        doctors =
                            doctors.Where(d => d.Country.Equals(countryCode) && DateTime.Parse(d.RecentDate).Year >= 2000)
                                .Select(d => d)
                                .ToList();
                    }
                }
                if (fy >= 0)
                {
                    doctors =
                        doctors.Where(d => DateTime.Parse(d.RecentDate).Year.ToString().Equals(year))
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
                    if (countryCode != "USA")
                    {
                        doctors =
                           doctors.Where(d => d.Country.Equals(countryCode))
                               .Select(d => d)
                               .ToList();
                    }
                    else
                    {
                        doctors =
                            doctors.Where(d => d.Country.Equals(countryCode) && DateTime.Parse(d.RecentDate).Year >= 2000)
                                .Select(d => d)
                                .ToList();
                    }
                }
                if (fy >= 0)
                {
                    doctors =
                        doctors.Where(d => DateTime.Parse(d.RecentDate).Year.ToString().Equals(year))
                            .Select(d => d)
                            .ToList();
                }
            }

            if (fs < 0 || fr < 0 || fst < 0 || fc < 0) return doctors; // fs >= 0 && fr >= 0 && fst >= 0

            var x = fs;
            var z = fr;
            var y = fst;
            doctors =
                allDoctors.Where(
                    d =>
                        (d.Specialty.Equals(specialties[x]) || d.Taxonomy_Specialization.Contains(specialties[x])) &&
                        (!ranks[z].IsNullOrWhiteSpace()
                            ? d.Rank.Equals(int.Parse(ranks[z]))
                            : d.Rank.ToString().Equals(ranks[z])) &&
                        d.State.Equals(states[y])).Select(d => d).ToList();
            if (fc >= 0)
            {
                if (countryCode != "USA")
                {
                    doctors =
                       doctors.Where(d => d.Country.Equals(countryCode))
                           .Select(d => d)
                           .ToList();
                }
                else
                {
                    doctors =
                        doctors.Where(d => d.Country.Equals(countryCode) && DateTime.Parse(d.RecentDate).Year >= 2000)
                            .Select(d => d)
                            .ToList();
                }
            }
            if (fy >= 0)
            {
                doctors =
                    doctors.Where(d => DateTime.Parse(d.RecentDate).Year.ToString().Equals(year))
                        .Select(d => d)
                        .ToList();
            }

            return doctors;
        }

        // GET: Doctors/Details/5
        public async Task<ActionResult> Details(long? id, string sortorder, int? page, string specialtyFilter,
            string rankFilter, string stateFilter, string yearFilter, string countryId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.DRID = id;
            ViewBag.CurrentSort = sortorder;
            ViewBag.PageNumber = page;
            ViewBag.SpecialtyFilter = specialtyFilter;
            ViewBag.RankFilter = rankFilter;
            ViewBag.StateFilter = stateFilter;
            ViewBag.YearFilter = yearFilter;
            ViewBag.Country = countryId;

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
        public async Task<ActionResult> Edit(long? id, string sortorder, int? page, string specialtyFilter,
            string rankFilter, string stateFilter, string yearFilter, string countryId)
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
            ViewBag.CurrentSort = sortorder;
            ViewBag.PageNumber = page;
            ViewBag.SpecialtyFilter = specialtyFilter;
            ViewBag.RankFilter = rankFilter;
            ViewBag.StateFilter = stateFilter;
            ViewBag.YearFilter = yearFilter;
            ViewBag.Country = countryId;

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
            long? id, string sortorder, int? page, string specialtyFilter, string rankFilter, string stateFilter, string yearFilter, string countryId)
        {
            ViewBag.DRID = id;
            ViewBag.CurrentSort = sortorder;
            ViewBag.PageNumber = page;
            ViewBag.SpecialtyFilter = specialtyFilter;
            ViewBag.RankFilter = rankFilter;
            ViewBag.StateFilter = stateFilter;
            ViewBag.YearFilter = yearFilter;
            ViewBag.Country = countryId;

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
                            sortorder = ViewBag.CurrentSort,
                            page = ViewBag.PageNumber,
                            specialtyFilter = ViewBag.SpecialtyFilter,
                            rankFilter = ViewBag.RankFilter,
                            stateFilter = ViewBag.StateFilter,
                            yearFilter = ViewBag.YearFilter,
                            countryId = ViewBag.Country
                        })
                    );
            }
            return View(consolidatedDoctorsView);
        }

        // GET: Doctors/Delete/5
        public async Task<ActionResult> Delete(long? id, string sortorder, int? page, string specialtyFilter,
            string rankFilter, string stateFilter, string countryId)
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
        public ActionResult CsvList(string sortorder, int? page, string specialtyFilter, string rankFilter,
            string stateFilter, string yearFilter, string countryId)
        {
            var timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var countryCode = AllCountries.Instance.GetCodeByName(CountriesList.Instance.GetCodeByIndex(countryId));

            if (countryCode != "USA")
            {
                if (countryCode == "CAN")
                {
                    return new CsvDownloader<DoctorsCanada>(_doctorsResultsCanada,
                        $"RSI-Export_{countryCode}_{timeStamp}.csv");
                }
                return new CsvDownloader<DoctorsOther>(_doctorsResultsOther,
                    $"RSI-Export_{countryCode}_{timeStamp}.csv");
            }
            return new CsvDownloader<Doctors>(_doctorsResults, 
                $"RSI-Export_{countryCode}_{timeStamp}.csv");
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