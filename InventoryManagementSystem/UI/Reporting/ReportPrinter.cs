using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;

namespace InventoryManagementSystem.UI.Reporting
{
    /// <summary>One printed page: which rows it shows and whether the summary block is drawn on it.</summary>
    public class ReportPage
    {
        public ReportPage(int firstRow, int rowCount, bool hasSummary)
        {
            FirstRow = firstRow;
            RowCount = rowCount;
            HasSummary = hasSummary;
        }

        public int FirstRow { get; private set; }
        public int RowCount { get; private set; }
        public bool HasSummary { get; private set; }
    }

    /// <summary>
    /// Renders a <see cref="ReportDocument"/> as printed pages. The same instance feeds the on-screen preview
    /// (PrintPreviewControl) and the real print job, so both always look identical. All layout units are
    /// hundredths of an inch (the PrintDocument default). The class does no database access.
    /// </summary>
    public class ReportPrinter : PrintDocument
    {
        public const int PageMargin = 50;

        // Fixed geometry keeps pagination deterministic: it depends only on the page size and the row count.
        internal const int HeaderHeight = 90;
        internal const int TableHeaderHeight = 32;
        internal const int RowHeight = 22;
        internal const int FooterHeight = 30;
        internal const int SummaryTitleHeight = 26;
        internal const int SummaryLineHeight = 18;
        internal const int SummaryPadding = 12;
        private const int CellPadding = 4;

        private readonly ReportDocument _document;
        private List<ReportPage> _pages;
        private int _pageIndex;

        public ReportPrinter(ReportDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            _document = document;
            DocumentName = document.Title;

            try
            {
                DefaultPageSettings.Landscape = document.Landscape;
                DefaultPageSettings.Margins = new Margins(PageMargin, PageMargin, PageMargin, PageMargin);
            }
            catch (InvalidPrinterException)
            {
                // No printer is installed: the fallback page size in GetMarginBounds() is used instead.
            }
        }

        public ReportDocument Document { get { return _document; } }

        /// <summary>Total number of pages the report needs on the current default page settings (always at least 1).</summary>
        public int PageCount { get { return Paginate(_document, GetMarginBounds()).Count; } }

        /// <summary>Printable area of the default page settings, or a Letter-size fallback when no printer exists.</summary>
        public Rectangle GetMarginBounds()
        {
            int width = 850;
            int height = 1100;
            try
            {
                Rectangle bounds = DefaultPageSettings.Bounds;
                width = bounds.Width;
                height = bounds.Height;
            }
            catch (InvalidPrinterException)
            {
                if (_document.Landscape)
                {
                    width = 1100;
                    height = 850;
                }
            }

            return new Rectangle(PageMargin, PageMargin, width - 2 * PageMargin, height - 2 * PageMargin);
        }

        /// <summary>
        /// Deterministic pagination. Every page holds the same number of rows; the summary goes on the last
        /// page if it fits under the last rows, otherwise on one extra summary-only page. An empty report is one page.
        /// </summary>
        public static List<ReportPage> Paginate(ReportDocument document, Rectangle marginBounds)
        {
            int available = marginBounds.Height - HeaderHeight - TableHeaderHeight - FooterHeight;
            int rowsPerPage = Math.Max(1, available / RowHeight);
            int rowCount = document.Rows.Count;

            var pages = new List<ReportPage>();
            for (int first = 0; first < rowCount; first += rowsPerPage)
                pages.Add(new ReportPage(first, Math.Min(rowsPerPage, rowCount - first), false));

            if (pages.Count == 0)
                pages.Add(new ReportPage(0, 0, false));

            int summaryHeight = SummaryHeight(document);
            if (summaryHeight > 0)
            {
                ReportPage last = pages[pages.Count - 1];
                // An empty page shows a one-row "no records" message above the summary.
                int used = (last.RowCount == 0 ? 1 : last.RowCount) * RowHeight;
                if (used + summaryHeight <= available)
                    pages[pages.Count - 1] = new ReportPage(last.FirstRow, last.RowCount, true);
                else
                    pages.Add(new ReportPage(rowCount, 0, true));
            }

            return pages;
        }

        private static int SummaryHeight(ReportDocument document)
        {
            if (document.SummaryLines.Count == 0)
                return 0;
            return SummaryPadding + SummaryTitleHeight + document.SummaryLines.Count * SummaryLineHeight;
        }

        // ---- PrintDocument events -------------------------------------------------------------

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
            _pageIndex = 0;
            _pages = null;
        }

        protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            base.OnQueryPageSettings(e);
            // A different printer chosen in the print dialog must not change the report's orientation or margins.
            e.PageSettings.Landscape = _document.Landscape;
            e.PageSettings.Margins = new Margins(PageMargin, PageMargin, PageMargin, PageMargin);
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);

            if (_pages == null)
                _pages = Paginate(_document, e.MarginBounds);

            DrawPage(e.Graphics, e.MarginBounds, _pages[_pageIndex], _pageIndex + 1, _pages.Count);

            _pageIndex++;
            e.HasMorePages = _pageIndex < _pages.Count;
        }

        // ---- drawing --------------------------------------------------------------------------

        private void DrawPage(Graphics g, Rectangle bounds, ReportPage page, int pageNumber, int pageCount)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            using (var titleFont = new Font("Segoe UI", 16f, FontStyle.Bold, GraphicsUnit.Point))
            using (var textFont = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point))
            using (var boldFont = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point))
            using (var textBrush = new SolidBrush(Theme.TextDark))
            using (var mutedBrush = new SolidBrush(Theme.TextMuted))
            using (var linePen = new Pen(Theme.Border, 1f))
            using (var accentPen = new Pen(Theme.Primary, 2f))
            {
                DrawHeader(g, bounds, titleFont, textFont, textBrush, mutedBrush, accentPen);

                int y = bounds.Top + HeaderHeight;
                bool showTable = page.RowCount > 0 || (_document.Rows.Count == 0 && pageNumber == 1);

                if (showTable)
                {
                    float[] columnX = ColumnPositions(bounds);
                    DrawTableHeader(g, bounds, columnX, y, boldFont, textBrush, linePen);
                    y += TableHeaderHeight;

                    for (int i = 0; i < page.RowCount; i++)
                    {
                        DrawRow(g, bounds, columnX, y, _document.Rows[page.FirstRow + i], i, textFont, textBrush, linePen);
                        y += RowHeight;
                    }

                    if (_document.Rows.Count == 0)
                    {
                        DrawText(g, ReportDocument.EmptyMessage, textFont, mutedBrush,
                            new RectangleF(bounds.Left + CellPadding, y, bounds.Width, RowHeight), ReportAlign.Left, false);
                        y += RowHeight;
                    }
                }

                if (page.HasSummary)
                    DrawSummary(g, bounds, y + SummaryPadding, boldFont, textFont, textBrush);

                DrawFooter(g, bounds, pageNumber, pageCount, textFont, mutedBrush, linePen);
            }
        }

        private void DrawHeader(Graphics g, Rectangle bounds, Font titleFont, Font textFont, Brush textBrush,
            Brush mutedBrush, Pen accentPen)
        {
            DrawText(g, _document.Title, titleFont, textBrush,
                new RectangleF(bounds.Left, bounds.Top, bounds.Width * 0.65f, 34), ReportAlign.Left, false);
            DrawText(g, "Inventory Management System", textFont, mutedBrush,
                new RectangleF(bounds.Left + bounds.Width * 0.65f, bounds.Top, bounds.Width * 0.35f, 34), ReportAlign.Right, false);

            DrawText(g, "Generated: " + _document.GeneratedAt.ToString("yyyy-MM-dd HH:mm"), textFont, mutedBrush,
                new RectangleF(bounds.Left, bounds.Top + 36, bounds.Width, 18), ReportAlign.Left, false);
            DrawText(g, "Filters: " + _document.FilterDescription, textFont, mutedBrush,
                new RectangleF(bounds.Left, bounds.Top + 54, bounds.Width, 18), ReportAlign.Left, false);

            g.DrawLine(accentPen, bounds.Left, bounds.Top + HeaderHeight - 8, bounds.Right, bounds.Top + HeaderHeight - 8);
        }

        private float[] ColumnPositions(Rectangle bounds)
        {
            int columns = _document.Columns.Count;
            var x = new float[columns + 1];
            float totalWeight = 0;
            foreach (ReportColumn c in _document.Columns)
                totalWeight += Math.Max(1, c.Width);

            float position = bounds.Left;
            for (int i = 0; i < columns; i++)
            {
                x[i] = position;
                position += bounds.Width * Math.Max(1, _document.Columns[i].Width) / totalWeight;
            }
            x[columns] = bounds.Right;   // last column ends exactly on the margin (no rounding drift)
            return x;
        }

        private void DrawTableHeader(Graphics g, Rectangle bounds, float[] columnX, int y, Font boldFont, Brush textBrush,
            Pen linePen)
        {
            using (var back = new SolidBrush(Theme.Band))
                g.FillRectangle(back, bounds.Left, y, bounds.Width, TableHeaderHeight);

            for (int i = 0; i < _document.Columns.Count; i++)
            {
                var cell = new RectangleF(columnX[i] + CellPadding, y, columnX[i + 1] - columnX[i] - 2 * CellPadding, TableHeaderHeight);
                DrawText(g, _document.Columns[i].Header, boldFont, textBrush, cell, _document.Columns[i].Align, true);
            }

            g.DrawLine(linePen, bounds.Left, y + TableHeaderHeight, bounds.Right, y + TableHeaderHeight);
        }

        private void DrawRow(Graphics g, Rectangle bounds, float[] columnX, int y, string[] cells, int rowOnPage,
            Font textFont, Brush textBrush, Pen linePen)
        {
            if (rowOnPage % 2 == 1)
                using (var alt = new SolidBrush(Theme.RowAlt))
                    g.FillRectangle(alt, bounds.Left, y, bounds.Width, RowHeight);

            for (int i = 0; i < _document.Columns.Count && i < cells.Length; i++)
            {
                var cell = new RectangleF(columnX[i] + CellPadding, y, columnX[i + 1] - columnX[i] - 2 * CellPadding, RowHeight);
                DrawText(g, cells[i], textFont, textBrush, cell, _document.Columns[i].Align, false);
            }

            g.DrawLine(linePen, bounds.Left, y + RowHeight, bounds.Right, y + RowHeight);
        }

        private void DrawSummary(Graphics g, Rectangle bounds, int y, Font boldFont, Font textFont, Brush textBrush)
        {
            DrawText(g, "Summary", boldFont, textBrush,
                new RectangleF(bounds.Left, y, bounds.Width, SummaryTitleHeight), ReportAlign.Left, false);
            y += SummaryTitleHeight;

            foreach (string line in _document.SummaryLines)
            {
                DrawText(g, line, textFont, textBrush,
                    new RectangleF(bounds.Left + CellPadding, y, bounds.Width, SummaryLineHeight), ReportAlign.Left, false);
                y += SummaryLineHeight;
            }
        }

        private void DrawFooter(Graphics g, Rectangle bounds, int pageNumber, int pageCount, Font textFont, Brush mutedBrush,
            Pen linePen)
        {
            int top = bounds.Bottom - FooterHeight + 6;
            g.DrawLine(linePen, bounds.Left, top, bounds.Right, top);
            DrawText(g, _document.Title, textFont, mutedBrush,
                new RectangleF(bounds.Left, top + 4, bounds.Width * 0.6f, 18), ReportAlign.Left, false);
            DrawText(g, "Page " + pageNumber + " of " + pageCount, textFont, mutedBrush,
                new RectangleF(bounds.Left + bounds.Width * 0.6f, top + 4, bounds.Width * 0.4f, 18), ReportAlign.Right, false);
        }

        // Text never leaves its cell: no wrapping (except table headers, limited to whole lines) and an ellipsis on overflow.
        private static void DrawText(Graphics g, string text, Font font, Brush brush, RectangleF cell, ReportAlign align, bool wrap)
        {
            if (string.IsNullOrEmpty(text))
                return;

            using (var format = new StringFormat(wrap ? StringFormatFlags.LineLimit : StringFormatFlags.NoWrap | StringFormatFlags.LineLimit))
            {
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = align == ReportAlign.Right ? StringAlignment.Far : StringAlignment.Near;
                g.DrawString(text, font, brush, cell, format);
            }
        }
    }
}
