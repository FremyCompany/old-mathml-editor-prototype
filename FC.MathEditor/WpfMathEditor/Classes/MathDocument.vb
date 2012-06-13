Public Class MathDocument : Inherits RowLayoutEngine

    Public Sub New()
        MyBase.New() : Sel = New SelectionHelper(Me)
    End Sub

    Public Property LayoutStyle As LayoutModes = MathEditor.LayoutModes.Block

    Protected Overrides Function Clone_Internal(ByRef ShouldCloneChildren As Boolean) As MathElement
        Return New MathDocument()
    End Function

    Private WithEvents Sel As SelectionHelper
    Public Overloads ReadOnly Property Selection As SelectionHelper
        Get
            Return Sel
        End Get
    End Property

    Public ReadOnly Property CurrentInput As InputHelper
        Get
            Return Input.CurrentInput
        End Get
    End Property

    Public Event SelectionChanged As EventHandler
    Public Event SelectionChanging As EventHandler(Of SelectionHelper.SelectionChangedEventArgs)

    Private Sub Sel_Changed(sender As Object, e As System.EventArgs) Handles Sel.Changed
        RaiseEvent SelectionChanged(Me, e)
    End Sub

    Private Sub Sel_Changing(sender As Object, e As SelectionHelper.SelectionChangedEventArgs) Handles Sel.Changing
        RaiseEvent SelectionChanging(Me, e)
    End Sub
End Class
