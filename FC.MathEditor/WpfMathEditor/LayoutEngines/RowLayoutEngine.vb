Public Class RowLayoutEngine : Inherits MathElement

    Public Sub New()
        Me._Children = New LayoutEngineChildrenHelper(Me)
        Me.Export = New RowLayoutEngineExportHelper(Me)
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

End Class
