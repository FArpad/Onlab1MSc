using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Company.ResultPackage
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        public MyControl()
        {
            InitializeComponent();
        }

        public void RefreshData()
        {
            list1.Items.Clear();
            //foreach (var processInfo in ProcessList.GetProcesses())
            //{
            //    var item = new ListViewItem(
            //      new[]{
            //              processInfo.Name,
            //              processInfo.Id.ToString(),
            //              processInfo.Priority.ToString(),
            //            processInfo.ThreadCount.ToString()
            //          }
            //    );
            //    list1.Items.Add(item);
            //}

            list1.SelectedItems.Clear();
            list1.SelectedIndex = 0;
            list1.InvalidateVisual();
        }
    }
}