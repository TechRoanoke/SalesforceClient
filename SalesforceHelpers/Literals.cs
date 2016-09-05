using System;
namespace Salesforce.Helpers
{
    public static class Literals
    {
        public static string ToSoql(DateTime d)
        {
            return d.ToString("yyyy-MM-ddThh:mm:ssZ");
        }
        public static string ToSoql(string x)
        {
            return "'" + x + "'";
        }
        public static string ToSoql(bool b)
        {
            return b.ToString().ToUpper();
        }
    }
}
