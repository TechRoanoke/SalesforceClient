using Salesforce.Force;
using Salesforce.Helpers.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Helpers
{
    // Extensions to the main force flient. 
    public static class ForceClientExtensions
    {
        // GEt the fields on table. 
        // Critical for tooling - to display possibilities, and for queries to getting the internal name.
        public static async Task<FieldDescription[]> GetFields(this ForceClient forceClient, string tableName)
        {
            var xx = await forceClient.DescribeAsync<SFFieldDesc>(tableName);

            List<FieldDescription> l = new List<FieldDescription>();
            foreach (var x in xx.fields)
            {
                var f = new FieldDescription
                {
                    DisplayName = x.label,
                    InternalName = x.name
                };

                if (x.picklistValues != null && x.picklistValues.Length > 0)
                {
                    f.PossibleValues = Array.ConvertAll(x.picklistValues, p => Tuple.Create(p.label, p.value));
                }

                {
                    SFFieldType t;
                    if (!Enum.TryParse<SFFieldType>(x.type, true, out t))
                    {
                    }
                    f.Kind = t;
                }

                var soapType = x.soapType;
                if (soapType.StartsWith("xsd:"))
                {
                    var right = soapType.Substring(4);
                    SFSoapType t;
                    if (!Enum.TryParse<SFSoapType>(right, true, out t))
                    {
                    }
                    f.CoreKind = t;
                }

                l.Add(f);
            };


            return l.ToArray();

        }
    }
}
