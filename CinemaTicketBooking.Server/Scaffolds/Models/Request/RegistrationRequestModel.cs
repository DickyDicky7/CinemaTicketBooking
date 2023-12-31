﻿using CinemaTicketBooking.Server.Scaffolds.Models.EntityLayer;
using System.Collections.Specialized;

namespace CinemaTicketBooking.Server.Scaffolds.Models.ModelLayer
{
    public class RegistrationRequestModel
    {
        // Define the properties you expect from the registration request
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        // Add additional fields as necessary, like Email, FullName, etc.

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public RoleEnum? Role { get; set; }

    }
}
