using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using StaticMethodCollection;

namespace Onlab1
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{

        CommandBarButton button1 = null, button2 = null, button3 = null, button4 = null;
        public enum Windowtype { Wrename, Wfindref, Wsearch };
        private Dictionary<string, IDocument> docBuffer = null;
        bool DocReloadOption;
        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}
        
		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

            Command command1 = null;
            Command command2 = null;
            Command command3 = null;
            Command command4 = null;
            Command command99 = null;
            
            object[] contextGUIDS = new object[] { };
            Commands2 commands = (Commands2)_applicationObject.Commands;
            CommandBars commandBars = (CommandBars)_applicationObject.CommandBars;

            CommandBar menuBarCommandBar = (commandBars)["MenuBar"];
            CommandBar standardToolBar = (commandBars)["Project"];
            CommandBar CodeToolBar = (commandBars)["Code Window"];

            //Find the Tools command bar on the MenuBar command bar:
            CommandBarControl toolsControl = menuBarCommandBar.Controls["Tools"];
            CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

            if (connectMode == ext_ConnectMode.ext_cm_UISetup)
            {        

                try
                {
                    //Add a command to the Commands collection:
                    command99 = commands.AddNamedCommand2(_addInInstance, "SolExpname", "Turn it on", "Turns on Onlab1 Addin", true,
                        186, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                }
                catch (System.ArgumentException)
                {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                    command99 = commands.Item("Onlab1.Connect.SolExpname");
                }

                //Add a control for the command to the tools menu:
                if ((command99 != null) && (toolsPopup != null) && (standardToolBar != null))
                {
                    command99.AddControl(toolsPopup.CommandBar, 1);
                    CommandBarControl ctrl = (CommandBarControl)command99.AddControl(standardToolBar, 1);
                    ctrl.TooltipText = "Turns on Onlab1 Addin";
                }
            }
            else
            {
                DocReloadOption = 
                    (bool)_applicationObject.Properties["Environment", "Documents"]
                    .Item("AutoloadExternalChanges").Value;

                try
                {
                    //Add a command to the Commands collection:
                    command1 = commands.AddNamedCommand(_addInInstance, "refract", "Replace", 
                        "Replaces the word under the cursor with a new one in the whole document", 
                        true, 219, null, (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled));
                    command2 = commands.AddNamedCommand(_addInInstance, "collapse", "Collapse", "Collapses text in the document", true,
                        194, null, (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled));
                    command3 = commands.AddNamedCommand(_addInInstance, "findrefs", "Find References", "Find all references in the document", true,
                        195, null, (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled));
                    command4 = commands.AddNamedCommand(_addInInstance, "search", "Search", "Search for patterns in the solution", true,
                        25, null, (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled));
                    command4.Bindings = "Global::shift+ctrl+r";
                }
                catch (System.ArgumentException)
                {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                    command1 = commands.Item("Onlab1.Connect.refract");
                    command2 = commands.Item("Onlab1.Connect.collapse");
                    command3 = commands.Item("Onlab1.Connect.findrefs");
                    command4 = commands.Item("Onlab1.Connect.search");
                }
                if ((command1 != null) && (command2 != null) && (command3 != null) && (command4 != null) && (CodeToolBar != null))
                {
                    button1 = (CommandBarButton)command1.AddControl(CodeToolBar, CodeToolBar.Controls.Count + 1);
                    button2 = (CommandBarButton)command2.AddControl(CodeToolBar, CodeToolBar.Controls.Count + 1);
                    button3 = (CommandBarButton)command3.AddControl(CodeToolBar, CodeToolBar.Controls.Count + 1);
                    button4 = (CommandBarButton)command4.AddControl(CodeToolBar, CodeToolBar.Controls.Count + 1);
                    button1.Enabled = true;
                    button1.Visible = true;
                    button2.Enabled = true;
                    button2.Visible = true;
                    button3.Enabled = true;
                    button3.Visible = true;
                    button4.Enabled = true;
                    button4.Visible = true;
                }
            }
	}

        public void Selection(DTE2 dte)
        {
            // If no document is open, do nothing and return.
            if (dte.ActiveDocument == null) return;

            TextSelection objSel = (EnvDTE.TextSelection)(
            dte.ActiveDocument.Selection);
            string srchPattern1 = "{";
            string srchPattern2 = "}";
            long lCursorLine = objSel.TopPoint.Line;
            long lCursorColumn = objSel.TopPoint.LineCharOffset;
                        
            long cntr = 1;
            objSel.SelectLine();

            while (cntr > 0)
            {
                if (objSel.Text.Contains(srchPattern1)) cntr--;
                if (objSel.Text.Contains(srchPattern2)) cntr++;
                objSel.LineUp(false, 1);
                if(objSel.TopPoint.AtStartOfDocument) return;
                objSel.SelectLine();
            }
            objSel.LineDown(false, 1);

            //  If it's the beginning of a pattern, 
            // save the position.
            long lStartLine = objSel.TopPoint.Line;
            long lStartColumn = objSel.TopPoint.LineCharOffset;

            objSel.MoveToLineAndOffset(System.Convert.ToInt32
                    (lCursorLine), System.Convert.ToInt32(lCursorColumn), true);
            objSel.SelectLine();
            cntr = 1;

            while (cntr > 0)
            {
                if (objSel.Text.Contains(srchPattern2)) cntr--;
                if (objSel.Text.Contains(srchPattern1)) cntr++;
                objSel.LineDown(false, 0);
                if (objSel.TopPoint.AtEndOfDocument) return;
                objSel.SelectLine();
            }
            objSel.LineUp(false, 1);


             //  Select the entire section and outline it.
            objSel.SwapAnchor();
            objSel.MoveToLineAndOffset(System.Convert.ToInt32
            (lStartLine), System.Convert.ToInt32(lStartColumn), true);
            objSel.OutlineSection();
            objSel.LineDown(false, 1);
        }


        public void ReplacePattern(SyntaxToken token, string to)
        {            
            var refs = Findrefs(token);

            _applicationObject.Properties["Environment", "Documents"]
                .Item("AutoloadExternalChanges").Value = true;
            
            foreach (var document in refs.Select(t => t.Document).Distinct())
            {
                var vars = refs.Where(t => t.Document == document).Select(t => t.Syntaxnode);

                Func<SyntaxToken, SyntaxToken, SyntaxToken> foo = (i1, i2) =>
                    Syntax.Identifier(i1.LeadingTrivia, to, i1.TrailingTrivia);
                SyntaxNode newRoot = (document.GetSyntaxRoot() as SyntaxNode).ReplaceTokens(vars, foo);

                Roslyn.Compilers.IText newText = newRoot.GetText();
                workspace.UpdateDocument(document.Id, newText);
                

                docBuffer[document.FilePath] = workspace.CurrentSolution.GetDocument(document.Id);
            }
            solution = workspace.CurrentSolution;
            Setbases();
        }

        public List<DetailedSyntaxNode> Search(string exp, bool unique = false, int? top = null)
        {
            if(exp == "") return null;
            
            List<DetailedSyntaxNode> result = new List<DetailedSyntaxNode>();
            string realexp = "";
            if (exp.ToUpper() == exp)
                foreach (char c in exp)
                {
                    realexp = realexp + c + "[A-Za-z0-9]*";
                }
            else realexp = exp;

            foreach (var doc in docBuffer.Values)
            { 
                SyntaxNode where = doc.GetSyntaxRoot() as SyntaxNode;
                MatchCollection matches = Regex.Matches(where.ToFullString(), realexp);

                foreach (Match match in matches)
                {
                    var txtline = doc.GetText().GetLineFromPosition(match.Index);
                    var line = txtline.LineNumber + 1;
                    var col = match.Index - txtline.Start + 1;
                    try
                    {
                        var keyval = new DetailedSyntaxNode(doc, line, col);
                        if (!(unique && result.Any(r => r.Syntaxnode.ValueText == keyval.Syntaxnode.ValueText)))
                        {
                            result.Add(keyval);
                            if (top.HasValue)
                            {
                                top--;
                                if (top.Value <= 0) return result;
                            }
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
            return result;
        }

        public List<DetailedSyntaxNode> Findrefs(SyntaxToken token)
        {
            List<DetailedSyntaxNode> result = new List<DetailedSyntaxNode>();
            //result = result.Add(token.Parent);
            IDocument declaringdoc = basedoc;
            
            SyntaxNode n = StaticMethods.FindDeclaration(token, basedoc);
            
            if (n == null) return null;

            bool isDeclared = 
                 token.Parent is VariableDeclaratorSyntax ||
                 token.Parent is MethodDeclarationSyntax ||
                 token.Parent is ConstructorDeclarationSyntax ||
                 token.Parent is ParameterSyntax ||
                 token.Parent is ClassDeclarationSyntax ||
                 token.Parent is StructDeclarationSyntax ||
                 token.Parent is DelegateDeclarationSyntax ||
                 token.Parent is NamespaceDeclarationSyntax;

            if (!basetree.GetRoot().DescendantNodesAndSelf().Contains(n))
            {
                foreach (var doc in docBuffer.Values)
                    if (doc.GetSyntaxRoot().DescendantNodesAndSelf().Where(t => t == n).Any())
                    {
                        declaringdoc = doc;
                        break;
                    }
            }
            
            // Process each syntax tree...
            var compilation = declaringdoc.Project.GetCompilation();
            var semanticModel = compilation.GetSemanticModel(basetree);
            
            try
            {
                ISymbol varSymbol = null;
               
                if (!isDeclared)
                    varSymbol = semanticModel.GetSymbolInfo(token.Parent).Symbol;
                else
                    varSymbol = semanticModel.GetDeclaredSymbol(n);

                var refs = varSymbol.FindReferences(solution);

                var keyval = new DetailedSyntaxNode(declaringdoc, (SyntaxToken)n.DescendantTokens().First(t => t.ValueText == token.ValueText));
                result.Add(keyval);

                foreach (var reference in refs)
                    foreach (var loc in reference.Locations)
                    {
                        var tree = loc.Location.SourceTree;
                        var node = tree.GetRoot().DescendantTokens().First(t => loc.Location == t.Parent.GetLocation());
                        keyval = new DetailedSyntaxNode(loc.Document, (SyntaxToken)node);
                        result.Add(keyval);
                    }
            }
            catch (Exception)
            { }
            return result;
        }

        public void Setbases()
        {
            docBuffer.TryGetValue(_applicationObject.ActiveDocument.FullName, out basedoc);
            basetree = basedoc.GetSyntaxTree() as SyntaxTree;
        }

        public void RefreshDocBuffer()
        {
            string solutionName = _applicationObject.Solution.FullName;
            if (docBuffer != null && _applicationObject.ActiveDocument.Saved)
                return;
            
            _applicationObject.Solution.SaveAs(solutionName);
            
            workspace = Workspace.LoadSolution(solutionName);
            solution = workspace.CurrentSolution;
            //TextDocument doc = (TextDocument)_applicationObject.ActiveDocument.Object("TextDocument");
            //basetree = SyntaxTree.ParseText(doc.CreateEditPoint().GetText(doc.EndPoint));
            docBuffer = new System.Collections.Generic.Dictionary<string, IDocument>();
            foreach (var project in solution.Projects)
                foreach (var document in project.Documents)
                    if (!docBuffer.ContainsKey(document.FilePath)) docBuffer.Add(document.FilePath, document);
                        
        }

        public void MakeWindow(Windowtype type, SyntaxToken? token)
        {
            RenameControl renc;
            RefControl refc;
            SearchControl srchc;

            if (toolWindows[type] == null)
            {
                Windows2 wins2obj = (Windows2)_applicationObject.Windows;
                string an = Assembly.GetExecutingAssembly().Location;   
                object myUserControlObject = null;
                switch (type)
                {
                    case Windowtype.Wrename:
                    {                            
                        string className = typeof(RenameControl).FullName;
                        const string aGuid = "{6CCD0EE9-20DB-4636-9149-665A958D8A9A}"; 
                        
                        toolWindows[type] = wins2obj.CreateToolWindow2(_addInInstance, an,
                             className, "", aGuid, ref myUserControlObject);
                        toolWindows[type].IsFloating = true;
                        //toolWindows[type].Linkable = false;
                        
                        renc = (RenameControl)myUserControlObject;
                        renc.func = new RenameControl.Refactorfunc(ReplacePattern);
                        renc.close = new RenameControl.Closefunc(toolWindows[type].Close);
                        if (token.HasValue) renc.Token = token.Value;

                        toolWindows[type].Height = renc.Height;
                        toolWindows[type].Width = renc.Width;
                    }
                    break;
                    case Windowtype.Wfindref:
                    {
                        string className = typeof(RefControl).FullName;
                        const string aGuid = "{6CCD0EE9-20DB-4636-9149-665A958D8A9B}";

                        toolWindows[type] = wins2obj.CreateToolWindow2(_addInInstance, an,
                             className, "Roslyn Search Results", aGuid, ref myUserControlObject);
                        toolWindows[type].IsFloating = false;
                        refc = (RefControl)myUserControlObject;
                        refc.setDTE(ref _applicationObject);
                        refc.reffunc = new RefControl.Findreffunc(Findrefs);
                        refc.srchfunc = new RefControl.Searchfunc(Search);
                        if (token.HasValue) refc.Token = token.Value;
                    }
                    break;
                    case Windowtype.Wsearch:
                    {
                        string className = typeof(SearchControl).FullName;
                        const string aGuid = "{6CCD0EE9-20DB-4636-9149-665A958D8A9C}";

                        toolWindows[type] = wins2obj.CreateToolWindow2(_addInInstance, an,
                             className, "", aGuid, ref myUserControlObject);
                        srchc = (SearchControl)myUserControlObject;
                        srchc.close = new SearchControl.Closefunc(toolWindows[type].Close);
                        srchc.srchfunc = new SearchControl.Searchfunc(Search);
                        srchc.setResultWindow(toolWindows[Windowtype.Wfindref]);
                        toolWindows[type].Height = srchc.Height;
                        toolWindows[type].Width = srchc.Width;
                    }
                    break;
                }
            }
            else switch(type)
            {
                case Windowtype.Wrename:
                {
                    renc = (RenameControl)toolWindows[type].Object;
                    if (token.HasValue) renc.Token = token.Value;
                } 
                break;
                case Windowtype.Wfindref:
                {
                    refc = (RefControl)toolWindows[type].Object;
                    if (token.HasValue) refc.Token = token.Value;
                }
                break;
                case Windowtype.Wsearch:
                {
                    srchc = (SearchControl)toolWindows[type].Object;
                }
                break;
            }
            
        }

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
            if (button1 != null) button1.Delete();
            if (button2 != null) button2.Delete();
            if (button3 != null) button3.Delete();
            if (button4 != null) button4.Delete();

            _applicationObject.Properties["Environment", "Documents"]
                .Item("AutoloadExternalChanges").Value = DocReloadOption;
        }

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
        {
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "Onlab1.Connect.SolExpname")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
                if (commandName.EndsWith("refract"))
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
                if (commandName.EndsWith("collapse"))
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
                if (commandName.EndsWith("findrefs"))
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
                if (commandName.EndsWith("search"))
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
                
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;

            TextSelection objSel = (EnvDTE.TextSelection)(
            _applicationObject.ActiveDocument.Selection);
            if (objSel == null) return;

            int lStartLine = objSel.TopPoint.Line; 
            int lStartColumn = objSel.TopPoint.LineCharOffset;
 
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
                if (commandName.EndsWith("SolExpname"))
                    return;

                RefreshDocBuffer();
                Setbases();
                
                if (commandName.EndsWith("refract"))
				{
                    handled = true;
                    SyntaxToken? token = StaticMethods.FindToken(basetree, lStartLine, lStartColumn);
                    if (!token.HasValue) {System.Windows.Forms.MessageBox.Show("Nothing here!"); return;  }

                    if (StaticMethods.FindDeclaration(token.Value, basedoc) != null)
                    {
                        MakeWindow(Windowtype.Wrename, token.Value);
                        toolWindows[Windowtype.Wrename].Visible = true;
                    }
                    else System.Windows.Forms.MessageBox.Show("Keywords cannot be renamed!");
                    return;
				}
                if (commandName.EndsWith("collapse"))
                {
                    handled = true;
                    
                    Selection(_applicationObject);
                    return;
                }
                if (commandName.EndsWith("findrefs"))
                {
                    handled = true;
                    SyntaxToken? token = StaticMethods.FindToken(basetree, lStartLine, lStartColumn);
                    if (!token.HasValue) { System.Windows.Forms.MessageBox.Show("Nothing here!"); return; }

                    MakeWindow(Windowtype.Wfindref, token.Value);
                    toolWindows[Windowtype.Wfindref].Visible = true;
                    (toolWindows[Windowtype.Wfindref].Object as RefControl).fillResults();
                    return;
                }
                if (commandName.EndsWith("search"))
                {
                    handled = true;
                    
                    MakeWindow(Windowtype.Wfindref, null);
                    toolWindows[Windowtype.Wfindref].Visible = false;
                    MakeWindow(Windowtype.Wsearch, null);
                    toolWindows[Windowtype.Wsearch].Visible = true;
                    return;
                }
			}
		}
        private DTE2 _applicationObject;
		private AddIn _addInInstance;

        private IWorkspace workspace;
        private ISolution solution;
        private IDocument basedoc;
        private SyntaxTree basetree;
        
        private struct ToolWindows
        {
            Window renameToolWindow;
            Window findrefToolWindow;
            Window searchToolWindow;

            public Window this[Windowtype t] 
            {
                get
                {
                    switch (t)
                    {
                        case Windowtype.Wrename: return renameToolWindow;
                        case Windowtype.Wfindref: return findrefToolWindow;
                        case Windowtype.Wsearch: return searchToolWindow;
                        default: return null;
                    }
                }
                set
                {
                    switch (t)
                    {
                        case Windowtype.Wrename: renameToolWindow = value; break;
                        case Windowtype.Wfindref: findrefToolWindow = value; break;
                        case Windowtype.Wsearch: searchToolWindow = value; break;
                    }
                }
            }
        }
        private ToolWindows toolWindows = new ToolWindows();
	}
}