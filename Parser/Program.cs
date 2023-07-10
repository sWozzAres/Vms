namespace Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var p = new Parser("<html></html>");
            p.Parse();
        }

        
        //static void Parse(string text)
        //{
        //    var span = new ReadOnlySpan<char>(text.ToArray());

        //    bool intag = false;
        //    string tagName = "";
        //    bool isEndTag = false;
        //    bool inString = false;
        //    string str = "";

        //    //char nextChar() => span[1];

        //    for (int i = 0; i < span.Length; i++)
        //    {
        //        var c = span[i];

        //        if (intag)
        //        {
        //            if (inString)
        //            {
        //                if (c == '"')
        //                {
        //                    inString = false;
        //                }
        //                else
        //                {
        //                    str += c;
        //                }
        //            }
        //            else
        //            {
        //                if (c == '"')
        //                {
        //                    inString = true;
        //                }

        //                else if (c == '>')
        //                {
        //                    intag = false;
        //                }

        //                else if (c == '/')
        //                {
        //                    isEndTag = true;
        //                }

        //                else
        //                {
        //                    tagName += c;
        //                }
        //            }
        //        }

        //        else if (c == '<')
        //        {
        //            intag = true;
        //        };
        //    }

        //    bool IsWhitespace(char c) => c == ' ' || c == '\t';

            

        //}
    }

    public ref struct Parser(string text)
    {
        const char EOF = '\u001A';

        int index = 0;
        ReadOnlySpan<char> _text = new ReadOnlySpan<char>(text.ToArray());

        public void Parse()
        {
            var c = getChar();
            if (c == '<')
            {
                Tag();
            }
        }

        public void Tag()
        {
            var inTag = true;
            while (inTag)
            {
                var c = getChar();

                if (IsWhitespace(c)) 
                { 
                    // do nothing
                }
                else if (c == '>')
                {
                    inTag = false;
                }
                else
                {
                    TagName();
                }
            }

            void TagName()
            {

            }
        }

        public char getChar() => index >= _text.Length ? EOF : _text[index++];
        public bool IsWhitespace(char ch) => ch == ' ' || ch == '\t';
    }
}

