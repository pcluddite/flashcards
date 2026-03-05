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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Baxendale.Data.Collections;
using Baxendale.Data.Xml;

namespace Baxendale.Quiz.Objects
{
    public partial class MultiAnswer : Answer, IEquatable<MultiAnswer>
    {
        public const string TYPE = "multi";

        protected MultiAnswerMap OptionDictionary = new MultiAnswerMap();
        
        protected MultiAnswer()
        {
        }

        protected MultiAnswer(MultiAnswer other)
        {
            foreach (var option in other.OptionDictionary)
                OptionDictionary[option.Key] = new MultiAnswerOption(option.Value);
        }

        protected MultiAnswer(XElement node)
        {
            using(IEnumerator<char> sequence = 'a'.AlphaSequence().GetEnumerator())
            {
                foreach (XElement optionNode in node.Elements("option").Randomize())
                {
                    if (!sequence.MoveNext())
                        throw new XmlSerializationException(optionNode, "Too many answers specified for the question");
                    OptionDictionary.Add(sequence.Current, XmlSerializer.Default.Deserialize<MultiAnswerOption>(optionNode));
                }
            }
        }

        public override bool IsCorrect(string input)
        {
            return OptionDictionary.CorrectKeys.ContainsOnly(input.Split(',').Select(o => new MultiAnswerKey(o)));
        }

        public override Answer CloneWithNewInput(string input)
        {
            MultiAnswer answer = new MultiAnswer();
            string[] answers = input.ToLower().Split(',');
            foreach (var option in OptionDictionary)
                answer.OptionDictionary[option.Key] = new MultiAnswerOption(option.Value.Text, Array.IndexOf(answers, option.Key.Key) > -1);
            return answer;
        }

        public override XElement ToXml(XName name)
        {
            XElement elem = base.ToXml(name);
            foreach (MultiAnswerOption option in OptionDictionary.Values)
                elem.Add(XmlSerializer.Default.Serialize(option, "option"));
            return elem;
        }

        public override string GetPrompt()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Choose all that apply:");
            sb.AppendLine();
            foreach (var option in OptionDictionary)
                sb.AppendLine(option.Value.ToString(option.Key));
            return sb.ToString();
        }

        public override bool IsValid(string input)
        {
            foreach (string answer in input.Split(','))
            {
                if (!OptionDictionary.ContainsKey(answer))
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var option in OptionDictionary)
            {
                sb.Append(option.Value.IsCorrect ? "[*] " : "[ ] ");
                sb.AppendLine(option.Value.ToString(option.Key));
            }
            return sb.ToString();
        }

        public override bool Equals(Answer other)
        {
            return Equals(other as MultiAnswer);
        }

        public virtual bool Equals(MultiAnswer other)
        {
            if ((object)other == null)
                return false;
            if ((object)this == other)
                return true;
            return OptionDictionary.Equals(other.OptionDictionary);
        }

        public override int GetHashCode()
        {
            return OptionDictionary.GetHashCode();
        }
    }
}
