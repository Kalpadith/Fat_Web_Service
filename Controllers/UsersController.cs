/*
 * This controller is used to manage the users
 */
using Service.Models;
using Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Service.Controllers;

// Defining the user API controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }

    // This method handles the retrieval of all users.

    [HttpGet]
    [Route("getUser")]
    [Authorize(Roles = "office")]
    public async Task<List<User>> Get() => await _usersService.GetAsync();

    // This method handles the retrieval of a user by their NIC.

    [HttpGet]
    [Route("getUsers/{NIC}")]
    [Authorize(Roles = "agent, travelller, office")]
    public async Task<ActionResult<User>> Get(string NIC)
    {
        var user = await _usersService.GetAsync(NIC);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    // This method handles the creation of a new user.

    [HttpPost]
    [Route("addUsers")]
    [Authorize(Roles = "agent, office")]
    public async Task<IActionResult> Post(User newUser)
    {
        await _usersService.CreateAsync(newUser);

        return CreatedAtAction(nameof(Get), new { NIC = newUser.NIC }, newUser);
    }

    // This method handles the update of an existing user.

    [HttpPut]
    [Route("updateUsers/{NIC}")]
    [Authorize(Roles = "agent, office")]
    public async Task<IActionResult> Update(string NIC, User updatedUser)
    {
        var user = await _usersService.GetAsync(NIC);

        if (user is null)
        {
            return NotFound();
        }

        // Iterate through the properties of the updatedUser object and update only non-null values.
        var userProperties = typeof(User).GetProperties();
        foreach (var property in userProperties)
        {
            var newValue = property.GetValue(updatedUser);
            if (newValue != null)
            {
                property.SetValue(user, newValue);
            }
        }

        await _usersService.UpdateAsync(NIC, user);

        return NoContent();
    }

    // This method handles the deletion of an existing users.

    [HttpDelete]
    [Route("deleteUsers/{NIC}")]
    [Authorize(Roles = "office")]
    public async Task<IActionResult> Delete(string NIC)
    {
        var user = await _usersService.GetAsync(NIC);

        if (user is null)
        {
            return NotFound();
        }

        await _usersService.RemoveAsync(NIC);

        return NoContent();
    }
}
