using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using StaticMethodCollection;

namespace Onlab1
{
    public partial class RefControl : UserControl
    {
        public delegate List<DetailedSyntaxNode> Findreffunc(SyntaxToken token);
        public delegate List<DetailedSyntaxNode> Searchfunc(string exp, bool unique = false, int? top = null);
        public Findreffunc reffunc = null;
        public Searchfunc srchfunc = null;
        bool isRef = true;

        List<DetailedSyntaxNode> list;
        DTE2 appobj;

        SyntaxToken referredtoken;
        string searchval;
        enum GroupType {Project = 0, Type = 1};
        GroupType grouping = GroupType.Project;


        public SyntaxToken Token
        {
            get { return referredtoken; }
            set { referredtoken = value; 
                  searchval = value.ValueText;
                  isRef = true;
                }
        }

        public string SearchVal
        {
            set{
                searchval = value;
                isRef = false;
               }
        }

        private T Groupbase<T>(DetailedSyntaxNode node, int lvl)
        {
            object result = null;
            switch (grouping)
            {
                case GroupType.Project:
                {
                    if (lvl == 0) result = node.Document.Project.Name;
                    if (lvl == 1) result = node.Document.Name;
                    if (lvl == 2) result = node.Syntaxnode.Span.Start;
                } break;
                case GroupType.Type:
                {
                    if (lvl == 0) result = node.Type;
                    if (lvl == 1) result = node.Document.Name;
                    if (lvl == 2) result = node.Syntaxnode.Span.Start;
                } break;
            }
            return (T)result;
        }

        public RefControl()
        {
            InitializeComponent();
        }
        
        public void setDTE(ref DTE2 appobj)
        {
            this.appobj = appobj;
        }

        public void fillResults()
        {
            fillResults(!isRef);
        }

        private void fillResults(bool inWholeSolution)
        {
            textBox1.Text = searchval;
            treeView1.Nodes.Clear();
            if(!inWholeSolution) list = reffunc(referredtoken);
            else list = srchfunc(searchval);
            if (list == null) return;


            list.Sort(delegate(DetailedSyntaxNode p, DetailedSyntaxNode q) 
                              { int c1 = Groupbase<string>(p, 0).CompareTo(Groupbase<string>(q, 0));
                                if (c1 != 0) return c1;
                                int c2 = Groupbase<string>(p, 1).CompareTo(Groupbase<string>(q, 1)); 
                                if (c2 != 0) return c2;
                                return Groupbase<int>(p, 2).CompareTo(Groupbase<int>(q, 2));
                              });
            //tn: Top Node, mn: Middle Node, ln: Leaf Node
            TreeNode tn = null, mn = null, ln = null;
            int i = 0;
            foreach (var item in list)
            {
                if (tn == null || tn.Text != Groupbase<string>(item, 0))
                {
                    tn = treeView1.Nodes.Add(Groupbase<string>(item, 0));
                    mn = null;
                }
                if (mn == null || mn.Text != Groupbase<string>(item, 1))
                    mn = tn.Nodes.Add(Groupbase<string>(item, 1));
                ln = mn.Nodes.Add(
                    item.Syntaxnode.SyntaxTree.FilePath + " - " + "(" + item.Syntaxnode.Span + ")" + " - " + item.Syntaxnode.ValueText);
                if (!tn.IsExpanded) tn.Expand();
                if (!mn.IsExpanded) mn.Expand();
                ln.Tag = i++;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Nodes.Count != 0) return;
            var node = list.ElementAt((int)treeView1.SelectedNode.Tag);
            appobj.ItemOperations.OpenFile(node.Syntaxnode.SyntaxTree.FilePath);
            TextSelection objSel = (EnvDTE.TextSelection)(appobj.ActiveDocument.Selection);
            objSel.MoveToLineAndOffset(node.Row, node.Column);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchval = textBox1.Text;
                isRef = false;
                fillResults(true);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            grouping = (GroupType)comboBox1.SelectedIndex;
            fillResults();
        }
    }
}
