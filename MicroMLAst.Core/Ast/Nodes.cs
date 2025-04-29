namespace MicroMLAst.Core.Ast;

// ── base ───────────────────────────────────────────────────────────────
public abstract record Expr;

// ── leaf nodes ─────────────────────────────────────────────────────────
public sealed record CstI(int Value)                           : Expr;    // 42
public sealed record CstB(bool Value)                          : Expr;    // true / false
public sealed record Var (string Name)                         : Expr;    // x

// ── composite nodes ────────────────────────────────────────────────────
public sealed record Prim  (string Op, Expr L, Expr R)                     : Expr; // e1 + e2, e1 < e2, …
public sealed record If    (Expr Cond, Expr Then, Expr Else)               : Expr; // if cond then … else …
public sealed record Let   (string Id, Expr Rhs, Expr Body)                : Expr; // let x = e in body end
public sealed record LetFun(string F, string X, Expr FBody, Expr LetBody)  : Expr; // let f x = … in …
public sealed record Call  (Expr Fun, Expr Arg)                            : Expr; // f(e)
