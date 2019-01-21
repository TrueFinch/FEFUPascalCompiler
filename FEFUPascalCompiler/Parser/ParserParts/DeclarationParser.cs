using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FEFUPascalCompiler.Parser.AstNodes;
using FEFUPascalCompiler.Parser.Sematics;
using FEFUPascalCompiler.Tokens;
using Type = System.Type;

namespace FEFUPascalCompiler.Parser.ParserParts
{
    internal partial class PascalParser
    {
        private List<AstNode> ParseDeclsParts()
        {
            List<AstNode> declsParts = new List<AstNode>();
            bool stopParse = false;

            while (!stopParse)
            {
                AstNode declsPart = null;
                switch (PeekToken().Type)
                {
                    case TokenType.Const:
                    {
                        declsPart = ParseConstDeclsPart();
                        break;
                    }
                    case TokenType.Type:
                    {
                        declsPart = ParseTypeDeclsPart();
                        break;
                    }
                    case TokenType.Var:
                    {
                        declsPart = ParseVarDeclsPart();
                        break;
                    }
                    case TokenType.Procedure:
                    {
                        declsPart = ParseProcDecl();
                        break;
                    }
                    case TokenType.Function:
                    {
                        declsPart = ParseFuncDecl();
                        break;
                    }
                    default:
                    {
                        stopParse = true;
                        break;
                    }
                }

                if (declsPart == null)
                {
                    continue;
                }

                declsParts.Add(declsPart);
            }

            return declsParts;
        }

        private AstNode ParseConstDeclsPart()
        {
            var token = PeekAndNext();

            var constDecls = new List<AstNode>();
            var constDecl = ParseConstDecl();
            if (constDecl == null)
            {
                throw new Exception(string.Format("{0}, {1} : Wrong const declaratio",
                    PeekToken().Line, PeekToken().Column));
            }

            constDecls.Add(constDecl);
            do
            {
                constDecl = ParseConstDecl();
                if (constDecl == null) break;
                constDecls.Add(constDecl);
            } while (true);

            return new ConstDeclsPart(token, constDecls);
        }

        private AstNode ParseConstDecl()
        {
            var constIdent = ParseIdent();
            var token = PeekToken();
            if (token.Type != TokenType.EqualOperator)
            {
                //some parser exception
                return null;
            }

            NextToken();
            var expression = ParseExpression();
            var semicolonToken = PeekToken();
            if (semicolonToken == null || semicolonToken.Type != TokenType.Semicolon)
            {
                //some parser exception
                return null;
            }

            NextToken();

            return new ConstDecl(token, constIdent, expression);
        }

        private AstNode ParseTypeDeclsPart()
        {
            var token = PeekAndNext();
            var typeDecls = new List<AstNode> {ParseTypeDecl()};
            if (typeDecls[0] == null)
            {
                throw new Exception(string.Format("{0}, {1} : Empty type block", PeekToken().Line, PeekToken().Column));
            }

            do
            {
                var typeDecl = ParseTypeDecl();
                if (typeDecl == null) break;
                typeDecls.Add(typeDecl);
            } while (true);

            return new TypeDeclsPart(token, typeDecls);
        }

        private AstNode ParseTypeDecl()
        {
            var typeIdent = ParseIdent();

            if (typeIdent == null)
            {
                // this meants that we get to another part of declaration or compound statement and key word was met
                return null;
            }

            CheckDuplicateIdentifier(typeIdent.Token);

            var token = PeekToken();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.EqualOperator},
                string.Format("{0} {1} : syntax error, '=' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var type = ParseType();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            if (type.Item1.Ident.Length != 0 && _symbolTableStack.FindType(type.Item1.Ident) != null)
            {
                _symbolTableStack.AddAlias(typeIdent.ToString(), type.Item1);
            }
            else
            {
                _symbolTableStack.AddType(typeIdent.ToString(), type.Item1);
            }


            return new TypeDecl(typeIdent, type.Item2);
        }

        private AstNode ParseVarDeclsPart(bool local = false)
        {
            var token = PeekAndNext();

            var varDecls = new List<AstNode> {ParseVarDecl(local)};
            if (varDecls[0] == null)
            {
                throw new Exception(string.Format("{0}, {1} : Empty var block", PeekToken().Line, PeekToken().Column));
            }

            do
            {
                var varDecl = ParseVarDecl(local);
                if (varDecl == null) break;
                varDecls.Add(varDecl);
            } while (true);

            return new VarDeclsPart(token, varDecls);
        }

        private AstNode ParseVarDecl(bool local = false)
        {
            if (PeekToken().Type != TokenType.Ident)
                return null;
            var varIdents = ParseIdentList();

            var token = PeekToken();
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
                string.Format("{0} {1} : syntax error, ':' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var type = ParseType();

            foreach (var varIdent in varIdents)
            {
                CheckDuplicateIdentifier(varIdent.Token);
                _symbolTableStack.Peek().AddVariable(local, varIdent.ToString(), type.Item1);
            }

            if (varIdents.Count == 1)
            {
                if (PeekToken().Type == TokenType.EqualOperator)
                {
                    NextToken();
                    var expr = ParseExpression();
                    CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                        string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                            PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

                    return new InitVarDecl(varIdents[0], type.Item2, expr);
                }
            }

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            return new SimpleVarDecl(varIdents, type.Item2);
        }

        private AstNode ParseProcFuncDeclsPart()
        {
            var declarations = new List<AstNode>();
            bool stopParse = false;
            while (!stopParse)
            {
                stopParse = true;
                switch (PeekToken().Type)
                {
                    case TokenType.Procedure:
                    {
                        stopParse = false;
                        var procDecl = ParseProcDecl();
                        declarations.Add(procDecl);
                        break;
                    }
                    case TokenType.Function:
                    {
                        stopParse = false;
                        var funcDecl = ParseFuncDecl();
                        declarations.Add(funcDecl);
                        break;
                    }
                }
            }

            return new ProcFuncDeclsPart(declarations);
        }

        private AstNode ParseFuncDecl()
        {
            FunctionSymbol functionSymbol;
            FuncHeader funcHeader;
            (functionSymbol, funcHeader) = ParseFuncHeader();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            _symbolTableStack.PrepareFunction(functionSymbol.Ident, functionSymbol); // Add function to handle recursion 

            _symbolTableStack.Push(); // this will be local table
            var funcSubroutineBlock = ParseSubroutineBlock();

            functionSymbol.Body = funcSubroutineBlock;

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));
            
            _symbolTableStack.AddFunction(functionSymbol.Ident, functionSymbol);

            //TODO: remove this to typeChecker visitor
//            _symbolTableStack.Push(functionSymbol.Parameters); // push parameters table
//            _symbolTableStack.Push(functionSymbol.Local); // push local table
//
//            // TODO: add type checking of block here
//
//            _symbolTableStack.Pop(); // pop local table
//            _symbolTableStack.Pop(); // pop parameters table

            return new FuncDecl(funcHeader, funcSubroutineBlock);
        }

        private (FunctionSymbol, FuncHeader) ParseFuncHeader()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Function},
                string.Format("{0} {1} : syntax error, 'function' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var funcName = ParseIdent();
            CheckDuplicateIdentifier(funcName.Token);
            var funcSymbol = new FunctionSymbol(funcName.ToString());

            _symbolTableStack.Push(); //this will be Parameters table
            var paramList = ParseFormalParamList();

//            funcSymbol.Parameters = _symbolTableStack.Peek();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Colon},
                string.Format("{0} {1} : syntax error, ':' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var returnType = ParseSimpleType();
            funcSymbol.ReturnSymType = returnType.Item1;

            return (funcSymbol, new FuncHeader(funcName, paramList, returnType.Item2));
        }

        //TODO: add declaration correctness 
        private AstNode ParseProcDecl()
        {
            ProcedureSymbol procedureSymbol;
            ProcHeader procHeader;
            (procedureSymbol, procHeader) = ParseProcHeader();

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            _symbolTableStack.PrepareProcedure(procedureSymbol.Ident, procedureSymbol); // Add function to handle recursion 
            
            _symbolTableStack.Push(); // this will be local table
            var procSubroutineBlock = ParseSubroutineBlock();
            procedureSymbol.Body = procSubroutineBlock;

            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Semicolon},
                string.Format("{0} {1} : syntax error, ';' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            _symbolTableStack.AddProcedure(procedureSymbol.Ident, procedureSymbol);

            //TODO: move to type checker
//            _symbolTableStack.Push(procedureSymbol.Parameters); // push parameters table
//            _symbolTableStack.Push(procedureSymbol.Local); // push local table
//
//            //TODO: add type checking here
//
//            _symbolTableStack.Pop(); // pop local table
//            _symbolTableStack.Pop(); // pop parameters table

            return new ProcDecl(procHeader, procSubroutineBlock);
        }

        private (ProcedureSymbol, ProcHeader) ParseProcHeader()
        {
            CheckToken(PeekToken().Type, new List<TokenType> {TokenType.Procedure},
                string.Format("{0} {1} : syntax error, 'procedure' expected, but {2} found",
                    PeekToken().Line, PeekToken().Column, PeekAndNext().Lexeme));

            var procName = ParseIdent();
            CheckDuplicateIdentifier(procName.Token);
            var procSymbol = new ProcedureSymbol(procName.ToString());

            _symbolTableStack.Push(); //this will be Parameters table
            var paramList = ParseFormalParamList();

//            procSymbol.Parameters = _symbolTableStack.Peek();

            return (procSymbol, new ProcHeader(procName, paramList));
        }
        
        private AstNode ParseSubroutineBlock()
        {
            if (PeekToken().Type == TokenType.Forward)
            {
                return new Forward(PeekAndNext());
            }

            var declsParts = new List<AstNode>();
            bool stopParse = false;
            while (!stopParse)
            {
                stopParse = true;
                switch (PeekToken().Type)
                {
                    case TokenType.Const:
                    {
                        stopParse = false;
                        var constDeclsPart = ParseConstDeclsPart();
                        declsParts.Add(constDeclsPart);
                        break;
                    }
                    case TokenType.Type:
                    {
                        stopParse = false;
                        var typeDeclsPart = ParseTypeDeclsPart();
                        declsParts.Add(typeDeclsPart);
                        break;
                    }
                    case TokenType.Var:
                    {
                        stopParse = false;
                        var varDeclsPart = ParseVarDeclsPart();
                        declsParts.Add(varDeclsPart);
                        break;
                    }
                }
            }

            var compound = ParseCompoundStatement();

            return new SubroutineBlock(declsParts, compound);
        }
    }
}