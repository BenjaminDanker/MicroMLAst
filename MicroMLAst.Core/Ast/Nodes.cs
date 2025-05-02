namespace MicroMLAst.Core.Ast;

// ── Abstract base for all AST nodes ────────────────────────────────────
public abstract record Expr;

// ── Leaf nodes ─────────────────────────────────────────────────────────
// Integer constant, e.g., 42
public sealed record CstI(int Value) : Expr;
// Boolean constant, e.g., true or false
public sealed record CstB(bool Value) : Expr;
// Variable reference, e.g., x
public sealed record Var(string Name) : Expr;

// ── Composite nodes ────────────────────────────────────────────────────
// Primitive operation, e.g., e1 + e2, e1 < e2
public sealed record Prim(string Op, Expr L, Expr R) : Expr;
// Conditional expression: if cond then ... else ...
public sealed record If(Expr Cond, Expr Then, Expr Else) : Expr;
// Let binding: let x = e in body end
public sealed record Let(string Id, Expr Rhs, Expr Body) : Expr;
// Function definition: let f x = ... in ...
public sealed record LetFun(string F, string X, Expr FBody, Expr LetBody) : Expr;
// Function call: f(e)
public sealed record Call(Expr Fun, Expr Arg) : Expr;
