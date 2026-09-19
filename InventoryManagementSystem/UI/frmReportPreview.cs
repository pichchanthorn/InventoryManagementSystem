using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Windows.Forms;
using InventoryManagementSystem.UI.Reporting;

namespace InventoryManagementSystem.UI
{
    /// <summary>
    /// Report Viewer-style preview: a toolbar (Print, page navigation, zoom, Close) above a PrintPreviewControl.
    /// The preview and the real print job use the very same <see cref="ReportPrinter"/> instance, so what is
    /// previewed is exactly what is printed. The form never touches the database.
    /// </summary>
    public partial class frmReportPreview : Form
    {
        private const string FitPage = "Fit page";
        private static readonly string[] ZoomLevels = { FitPage, "50%", "75%", "100%", "125%", "150%" };

        private readonly ReportPrinter _printer;
        private int _pageCount = 1;

        public frmReportPreview(ReportDocument document)
        {
            Font = Theme.BaseFont;
            InitializeComponent();

            _printer = new ReportPrinter(document);
            Text = "Report Preview - " + document.Title;

            tsToolbar.BackColor = Theme.Band;
            tsToolbar.ForeColor = Theme.TextDark;
            lblPage.Font = Theme.SemiboldFont;
            btnPrint.BackColor = Theme.Primary;
            btnPrint.ForeColor = System.Drawing.Color.White;
            btnPrint.Font = Theme.SemiboldFont;
        }

        /// <summary>Number of pages in the report (at least 1).</summary>
        public int PageCount { get { return _pageCount; } }

        /// <summary>The page currently at the top of the preview, 1-based.</summary>
        public int CurrentPage { get { return ppcPreview.StartPage + 1; } }

        private void frmReportPreview_Load(object sender, EventArgs e)
        {
            _pageCount = _printer.PageCount;

            cmbZoom.Items.AddRange(ZoomLevels);
            cmbZoom.SelectedItem = "Fit page";

            ppcPreview.Document = _printer;
            ppcPreview.Rows = 1;
            ppcPreview.Columns = 1;
            ppcPreview.StartPage = 0;
            UpdatePager();
        }

        // ---- navigation -----------------------------------------------------------------------

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            GoToPage(CurrentPage - 1);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            GoToPage(CurrentPage + 1);
        }

        /// <summary>Shows a 1-based page, clamped to the valid range.</summary>
        public void GoToPage(int page)
        {
            int target = Math.Max(1, Math.Min(_pageCount, page));
            ppcPreview.StartPage = target - 1;
            UpdatePager();
        }

        private void ppcPreview_StartPageChanged(object sender, EventArgs e)
        {
            UpdatePager();
        }

        private void UpdatePager()
        {
            int current = Math.Max(1, Math.Min(_pageCount, ppcPreview.StartPage + 1));
            lblPage.Text = "Page " + current + " of " + _pageCount;
            btnPrevious.Enabled = current > 1;
            btnNext.Enabled = current < _pageCount;
        }

        // ---- zoom -----------------------------------------------------------------------------

        private void cmbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            string level = cmbZoom.SelectedItem as string;
            if (level == null)
                return;

            if (level == FitPage)
            {
                ppcPreview.AutoZoom = true;
                return;
            }

            ppcPreview.AutoZoom = false;
            ppcPreview.Zoom = int.Parse(level.TrimEnd('%')) / 100.0;
        }

        // ---- printing -------------------------------------------------------------------------

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // The normal Windows print dialog: nothing is sent to a printer unless the user confirms it there.
                using (var dialog = new PrintDialog())
                {
                    dialog.Document = _printer;
                    dialog.AllowSomePages = false;
                    dialog.AllowSelection = false;

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                        return;
                }

                _printer.Print();
            }
            catch (InvalidPrinterException)
            {
                MessageBox.Show(this, "No valid printer is available. Please install or select a printer and try again.",
                    "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(this, "The report could not be printed: " + ex.Message,
                    "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmReportPreview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                Close();
            }
        }
    }
}
