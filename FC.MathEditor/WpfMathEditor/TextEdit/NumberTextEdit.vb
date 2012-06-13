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

    Public Overrides Function IsCharAccepted(C As Integer, Position As Integer) As Boolean
        ' TODO: Change this implementation to avoid +++++0.5 and allow 5e-3
        Select Case Position
            Case 0
                If (C = Asc("-"c) OrElse C = Asc("+"c)) Then
                    If Char.IsDigit(Me.FirstChild.Export.ToSimpleText()) Then
                        Return Me.PreviousSibling Is Nothing OrElse TryCast(Me.PreviousSibling, OperatorTextEdit) IsNot Nothing
                    Else
                        Return False
                    End If
                Else
                    Return Char.IsDigit(Char.ConvertFromUtf32(C))
                End If
            Case Else
                If C = Asc("-"c) OrElse C = Asc("+") Then
                    Return Me.Children(Position - 1).Export.ToSimpleText().ToLowerInvariant() = "e"
                ElseIf C = Asc("."c) Then
                    Return Char.IsDigit(Me.Children(Position - 1).Export.ToSimpleText())
                Else
                    Return Char.IsDigit(Char.ConvertFromUtf32(C)) OrElse (C = Asc("e"c) OrElse C = Asc("E"c))
                End If
        End Select
    End Function

    Public Overrides ReadOnly Property EatInputByDefault As Boolean
        Get
            Return True
        End Get
    End Property

End Class
