namespace InventoryManagementSystem.UI
{
    partial class frmReportPreview
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tsToolbar = new System.Windows.Forms.ToolStrip();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.sepPrint = new System.Windows.Forms.ToolStripSeparator();
            this.btnPrevious = new System.Windows.Forms.ToolStripButton();
            this.lblPage = new System.Windows.Forms.ToolStripLabel();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.sepZoom = new System.Windows.Forms.ToolStripSeparator();
            this.lblZoom = new System.Windows.Forms.ToolStripLabel();
            this.cmbZoom = new System.Windows.Forms.ToolStripComboBox();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.ppcPreview = new System.Windows.Forms.PrintPreviewControl();
            this.tsToolbar.SuspendLayout();
            this.SuspendLayout();
            //
            // tsToolbar
            //
            this.tsToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPrint,
            this.sepPrint,
            this.btnPrevious,
            this.lblPage,
            this.btnNext,
            this.sepZoom,
            this.lblZoom,
            this.cmbZoom,
            this.btnClose});
            this.tsToolbar.Location = new System.Drawing.Point(0, 0);
            this.tsToolbar.Name = "tsToolbar";
            this.tsToolbar.Size = new System.Drawing.Size(1100, 34);
            this.tsToolbar.TabIndex = 0;
            //
            // btnPrint
            //
            this.btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            //
            // sepPrint
            //
            this.sepPrint.Name = "sepPrint";
            //
            // btnPrevious
            //
            this.btnPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Text = "< Previous";
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            //
            // lblPage
            //
            this.lblPage.Name = "lblPage";
            this.lblPage.Text = "Page 1 of 1";
            //
            // btnNext
            //
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNext.Name = "btnNext";
            this.btnNext.Text = "Next >";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            //
            // sepZoom
            //
            this.sepZoom.Name = "sepZoom";
            //
            // lblZoom
            //
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Text = "Zoom:";
            //
            // cmbZoom
            //
            this.cmbZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZoom.Name = "cmbZoom";
            this.cmbZoom.Size = new System.Drawing.Size(100, 25);
            this.cmbZoom.SelectedIndexChanged += new System.EventHandler(this.cmbZoom_SelectedIndexChanged);
            //
            // btnClose
            //
            this.btnClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClose.Name = "btnClose";
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // ppcPreview
            //
            this.ppcPreview.BackColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.ppcPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ppcPreview.Location = new System.Drawing.Point(0, 34);
            this.ppcPreview.Name = "ppcPreview";
            this.ppcPreview.Size = new System.Drawing.Size(1100, 716);
            this.ppcPreview.TabIndex = 1;
            this.ppcPreview.UseAntiAlias = true;
            this.ppcPreview.StartPageChanged += new System.EventHandler(this.ppcPreview_StartPageChanged);
            //
            // frmReportPreview
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 750);
            this.Controls.Add(this.ppcPreview);
            this.Controls.Add(this.tsToolbar);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "frmReportPreview";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Preview";
            this.Load += new System.EventHandler(this.frmReportPreview_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmReportPreview_KeyDown);
            this.tsToolbar.ResumeLayout(false);
            this.tsToolbar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip tsToolbar;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripSeparator sepPrint;
        private System.Windows.Forms.ToolStripButton btnPrevious;
        private System.Windows.Forms.ToolStripLabel lblPage;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripSeparator sepZoom;
        private System.Windows.Forms.ToolStripLabel lblZoom;
        private System.Windows.Forms.ToolStripComboBox cmbZoom;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.PrintPreviewControl ppcPreview;
    }
}
