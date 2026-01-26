using MockMe.Model;
using System.Collections.Generic;

namespace MockMe.Common
{
    public class Constants
    {
        public const string Admin = "Admin";
        public const string BasicUser = "BasicUser";

        public static readonly IDictionary<string, string> USERS = new Dictionary<string, string> 
        {
            { "admin", "password" },
            { "test", "password" }
        };

        public static readonly IList<Country> COUNTRIES = new List<Country>
        {
            new Country { CountryId = 1, CountryName = "Australia", CountryCode = "AU" },
            new Country { CountryId = 2, CountryName = "Canada", CountryCode = "CA" },
            new Country { CountryId = 3, CountryName = "United Kingdom", CountryCode = "UK" },
            new Country { CountryId = 4, CountryName = "Germany", CountryCode = "DE" },
            new Country { CountryId = 5, CountryName = "France", CountryCode = "FR" },
            new Country { CountryId = 6, CountryName = "Israel", CountryCode = "IL" },
            new Country { CountryId = 7, CountryName = "Japan", CountryCode = "JP" },
            new Country { CountryId = 8, CountryName = "China", CountryCode = "CN" },
            new Country { CountryId = 9, CountryName = "India", CountryCode = "IN" },
            new Country { CountryId = 10, CountryName = "Brazil", CountryCode = "BR" }
        };
    }
}
