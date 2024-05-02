namespace FurnitureCompanyApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Заказ компонентов");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Сделанные заказы");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Заказать сборку");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Заказы", new System.Windows.Forms.TreeNode[] { treeNode1, treeNode2, treeNode3 });
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Комплектующие");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Собранная мебель");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Склад", new System.Windows.Forms.TreeNode[] { treeNode5, treeNode6 });
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Сборка мебели");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Схемы", new System.Windows.Forms.TreeNode[] { treeNode8 });
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 12);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node4";
            treeNode1.Text = "Заказ компонентов";
            treeNode2.Name = "Node5";
            treeNode2.Text = "Сделанные заказы";
            treeNode3.Name = "Node6";
            treeNode3.Text = "Заказать сборку";
            treeNode4.BackColor = System.Drawing.Color.Transparent;
            treeNode4.ForeColor = System.Drawing.Color.Black;
            treeNode4.Name = "Node1";
            treeNode4.Text = "Заказы";
            treeNode5.Name = "Node7";
            treeNode5.Text = "Комплектующие";
            treeNode6.Name = "Node8";
            treeNode6.Text = "Собранная мебель";
            treeNode7.Name = "Node2";
            treeNode7.Text = "Склад";
            treeNode8.Name = "Node9";
            treeNode8.Text = "Сборка мебели";
            treeNode9.Name = "Node3";
            treeNode9.Text = "Схемы";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { treeNode4, treeNode7, treeNode9 });
            this.treeView1.Size = new System.Drawing.Size(151, 426);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.treeView1);
            this.Name = "MainForm";
            this.Text = "Furniture Company";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TreeView treeView1;

        #endregion
    }
}