using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FurnitureCompanyApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LaunchApp();
        }

        private static void Test()
        {
            Regex regex = new Regex(@"^[0-9]+(?:[,]\d*)?\z");
            string test = "356,5346rjyj";
            Console.WriteLine(regex.Matches(test).Count);
            Console.WriteLine(regex.IsMatch(test));
        }
        
        public static bool AreAllNodesExpanded(TreeView treeView)
        {
            foreach (TreeNode node in treeView.Nodes)
                if (!node.IsExpanded)
                    return false;
            return true;
        }

        public static bool IsAnyNodeExpanded(TreeView treeView)
        {
            foreach (TreeNode node in treeView.Nodes)
                if (node.IsExpanded)
                    return true;
            return false;
        }

        private static void LaunchApp()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}