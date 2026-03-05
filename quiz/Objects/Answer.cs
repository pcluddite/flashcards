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
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Baxendale.Data.Xml;

namespace Baxendale.Quiz.Objects
{
    public abstract class Answer : IEquatable<Answer>, IXmlSerializableObject
    {
        public abstract bool IsCorrect(string input);

        public abstract Answer CloneWithNewInput(string input);

        public virtual bool IsValid(string input)
        {
            return true;
        }

        public virtual XElement ToXml(XName name)
        {
            XElement elem = new XElement(name);
            elem.SetAttributeValue("type", GetAnswerType(GetType()));
            return elem;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            if ((object)this == obj)
                return true;
            return Equals(obj as Answer);
        }

        public abstract bool Equals(Answer other);

        public abstract override int GetHashCode();

        public abstract override string ToString();

        public abstract string GetPrompt();

        public static Answer FromXml(XElement node, XName name)
        {
            if (name != null)
                node = node.Element(name);
            if (node.Name != "answer")
                throw new ArgumentException("Cannot convert node to Answer class because node is not an Answer");
            string ansTypeName = node.Attribute("type").Value("");
            Type t = Type.GetType(GetAnswerClassNameFromType(ansTypeName));
            if (t == null)
                throw new ArgumentException("Encountered unknown answer type '" + ansTypeName + "'. The document may not be supported by this version of Flash Cards");
            ConstructorInfo ctor = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(XElement) }, null);
            return (Answer)ctor.Invoke(new object[] { node });
        }

        private static string GetAnswerType(Type ansType)
        {
            if (!ansType.IsAssignableFrom(typeof(Answer)))
                throw new ArgumentException("GetAnswerType() was not passed a type of Answer");
            FieldInfo typeField = ansType.GetField("TYPE");
            if (typeField == null || !typeof(string).IsAssignableFrom(typeField.FieldType) || !typeField.IsLiteral)
                throw new ArgumentException("The Answer object must have a constant field named TYPE");
            return (string)typeField.GetValue(null);
        }

        private static string GetAnswerClassNameFromType(string ansTypeName)
        {
            StringBuilder sb = new StringBuilder(typeof(Answer).Namespace.Length + ansTypeName.Length + "Answer".Length + 1);
            sb.Append(typeof(Answer).Namespace);
            sb.Append('.');
            sb.Append(char.ToUpper(ansTypeName[0]));
            for (int i = 1; i < ansTypeName.Length; ++i)
            {
                sb.Append(char.ToLower(ansTypeName[i]));
            }
            sb.Append("Answer");
            return sb.ToString();
        }
    }
}
