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
            _doctors = doctors;
            return _doctors;
        }

        public static void Reset()
        {
            _inst = new Lazy<DoctorsList>();
        }

        public List<DoctorsCanada> PrepareForCanada(List<Doctors> doctors)
        {
            _doctorsCanada = new List<DoctorsCanada>();
            foreach (var d in doctors)
            {
                var item = new DoctorsCanada();
                {
                
                item.DRID = d.DRID;
                item.Rank = d.Rank;
                item.Publications = d.Publications;
                item.RecentDate = d.RecentDate;
                item.REVIEWER_ID = d.REVIEWER_ID;
                item.Specialty = d.Specialty;
                item.First_Name = d.First_Name;
                item.Last_Name = d.Last_Name;
                item.Address = d.Address;
                item.City = d.City;
                item.Province = d.State;
                item.Postalcode = d.Zipcode;
                item.Phone = d.Phone;
                item.Fax = d.Fax;
                item.Email_Address = d.Email_Address;
                item.Company_Name = d.Company_Name;
                item.Latitude = d.Latitude;
                item.Longitude = d.Longitude;
                item.Timezone = d.Timezone;
                item.Website = d.Website;
                item.Gender = d.Gender;
                item.Credentials = d.Credentials;
                item.Taxonomy_Code = d.Taxonomy_Code;
                item.Taxonomy_Classification = d.Taxonomy_Classification;
                item.Taxonomy_Specialization = d.Taxonomy_Specialization;
                item.Medical_School = d.Medical_School;
                item.Residency_Training = d.Residency_Training;
                item.Graduation_Year = d.Graduation_Year;
                item.Patients = d.Patients;
                item.Claims = d.Claims;
                item.Prescriptions = d.Prescriptions;
            }

            _doctorsCanada.Add(item);
            }
            return _doctorsCanada;
        }

        public List<DoctorsOther> PrepareForOther(List<Doctors> doctors)
        {
            _doctorsOther = new List<DoctorsOther>();
            foreach (var d in doctors)
            {
                var item = new DoctorsOther();
                {

                    item.DRID = d.DRID;
                    item.Rank = d.Rank;
                    item.Publications = d.Publications;
                    item.RecentDate = d.RecentDate;
                    item.REVIEWER_ID = d.REVIEWER_ID;
                    item.Specialty = d.Specialty;
                    item.First_Name = d.First_Name;
                    item.Last_Name = d.Last_Name;
                    item.Address = d.Address;
                    item.City = d.City;
                    item.Postalcode = d.Zipcode;
                    item.Phone = d.Phone;
                    item.Fax = d.Fax;
                    item.Email_Address = d.Email_Address;
                    item.Company_Name = d.Company_Name;
                    item.Latitude = d.Latitude;
                    item.Longitude = d.Longitude;
                    item.Timezone = d.Timezone;
                    item.Website = d.Website;
                    item.Gender = d.Gender;
                    item.Credentials = d.Credentials;
                    item.Taxonomy_Code = d.Taxonomy_Code;
                    item.Taxonomy_Classification = d.Taxonomy_Classification;
                    item.Taxonomy_Specialization = d.Taxonomy_Specialization;
                    item.Medical_School = d.Medical_School;
                    item.Residency_Training = d.Residency_Training;
                    item.Graduation_Year = d.Graduation_Year;
                    item.Patients = d.Patients;
                    item.Claims = d.Claims;
                    item.Prescriptions = d.Prescriptions;
                    item.Country = d.Country;
                }

                _doctorsOther.Add(item);
            }
            return _doctorsOther;
        }
    }
}