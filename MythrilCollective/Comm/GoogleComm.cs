using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MythrilCollective.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading.Tasks;
using System.IO;

namespace MythrilCollective.Comm
{
    public class GoogleComm
    {
        private static string CalendarID = System.Web.Configuration.WebConfigurationManager.AppSettings["GroupCalendarID"];
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Mythril Events Client";

        public CalendarService CalendarServiceAuth()
        {
            UserCredential credential;
            string file = HttpContext.Current.Server.MapPath("/Comm/client_secret.json");
            using (var stream =
                new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-mythrilevents.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    System.Threading.CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });


            var clientid = "991389200156-d01096jh7suvfp827euicbp7ee31sjoi.apps.googleusercontent.com";
            var clientsecret = "HXFfvbc9IqbDN47BT_wBWYEu";
            return service;
        }

        public List<Event> GetUpcoming()
        {
            const int MaxEventsPerCalendar = 20;
            const int MaxEventsOverall = 50;

            var service = CalendarServiceAuth();
                var request = service.Events.List(CalendarID);
                request.MaxResults = MaxEventsPerCalendar;
                request.SingleEvents = true;
                request.TimeMin = DateTime.Now;
                var fetch = request.Execute();

            List<Event> Events = new List<Event>();
            foreach(var result in fetch.Items)
            {
                Event re = new Event();
                re.Title = result.Summary;
                re.Description = result.Description != null ? result.Description : "";
                if(result.Start.DateTime == null)
                {
                    //NO SPECIFIC START DATE, THEREFORE ALL DAY EVENT.
                    DateTime dt;
                    if(DateTime.TryParse(result.Start.Date, out dt))
                    {
                        re.Start = dt;
                        re.Duration = 0;
                    }
                }
                else
                {
                    re.EventStart = result.Start.DateTime;
                    re.EventEnd = result.End.DateTime;
                    re.Duration = re.EventEnd.Value.Subtract(re.EventStart.Value).Hours;
                }
                re.Location = result.Location;
                re.ColorCode = result.ColorId;
                
                Events.Add(re);
            }

            return Events;
        }

        public void GetColors()
        {
            


        }

        public bool InsertEvent(ref Event ev)
        {
            bool success = false;
            var service = CalendarServiceAuth();
            
            Google.Apis.Calendar.v3.Data.Event oEnv = new Google.Apis.Calendar.v3.Data.Event();
            oEnv.Kind = "calendar#event";


            oEnv.Summary = ev.Title;
            oEnv.Description = ev.Description;
            oEnv.Start = new Google.Apis.Calendar.v3.Data.EventDateTime()
            {
              DateTime = ev.Start,
              TimeZone = "America/New_York"
            };

            if(ev.Duration > 0)
            {
                oEnv.End = new Google.Apis.Calendar.v3.Data.EventDateTime()
                {
                  DateTime = ev.End,
                  TimeZone = "America/New_York"
                };
            }

            oEnv.Location = ev.Location;
            oEnv.ColorId = ev.ColorCode;

            try
            {
                EventsResource.InsertRequest request = service.Events.Insert(oEnv, CalendarID);
                var CalEvent = request.Execute();
                ev.Message = string.Format("Event Created: {0}", CalEvent.HtmlLink);
                
                success = true;
            }
            catch (Exception ex)
            {
                ev.Message = ex.Message;
            }

            return success;
        }
    }

    
}