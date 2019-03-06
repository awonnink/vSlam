using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Typespace
{
    public class Typer
    {
        static string typingId = "browser";
        public static int sequence = 0;
        public static int lasthandledsequence = 0;
        string PresentationGuid;
        static TypeCoder _typeCoder = null;
        static TypeCoder typeCoder
        {
            get
            {
                if(_typeCoder==null)
                {
                    _typeCoder = new TypeCoder();
                }
                return _typeCoder;
            }
        }
        public static List<WordStroke> WordsToSend = new List<WordStroke>();
        public static List<WordStroke> WordsRecieved = new List<WordStroke>();

        public Typer(string presentationGuid)
        {
            PresentationGuid = presentationGuid;
        }
        private string GetServiceUrlForTyping(string serviceUrl, string sessionGuid, string id, string word, int cursor, int sequence)
        {
            ;
            return string.Format("{0}/Typing/{1}/{2}/{3}/{4}/{5}/{6}", serviceUrl, sessionGuid, id, word, cursor, sequence, lasthandledsequence);
        }
        public static void AddCommandToSend(typecmd command, int cursor=0)
        {
            if(command== typecmd.clear)
            {
                WordsToSend.Clear();
            }
            var tcmd = typeCoder.Encode(command);
            sequence++;
            WordsToSend.Add(new WordStroke(tcmd, cursor, sequence));
        }
        public static void AddWordTosend(string text, int cursor)
        {
            var words = typeCoder.Encode(text, cursor);
            foreach (var word in words)
            {
                WordsToSend.Add(word);
            }
        }
        string GetNextWordToSendUrl(string serviceUrl)
        {
            string word = typeCoder.Encode(typecmd.none);
            int cursor = 0;
            int lsequence = 0;
            var wordStroke = WordsToSend.FirstOrDefault();
            if (wordStroke != null)
            {
                WordsToSend.Remove(wordStroke);
                word = wordStroke.Word;
                cursor = wordStroke.Cursor;
                lsequence = wordStroke.Sequence;
            }
            return GetServiceUrlForTyping(serviceUrl, PresentationGuid, typingId, word, cursor, lsequence);
        }
        public IEnumerator Typing(string serviceUrl)
        {
            string url = GetNextWordToSendUrl(serviceUrl);
            string t = null;
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            var handler = (DownloadHandler)www.downloadHandler;
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                t = handler.text;
            }
            HandleTextRecieved(t);
        }

        private static void HandleTextRecieved(string t)
        {
            if (!string.IsNullOrEmpty(t))
            {
                if (t.StartsWith("\"") && t.EndsWith("\"") && t.Length > 4)
                {
                    t = t.Substring(1, t.Length - 2);
                }
                var x = t.Split(Convert.ToChar("|"));
                if (x.Length == 3)
                {
                    int c = 0;
                    int s = 0;
                    string chr = x[0];
                    if (int.TryParse(x[1], out c) && int.TryParse(x[2], out s) && s>lasthandledsequence)
                    {
                        WordsRecieved.Add(new WordStroke(chr, c, s));
                    }
                }
            }
        }
        public static typecmd GetNextRecievedWord(out string word, out int cursor, out int sequence)
        {
            cursor = 0;
            sequence = 0;
            word = null;
            typecmd ret= typecmd.none;
            if (WordsRecieved.Count > 0)
            {
                var k = Typespace.Typer.WordsRecieved[0];
                Typespace.Typer.WordsRecieved.Remove(k);
                string tword = k.Word;
                ret=typeCoder.Decode(ref tword);
                word = tword;
                cursor = k.Cursor;
                sequence = k.Sequence;
            }
            return ret;
        }
    }
    public class WordStroke
    {
        public WordStroke()
        { }
        public WordStroke(string word, int cursor, int sequence)
        {
            Word = word;
            Cursor = cursor;
            Sequence = sequence;
        }
        public string Word { get; set; }
        public int Cursor { get; set; }
        public int Sequence { get; set; }
    }

}
