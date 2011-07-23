Public Class IdentifierTextEdit : Inherits TextEdit

    Public Shared Shadows DefaultFontStyle As FontStyle = FontStyles.Italic
    Public Sub New()
        Me.FontStyle = DefaultFontStyle
    End Sub

    Public Sub New(Children As IEnumerable(Of MathElement))
        Call Me.New()
        For Each C In Children
            Me.Children.Add(C)
        Next
    End Sub

    Public Overrides Function Clone_Internal(Optional ByVal CloneChildren As Boolean = True) As MathElement
        Dim Clone As New IdentifierTextEdit()

        If CloneChildren Then
            For Each C In Children
                Clone.Children.Add(C.Clone())
            Next
        End If

        Return Clone
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            Return "mi"
        End Get
    End Property

    Public Overrides Function IsAccepted(C As Integer, IsFirst As Boolean) As Boolean
        Return Char.IsLetter(Char.ConvertFromUtf32(C)) OrElse (Not IsFirst AndAlso C = Asc("_"))
    End Function

    ''' <summary>
    ''' Gets a value indicating if an accepted char typed from left or right should be added to the textedit or if another one should be created instead.
    ''' </summary>
    Public Overrides ReadOnly Property EatInputByDefault As Boolean
        Get
            Return False
        End Get
    End Property

End Class
