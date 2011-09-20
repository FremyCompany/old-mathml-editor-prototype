Public Class NumberTextEdit : Inherits TextEdit

    Public Sub New()
        Me.Input = New TextEditInputHelper(Me)
    End Sub

    Public Overrides Function TryGetDefaultAttribute(AttributeName As String, ByRef Result As String) As Boolean
        Select Case AttributeName
            Case "fontfamily"
                Result = "Cambria" : Return True
            Case Else
                Return MyBase.TryGetDefaultAttribute(AttributeName, Result)
        End Select
    End Function

    Protected Overrides Function Clone_Internal(ByRef ShouldCloneChildren As Boolean) As MathElement
        Return New NumberTextEdit()
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            ' TODO: Check MathML
            Return "mn"
        End Get
    End Property

    Public Overrides Function IsAccepted(C As Integer, IsFirst As Boolean) As Boolean
        Return Char.IsDigit(Char.ConvertFromUtf32(C)) OrElse (IsFirst AndAlso (C = Asc("-"c) OrElse C = Asc("+"c))) OrElse (Not IsFirst AndAlso (C = Asc("."c) OrElse C = Asc("e"c) OrElse C = Asc("E"c)))
    End Function

    Public Overrides ReadOnly Property EatInputByDefault As Boolean
        Get
            Return True
        End Get
    End Property

End Class
