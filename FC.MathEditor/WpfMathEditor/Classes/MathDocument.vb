Public Class MathDocument : Inherits RowLayoutEngine

    Public Sub New()
        MyBase.New() : Sel = New SelectionHelper(Me)
    End Sub

    Public Overrides Function Clone_Internal() As MathElement
        Dim X As New MathDocument()

        For Each Child In Me.Children
            X.AddChild(Child.Clone())
        Next

        Return X
    End Function

    Private Sel As SelectionHelper
    Public Overloads ReadOnly Property Selection As SelectionHelper
        Get
            Return Sel
        End Get
    End Property

End Class
