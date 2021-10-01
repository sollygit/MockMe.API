using System.Collections.Generic;

namespace MockMe.Common
{
    public class Constants
    {
        public static readonly List<KeyValuePair<int, string>> DIRECTIONS = new List<KeyValuePair<int, string>> 
        {
            new KeyValuePair<int, string>(1, "UP"),
    		new KeyValuePair<int, string>(2, "DOWN")
        };

        public static readonly IDictionary<string, string> USERS = new Dictionary<string, string>
        {
            { "admin", "password" },
            { "test1", "password1" },
            { "test2", "password2" }
        };
    }
}
