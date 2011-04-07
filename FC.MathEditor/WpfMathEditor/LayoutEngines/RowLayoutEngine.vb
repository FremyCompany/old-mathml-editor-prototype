Public Class RowLayoutEngine : Inherits MathElement

    Public Sub New()
    End Sub

    Public Sub New(ByVal Children As IEnumerable(Of MathElement))
        Me.New()
        For Each Child In Children
            Me.Children.Add(Child)
        Next
    End Sub

    Public Overrides Function Clone_Internal() As MathElement
        Dim Clone As New RowLayoutEngine()
        For Each Child In Children
            Clone.AddChild(Child.Clone())
        Next : Return Clone
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
