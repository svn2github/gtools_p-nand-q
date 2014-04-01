using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pserv4
{
    public class ActionTemplateInfo : Dictionary<string, string>
    {
        public readonly string ID;

        public ActionTemplateInfo(string id)
        {
            ID = id;
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append("ActionTemplateInfo{");
            output.Append(ID);
            output.Append(",StartType=");
            output.Append(this["StartTypeString"]);
            output.Append("}");
            return output.ToString();
        }
    }
}
