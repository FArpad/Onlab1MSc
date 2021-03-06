using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace MyAddin2
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{

        CommandBarButton button1 = null, button2 = null;

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

            Command command1;
            Command command2;
            Command command3;
            
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
                    command3 = commands.AddNamedCommand2(_addInInstance, "SolExpname", "Turn it on", "Turns on MyAddin", true,
                        186, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                }
                catch (System.ArgumentException)
                {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                    return;
                }

                //Add a control for the command to the tools menu:
                if ((command3 != null) && (toolsPopup != null) && (standardToolBar != null))
                {
                    command3.AddControl(toolsPopup.CommandBar, 1);
                    CommandBarControl ctrl = (CommandBarControl)command3.AddControl(standardToolBar, 1);
                    ctrl.TooltipText = "Turns on MyAddin";
                }
            }
            else
            {
                try
                {
                    //Add a command to the Commands collection:
                    command1 = commands.AddNamedCommand(_addInInstance, "refract", "Replace", 
                        "Replaces the word under the cursor with a new one in the whole document", 
                        true, 219, null, (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled));
                    command2 = commands.AddNamedCommand(_addInInstance, "collapse", "Collapse", "Collapses text in the document", true,
                        194, null, (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled));
                }
                catch (System.ArgumentException)
                {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                    return;
                }
                if ((command1 != null) && (command2 != null) && (CodeToolBar != null))
                {
                    button1 = (CommandBarButton)command1.AddControl(CodeToolBar, CodeToolBar.Controls.Count + 1);
                    button2 = (CommandBarButton)command2.AddControl(CodeToolBar, CodeToolBar.Controls.Count + 1);
                    button1.Enabled = true;
                    button1.Visible = true;
                    button2.Enabled = true;
                    button2.Visible = true;
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
            EnvDTE.TextRanges textRanges = null;
            long lCursorLine = objSel.TopPoint.Line;
            long lCursorColumn = objSel.TopPoint.LineCharOffset;

            //Move to the beginning of the document so we can iterate 
            // over the whole thing.
            //objSel.StartOfDocument(false);

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


        public void ReplacePattern(DTE2 dte, string to)
        {
            // If no document is open, do nothing and return.
            if (dte.ActiveDocument == null) return;
            //first it calculates the word it was clicked on, and makes that the replacable pattern
            string  from;

            TextSelection objSel = (EnvDTE.TextSelection)(
            dte.ActiveDocument.Selection);
            
            //cursor position within line
            long lStartColumn = objSel.TopPoint.LineCharOffset;
            objSel.SelectLine();
            //filter out the actual words from the line ignoring the junk
            string[] words = System.Text.RegularExpressions.Regex.Split(objSel.Text, @"\W+");
            
            int index = 0, i = 0;

            index = objSel.Text.IndexOf(words[0], 0);
            while (index + words[i].Length < lStartColumn && index < objSel.Text.Length)
            {
                index = objSel.Text.IndexOf(words[++i], index + words[i - 1].Length);
            }
            
            from = words[i];

            //And now calls replace on the whole document
            objSel.SelectAll();
            objSel.ReplaceText(from, to);

        }

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
            if (button1 != null) button1.Delete();
            if (button2 != null) button2.Delete();
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
				if(commandName == "MyAddin2.Connect.MyAddin2")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
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
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
                if (commandName.EndsWith("refract"))
				{
                    handled = true;
                    string result;
                    inputform fm = new inputform();
                    fm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    if (fm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        result = fm.outref;
                    else return;
                     ReplacePattern(_applicationObject, result);
					return;
				}
                if (commandName.EndsWith("collapse"))
                {
                    handled = true; 
                    Selection(_applicationObject);
                    return;
                }
			}
		}
		private DTE2 _applicationObject;
		private AddIn _addInInstance;
	}
}