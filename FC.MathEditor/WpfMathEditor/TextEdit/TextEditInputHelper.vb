﻿Public Class TextEditInputHelper : Inherits InputHelper

    Private IsAccepted As Func(Of Integer, Boolean)
    Public Sub New(ByVal This As TextEdit, ByVal IsAccepted As Func(Of Integer, Boolean))
        MyBase.New(This)
    End Sub

    Public Overrides Function ProcessChar_Internal(ByVal InputChar As Integer) As Boolean
        '
        ' TODO: TextEdit.ProcessChar_Internal
        '
        ' A TextEdit treat char typed in between by simply accepting it 
        ' or by splitting itself in two parts and by inserting the unaccepted
        ' char in a new text-edit between the two

        If IsAccepted(InputChar) Then

            Console.WriteLine("TODO: TextEdit.ProcessChar_Internal")
            This.Selection.DeleteContents()
            This.Selection.CommonAncestror.Children.InsertAfter(New UnicodeGlyph(InputChar), This.Selection.SelectionStart)

            Return True

        Else

            Return False
        End If
    End Function

    Public Overrides Function ProcessChar_FromLeft_Internal(ByVal InputChar As Integer) As Boolean
        If IsAccepted(Char.ConvertFromUtf32(InputChar)) Then

            This.Selection.DeleteContents()

            Dim C = New UnicodeGlyph(InputChar)
            This.Children.InsertAfter(C, Nothing)
            This.Selection.SetSelection(This, C, C.NextSibling)
            Return True

        Else

            Return MyBase.ProcessChar_FromLeft_Internal(InputChar)

        End If
    End Function

    Public Overrides Function ProcessChar_FromRight_Internal(ByVal InputChar As Integer) As Boolean
        If IsAccepted(Char.ConvertFromUtf32(InputChar)) Then

            This.Selection.DeleteContents()

            Dim C = New UnicodeGlyph(InputChar)
            This.AddChild(C)
            This.Selection.SetSelection(This, C, C.NextSibling)
            Return True

        Else

            Return MyBase.ProcessChar_FromRight_Internal(InputChar)

        End If
    End Function

    Public Overrides Function ProcessBackSpace_FromRight_Internal() As Boolean
        Return MyBase.ProcessBackSpace_FromRight_Internal()
    End Function

    Public Overrides Function ProcessDelete_FromLeft_Internal() As Boolean
        Return MyBase.ProcessDelete_FromLeft_Internal()
    End Function

End Class
