using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using D3TypeDescriptor;

using System.Runtime.InteropServices;

namespace StructCompare
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GUI.button1_click(this);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;

            var x = GUI.listToLView.Find(
                (w1) =>
                {
                    return w1.old_litem == dataGridView1.SelectedRows[0].Index;
                }
            );

            if (x == null)
                return;

            GUI.tofieldlist((StructureTypeDescriptor)x.new_item, dataGridView3, checkBox2.Checked);
            GUI.tofieldlist((StructureTypeDescriptor)x.old_item, dataGridView4, checkBox2.Checked);
        }

        private void dataGridView3_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (dataGridView3.FirstDisplayedScrollingRowIndex !=
                    dataGridView4.FirstDisplayedScrollingRowIndex)
                    dataGridView4.FirstDisplayedScrollingRowIndex =
                        dataGridView3.FirstDisplayedScrollingRowIndex;
            }
            catch (Exception ex)
            {
            }
        }

        private void dataGridView4_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (dataGridView3.FirstDisplayedScrollingRowIndex !=
                dataGridView4.FirstDisplayedScrollingRowIndex)
                    dataGridView3.FirstDisplayedScrollingRowIndex =
                        dataGridView4.FirstDisplayedScrollingRowIndex;
            }
            catch (Exception ex)
            {
            }
        }

        private void dataGridView2_Scroll(object sender, ScrollEventArgs e)
        {
            if (dataGridView1.FirstDisplayedScrollingRowIndex !=
                dataGridView2.FirstDisplayedScrollingRowIndex)
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView2.FirstDisplayedScrollingRowIndex;
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (dataGridView1.FirstDisplayedScrollingRowIndex !=
                dataGridView2.FirstDisplayedScrollingRowIndex)
                dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex;
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
             string l = dataGridView3.CurrentCell.Value.ToString();
             if (l.IndexOf("DT") == -1)
             {
                  MessageBox.Show("hi");
             }
        }

        ListToLView selectedrow = null;
        List<int> navigation = new List<int>();

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
                return;

            if (e.ColumnIndex == 2)
            {
                dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            }
            var x = GUI.listToLView.Find(
                (w1) =>
                {
                    return w1.old_litem == e.RowIndex;
                }
            );

            if (x == null)
                return;

            GUI.tofieldlist((StructureTypeDescriptor)x.new_item, dataGridView3, checkBox2.Checked);
            GUI.tofieldlist((StructureTypeDescriptor)x.old_item, dataGridView4, checkBox2.Checked);

            selectedrow = x;
            navigation.Add(x.new_litem);
        }

        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            if (dataGridView2.SelectedCells.Count == 0)
                return;

            if (e.ColumnIndex == 2)
            {
                dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter;
            }
            var x = GUI.listToLView.Find(
                (w1) =>
                {
                    return w1.old_litem == e.RowIndex;
                }
            );

            if (x == null)
                return;

            GUI.tofieldlist((StructureTypeDescriptor)x.new_item, dataGridView3, checkBox2.Checked);
            GUI.tofieldlist((StructureTypeDescriptor)x.old_item, dataGridView4, checkBox2.Checked);

            selectedrow = x;
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {

            var x = GUI.listToLView.Single((o1) => { return o1.new_litem == e.RowIndex; });
            if (x == null)
                return;
            Color color = Color.Orange;
            if (x.compactivility == 1)
                color = Color.LightGreen;
            if (x.new_item == null)
                 color = Color.Red; //DataGridViewCellEventArgs
            if (x.old_item == null)  //e.ColumnIndex == dataGridView1.Columns["checkBox_Done"].Index
                color = Color.DarkCyan;

            bool selected = Convert.ToBoolean(dataGridView1.Rows[e.RowIndex].Cells["checkBox_Done"].Value);
            if (selected) 
                 color = Color.LightGray;

            e.PaintParts &= ~(DataGridViewPaintParts.Background);

            // Calculate the bounds of the row.
            Rectangle rowBounds = new Rectangle(
                0, e.RowBounds.Top,
                this.dataGridView1.Columns.GetColumnsWidth(
                    DataGridViewElementStates.Visible) -
                this.dataGridView1.HorizontalScrollingOffset + 1,
                e.RowBounds.Height);

            // Paint the custom selection background.
            using (Brush backbrush =
                new System.Drawing.SolidBrush(color))
            {
                e.Graphics.FillRectangle(backbrush, rowBounds);
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            GUI.transfer_names();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (GUI.listToLView != null)
                if (GUI.listToLView.Count > 0)
                    GUI.Save(checkBox1.Checked);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel2.Dock = DockStyle.Fill;

            dataGridView1.Scroll += dataGridView1_Scroll;
            dataGridView1.RowPrePaint += dataGridView1_RowPrePaint;
            dataGridView1.CellEnter += dataGridView1_RowEnter;

            dataGridView2.Scroll += dataGridView2_Scroll;
            dataGridView2.RowPrePaint += dataGridView1_RowPrePaint;
            dataGridView2.CellEnter += dataGridView2_RowEnter;

            dataGridView3.Scroll += dataGridView3_Scroll;
            dataGridView4.Scroll += dataGridView4_Scroll;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;

            var x = GUI.listToLView.Single((o1) =>
                {
                    return o1.new_litem == e.RowIndex;
                }
            );
            if (x == null)
                return;
            if (x.new_item == null)
                return;
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                x.new_item.CustomName = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        private void dataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;
            if (selectedrow == null)
                return;
            if (selectedrow.new_item == null)
                return;
            if (e.RowIndex >= selectedrow.new_item.Fields.Length)
                return;

            if (dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                selectedrow.new_item.Fields[e.RowIndex].CustomName = null;
                return;
            }
            selectedrow.new_item.Fields[e.RowIndex].CustomName = dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var text = textBox1.Text.ToLower();
            var x = GUI.listToLView.Where(
                (w1) =>
                {
                    if (w1.new_item == null)
                        return false;
                    var n = w1.new_item._Name.ToLower();
                    return (n.Contains(text) || (n == text));
                }
            ).ToArray();
            if (x.Length == 0)
                return;

            int i = 0;
            for (int j = 0; j < x.Length; j++)
            {
                if (dataGridView1.CurrentCell.RowIndex < x[j].new_litem)
                {
                    i = j;
                    break;
                }
            }
            dataGridView1.FirstDisplayedScrollingRowIndex = x[i].new_litem;
            dataGridView1.Refresh();
            dataGridView1.CurrentCell = dataGridView1.Rows[x[i].new_litem].Cells[0];
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button4_Click(this, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (var x in GUI.listToLView)
            {
                if (x.new_item == null)
                    continue;
                var serializeds = from f1 in x.new_item.Fields
                                  where ((f1.Type != null) && (f1.Type.Name == "SerializeData"))
                                  select f1;
                var arrays = from f1 in x.new_item.Fields
                             where ((f1.Type != null) && (f1.VariableOffset != 0) && (f1.Type.Name != "DT_TAGMAP"))
                             select f1;
                for (int i = 0; i < arrays.Count(); i++)
                {
                    if (!(string.IsNullOrEmpty(arrays.ElementAt(i).CustomName)))
                        continue;
                    for (int j = 0; j < serializeds.Count(); j++)
                    {
                        if (arrays.ElementAt(i).Offset + arrays.ElementAt(i).VariableOffset
                            ==
                            serializeds.ElementAt(j).Offset)
                        {
                            arrays.ElementAt(i).CustomName = serializeds.ElementAt(j).Name.Replace("ser", "");
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
             if (e.KeyCode == Keys.Back)
             {
                  if (navigation.Count <= 1)
                       return;

                  navigation.RemoveAt(navigation.Count - 1);
                  int members = navigation.Count - 1;



                  var x = GUI.listToLView.Find(
                      (w1) =>
                      {
                           return w1.old_litem == navigation[members];
                      }
                  );

                  if (x == null)
                       return;

                  dataGridView1.Rows[navigation[members]].Selected = true;
                  if (dataGridView1.Rows[navigation[members]].Displayed == false)
                  {
                       dataGridView1.FirstDisplayedScrollingRowIndex = navigation[members];
                  }

                  GUI.tofieldlist((StructureTypeDescriptor)x.new_item, dataGridView3, checkBox2.Checked);
                  GUI.tofieldlist((StructureTypeDescriptor)x.old_item, dataGridView4, checkBox2.Checked);

                  selectedrow = x;
             }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
