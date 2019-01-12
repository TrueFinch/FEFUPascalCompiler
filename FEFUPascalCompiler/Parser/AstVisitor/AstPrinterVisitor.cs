using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEFUPascalCompiler.Parser.AstVisitor
{
    internal class AstPrintVisitor : IAstVisitor<AstPrinterNode>
    {
        public AstPrinterNode Visit(Identifier node)
        {
            return new AstPrinterNode(node.ToString());
        }

        public AstPrinterNode Visit(ConstIntegerLiteral node)
        {
            return new AstPrinterNode(node.ToString());
        }

        public AstPrinterNode Visit(ConstDoubleLiteral node)
        {
            return new AstPrinterNode(node.ToString());
        }

        public AstPrinterNode Visit(BinOperation node)
        {
            var printer = new AstPrinterNode(node.Oper.Value);
            printer.AddChild(node.Left.Accept(this));
            printer.AddChild(node.Right.Accept(this));
            return printer;
        }
    }

    public class AstPrinterNode
    {
        public void PrintTree(in List<StringBuilder> canvas, int currDepth = 0, int offset = 0,
            bool isNeedSpace = false)
        {
            if (canvas.Count - 1 < currDepth)
                canvas.Add(new StringBuilder());

            var leftPadding = (Width - Header.Length) / 2 + 1;

            if (canvas[currDepth].Length < offset + leftPadding)
            {
                var lastIndex = canvas[currDepth].Length;
                var needInsert = offset + leftPadding - canvas[currDepth].Length;
                canvas[currDepth].Insert(lastIndex, ".", needInsert);
            }

            canvas[currDepth].Insert(offset + leftPadding, Header);

            if (isNeedSpace)
                canvas[currDepth].Insert(canvas[currDepth].Length, "..");

            var start = (Width + 1) / 2 + offset;
            for (var i = 0; i < _children.Count; ++i)
            {
                var isLast = i == _children.Count - 1;
                var finish = offset + (_children[i].Width + 1) / 2;

                PrintEdge(in canvas, currDepth + 1, start, finish, i == 0, i == _children.Count - 1);

                _children[i].PrintTree(canvas, currDepth + 2, offset, isLast);
                offset += _children[i].Width + (!isLast ? 2 : 0);
            }
        }

        private void PrintEdge(in List<StringBuilder> canvas, int currDepth, int start, int finish, bool isFarLeft,
            bool isFarRight)
        {
            if (canvas.Count - 1 < currDepth)
            {
                canvas.Add(new StringBuilder());
            }

            if (Math.Max(start, finish) > canvas[currDepth].Length - 1)
            {
                canvas[currDepth].Insert(canvas[currDepth].Length, ".",
                    Math.Max(start, finish) - canvas[currDepth].Length + 1);
            }

            canvas[currDepth].Replace('.', '─', Math.Min(start, finish),
                Math.Max(start, finish) - Math.Min(start, finish));

            canvas[currDepth][start] = '┴';
            if (finish > start)
            {
                canvas[currDepth][finish] = isFarRight ? '┐' : '┬';
            }
            else if (start > finish)
            {
                canvas[currDepth][finish] = isFarLeft ? '┌' : '┬';
            }
            else
            {
                canvas[currDepth][start] = '┼';
            }
        }

        public void AddChild(AstPrinterNode node)
        {
            _children.Add(node);
        }

        public int ChildrenWidth()
        {
            //reserve space for separations between nodes
            int width = _children.Count > 1 ? (_children.Count - 1) * 2 : 0;
            foreach (var child in _children)
            {
                width += child.Width;
            }

            return width;
        }

        public AstPrinterNode(string header)
        {
            Header = header;
        }

        public int Width => Math.Max(Header.Length, ChildrenWidth());
        public string Header { get; }
        private List<AstPrinterNode> _children = new List<AstPrinterNode>();
    }
}