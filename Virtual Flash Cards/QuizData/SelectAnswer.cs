﻿//
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace VirtualFlashCards.QuizData
{
    public class SelectAnswer : MultiAnswer
    {
        public new const string TYPE = "select";

        public override IEnumerable<string> CorrectOptions
        {
            get
            {
                foreach (KeyValuePair<string, bool> option in OptionDictionary)
                {
                    if (option.Value)
                    {
                        return new string[] { option.Key };
                    }
                }
                return new string[0];
            }
        }

        protected SelectAnswer()
            : base()
        {
        }

        protected SelectAnswer(XElement node)
            : base(node)
        {
        }

        public override XElement ToXml(XName name)
        {
            return base.ToXml(name);
        }

        public override Answer CloneWithNewInput(Control control)
        {
            GroupBox group = (GroupBox)control;
            SelectAnswer answer = new SelectAnswer();
            foreach (RadioButton radio in group.Controls.Cast<RadioButton>())
            {
                answer.OptionDictionary[radio.Text] = radio.Checked;
            }
            return answer;
        }

        public override Control CreateFormControl(Font font)
        {
            GroupBox group = new GroupBox()
            {
                Name = "grpSelect",
                Text = "Select One",
                Font = font
            };
            int i = 0;
            foreach (string option in OptionsRandomized)
            {
                RadioButton radio = new RadioButton();
                radio.Name = "radioAnswer" + i++;
                radio.Text = option;
                radio.Location = new Point(10, i * (radio.Height + 5));
                radio.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                group.Controls.Add(radio);
            }
            if (group.Controls.Count > 0)
            {
                Control lastControl = group.Controls[group.Controls.Count - 1];
                group.Height = lastControl.Height + lastControl.Location.Y + 10;
            }
            return group;
        }

        public override bool IsCorrect(Control control)
        {
            GroupBox group = (GroupBox)control;
            foreach (RadioButton radio in group.Controls.Cast<RadioButton>())
            {
                if (radio.Checked)
                {
                    return OptionDictionary[radio.Text];
                }
            }
            return false;
        }

        public override bool Equals(MultiAnswer other)
        {
            return Equals(other as SelectAnswer);
        }

        public virtual bool Equals(SelectAnswer other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if ((object)other == null)
                return false;
            if (other.OptionDictionary.Count != OptionDictionary.Count)
                return false;
            return OptionDictionary.Equals(other.OptionDictionary);
        }
    }
}
