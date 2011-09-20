Public Class RowLayoutEngine : Inherits MathElement

    Public Sub New()
    End Sub

    Public Sub New(Children As IEnumerable(Of MathElement))
        Me.New()
        For Each Child In Children
            Me.Children.Add(Child)
        Next
    End Sub

    Protected Overrides Function Clone_Internal(ByRef ShouldCloneChildren As Boolean) As MathElement
        Return New RowLayoutEngine()
    End Function

    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New LayoutEngineChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New RowLayoutEngineExportHelper(Me)
    End Function

    Protected Overrides Function GetInitialInputHelper() As InputHelper
        Return New RowLayoutEngineInputHelper(Me)
    End Function
End Class
