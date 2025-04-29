using MicroMLAst.Core.Ast;
using TokenType = MicroMLAst.Core.Parsing.TokenType;

namespace MicroMLAst.Core.Parsing;

public interface IParser { Expr Parse(string code); }

public sealed class Parser : IParser
{
    private IEnumerator<Token>? _it;
    private Token _cur = null!;

    public Expr Parse(string code)
    {
        _it = new Lexer().Lex(code).GetEnumerator();
        Next();
        var e = Expr();
        // just verify we’re at EOF, do *not* call Next()
        if (_cur.Type != TokenType.EOF)
            throw new Exception($"Expected EOF but found {_cur.Type}");
        return e;
    }


    // ───────────────── helpers ───────────────────────
    private void Next() => _cur = _it!.MoveNext() ? _it.Current : throw new("lex error");
    private bool Eat(TokenType t) { if (_cur.Type==t){Next();return true;} return false; }
    private void Expect(TokenType t)
    {
        if (_cur.Type != t)
            throw new Exception($"Expected {t} but found {_cur.Type}");
        // only advance on *non*-EOF
        if (t != TokenType.EOF)
            Next();
    }


    // ───────────────── grammar ────────────────────────
    // expr ::= letExpr | ifExpr | funCall
    private Expr Expr()
    {
        if (Eat(TokenType.Let))     return LetExpr();
        if (Eat(TokenType.If))      return IfExpr();
        return CallExpr();
    }

    private Expr LetExpr()
    {
        // we have just consumed 'let'
        // 1) read the name (either a variable or a function name)
        var id = _cur.Lexeme;
        Expect(TokenType.Ident);

        // 2) if the next token is another Ident, it's "let f x = ..."
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

        // 3) otherwise it's the simple let: "let x = ..."
        Expect(TokenType.Equal);
        var rhs  = Expr();
        Expect(TokenType.In);
        var body = Expr();
        Expect(TokenType.End);

        return new Let(id, rhs, body);
    }

    private Expr IfExpr()
    {
        var cond = Expr();
        Expect(TokenType.Then);
        var thn  = Expr();
        Expect(TokenType.Else);
        var els  = Expr();
        return new If(cond,thn,els);
    }

    // callExpr := addExpr { addExpr }   -- left-associative f a b
    private Expr CallExpr()
    {
        var e = AddExpr();
        while (PeekIsStartOfAtom()) e = new Call(e,AddExpr());
        return e;
    }

    // add/sub precedence
    private Expr AddExpr()
    {
        var e = MulExpr();
        while (_cur.Type is TokenType.Plus or TokenType.Minus or TokenType.Less or TokenType.Equal)
        {
            var op = _cur.Lexeme; Next();
            var r  = MulExpr();
            e = new Prim(op,e,r);
        }
        return e;
    }
    private Expr MulExpr()
    {
        var e = Atom();
        while (_cur.Type==TokenType.Star)
        {
            Next();
            e = new Prim("*",e,Atom());
        }
        return e;
    }

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

        Expr Int()  { var v=int.Parse(_cur.Lexeme); Next(); return new CstI(v); }
        Expr Bool() { var v=_cur.Lexeme=="true";   Next(); return new CstB(v); }
        Expr IdOrParCall()
        {
            var name=_cur.Lexeme; Next();
            return new Var(name);
        }
        Expr Paren()
        {
            Next();
            var e=Expr();
            Expect(TokenType.RParen);
            return e;
        }
    }

    private bool PeekIsStartOfAtom() =>
        _cur.Type is TokenType.Ident or TokenType.IntLit or TokenType.BoolLit or TokenType.LParen;
}
