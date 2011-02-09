Public Class RowLayoutEngine : Inherits MathElement

    Public Sub New()
        Me._Children = New LayoutEngineChildrenHelper(Me)
        ' Do nothing
    End Sub

    Public Sub New(ByVal Children As IEnumerable(Of MathElement))
        For Each Child In Children
            Me.Children.Add(Child)
        Next
    End Sub

    Public Overrides Function Clone() As MathElement
        Clone = New RowLayoutEngine()

        For Each Child In Children
            Clone.AddChild(Child.Clone())
        Next

        Return Clone
    End Function

End Class
