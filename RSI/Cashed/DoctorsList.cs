using System;
using System.Collections.Generic;
using System.Linq;
using RSI.Models;

namespace RSI.Cashed
{
    public class DoctorsList
    {
        private static Lazy<DoctorsList> _inst = new Lazy<DoctorsList>();

        private List<Doctors> _doctors;
        private List<DoctorsCanada> _doctorsCanada;
        private List<DoctorsOther> _doctorsOther;
        public static DoctorsList Instance => _inst.Value;

        public List<Doctors> Get()
        {
            if (_doctors != null) return _doctors;
            var db = new Entities();
            var doctors = db.Doctors.Select(d => d).ToList();
            _doctors = doctors.Where(d => DateTime.Parse(d.RecentDate).Year >= 2000).ToList();
            return _doctors;
        }

        public static void Reset()
        {
            _inst = new Lazy<DoctorsList>();
        }

        public List<DoctorsCanada> PrepareForCanada(IEnumerable<Doctors> doctors)
        {
            _doctorsCanada = new List<DoctorsCanada>();
            foreach (var d in doctors)
            {
                var item = new DoctorsCanada
                {
                    DRID = d.DRID,
                    Rank = d.Rank,
                    Publications = d.Publications,
                    RecentDate = d.RecentDate,
                    REVIEWER_ID = d.REVIEWER_ID,
                    Specialty = d.Specialty,
                    First_Name = d.First_Name,
                    Last_Name = d.Last_Name,
                    Address = d.Address,
                    City = d.City,
                    Province = d.State,
                    Postalcode = d.Zipcode,
                    Phone = d.Phone,
                    Fax = d.Fax,
                    Email_Address = d.Email_Address,
                    Company_Name = d.Company_Name,
                    Latitude = d.Latitude,
                    Longitude = d.Longitude,
                    Timezone = d.Timezone,
                    Website = d.Website,
                    Gender = d.Gender,
                    Credentials = d.Credentials,
                    Taxonomy_Code = d.Taxonomy_Code,
                    Taxonomy_Classification = d.Taxonomy_Classification,
                    Taxonomy_Specialization = d.Taxonomy_Specialization,
                    Medical_School = d.Medical_School,
                    Residency_Training = d.Residency_Training,
                    Graduation_Year = d.Graduation_Year,
                    Patients = d.Patients,
                    Claims = d.Claims,
                    Prescriptions = d.Prescriptions
                };
                _doctorsCanada.Add(item);
            }
            return _doctorsCanada;
        }

        public List<DoctorsOther> PrepareForOther(IEnumerable<Doctors> doctors)
        {
            _doctorsOther = new List<DoctorsOther>();
            foreach (var d in doctors)
            {
                var item = new DoctorsOther
                {
                    DRID = d.DRID,
                    Rank = d.Rank,
                    Publications = d.Publications,
                    RecentDate = d.RecentDate,
                    REVIEWER_ID = d.REVIEWER_ID,
                    Specialty = d.Specialty,
                    First_Name = d.First_Name,
                    Last_Name = d.Last_Name,
                    Address = d.Address,
                    City = d.City,
                    Postalcode = d.Zipcode,
                    Phone = d.Phone,
                    Fax = d.Fax,
                    Email_Address = d.Email_Address,
                    Company_Name = d.Company_Name,
                    Latitude = d.Latitude,
                    Longitude = d.Longitude,
                    Timezone = d.Timezone,
                    Website = d.Website,
                    Gender = d.Gender,
                    Credentials = d.Credentials,
                    Taxonomy_Code = d.Taxonomy_Code,
                    Taxonomy_Classification = d.Taxonomy_Classification,
                    Taxonomy_Specialization = d.Taxonomy_Specialization,
                    Medical_School = d.Medical_School,
                    Residency_Training = d.Residency_Training,
                    Graduation_Year = d.Graduation_Year,
                    Patients = d.Patients,
                    Claims = d.Claims,
                    Prescriptions = d.Prescriptions,
                    Country = d.Country
                };
                _doctorsOther.Add(item);
            }
            return _doctorsOther;
        }
    }
}