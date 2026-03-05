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
using System.Xml;
using System.Xml.Linq;
using Baxendale.Data.Xml;

namespace VirtualFlashCards.QuizData
{
    public class Quiz : IXmlSerializableObject, IList<Question>
    {
        private const int VERSION = 4;

        private List<Question> allQuestions = new List<Question>();
        private QuestionAnswerMap incorrect = new QuestionAnswerMap();

        public int Count
        {
            get
            {
                return allQuestions.Count;
            }
        }

        public Question this[int index]
        {
            get
            {
                return allQuestions[index];
            }
            set
            {
                allQuestions[index] = value;
            }
        }

        public Quiz()
        {
        }

        public Quiz(IEnumerable<Question> questions)
        {
            AddRange(questions);
        }

        public void Shuffle()
        {
            List<Question> oldQuestionList = allQuestions;
            List<Question> newQuestionList = new List<Question>(oldQuestionList.Count);
            Random r = new Random();
            while (oldQuestionList.Count > 0)
            {
                int oldIndex = r.Next(0, oldQuestionList.Count);
                Question q = oldQuestionList[oldIndex];
                oldQuestionList.RemoveAt(oldIndex);
                newQuestionList.Add(q);
            }
            allQuestions = newQuestionList;
        }

        public void AddWrongAnswer(int questionIndex, Answer wrongAnswer)
        {
            if (wrongAnswer == null)
                throw new ArgumentNullException();
            incorrect.Add(allQuestions[questionIndex], wrongAnswer);
        }

        public Answer GetWrongAnswer(int questionIndex)
        {
            return incorrect[allQuestions[questionIndex]];
        }

        public Quiz WrongQuiz()
        {
            return new Quiz(incorrect.Questions);
        }

        public static Quiz FromFile(string path)
        {
            XDocument doc = XDocument.Load(path);
            return XmlSerializer.Default.Deserialize<Quiz>(doc.Root);
        }

        public static Quiz FromXml(XElement quizNode, XName name)
        {
            if (name != null && quizNode != null)
                quizNode = quizNode.Element(name);
            if (quizNode == null)
                throw new XmlException("Could not find quiz node. This file may be corrupt.");
            if (quizNode.Attribute("version").Value(0) != VERSION)
                throw new XmlException("This quiz file is not compatible with this version of Flash Cards");
            Quiz q = new Quiz();
            foreach (XElement question in quizNode.Elements("question"))
            {
                q.Add(XmlSerializer.Default.Deserialize<Question>(question));
            }
            return q;
        }

        public XElement ToXml(XName name)
        {
            XElement quizNode = new XElement(name);
            quizNode.SetAttributeValue("version", VERSION);
            foreach (Question q in allQuestions)
            {
                quizNode.Add(XmlSerializer.Default.Serialize<Question>(q, "question"));
            }
            return quizNode;
        }

        public void AddRange(IEnumerable<Question> questions)
        {
            foreach (Question q in questions)
            {
                Add(q);
            }
        }

        #region IList<Question> Members

        int IList<Question>.IndexOf(Question item)
        {
            for (int idx = 0; idx < allQuestions.Count; ++idx)
            {
                if (item == allQuestions[idx])
                    return idx;
            }
            return -1;
        }

        void IList<Question>.Insert(int index, Question item)
        {
            allQuestions.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Question q = allQuestions[index];
            allQuestions.RemoveAt(index);
            incorrect.Remove(q);
        }

        #endregion

        #region ICollection<Question> Members

        public void Add(Question question)
        {
            allQuestions.Add(question);
        }

        public void Clear()
        {
            allQuestions.Clear();
            incorrect.Clear();
        }

        public bool Contains(Question question)
        {
            return allQuestions.Contains(question);
        }

        public void CopyTo(Question[] array, int arrayIndex)
        {
            allQuestions.CopyTo(array, arrayIndex);
        }

        bool ICollection<Question>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Question question)
        {
            if (!allQuestions.Remove(question))
                return false;
            incorrect.Remove(question);
            return true;
        }

        #endregion

        #region IEnumerable<Question> Members

        IEnumerator<Question> IEnumerable<Question>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
