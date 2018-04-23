using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.Data.Filtering;
using DevExpress.XtraGrid;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        private const string TestExpression = "[ID]*2 > ([Number] + 10)";
                private DataTable CreateTable(int RowCount)
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("Name", typeof(string));
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("Number", typeof(int));
            tbl.Columns.Add("Date", typeof(DateTime));
            for (int i = 0; i < RowCount; i++)
                tbl.Rows.Add(new object[] { String.Format("Name{0}", i), i, 3 - i, DateTime.Now.AddDays(i) });
            return tbl;
        }

        ComplexFilterHelper helper;
        public Form1()
        {
            InitializeComponent();
            gridControl1.DataSource = CreateTable(20);
            ((StyleFormatCondition)gridView1.FormatConditions[0]).Expression = TestExpression;
            helper = new ComplexFilterHelper(gridView1);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            gridView1.ShowFilterEditor(helper.FilterColumn);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            helper.ApplyMathFilter(TestExpression);
            CriteriaOperator[] operands = new CriteriaOperator[2];
            operands[0] = gridView1.ActiveFilterCriteria;
            operands[1] = new BinaryOperator("ID", 5, BinaryOperatorType.Greater );
            gridView1.ActiveFilterCriteria = new GroupOperator(GroupOperatorType.And, operands);
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            helper.ShowFilterEditor();
        }
    }

}