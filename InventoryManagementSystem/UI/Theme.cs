using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryManagementSystem.UI
{
    internal enum ButtonKind
    {
        Primary,        // Add, Update, Save, Record, Confirm, Apply, Search, Refresh
        Secondary,      // Clear, Clear Filters, Remove Item, View Details
        Danger,         // Delete
        DangerOutline   // Cancel Order (destructive, but not the page's main action)
    }

    /// <summary>
    /// Small visual design system built only from standard WinForms controls: palette, typography,
    /// button hierarchy, grid style, empty states and section bands. Pages call Theme.Apply(this).
    /// </summary>
    internal static class Theme
    {
        // ---- palette ---------------------------------------------------------------------------
        public static readonly Color Navy = Color.FromArgb(33, 47, 61);          // application header (unchanged)
        public static readonly Color Primary = Color.FromArgb(0, 120, 215);      // professional blue
        public static readonly Color PrimaryHover = Color.FromArgb(0, 96, 182);
        public static readonly Color TextDark = Color.FromArgb(30, 41, 59);
        public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);
        public static readonly Color Border = Color.FromArgb(203, 213, 225);
        public static readonly Color PageBack = Color.White;
        public static readonly Color Band = Color.FromArgb(241, 245, 249);       // filter bars / section bands
        public static readonly Color RowAlt = Color.FromArgb(248, 250, 252);
        public static readonly Color SelectionBack = Color.FromArgb(219, 234, 254);
        public static readonly Color Success = Color.FromArgb(22, 128, 61);
        public static readonly Color Warning = Color.FromArgb(180, 83, 9);
        public static readonly Color Danger = Color.FromArgb(200, 35, 51);
        public static readonly Color DangerHover = Color.FromArgb(165, 25, 40);
        public static readonly Color DisabledBack = Color.FromArgb(226, 232, 240);
        public static readonly Color DisabledFore = Color.FromArgb(148, 163, 184);

        // ---- typography (Segoe UI throughout) ----------------------------------------------------
        public static readonly Font BaseFont = new Font("Segoe UI", 10F);
        public static readonly Font SemiboldFont = new Font("Segoe UI Semibold", 10F);
        public static readonly Font PageTitleFont = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static readonly Font SectionFont = new Font("Segoe UI", 12F, FontStyle.Bold);

        // ---- entry point -------------------------------------------------------------------------
        public static void Apply(Control root)
        {
            root.BackColor = PageBack;
            root.ForeColor = TextDark;

            foreach (Control c in Descendants(root))
            {
                var button = c as Button;
                if (button != null) { StyleButton(button, Classify(button)); continue; }

                var grid = c as DataGridView;
                if (grid != null) { StyleGrid(grid); continue; }

                var label = c as Label;
                if (label != null && label.Name == "lblHeader")
                {
                    label.Font = PageTitleFont;
                    label.ForeColor = TextDark;
                    continue;
                }

                if (c is Panel && c.Name == "pnlFilter")
                    c.BackColor = Band;
            }

            // After the page has been scaled for the larger font, make sure action buttons never touch or overlap the
            // input above them (rounding in font auto-scaling can leave them flush against multi-line text boxes).
            var page = root as UserControl;
            if (page != null)
                page.Load += (s, e) => { SeparateButtonsFromInputs(page); FitScrollWidth(page); };
        }

        /// <summary>
        /// Fixed-layout pages need a minimum width (the designer's value is in unscaled units and ignores the larger font).
        /// Measure the real right-most edge of the page's laid-out controls and let the page scroll horizontally only if the
        /// window is narrower than that; otherwise no scrollbar appears.
        /// </summary>
        private static void FitScrollWidth(UserControl page)
        {
            int needed = 0;
            foreach (Control c in Descendants(page))
            {
                if (c.Dock != DockStyle.None || !c.Visible || IsInsideGrid(c, page))
                    continue;

                Point topLeft = page.PointToClient(c.Parent.PointToScreen(c.Location));
                needed = Math.Max(needed, topLeft.X + c.Width);
            }

            // Scroll only when something really extends past the visible width; otherwise no scrollbar at all.
            page.AutoScrollMinSize = new Size(needed > page.ClientSize.Width ? needed + 8 : 0, 0);
        }

        private static bool IsInsideGrid(Control c, Control page)
        {
            for (Control p = c.Parent; p != null && p != page; p = p.Parent)
                if (p is DataGridView)
                    return true;
            return c is DataGridView;
        }

        private static void SeparateButtonsFromInputs(Control host)
        {
            var containers = new List<Control> { host };
            containers.AddRange(Descendants(host));

            foreach (Control parent in containers)
            {
                int delta = 0;
                int rowTop = int.MaxValue;

                foreach (Control control in parent.Controls)
                {
                    var button = control as Button;
                    if (button == null || !button.Visible)
                        continue;

                    foreach (Control input in parent.Controls)
                    {
                        if (!(input is TextBoxBase || input is ComboBox || input is DateTimePicker) || !input.Visible)
                            continue;

                        bool overlapsHorizontally = button.Left < input.Right && button.Right > input.Left;
                        if (overlapsHorizontally && button.Top >= input.Top && button.Top < input.Bottom + 8)
                        {
                            delta = Math.Max(delta, input.Bottom + 8 - button.Top);
                            rowTop = Math.Min(rowTop, button.Top);
                        }
                    }
                }

                if (delta <= 0)
                    continue;

                foreach (Control control in parent.Controls)
                    if (control.Top >= rowTop - 2)
                        control.Top += delta;

                if (parent.Dock == DockStyle.Top || parent.Dock == DockStyle.Bottom)
                    parent.Height += delta;
            }
        }

        private static IEnumerable<Control> Descendants(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (Control grandChild in Descendants(child))
                    yield return grandChild;
            }
        }

        // ---- buttons -----------------------------------------------------------------------------
        public static ButtonKind Classify(Button b)
        {
            string t = (b.Text ?? "").Trim();
            if (t == "Delete") return ButtonKind.Danger;
            if (t == "Cancel Order") return ButtonKind.DangerOutline;
            if (t == "Clear" || t == "Clear Filters" || t == "Remove Item" || t == "View Details" || t == "Close" || t == "Cancel")
                return ButtonKind.Secondary;
            return ButtonKind.Primary;
        }

        public static void StyleButton(Button b, ButtonKind kind)
        {
            b.UseVisualStyleBackColor = false;
            b.FlatStyle = FlatStyle.Flat;
            b.Font = SemiboldFont;
            b.Cursor = Cursors.Hand;
            b.FlatAppearance.BorderSize = 1;

            Action<bool> paint = hover =>
            {
                if (!b.Enabled)
                {
                    b.BackColor = DisabledBack; b.ForeColor = DisabledFore; b.FlatAppearance.BorderColor = DisabledBack;
                    return;
                }
                switch (kind)
                {
                    case ButtonKind.Primary:
                        b.BackColor = hover ? PrimaryHover : Primary; b.ForeColor = Color.White; b.FlatAppearance.BorderColor = b.BackColor; break;
                    case ButtonKind.Danger:
                        b.BackColor = hover ? DangerHover : Danger; b.ForeColor = Color.White; b.FlatAppearance.BorderColor = b.BackColor; break;
                    case ButtonKind.DangerOutline:
                        b.BackColor = hover ? Color.FromArgb(254, 226, 226) : Color.White; b.ForeColor = Danger; b.FlatAppearance.BorderColor = Danger; break;
                    default:
                        b.BackColor = hover ? Band : Color.White; b.ForeColor = TextDark; b.FlatAppearance.BorderColor = Border; break;
                }
            };

            paint(false);
            b.MouseEnter += (s, e) => paint(true);
            b.MouseLeave += (s, e) => paint(false);
            b.EnabledChanged += (s, e) => paint(false);
        }

        // ---- grids -------------------------------------------------------------------------------
        public static void StyleGrid(DataGridView g)
        {
            g.BackgroundColor = Color.White;               // no grey "workspace" area under the rows
            g.BorderStyle = BorderStyle.None;
            g.EnableHeadersVisualStyles = false;
            g.RowHeadersVisible = false;
            g.AllowUserToResizeRows = false;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor = Color.FromArgb(226, 232, 240);
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 34;
            g.RowTemplate.Height = 28;

            g.ColumnHeadersDefaultCellStyle.BackColor = Band;
            g.ColumnHeadersDefaultCellStyle.ForeColor = TextDark;
            g.ColumnHeadersDefaultCellStyle.Font = SemiboldFont;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Band;
            g.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextDark;
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            g.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;   // one-line headers, never clipped

            g.DefaultCellStyle.Font = BaseFont;
            g.DefaultCellStyle.BackColor = Color.White;
            g.DefaultCellStyle.ForeColor = TextDark;
            g.DefaultCellStyle.SelectionBackColor = SelectionBack;
            g.DefaultCellStyle.SelectionForeColor = TextDark;
            g.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            g.AlternatingRowsDefaultCellStyle.BackColor = RowAlt;
            g.AlternatingRowsDefaultCellStyle.SelectionBackColor = SelectionBack;
            g.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextDark;

            g.DataBindingComplete += (s, e) => FitColumns(g);
            g.HandleCreated += (s, e) => FitColumns(g);
        }

        /// <summary>
        /// Widens (never narrows) fixed-width columns so their header and the first rows' values are fully visible.
        /// Fill columns keep absorbing the remaining space.
        /// </summary>
        public static void FitColumns(DataGridView g)
        {
            if (g.Columns.Count == 0)
                return;

            int rows = Math.Min(g.Rows.Count, 100);
            foreach (DataGridViewColumn col in g.Columns)
            {
                if (!col.Visible || col.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                    continue;

                int need = TextRenderer.MeasureText(col.HeaderText ?? "", SemiboldFont).Width + 32;
                for (int i = 0; i < rows; i++)
                {
                    object value = g.Rows[i].Cells[col.Index].FormattedValue;
                    if (value != null)
                        need = Math.Max(need, TextRenderer.MeasureText(value.ToString(), BaseFont).Width + 20);
                }

                need = Math.Min(need, 360);
                if (col.Width < need)
                    col.Width = need;
            }
        }

        /// <summary>Shows a clear message inside the grid area while it has no rows (instead of a blank area).</summary>
        public static void AttachEmptyState(DataGridView g, string message)
        {
            var label = new Label
            {
                Text = message,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = TextMuted,
                Font = BaseFont,
                BackColor = Color.White,
                Visible = false
            };
            g.Controls.Add(label);

            Action update = () =>
            {
                bool empty = g.Rows.Count == 0;
                if (empty)
                {
                    int top = g.ColumnHeadersVisible ? g.ColumnHeadersHeight : 0;
                    label.SetBounds(0, top, g.ClientSize.Width, Math.Min(90, Math.Max(40, g.ClientSize.Height - top)));
                    label.BringToFront();
                }
                label.Visible = empty;
            };

            g.DataBindingComplete += (s, e) => update();
            g.RowsAdded += (s, e) => update();
            g.RowsRemoved += (s, e) => update();
            g.SizeChanged += (s, e) => update();
            g.HandleCreated += (s, e) => update();
            update();
        }

        // ---- section bands -----------------------------------------------------------------------
        /// <summary>
        /// Inserts a slim titled band into a stack of Dock=Top panels, directly above <paramref name="below"/>.
        /// Used to make areas such as "History" visually distinct from the entry form above them.
        /// </summary>
        public static Label InsertSectionBand(Control host, Control below, string text)
        {
            var band = new Label
            {
                Name = "band" + text.Replace(" ", ""),
                Text = "  " + text,
                Dock = DockStyle.Top,
                Height = 34,
                Font = SectionFont,
                ForeColor = Color.White,
                BackColor = Navy,
                TextAlign = ContentAlignment.MiddleLeft
            };
            host.Controls.Add(band);
            // Dock=Top controls are stacked by z-order: the control at the higher child index is docked first (nearer the top).
            // Index(below)+1 puts the band directly after `below` in docking order, i.e. just above it (anything that was
            // above `below` shifts up by one and stays above the band).
            host.Controls.SetChildIndex(band, host.Controls.GetChildIndex(below) + 1);
            return band;
        }
    }
}
