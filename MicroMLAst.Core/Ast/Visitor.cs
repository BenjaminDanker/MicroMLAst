using System.Text;

namespace MicroMLAst.Core.Ast;

/// <summary>
/// Traverses an AST and generates a Mermaid.js graph representation.
/// </summary>
public sealed class MermaidVisitor
{
    private readonly StringBuilder _sb = new();
    private int _id;

    /// <summary>
    /// Render the given expression as a Mermaid.js graph.
    /// </summary>
    public string Render(Expr e)
    {
        _sb.AppendLine("graph TD");
        Visit(e, "root");
        return _sb.ToString();
    }

    // Generate a new unique node ID for the graph.
    private string NewId() => $"N{_id++}";

    // Emit an edge in the Mermaid graph, optionally with a label.
    private void EmitEdge(string from, string to, string label = "") =>
        _sb.AppendLine($"  {from} -->{(label == "" ? "" : $"|{label}|")}{to}");

    // Recursively visit AST nodes and emit graph nodes/edges.
    private string Visit(Expr e, string lbl)
    {
        var id = NewId();
        switch (e)
        {
            case CstI c:
                _sb.AppendLine($"  {id}([\"{c.Value}\"])");
                break;
            case CstB b:
                _sb.AppendLine($"  {id}([\"{b.Value}\"])");
                break;
            case Var v:
                _sb.AppendLine($"  {id}(\"{v.Name}\")");
                break;
            case Prim p:
                _sb.AppendLine($"  {id}[\"{p.Op}\"]");
                EmitEdge(id, Visit(p.L, "L"));
                EmitEdge(id, Visit(p.R, "R"));
                break;
            case If iff:
                _sb.AppendLine($"  {id}(\"if\")");
                EmitEdge(id, Visit(iff.Cond, "cond"), "cond");
                EmitEdge(id, Visit(iff.Then, "then"), "then");
                EmitEdge(id, Visit(iff.Else, "else"), "else");
                break;
            case Let l:
                _sb.AppendLine($"  {id}(\"let {l.Id}\")");
                EmitEdge(id, Visit(l.Rhs, "rhs"), "rhs");
                EmitEdge(id, Visit(l.Body, "body"), "body");
                break;
            case LetFun lf:
                _sb.AppendLine($"  {id}(\"letfun {lf.F}\")");
                EmitEdge(id, Visit(lf.FBody, "fbody"), "fBody");
                EmitEdge(id, Visit(lf.LetBody, "letBody"), "letBody");
                break;
            case Call c:
                _sb.AppendLine($"  {id}(\"call\")");
                EmitEdge(id, Visit(c.Fun, "fun"), "fun");
                EmitEdge(id, Visit(c.Arg, "arg"), "arg");
                break;
        }
        return id;
    }
}
