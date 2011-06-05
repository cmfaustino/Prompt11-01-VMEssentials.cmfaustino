using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Session4.Binding
{
    public abstract class BinderException : Exception
    {
        protected BinderException(string msg) : base(msg)
        {
        }
    }

    public class InvalidMemberTypeException : BinderException
    {
        public MemberInfo MemberInfo { get; private set; }
        public Type MemberType { get; private set; }

        public InvalidMemberTypeException(MemberInfo mi, Type type)
            : base(string.Format("Member {0} of type {1} has an invalid type {2}",
            mi.Name, mi.DeclaringType.Name, type.Name))
        {
            this.MemberInfo = mi;
            this.MemberType = type;
        }
    }
}
