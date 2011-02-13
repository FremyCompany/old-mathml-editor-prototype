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
        For Each InputChar In InputString
            CurrentInput.ProcessChar(InputChar)
        Next
    End Sub

    Public Sub ProcessChar(ByVal InputChar As Char)

        ' Can't process char if not currently selected
        If This.Selection.CommonAncestror IsNot Me Then
            Throw New InvalidOperationException("Sending text input can be done only to the element who host the selection. Modify your code to change the selection before sending any input.")
        End If

        ' TODO: Implement ProcessChar logic

        ' When a char wasn't handled neither by right, by left or by current element:
        If This.Selection.SelectionEnd Is Nothing Then
            This.Selection.MoveNext()
        End If

    End Sub

    Public Function ProcessChar_FromLeft(ByVal InputChar As Char) As Boolean
        Return ProcessChar_FromLeft_Internal(InputChar)
    End Function

    Public Function ProcessChar_FromRight(ByVal InputChar As Char) As Boolean
        Return ProcessChar_FromRight(InputChar)
    End Function

    Public Sub ProcessDelete()

        If This.Selection.IsEmpty Then
            Dim RightElement = This.Selection.SelectionEnd
            If RightElement.Input.ProcessDelete_FromLeft() Then
                Exit Sub
            End If
        End If

        If ProcessDelete_Internal() Then
            Exit Sub
        End If

        If This.Selection.SelectionEnd Is Nothing Then
            This.Selection.MoveNext()
            This.Selection.CommonAncestror.Input.ProcessDelete()
            Exit Sub
        End If

        Exit Sub

    End Sub

    Public Sub ProcessBackSpace()

        If This.Selection.IsEmpty Then
            Dim RightElement = This.Selection.SelectionEnd
            If RightElement.Input.ProcessDelete_FromLeft() Then
                Exit Sub
            End If
        End If

        If ProcessDelete_Internal() Then
            Exit Sub
        End If

        If This.Selection.SelectionEnd Is Nothing Then
            This.Selection.MoveNext()
            This.Selection.CommonAncestror.Input.ProcessDelete()
            Exit Sub
        End If

        Exit Sub

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

    Public MustOverride Function ProcessChar_Internal(ByVal InputChar As Char) As Boolean
    Public Overridable Function ProcessChar_FromLeft_Internal(ByVal InputChar As Char) As Boolean
        Return False
    End Function
    Public Overridable Function ProcessChar_FromRight_Internal(ByVal InputChar As Char) As Boolean
        Return False
    End Function

    Public Function ProcessDelete_Internal() As Boolean
        If This.Selection.IsEmpty Then
            This.Selection.DeleteContents()
            Return True
        ElseIf This.Selection.SelectionEnd Is Nothing Then
            This.Selection.SetSelection(This.Selection.CommonAncestror, This.Selection.SelectionStart, This.Selection.SelectionEnd)
            Return True
        Else
            Return False
        End If
    End Function
    Public MustOverride Function ProcessBackSpace_Internal() As Boolean
    Public MustOverride Function ProcessDelete_FromLeft_Internal() As Boolean
    Public MustOverride Function ProcessBackSpace_FromRight_Internal() As Boolean

    Public MustOverride Function ProcessLeftKey_Internal() As Boolean
    Public MustOverride Function ProcessRightKey_Internal() As Boolean
    Public MustOverride Function ProcessLeftKey_FromRight_Internal() As Boolean
    Public MustOverride Function ProcessRightKey_FromLeft_Internal() As Boolean

    Public MustOverride Function ProcessUpKey_Internal() As Boolean
    Public MustOverride Function ProcessDownKey_Internal() As Boolean

    Public MustOverride Function ProcessHat_FromRight_Internal() As Boolean
    Public MustOverride Function ProcessUnderscore_FromRight_Internal() As Boolean
    Public MustOverride Function ProcessFraction_FromRight_Internal() As Boolean

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

    Public Property WaitChar As Char
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