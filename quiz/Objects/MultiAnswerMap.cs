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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Baxendale.Data.Collections;

namespace Baxendale.Quiz.Objects
{
    public partial class MultiAnswer
    {
        protected class MultiAnswerMap : IDictionary<MultiAnswerKey, MultiAnswerOption>,
            IEquatable<MultiAnswerMap>, IEquatable<IDictionary<MultiAnswerKey, MultiAnswerOption>>
        {
            private IDictionary<MultiAnswerKey, MultiAnswerOption> options = new Dictionary<MultiAnswerKey, MultiAnswerOption>();

            public virtual IEnumerable<KeyValuePair<MultiAnswerKey, MultiAnswerOption>> CorrectOptions
            {
                get
                {
                    foreach (KeyValuePair<MultiAnswerKey, MultiAnswerOption> choice in options)
                    {
                        if (choice.Value.IsCorrect)
                            yield return choice;
                    }
                }
            }

            public virtual IEnumerable<MultiAnswerKey> CorrectKeys
            {
                get
                {
                    return CorrectOptions.Select(o => o.Key);
                }
            }

            public MultiAnswerMap()
            {
            }

            public MultiAnswerMap(IEnumerable<MultiAnswerOption> options)
            {
                
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as IDictionary<MultiAnswerKey, MultiAnswerOption>);
            }

            public override int GetHashCode()
            {
                return options.GetHashCode();
            }

            #region IDictionary<MultiAnswerKey,MultiAnswerOption> Members

            public void Add(MultiAnswerKey key, MultiAnswerOption value)
            {
                options.Add(key, value);
            }

            public bool ContainsKey(MultiAnswerKey key)
            {
                return options.ContainsKey(key);
            }

            public ICollection<MultiAnswerKey> Keys
            {
                get { return options.Keys; }
            }

            public bool Remove(MultiAnswerKey key)
            {
                return options.Remove(key);
            }

            public bool TryGetValue(MultiAnswerKey key, out MultiAnswerOption value)
            {
                return options.TryGetValue(key, out value);
            }

            public ICollection<MultiAnswerOption> Values
            {
                get { return options.Values; }
            }

            public MultiAnswerOption this[MultiAnswerKey key]
            {
                get
                {
                    return options[key];
                }
                set
                {
                    options[key] = value;
                }
            }

            #endregion

            #region ICollection<KeyValuePair<MultiAnswerKey,MultiAnswerOption>> Members

            void ICollection<KeyValuePair<MultiAnswerKey,MultiAnswerOption>>.Add(KeyValuePair<MultiAnswerKey, MultiAnswerOption> item)
            {
                ((ICollection<KeyValuePair<MultiAnswerKey, MultiAnswerOption>>)options).Add(item);
            }

            public void Clear()
            {
                options.Clear();
            }

            bool ICollection<KeyValuePair<MultiAnswerKey,MultiAnswerOption>>.Contains(KeyValuePair<MultiAnswerKey, MultiAnswerOption> item)
            {
                return ((ICollection<KeyValuePair<MultiAnswerKey, MultiAnswerOption>>)options).Contains(item);
            }

            void ICollection<KeyValuePair<MultiAnswerKey,MultiAnswerOption>>.CopyTo(KeyValuePair<MultiAnswerKey, MultiAnswerOption>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<MultiAnswerKey, MultiAnswerOption>>)options).CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return options.Count; }
            }

            public bool IsReadOnly
            {
                get { return options.IsReadOnly; }
            }

            public bool Remove(KeyValuePair<MultiAnswerKey, MultiAnswerOption> item)
            {
                return ((ICollection<KeyValuePair<MultiAnswerKey, MultiAnswerOption>>)options).Remove(item);
            }

            #endregion

            #region IEnumerable<KeyValuePair<MultiAnswerKey,MultiAnswerOption>> Members

            public IEnumerator<KeyValuePair<MultiAnswerKey, MultiAnswerOption>> GetEnumerator()
            {
                foreach (char alpha in 'a'.AlphaSequence(options.Count))
                    yield return new KeyValuePair<MultiAnswerKey, MultiAnswerOption>(alpha, options[alpha]);
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region IEquatable<MultiAnswerMap> Members

            public bool Equals(MultiAnswerMap other)
            {
                if ((object)this == other)
                    return true;
                if ((object)other == null)
                    return false;
                if (Count != other.Count)
                    return false;
                return this.ContainsOnly(other);
            }

            #endregion

            #region IEquatable<IDictionary<MultiAnswerKey,MultiAnswerOption>> Members

            public bool Equals(IDictionary<MultiAnswerKey, MultiAnswerOption> other)
            {
                if ((object)other == null)
                    return false;
                MultiAnswerMap otherAsMap = other as MultiAnswerMap;
                if ((object)otherAsMap != null)
                    return Equals(otherAsMap);
                if (Count != other.Count)
                    return false;
                return this.ContainsOnly(other);
            }

            #endregion
        }
    }
}
