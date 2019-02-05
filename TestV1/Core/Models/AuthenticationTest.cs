using System;
using OnlineShopV1;

namespace TestV1.Models
{
    public class AuthenticationTest
    {


        public static Authentication GetTestAuth()
        {
            var auth = new Authentication
            {
                Code = "some_random_string",
                Expires = DateTime.Now,
                Username = "omid",
                UserType = UserType.Admin,
                ID = 1
            };

            return auth;
        }
    }
}