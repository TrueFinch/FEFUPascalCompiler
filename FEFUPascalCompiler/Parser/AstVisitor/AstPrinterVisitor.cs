using System;
using System.Collections.Generic;
using System.Text;
using FEFUPascalCompiler.Parser.AstNodes;

namespace FEFUPascalCompiler.Parser.AstVisitor
{
    internal class AstPrintVisitor : IAstVisitor<AstPrinterNode>
    {
        public AstPrinterNode Visit(Ident node)
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

        public AstPrinterNode Visit(BinOperator node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Left.Accept(this));
            printer.AddChild(node.Right.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(AssignStatement node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Left.Accept(this));
            printer.AddChild(node.Right.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(Program node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.MainBlock.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(MainBlock node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var nodeDeclsPart in node.DeclsParts)
            {
                printer.AddChild(nodeDeclsPart.Accept(this));
            }

            printer.AddChild(node.MainCompound.Accept(this));

            return printer;
        }

        public AstPrinterNode Visit(ConstDeclsPart node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var constDecl in node.Decls)
            {
                printer.AddChild(constDecl.Accept(this));
            }

            return printer;
        }

        public AstPrinterNode Visit(TypeDeclsPart node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var typeDecl in node.Decls)
            {
                printer.AddChild(typeDecl.Accept(this));
            }

            return printer;
        }

        public AstPrinterNode Visit(TypeDecl node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Ident.Accept(this));
            printer.AddChild(node.IdentType.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(ConstDecl node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Ident.Accept(this));
            printer.AddChild(node.Expression.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(VarDeclsPart node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var varDecl in node.Decls)
            {
                printer.AddChild(varDecl.Accept(this));
            }

            return printer;
        }

        public AstPrinterNode Visit(SimpleVarDecl node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var ident in node.IdentList)
            {
                printer.AddChild(ident.Accept(this));
            }

            printer.AddChild(node.IdentsType.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(InitVarDecl node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Ident.Accept(this));
            printer.AddChild(node.IdentType.Accept(this));
            printer.AddChild(node.Expression.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(ProcFuncDeclsPart node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var ident in node.Decls)
            {
                printer.AddChild(ident.Accept(this));
            }

            return printer;
        }

        public AstPrinterNode Visit(ProcDecl node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.ProcHeader.Accept(this));
            printer.AddChild(node.Block.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(ProcHeader node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Name.Accept(this));
            foreach (var param in node.ParamList)
            {
                printer.AddChild(param.Accept(this));
            }

            return printer;
        }

        public AstPrinterNode Visit(FuncDecl node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.FuncHeader.Accept(this));
            printer.AddChild(node.Block.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(FuncHeader node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Name.Accept(this));
            foreach (var param in node.ParamList)
            {
                printer.AddChild(param.Accept(this));
            }

            printer.AddChild(node.ReturnType.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(SubroutineBlock node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var declPart in node.DeclParts)
            {
                printer.AddChild(declPart.Accept(this));
            }

            printer.AddChild(node.CompoundStatement.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(Forward node)
        {
            var printer = new AstPrinterNode(node.ToString());
            return printer;
        }

        public AstPrinterNode Visit(UnaryOperator node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Right.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(ArrayAccess node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Array.Accept(this));
            foreach (var expr in node.AccessExpr)
            {
                printer.AddChild(expr.Accept(this));
            }

            return printer;
        }

        public AstPrinterNode Visit(RecordAccess node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.RecordIdent.Accept(this));
            printer.AddChild(node.Field.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(FunctionCall node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.FuncIdent.Accept(this));
            foreach (var param in node.ParamList)
            {
                printer.AddChild(param.Accept(this));
            }

            return printer;
        }

        public AstPrinterNode Visit(FormalParamSection node)
        {
            var printer = new AstPrinterNode(node.ToString());
            if (node.ParamModifier != null)
                printer.AddChild(node.ParamModifier.Accept(this));
            foreach (var param in node.ParamList)
            {
                printer.AddChild(param.Accept(this));
            }

            printer.AddChild(node.ParamType.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(Modifier node)
        {
            var printer = new AstPrinterNode(node.ToString());
            return printer;
        }

        public AstPrinterNode Visit(ConstCharLiteral node)
        {
            var printer = new AstPrinterNode(node.ToString());
            return printer;
        }

        public AstPrinterNode Visit(ConstStringLiteral node)
        {
            var printer = new AstPrinterNode(node.ToString());
            return printer;
        }

        public AstPrinterNode Visit(Nil node)
        {
            var printer = new AstPrinterNode(node.ToString());
            return printer;
        }

        public AstPrinterNode Visit(CompoundStatement node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var stmt in node.Statements)
            {
                printer.AddChild(stmt.Accept(this));
            }

            return printer;
        }

        public AstPrinterNode Visit(EmptyStatement node)
        {
            var printer = new AstPrinterNode(node.ToString());
            return printer;
        }

        public AstPrinterNode Visit(IfStatement node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Expression.Accept(this));
            printer.AddChild(node.ThenStatement.Accept(this));
            if (node.ElseStatement != null)
                printer.AddChild(node.ElseStatement.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(WhileStatement node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Expression.Accept(this));
            printer.AddChild(node.Statement.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(ForStatement node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Iterator.Accept(this));
            printer.AddChild(node.Range.Accept(this));
            printer.AddChild(node.Statement.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(SimpleType node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.TypeIdent.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(ArrayType node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var range in node.IndexRanges)
            {
                printer.AddChild(range.Accept(this));
            }
            printer.AddChild(node.TypeOfArray.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(IndexRange node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.LeftBound.Accept(this));
            printer.AddChild(node.RightBound.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(RecordType node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var field in node.FieldsList)
            {
                printer.AddChild(field.Accept(this));
            }
            return printer;
        }

        public AstPrinterNode Visit(FieldSection node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var ident in node.Idents)
            {
                printer.AddChild(ident.Accept(this));
            }
            printer.AddChild(node.IdentsType.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(PointerType node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.SimpleType.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(ProcSignature node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var param in node.ParamList)
            {
                printer.AddChild(param.Accept(this));
            }
            return printer;
        }

        public AstPrinterNode Visit(FuncSignature node)
        {
            var printer = new AstPrinterNode(node.ToString());
            foreach (var param in node.ParamList)
            {
                printer.AddChild(param.Accept(this));
            }
            printer.AddChild(node.ReturnType.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(ConformantArray node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.ArrayType.Accept(this));
            return printer;
        }

        public AstPrinterNode Visit(ForRange node)
        {
            var printer = new AstPrinterNode(node.ToString());
            printer.AddChild(node.Start.Accept(this));
            printer.AddChild(node.Finish.Accept(this));
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
                canvas[currDepth].Insert(lastIndex, _bgSign.ToString(), needInsert);
            }

            canvas[currDepth].Insert(offset + leftPadding, Header);

            if (isNeedSpace)
                canvas[currDepth].Insert(canvas[currDepth].Length, _bgSpace);

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
                canvas[currDepth].Insert(canvas[currDepth].Length, _bgSign.ToString(),
                    Math.Max(start, finish) - canvas[currDepth].Length + 1);
            }

            canvas[currDepth].Replace(_bgSign, '─', Math.Min(start, finish),
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

        public void AlignBG(in List<StringBuilder> canvas)
        {
            var maxWidth = 0;
            foreach (var strBuilder in canvas)
            {
                maxWidth = maxWidth > strBuilder.Length ? maxWidth : strBuilder.Length;
            }

            foreach (var stringBuilder in canvas)
            {
                stringBuilder.Insert(stringBuilder.Length, _bgSign.ToString(), maxWidth - stringBuilder.Length);
            }
        }
        
        public AstPrinterNode(string header)
        {
            Header = header;
        }

        public int Width => Math.Max(Header.Length, ChildrenWidth());
        public string Header { get; }
        private List<AstPrinterNode> _children = new List<AstPrinterNode>();
        private char _bgSign = ' ';
        private string _bgSpace = " ";
    }
}