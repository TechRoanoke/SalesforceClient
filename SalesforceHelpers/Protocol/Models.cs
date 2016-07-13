using Newtonsoft.Json.Linq;
using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Describe wire protocols
namespace Salesforce.Helpers.Protocol
{
    // Get the fields on a table. 
    public class SFFieldDesc
    {
        public Field[] fields { get; set; }
        public class Field
        {
            public string label { get; set; } // display name
            public string name { get; set; } // API name
            public string type { get; set; } // - derived complex type 
            public string soapType { get; set; }

            public PicklistValue[] picklistValues { get; set; }


            public class PicklistValue
            {
                public string label { get; set; }
                public string value { get; set; }
            }

        }
    }

    // Semantics 
    public enum SFFieldType
    {
        None, // 0 = default 
        id,
        boolean,
        reference, // ??
        @string,
        picklist,
        multipicklist,
        textarea,
        @double,
        address,
        phone,
        email,
        date,
        datetime,
        url,
        currency
    }

    // Limited. All start with "xsd:" 
    public enum SFSoapType
    {
        None, // 0 = default
        @string,
        @boolean,
        @double,
        @date,
        @datetime,

    }
}