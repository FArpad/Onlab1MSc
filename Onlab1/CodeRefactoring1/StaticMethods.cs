using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;

namespace CodeRefactoring1
{
    public class StaticMethods
    {
        public static SyntaxNode ReplaceNodes(SyntaxNode root, SyntaxToken _from, string to)
        {

            SyntaxNode block = FindDeclarationBlock(_from);
            //if (_from.Parent is IdentifierNameSyntax)
            //    block = FindDeclarationBlock(_from);
            //else block = root;

            var vars = from r in block.DescendantTokens().OfType<SyntaxToken>()
                       where (r.ToString() == _from.ValueText)
                       select r;

            Func<SyntaxToken, SyntaxToken, SyntaxToken> foo = (i1, i2) =>
                    Syntax.Identifier(i1.LeadingTrivia, to, i1.TrailingTrivia);
            SyntaxNode newRoot = root.ReplaceTokens(vars, foo);

            return newRoot;
        }

        private static SyntaxNode FindBlock(SyntaxNode node)
        {
            var block = node;
            while (block.Parent != null && !(block is BlockSyntax))
                block = block.Parent;

            return block;
        }

        private static SyntaxNode FindDeclarationBlock(SyntaxToken token)
        {
            SyntaxNode block = FindBlock(token.Parent);
            if (token.Parent is VariableDeclaratorSyntax) return block;

            while (block.Parent != null)
            {
                var i = from VariableDeclarationSyntax r in block.DescendantNodes().OfType<VariableDeclarationSyntax>()
                        where r.Variables.Any(t => t.Identifier.ValueText == token.ValueText)
                        select r;

                var j = from r in block.Parent.DescendantNodes().OfType<ParameterSyntax>()
                        where r.Identifier.ValueText == token.ValueText
                        select r;

                if (i.Any()) return block;
                else if (j.Any()) return block.Parent;
                else block = FindBlock(block.Parent);
            }
            return block;
        }

        public static SyntaxNode FindDeclaration(SyntaxToken token)
        {
            if (token.Parent is VariableDeclaratorSyntax)  return token.Parent;

            SyntaxNode block = FindBlock(token.Parent.Parent);

            while (block.Parent != null)
            {
                var i = from VariableDeclarationSyntax r in block.DescendantNodes().OfType<VariableDeclarationSyntax>()
                            .Where(s => s.Variables.Any(t => t.Identifier.ValueText == token.ValueText))
                        select r;

                var j = from r in block.Parent.DescendantNodes().OfType<ParameterSyntax>()
                        where r.Identifier.ValueText == token.ValueText
                        select r;

                if (i.Any()) return i.Single();
                else if (j.Any()) return j.Single();
                else block = FindBlock(block.Parent);
            }
            return null;
        }

        public static SyntaxToken FindToken(SyntaxTree tree, int line, int col)
        {
            //var i = from r in tree.GetRoot().DescendantTokens().OfType<SyntaxToken>()
            //            .Where(n => n.Span.Start <= pos)
            //        select r;

            var lineSpan = tree.GetText().GetLineFromLineNumber(line - 1).Extent;
            return tree.GetRoot().DescendantTokens(lineSpan).First(n => lineSpan.Contains(n.Span) && n.Span.End - lineSpan.Start >= col);
        }
    }
}
