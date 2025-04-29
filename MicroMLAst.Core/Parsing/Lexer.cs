using System.Text.RegularExpressions;

namespace MicroMLAst.Core.Parsing;

public enum TokenType
{
    // keywords
    Let, In, End, If, Then, Else,
    // symbols & operators
    LParen, RParen, Equal, Plus, Minus, Star, Less, Assign,            // = + - * <
    Ident, IntLit, BoolLit,
    EOF
}

public record Token(TokenType Type, string Lexeme, int Pos);

public sealed class Lexer
{
    private static readonly Regex _rx = new
    (
        @"(?<ws>\s+)|"                    + // ignore whitespace
        @"(?<num>\d+)|"                   +
        @"(?<bool>true|false)\b|"         +
        @"(?<id>[a-zA-Z_]\w*)|"           +
        @"(?<sym>\=\=|:=|[+\-*<()\=])",
        RegexOptions.Compiled
    );

    public IEnumerable<Token> Lex(string src)
    {
        var pos = 0;
        foreach (Match m in _rx.Matches(src))
        {
            if (m.Groups["ws"].Success)   { pos += m.Length; continue; }
            if (m.Groups["num"].Success)  { yield return Tok(TokenType.IntLit,  m.Value); }
            else if (m.Groups["bool"].Success)
                                          { yield return Tok(TokenType.BoolLit, m.Value); }
            else if (m.Groups["id"].Success)
            {
                yield return Tok(m.Value switch
                {
                    "let"  => TokenType.Let,
                    "in"   => TokenType.In,
                    "end"  => TokenType.End,
                    "if"   => TokenType.If,
                    "then" => TokenType.Then,
                    "else" => TokenType.Else,
                    _      => TokenType.Ident
                }, m.Value);
            }
            else                           { yield return Sym(m.Value); }
            pos += m.Length;
        }
        yield return new Token(TokenType.EOF, "<eof>", pos);

        Token Tok(TokenType t,string v) => new(t,v,pos);
        Token Sym(string v) => v switch
        {
            "(" => Tok(TokenType.LParen,v),
            ")" => Tok(TokenType.RParen,v),
            "+" => Tok(TokenType.Plus ,v),
            "-" => Tok(TokenType.Minus,v),
            "*" => Tok(TokenType.Star ,v),
            "<" => Tok(TokenType.Less ,v),
            "=" => Tok(TokenType.Equal,v),
            ":="=> Tok(TokenType.Assign,v),
            _   => throw new($"Unknown char {v}")
        };
    }
}
