Public Class IdentifierTextEdit : Inherits TextEdit

    Public Overrides Function Clone() As MathElement
        Clone = New IdentifierTextEdit()
        For Each C In Children
            Clone.Children.Add(C.Clone())
        Next
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            Return "mi"
        End Get
    End Property
End Class
