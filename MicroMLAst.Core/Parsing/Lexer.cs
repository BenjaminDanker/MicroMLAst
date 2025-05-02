using System.Text.RegularExpressions;

namespace MicroMLAst.Core.Parsing;

/// <summary>
/// Token types for the MicroML language.
/// </summary>
public enum TokenType
{
    // Keywords
    Let, In, End, If, Then, Else,
    // Symbols & operators
    LParen, RParen, Equal, Plus, Minus, Star, Less, Assign,
    Ident, IntLit, BoolLit,
    EOF
}

public record Token(TokenType Type, string Lexeme, int Pos);

/// <summary>
/// Converts source code into a sequence of tokens.
/// </summary>
public sealed class Lexer
{
    // Regex for tokenizing input.
    private static readonly Regex _rx = new
    (
        @"(?<ws>\s+)|"                    + // Whitespace (ignored)
        @"(?<num>\d+)|"                   + // Integer literals
        @"(?<bool>true|false)\b|"         + // Boolean literals
        @"(?<id>[a-zA-Z_]\w*)|"           + // Identifiers and keywords
        @"(?<sym>\=\=|:=|[+\-*<()\=])",     // Symbols and operators
        RegexOptions.Compiled
    );

    /// <summary>
    /// Lex the input string into tokens.
    /// </summary>
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
                // Distinguish keywords from identifiers
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

        // Helper to create a token
        Token Tok(TokenType t, string v) => new(t, v, pos);

        // Helper to map symbol strings to token types
        Token Sym(string v) => v switch
        {
            "(" => Tok(TokenType.LParen, v),
            ")" => Tok(TokenType.RParen, v),
            "+" => Tok(TokenType.Plus, v),
            "-" => Tok(TokenType.Minus, v),
            "*" => Tok(TokenType.Star, v),
            "<" => Tok(TokenType.Less, v),
            "=" => Tok(TokenType.Equal, v),
            ":=" => Tok(TokenType.Assign, v),
            _   => throw new($"Unknown char {v}")
        };
    }
}
