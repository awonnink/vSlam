using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typespace
{
    public class TypeCoder
    {
        string delim = "@@";
        public string Encode(typecmd cmd)
        {
            return delim + cmd.ToString() + delim;
        }
        char[] splitChar = { ' ' };
        public typecmd Decode(ref string word)
        {
            typecmd ret = typecmd.txt;
            if (word != null && word.StartsWith(delim) && word.EndsWith(delim))
            {
                var c = word.Replace(delim, "");
                typecmd tret = typecmd.none;
                foreach (typecmd t in Enum.GetValues(typeof(typecmd)))
                {
                    if (t.ToString() == c)
                    {
                        tret = t;
                        break;
                    }
                }
                switch (tret)
                {
                    case typecmd.amp:
                        word = "&";
                        ret = typecmd.txt;
                        break;
                    case typecmd.dot:
                        word = ".";
                        ret = typecmd.txt;
                        break;
                    case typecmd.question:
                        word = "?";
                        ret = typecmd.txt;
                        break;
                    case typecmd.backslash:
                        word = @"\";
                        ret = typecmd.txt;
                        break;
                    case typecmd.slash:
                        word = "/";
                        ret = typecmd.txt;
                        break;
                    case typecmd.colon:
                        word = ":";
                        ret = typecmd.txt;
                        break;
                    case typecmd.semicolon:
                        word = ";";
                        ret = typecmd.txt;
                        break;
                    case typecmd.plus:
                        word = "+";
                        ret = typecmd.txt;
                        break;
                    case typecmd.space:
                        word = " ";
                        ret = typecmd.txt;
                        break;
                    case typecmd.hack:
                        word = "#";
                        ret = typecmd.txt;
                        break;
                    case typecmd.lt:
                        word = "<";
                        ret = typecmd.txt;
                        break;
                    case typecmd.gt:
                        word = ">";
                        ret = typecmd.txt;
                        break;
                    default:
                        ret = tret;
                        break;
                }

            }
            if (word.Length == 0)
            {
                ret = typecmd.none;
            }
            return ret;
        }

        WordStroke CreateWordStroke(string text, int cursor)
        {
            Typer.sequence++;
            WordStroke w = new WordStroke(text, cursor, Typer.sequence);
            
            return w;
        }
        public List<WordStroke> Encode(string text, int cursor)
        {
            List<WordStroke> res = new List<WordStroke>();
            if (string.IsNullOrEmpty(text))
            {
                res.Add(CreateWordStroke("", cursor));
                return res;
            }
            List<string> words = text.Split(splitChar).ToList();
            bool started = false;
            foreach (var word in words)
            {
                StringBuilder sb = new StringBuilder();
                if (started)
                {
                   
                    res.Add(CreateWordStroke(Encode(typecmd.space), cursor));
                    cursor++;
                }
                foreach (var k in word)
                {
                    var key = k.ToString();
                    typecmd cmd = typecmd.none;
                    switch (key)
                    {
                        case @"/":
                            cmd = typecmd.slash;
                            break;
                        case @".":
                            cmd = typecmd.dot;
                            break;
                        case @"?":
                            cmd = typecmd.question;
                            break;
                        case @"\":
                            cmd = typecmd.backslash;
                            break;
                        case @":":
                            cmd = typecmd.colon;
                            break;
                        case @";":
                            cmd = typecmd.semicolon;
                            break;
                        case @"+":
                            cmd = typecmd.plus;
                            break;
                        case @"&":
                            cmd = typecmd.amp;
                            break;
                        case @"|":
                            cmd = typecmd.pipe;
                            break;
                        case @"<":
                            cmd = typecmd.lt;
                            break;
                        case @">":
                            cmd = typecmd.gt;
                            break;
                    }
                    if (cmd != typecmd.none)
                    {
                        if (sb.Length > 0)
                        {
                            res.Add(CreateWordStroke(sb.ToString(), cursor));
                            cursor += sb.Length;
                            sb = new StringBuilder();
                        }
                        res.Add(CreateWordStroke(Encode(cmd), cursor));
                        cursor++;
                    }
                    else
                    {
                        sb.Append(key);
                    }
                }
                if (sb.Length > 0)
                {
                    res.Add(CreateWordStroke(sb.ToString(), cursor));
                    cursor += sb.Length;
                }
                started = true;
            }
            return res;
        }

    }
    public enum typecmd
    {
        none,
        backspace,
        clear,
        enter,
        dot,
        slash,
        backslash,
        plus,
        amp,
        colon,
        semicolon,
        space,
        pipe,
        txt,
        hack,
        question,
        gt,
        lt,
        stop
    }
}
