Public Class OperatorTextEdit : Inherits TextEdit

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
        Dim Clone As New OperatorTextEdit()
        For Each C In Children
            Clone.Children.Add(C.Clone())
        Next : Return Clone
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            Return "mo"
        End Get
    End Property

    Public Overrides Function IsAccepted(ByVal C As Integer) As Boolean
        '? Can this really be done? I mean, is it a good idea to disallow this here?
        Return Not (Children.HasAny OrElse Char.IsLetter(Char.ConvertFromUtf32(C)))
    End Function

End Class
