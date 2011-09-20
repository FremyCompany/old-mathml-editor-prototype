Public Class RowLayoutEngineInputHelper : Inherits InputHelper

    Public Sub New(This As RowLayoutEngine)
        MyBase.New(This)
    End Sub

    Public Overrides Function ProcessBackSpace_FromRight_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return False
    End Function

    Public Overrides Function ProcessChar_Internal(InputChar As Integer) As Boolean

        ' [[Assumption]] When we reach this function, the adjacent textedits refused the char.
        ' Create a new textedit to host the new char
        Dim TextEdit As TextEdit = FC.MathEditor.TextEdit.FromChar(InputChar, This)

        This.Selection.ReplaceContents(TextEdit)

        ' Set the selection after the TextEdit
        This.Selection.SetPoint(TextEdit.GetSelectionAfter())

        Return True

    End Function

    Public Overrides Function ProcessDelete_FromLeft_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return False
    End Function

    Public Overrides Function ProcessDownKey_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return False
    End Function

    Public Overrides Function ProcessFraction_FromRight_Internal() As Boolean
        Return False
    End Function

    Public Overrides Function ProcessHat_FromRight_Internal() As Boolean
        Return False
    End Function

    Public Overrides Function ProcessLeftKey_FromRight_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        If Shift Then
            This.Selection.SetPoint(This.GetSelectionAtOrigin(), SelectionHelper.SelectionPointType.EndPoint) : Return True
        Else
            This.Selection.SetPoint(This.GetSelectionAtOrigin()) : Return True
        End If
    End Function

    Public Overrides Function ProcessRightKey_FromLeft_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        If Shift Then
            This.Selection.SetPoint(This.GetSelectionAtEnd(), SelectionHelper.SelectionPointType.EndPoint) : Return True
        Else
            This.Selection.SetPoint(This.GetSelectionAtEnd()) : Return True
        End If
    End Function

    Public Overrides Function ProcessUnderscore_FromRight_Internal() As Boolean
        Return False
    End Function

    Public Overrides Function ProcessUpKey_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return False
    End Function

End Class
