//
//    Virtual Flash Cards
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Baxendale.Data.Xml;

namespace VirtualFlashCards.QuizData
{
    public class MultiAnswer : Answer
    {
        public const string TYPE = "multi";

        protected Dictionary<string, bool> OptionDictionary = new Dictionary<string, bool>();

        public virtual IEnumerable<string> Options
        {
            get
            {
                return OptionDictionary.Keys;
            }
        }

        public virtual IEnumerable<string> CorrectOptions
        {
            get
            {
                foreach (KeyValuePair<string, bool> choice in OptionDictionary)
                {
                    if (choice.Value)
                        yield return choice.Key;
                }
            }
        }

        public virtual IEnumerable<string> OptionsRandomized
        {
            get
            {
                List<string> options = new List<string>(Options);
                Random r = new Random();
                while (options.Count > 0)
                {
                    int idx = r.Next(0, options.Count);
                    yield return options[idx];
                    options.RemoveAt(idx);
                }
            }
        }

        protected MultiAnswer()
        {
        }

        protected MultiAnswer(XElement node)
        {
            foreach (XElement optionNode in node.Elements("option"))
            {
                MultiAnswerOption option = XmlSerializer.Default.Deserialize<MultiAnswerOption>(optionNode);
                OptionDictionary.Add(option.Text, option.IsCorrect);
            }
        }

        public override bool IsCorrect(Control control)
        {
            GroupBox group = (GroupBox)control;
            foreach (CheckBox check in group.Controls.Cast<CheckBox>())
            {
                if ((check.Checked && !OptionDictionary[check.Text])
                     || (!check.Checked && OptionDictionary[check.Text]))
                {
                    return false;
                }
            }
            return true;
        }

        public override Answer CloneWithNewInput(Control control)
        {
            GroupBox group = (GroupBox)control;
            MultiAnswer answer = new MultiAnswer();
            foreach (CheckBox check in group.Controls.Cast<CheckBox>())
            {
                answer.OptionDictionary[check.Text] = check.Checked;
            }
            return answer;
        }

        public override Control CreateFormControl(Font font)
        {
            GroupBox group = new GroupBox()
            {
                Name = "grpMulti",
                Text = "Select All That Apply",
                Font = font
            };
            int i = 0;
            foreach (string option in OptionsRandomized)
            {
                CheckBox checkAnswer = new CheckBox();
                checkAnswer.Name = "checkAnswer" + i++;
                checkAnswer.Text = option;
                checkAnswer.Location = new Point(10, i * (checkAnswer.Height + 5));
                checkAnswer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                group.Controls.Add(checkAnswer);
            }
            if (group.Controls.Count > 0)
            {
                Control lastControl = group.Controls[group.Controls.Count - 1];
                group.Height = lastControl.Height + lastControl.Location.Y + 10;
            }
            return group;
        }

        public override XElement ToXml(XName name)
        {
            XElement elem = base.ToXml(name);
            foreach (KeyValuePair<string, bool> option in OptionDictionary)
            {
                elem.Add(XmlSerializer.Default.Serialize(new MultiAnswerOption(option.Key, option.Value), "option"));
            }
            return elem;
        }

        public override bool Equals(Answer other)
        {
            return Equals(other as MultiAnswer);
        }

        public virtual bool Equals(MultiAnswer other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if ((object)other == null)
                return false;
            if (other.OptionDictionary.Count != OptionDictionary.Count)
                return false;
            return OptionDictionary.Equals(other.OptionDictionary);
        }

        public override int GetHashCode()
        {
            return OptionDictionary.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, bool> option in OptionDictionary)
            {
                sb.Append(option.Value ? "[*] " : "[ ] ");
                sb.AppendLine(option.Key);
            }
            return sb.ToString();
        }

        private class MultiAnswerOption : IXmlSerializableObject
        {
            [XmlSerializableField(Name = "value", Default = false)]
            public readonly bool IsCorrect;

            [XmlSerializableField(Name = "text")]
            public readonly string Text;

            public MultiAnswerOption()
            {
            }

            public MultiAnswerOption(string text, bool correct)
            {
                Text = text;
                IsCorrect = correct;
            }

            public override string ToString()
            {
                return Text;
            }

            public override bool Equals(object obj)
            {
                MultiAnswerOption other = obj as MultiAnswerOption;
                if (other == null)
                    return false;
                return Equals(other);
            }

            public bool Equals(MultiAnswerOption other)
            {
                return Text == other.Text && IsCorrect == other.IsCorrect;
            }

            public override int GetHashCode()
            {
                return Text.GetHashCode() ^ IsCorrect.GetHashCode();
            }

            public static bool operator ==(MultiAnswerOption left, MultiAnswerOption right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(MultiAnswerOption left, MultiAnswerOption right)
            {
                return !left.Equals(right);
            }
        }
    }
}
