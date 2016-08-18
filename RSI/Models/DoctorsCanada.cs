using System.Collections;

namespace RSI.Models
{
    public class DoctorsCanada : IEnumerable
    {
        public long DRID { get; set; }
        public int? Rank { get; set; }
        public int? Publications { get; set; }
        public string RecentDate { get; set; }
        public long? REVIEWER_ID { get; set; }
        public string Specialty { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Postalcode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email_Address { get; set; }
        public string Company_Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Timezone { get; set; }
        public string Website { get; set; }
        public string Gender { get; set; }
        public string Credentials { get; set; }
        public string Taxonomy_Code { get; set; }
        public string Taxonomy_Classification { get; set; }
        public string Taxonomy_Specialization { get; set; }
        public string Medical_School { get; set; }
        public string Residency_Training { get; set; }
        public string Graduation_Year { get; set; }
        public int? Patients { get; set; }
        public int? Claims { get; set; }
        public int? Prescriptions { get; set; }
        public string Country { get; set; } = "CAN";

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }

        private DoctorsCanada GetEnumerator()
        {
            return new DoctorsCanada();
        }


    }

}