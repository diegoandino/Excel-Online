using Newtonsoft.Json;
using System;

namespace SpreadsheetModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Select
    {
        [JsonProperty()]
        public string requestType { get; private set; } = "\"selectCell\"";

        [JsonProperty()]
        public string cellName { get; private set; }


        public Select(string name)
        {
            cellName = name;
        }

    }
    [JsonObject(MemberSerialization.OptIn)]
    public class Revert
    {
        [JsonProperty()]
        public string requestType { get; private set; } = "\"revertCell\"";

        [JsonProperty()]
        public string cellName { get; private set; }

        public Revert(string name)
        { 
            cellName = name;
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class Undo
    {
        [JsonProperty()]
        public string requestType { get; private set; } = "\"undo\"";

        public Undo()
        {
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class Edit
    {
        [JsonProperty()]
        public string requestType { get; private set; } = "\"editCell\"";

        [JsonProperty()]
        public string cellName { get; private set; }

        [JsonProperty()]
        public string contents { get; private set; }

        public Edit(string name, string content)
        {
            cellName = name;
            contents = content;
        }
    }
}
