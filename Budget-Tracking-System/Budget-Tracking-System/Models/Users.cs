using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Budget_Tracking_System
{
    public class Users
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerificationCode { get; set; }
        public bool IsAuditor { get; set; }
        public byte[] Salt { get; set; }
        public byte[] Key { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }

        public int Insert()
        {
            SetPassword();
            DateCreated = DateTime.Now;
            DataContext.Insert(this);
            return Id;
        }

        public static Users GetById(int Id)
        {
            return DataContext.FirstOrDefault<Users>("; select * from users where id = @0", Id);
        }

        public void SetPassword()
        {
            byte[] SaltKey;
            Salt = Encryptions.HashPassword(Password, out SaltKey);
            Key = SaltKey;
            Password = null;//Salt and key combination will retrieve password
        }

        public bool Update()
        {
            try
            {
                DataContext.Update(this);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        public static Users Authenticate(string Email, string password,bool StayLoggedIn = false)
        {
            try
            {
                var user = Users.Get(Email);
                if (user == null) return null;

                if (!Encryptions.VerifyPassword(password, user.Salt, user.Key))
                {
                    return null;
                }
                return user.Stripped();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public Users Stripped()
        {
            try
            {
                Key = new byte[] { };
                Salt = new byte[] { };
                Password = "";
                VerificationCode = "";
                return this;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }



        public static Users GetUsersByverifyCode(string Code)
        {
            try
            {
                return DataContext.
                     FirstOrDefault<Users>("; select * from Users where VerificationCode = @0", Code);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public static Users Get(string Email)
        {
            try
            {
                return DataContext
                    .FirstOrDefault<Users>("; select * from Users where email = @0", Email);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public bool SendActivationEmail()
        {
            try
            {

                VerificationCode = Codes.New("email-confirmation", 15);
                string encoded_email_code = HttpUtility.UrlEncode(VerificationCode);
                string Url = Globals.BaseURL + "api/v1/Users/VerifyJoinCode/" + encoded_email_code;
                if (Update())
                {
                    List<KeyValuePair<string, string>> EmailParameters = new List<KeyValuePair<string, string>>();
                    EmailParameters.Add(new KeyValuePair<string, string>("URL", Url));
                    string EmailHTML = Budget_Tracking_System.Email.ComposeFromTemplate("new-user.html", EmailParameters);
                    return Budget_Tracking_System.Email.SendMessage("Activate your account", EmailHTML, Email);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return false;
        }
    }
}
