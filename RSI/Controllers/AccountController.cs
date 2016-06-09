using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RSI.Models;

namespace RSI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                var user2 = await UserManager.FindByNameAsync(model.UserName);

                if (user != null && user2 != null && user.Equals(user2))
                {
                    if (!user.EmailConfirmed)
                    {
                        ModelState.AddModelError("", "Confirm Email Address.");
                        return View(model);
                    }

                    var validCredentials = await UserManager.FindAsync(model.UserName, model.Password);

                    // When a user is lockedout, this check is done to ensure that even if the credentials are valid
                    // the user can not login until the lockout duration has passed

                    var message =
                        $"Your account has been locked out for {ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"]} minutes due to multiple failed login attempts.";

                    if (await UserManager.IsLockedOutAsync(user.Id))
                    {
                        message = buildLockoutMessage(user, message);
                        ModelState.AddModelError("", message);
                    }
                    // if user is subject to lockouts and the credentials are invalid
                    // record the failure and check if user is lockedout and display message, otherwise,
                    // display the number of attempts remaining before lockout
                    else if (await UserManager.GetLockoutEnabledAsync(user.Id) && validCredentials == null)
                    {
                        // Record the failure which also may cause the user to be locked out
                        await UserManager.AccessFailedAsync(user.Id);

                        if (await UserManager.IsLockedOutAsync(user.Id))
                        {
                            message = buildLockoutMessage(user, message);
                        }
                        else
                        {
                            var accessFailedCount = await UserManager.GetAccessFailedCountAsync(user.Id);

                            var attemptsLeft =
                                Convert.ToInt32(
                                    ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"]) -
                                accessFailedCount;

                            message =
                                $"Invalid credentials. You have {attemptsLeft} more attempt(s) before your account gets locked out.";
                        }

                        ModelState.AddModelError("", message);
                    }
                    else if (validCredentials == null)
                    {
                        ModelState.AddModelError("", "Invalid credentials. Please try again.");
                    }
                    else
                    {
                        var result =
                            await
                                SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe,
                                    true);

                        switch (result)
                        {
                            case SignInStatus.Success:
                                // When token is verified correctly, clear the access failed count used for lockout
                                await UserManager.ResetAccessFailedCountAsync(user.Id);
                                return RedirectToLocal(returnUrl);
                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode",
                                    new {ReturnUrl = returnUrl, model.RememberMe});
                            case SignInStatus.Failure:
                            default:
                                ModelState.AddModelError("", "Invalid login attempt.");
                                return View(model);
                        }
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private string buildLockoutMessage(ApplicationUser user, string message)
        {
            if (user.LockoutEndDateUtc == null) return message;
            var minutesLeft = ((DateTime) user.LockoutEndDateUtc).Subtract(DateTime.UtcNow).Minutes;
            var secondsLeft = ((DateTime) user.LockoutEndDateUtc).Subtract(DateTime.UtcNow).Seconds;

            if (minutesLeft < 0 && secondsLeft < 0) return message;
            if (minutesLeft == 1)
                message +=
                    $" {minutesLeft}:{("0" + secondsLeft).Substring(("0" + secondsLeft).Length - Math.Min(2, ("0" + secondsLeft).Length))} remaining.";
            else
            {
                message +=
                    $" {minutesLeft}:{("0" + secondsLeft).Substring(("0" + secondsLeft).Length - Math.Min(2, ("0" + secondsLeft).Length))} remaining.";
            }
            return message;
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel {Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe});
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result =
                await
                    SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe,
                        model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {UserName = model.UserName, Email = model.Email, EmailConfirmed = false};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // Send an email with this link
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var link = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code, email = user.Email},
                        Request.Url.Scheme);

                    using (var m = new MailMessage(
                        new MailAddress("register@linea.com", "Web Registration"),
                        new MailAddress(user.Email))
                    {
                        Subject = "Email confirmation",
                        Body = $"Dear {user.UserName} " +
                               "<BR/ > Thank you for your registration, please click on the below link to complete your registration: " +
                               $@"<a href =""{link}"" " +
                               $@"title =""User Email Confirm"">{link}</a>",
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8
                    })
                    {
                        using (var smtp = getSmtpClient())
                        {
                            try
                            {
                                smtp.Send(m);
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Length <= 0) return RedirectToAction("Index", "Home");
                                var rollback = await UserManager.DeleteAsync(user);
                                if (rollback.Succeeded)
                                    return RedirectToAction("FailedRegistration", "Account");
                            }
                        }
                    }
                    return RedirectToAction("ConfirmRegistration", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private SmtpClient getSmtpClient()
        {
            return new SmtpClient
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mazbros@hotmail.com", "xsttiger1091996^"),
                Timeout = 30000,
                Host = "smtp.live.com"
            };
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code, string email)
        {
            if (userId == null || code == null || email == null)
            {
                return View("Error");
            }

            var user = UserManager.FindById(userId);

            if (user.Email == email)
            {
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                return View(result.Succeeded ? "ConfirmEmail" : "Error");
            }
            return View("Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // Send an email with this link
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                var link = Url.Action("ResetPassword", "Account", new {userId = user.Id, code, email = user.Email},
                    Request.Url.Scheme);
                var smtp = getSmtpClient();

                using (var m = new MailMessage(
                    new MailAddress("register@linea.com", "Password Reset"),
                    new MailAddress(user.Email))
                {
                    Subject = "Password Reset",
                    Body = $"Dear {user.UserName} " +
                           $"<BR/ > Please reset your password by clicking here: " +
                           $"<a href =\"{link}\" " +
                           $"title =\"User Email Confirm\">{link}</a>",
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8
                })
                {
                    //smtp.ServerCertificateValidationCallback = () => true; //Solution for client certificate error
                    smtp.Send(m);
                }

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider,
                Url.Action("ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl}));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions =
                userFactors.Select(purpose => new SelectListItem {Text = purpose, Value = purpose}).ToList();
            return
                View(new SendCodeViewModel {Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe});
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode",
                new {Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe});
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new {ReturnUrl = returnUrl, RememberMe = false});
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel {Email = loginInfo.Email});
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        //
        // GET: /Account/ConfirmRegistration
        [AllowAnonymous]
        public ActionResult ConfirmRegistration()
        {
            return View();
        }

        //
        // GET: /Account/FailedRegistration
        [AllowAnonymous]
        public ActionResult FailedRegistration()
        {
            return View();
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XSRF_KEY = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties {RedirectUri = RedirectUri};
                if (UserId != null)
                {
                    properties.Dictionary[XSRF_KEY] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}