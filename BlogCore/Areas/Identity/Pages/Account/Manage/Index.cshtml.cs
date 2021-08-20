using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BlogCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogCore.Areas.Identity.Pages.Account.Manage
{
    //uso la clase creada propia para usuarios: aplication user que hereda de aplication user
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Display(Name ="Email de Usuario")]
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Telefono")]
            public string PhoneNumber { get; set; }

            //copiar los campos de applicationUser.cs y pegarlos aqui
            
            public string Nombre { get; set; }
            public string Direccion { get; set; }
            
            public string Ciudad { get; set; }
           
            public string Pais { get; set; }
        }

        //private async Task LoadAsync(ApplicationUser user)
        //{
        //    var userName = await _userManager.GetUserNameAsync(user);
        //    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        //    Username = userName;

        //    //añado los campos que agregue de aplicationuser
        //    Input = new InputModel
        //    {
        //        //aparecen los campos previamente llenados para que yo los pueda cambiar
        //        PhoneNumber = phoneNumber,
        //        Nombre = user.Nombre,
        //        Ciudad = user.Ciudad,
        //        Direccion = user.Direccion,
        //        Pais = user.Pais
        //    };
        //}

        //obtiene los datos del usuario de la bbdd y los previsualiza en el form del view index
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            //añado los campos que agregue de aplicationuser
            Input = new InputModel
            {
                //aparecen los campos previamente llenados para que yo los pueda cambiar
                PhoneNumber = phoneNumber,
                Nombre = user.Nombre,
                Ciudad = user.Ciudad,
                Direccion = user.Direccion,
                Pais = user.Pais
            };


            //await LoadAsync(user);
            return Page();
        }

        //al darle a guardar cambio submit, envia los datos a la bbdd y los guarda alli
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //if (!ModelState.IsValid)
            //{
            //    await LoadAsync(user);
            //    return Page();
            //}

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            user.Nombre = Input.Nombre;
            user.PhoneNumber = Input.PhoneNumber;
            user.Ciudad = Input.Ciudad;
            user.Direccion = Input.Direccion;
            user.Pais = Input.Pais;

            //guarda el user con sus campos actualizados
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "su perfil esta actualizado";
            return RedirectToPage();
        }
    }
}
