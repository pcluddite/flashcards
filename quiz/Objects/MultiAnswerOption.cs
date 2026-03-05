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
using Baxendale.Data.Xml;

namespace Baxendale.Quiz.Objects
{
    public partial class MultiAnswer
    {
        protected class MultiAnswerOption : IEquatable<MultiAnswerOption>, IXmlSerializableObject
        {
            [XmlSerializableProperty(Name = "value", Default = false)]
            public virtual bool IsCorrect { get; set; }

            [XmlSerializableProperty(Name = "text")]
            public virtual string Text { get; set; }

            public MultiAnswerOption()
            {
            }

            public MultiAnswerOption(string text, bool correct)
            {
                Text = text;
                IsCorrect = correct;
            }

            public MultiAnswerOption(MultiAnswerOption other)
            {
                Text = other.Text;
                IsCorrect = other.IsCorrect;
            }

            public override string ToString()
            {
                return Text;
            }

            public virtual string ToString(char alpha)
            {
                return string.Format("{0,-3}. {1}", alpha, ToString());
            }

            public virtual string ToString(MultiAnswerKey multiAnswerKey)
            {
                return ToString((char)multiAnswerKey);
            }

            public override bool Equals(object obj)
            {
                MultiAnswerOption other = obj as MultiAnswerOption;
                if (other == null)
                    return false;
                return Equals(other);
            }

            public override int GetHashCode()
            {
                return Text.GetHashCode() ^ IsCorrect.GetHashCode();
            }

            #region IEquatable<MultiAnswerOption> Members

            public bool Equals(MultiAnswerOption other)
            {
                return Text == other.Text && IsCorrect == other.IsCorrect;
            }

            public static bool operator ==(MultiAnswerOption left, MultiAnswerOption right)
            {
                if (left == null)
                    return (object)right == null;
                return left.Equals(right);
            }

            public static bool operator !=(MultiAnswerOption left, MultiAnswerOption right)
            {
                return !(left == right);
            }

            #endregion
        }
    }
}
