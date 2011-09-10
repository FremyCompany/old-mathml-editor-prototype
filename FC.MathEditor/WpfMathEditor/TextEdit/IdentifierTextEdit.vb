Public Class IdentifierTextEdit : Inherits TextEdit

    Public Sub New()
        ' Do nothing
    End Sub

    Public Sub New(Children As IEnumerable(Of MathElement))
        Call Me.New()
        For Each C In Children
            Me.Children.Add(C)
        Next
    End Sub

    Public Overrides Function TryGetDefaultAttribute(AttributeName As String, ByRef Result As String) As Boolean
        Select Case AttributeName
            Case "fontstyle"
                ' Default style for a single char identifier is "italic"
                If Children.HasMany Then
                    Result = "normal"
                Else
                    Result = "italic"
                End If
                Return True
            Case Else
                Return MyBase.TryGetDefaultAttribute(AttributeName, Result)
        End Select
    End Function

    Public Overrides Function Clone_Internal() As MathElement
        Return New IdentifierTextEdit()
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
