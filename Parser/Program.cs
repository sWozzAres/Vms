﻿using System.Buffers.Text;
using System.ComponentModel.Design;
using System.Net.Http.Headers;

namespace Parser
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            

            T();
            return;


            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/114.0");

            //var html = await client.GetStringAsync(@"https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent?utm_source=mozilla&utm_medium=devtools-netmonitor&utm_campaign=default");
            //Parse(html);

            Parse("<tag/>");
            Parse("<tag>123</tag>");
            Parse("<tag><div class=\"className\">123</div></tag>");

            //Console.WriteLine(html);
        }

        static void T()
        {
            //string originalString = "Mark";
            //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(originalString);
            //string base64String = Convert.ToBase64String(bytes);

            //Console.WriteLine(base64String);

            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String("TWFyaw==")));

            var t = new Infix2PostfixTranslator("9+");
            t.Parse();
        }
        static void Parse(string html)
        {
            ReadOnlySpan<char> text = html.AsSpan();

            bool inTag = false;
            bool closingTag = false;
            string tagName = string.Empty;
            string content = string.Empty;

            foreach (char c in text)
            {
                if (c == '<')
                {
                    inTag = true;
                }

                else if (inTag)
                {
                    if (c == '>')
                    {
                        inTag = false;

                        GotTag(tagName, closingTag, content);

                        tagName = string.Empty;
                        content = string.Empty;

                        closingTag = false;
                    }
                    else if (c == '/')
                    {
                        closingTag = true;
                    }
                    else
                    {
                        tagName += c;
                    }
                }
                else
                {
                    content += c;
                }
            }

            void GotTag(string tagName, bool closing, string content)
            {
                Console.WriteLine(tagName + " " + closing + " " + content);
            }
        }
    }
}