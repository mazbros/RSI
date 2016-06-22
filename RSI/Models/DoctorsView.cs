using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.DynamicData;

namespace RSI.Models
{
    public class DoctorsView : Doctors
    {
        public string Grouping { get; set; }
        public string Classification { get; set; }
        public string Specialization { get; set; }

        public DoctorsView(IList<object> list) 
        {
            DRID = (long) list[0];
            Rank = (int) list[1];
            Publications = (int) list[2];
            RecentDate = (string) list[3];
            NPI = (string) list[4];
            REVIEWER_ID = (long) list[5];
            Specialty = (string) list[6];
            First_Name = (string) list[7];
            Last_Name = (string) list[8];
            Address = (string) list[9];
            City = (string) list[10];
            State = (string) list[11];
            Zipcode = (string) list[12];
            Phone = (string) list[13];
            Fax = (string) list[14];
            Email_Address = (string) list[15];
            County = (string) list[16];
            Company_Name = (string)list[17];
            Latitude = (double)list[18];
            Longitude = (double)list[19];
            Timezone = (string)list[20];
            Website = (string)list[21];
            Gender = (string)list[22];
            Credentials = (string)list[23];
            Taxonomy_Code = (string)list[24];
            License_Number = (string)list[25];
            License_State = (string)list[26];
            Grouping = (string)list[27];
            Classification = (string)list[28];
            Specialization = (string) list[29];
        }
    }
}
