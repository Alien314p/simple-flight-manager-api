namespace flight.Models
{
    public class Passenger
    {
        public string Fullname { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string TicketId { get; set; }
        public string personId { get; set; }
        public string FlightId { get; set; }

    }


    public class Ticket
    {
        public string TicketId { get; set; }

        public string FlightId { get; set; }

        public bool Status { get; set; }

        public int Price { get; set; }

        public string Type { get; set; } = string.Empty;


    }
    public class Flight
    {
        public string Destination { get; set; }
        public string Origin { get; set; }


        public string Capacity { get; set; }
        public string FlightId { get; set; }

        public string FlightDate { get; set; }

        public string FlightTime { get; set; }

        public string Company { get; set; }

    }
}
