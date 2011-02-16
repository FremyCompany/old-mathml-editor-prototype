Public Class MathDocument : Inherits MathElement

    Public Overrides Function Clone_Internal() As MathElement
        Dim X As New MathDocument()

        For Each Child In Me.Children
            X.AddChild(Child.Clone())
        Next

        Return X
    End Function

    Private Sel As New SelectionHelper(Me)
    Public Overloads ReadOnly Property Selection As SelectionHelper
        Get
            Return Sel
        End Get
    End Property

End Class
