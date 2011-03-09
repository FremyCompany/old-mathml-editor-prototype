Public Class RowLayoutEngineInputHelper : Inherits InputHelper

    Public Sub New(ByVal This As MathElement)
        MyBase.New(This)
    End Sub

    Public Overrides Function ProcessChar_Internal(ByVal InputChar As Integer) As Boolean

        Dim TextEdit As TextEdit
        This.Selection.DeleteContents()

        If Char.IsLetter(Char.ConvertFromUtf32(InputChar)) Then
            TextEdit = New IdentifierTextEdit()
        ElseIf _
        ( _
            (Char.IsDigit(Char.ConvertFromUtf32(InputChar))) OrElse
            (InputChar = ".") OrElse
            ((InputChar = "-" OrElse InputChar = "+") AndAlso TryCast(This.Selection.SelectionStart, OperatorTextEdit) IsNot Nothing) _
        ) Then
            TextEdit = New NumberTextEdit()
        Else
            TextEdit = New OperatorTextEdit()
        End If

        Dim X As New UnicodeGlyph(AscW(InputChar))
        TextEdit.AddChild(X)
        This.AddChild(TextEdit)
        This.Selection.SetSelection(TextEdit, TextEdit.LastChild, Nothing)

        Return True

    End Function

End Class
