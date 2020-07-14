using FluentAssertions;
using NUnit.Framework;
using System;

namespace EnumParser.Test
{
    public class EnumTest
    {
        [Test]
        public void TestParseWithMapping()
        {
            var enumValue = EnumParser.Parse(typeof(CashTransactionType), "Other Fees");

            enumValue.Should().Be(CashTransactionType.OtherFees);
        }

        [Test]
        public void TestParseFlags()
        {
            var enumValue = EnumParser.Parse(typeof(OpenClose), "O;C");

            enumValue.Should().HaveFlag(OpenClose.C).And.HaveFlag(OpenClose.O);
        }

        [Test]
        public void TestNotesMoreThan32Flags()
        {
            var enumValue = EnumParser.Parse(typeof(Notes), "Ex;A");

            enumValue.Should().HaveFlag(Notes.Assigned).And.HaveFlag(Notes.Exercised);
        }

        [Test]
        public void TestLeadingDelimiter()
        {
            var enumValue = EnumParser.Parse(typeof(OpenClose), ";O;C");

            enumValue.Should().HaveFlag(OpenClose.C).And.HaveFlag(OpenClose.O);
        }
    }

    [EnumName]
    enum CashTransactionType
    {
        [EnumName("Other Fees")]
        OtherFees
    }
    
    [Flags]
    enum OpenClose
    {
        O = 2^0, C = 2^1
    }

    [EnumName]
    [Flags]
    enum Notes : long
    {
        [EnumName("A")]
        Assigned = 2^0,
        [EnumName("Ex")]
        Exercised = 2^1
    }
}
