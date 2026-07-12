using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApi.Controllers;

namespace RealEstateApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountServiceForWebApi _accountServiceForWebApi;

        public AccountController(IAccountServiceForWebApi accountServiceForWebApi)
        {
            _accountServiceForWebApi = accountServiceForWebApi;
        }

        /// <summary>Autentica un usuario Administrador o Desarrollador y devuelve el token JWT con usuario, roles y expiración</summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseForApiDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _accountServiceForWebApi.AuthenticateAsync(dto);

                if (response.IsForbidden)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { errors = response.Errors });
                }

                if (response.HasError)
                {
                    return Unauthorized(new { errors = response.Errors });
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Registra un usuario con rol Administrador; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegisterResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequestDto dto)
        {
            return await RegisterAsync(dto, Roles.Admin);
        }

        /// <summary>Registra un usuario con rol Desarrollador; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("register-developer")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegisterResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterDeveloper([FromBody] RegisterRequestDto dto)
        {
            return await RegisterAsync(dto, Roles.Developer);
        }

        private async Task<IActionResult> RegisterAsync(RegisterRequestDto dto, Roles role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _accountServiceForWebApi.RegisterAsync(dto, role.ToString());

                if (response.HasError)
                {
                    return BadRequest(new { errors = response.Errors });
                }

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }
    }
}
