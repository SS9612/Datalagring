using Buisness.Models;
using DataStorage.Entities;

namespace Buisness.Factories
{
    public static class UserFactory
    {
        public static UserEntity? Create(UserRegistrationForm? form) => form == null ? null : new UserEntity()
        {
            FirstName = form.Firstname,
            LastName = form.Lastname,
            Email = form.Email
        };

        public static User? Create(UserEntity? entity) => entity == null ? null : new User()
        {
            Id = entity.Id,
            Firstname = entity.FirstName,
            Lastname = entity.LastName,
            Email = entity.Email
        };
    }
}
