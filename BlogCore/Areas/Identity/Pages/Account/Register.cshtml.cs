using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlogCore.Models;
using BlogCore.Utilidades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace BlogCore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        //uso roles en mi app asi que tengo que instanciar RoleManager: tablas que relacionan roles y usuarios
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;


            //uso roles en mi app asi que tengo que instanciar RoleManager
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "EL {0} debe ser al m enos de {2} y máximo {1} de caracteres de longitud.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
            public string ConfirmPassword { get; set; }


            //agrego al inputModel los campos que agregue a la tabla
            public string Nombre { get; set; }
            public string Direccion { get; set; }

            public string Ciudad { get; set; }

            public string Pais { get; set; }

            public string PhoneNumber { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                //uso el ApplicationUser que es la clase que cree con los campos agregados e instancio todos sus campos
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Nombre = Input.Nombre,
                    Ciudad = Input.Ciudad,
                    Direccion = Input.Direccion,
                    Pais = Input.Pais,
                    PhoneNumber = Input.PhoneNumber,
                    //se autologuee una vez que lo campos esten validados
                    EmailConfirmed = true
                };

                //obtengo resultado del password
                var result = await _userManager.CreateAsync(user, Input.Password);

                //si el resultado es exitoso
                if (result.Succeeded)
                {
                    //valido rol, si no existe, lo creo
                    if (!await _roleManager.RoleExistsAsync(CNT.Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(CNT.Admin));
                        await _roleManager.CreateAsync(new IdentityRole(CNT.Usuario));
                    }

                    //obtener el rol seleccionado en el radio Button del form
                    string rol = Request.Form["radUsuarioRole"].ToString();

                    //valido si el rol seleccionado es Admin y si lo es lo agrego a userManager como admin
                    if (rol == CNT.Admin)
                    {
                        await _userManager.AddToRoleAsync(user, CNT.Admin);
                    }
                    //valido si el rol seleccionado es Admin y si lo es lo agrego a userManager cono usuario
                    else
                    {
                        if (rol == CNT.Usuario)
                        {
                            await _userManager.AddToRoleAsync(user, CNT.Usuario);
                        }
                    }

                    //informa
                    _logger.LogInformation("Usuario creado con exito.");

                    //se agrega el usuario a la bbdd
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    //retorna a la url
                    return LocalRedirect(returnUrl);

                    //codigo que genera mail de confirmacion
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
