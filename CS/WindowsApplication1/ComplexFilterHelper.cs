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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors;

namespace WindowsApplication1
{
    public class ComplexFilterHelper
    {
        private const string DefaultFieldName = "MathematicCondition";
        private GridColumn _FilterColumn;
        private GridView _View;
        public GridColumn FilterColumn
        {
            get { return _FilterColumn; }
        }

        private CriteriaOperator _LastOperator;
        public CriteriaOperator LastOperator
        {
            get { return _LastOperator; }
        }
        private RepositoryItem _FilterEditorRepositoryItem;
        public RepositoryItem FilterEditorRepositoryItem
        {
            get { return _FilterEditorRepositoryItem; }
            set { _FilterEditorRepositoryItem = value; }
        }

        public ComplexFilterHelper(GridView view)
        {
            _View = view;
            _View.CustomFilterDisplayText += _View_CustomFilterDisplayText;
            _View.FilterEditorCreated += _View_FilterEditorCreated;
            InitDefaultFilterEditorRepositoryItem();
            InitFilterColumn();
        }

        private void InitDefaultFilterEditorRepositoryItem()
        {
            RepositoryItemButtonEdit riBE = new RepositoryItemButtonEdit();
            riBE.CustomDisplayText += riBE_CustomDisplayText;
            riBE.Buttons[0].Kind = ButtonPredefines.Glyph;
            riBE.Buttons[0].Caption = "Edit expression";
            riBE.Enter += riBE_Enter;
            FilterEditorRepositoryItem = riBE;
            riBE.ButtonClick += riBE_ButtonClick;
        }

        void riBE_Enter(object sender, EventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            editor.Properties.TextEditStyle = TextEditStyles.HideTextEditor;
            ShowFilterEditor();
            editor.EditValue = "True";
        }

        void riBE_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ShowFilterEditor();
        }

        void riBE_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            e.DisplayText = FilterColumn.UnboundExpression;
        }

        void _View_FilterEditorCreated(object sender, FilterControlEventArgs e)
        {
            e.FilterControl.BeforeShowValueEditor += FilterControl_BeforeShowValueEditor;
        }

        void FilterControl_BeforeShowValueEditor(object sender, ShowValueEditorEventArgs e)
        {
            if (e.CurrentNode.FirstOperand.PropertyName != DefaultFieldName) return;
            e.CustomRepositoryItem = FilterEditorRepositoryItem;
        }

        void _View_CustomFilterDisplayText(object sender, ConvertEditValueEventArgs e)
        {
            if (!ReferenceEquals(LastOperator, null))
            {
                e.Value = e.Value.ToString().Replace(LastOperator.ToString(), FilterColumn.UnboundExpression);
                e.Handled = true;
            }
        }

        private void InitFilterColumn()
        {
            _FilterColumn = _View.Columns.AddField(DefaultFieldName);
            _FilterColumn.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            _FilterColumn.ColumnEdit = FilterEditorRepositoryItem;
        }

        public void ShowFilterEditor()
        {
            ShowUnboundExpressionEditor(_FilterColumn);
        }

        public void ApplyMathFilter(string expression)
        {
            ApplyFilter(FilterColumn, expression);
        }


        private void ApplyFilter(GridColumn column, string expression)
        {
            column.UnboundExpression = expression;
            _View.ActiveFilterCriteria = new BinaryOperator(FilterColumn.FieldName, true);
            _LastOperator = _View.ActiveFilterCriteria;
        }

        protected virtual void ShowUnboundExpressionEditor(GridColumn column)
        {
            using (ExpressionEditorForm form = new UnboundColumnExpressionEditorForm(column, null))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ApplyFilter(column, form.Expression);
                }
            }
        }
    }
}
