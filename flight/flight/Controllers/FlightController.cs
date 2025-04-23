using flight.Models;
using flight.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Sockets;
using StackExchange.Redis;
namespace flight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly flightService _flightService;

        public static int counter;


        public FlightController(flightService fs)
        {
            _flightService = fs;
        }



        /*
                [HttpPost]
                [Route("Buy")]
                public ActionResult<Passenger> BuyTicket(Passenger p)
                {
                    _flightService.IncrementViewCount("Buy");

                    try
                    {
                        var key= _flightService.AddNewPassengerAsync(p);
                        bool isvalid = key.Result;

                        if (isvalid)
                        {
                            return Content("done adding a passenger, your ticket was swas successfully bought ");
                        }
                        else
                        {
                            return Content("error. either the ticket is sold or does not exist");
                        }

                    }
                    catch (Exception ex)
                    {
                        return Content(ex.Message);
                    }

                }

                [HttpPost]
                [Route("addTicket")]
                public ActionResult AddTicket(Ticket ticket)
                {
                    _flightService.IncrementViewCount("addTicket");

                    try
                    {
                        var key = _flightService.AddNewTicketAsync(ticket);
                        bool isvalid = key.Result;
                        if (isvalid)
                        {
                            return Content("done adding a ticket.");
                        }
                        else
                        {
                            return Content("use a uniqe ticket id");
                        }

                    }
                    catch (Exception ex)
                    {
                        return Content("ERROR ACCURED X-X");
                    }
                }


                [HttpGet]
                [Route("AvailableFlights")]

                public List<Dictionary<string,string>> showavalible()
                {
                    _flightService.IncrementViewCount("avalibleflights");
                    return _flightService.ShowAvalibleTickets();
                }

                [HttpGet]
                [Route("passengerlist")]
                public List<Dictionary<string,string>> GetPassList(int id)  
                {
                    _flightService.IncrementViewCount("passengerlist");
                    return _flightService.GetPassengerNamesForAFlight(id);

                }


                [HttpGet]
                [Route("getview")]
                public int GetViews(string url)
                {
                    var count = 0;
                    count=  _flightService.GetViewCount(url);
                    return count;
                }

                [HttpGet]
                [Route("SoldTickets")]

                public List<Dictionary<string, string>> showsoldtickets()
                {
                    return _flightService.ReadFromDb();
                }

                [HttpGet]
                [Route("flightsForDest")]

                public List<Dictionary<string, string>> flightForDest(string dest)
                {
                    return _flightService.AvalibleFlightsForDest(dest);
                }

                [HttpGet]
                [Route("avalibleflightsDB")]

                public List<Dictionary<string, string>> avalflightDb()
                {
                    return _flightService.DbAvalibleflights();
                }*/


        [HttpPost]
        [Route("addflight")]
        public ActionResult AddNewFlight(Flight f)
        {
            var m = _flightService.AddFlight(f);
            var valid = m.Result;
            if (!valid)
            {
                return Content("ERROR. EITHER INVALID INPUT or something else ( check console ) ");
            }
            else
            {
                return Content("DONE. NEW FLIGHT ADDED");
            }

        }


        [HttpPost]
        [Route("addticket")]
        public ActionResult AddNewTicket(Ticket t)
        {
            var m = _flightService.AddTicket(t);
            var valid = m.Result;
            if (!valid)
            {
                return Content("ERROR. EITHER INVALID INPUT or something else ( check console ) ");
            }
            else
            {
                return Content("DONE. NEW FLIGHT ADDED");
            }

        }


        [HttpPost]
        [Route("buyticket")]
        public ActionResult AddNewPass(Passenger p)
        {
            var m = _flightService.AddPassBuyTicket(p);
            var valid = m.Result;
            if (!valid)
            {
                return Content("ERROR. EITHER INVALID INPUT or something else ( check console ) ");
            }
            else
            {
                return Content("DONE. NEW PASSENGER ADDED");
            }

        }

        [HttpGet]
        [Route("ShowFlightsPassenger")]

        public List<Dictionary<string, string>> FlightsPass(string flight_id)
        {
            return _flightService.GetPassList(flight_id);
        }


        [HttpGet]
        [Route("ShowAvalibleTickets")]

        public List<Dictionary<string, string>> Capacityleft()
        {
            return _flightService.CapLeft();
        }
    }
}
