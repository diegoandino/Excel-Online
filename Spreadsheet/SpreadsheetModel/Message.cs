using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateCell
    {
        [JsonProperty()]
        public string messageType { get; private set; } = "\"cellUpdated\"";

        [JsonProperty()]
        public string cellName { get; private set; }

        [JsonProperty()]
        public string contents { get; private set; }

        public UpdateCell( string name, string content )
        {
            cellName = name;
            contents = content;
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class SelectCell
    {
        [JsonProperty()]
        public string messageType { get; private set; } = "\"cellSelected\"";

        [JsonProperty()]
        public string cellName { get; private set; }

        [JsonProperty(PropertyName = "selector")]
        public int selectorID { get; private set; }

        [JsonProperty()]
        public string selectorName { get; private set; }
        public SelectCell(string name, int selector, string nameSelector ) 
        {
            cellName = name;
            selectorID = selector;
            selectorName = nameSelector;
        }
        
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class Disconnect
    {
        [JsonProperty()]
        public string messageType { get; private set; } = "\"disconnect\"";

        [JsonProperty(PropertyName = "user")]
        public int userID { get; private set; }

        public Disconnect(int id)
        {
            userID = id;
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class InvalidRequest
    {
        [JsonProperty()]
        public string messageType { get; private set; } = "\"requestError\"";

        [JsonProperty()]
        public string cellName { get; private set; }

        [JsonProperty()]
        public string message { get; private set; }

        public InvalidRequest( string name, string error)
        {
            cellName = name;
            message = error;
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class Shutdown
    {
        [JsonProperty()]
        public string messageType { get; private set; } = "\"serverError\"";

        [JsonProperty()]
        public string message { get; private set; }

        public Shutdown(string error)
        {
            message = error;
        }
    }

}
