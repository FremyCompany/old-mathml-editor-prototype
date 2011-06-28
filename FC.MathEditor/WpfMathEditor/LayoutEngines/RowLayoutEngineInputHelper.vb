Public Class RowLayoutEngineInputHelper : Inherits InputHelper

    Public Sub New(This As RowLayoutEngine)
        MyBase.New(This)
    End Sub

    Public Overrides Function ProcessBackSpace_FromRight_Internal() As Boolean
        Return False
    End Function

    Public Overrides Function ProcessChar_Internal(InputChar As Integer) As Boolean

        ' [[Assumption]] When we reach this function, the adjacent textedits refused the char.
        ' Create a new textedit to host the new char
        Dim TextEdit As TextEdit

        If Char.IsLetter(Char.ConvertFromUtf32(InputChar)) Then
            TextEdit = New IdentifierTextEdit()
        ElseIf _
        ( _
            (Char.IsDigit(Char.ConvertFromUtf32(InputChar))) OrElse
            (InputChar = ".") OrElse
            ((InputChar = "-" OrElse InputChar = "+") AndAlso TryCast(This.Selection.PreviousSibling, OperatorTextEdit) IsNot Nothing) _
        ) Then
            TextEdit = New NumberTextEdit()
        Else
            TextEdit = New OperatorTextEdit()
        End If

        ' Append the newly created char to the textedit
        Dim X As New UnicodeGlyph(InputChar)
        TextEdit.AddChild(X)
        This.Selection.ReplaceContents(TextEdit)

        ' Change the selection to the TextEdit
        ' TODO: Determine if it wouldn't be better to put the selection in RLE instead
        This.Selection.SetSelection(TextEdit, TextEdit.LastChild, Nothing)

        Return True

    End Function

    Public Overrides Function ProcessDelete_FromLeft_Internal() As Boolean
        Return False
    End Function

    Public Overrides Function ProcessDownKey_Internal() As Boolean
        Return False
    End Function

    Public Overrides Function ProcessFraction_FromRight_Internal() As Boolean
        Return False
    End Function

    Public Overrides Function ProcessHat_FromRight_Internal() As Boolean
        Return False
    End Function

    Public Overrides Function ProcessLeftKey_FromRight_Internal() As Boolean

    End Function

    Public Overrides Function ProcessLeftKey_Internal() As Boolean

    End Function

    Public Overrides Function ProcessRightKey_FromLeft_Internal() As Boolean

    End Function

    Public Overrides Function ProcessRightKey_Internal() As Boolean

    End Function

    Public Overrides Function ProcessUnderscore_FromRight_Internal() As Boolean
        Return False
    End Function

    Public Overrides Function ProcessUpKey_Internal() As Boolean
        Return False
    End Function

End Class
