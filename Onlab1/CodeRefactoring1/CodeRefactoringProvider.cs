using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;

namespace CodeRefactoring1
{
    [ExportCodeRefactoringProvider("CodeRefactoring1", LanguageNames.CSharp)]
    public class CodeRefactoringProvider : ICodeRefactoringProvider
    {
        public CodeRefactoring GetRefactoring(IDocument document, TextSpan textSpan, CancellationToken cancellationToken)
        {
            // Retrieve the token containing textSpan
            var root = (SyntaxNode)document.GetSyntaxRoot(cancellationToken);
            var token = root.FindToken(textSpan.Start, findInsideTrivia: true);

            // 
            if (token.Kind == SyntaxKind.IdentifierToken && token.Span.Start <= textSpan.End && token.Span.End >= textSpan.End)
            {
                if (true)
                {
                    return new CodeRefactoring(
                        new ICodeAction[] { 
                                            new Refactor(document, token),
                                            new Collapse(document, token)
                                          },
                        token.Span);
                }
            }

            return null;
        }
    }
    [ExportSyntaxNodeOutliner(typeof(BlockSyntax))]
    public class Outliner : ISyntaxOutliner
    {

        public IEnumerable<OutliningSpan> GetOutliningSpans(IDocument document, CommonSyntaxTrivia trivia, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OutliningSpan> GetOutliningSpans(IDocument document, CommonSyntaxNode node, CancellationToken cancellationToken)
        {
            return new OutliningSpan[]{ CreateRegion(((SyntaxNode)node).Parent.DescendantTokens().First(t => t.Kind == SyntaxKind.OpenBraceToken), (SyntaxNode)node)};//new OutliningSpan(TextSpan.FromBounds(9, 10), "hyup", true) };
        }

        private static OutliningSpan CreateRegion(SyntaxToken token, SyntaxNode node)
        {
            return new OutliningSpan(
                TextSpan.FromBounds(token.Span.End - 1, node.Span.End),
                bannerText: "Custom Outline",
                autoCollapse: false);
        }
    }
}
