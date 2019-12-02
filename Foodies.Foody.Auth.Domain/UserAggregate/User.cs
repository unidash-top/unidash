﻿using Foodies.Foody.Core.Domain;

namespace Foodies.Foody.Auth.Domain.UserAggregate
{
    public class User : Entity
    {
        public string DisplayName { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string PasswordSalt { get; set; }
    }
}