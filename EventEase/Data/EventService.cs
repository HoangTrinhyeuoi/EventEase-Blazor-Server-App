using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventEase.Data;

public class EventService
{
    private readonly List<Event> _events;
    private readonly List<Registration> _registrations;
    private readonly List<AttendanceRecord> _attendanceRecords;

    public EventService()
    {
        _registrations = new List<Registration>();
        _attendanceRecords = new List<AttendanceRecord>();
        
        _events = new List<Event>
        {
            new Event
            {
                Id = 1,
                Name = "Tech Innovation Summit 2024",
                Date = DateTime.Now.AddDays(15),
                Location = "Seattle Convention Center, WA",
                Description = "Join industry leaders for a day of technological breakthroughs and networking opportunities. Experience cutting-edge demos, participate in hands-on workshops, and network with fellow innovators.",
                Category = "Technology",
                MaxAttendees = 500,
                CurrentAttendees = 342,
                Price = 299.99m,
                ImageUrl = "/images/tech-summit.jpg"
            },
            new Event
            {
                Id = 2,
                Name = "Kaopiz Software New Year Party 2026",
                Date = DateTime.Now.AddDays(22),
                Location = "Downtown Business Center, NY",
                Description = "Develop your leadership skills with expert facilitators and hands-on exercises. Learn modern management techniques and build your professional network.",
                Category = "Business",
                MaxAttendees = 100,
                CurrentAttendees = 87,
                Price = 199.99m,
                ImageUrl = "/images/leadership.jpg"
            },
            new Event
            {
                Id = 3,
                Name = "New office expansion ceremony",
                Date = DateTime.Now.AddDays(45),
                Location = "Grand Ballroom, Chicago, IL",
                Description = "An elegant evening supporting local charities with dinner, entertainment, and auction. Join us in making a difference in our community.",
                Category = "Charity",
                MaxAttendees = 300,
                CurrentAttendees = 156,
                Price = 150.00m,
                ImageUrl = "/images/charity-gala.jpg"
            },
            new Event
            {
                Id = 4,
                Name = "Digital Marketing Masterclass",
                Date = DateTime.Now.AddDays(8),
                Location = "Virtual Event",
                Description = "Learn cutting-edge digital marketing strategies from industry experts. Master SEO, social media marketing, and data analytics.",
                Category = "Education",
                MaxAttendees = 1000,
                CurrentAttendees = 623,
                Price = 79.99m,
                ImageUrl = "/images/digital-marketing.jpg"
            }
        };

        Console.WriteLine($"EventService initialized with {_events.Count} events");
    }

    public Task<List<Event>> GetEventsAsync()
    {
        Console.WriteLine($"GetEventsAsync called, returning {_events.Count} events");
        return Task.FromResult(_events);
    }

    public Task<Event?> GetEventByIdAsync(int id)
    {
        var eventItem = _events.FirstOrDefault(e => e.Id == id);
        Console.WriteLine($"GetEventByIdAsync called with ID {id}, found: {eventItem?.Name ?? "null"}");
        return Task.FromResult(eventItem);
    }

    public Task<(bool Success, string Message, int? RegistrationId)> RegisterForEventAsync(
        int eventId, RegistrationFormModel model)
    {
        var eventItem = _events.FirstOrDefault(e => e.Id == eventId);
        if (eventItem == null)
        {
            return Task.FromResult<(bool, string, int?)>((false, "Event not found", null));
        }

        if (eventItem.CurrentAttendees >= eventItem.MaxAttendees)
        {
            return Task.FromResult<(bool, string, int?)>((false, "Event is fully booked", null));
        }

        // Check for duplicate registration
        var existingRegistration = _registrations.FirstOrDefault(r => 
            r.EventId == eventId && r.Email.ToLower() == model.Email.ToLower());
        
        if (existingRegistration != null)
        {
            return Task.FromResult<(bool, string, int?)>((false, "You are already registered for this event", existingRegistration.Id));
        }

        var registration = new Registration
        {
            Id = _registrations.Count + 1,
            EventId = eventId,
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            SpecialRequirements = model.SpecialRequirements,
            RegistrationDate = DateTime.Now,
            Status = RegistrationStatus.Confirmed
        };

        _registrations.Add(registration);
        eventItem.CurrentAttendees++;

        return Task.FromResult<(bool, string, int?)>((true, "Registration successful!", registration.Id));
    }

    public Task<List<Registration>> GetRegistrationsForEventAsync(int eventId)
    {
        var registrations = _registrations.Where(r => r.EventId == eventId).ToList();
        return Task.FromResult(registrations);
    }

    public Task<Registration?> GetRegistrationByIdAsync(int registrationId)
    {
        var registration = _registrations.FirstOrDefault(r => r.Id == registrationId);
        return Task.FromResult(registration);
    }

    public Task<List<Registration>> GetRegistrationsForUserAsync(string email)
    {
        var registrations = _registrations.Where(r => 
            r.Email.ToLower() == email.ToLower()).ToList();
        return Task.FromResult(registrations);
    }

    public Task<bool> CheckInAttendeeAsync(int registrationId, string method = "Manual")
    {
        var registration = _registrations.FirstOrDefault(r => r.Id == registrationId);
        if (registration == null) return Task.FromResult(false);

        var existingCheckIn = _attendanceRecords.FirstOrDefault(a => 
            a.RegistrationId == registrationId);
        
        if (existingCheckIn != null) return Task.FromResult(false); // Already checked in

        var attendanceRecord = new AttendanceRecord
        {
            Id = _attendanceRecords.Count + 1,
            EventId = registration.EventId,
            RegistrationId = registrationId,
            CheckInTime = DateTime.Now,
            CheckInMethod = method
        };

        _attendanceRecords.Add(attendanceRecord);
        registration.IsAttended = true;
        registration.Status = RegistrationStatus.Attended;

        return Task.FromResult(true);
    }

    public Task<List<AttendanceRecord>> GetAttendanceForEventAsync(int eventId)
    {
        var attendance = _attendanceRecords.Where(a => a.EventId == eventId).ToList();
        return Task.FromResult(attendance);
    }

    public Task<int> GetAttendanceCountAsync(int eventId)
    {
        var count = _attendanceRecords.Count(a => a.EventId == eventId);
        return Task.FromResult(count);
    }
}