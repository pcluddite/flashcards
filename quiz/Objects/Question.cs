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
using System.Xml;
using Baxendale.Data.Xml;

namespace Baxendale.Quiz.Objects
{
    public class Question : IXmlSerializableObject
    {
        [XmlSerializableProperty(Name = "prompt")]
        public string Prompt { get; set; }

        [XmlSerializableProperty(Name = "answer")]
        public Answer Answer { get; set; }

        public Question()
        {
        }

        public Question(string prompt, Answer answer)
        {
            Prompt = prompt;
            Answer = answer;
        }

        public override string ToString()
        {
            return Prompt;
        }

        public override bool Equals(object obj)
        {
            Question q = obj as Question;
            if ((object)q == null)
                return false;
            return Equals(q);
        }

        public bool Equals(Question q)
        {
            if (ReferenceEquals(q, this))
                return true;
            if ((object)q == null)
                return false;
            return Prompt == q.Prompt && Answer == q.Answer;
        }

        public static bool operator ==(Question left, Question right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if ((object)left == null)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(Question left, Question right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (Prompt == null ? 0 : Prompt.GetHashCode()) | (Answer == null ? 0 : Answer.GetHashCode());
        }
    }
}
