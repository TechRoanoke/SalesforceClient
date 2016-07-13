using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesforce.Helpers; // Helpers on top of the nuget package
using Salesforce.Force;

namespace Samples
{
    // Demonstrate using the Salesforce API. 
    class Program
    {
        static void Main(string[] args)
        {
            Work().Wait();
        }
        static async Task Work()
        {
            // Get your secret connection string
            string connectionString = File.ReadAllText(@"c:\temp\salesforce-secret.txt");
            var sfx = ConnectionString.FromConnectionString(connectionString);
            ForceClient forceClient = await sfx.ToClientAsync();

            FieldDescription[] fields = await forceClient.GetFields("Contact");

            PrintAllFields(fields, Console.Out);
            
            // Do a type-safe query 
            var q = new SFQ(fields, "Contact");
            var s = q.Select("id", "FirstName", "LastName").Where(
                        q.And(
                            q.Equals("Canvassable", true),
                            q.Equals("Canvass Group", "GroupWA")));
            var s2 = s.ToString();
            var accounts2 = await forceClient.QueryAsync<dynamic>(s2);
        }

        // Print all the fields. 
        private static void PrintAllFields(FieldDescription[] fields, TextWriter textWriter)
        {
            textWriter.WriteLine("DisplayName, Internal Name, Kind, soapKind, PossibleValues");
            {
                foreach (FieldDescription field in fields)
                {
                    string p = null;
                    if (field.PossibleValues != null)
                    {
                        p = string.Join(";", Array.ConvertAll(field.PossibleValues, x => x.ToString()));
                    }
                    textWriter.WriteLine("{0},{1},{2},{3},{4}", field.DisplayName, field.InternalName, field.Kind, field.CoreKind, p);
                }
            }
        }
    }
}
