using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BloomGarden.Core.Json
{
    public sealed class JsonParseException : Exception
    {
        public int Position { get; }

        public JsonParseException(string message, int position)
            : base($"{message} (position {position})")
        {
            Position = position;
        }
    }

    /// <summary>
    /// Minimal strict JSON reader with no UnityEngine dependency.
    /// Produces Dictionary&lt;string, object&gt;, List&lt;object&gt;, string, long, double, bool or null.
    /// Integers without fraction/exponent are returned as long so capacities can be checked exactly.
    /// </summary>
    public static class MiniJson
    {
        public static object Parse(string text)
        {
            if (text == null) throw new JsonParseException("Input is null", 0);
            var reader = new Reader(text);
            reader.SkipWhitespace();
            object value = reader.ReadValue(0);
            reader.SkipWhitespace();
            if (!reader.AtEnd) throw new JsonParseException("Unexpected trailing characters", reader.Position);
            return value;
        }

        private sealed class Reader
        {
            private const int MaxDepth = 64;
            private readonly string _text;
            private int _pos;

            public Reader(string text)
            {
                _text = text;
                // Tolerate a UTF-8 BOM decoded as U+FEFF.
                if (_text.Length > 0 && _text[0] == '﻿') _pos = 1;
            }

            public int Position => _pos;
            public bool AtEnd => _pos >= _text.Length;

            public void SkipWhitespace()
            {
                while (_pos < _text.Length)
                {
                    char c = _text[_pos];
                    if (c == ' ' || c == '\t' || c == '\n' || c == '\r') _pos++;
                    else break;
                }
            }

            public object ReadValue(int depth)
            {
                if (depth > MaxDepth) throw new JsonParseException("Nesting too deep", _pos);
                if (AtEnd) throw new JsonParseException("Unexpected end of input", _pos);
                char c = _text[_pos];
                switch (c)
                {
                    case '{': return ReadObject(depth);
                    case '[': return ReadArray(depth);
                    case '"': return ReadString();
                    case 't': ExpectLiteral("true"); return true;
                    case 'f': ExpectLiteral("false"); return false;
                    case 'n': ExpectLiteral("null"); return null;
                    default:
                        if (c == '-' || (c >= '0' && c <= '9')) return ReadNumber();
                        throw new JsonParseException($"Unexpected character '{c}'", _pos);
                }
            }

            private Dictionary<string, object> ReadObject(int depth)
            {
                var result = new Dictionary<string, object>(StringComparer.Ordinal);
                _pos++; // {
                SkipWhitespace();
                if (Peek() == '}') { _pos++; return result; }
                while (true)
                {
                    SkipWhitespace();
                    if (Peek() != '"') throw new JsonParseException("Expected property name", _pos);
                    string key = ReadString();
                    SkipWhitespace();
                    Expect(':');
                    SkipWhitespace();
                    if (result.ContainsKey(key)) throw new JsonParseException($"Duplicate property '{key}'", _pos);
                    result[key] = ReadValue(depth + 1);
                    SkipWhitespace();
                    char c = Next();
                    if (c == '}') return result;
                    if (c != ',') throw new JsonParseException("Expected ',' or '}'", _pos - 1);
                }
            }

            private List<object> ReadArray(int depth)
            {
                var result = new List<object>();
                _pos++; // [
                SkipWhitespace();
                if (Peek() == ']') { _pos++; return result; }
                while (true)
                {
                    SkipWhitespace();
                    result.Add(ReadValue(depth + 1));
                    SkipWhitespace();
                    char c = Next();
                    if (c == ']') return result;
                    if (c != ',') throw new JsonParseException("Expected ',' or ']'", _pos - 1);
                }
            }

            private string ReadString()
            {
                Expect('"');
                var sb = new StringBuilder();
                while (true)
                {
                    if (AtEnd) throw new JsonParseException("Unterminated string", _pos);
                    char c = _text[_pos++];
                    if (c == '"') return sb.ToString();
                    if (c < 0x20) throw new JsonParseException("Control character in string", _pos - 1);
                    if (c != '\\') { sb.Append(c); continue; }
                    if (AtEnd) throw new JsonParseException("Unterminated escape", _pos);
                    char e = _text[_pos++];
                    switch (e)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            if (_pos + 4 > _text.Length) throw new JsonParseException("Invalid unicode escape", _pos);
                            string hex = _text.Substring(_pos, 4);
                            if (!int.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out int code))
                                throw new JsonParseException("Invalid unicode escape", _pos);
                            sb.Append((char)code);
                            _pos += 4;
                            break;
                        default:
                            throw new JsonParseException($"Invalid escape '\\{e}'", _pos - 1);
                    }
                }
            }

            private object ReadNumber()
            {
                int start = _pos;
                bool isInteger = true;
                if (Peek() == '-') _pos++;
                if (AtEnd || !char.IsDigit(_text[_pos])) throw new JsonParseException("Invalid number", start);
                if (_text[_pos] == '0') _pos++;
                else while (!AtEnd && char.IsDigit(_text[_pos])) _pos++;
                if (!AtEnd && _text[_pos] == '.')
                {
                    isInteger = false;
                    _pos++;
                    if (AtEnd || !char.IsDigit(_text[_pos])) throw new JsonParseException("Invalid number", start);
                    while (!AtEnd && char.IsDigit(_text[_pos])) _pos++;
                }
                if (!AtEnd && (_text[_pos] == 'e' || _text[_pos] == 'E'))
                {
                    isInteger = false;
                    _pos++;
                    if (!AtEnd && (_text[_pos] == '+' || _text[_pos] == '-')) _pos++;
                    if (AtEnd || !char.IsDigit(_text[_pos])) throw new JsonParseException("Invalid number", start);
                    while (!AtEnd && char.IsDigit(_text[_pos])) _pos++;
                }
                string token = _text.Substring(start, _pos - start);
                if (isInteger && long.TryParse(token, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long l))
                    return l;
                return double.Parse(token, NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            private void ExpectLiteral(string literal)
            {
                if (string.CompareOrdinal(_text, _pos, literal, 0, literal.Length) != 0)
                    throw new JsonParseException($"Expected '{literal}'", _pos);
                _pos += literal.Length;
            }

            private void Expect(char c)
            {
                if (AtEnd || _text[_pos] != c) throw new JsonParseException($"Expected '{c}'", _pos);
                _pos++;
            }

            private char Peek() => AtEnd ? '\0' : _text[_pos];

            private char Next()
            {
                if (AtEnd) throw new JsonParseException("Unexpected end of input", _pos);
                return _text[_pos++];
            }
        }
    }
}
