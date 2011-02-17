Public Class IdentifierTextEdit : Inherits TextEdit

    Public Shared Shadows DefaultFontStyle As FontStyle = FontStyles.Italic
    Public Sub New()
        Me.FontStyle = DefaultFontStyle
    End Sub

    Public Sub New(ByVal Children As IEnumerable(Of MathElement))
        Call Me.New()
        For Each C In Children
            Me.Children.Add(C)
        Next
    End Sub

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
