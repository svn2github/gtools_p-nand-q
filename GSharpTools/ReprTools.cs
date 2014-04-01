using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSharpTools
{
    public static class ReprTools
    {
        public static string repr<TKey, TValue>(Dictionary<TKey, TValue> input)
        {
            StringBuilder result = new StringBuilder();
            result.Append("{ ");

            bool first = true;
            foreach(TKey key in input.Keys)
            {
                if (first)
                    first = false;
                else
                    result.Append(", ");
                result.Append(key);
                result.Append("=");
                result.Append(input[key]);
            }
            result.Append("}");
            return result.ToString();
        }
    }
}
