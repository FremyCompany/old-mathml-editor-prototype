Partial Public MustInherit Class InputHelper

    Protected WithEvents This As MathElement
    Public Sub New(ByVal This As MathElement)
        Me.This = This
    End Sub

    Private Shared Rep As New List(Of Replacement)
    Public Shared ReadOnly Property Replacements As List(Of Replacement)
        Get
            Return Rep
        End Get
    End Property

    Private Shared InRep As New List(Of Replacement)
    Public Shared ReadOnly Property InputReplacements As List(Of Replacement)
        Get
            Return InRep
        End Get
    End Property

    Public ReadOnly Property CurrentInput As InputHelper
        Get
            Return This.Selection.CommonAncestror.Input
        End Get
    End Property

    Public Sub ProcessString(ByVal InputString As String)
        For X As Integer = 0 To InputString.Length - 1
            Dim InputChar = InputString(X)
            If AscW(InputChar) >= 899072 AndAlso AscW(InputChar) <= 901119 Then
                CurrentInput.ProcessChar(Char.ConvertToUtf32(InputChar, InputString(X + 1)))
                X += 1
            Else
                CurrentInput.ProcessChar(AscW(InputChar))
            End If
        Next
    End Sub

    Shared StartBlockChars As String = "├([{"
    Shared EndBlockChars As String = "┤)]}"

    Public Sub ProcessChar(ByVal InputChar As Integer)

        ' Can't process char if not currently selected
        If This.Selection.CommonAncestror IsNot This Then
            Throw New InvalidOperationException("Sending text input can be done only to the element who host the selection. Modify your code to change the selection before sending any input.")
        End If

        '
        ' TODO: Implement ProcessChar logic
        '

        ' Preprocessing (first bubling phase)
        If PreProcessChar(InputChar) Then Exit Sub

        ' Block characters
        If StartBlockChars.Contains(InputChar) Then
            If ProcessStartOfBlock(InputChar) Then Exit Sub
        ElseIf EndBlockChars.Contains(InputChar) Then
            If ProcessEndOfBlock(InputChar) Then Exit Sub
        End If

        This.Selection.DeleteContents()

        ' Handle chars by right, by left, or by this input helper
        If This.Selection.SelectionStart IsNot Nothing AndAlso This.Selection.SelectionStart.Input.ProcessChar_FromRight(InputChar) Then Exit Sub
        If This.Selection.SelectionEnd IsNot Nothing AndAlso This.Selection.SelectionEnd.Input.ProcessChar_FromLeft(InputChar) Then Exit Sub
        If ProcessChar_Internal(InputChar) Then Exit Sub

        ' When a char wasn't handled neither by right, by left or by current element:
        If This.Selection.SelectionEnd Is Nothing Then
            This.Selection.MoveNext() : This.Selection.CommonAncestror.Input.ProcessChar(InputChar)
        End If

    End Sub

    Public Function ProcessChar_FromLeft(ByVal InputChar As Integer) As Boolean
        Return ProcessChar_FromLeft_Internal(InputChar)
    End Function

    Public Function ProcessChar_FromRight(ByVal InputChar As Integer) As Boolean
        Return ProcessChar_FromRight_Internal(InputChar)
    End Function

    Public Sub ProcessDelete()

        If This.Selection.IsEmpty Then

            ' If the selection is emtpy, try to give the priority to the left element
            Dim RightElement = This.Selection.SelectionEnd
            If RightElement.Input.ProcessDelete_FromLeft() Then
                Exit Sub
            End If

        Else

            ' If the selection is not empty, we just need to delete its content
            This.Selection.DeleteContents()
            Exit Sub

        End If

        ' Perform the classical action for Delete for this element
        If ProcessDelete_Internal() Then
            Exit Sub
        End If

        ' If this element didn't respond to the keypress, we may forward the event to another one
        If This.Selection.SelectionEnd Is Nothing Then
            This.Selection.MoveNext()
            This.Selection.CommonAncestror.Input.ProcessDelete()
            Exit Sub
        End If

    End Sub

    Public Sub ProcessBackSpace()

        If This.Selection.IsEmpty Then

            ' If the selection is emtpy, try to give the priority to the left element
            Dim LeftElement = This.Selection.SelectionStart
            If (LeftElement IsNot Nothing) AndAlso (LeftElement.Input.ProcessBackSpace_FromRight()) Then
                Exit Sub
            End If

        Else

            ' If the selection is not empty, we just need to delete its content
            This.Selection.DeleteContents()
            Exit Sub

        End If

        ' Perform the classical action for Backspace for this element
        If ProcessBackSpace_Internal() Then
            Exit Sub
        End If

        ' If this element didn't respond to the keypress, we may forward the event to another one
        If This.Selection.SelectionStart Is Nothing Then
            This.Selection.MovePrevious()
            This.Selection.CommonAncestror.Input.ProcessBackSpace()
            Exit Sub
        End If

    End Sub

    Public Function ProcessDelete_FromLeft() As Boolean
        Return ProcessDelete_FromLeft_Internal()
    End Function

    Public Function ProcessBackSpace_FromRight() As Boolean
        Return ProcessBackSpace_FromRight_Internal()
    End Function

    Public Function ProcessLeftKey() As Boolean
        Return ProcessLeftKey_Internal()
    End Function

    Public Function ProcessRightKey() As Boolean
        Return ProcessRightKey_Internal()
    End Function

    Public Function ProcessLeftKey_FormRight() As Boolean
        Return ProcessLeftKey_FormRight()
    End Function

    Public Function ProcessRightKey_FromLeft() As Boolean
        Return ProcessRightKey_FromLeft_Internal()
    End Function

    Public Function ProcessUpKey() As Boolean
        Return ProcessUpKey_Internal()
    End Function

    Public Function ProcessDownKey() As Boolean
        Return ProcessDownKey_Internal()
    End Function

    Public Function ProcessHat_FromRight() As Boolean
        Return ProcessHat_FromRight_Internal()
    End Function

    Public Function ProcessUnderscore_FromRight() As Boolean
        Return ProcessUnderscore_FromRight_Internal()
    End Function

    Public Function ProcessFraction_FromRight() As Boolean
        Return ProcessFraction_FromRight_Internal()
    End Function

    Public Function PreProcessChar(ByVal InputChar As Integer) As Boolean
        Return PreProcessChar_Internal(InputChar)
    End Function

    Public Overridable Function PreProcessChar_Internal(ByVal InputChar As Integer) As Boolean

        ' WaitChar (default pre-process)
        If IsWaitingForChar Then
            If InputChar = WaitChar Then
                If ProcessWaitChar(InputChar) Then
                    Return True
                End If
            End If
        End If

        ' The parent may want to pre-process, too
        If This.Parent IsNot Nothing Then
            This.Parent.Input.PreProcessChar(InputChar)
        End If

        ' No pre-process
        Return False

    End Function

    Public Overridable Function ProcessWaitChar(ByVal InputChar As Integer) As Boolean
        ' Default wait char processing : eat, and walk to next child
        WaitChar = Nothing : This.Selection.SetSelection(This.Parent, This, This.NextSibling)
        Return True
    End Function

    Public Function ProcessStartOfBlock(ByVal InputChar As Integer) As Boolean
        Return ProcessStartOfBlock_Internal(InputChar)
    End Function

    Public Overridable Function ProcessStartOfBlock_Internal(ByVal InputChar As Integer) As Boolean
        Return False
    End Function

    Public Function ProcessEndOfBlock(ByVal InputChar As Integer) As Boolean
        Return ProcessEndOfBlock_Internal(InputChar)
    End Function

    Public Overridable Function ProcessEndOfBlock_Internal(ByVal InputChar As Integer) As Boolean
        Return False
    End Function

    Public MustOverride Function ProcessChar_Internal(ByVal InputChar As Integer) As Boolean
    Public Overridable Function ProcessChar_FromLeft_Internal(ByVal InputChar As Integer) As Boolean
        Return False
    End Function
    Public Overridable Function ProcessChar_FromRight_Internal(ByVal InputChar As Integer) As Boolean
        Return False
    End Function

    Public Overridable Function ProcessDelete_Internal() As Boolean

        ' In case there's no element to delete, forward the call
        If This.Selection.SelectionStart Is Nothing Then Return False

        ' If there's one element to delete, delete it
        This.Selection.MoveLeft(SelectionHelper.SelectionPointType.StartPoint)
        This.Selection.DeleteContents()

        ' Inform that the delete key was handled
        Return True

    End Function

    Public Overridable Function ProcessBackSpace_Internal() As Boolean

        ' In case there's no element to delete, forward the call
        If This.Selection.SelectionEnd Is Nothing Then Return False

        ' If there's one element to delete, delete it
        This.Selection.MoveRight(SelectionHelper.SelectionPointType.EndPoint)
        This.Selection.DeleteContents()

        ' Inform that the delete key was handled
        Return True

    End Function

    Public Overridable Function ProcessDelete_FromLeft_Internal() As Boolean
        Return False
    End Function

    Public Overridable Function ProcessBackSpace_FromRight_Internal() As Boolean
        Return False
    End Function

    Public Overridable Function ProcessLeftKey_Internal() As Boolean
        This.Selection.MoveLeft() : Return True
    End Function

    Public Overridable Function ProcessRightKey_Internal() As Boolean
        This.Selection.MoveRight() : Return True
    End Function

    ' TODO: Check those functions (LeftKey, from right) are actually used
    Public Overridable Function ProcessLeftKey_FromRight_Internal() As Boolean
        Return False
    End Function

    Public Overridable Function ProcessRightKey_FromLeft_Internal() As Boolean
        Return False
    End Function

    Public Overridable Function ProcessUpKey_Internal() As Boolean
        Return False
    End Function

    Public Overridable Function ProcessDownKey_Internal() As Boolean
        Return False
    End Function

    Public Overridable Function ProcessHat_FromRight_Internal() As Boolean
        ' TODO: ProcessHat
        Return True
    End Function

    Public Overridable Function ProcessUnderscore_FromRight_Internal() As Boolean
        ' TODO: ProcessUnderscore
        Return True
    End Function

    Public Overridable Function ProcessFraction_FromRight_Internal() As Boolean
        ' TODO: ProcessFraction
        Return True
    End Function

    Public Event BeforeProcessChar As EventHandler(Of InputEventArgs)

    Private WM As InputWritingMode
    Public Property WritingMode As InputWritingMode
        Get
            If WM = 0 Then
                If This.Parent Is Nothing Then
                    Return InputWritingMode.Linear
                Else
                    Return This.Parent.Input.WritingMode
                End If
            Else
                Return WM
            End If
        End Get
        Set(ByVal value As InputWritingMode)
            WM = value
        End Set
    End Property

    Public Property WaitChar As Integer
    Public ReadOnly Property IsWaitingForChar As Boolean
        Get
            Return WaitChar <> Nothing
        End Get
    End Property

    Private Sub This_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles This.LostFocus
        ' Make sure the element input mode revert back to defaults
        WritingMode = InputWritingMode.Inherit
        WaitChar = Nothing
    End Sub

End Class

Partial Public MustInherit Class InputHelper

    Public Enum InputWritingMode

        Inherit = 0

        OneBlock = 2 ^ 20
        OneChar = 2 ^ 21

        [Edit] = 2 ^ 0
        [Function] = 2 ^ 1
        [LinearLike] = 2 ^ 2
        [LaTeXLike] = 2 ^ 3

        [Linear] = InputWritingMode.LinearLike Or InputWritingMode.OneBlock
        [LaTeX] = InputWritingMode.LaTeXLike Or InputWritingMode.OneChar
        [SoftLaTeX] = InputWritingMode.LaTeXLike Or InputWritingMode.OneBlock

    End Enum

End Class

Public Class InputEventArgs : Inherits EventArgs
    Public Property InputChar As Char
    Public Property Handled As Boolean
End Class

Public Class Replacement
    ' TODO: Implement replacements
End Class
