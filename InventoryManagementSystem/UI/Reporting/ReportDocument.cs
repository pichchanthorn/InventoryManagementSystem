using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.UI.Reporting
{
    public enum ReportAlign
    {
        Left,
        Right
    }

    /// <summary>One table column. Width is a relative weight; the printer scales all weights to the page width.</summary>
    public class ReportColumn
    {
        public ReportColumn(string header, int width, ReportAlign align = ReportAlign.Left)
        {
            Header = header;
            Width = width;
            Align = align;
        }

        public string Header { get; private set; }
        public int Width { get; private set; }
        public ReportAlign Align { get; private set; }
    }

    /// <summary>
    /// Presentation model for one printable report. It holds already-formatted text only, is built from the
    /// BLL report results and is independent of database entities and of any drawing code.
    /// </summary>
    public class ReportDocument
    {
        public const string EmptyMessage = "No records found for the selected filters.";

        public ReportDocument(string title, DateTime generatedAt, string filterDescription, bool landscape)
        {
            Title = title;
            GeneratedAt = generatedAt;
            FilterDescription = filterDescription;
            Landscape = landscape;
            Columns = new List<ReportColumn>();
            Rows = new List<string[]>();
            SummaryLines = new List<string>();
        }

        public string Title { get; private set; }
        public DateTime GeneratedAt { get; private set; }
        public string FilterDescription { get; private set; }

        /// <summary>Page configuration: landscape for wide reports, portrait otherwise.</summary>
        public bool Landscape { get; private set; }

        public List<ReportColumn> Columns { get; private set; }
        public List<string[]> Rows { get; private set; }
        public List<string> SummaryLines { get; private set; }
    }
}
