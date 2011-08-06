Partial Public MustInherit Class InputHelper

    Protected WithEvents This As MathElement
    Public Sub New(This As MathElement)
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
            Return This.Selection.ParentElement.Input
        End Get
    End Property

    Public Sub ProcessString(InputString As String)
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

    Public Sub ProcessChar(InputChar As Integer)

        ' Can't process char if not currently selected
        If This.Selection.ParentElement IsNot This Then
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
        If This.Selection.PreviousSibling IsNot Nothing AndAlso This.Selection.PreviousSibling.Input.ProcessChar_FromRight(InputChar) Then Exit Sub
        If This.Selection.NextSibling IsNot Nothing AndAlso This.Selection.NextSibling.Input.ProcessChar_FromLeft(InputChar) Then Exit Sub
        If ProcessChar_Internal(InputChar) Then Exit Sub

        ' When a char wasn't handled neither by right, by left or by current element:
        If This.Selection.NextSibling Is Nothing AndAlso This.ParentElement IsNot Nothing Then
            If This.Selection.MoveAfterParent() Then This.Selection.ParentElement.Input.ProcessChar(InputChar)
        ElseIf This.Selection.PreviousSibling Is Nothing Then
            If This.Selection.MoveBeforeParent() Then This.Selection.ParentElement.Input.ProcessChar(InputChar)
        End If

    End Sub

    Public Function ProcessChar_FromLeft(InputChar As Integer) As Boolean
        Return ProcessChar_FromLeft_Internal(InputChar)
    End Function

    Public Function ProcessChar_FromRight(InputChar As Integer) As Boolean
        Return ProcessChar_FromRight_Internal(InputChar)
    End Function

    Public Sub ProcessDelete(Ctrl As Boolean, Alt As Boolean, Shift As Boolean)

        If This.Selection.IsCollapsed Then

            ' If the selection is emtpy, try to give the priority to the left element
            Dim RightElement = This.Selection.NextSibling
            If RightElement.Input.ProcessDelete_FromLeft(Ctrl, Alt, Shift) Then
                Exit Sub
            End If

        Else

            ' If the selection is not empty, we just need to delete its content
            This.Selection.DeleteContents()
            Exit Sub

        End If

        ' Perform the classical action for Delete for this element
        If ProcessDelete_Internal(Ctrl, Alt, Shift) Then
            Exit Sub
        End If

        ' If this element didn't respond to the keypress, we may forward the event to another one
        If This.Selection.NextSibling Is Nothing Then
            If This.Selection.MoveAfterParent() Then This.Selection.ParentElement.Input.ProcessDelete(Ctrl, Alt, Shift)
            Exit Sub
        End If

    End Sub

    Public Sub ProcessBackSpace(Ctrl As Boolean, Alt As Boolean, Shift As Boolean)

        If This.Selection.IsCollapsed Then

            ' If the selection is emtpy, try to give the priority to the left element
            Dim LeftElement = This.Selection.PreviousSibling
            If (LeftElement IsNot Nothing) AndAlso (LeftElement.Input.ProcessBackSpace_FromRight(Ctrl, Alt, Shift)) Then
                Exit Sub
            End If

        Else

            ' If the selection is not empty, we just need to delete its content
            This.Selection.DeleteContents()
            Exit Sub

        End If

        ' Perform the classical action for Backspace for this element
        If ProcessBackSpace_Internal(Ctrl, Alt, Shift) Then
            Exit Sub
        End If

        ' If this element didn't respond to the keypress, we may forward the event to another one
        If This.Selection.PreviousSibling Is Nothing AndAlso This.Selection.ParentElement.ParentElement IsNot Nothing Then
            If This.Selection.MoveBeforeParent() Then This.Selection.ParentElement.Input.ProcessBackSpace(Ctrl, Alt, Shift)
            Exit Sub
        End If

    End Sub

    Public Function ProcessDelete_FromLeft(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return ProcessDelete_FromLeft_Internal(Ctrl, Alt, Shift)
    End Function

    Public Function ProcessBackSpace_FromRight(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return ProcessBackSpace_FromRight_Internal(Ctrl, Alt, Shift)
    End Function

    Public Function ProcessLeftKey(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean

        Dim InitialEP = This.Selection.EndPoint

        ' Before everything, we should try to collapse selection
        If Not Shift AndAlso Not This.Selection.IsCollapsed Then
            This.Selection.CollapseToStart() : Return True
        End If

        ' First, the right element has the possibility to react, under certain circumstances
        If (Not Ctrl) AndAlso (This.Selection.IsCollapsed) AndAlso (This.Selection.PreviousSibling IsNot Nothing) Then
            If This.Selection.PreviousSibling.Input.ProcessLeftKey_FromRight(Ctrl, Alt, Shift) Then
                If Shift Then
                    This.Selection.SetPoint(This.Selection.EndPoint, PointToChange:=SelectionHelper.SelectionPointType.EndPoint)
                    If This.Selection.EndPoint = InitialEP Then
                        If Not This.Selection.EndPoint.IsAtOrigin Then
                            This.Selection.SetPoint(This.Selection.EndPoint.Increment(-1), SelectionHelper.SelectionPointType.EndPoint)
                        End If
                    End If
                End If : Return True
            End If
        End If

        ' Then, we fall back to the normal behavior
        If ProcessLeftKey_Internal(Ctrl, Alt, Shift) Then
            If Shift Then
                This.Selection.SetPoint(This.Selection.EndPoint, PointToChange:=SelectionHelper.SelectionPointType.EndPoint)
                If This.Selection.EndPoint = InitialEP Then
                    If Not This.Selection.EndPoint.IsAtOrigin Then
                        This.Selection.SetPoint(This.Selection.EndPoint.Increment(-1), SelectionHelper.SelectionPointType.EndPoint)
                    End If
                End If
            End If : Return True
        Else
            Return False
        End If

    End Function

    Public Function ProcessRightKey(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean

        Dim InitialEP = This.Selection.EndPoint

        ' Before everything, we should try to collapse selection
        If Not Shift AndAlso Not This.Selection.IsCollapsed Then
            This.Selection.CollapseToEnd() : Return True
        End If

        ' First, the right element has the possibility to react, under certain circumstances
        If (Not Ctrl) AndAlso (This.Selection.IsCollapsed) AndAlso (This.Selection.NextSibling IsNot Nothing) Then
            If This.Selection.NextSibling.Input.ProcessRightKey_FromLeft(Ctrl, Alt, Shift) Then
                If Shift Then
                    This.Selection.SetPoint(This.Selection.EndPoint, PointToChange:=SelectionHelper.SelectionPointType.EndPoint)
                    If This.Selection.EndPoint = InitialEP Then
                        If Not This.Selection.EndPoint.IsAtEnd Then
                            This.Selection.SetPoint(This.Selection.EndPoint.Increment(1), SelectionHelper.SelectionPointType.EndPoint)
                        End If
                    End If
                End If : Return True
            End If
        End If

        ' Then, we fall back to the no
        If ProcessRightKey_Internal(Ctrl, Alt, Shift) Then
            If Shift Then
                This.Selection.SetPoint(This.Selection.EndPoint, PointToChange:=SelectionHelper.SelectionPointType.EndPoint)
                If This.Selection.EndPoint = InitialEP Then
                    If Not This.Selection.EndPoint.IsAtEnd Then
                        This.Selection.SetPoint(This.Selection.EndPoint.Increment(1), SelectionHelper.SelectionPointType.EndPoint)
                    End If
                End If
            End If : Return True
        Else
            Return False
        End If

    End Function

    Public Function ProcessLeftKey_FromRight(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return ProcessLeftKey_FromRight_Internal(Ctrl, Alt, Shift)
    End Function

    Public Function ProcessRightKey_FromLeft(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return ProcessRightKey_FromLeft_Internal(Ctrl, Alt, Shift)
    End Function

    Public Function ProcessUpKey(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return ProcessUpKey_Internal(Ctrl, Alt, Shift)
    End Function

    Public Function ProcessDownKey(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return ProcessDownKey_Internal(Ctrl, Alt, Shift)
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

    Public Function PreProcessChar(InputChar As Integer, Optional FromChild As Boolean = False) As Boolean
        Return PreProcessChar_Internal(InputChar)
    End Function

    Public Overridable Function PreProcessChar_Internal(InputChar As Integer) As Boolean

        ' WaitChar (default pre-process)
        If IsWaitingForChar Then
            If InputChar = WaitChar Then
                If ProcessWaitChar(InputChar) Then
                    Return True
                End If
            End If
        End If

        ' The parent may want to pre-process, too
        If This.ParentElement IsNot Nothing Then
            If This.ParentElement.Input.PreProcessChar(InputChar, True) Then
                Return True
            End If
        End If

        ' No pre-process
        Return False

    End Function

    Public Overridable Function ProcessWaitChar(InputChar As Integer) As Boolean
        ' Default wait char processing : eat, and walk to next child
        WaitChar = Nothing : This.Selection.SetPoint(This.ParentElement.GetSelectionAt(This.ChildIndex + 1))
        Return True
    End Function

    Public Function ProcessStartOfBlock(InputChar As Integer) As Boolean
        Return ProcessStartOfBlock_Internal(InputChar)
    End Function

    Public Overridable Function ProcessStartOfBlock_Internal(InputChar As Integer) As Boolean
        Return False
    End Function

    Public Function ProcessEndOfBlock(InputChar As Integer) As Boolean
        Return ProcessEndOfBlock_Internal(InputChar)
    End Function

    Public Overridable Function ProcessEndOfBlock_Internal(InputChar As Integer) As Boolean
        Return False
    End Function

    Public MustOverride Function ProcessChar_Internal(InputChar As Integer) As Boolean
    Public Overridable Function ProcessChar_FromLeft_Internal(InputChar As Integer) As Boolean
        Return False
    End Function
    Public Overridable Function ProcessChar_FromRight_Internal(InputChar As Integer) As Boolean
        Return False
    End Function

    Public Overridable Function ProcessDelete_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean

        ' Retreive the first element before the selection
        Dim ElementToDelete = This.Selection.NextSibling

        ' In case there's no element to delete, forward the call
        If ElementToDelete Is Nothing Then Return False

        ' If there's one element to delete, delete it
        ProcessRightKey(Ctrl, Alt, True)
        This.Selection.DeleteContents()
        'x ElementToDelete.ParentElement.RemoveChild(ElementToDelete)

        ' Inform that the delete key was handled
        Return True

    End Function

    Public Overridable Function ProcessBackSpace_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean

        ' Retreive the first element before the selection
        Dim ElementToDelete = This.Selection.PreviousSibling

        ' In case there's no element to delete, forward the call
        If ElementToDelete Is Nothing Then Return False

        ' If there's one element to delete, delete it
        ProcessLeftKey(Ctrl, Alt, True)
        This.Selection.DeleteContents()
        'x ElementToDelete.ParentElement.RemoveChild(ElementToDelete)

        ' Inform that the delete key was handled
        Return True

    End Function

    Public Overridable Function ProcessDelete_FromLeft_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return False
    End Function

    Public Overridable Function ProcessBackSpace_FromRight_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return False
    End Function

    Public Overridable Function ProcessLeftKey_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean

        If Shift Then
            If Ctrl AndAlso Not This.Selection.EndPoint.IsAtOrigin Then
                This.Selection.Increment(SelectionHelper.SelectionPointType.EndPoint, -1)
            Else
                ' If we are at the start of the textedit, 
                ' we need to change the selection to outside the textedit before treating the key
                If This.Selection.EndPoint.IsAtOrigin Then
                    If This.Selection.MoveBeforeParent(SelectionHelper.SelectionPointType.EndPoint) Then
                        This.ParentDocument.CurrentInput.ProcessLeftKey(Ctrl, Alt, Shift)
                    End If
                Else
                    This.Selection.MoveLeft(SelectionHelper.SelectionPointType.EndPoint)
                End If
            End If
        Else
            If This.Selection.IsCollapsed Then
                This.Selection.MoveLeft(SelectionHelper.SelectionPointType.Selection)
            Else
                This.Selection.CollapseToStart()
            End If
        End If

        Return True

    End Function

    Public Overridable Function ProcessRightKey_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean

        If Shift Then
            If Ctrl AndAlso Not This.Selection.EndPoint.IsAtEnd Then
                This.Selection.Increment(SelectionHelper.SelectionPointType.EndPoint, 1)
            Else
                ' If we are at the end of the textedit, 
                ' we need to change the selection to outside the textedit before treating the key
                If This.Selection.EndPoint.IsAtEnd Then
                    If This.Selection.MoveAfterParent(SelectionHelper.SelectionPointType.EndPoint) Then
                        This.ParentDocument.CurrentInput.ProcessRightKey(Ctrl, Alt, Shift)
                    End If
                Else
                    This.Selection.MoveRight(SelectionHelper.SelectionPointType.EndPoint)
                End If
            End If
        Else
            If This.Selection.IsCollapsed Then
                This.Selection.MoveRight(SelectionHelper.SelectionPointType.Selection)
            Else
                This.Selection.CollapseToEnd()
            End If
        End If

        Return True

    End Function

    Public Overridable Function ProcessLeftKey_FromRight_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return False
    End Function

    Public Overridable Function ProcessRightKey_FromLeft_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return False
    End Function

    Public Overridable Function ProcessUpKey_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Return False
    End Function

    Public Overridable Function ProcessDownKey_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
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
                If This.ParentElement Is Nothing Then
                    Return InputWritingMode.Linear
                Else
                    Return This.ParentElement.Input.WritingMode
                End If
            Else
                Return WM
            End If
        End Get
        Set(value As InputWritingMode)
            WM = value
        End Set
    End Property

    Public Property WaitChar As Integer
    Public ReadOnly Property IsWaitingForChar As Boolean
        Get
            Return WaitChar <> Nothing
        End Get
    End Property

    Private Sub This_LostFocus(sender As Object, e As System.EventArgs) Handles This.LostFocus
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
