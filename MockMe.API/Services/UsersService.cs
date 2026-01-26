using Microsoft.Extensions.Logging;
using MockMe.Common;

namespace MockMe.API.Services
{
    public interface IUserService
    {
        bool IsAnExistingUser(string userName);
        bool IsValidUserCredentials(string userName, string password);
        string GetUserRole(string userName);
    }

    public class UserService : IUserService
    {
        readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        public bool IsValidUserCredentials(string userName, string password)
        {
            _logger.LogDebug("Validating user credentials for {UserName}", userName);
            if (string.IsNullOrWhiteSpace(userName)) return false;

            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            return Constants.USERS.TryGetValue(userName, out var p) && p == password;
        }

        public bool IsAnExistingUser(string userName)
        {
            return Constants.USERS.ContainsKey(userName);
        }

        public string GetUserRole(string userName)
        {
            if (!IsAnExistingUser(userName))
            {
                return string.Empty;
            }

            if (userName == "admin")
            {
                return Constants.Admin;
            }

            return Constants.BasicUser;
        }
    }
}
