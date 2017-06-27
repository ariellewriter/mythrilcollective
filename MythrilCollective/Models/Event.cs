using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MythrilCollective.Models
{
    public class Event
    {
        private double _duration = 1;
        private string _color = null;
        private string _type = null;
        private bool _success = false;
        private DateTime? _estart = null;
        private DateTime? _eend = null;
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime Start { get; set; }
        public DateTime? EventStart { 
            get { return this._estart; } 
            set { _estart = value; 
                if (_estart != null) { this.Start = _estart.Value; } 
            } 
        }
        public double Duration { get { return this._duration; } set { this._duration = value; } }
        public DateTime? EventEnd { get; set; }
        public DateTime End
        {
            get
            {
                if (this.Duration > 0)
                {
                    return this.Start.AddHours(this.Duration);
                }
                return new DateTime(Start.Year, Start.Month, Start.Day).AddDays(1);
            }
        }
        public string ColorCode {
            get {
                if (string.IsNullOrEmpty(this._color))
                    this._color = CalendarEventType.GetColorCode(this._type);

                return this._color;
            }
            set { this._color = value;
                this._type = CalendarEventType.GetEventType(value);
            }
        }
        public string EventType
        {
            get {
                if (string.IsNullOrEmpty(this._type))
                    this._type = CalendarEventType.GetEventType(this._color);

                return this._type;
            }
            set { this._type = value; }
        }
        public string OfficerCode { get; set; }
        public string Description { get; set; }
        public bool Success { get { return this._success; } }
        public string Message { get; set; }
    }

    public class CalendarEventType
    {
        static Dictionary<string, string> _dict = new Dictionary<string, string>() { 
            {"none","None"},
            {"bold blue","Server/Public Events"},
            {"turquoise","Non-FC Events"},
            {"bold green","FC RP Event"},
            {"bold red","FC OOC Events"},
            {"red","FC PvE Events"},
            {"purple","Member Events"},
            {"gray","Other Events"}
        };

        public static string GetEventType(string code)
        {
            // Try to get the result in the static Dictionary
            string result;
            if (code != null)
            {
                if (_dict.TryGetValue(code, out result))
                {
                    return result;
                }
                else
                {
                    return "None";
                }
            }
            return null;
        }
        public static string GetColorCode(string eventname)
        {
            // Try to get the result in the static Dictionary
            string result;
            var oResult = _dict.FirstOrDefault(x => x.Value == eventname);
            return oResult.Key;
        }
    }
}