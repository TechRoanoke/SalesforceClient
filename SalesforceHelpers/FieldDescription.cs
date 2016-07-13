using Salesforce.Helpers.Protocol;
using System;

namespace Salesforce.Helpers
{
    // Friendly wrapper for describing a salesforce field. This can be used in SF queries. 
    public class FieldDescription
    {
        public string DisplayName { get; set; } // display name.
        public string InternalName { get; set; } // API name. This is in the query. 
        public SFFieldType Kind { get; set; } // phone, address, etc. Has semantics. 
        public SFSoapType CoreKind { get; set; } // basic 

        // Maybe null.
        public Tuple<string, string>[] PossibleValues { get; set; }

        public override string ToString()
        {
            return this.DisplayName;
        } 
    }
}
