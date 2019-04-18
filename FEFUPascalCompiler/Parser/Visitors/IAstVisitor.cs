using FEFUPascalCompiler.Parser.AstNodes;

namespace FEFUPascalCompiler.Parser.Visitors
{
    public interface IAstVisitor<T>
    {
        T Visit(Ident node);
        T Visit(ConstIntegerLiteral node);

        T Visit(ConstFloatLiteral node);

//        T Visit(BinOperator node);
        T Visit(AssignStatement node);
        T Visit(Program node);
        T Visit(MainBlock node);
        T Visit(ConstDeclsPart node);
        T Visit(TypeDeclsPart node);
        T Visit(TypeDecl node);
        T Visit(ConstDecl node);
        T Visit(VarDeclsPart node);

        T Visit(SimpleVarDecl node);

//        T Visit(InitVarDecl node);
//        T Visit(ProcFuncDeclsPart node);
//        T Visit(ProcDecl node);
//        T Visit(ProcHeader node);
//        T Visit(FuncDecl node);
//        T Visit(FuncHeader node);
        T Visit(SubroutineBlock node);

//        T Visit(Forward node);
        T Visit(UnaryOperator node);
        T Visit(ArrayAccess node);
        T Visit(RecordAccess node);
        T Visit(UserFunctionCall node);
        T Visit(WriteFunctionCall node);
        T Visit(FormalParamSection node);
        T Visit(ConstCharLiteral node);
        T Visit(ConstStringLiteral node);
        T Visit(Nil node);
        T Visit(CompoundStatement node);
        T Visit(EmptyStatement node);
        T Visit(IfStatement node);
        T Visit(WhileStatement node);
        T Visit(ForStatement node);
        T Visit(SimpleTypeNode node);
        T Visit(ArrayTypeNode node);
        T Visit(IndexRangeNode node);
        T Visit(RecordTypeNode node);
        T Visit(FieldSectionNode node);

        T Visit(PointerTypeNode node);

//        T Visit(ProcSignature node);
//        T Visit(FuncSignature node);
        T Visit(ConformantArray node);
        T Visit(ForRange node);
        T Visit(DereferenceOperator node);
        T Visit(Cast node);
        T Visit(ConstBooleanLiteral node);
        T Visit(ComparingOperator node);
        T Visit(AdditiveOperator node);
        T Visit(MultiplyingOperator node);
        T Visit(CallableDeclNode node);
        T Visit(CallableHeader node);

        T Visit(CallableCallStatement node);
//        T Visit(FunctionCall node);
    }
}