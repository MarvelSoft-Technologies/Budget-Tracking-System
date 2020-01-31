using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Budget_Tracking_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public static StandardResponse Register(Users NewUser)
        {
            try
            {
                if(NewUser == null)
                {
                    return StandardResponse.Error("An Error Occured");
                }
                if(NewUser.Insert() > 1)
                {
                    NewUser.SendActivationEmail();
                    return StandardResponse.Success(NewUser);
                }
                return StandardResponse.Error("There was an error creating your account please try again later");
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("verifyjoincode/{code}")]
        public ActionResult ValidateUser(string Code)
        {
            try
            {
                if((Code??"").Trim() != "")
                {
                    Users UserToValidate = Users.GetUsersByverifyCode(Code);
                    if (UserToValidate.Id > 1)
                    {
                        UserToValidate.IsActive = true;
                        UserToValidate.Update();
                        return Redirect(Globals.FrontendURL + "/emailconfirmed");
                    }
                    return BadRequest("User Not Found");
                }
                return BadRequest("URl Expired. Please contact support for help.");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("authenticate")]
        public static StandardResponse Authenticate(string Email, string Password)
        {
            if(Email != null && Password != null)
            {
               return StandardResponse.Success(Users.Authenticate(Email, Password)); 
            }
            return StandardResponse.Error(); 
        }

    }
}