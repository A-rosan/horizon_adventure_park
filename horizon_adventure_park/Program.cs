using HorizonAdventurePark.Data;
using HorizonAdventurePark.Enums;
using HorizonAdventurePark.Exceptions;
using HorizonAdventurePark.Models;
using HorizonAdventurePark.Services;

namespace HorizonAdventurePark;

class Program
{
    static void Main()
    {
        ParkData data = new(); 
        
        VisitorService visitorService =
            new(data);

        TicketService ticketService =
            new(data);

        RideService rideService =
            new(data);

        ReservationService reservationService =
            new(
                data,
                ticketService,
                rideService);

        EmployeeService employeeService =
            new(data);

        SeedData(
            visitorService,
            rideService,
            employeeService);

        while (true)
        {
            ShowMenu();

            int choice = ReadInt("Select an option: ");

            Console.Clear(); // Clear the console for better readability

            // Handle the user's choice with exception handling
            try
            {
                switch (choice)
                {
                    case 1:
                        RegisterVisitor(visitorService); 
                        break;

                    case 2:
                        IssueTicket(
                            visitorService,
                            ticketService);
                        break;

                    case 3:
                        ValidateRideAccess(
                            visitorService,
                            rideService,
                            ticketService);
                        break;

                    case 4:
                        CreateReservation(
                            reservationService);
                        break;

                    case 5:
                        ManageRideStatus(
                            rideService);
                        break;

                    case 6:
                        AssignStaff(
                            employeeService);
                        break;

                    case 7:
                        CancelReservation(
                            reservationService);
                        break;

                    case 8:
                        CancelTicket(
                            ticketService);
                        break;

                    case 9:
                        ShowRideStatus(data);
                        break;

                    case 10:
                        Console.WriteLine(
                            "Thank you for using Horizon Adventure Park!");

                        return;

                    default:
                        Console.WriteLine(
                            "Invalid menu option.");
                        break;
                }
            }
            catch (BusinessRuleException ex)
            {
                Console.WriteLine(
                    $"Operation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Unexpected error: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine(
                "Press ENTER to continue...");

            Console.ReadLine();
            Console.Clear();
        }
    }

    static void ShowMenu() //main menu for the console application
    {
        Console.WriteLine(
            "=== Horizon Adventure Park — Operations System ===");

        Console.WriteLine();

        Console.WriteLine("1. Register Visitor");
        Console.WriteLine("2. Issue Ticket");
        Console.WriteLine("3. Validate Ride Access");
        Console.WriteLine("4. Create Reservation");
        Console.WriteLine("5. Manage Ride Status");
        Console.WriteLine("6. Assign Staff");
        Console.WriteLine("7. Cancel Reservation");
        Console.WriteLine("8. Deactivate Ticket");
        Console.WriteLine("9. View Ride Occupancy");
        Console.WriteLine("10. Exit");

        Console.WriteLine();
    }

    //register a new visitor
    static void RegisterVisitor(
        VisitorService service)
    {
        Console.WriteLine("=== Register Visitor ===");

        string id = ReadString("Visitor ID: ");
        string name = ReadString("Full name: ");
        int age = ReadInt("Age: ");
        double height = ReadDouble("Height (cm): ");

        VisitorCategory category =
            ReadVisitorCategory();

        Visitor visitor =
            service.RegisterVisitor(
                id,
                name,
                age,
                height,
                category);

        Console.WriteLine();
        Console.WriteLine(
            $"Visitor registered successfully: {visitor.Id}");
    }

    //issue a ticket for a visitor
    static void IssueTicket(
        VisitorService visitorService,
        TicketService ticketService)
    {
        Console.WriteLine("=== Issue Ticket ===");

        string visitorId =
            ReadString("Visitor ID: ");

        var visitor =
            visitorService.FindVisitor(visitorId);

        if (visitor == null)
        {
            Console.WriteLine(
                "Visitor does not exist.");

            return;
        }

        TicketType ticketType =
            ConvertCategoryToTicketType(
                visitor.Category);

        decimal price =
            GetTicketPrice(ticketType);

        DateTime validFrom = DateTime.Now;

        DateTime validUntil =
            validFrom.AddDays(1);

        var ticket =
            ticketService.IssueTicket(
                visitorId,
                ticketType,
                price,
                validFrom,
                validUntil);

        Console.WriteLine();
        Console.WriteLine(
            $"Ticket issued successfully.");
        Console.WriteLine(
            $"Ticket ID: {ticket.Id}");
        Console.WriteLine(
            $"Type: {ticket.Type}");
        Console.WriteLine(
            $"Price: {ticket.Price:C}");
        Console.WriteLine(
            $"Valid until: {ticket.ValidUntil}");
    }

    //validate if a visitor can access a ride
    static void ValidateRideAccess(
        VisitorService visitorService,
        RideService rideService,
        TicketService ticketService)
    {
        Console.WriteLine("=== Validate Ride Access ===");

        string visitorId =
            ReadString("Visitor ID: ");

        string rideId =
            ReadString("Ride ID: ");

        var visitor =
            visitorService.FindVisitor(visitorId);

        if (visitor == null)
        {
            Console.WriteLine(
                "Visitor does not exist.");

            return;
        }

        var ride =
            rideService.FindRide(rideId);

        if (ride == null)
        {
            Console.WriteLine(
                "Ride does not exist.");

            return;
        }

        if (!ticketService.ValidateTicket(
                visitorId,
                out string ticketReason))
        {
            Console.WriteLine(
                "ACCESS DENIED");

            Console.WriteLine(
                $"Reason: {ticketReason}");

            return;
        }

        var ticket =
            ticketService.GetVisitorTicket(visitorId);

        if (ticket == null)
        {
            Console.WriteLine(
                "ACCESS DENIED");

            Console.WriteLine(
                "Reason: No valid ticket.");

            return;
        }

        if (ride.Status != RideStatus.Open)
        {
            Console.WriteLine(
                "ACCESS DENIED");

            Console.WriteLine(
                $"Reason: Ride is currently {ride.Status}.");

            return;
        }

        if (!rideService.CheckEligibility(
                visitor,
                ride,
                out string eligibilityReason))
        {
            Console.WriteLine(
                "ACCESS DENIED");

            Console.WriteLine(
                $"Reason: {eligibilityReason}");

            return;
        }

        if (!rideService.HasAvailableCapacity(ride))
        {
            Console.WriteLine(
                "ACCESS DENIED");

            Console.WriteLine(
                "Reason: Ride has reached maximum capacity.");

            return;
        }

        Console.WriteLine(
            "ACCESS GRANTED");

        rideService.AdmitVisitor(
            visitorId,
            rideId,
            ticketService);

        Console.WriteLine(
            $"Visitor admitted to {ride.Name}.");
    }

    static void CreateReservation(
        ReservationService service)
    {
        Console.WriteLine(
            "=== Create Reservation ===");

        string visitorId =
            ReadString("Visitor ID: ");

        string rideId =
            ReadString("Ride ID: ");

        string timeSlot =
            ReadString("Time Slot: ");

        var reservation =
            service.CreateReservation(
                visitorId,
                rideId,
                timeSlot);

        Console.WriteLine();
        Console.WriteLine(
            "RESERVATION CREATED");

        Console.WriteLine(
            $"Reservation ID: {reservation.Id}");
    }

    static void ManageRideStatus(
        RideService service)
    {
        Console.WriteLine(
            "=== Manage Ride Status ===");

        string rideId =
            ReadString("Ride ID: ");

        RideStatus status =
            ReadRideStatus();

        service.UpdateRideStatus(
            rideId,
            status);

        Console.WriteLine(
            "Ride status updated successfully.");
    }

    static void AssignStaff(
        EmployeeService service)
    {
        Console.WriteLine(
            "=== Assign Staff ===");

        string employeeId =
            ReadString("Employee ID: ");

        string locationId =
            ReadString("Ride/Facility ID: ");

        string timeSlot =
            ReadString("Time Slot: ");

        service.AssignEmployee(
            employeeId,
            locationId,
            timeSlot);

        Console.WriteLine(
            "Employee assigned successfully.");
    }

    static void CancelReservation(
        ReservationService service)
    {
        Console.WriteLine(
            "=== Cancel Reservation ===");

        string reservationId =
            ReadString("Reservation ID: ");

        service.CancelReservation(
            reservationId);

        Console.WriteLine(
            "Reservation cancelled successfully.");
    }

    static void CancelTicket(
        TicketService service)
    {
        Console.WriteLine(
            "=== Deactivate Ticket ===");

        string visitorId =
            ReadString("Visitor ID: ");

        service.CancelTicket(
            visitorId);

        Console.WriteLine(
            "Ticket deactivated successfully.");
    }

    static void ShowRideStatus(
        ParkData data)
    {
        Console.WriteLine(
            "=== Ride Occupancy ===");

        if (!data.Rides.Any())
        {
            Console.WriteLine(
                "No rides available.");

            return;
        }

        foreach (var ride in data.Rides)
        {
            Console.WriteLine(
                $"{ride.Name} | " +
                $"Status: {ride.Status} | " +
                $"Occupancy: {ride.CurrentOccupancy}/{ride.MaximumCapacity}");
        }
    }

    static VisitorCategory ReadVisitorCategory()
    {
        Console.WriteLine();
        Console.WriteLine("Visitor Category:");

        Console.WriteLine("1. General");
        Console.WriteLine("2. VIP");
        Console.WriteLine("3. Child");
        Console.WriteLine("4. Senior");
        Console.WriteLine("5. Staff Accompanied Minor");

        int choice =
            ReadInt("Choose category: ");

        return choice switch
        {
            1 => VisitorCategory.General,
            2 => VisitorCategory.VIP,
            3 => VisitorCategory.Child,
            4 => VisitorCategory.Senior,
            5 => VisitorCategory.StaffAccompaniedMinor,

            _ => throw new BusinessRuleException(
                "Invalid visitor category.")
        };
    }

    static RideStatus ReadRideStatus()
    {
        Console.WriteLine();
        Console.WriteLine("Ride Status:");

        Console.WriteLine("1. Open");
        Console.WriteLine("2. Closed");
        Console.WriteLine("3. Under Maintenance");

        int choice =
            ReadInt("Choose status: ");

        return choice switch
        {
            1 => RideStatus.Open,
            2 => RideStatus.Closed,
            3 => RideStatus.UnderMaintenance,

            _ => throw new BusinessRuleException(
                "Invalid ride status.")
        };
    }

    static TicketType ConvertCategoryToTicketType(
        VisitorCategory category)
    {
        return category switch
        {
            VisitorCategory.General =>
                TicketType.Regular,

            VisitorCategory.VIP =>
                TicketType.VIP,

            VisitorCategory.Child =>
                TicketType.Child,

            VisitorCategory.Senior =>
                TicketType.Senior,

            VisitorCategory.StaffAccompaniedMinor =>
                TicketType.StaffAccompaniedMinor,

            _ => throw new BusinessRuleException(
                "Invalid visitor category.")
        };
    }

    static decimal GetTicketPrice(
        TicketType type)
    {
        return type switch
        {
            TicketType.Regular => 25m,
            TicketType.VIP => 50m,
            TicketType.Child => 15m,
            TicketType.Senior => 18m,
            TicketType.StaffAccompaniedMinor => 10m,

            _ => 0m
        };
    }

    static void SeedData(
        VisitorService visitorService,
        RideService rideService,
        EmployeeService employeeService)
    {
        rideService.AddRide(
            "R-001",
            "Thunder Peak Coaster",
            RideType.Thrill,
            12,
            110,
            false,
            30);

        rideService.AddRide(
            "R-002",
            "Splash Voyage",
            RideType.Water,
            8,
            100,
            true,
            20);

        rideService.AddRide(
            "R-003",
            "Family Carousel",
            RideType.Family,
            0,
            80,
            true,
            25);

        employeeService.AddEmployee(
            "E-001",
            "Abdullah Helmi",
            EmployeeRole.RideOperator);

        employeeService.AddEmployee(
            "E-002",
            "Mohammad Saeed",
            EmployeeRole.TicketBoothStaff);

        employeeService.AddEmployee(
            "E-003",
            "Ibrahim Adawi",
            EmployeeRole.OperationsManager);
    }

    static string ReadString(string message)
    {
        Console.Write(message);

        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            throw new BusinessRuleException(
                "Input cannot be empty.");
        }

        return input.Trim();
    }

    static int ReadInt(string message)
    {
        Console.Write(message);

        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int result))
        {
            throw new BusinessRuleException(
                "Please enter a valid whole number.");
        }

        return result;
    }

    static double ReadDouble(string message)
    {
        Console.Write(message);

        string? input = Console.ReadLine();

        if (!double.TryParse(input, out double result))
        {
            throw new BusinessRuleException(
                "Please enter a valid number.");
        }

        return result;
    }
}