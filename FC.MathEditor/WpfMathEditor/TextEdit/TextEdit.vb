Public MustInherit Class TextEdit : Inherits MathElement

    Public MustOverride ReadOnly Property ElementName As String

    Public MustOverride Function CanHaveMultipleChild() As Boolean
    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New TextEditChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New TextEditExportHelper(Me)
    End Function

    Public MustOverride Function IsAccepted(C As Integer) As Boolean
    Protected Overrides Function GetInitialInputHelper() As InputHelper
        Return New TextEditInputHelper(Me, AddressOf IsAccepted)
    End Function

    Public Shared Function FromChar(ByVal InputChar As Integer, Optional ByVal This As MathElement = Nothing) As TextEdit

        Dim TextEdit As TextEdit

        ' Guess the correct text edit from the context
        If Char.IsLetter(Char.ConvertFromUtf32(InputChar)) Then
            TextEdit = New IdentifierTextEdit()
        ElseIf _
        ( _
            (Char.IsDigit(Char.ConvertFromUtf32(InputChar))) OrElse
            (InputChar = Asc(".")) OrElse
            ((InputChar = Asc("-") OrElse InputChar = Asc("+")) AndAlso (This IsNot Nothing) AndAlso (This.Selection IsNot Nothing) AndAlso TryCast(This.Selection.PreviousSibling, OperatorTextEdit) IsNot Nothing) _
        ) Then
            TextEdit = New NumberTextEdit()
        Else
            TextEdit = New OperatorTextEdit()
        End If

        ' Append the newly created char to the textedit
        Dim X As New UnicodeGlyph(InputChar)
        TextEdit.AddChild(X)

        Return TextEdit

    End Function

End Class
