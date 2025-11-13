using Ticklette.Domain.Models;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;

namespace Ticklette.DTOs.Mappers;

public static class EventMappers
{
    public static EventResponse ToEventResponse(this Event eventEntity)
    {
        return new EventResponse
        {
            EventId = eventEntity.EventId,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            DateTime = eventEntity.DateTime,
            Location = eventEntity.Location,
            Type = eventEntity.Type,
            Status = eventEntity.Status,
            ImageUrl = eventEntity.ImageUrl,
            OrganizingHouseId = eventEntity.OrganizingHouseId
            // CreatedAt se puede agregar si tienes ese campo en la entidad
        };
    }

    public static Event ToEventEntity(this CreateEventRequest request)
    {
        return new Event
        {
            Name = request.Name,
            Description = request.Description,
            DateTime = request.DateTime,
            Location = request.Location,
            Type = request.Type,
            Status = request.Status,
            OrganizingHouseId = request.OrganizingHouseId,
            ImageUrl = null // Se establecerá después de subir a Cloudinary
        };
    }

    public static void UpdateEventFromRequest(this Event eventEntity, UpdateEventRequest request)
    {
        if (!string.IsNullOrEmpty(request.Name))
            eventEntity.Name = request.Name;

        if (!string.IsNullOrEmpty(request.Description))
            eventEntity.Description = request.Description;

        if (request.DateTime.HasValue)
            eventEntity.DateTime = request.DateTime.Value;

        if (!string.IsNullOrEmpty(request.Location))
            eventEntity.Location = request.Location;

        if (!string.IsNullOrEmpty(request.Type))
            eventEntity.Type = request.Type;

        if (!string.IsNullOrEmpty(request.Status))
            eventEntity.Status = request.Status;

        if (request.OrganizingHouseId.HasValue)
            eventEntity.OrganizingHouseId = request.OrganizingHouseId.Value;
    }
}

public static class UserMappers
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CustomRole = user.CustomRole,
            CreatedAt = user.CreatedAt,
            PhotoUrl = user.PhotoUrl
        };
    }

    public static User ToUserEntity(this CreateUserRequest request)
    {
        return new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CustomRole = request.CustomRole,
            PhotoUrl = request.PhotoUrl
        };
    }
}

public static class AttendeeMappers
{
    public static AttendeeResponse ToAttendeeResponse(this Attendee attendee)
    {
        return new AttendeeResponse
        {
            AttendeeId = attendee.AttendeeId,
            UserId = attendee.UserId,
            DateOfBirth = attendee.DateOfBirth,
            Gender = attendee.Gender,
            User = attendee.User?.ToUserResponse() ?? new UserResponse()
        };
    }

    public static Attendee ToAttendeeEntity(this CreateAttendeeRequest request, string userId)
    {
        return new Attendee
        {
            UserId = userId,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender
        };
    }
}

public static class OrganizerMappers
{
    public static OrganizerResponse ToOrganizerResponse(this Organizer organizer)
    {
        return new OrganizerResponse
        {
            OrganizerId = organizer.OrganizerId,
            UserId = organizer.UserId,
            Company = organizer.Company,
            TaxId = organizer.TaxId,
            FiscalAddress = organizer.FiscalAddress,
            User = organizer.User?.ToUserResponse() ?? new UserResponse(),
            OrganizingHouses = organizer.OrganizingHouses?.Select(oh => oh.ToOrganizingHouseResponse()).ToList() ?? new List<OrganizingHouseResponse>()
        };
    }

    public static Organizer ToOrganizerEntity(this CreateOrganizerRequest request, string userId)
    {
        return new Organizer
        {
            UserId = userId,
            Company = request.Company,
            TaxId = request.TaxId,
            FiscalAddress = request.FiscalAddress
        };
    }
}

public static class OrganizingHouseMappers
{
    public static OrganizingHouseResponse ToOrganizingHouseResponse(this OrganizingHouse organizingHouse)
    {
        return new OrganizingHouseResponse
        {
            OrganizingHouseId = organizingHouse.OrganizingHouseId,
            Name = organizingHouse.Name,
            Address = organizingHouse.Address,
            Contact = organizingHouse.Contact,
            TaxData = organizingHouse.TaxData,
            OrganizerId = organizingHouse.OrganizerId,
            EventCount = organizingHouse.Events?.Count ?? 0
        };
    }

    public static OrganizingHouse ToOrganizingHouseEntity(this CreateOrganizingHouseRequest request, int organizerId)
    {
        return new OrganizingHouse
        {
            Name = request.Name,
            Address = request.Address,
            Contact = request.Contact,
            TaxData = request.TaxData,
            OrganizerId = organizerId
        };
    }
}

public static class TicketTypeMappers
{
    public static TicketTypeResponse ToTicketTypeResponse(this TicketType ticketType)
    {
        return new TicketTypeResponse
        {
            TicketTypeId = ticketType.TicketTypeId,
            Name = ticketType.Name,
            Description = ticketType.Description,
            Price = ticketType.Price,
            AvailableQuantity = ticketType.AvailableQuantity,
            SoldQuantity = ticketType.SoldQuantity,
            EventId = ticketType.EventId
        };
    }

    public static TicketType ToTicketTypeEntity(this CreateTicketTypeRequest request, int eventId)
    {
        return new TicketType
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            AvailableQuantity = request.AvailableQuantity,
            SoldQuantity = 0,
            EventId = eventId
        };
    }
}

public static class ProductMappers
{
    public static ProductResponse ToProductResponse(this Product product)
    {
        return new ProductResponse
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            EventId = product.EventId
        };
    }

    public static Product ToProductEntity(this CreateProductRequest request, int eventId)
    {
        return new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            EventId = eventId
        };
    }
}

public static class TicketMappers
{
    public static TicketResponse ToTicketResponse(this Ticket ticket)
    {
        return new TicketResponse
        {
            TicketId = ticket.TicketId,
            Type = ticket.Type,
            Price = ticket.Price,
            Status = ticket.Status,
            UniqueCode = ticket.UniqueCode,
            PurchaseDate = ticket.PurchaseDate,
            UserId = ticket.UserId,
            TicketTypeId = ticket.TicketTypeId,
            Entry = ticket.Entry?.ToEntryResponse()
        };
    }
}

public static class EntryMappers
{
    public static EntryResponse ToEntryResponse(this Entry entry)
    {
        return new EntryResponse
        {
            EntryId = entry.EntryId,
            TicketId = entry.TicketId,
            DateTime = entry.DateTime,
            AccessMethod = entry.AccessMethod
        };
    }

    public static Entry ToEntryEntity(this CreateEntryRequest request)
    {
        return new Entry
        {
            TicketId = request.TicketId,
            AccessMethod = request.AccessMethod,
            DateTime = DateTime.UtcNow
        };
    }
}

public static class SaleMappers
{
    public static SaleResponse ToSaleResponse(this Sale sale)
    {
        return new SaleResponse
        {
            SaleId = sale.SaleId,
            UserId = sale.UserId,
            ProductId = sale.ProductId,
            Quantity = sale.Quantity,
            Amount = sale.Amount,
            Date = sale.Date,
            Product = sale.Product?.ToProductResponse() ?? new ProductResponse()
        };
    }
}

public static class VirtualCurrencyMappers
{
    public static VirtualCurrencyResponse ToVirtualCurrencyResponse(this VirtualCurrency virtualCurrency)
    {
        return new VirtualCurrencyResponse
        {
            VirtualCurrencyId = virtualCurrency.VirtualCurrencyId,
            UserId = virtualCurrency.UserId,
            Balance = virtualCurrency.Balance,
            LastUpdated = virtualCurrency.LastUpdated
        };
    }
}