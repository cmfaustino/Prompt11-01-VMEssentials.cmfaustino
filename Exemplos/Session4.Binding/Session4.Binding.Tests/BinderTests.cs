using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Drawing;

namespace Session4.Binding.Tests
{
    [TestFixture]
    class BinderTests
    {
        class A
        {
            public int AnInteger { get; set; }
            public string AString { get; set; }
        }

        [Test]
        public void can_bind_to_int_and_string_prop_and_field_of_A()
        {
            var binder = new Binder();
            var pairs = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("AnInteger","2"),
                    new KeyValuePair<string, string>("AString","abc")
                };
            var a = binder.BindTo<A>(pairs);

            Assert.AreEqual(2, a.AnInteger);
            Assert.AreEqual("abc", a.AString);
        }

        class B
        {
            public int AnInteger { get; set; }
            public string AString { get; set; }
            public Point APoint { get; set; }
        }

        [Test]
        public void when_member_has_invalid_type_then_BindTo_throws()
        {
            var binder = new Binder();
            var pairs = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("AnInteger","2"),
                    new KeyValuePair<string, string>("AString","abc"),
                    new KeyValuePair<string, string>("APoint","1,2")
                };
            var exc = Assert.Throws<InvalidMemberTypeException>(() =>
                binder.BindTo<B>(pairs)
            );
            Assert.AreEqual(typeof(B).GetProperty("APoint"),exc.MemberInfo);
        }
    }
}
