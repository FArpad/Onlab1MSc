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
    class Collapse : ICodeAction
    {
        private IDocument document;
        private SyntaxToken token;

        

        public Collapse(IDocument document, SyntaxToken token)
        {
            this.document = document;
            this.token = token;
        }

        public string Description
        {
            get { return "Collapse block"; }
        }

        private SyntaxNode FindBlock(SyntaxNode node)
        {
            var block = node;
            while (block.Parent != null && !(block is BlockSyntax))
                block = block.Parent;

            return block;
        }

        public CodeActionEdit GetEdit(CancellationToken cancellationToken)
        {
            var oldRoot = (SyntaxNode)document.GetSyntaxRoot(cancellationToken);
            SyntaxNode block = FindBlock(token.Parent);

            var newRoot = oldRoot;//ReplaceNodes(oldRoot, token, "burp");

            return new CodeActionEdit(document.UpdateSyntaxRoot(newRoot));
        }
    }
}
