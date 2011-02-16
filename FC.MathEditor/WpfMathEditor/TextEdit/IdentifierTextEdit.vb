Public Class IdentifierTextEdit : Inherits TextEdit

    Public Overrides Function Clone_Internal() As MathElement
        Dim Clone As New IdentifierTextEdit()
        For Each C In Children
            Clone.Children.Add(C.Clone())
        Next : Return Clone
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            Return "mi"
        End Get
    End Property
End Class
