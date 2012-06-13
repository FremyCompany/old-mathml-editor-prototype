Public Class TextEditInputHelper : Inherits InputHelper

    Private IsCharAccepted As Func(Of Integer, Integer, Boolean) = Function(C, F) This.IsCharAccepted(C, F)
    Public Sub New(This As TextEdit)
        MyBase.New(This)
    End Sub

    Protected Shadows ReadOnly Property This As TextEdit
        Get
            Return MyBase.This
        End Get
    End Property

    Public Overrides Function ProcessChar_Internal(InputChar As Integer) As Boolean
        '
        ' TODO: TextEdit.ProcessChar_Internal
        '
        ' A TextEdit treat char typed in between by simply accepting it 
        ' or by splitting itself in two parts and by inserting the unaccepted
        ' char in a new text-edit between the two

        If IsCharAccepted(InputChar, This.Selection.StartPoint.ChildIndex) _
            AndAlso (This.EatInputByDefault OrElse Not (This.Selection.LogicalEndPoint.IsAtEnd Or This.Selection.LogicalEndPoint.IsAtOrigin)) _
            AndAlso (This.Children.CanAdd) Then

            ' Insert a new unicode glyph for the car
            ' HYPOTHESE: the selection has already been deleted
            Dim NewElement = New UnicodeGlyph(InputChar)
            This.Selection.ParentElement.Children.InsertAfter(NewElement, This.Selection.PreviousSibling)

            ' Set the selection point after the text edit (if insertion happens at end)
            ' or after the inserted char (if insertion happens anywhere else)
            If NewElement.NextSibling Is Nothing Then
                This.Selection.SetPoint(This.GetSelectionAfter())
            Else
                This.Selection.SetPoint(NewElement.GetSelectionAfter())
            End If

            Return True

        ElseIf (This.Selection.NextSibling IsNot Nothing) AndAlso (This.Selection.PreviousSibling IsNot Nothing) Then

            ' Generate the new element, and perform the split
            Dim NewTextEdit As TextEdit = TextEdit.FromChar(InputChar, This)
            Dim FirstPart As TextEdit
            Dim SecondPart As TextEdit

            Select Case This.Selection.StartPoint.ChildIndex
                Case 1
                    FirstPart = TextEdit.FromChar(CType(This.FirstChild, UnicodeGlyph).DisplayCharCode)
                    SecondPart = This.Clone(False)

                    For Each child In This.Children
                        If child.ChildIndex > 0 Then
                            SecondPart.AddChild(child.Clone())
                        End If
                    Next
                Case This.Children.Count
                    FirstPart = This.Clone(False)
                    SecondPart = TextEdit.FromChar(CType(This.LastChild, UnicodeGlyph).DisplayCharCode)

                    For Each child In This.Children
                        If child.ChildIndex < This.Children.Count - 1 Then
                            FirstPart.AddChild(child.Clone())
                        End If
                    Next

                Case Else
                    FirstPart = This.Clone(False)
                    SecondPart = This.Clone(False)

                    For Each child In This.Children
                        If child.ChildIndex < This.Selection.GetPoint(SelectionHelper.SelectionPointType.Selection).ChildIndex Then
                            FirstPart.AddChild(child.Clone())
                        Else
                            SecondPart.AddChild(child.Clone())
                        End If
                    Next
            End Select



            ' Insert the new elements (two before & one after, to preserve SelectionPoint order)
            This.ParentElement.Children.InsertBefore(FirstPart, This)
            This.ParentElement.Children.InsertBefore(NewTextEdit, This)
            This.ParentElement.Children.InsertAfter(SecondPart, This)

            ' Remove the old one
            This.RemoveFromParent()

            ' Set the selection after the new text edit
            NewTextEdit.Selection.SetPoint(NewTextEdit.GetSelectionAfter())

            Return True

        Else

            Return False

        End If
    End Function

    Public Overrides Function ProcessChar_FromLeft_Internal(InputChar As Integer) As Boolean

        If IsCharAccepted(InputChar, 0) AndAlso This.EatInputByDefault AndAlso This.Children.CanAdd Then

            This.Selection.DeleteContents()

            Dim C = New UnicodeGlyph(InputChar)
            This.Children.InsertAfter(C, Nothing)
            This.Selection.SetPoint(C.GetSelectionAfter())
            Return True

        Else

            Return False

        End If

    End Function

    Public Overrides Function ProcessChar_FromRight_Internal(InputChar As Integer) As Boolean

        If IsCharAccepted(InputChar, This.Children.Count) AndAlso This.EatInputByDefault AndAlso This.Children.CanAdd Then

            This.Selection.DeleteContents()

            Dim C = New UnicodeGlyph(InputChar)
            This.AddChild(C)
            This.Selection.SetPoint(This.GetSelectionAfter())
            Return True

        Else

            Return False

        End If

    End Function

    Public Overrides Function ProcessBackSpace_FromRight_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        If This.Children.HasMany Then
            This.LastChild.RemoveFromParent() : Return True
        Else
            This.RemoveFromParent() : Return True
        End If
    End Function

    Public Overrides Function ProcessDelete_FromLeft_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        If This.Children.HasMany Then
            This.FirstChild.RemoveFromParent() : Return True
        Else
            This.RemoveFromParent() : Return True
        End If
    End Function

    Public Overrides Function ProcessBackSpace_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Dim R = MyBase.ProcessBackSpace_Internal(Ctrl, Alt, Shift)
        If R AndAlso This.Children.HasNo Then This.RemoveFromParent()
        Return R
    End Function

    Public Overrides Function ProcessDelete_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        Dim R = MyBase.ProcessDelete_Internal(Ctrl, Alt, Shift)
        If R AndAlso This.Children.HasNo Then This.RemoveFromParent()
        Return R
    End Function

    Public Overrides Function ProcessLeftKey_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        If (This.Selection.IsCollapsed) AndAlso (This.Selection.EndPoint.ChildIndex = 1) Then
            If Shift Then
                If This.Children.HasMany Then
                    ' We must allow the selection of the first char only
                    This.Selection.SetPoint(This.Selection.GetPoint(SelectionHelper.SelectionPointType.EndPoint).Increment(-1), SelectionHelper.SelectionPointType.EndPoint)
                    Return True
                Else
                    ' In case we're at the first char, walk outside the textedit
                    If This.Selection.GetPoint(SelectionHelper.SelectionPointType.StartPoint).ParentElement Is This Then
                        If This.Selection.StartPoint.ChildIndex = 1 Then
                            This.Selection.SetSelection(This.GetSelectionAfter(), This.GetSelectionBefore()) : Return True
                        Else
                            This.Selection.SetSelection(This.GetSelectionBefore(), This.GetSelectionBefore()) : Return True
                        End If
                    Else
                        This.Selection.SetPoint(This.GetSelectionBefore(), SelectionHelper.SelectionPointType.EndPoint) : Return True
                    End If
                End If
            Else
                ' In case we're at the first char, walk outside the textedit
                This.Selection.MoveBeforeParent() : Return True
            End If
        Else
            Return MyBase.ProcessLeftKey_Internal(Ctrl, Alt, Shift)
        End If
    End Function

    Public Overrides Function ProcessRightKey_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        If (This.Selection.IsCollapsed) AndAlso (This.Selection.EndPoint.ChildIndex + 1 = This.Children.Count) Then
            If Shift Then
                If This.Children.HasMany Then
                    ' We must allow the selection of the last char only
                    This.Selection.SetPoint(This.Selection.GetPoint(SelectionHelper.SelectionPointType.EndPoint).Increment(1), SelectionHelper.SelectionPointType.EndPoint)
                    Return True
                Else
                    ' In case we're at the last char, walk outside the textedit
                    If This.Selection.GetPoint(SelectionHelper.SelectionPointType.StartPoint).ParentElement Is This Then
                        If This.Selection.StartPoint.ChildIndex = 0 Then
                            This.Selection.SetSelection(This.GetSelectionBefore(), This.GetSelectionAfter()) : Return True
                        Else
                            This.Selection.SetSelection(This.GetSelectionAfter(), This.GetSelectionAfter()) : Return True
                        End If
                    Else
                        This.Selection.SetPoint(This.GetSelectionAfter(), SelectionHelper.SelectionPointType.EndPoint) : Return True
                    End If
                End If
            Else
                ' In case we're at the last char, walk outside the textedit
                This.Selection.MoveAfterParent() : Return True
            End If
        Else
            Return MyBase.ProcessRightKey_Internal(Ctrl, Alt, Shift)
        End If
    End Function

    Public Overrides Function ProcessLeftKey_FromRight_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        ' Skip first char when navigating in
        If This.Children.HasMany Then
            If Shift Then
                If This.Selection.IsCollapsed Then
                    This.Selection.SetSelection(This.LastChild.GetSelectionAfter(), This.LastChild.GetSelectionBefore()) : Return True
                Else
                    This.Selection.SetPoint(This.GetSelectionAt(This.LastChild.ChildIndex), SelectionHelper.SelectionPointType.EndPoint) : Return True
                End If
            Else
                This.Selection.SetPoint(This.GetSelectionAtEnd.Increment(-1)) : Return True
            End If
        Else
            If Shift Then
                If This.Selection.GetPoint(SelectionHelper.SelectionPointType.StartPoint).ParentElement Is This Then
                    If This.Selection.StartPoint.ChildIndex = 1 Then
                        This.Selection.SetSelection(This.GetSelectionAfter(), This.GetSelectionBefore()) : Return True
                    Else
                        This.Selection.SetSelection(This.GetSelectionBefore(), This.GetSelectionBefore()) : Return True
                    End If
                Else
                    This.Selection.SetPoint(This.GetSelectionBefore(), SelectionHelper.SelectionPointType.EndPoint) : Return True
                End If
            Else
                This.Selection.SetPoint(This.GetSelectionBefore()) : Return True
            End If
        End If
    End Function

    Public Overrides Function ProcessRightKey_FromLeft_Internal(Ctrl As Boolean, Alt As Boolean, Shift As Boolean) As Boolean
        ' Skip last char when navigating in
        If This.Children.HasMany Then
            If Shift Then
                If This.Selection.IsCollapsed Then
                    This.Selection.SetSelection(This.FirstChild.GetSelectionBefore(), This.FirstChild.GetSelectionAfter()) : Return True
                Else
                    This.Selection.SetPoint(This.GetSelectionAt(This.FirstChild.ChildIndex + 1), SelectionHelper.SelectionPointType.EndPoint) : Return True
                End If
            Else
                This.Selection.SetPoint(This.GetSelectionAt(1)) : Return True
            End If
        Else
            If Shift Then
                If This.Selection.GetPoint(SelectionHelper.SelectionPointType.StartPoint).ParentElement Is This Then
                    If This.Selection.StartPoint.ChildIndex = 0 Then
                        This.Selection.SetSelection(This.GetSelectionBefore(), This.GetSelectionAfter()) : Return True
                    Else
                        This.Selection.SetSelection(This.GetSelectionAfter(), This.GetSelectionAfter()) : Return True
                    End If
                Else
                    This.Selection.SetPoint(This.GetSelectionAfter(), SelectionHelper.SelectionPointType.EndPoint) : Return True
                End If
            Else
                This.Selection.SetPoint(This.GetSelectionAfter()) : Return True
            End If
        End If
    End Function

End Class
