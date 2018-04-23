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
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraEditors.Filtering
Imports DevExpress.XtraEditors

Namespace WindowsApplication1
	Public Class ComplexFilterHelper
		Private Const DefaultFieldName As String = "MathematicCondition"
		Private _FilterColumn As GridColumn
		Private _View As GridView
		Public ReadOnly Property FilterColumn() As GridColumn
			Get
				Return _FilterColumn
			End Get
		End Property

		Private _LastOperator As CriteriaOperator
		Public ReadOnly Property LastOperator() As CriteriaOperator
			Get
				Return _LastOperator
			End Get
		End Property
		Private _FilterEditorRepositoryItem As RepositoryItem
		Public Property FilterEditorRepositoryItem() As RepositoryItem
			Get
				Return _FilterEditorRepositoryItem
			End Get
			Set(ByVal value As RepositoryItem)
				_FilterEditorRepositoryItem = value
			End Set
		End Property

		Public Sub New(ByVal view As GridView)
			_View = view
			AddHandler _View.CustomFilterDisplayText, AddressOf _View_CustomFilterDisplayText
			AddHandler _View.FilterEditorCreated, AddressOf _View_FilterEditorCreated
			InitDefaultFilterEditorRepositoryItem()
			InitFilterColumn()
		End Sub

		Private Sub InitDefaultFilterEditorRepositoryItem()
			Dim riBE As New RepositoryItemButtonEdit()
			AddHandler riBE.CustomDisplayText, AddressOf riBE_CustomDisplayText
			riBE.Buttons(0).Kind = ButtonPredefines.Glyph
			riBE.Buttons(0).Caption = "Edit expression"
			AddHandler riBE.Enter, AddressOf riBE_Enter
			FilterEditorRepositoryItem = riBE
			AddHandler riBE.ButtonClick, AddressOf riBE_ButtonClick
		End Sub

		Private Sub riBE_Enter(ByVal sender As Object, ByVal e As EventArgs)
			Dim editor As ButtonEdit = TryCast(sender, ButtonEdit)
			editor.Properties.TextEditStyle = TextEditStyles.HideTextEditor
			ShowFilterEditor()
			editor.EditValue = "True"
		End Sub

		Private Sub riBE_ButtonClick(ByVal sender As Object, ByVal e As ButtonPressedEventArgs)
			ShowFilterEditor()
		End Sub

		Private Sub riBE_CustomDisplayText(ByVal sender As Object, ByVal e As CustomDisplayTextEventArgs)
			e.DisplayText = FilterColumn.UnboundExpression
		End Sub

		Private Sub _View_FilterEditorCreated(ByVal sender As Object, ByVal e As FilterControlEventArgs)
			AddHandler e.FilterControl.BeforeShowValueEditor, AddressOf FilterControl_BeforeShowValueEditor
		End Sub

		Private Sub FilterControl_BeforeShowValueEditor(ByVal sender As Object, ByVal e As ShowValueEditorEventArgs)
			If e.CurrentNode.FirstOperand.PropertyName <> DefaultFieldName Then
				Return
			End If
			e.CustomRepositoryItem = FilterEditorRepositoryItem
		End Sub

		Private Sub _View_CustomFilterDisplayText(ByVal sender As Object, ByVal e As ConvertEditValueEventArgs)
			If (Not ReferenceEquals(LastOperator, Nothing)) Then
				e.Value = e.Value.ToString().Replace(LastOperator.ToString(), FilterColumn.UnboundExpression)
				e.Handled = True
			End If
		End Sub

		Private Sub InitFilterColumn()
			_FilterColumn = _View.Columns.AddField(DefaultFieldName)
			_FilterColumn.UnboundType = DevExpress.Data.UnboundColumnType.Boolean
			_FilterColumn.ColumnEdit = FilterEditorRepositoryItem
		End Sub

		Public Sub ShowFilterEditor()
			ShowUnboundExpressionEditor(_FilterColumn)
		End Sub

		Public Sub ApplyMathFilter(ByVal expression As String)
			ApplyFilter(FilterColumn, expression)
		End Sub


		Private Sub ApplyFilter(ByVal column As GridColumn, ByVal expression As String)
			column.UnboundExpression = expression
			_View.ActiveFilterCriteria = New BinaryOperator(FilterColumn.FieldName, True)
			_LastOperator = _View.ActiveFilterCriteria
		End Sub

		Protected Overridable Sub ShowUnboundExpressionEditor(ByVal column As GridColumn)
			Using form As ExpressionEditorForm = New UnboundColumnExpressionEditorForm(column, Nothing)
				form.StartPosition = FormStartPosition.CenterParent
				If form.ShowDialog() = DialogResult.OK Then
					ApplyFilter(column, form.Expression)
				End If
			End Using
		End Sub
	End Class
End Namespace
