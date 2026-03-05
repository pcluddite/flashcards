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
using System.Globalization;
using System.Linq;
using System.Text;
using Baxendale.Data.Collections;

namespace Baxendale.Quiz.Commands
{
    internal class Input
    {
        private List<string> args;

        public string Text { get; private set; }

        public string Name
        {
            get
            {
                return args[0];
            }
        }

        public IList<string> Arguments
        {
            get
            {
                if (ArgumentCount == 0)
                    return Collections<string>.EmptyList;
                return args.GetRange(1, ArgumentCount);
            }
        }

        public int ArgumentCount
        {
            get
            {
                return args.Count - 1;
            }
        }

        public Input(string text)
        {
            if (text == null)
                throw new ArgumentNullException();
            Text = text;
            args = Parse(text);
        }

        private static List<string> Parse(string text)
        {
            List<string> args = new List<string>();
            StringBuilder currentArg = new StringBuilder();
            int currIdx = 0;
            while(currIdx < text.Length)
            {
                for (; currIdx < text.Length && !char.IsWhiteSpace(text[currIdx]); ++currIdx)
                {
                    char c = text[currIdx];
                    if (c == '\'' || c == '\"')
                    {
                        string parsed;
                        currIdx = ReadString(text, currIdx, c, out parsed);
                        currentArg.Append(parsed);
                    }
                    else
                    {
                        currentArg.Append(c);
                    }
                }
                if (currentArg.Length > 0)
                {
                    args.Add(currentArg.ToString());
                    currentArg.Clear();
                }

                while (currIdx < text.Length && char.IsWhiteSpace(text[currIdx])) 
                    ++currIdx;
            }
            args.TrimExcess();
            return args;
        }

        private static int ReadString(string text, int startIndex, char closeChar, out string parsed)
        {
            StringBuilder sb = new StringBuilder(text.Length);
            int curIdx = startIndex + 1; // The first character should be the quote
            for (; curIdx < text.Length; ++curIdx)
            {
                char cur = text[curIdx];
                if (cur == '\n' || cur == '\r')
                {
                    throw new UnterminatedStringException(text, startIndex, closeChar);
                }
                else if (cur == '\\')
                {
                    if (++curIdx >= text.Length)
                        throw new UnterminatedStringException(text, startIndex, closeChar);
                    switch (text[curIdx])
                    {
                        case 'b': sb.Append('\b'); break;
                        case 't': sb.Append('\t'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'r': sb.Append('\r'); break;
                        case '\"': sb.Append('\"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '\'': sb.Append('\''); break;
                        case 'u':
                            if ((curIdx += 4) >= text.Length)
                                throw new UnterminatedUnicodeEscapeException(text, curIdx - 4);
                            ushort code;
                            if (ushort.TryParse(text.Substring(curIdx - 3, 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out code))
                            {
                                sb.Append((char)code);
                            }
                            else
                            {
                                throw new UnknownEscapeCharacterException(text, curIdx - 4);
                            }
                            break;
                        default:
                            throw new UnknownEscapeCharacterException(text, curIdx);
                    }
                }
                else if (cur == closeChar)
                {
                    break;
                }
                else
                {
                    sb.Append(cur);
                }
            }
            parsed = sb.ToString();
            return curIdx;
        }

        public T GetArg<T>(int index) where T : IConvertible
        {
            return GetArg<T>(index, null);
        }

        public T GetArg<T>(int index, string errorMsg) where T : IConvertible
        {
            try
            {
                return (T)Convert.ChangeType(args[index], typeof(T));
            }
            catch(InvalidCastException)
            {
                throw new InvalidTypeException(index, typeof(T), errorMsg);
            }
        }

        public static implicit operator Input(string text)
        {
            return new Input(text);
        }
    }
}
