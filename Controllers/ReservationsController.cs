/*
 * This controller is used to manage the user reservations
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Service.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

// Defining the reservation API controller
[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly ReservationService _reservationService;
    private readonly UsersService _usersService;

    public ReservationController(ReservationService reservationService, UsersService usersService )
    {
        _reservationService = reservationService;
        _usersService = usersService;
    }

    
     // This method handles the retrieval of all reservations.
     
    [HttpGet]
    [Authorize(Roles = "office")]
    public async Task<IActionResult> GetAllReservations()
    {
        var reservations = await _reservationService.GetAllReservationsAsync();
        return Ok(reservations);
    }

    
     // This method handles the retrieval of a reservation by its ID.
     
    [HttpGet]
    [Route("getUsers/{ReserveId}")]
    [Authorize(Roles = "agent,travelller,office")]
    public async Task<IActionResult> GetReservationById(string ReserveId)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(ReserveId);
        if (reservation == null)
        {
            return NotFound();
        }
        return Ok(reservation);
    }


    // This method retrieves existing reservations for a traveler.
    [HttpGet("reservations")]
    public async Task<IActionResult> GetTravelerReservations()
    {
        var travelerNIC = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(travelerNIC))
        {
            return Unauthorized();
        }

        var traveler = await _usersService.GetAsync(travelerNIC);
        if (traveler == null)
        {
            return NotFound("Traveler not found.");
        }

        var reservations = await _reservationService.GetReservationsForTraveler(traveler.ReservationIds);
        return Ok(reservations);
    }

    // This method handles the creation of a new reservation.

    [HttpPost]
    [Route("addReservations")]
    [Authorize(Roles = "agent,travelller,office")]
    public async Task<IActionResult> CreateReservation(Reservation reservation)
    {
        try
        {
            // Check if the reservation date is within 30 days from the booking date
            if ((reservation.ReservationDate - DateTime.Now).Days > 30)
            {
                return BadRequest("Reservation date must be within 30 days from the booking date.");
            }

            // Check if the traveler has already made the maximum allowed reservations (4)
            var existingReservations = _reservationService.GetReservationsCountByReferenceId(reservation.ReferenceId);

            if (existingReservations >= 4)
            {
                return BadRequest("Maximum 4 reservations per reference ID are allowed.");
            }
            var user = await _usersService.GetAsync(reservation.ReferenceId);
            if (user != null)
            {
                user.ReservationIds.Add(reservation.ReserveId);
                await _usersService.UpdateAsync(user.NIC, user);
            }

            await _reservationService.CreateReservationAsync(reservation);

            return Ok("Reservation created successfully.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    
     // This method handles the update of an existing reservation.
     
    [HttpPut]
    [Route("updateReservations/{ReserveId}")]
    [Authorize(Roles = "agent,travelller,office")]
    public async Task<IActionResult> UpdateReservation(string ReserveId, Reservation reservation)
    {
        try
        {
            await _reservationService.UpdateReservationAsync(ReserveId, reservation);
            return Ok("Reservation updated successfully.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    
     // This method handles the cancellation of an existing reservation.
     
    [HttpDelete]
    [Route("cancelReservations/{ReserveId}")]
    [Authorize(Roles = "agent,travelller,office")]
    public async Task<IActionResult> CancelReservation(string ReserveId)
    {
        try
        {
            await _reservationService.DeleteReservationAsync(ReserveId);
            return Ok("Reservation canceled successfully.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
