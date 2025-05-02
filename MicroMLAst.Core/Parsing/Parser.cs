using MicroMLAst.Core.Ast;
using TokenType = MicroMLAst.Core.Parsing.TokenType;

namespace MicroMLAst.Core.Parsing;

/// <summary>
/// Interface for parsing source code into an AST.
/// </summary>
public interface IParser { Expr Parse(string code); }

/// <summary>
/// Recursive descent parser for MicroML.
/// </summary>
public sealed class Parser : IParser
{
    private IEnumerator<Token>? _it;
    private Token _cur = null!;

    /// <summary>
    /// Parse the given code string into an AST.
    /// </summary>
    public Expr Parse(string code)
    {
        _it = new Lexer().Lex(code).GetEnumerator();
        Next();
        var e = Expr();
        // Ensure all input is consumed
        if (_cur.Type != TokenType.EOF)
            throw new Exception($"Expected EOF but found {_cur.Type}");
        return e;
    }

    // ──────────────── Helpers ──────────────────────

    // Advance to the next token
    private void Next() => _cur = _it!.MoveNext() ? _it.Current : throw new("lex error");

    // Consume a token if it matches the expected type
    private bool Eat(TokenType t) { if (_cur.Type == t) { Next(); return true; } return false; }

    // Expect a specific token type, throw if not found
    private void Expect(TokenType t)
    {
        if (_cur.Type != t)
            throw new Exception($"Expected {t} but found {_cur.Type}");
        if (t != TokenType.EOF)
            Next();
    }

    // ──────────────── Grammar ──────────────────────

    // expr ::= letExpr | ifExpr | callExpr
    private Expr Expr()
    {
        if (Eat(TokenType.Let))     return LetExpr();
        if (Eat(TokenType.If))      return IfExpr();
        return CallExpr();
    }

    // letExpr ::= let id = expr in expr end
    //           | let f x = expr in expr end
    private Expr LetExpr()
    {
        var id = _cur.Lexeme;
        Expect(TokenType.Ident);

        // Function definition: let f x = ...
        if (_cur.Type == TokenType.Ident)
        {
            var param = _cur.Lexeme;
            Expect(TokenType.Ident);

            Expect(TokenType.Equal);
            var fBody   = Expr();
            Expect(TokenType.In);
            var letBody = Expr();
            Expect(TokenType.End);

            return new LetFun(id, param, fBody, letBody);
        }

        // Simple let binding: let x = ...
        Expect(TokenType.Equal);
        var rhs  = Expr();
        Expect(TokenType.In);
        var body = Expr();
        Expect(TokenType.End);

        return new Let(id, rhs, body);
    }

    // ifExpr ::= if expr then expr else expr
    private Expr IfExpr()
    {
        var cond = Expr();
        Expect(TokenType.Then);
        var thn  = Expr();
        Expect(TokenType.Else);
        var els  = Expr();
        return new If(cond, thn, els);
    }

    // callExpr ::= addExpr { addExpr }
    // Left-associative function application: f a b
    private Expr CallExpr()
    {
        var e = AddExpr();
        while (PeekIsStartOfAtom()) e = new Call(e, AddExpr());
        return e;
    }

    // addExpr ::= mulExpr { (+ | - | < | =) mulExpr }
    private Expr AddExpr()
    {
        var e = MulExpr();
        while (_cur.Type is TokenType.Plus or TokenType.Minus or TokenType.Less or TokenType.Equal)
        {
            var op = _cur.Lexeme; Next();
            var r  = MulExpr();
            e = new Prim(op, e, r);
        }
        return e;
    }

    // mulExpr ::= atom { * atom }
    private Expr MulExpr()
    {
        var e = Atom();
        while (_cur.Type == TokenType.Star)
        {
            Next();
            e = new Prim("*", e, Atom());
        }
        return e;
    }

    // atom ::= IntLit | BoolLit | Ident | ( expr )
    private Expr Atom()
    {
        return _cur.Type switch
        {
            TokenType.IntLit  => Int(),
            TokenType.BoolLit => Bool(),
            TokenType.Ident   => IdOrParCall(),
            TokenType.LParen  => Paren(),
            _       => throw new($"Bad token: {_cur}")
        };

        Expr Int()  { var v = int.Parse(_cur.Lexeme); Next(); return new CstI(v); }
        Expr Bool() { var v = _cur.Lexeme == "true";   Next(); return new CstB(v); }
        Expr IdOrParCall()
        {
            var name = _cur.Lexeme; Next();
            return new Var(name);
        }
        Expr Paren()
        {
            Next();
            var e = Expr();
            Expect(TokenType.RParen);
            return e;
        }
    }

    // Check if the current token can start an atom
    private bool PeekIsStartOfAtom() =>
        _cur.Type is TokenType.Ident or TokenType.IntLit or TokenType.BoolLit or TokenType.LParen;
}
