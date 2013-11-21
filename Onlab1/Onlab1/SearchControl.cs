using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using StaticMethodCollection;
using AutocompleteMenuNS;

namespace Onlab1
{
    public partial class SearchControl : UserControl
    {
        Window refwindow = null;
        public delegate void Closefunc(EnvDTE.vsSaveChanges sc = EnvDTE.vsSaveChanges.vsSaveChangesNo);
        public delegate List<DetailedSyntaxNode> Searchfunc(string exp, bool unique = false, int? top = null);
        public Searchfunc srchfunc = null;
        public Closefunc close = null;
        bool JustSelected = false;

        public SearchControl()
        {
            InitializeComponent();
        }

        public void setResultWindow(Window rc)
        {
            refwindow = rc;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;
            if (JustSelected) 
            {
                JustSelected = false;
                return; 
            }

            var list = srchfunc(textBox1.Text, true, 10).Select(t => t.Syntaxnode.ValueText);

            if (textBox1.Text.ToUpper() == textBox1.Text)
            {
                string text = "";
                for (int i = 0; i < textBox1.Text.Length; i++)
                {
                    text = text + textBox1.Text[i] + "[A-Za-z0-9]*";
                }

                menu1.SearchPattern = text;
            }
            else menu1.SearchPattern = @"[\w\.]";
            menu1.SetAutocompleteItems(list.ToArray());
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "") return;

                ((RefControl)refwindow.Object).SearchVal = textBox1.Text;
                ((RefControl)refwindow.Object).fillResults();

                refwindow.Visible = true;
                textBox1.Text = "";
                close();
            }
        }

        private void menu1_Selecting(object sender, SelectingEventArgs e)
        {
            JustSelected = true;
            textBox1.Text = "";
        }
    }
}
