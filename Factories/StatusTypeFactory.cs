using Buisness.Models;
using DataStorage.Entities;

namespace Buisness.Factories
{
    public static class StatusTypeFactory
    {
        public static StatusTypeEntity? Create(StatusTypeRegistrationForm? form) => form is null ? null : new StatusTypeEntity()
        {
            Statusname = form.Statusname
        };

        public static StatusType? Create(StatusTypeEntity? entity) => entity is null ? null : new StatusType()
        {
            Id = entity.Id,
            Statusname = entity.Statusname
        };
    }
}
