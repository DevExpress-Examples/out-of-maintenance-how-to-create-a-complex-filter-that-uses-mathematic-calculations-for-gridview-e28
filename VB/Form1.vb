Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraEditors.Design
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.Data.Filtering
Imports DevExpress.XtraGrid

Namespace WindowsApplication1
	Partial Public Class Form1
		Inherits Form
		Private Const TestExpression As String = "[ID]*2 > ([Number] + 10)"
				Private Function CreateTable(ByVal RowCount As Integer) As DataTable
			Dim tbl As New DataTable()
			tbl.Columns.Add("Name", GetType(String))
			tbl.Columns.Add("ID", GetType(Integer))
			tbl.Columns.Add("Number", GetType(Integer))
			tbl.Columns.Add("Date", GetType(DateTime))
			For i As Integer = 0 To RowCount - 1
				tbl.Rows.Add(New Object() { String.Format("Name{0}", i), i, 3 - i, DateTime.Now.AddDays(i) })
			Next i
			Return tbl
				End Function

		Private helper As ComplexFilterHelper
		Public Sub New()
			InitializeComponent()
			gridControl1.DataSource = CreateTable(20)
			CType(gridView1.FormatConditions(0), StyleFormatCondition).Expression = TestExpression
			helper = New ComplexFilterHelper(gridView1)
		End Sub

		Private Sub simpleButton1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton1.Click
			gridView1.ShowFilterEditor(helper.FilterColumn)
		End Sub

		Private Sub simpleButton2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton2.Click
			helper.ApplyMathFilter(TestExpression)
			Dim operands(1) As CriteriaOperator
			operands(0) = gridView1.ActiveFilterCriteria
			operands(1) = New BinaryOperator("ID", 5, BinaryOperatorType.Greater)
			gridView1.ActiveFilterCriteria = New GroupOperator(GroupOperatorType.And, operands)
		End Sub

		Private Sub simpleButton3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton3.Click
			helper.ShowFilterEditor()
		End Sub
	End Class

End Namespace