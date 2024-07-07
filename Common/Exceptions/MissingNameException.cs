using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class MissingNameException : ApplicationBaseException
    {
        private const string InnerMessage = "Name {0} cannot be null.";
        public string TypeName { get; }
        public MissingNameException(string typeName) : this(typeName, null)
        {
        }

        public MissingNameException(string typeName, Exception innerException)
            : base(string.Format(InnerMessage, typeName), innerException)
        {
            TypeName = typeName;
        }
    }
}
