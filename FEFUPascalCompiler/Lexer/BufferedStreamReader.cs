using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FEFUPascalCompiler.Lexer
{
    public class BufferedStreamReader
    {
        public BufferedStreamReader(in StreamReader sr)
        {
            _sr = sr;
        }

        public char Peek()
        {
            return (char) (_buffer.Any() ? _buffer.Peek() : _sr.Peek());
        }

        public char Read()
        {
            return (char) (_buffer.Any() ? _buffer.Pop() : _sr.Read());
        }

        public void Kick(char ch)
        {
            _buffer.Push(ch);
        }

        public bool EndOfStream()
        {
            return _buffer.Count == 0 && _sr.EndOfStream;
        }

        public string ReadLine()
        {
            return _sr.ReadLine();
        }
        
        private StreamReader _sr;
        private Stack<char> _buffer = new Stack<char>();
    }
}