using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace StaticMethodCollection
{
    public struct DetailedSyntaxNode
    {
        SyntaxToken node;
        IDocument document;
        SyntaxNode declarator;
        int row;
        int col;

        public DetailedSyntaxNode(IDocument document, SyntaxToken node)
        {
            this.document = document;
            this.node = node;
            this.row = node.GetLocation().GetLineSpan(true).StartLinePosition.Line + 1;
            this.col = node.GetLocation().GetLineSpan(true).StartLinePosition.Character + 1;
            this.declarator = StaticMethods.FindDeclaration(node, document);
        }
        public DetailedSyntaxNode(IDocument document, int row, int col)
        {
            this.document = document;
            this.row = row;
            this.col = col;
            this.node = StaticMethods.FindToken(document.GetSyntaxTree() as SyntaxTree, row, col).Value;
            this.declarator = StaticMethods.FindDeclaration(node, document);
        }

        public SyntaxToken Syntaxnode
        { get { return node; } }
        public SyntaxNode Declaration
        { get { return declarator; } }
        public IDocument Document
        { get { return document; } }
        public int Row
        { get { return row; } }
        public int Column
        { get { return col; } }
        public string Type
        {
            get
            {
                if (declarator == null) return "Unknown";
                var sm = document.Project.GetCompilation().GetSemanticModel(declarator.SyntaxTree);
                if (declarator is VariableDeclaratorSyntax)
                    return ((VariableDeclarationSyntax)declarator.Parent).Type.ToString();
                if (declarator is MethodDeclarationSyntax)
                    return ((MethodDeclarationSyntax)declarator).ReturnType.ToString() + " Method";
                if (declarator is ConstructorDeclarationSyntax)
                    return "Constructor";
                if (declarator is ParameterSyntax)
                    return ((ParameterSyntax)declarator).Type.ToString();
                if (declarator is ClassDeclarationSyntax)
                    return "Class";
                if (declarator is StructDeclarationSyntax)
                    return "Struct";
                if (declarator is DelegateDeclarationSyntax)
                    return "Delegate";
                if (declarator is NamespaceDeclarationSyntax)
                    return "Namespace";

                var sym = sm.GetDeclaredSymbol(declarator);
                return sym.GetType().ToString();
            }
        }

    }

    public class StaticMethods
    {

        public static SyntaxNode FindDeclaration(SyntaxToken token, IDocument doc)
        {
            if ( token.Parent is VariableDeclaratorSyntax ||
                 token.Parent is MethodDeclarationSyntax ||
                 token.Parent is ConstructorDeclarationSyntax ||
                 token.Parent is ParameterSyntax ||
                 token.Parent is ClassDeclarationSyntax ||
                 token.Parent is StructDeclarationSyntax ||
                 token.Parent is DelegateDeclarationSyntax ||
                 token.Parent is NamespaceDeclarationSyntax) return token.Parent;

            try
            {
                var sm = doc.Project.GetCompilation().GetSemanticModel(doc.GetSyntaxTree());
                var sym = sm.GetSymbolInfo(token.Parent).Symbol;
                return sym.DeclaringSyntaxNodes.First() as SyntaxNode;
            }
            catch (Exception e)
            { return null; }

            //SyntaxNode block = FindBlock(token.Parent);

            //do
            //{
            //    var i1 = from r in block.DescendantNodes().OfType<VariableDeclarationSyntax>()
            //            where r.Variables.Any(t => t.Identifier.ValueText == token.ValueText)
            //            select r;

            //    if (i1.Any()) return i1.Single();

            //    var i2 = from r in block.DescendantNodes().OfType<MethodDeclarationSyntax>()
            //            where r.Identifier.ValueText == token.ValueText
            //            select r;

            //    if (i2.Any()) return i2.Single();

            //    var i3 = from r in block.DescendantNodes().OfType<ParameterSyntax>()
            //            where r.Identifier.ValueText == token.ValueText
            //            select r;

            //    if (i3.Any()) return i3.Single();

            //    var i4 = from r in block.DescendantNodes().OfType<ClassDeclarationSyntax>()
            //            where r.Identifier.ValueText == token.ValueText
            //            select r;

            //    if (i4.Any()) return i4.Single(); 

            //    var i5 = from r in block.DescendantNodes().OfType<DelegateDeclarationSyntax>()
            //            where r.Identifier.ValueText == token.ValueText
            //            select r;

            //    if (i5.Any()) return i5.Single();

            //    block = FindBlock(block.Parent);

            //} while (block != null);
            //return null;
        }
                
        public static SyntaxToken? FindToken(SyntaxTree tree, int line, int col)
        {
            var lineSpan = tree.GetText().GetLineFromLineNumber(line - 1).Extent;
            SyntaxToken? result = null;
            try 
	        {	        
		        result = tree.GetRoot().DescendantTokens(lineSpan).First(n => lineSpan.Contains(n.Span) && n.Span.End - lineSpan.Start >= col);
	        }
	        catch (Exception)
            { }

            return result;
        }
    }
}
