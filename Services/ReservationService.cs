/*
 * This is the service class that provide the services to ReservationsController
 */
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ReservationService
    {
        /* The way of initiating mongodb and implementeing mongo collection were referred using microsoft documentation
        https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-7.0&tabs=visual-studio */
        private readonly IMongoCollection<Reservation> _reservationsCollection;

        public ReservationService(IOptions<TravelDbDatabaseSettings> TravelDbDatabaseSettings)
        {
            // Constructor to initialize the MongoDB collection
            var mongoClient = new MongoClient(TravelDbDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                TravelDbDatabaseSettings.Value.DatabaseName
            );
            _reservationsCollection = mongoDatabase.GetCollection<Reservation>(
                TravelDbDatabaseSettings.Value.ReservationsCollectionName
            );
        }

        // Get all reservations
        public async Task<List<Reservation>> GetAllReservationsAsync() =>
            await _reservationsCollection.Find(_ => true).ToListAsync();

        // Get a reservation by its ID
        public async Task<Reservation> GetReservationByIdAsync(string ReserveId) =>
            await _reservationsCollection
                .Find(reservation => reservation.ReserveId == ReserveId)
                .FirstOrDefaultAsync();

        // Create a new reservation
        public async Task CreateReservationAsync(Reservation newReservation)
        {
            // Add additional validation logic if needed
            await _reservationsCollection.InsertOneAsync(newReservation);
        }

        // Update an existing reservation
        public async Task UpdateReservationAsync(string ReserveId, Reservation updatedReservation)
        {
            // Find the existing reservation
            var existingReservation = await _reservationsCollection
                .Find(reservation => reservation.ReserveId == ReserveId)
                .FirstOrDefaultAsync();

            if (existingReservation == null)
            {
                throw new ArgumentException("Reservation not found.");
            }

            // Check if the reservation date is at least 5 days before the reservation date
            if ((updatedReservation.ReservationDate - DateTime.Now).Days < 5)
            {
                throw new ArgumentException(
                    "Reservation date must be at least 5 days before the current date."
                );
            }

            // Update reservation details
            existingReservation.ReservationDate = updatedReservation.ReservationDate;
            // Update other reservation details as needed

            await _reservationsCollection.ReplaceOneAsync(
                reservation => reservation.ReserveId == ReserveId,
                existingReservation
            );
        }

        // Delete a reservation by its ID
        public async Task DeleteReservationAsync(string ReserveId)
        {
            // Find the existing reservation
            var existingReservation = await _reservationsCollection
                .Find(reservation => reservation.ReserveId == ReserveId)
                .FirstOrDefaultAsync();

            if (existingReservation == null)
            {
                throw new ArgumentException("Reservation not found.");
            }

            // Check if the reservation date is at least 5 days before the reservation date
            if ((existingReservation.ReservationDate - DateTime.Now).Days < 5)
            {
                throw new ArgumentException(
                    "Reservation can only be canceled at least 5 days before the reservation date."
                );
            }

            // Delete the reservation
            await _reservationsCollection.DeleteOneAsync(
                reservation => reservation.ReserveId == ReserveId
            );
        }

        // Get the count of reservations by reference ID
        public long GetReservationsCountByReferenceId(string referenceId)
        {
            return _reservationsCollection
                .Find(reservation => reservation.ReferenceId == referenceId)
                .CountDocuments();
        }

        // Get the reservations for a trveller by reservation ID
        public async Task<List<Reservation>> GetReservationsForTraveler(List<string> reservationIds)
        {
            return await _reservationsCollection
                .Find(reservation => reservationIds.Contains(reservation.ReserveId))
                .ToListAsync();
        }
    }
}
