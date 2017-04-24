using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskClient
{
    public static class Globals
    {
        public static string taskServiceUrl = "https://bizzaro.azurewebsites.net";
        public static string aadInstance = "https://login.microsoftonline.com/";
        public static string redirectUri = "urn:ietf:wg:oauth:2.0:oob";

        // TODO: Replace these five default with your own configuration values
        public static string tenant = "cryptonitehsaservice.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_HSA_SignUp_SignIn_Default";
        public static string clientId = "6a99c57e-3489-4e91-a398-e93f8cec4af8";
        public static string signInPolicy = "B2C_1_HSA_SignUp_SignIn_Default";
        public static string signUpPolicy = "B2C_1_HSA_SignUp_SignIn_Default";
        public static string editProfilePolicy = "B2C_1_HsaPasswordResetDefault";

        public static string authority = string.Concat(aadInstance, tenant);

    }
}
