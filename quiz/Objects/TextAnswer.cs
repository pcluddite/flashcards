//
//    Quiz
//    Copyright (C) 2009-2021 Timothy Baxendale
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
using System;
using System.Xml.Linq;
using Baxendale.Data.Xml;

namespace Baxendale.Quiz.Objects
{
    public class TextAnswer : Answer, IEquatable<TextAnswer>
    {
        public const string TYPE = "text";

        public string Value { get; set; }
        public bool MatchCase { get; set; }

        protected TextAnswer(XElement node)
        {
            Value = node.Attribute("value").Value;
            MatchCase = node.Attribute("matchCase").Value(false);
        }

        public TextAnswer(string value)
        {
            Value = value;
        }

        public TextAnswer(string value, bool matchCase)
        {
            Value = value;
            MatchCase = matchCase;
        }

        public override bool IsCorrect(string input)
        {
            if (MatchCase)
            {
                return string.Equals(Value, input, StringComparison.CurrentCulture);
            }
            else
            {
                return string.Equals(Value, input, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public override Answer CloneWithNewInput(string input)
        {
            return new TextAnswer(input, MatchCase);
        }

        public override XElement ToXml(XName name)
        {
            XElement node = base.ToXml(name);
            node.SetAttributeValue("value", Value);
            if (MatchCase)
                node.SetAttributeValue("matchCase", MatchCase.ToString());
            return node;
        }

        public override bool Equals(Answer other)
        {
            return Equals(other as TextAnswer);
        }

        public virtual bool Equals(TextAnswer other)
        {
            if (ReferenceEquals(other, this))
                return true;
            if ((object)other == null)
                return false;
            return Value == other.Value && MatchCase == other.MatchCase;
        }

        public override int GetHashCode()
        {
            return (Value == null ? 0 : Value.GetHashCode()) ^ MatchCase.GetHashCode();
        }

        public override string GetPrompt()
        {
            return "";
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
