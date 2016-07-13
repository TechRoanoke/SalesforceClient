using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Helpers
{
    public class ConnectionString
    {
        public string userName;
        public string password;
        public string passwordSecurityToken;
        public string consumerKey;
        public string consumerSecret;


        public const string Cookie = "SF1";
        // Get as a single connection string 
        public string ToConnectionString()
        {
            return string.Join(";", Cookie, userName, password, passwordSecurityToken, consumerKey, consumerSecret);
        }

        public static ConnectionString FromConnectionString(string cx)
        {
            var parts = cx.Split(';');
            if (parts.Length != 6 || parts[0] != Cookie)
            {
                throw new InvalidOperationException("Invalid SF connection string");
            }
            return new ConnectionString
            {
                userName = parts[1],
                password = parts[2],
                passwordSecurityToken = parts[3],
                consumerKey = parts[4],
                consumerSecret = parts[5]
            };
        }

        public async Task<ForceClient> ToClientAsync()
        {
            var auth = new AuthenticationClient();

            await auth.UsernamePasswordAsync(consumerKey, consumerSecret, userName, password + passwordSecurityToken);

            var forceClient = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            return forceClient;
        }
    }
}
