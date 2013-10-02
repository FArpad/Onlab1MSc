using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;

namespace CodeRefactoring1
{
    class Refactor : ICodeAction
    {
        private IDocument document;
        private SyntaxToken token;

        public Refactor(IDocument document, SyntaxToken token)
        {
            this.document = document;
            this.token = token;
        }

        public string Description
        {
            get { return "Refactor variable"; }
        }

        public SyntaxNode ReplaceNodes(SyntaxNode root, SyntaxToken _from, string to)
        {
            SyntaxNode block;
            if (_from.Parent is IdentifierNameSyntax)
                block = FindDeclarationBlock(_from);
            else block = root;

            var vars = from r in block.DescendantTokens().OfType<SyntaxToken>()
                              where (r.ToString() == _from.ValueText)
                              select r;

            Func<SyntaxToken, SyntaxToken, SyntaxToken> foo = (i1, i2) =>
                    Syntax.Identifier(i1.LeadingTrivia, to, i1.TrailingTrivia);
            SyntaxNode newRoot = root.ReplaceTokens(vars, foo);

            return newRoot;
        }

        private SyntaxNode FindBlock(SyntaxNode node)
        {
            var block = node;
            while(block.Parent != null && !(block is BlockSyntax))
                block = block.Parent;
         
            return block;
        }

        private SyntaxNode FindDeclarationBlock(SyntaxToken token)
        {
            SyntaxNode block = FindBlock(token.Parent);

            while(block.Parent != null)
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

        public CodeActionEdit GetEdit(CancellationToken cancellationToken)
        {
            var oldRoot = (SyntaxNode)document.GetSyntaxRoot(cancellationToken);
            var newRoot = ReplaceNodes(oldRoot, token, "burp");

            return new CodeActionEdit(document.UpdateSyntaxRoot(newRoot));
        }
    }
}
