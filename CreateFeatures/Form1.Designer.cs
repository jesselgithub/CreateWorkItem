﻿namespace CreateFeatures
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.createFeaturesButton = new System.Windows.Forms.Button();
            this.getIdsButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.RequestId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FeatureAreaPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FeatureIncrementPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AssignedTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FeatureParent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreateTask = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FeatureEstimatedEffort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewImageColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.titleToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RequestId,
            this.FeatureAreaPath,
            this.FeatureIncrementPath,
            this.AssignedTo,
            this.FeatureParent,
            this.CreateTask,
            this.FeatureEstimatedEffort,
            this.Status,
            this.Comment});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.Size = new System.Drawing.Size(1562, 553);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEnter);
            this.dataGridView1.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellMouseEnter);
            this.dataGridView1.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_UserAddedRow);
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1562, 553);
            this.panel1.TabIndex = 1;
            // 
            // createFeaturesButton
            // 
            this.createFeaturesButton.Location = new System.Drawing.Point(150, 5);
            this.createFeaturesButton.Name = "createFeaturesButton";
            this.createFeaturesButton.Size = new System.Drawing.Size(111, 37);
            this.createFeaturesButton.TabIndex = 2;
            this.createFeaturesButton.Text = "Create Features";
            this.createFeaturesButton.UseVisualStyleBackColor = true;
            this.createFeaturesButton.Click += new System.EventHandler(this.createFeaturesButton_Click);
            // 
            // getIdsButton
            // 
            this.getIdsButton.Location = new System.Drawing.Point(21, 5);
            this.getIdsButton.Name = "getIdsButton";
            this.getIdsButton.Size = new System.Drawing.Size(111, 37);
            this.getIdsButton.TabIndex = 3;
            this.getIdsButton.Text = "Get Request Ids...";
            this.getIdsButton.UseVisualStyleBackColor = true;
            this.getIdsButton.Click += new System.EventHandler(this.getIdsButton_Click);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "Status";
            this.dataGridViewImageColumn1.Image = global::CreateFeatures.Properties.Resources.NO;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.ReadOnly = true;
            this.dataGridViewImageColumn1.Width = 50;
            // 
            // RequestId
            // 
            this.RequestId.DataPropertyName = "AreaPathData";
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.RequestId.DefaultCellStyle = dataGridViewCellStyle1;
            this.RequestId.HeaderText = "Request Id";
            this.RequestId.Name = "RequestId";
            this.RequestId.ReadOnly = true;
            // 
            // FeatureAreaPath
            // 
            this.FeatureAreaPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FeatureAreaPath.HeaderText = "Feature Area Path";
            this.FeatureAreaPath.Name = "FeatureAreaPath";
            this.FeatureAreaPath.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // FeatureIncrementPath
            // 
            this.FeatureIncrementPath.DataPropertyName = "IncData";
            this.FeatureIncrementPath.HeaderText = "Feature Increment Path";
            this.FeatureIncrementPath.Name = "FeatureIncrementPath";
            this.FeatureIncrementPath.Width = 225;
            // 
            // AssignedTo
            // 
            this.AssignedTo.HeaderText = "Assigned To";
            this.AssignedTo.Name = "AssignedTo";
            this.AssignedTo.Width = 250;
            // 
            // FeatureParent
            // 
            this.FeatureParent.HeaderText = "Feature Parent";
            this.FeatureParent.Name = "FeatureParent";
            // 
            // CreateTask
            // 
            this.CreateTask.HeaderText = "Create Task In Downtrace";
            this.CreateTask.Name = "CreateTask";
            // 
            // FeatureEstimatedEffort
            // 
            this.FeatureEstimatedEffort.HeaderText = "Feature Estimated Effort";
            this.FeatureEstimatedEffort.Name = "FeatureEstimatedEffort";
            // 
            // Status
            // 
            this.Status.HeaderText = "Status";
            this.Status.Image = global::CreateFeatures.Properties.Resources.NO;
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.Width = 50;
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            this.Comment.ReadOnly = true;
            this.Comment.Width = 200;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(294, 12);
            this.progressBar1.MarqueeAnimationSpeed = 10;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1245, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1562, 601);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.getIdsButton);
            this.Controls.Add(this.createFeaturesButton);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Create WorkItems for Requests";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button createFeaturesButton;
        private System.Windows.Forms.Button getIdsButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn RequestId;
        private System.Windows.Forms.DataGridViewTextBoxColumn FeatureAreaPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn FeatureIncrementPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn AssignedTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn FeatureParent;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CreateTask;
        private System.Windows.Forms.DataGridViewTextBoxColumn FeatureEstimatedEffort;
        private System.Windows.Forms.DataGridViewImageColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.ToolTip titleToolTip;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

