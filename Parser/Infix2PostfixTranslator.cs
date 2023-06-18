using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser;

public ref struct Infix2PostfixTranslator
{
    static char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    static bool isdigit(char c) => digits.Contains(c);

    ReadOnlySpan<char> text;
    int index;
    char lookahead;

    public Infix2PostfixTranslator(string source)
    {
        text = source.AsSpan();
    }

    int state;

    public void Parse()
    {
        index = 0;
        state = 0;
        char c;
        string str = string.Empty;

        switch (state)
        {
            case 0:
                c = getchar();

                if (c == '<')
                {

                    state = 1;
                }

                break;
            case 1:
                str = string.Empty;
                
                c = getchar();

                break;
        }
    }

    char getchar()
    {
        try
        {
            return text[index++];
        }
        catch (IndexOutOfRangeException)
        {
            return '\uFFFF'; // EOF
        }
    }
    void ungetc() { if (index > 0) index--; }

    void putchar(char c) => Console.Write(c);
}
