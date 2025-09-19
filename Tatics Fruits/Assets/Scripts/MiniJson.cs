// MiniJson: parse/serialize simples baseado em Unity Wiki (domínio público/CC0).
// Versão compacta só com Deserialize(string)->object (Dictionary/List/string/numbers/bool/null).
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class MiniJson
{
    public static object Deserialize(string json) => Parser.Parse(json);

    private sealed class Parser
    {
        private readonly string json; private int index; private char Peek => json[index];
        private Parser(string json) { this.json = json; }
        public static object Parse(string json) => new Parser(json).ParseValue();

        private object ParseValue()
        {
            EatWhitespace();
            if (index >= json.Length) return null;
            var c = Peek;
            if (c == '{') return ParseObject();
            if (c == '[') return ParseArray();
            if (c == '"' || c == '\'') return ParseString();
            if (char.IsDigit(c) || c == '-') return ParseNumber();
            var word = NextWord();
            return word switch { "true" => true, "false" => false, "null" => null, _ => word };
        }

        private Dictionary<string, object> ParseObject()
        {
            var dict = new Dictionary<string, object>();
            index++; // {
            while (true)
            {
                EatWhitespace();
                if (index >= json.Length) break;
                if (json[index] == '}') { index++; break; }
                var key = ParseString();
                EatWhitespace(); index++; // :
                var val = ParseValue(); dict[key] = val;
                EatWhitespace();
                if (json[index] == ',') index++;
            }
            return dict;
        }

        private List<object> ParseArray()
        {
            var list = new List<object>(); index++; // [
            while (true)
            {
                EatWhitespace();
                if (index >= json.Length) break;
                if (json[index] == ']') { index++; break; }
                list.Add(ParseValue()); EatWhitespace();
                if (json[index] == ',') index++;
            }
            return list;
        }

        private string ParseString()
        {
            var sb = new StringBuilder(); var quote = json[index++]; // " ou '
            while (index < json.Length)
            {
                var c = json[index++];
                if (c == quote) break;
                if (c == '\\' && index < json.Length)
                {
                    var n = json[index++];
                    sb.Append(n switch { '"' => '"', '\\' => '\\', '/' => '/', 'b' => '\b', 'f' => '\f', 'n' => '\n', 'r' => '\r', 't' => '\t', _ => n });
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private object ParseNumber()
        {
            var start = index; while (index < json.Length && "0123456789+-.eE".IndexOf(json[index]) != -1) index++;
            var s = json.Substring(start, index - start);
            if (s.Contains(".") || s.Contains("e") || s.Contains("E")) { if (double.TryParse(s, out var d)) return d; }
            if (long.TryParse(s, out var l)) return l; return 0;
        }

        private void EatWhitespace() { while (index < json.Length && char.IsWhiteSpace(json[index])) index++; }
        private string NextWord() { var start = index; while (index < json.Length && char.IsLetter(json[index])) index++; return json.Substring(start, index - start); }
    }
}
