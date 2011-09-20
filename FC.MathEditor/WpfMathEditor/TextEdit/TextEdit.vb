Public MustInherit Class TextEdit : Inherits MathElement

    Public MustOverride ReadOnly Property ElementName As String

    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New TextEditChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New TextEditExportHelper(Me)
    End Function

    Public MustOverride Function IsAccepted(C As Integer, IsFirst As Boolean) As Boolean
    Protected Overrides Function GetInitialInputHelper() As InputHelper
        Return New TextEditInputHelper(Me)
    End Function

    ''' <summary>
    ''' Gets a value indicating if an accepted char typed from left or right should be added to the textedit or if another one should be created instead.
    ''' </summary>
    ''' <remarks>This property has no effect on content typed between glyphs already in the textedit</remarks>
    Public MustOverride ReadOnly Property EatInputByDefault() As Boolean

    Public Shared Function FromChar(InputChar As Integer, Optional This As MathElement = Nothing) As TextEdit

        Dim TextEdit As TextEdit

        ' Guess the correct text edit from the context
        If Char.IsLetter(Char.ConvertFromUtf32(InputChar)) Then
            TextEdit = New IdentifierTextEdit()
        ElseIf _
        ( _
            (Char.IsDigit(Char.ConvertFromUtf32(InputChar))) OrElse
            ((InputChar = Asc("-") OrElse InputChar = Asc("+")) AndAlso (This IsNot Nothing) AndAlso (This.Selection IsNot Nothing) AndAlso TryCast(This.Selection.PreviousSibling, OperatorTextEdit) IsNot Nothing AndAlso DirectCast(This.Selection.PreviousSibling, OperatorTextEdit).IsOperator) _
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
