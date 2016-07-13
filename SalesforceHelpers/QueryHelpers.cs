using Newtonsoft.Json.Linq;
using Salesforce.Common;
using Salesforce.Force;
using Salesforce.Helpers.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Helpers
{
    // Helpers for building up SF expresisons in a type-safe way and validating them on the client
    // with useful errors (Rather than obscure 400s). 
    public class SFQ
    {
        private readonly FieldDescription[] _fields;
        private readonly string _tableName;
        public SFQ(FieldDescription[] fields, string tableName)
        {
            _fields = fields;
            _tableName = tableName;
        }

        public SelectExpr Select(params string[] fieldNames)
        {
            var x2 = Array.ConvertAll(fieldNames, f => LookupField(f));

            return new SelectExpr
            {
                _fields = x2,
                _tableName = this._tableName
            };
        }

        private FieldDescription LookupField(string columnName)
        {
            // First try exact match
            foreach (var f in _fields)
            {
                if (string.Equals(f.InternalName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return f;
                }
            }

            // Then match on display name
            foreach (var f in _fields)
            {
                if (string.Equals(f.DisplayName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return f;
                }
            }

            throw new InvalidOperationException("Can't find column name: " + columnName);
        }

        public QueryExpr And(QueryExpr lhs, QueryExpr rhs)
        {
            return new QueryExprAnd { LHS = lhs, RHS = rhs };
        }

        public QueryExpr Equals(string columnName, bool value)
        {
            var f = LookupField(columnName);

            if (f.CoreKind != SFSoapType.boolean)
            {
                string msg = string.Format("Don't compare {0} to boolean since it's of type {1}", columnName, f.CoreKind);
                throw new InvalidOperationException(msg);
            }

            return new QueryExprEquals { LHS = f, RHS = value.ToString().ToUpper() };
        }
        public QueryExpr Equals(string columnName, string value)
        {
            var f = LookupField(columnName);

            if (f.PossibleValues != null)
            {
                string match = null;
                foreach (var t in f.PossibleValues)
                {
                    if (string.Equals(t.Item2, value, StringComparison.OrdinalIgnoreCase))
                    {
                        match = t.Item2;
                        break;
                    }
                    if (string.Equals(t.Item1, value, StringComparison.OrdinalIgnoreCase))
                    {
                        match = t.Item2;
                        break;
                    }
                }
                if (match == null)
                {
                    string msg = string.Format("Can't compare {0} to {1} since that's not a valid possible value", columnName, value);
                    throw new InvalidOperationException(msg);
                }
                value = match;
            }
            return new QueryExprEquals { LHS = f, RHS = "'" + value + "'" };
        }
    }

    public class SelectExpr
    {
        public FieldDescription[] _fields;
        public string _tableName;
        public QueryExpr _where;

        public SelectExpr Where(QueryExpr q)
        {
            _where = q;
            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            bool first = true;
            foreach (var f in _fields)
            {
                if (!first)
                {
                    sb.Append(",");
                }
                first = false;

                sb.Append(f.InternalName);
            }
            sb.AppendFormat(" FROM {0} ", _tableName);

            sb.Append("WHERE ");
            sb.Append(this._where.ToString());
            return sb.ToString();
        }
    }

    // Base class for expressions
    public class QueryExpr
    {
    }
    public class QueryExprEquals : QueryExpr
    {
        public FieldDescription LHS;
        public string RHS;

        public override string ToString()
        {
            return string.Format("{0}={1}", this.LHS.InternalName, this.RHS);
        }
    }
    public class QueryExprAnd : QueryExpr
    {
        public QueryExpr LHS;
        public QueryExpr RHS;

        public override string ToString()
        {
            return string.Format("({0}) AND ({1})", this.LHS, this.RHS);
        }
    }

}