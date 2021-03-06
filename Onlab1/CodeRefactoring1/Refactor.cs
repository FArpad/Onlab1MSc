﻿using System;
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

        public CodeActionEdit GetEdit(CancellationToken cancellationToken)
        {
            var oldRoot = (SyntaxNode)document.GetSyntaxRoot(cancellationToken);
            var newRoot = StaticMethods.ReplaceNodes(oldRoot, token, "burp");

            return new CodeActionEdit(document.UpdateSyntaxRoot(newRoot));
        }
    }
}
