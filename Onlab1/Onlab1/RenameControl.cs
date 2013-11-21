using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Roslyn.Compilers.CSharp;

namespace Onlab1
{
    public partial class RenameControl : UserControl
    {
        private string to = "";
        public delegate void Refactorfunc(SyntaxToken token, string to);
        public delegate void Closefunc(EnvDTE.vsSaveChanges sc = EnvDTE.vsSaveChanges.vsSaveChangesNo);
        public Refactorfunc func = null;
        public Closefunc close = null;
        SyntaxToken token;

        public SyntaxToken Token 
        {
            get {return token;}
            set {token = value;}
        }

        public RenameControl()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            func(token, to);
            close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            to = textBox1.Text;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
                OKButton_Click(sender, e);
        }
    }
}
