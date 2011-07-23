Partial Public Class SelectionHelper

    Public Enum SelectionPointType
        StartPoint
        EndPoint
        Selection
    End Enum

    Public ReadOnly Property IsCollapsed As Boolean
        Get
            Return Me.ApparentSEP.ChildIndex = Me.ApparentSSP.ChildIndex
        End Get
    End Property

    ''' <summary>
    ''' Exchanges the selection start and selection end points
    ''' </summary>
    Public Sub Reverse()
        Dim A = SSP : SSP = SEP : SEP = A
        A = ApparentSSP : ApparentSSP = ApparentSEP : ApparentSEP = A
    End Sub

    ''' <summary>
    ''' Sets the selection start point to the selection end point
    ''' </summary>
    ''' <param name="UseApparentSelection">True if you want to collapse the selection to its apparent end point, false if you want it to collapse to its true end point</param>
    Public Sub CollapseToEnd(Optional UseApparentSelection As Boolean = True)
        If UseApparentSelection Then
            SetSelection(LogicalEndPoint, LogicalEndPoint)
        Else
            SetSelection(SEP, SEP)
        End If
    End Sub

    ''' <summary>
    ''' Sets the selection end point to the selection start point
    ''' </summary>
    ''' <param name="UseApparentSelection">True if you want to collapse the selection to its apparent start point, false if you want it to collapse to its true start point</param>
    Public Sub CollapseToStart(Optional UseApparentSelection As Boolean = True)
        If UseApparentSelection Then
            SetSelection(LogicalStartPoint, LogicalStartPoint)
        Else
            SetSelection(SSP, SSP)
        End If
    End Sub

    ''' <summary>
    ''' Moves the selection to the next available parent location
    ''' </summary>
    ''' <param name="MovedPoint">The point to move</param>
    Public Function MoveAfterParent(Optional MovedPoint As SelectionPointType = SelectionPointType.Selection) As Boolean

        Dim P = GetPoint(MovedPoint)
        Dim E = P.ParentElement

        While E.ParentElement IsNot Nothing

            ' Try to find the next hot spot
            Select Case E.ParentElement.Children.ElementType

                Case MathElement.Type.Formatter
                    Dim Result = E.ParentElement.GetNextInputElement(E)
                    If Result IsNot Nothing Then
                        SetPoint(Result.GetSelectionAtOrigin(), MovedPoint)
                        Return True
                    End If

                Case MathElement.Type.LayoutEngine
                    SetPoint(E.ParentElement.GetSelectionAt(E.ChildIndex + 1), MovedPoint)
                    Return True

            End Select

            ' Move the the parent level
            E = E.ParentElement

        End While

        Return False

    End Function

    ''' <summary>
    ''' Moves the selection to the next available parent location (backward)
    ''' </summary>
    ''' <param name="MovedPoint">The point to move</param>
    Public Function MoveBeforeParent(Optional MovedPoint As SelectionPointType = SelectionPointType.Selection) As Boolean

        Dim P = GetPoint(MovedPoint)
        Dim E = P.ParentElement

        While E.ParentElement IsNot Nothing

            ' Try to find the next hot spot
            Select Case E.ParentElement.Children.ElementType

                Case MathElement.Type.Formatter
                    Dim Result = E.ParentElement.GetPreviousInputElement(E)
                    If Result IsNot Nothing Then
                        SetPoint(Result.GetSelectionAtEnd(), MovedPoint)
                        Return True
                    End If

                Case MathElement.Type.LayoutEngine
                    SetPoint(E.ParentElement.GetSelectionAt(E.ChildIndex), MovedPoint)
                    Return True

            End Select

            ' Move the the parent level
            E = E.ParentElement

        End While

        Return False

    End Function

    ''' <summary>
    ''' Moves the selection to the previous available insertion point (previous sibling or before current element in parent)
    ''' </summary>
    ''' <param name="MovedPoint">The selection point to move</param>
    Public Function MoveLeft(Optional MovedPoint As SelectionPointType = SelectionPointType.Selection) As Boolean

        Dim Sel = GetPoint(MovedPoint)

        If Sel.IsAtOrigin Then

            Return MoveBeforeParent(MovedPoint)

        Else

            SetPoint(Sel.Increment(-1), MovedPoint)
            Return True

        End If

    End Function

    ''' <summary>
    ''' Moves the selection to the next available insertion point (next sibling or after current element in parent)
    ''' </summary>
    ''' <param name="MovedPoint">The selection point to move</param>
    Public Function MoveRight(Optional MovedPoint As SelectionPointType = SelectionPointType.Selection) As Boolean

        Dim Sel = GetPoint(MovedPoint)

        If Sel.IsAtEnd Then

            Return MoveAfterParent(MovedPoint)

        Else

            SetPoint(Sel.Increment(1), MovedPoint)
            Return True

        End If

    End Function

    ''' <summary>
    ''' Moves the selection to the beginning of the current element
    ''' </summary>
    ''' <param name="MovedPoint">The selection point to move</param>
    Public Function MoveStart(Optional MovedPoint As SelectionPointType = SelectionPointType.Selection) As Boolean

        Dim Sel = GetPoint(MovedPoint)
        If Sel.IsAtOrigin Then

            ' No change needed
            Return False

        Else

            ' Modify the selection
            SetPoint(Sel.GetOrigin(), MovedPoint)
            Return True

        End If


    End Function

    ''' <summary>
    ''' Moves the selection to the end of the current element
    ''' </summary>
    ''' <param name="MovedPoint">The selection point to move</param>
    Public Function MoveEnd(Optional MovedPoint As SelectionPointType = SelectionPointType.Selection) As Boolean

        Dim Sel = GetPoint(MovedPoint)
        If Sel.IsAtEnd Then

            ' No change needed
            Return False

        Else

            ' Modify the selection
            SetPoint(Sel.GetEnd(), MovedPoint)
            Return True

        End If

    End Function

    Public Function MoveDocumentStart(Optional MovedPoint As SelectionPointType = SelectionPointType.Selection) As Boolean

        Dim DocumentStart = New SelectionPoint(This.ParentDocument, 0)
        If GetPoint(MovedPoint) = DocumentStart Then

            ' No change needed
            Return False

        Else

            ' Modify the selection
            SetPoint(DocumentStart, MovedPoint)
            Return True

        End If

    End Function

    Public Function MoveDocumentEnd(Optional MovedPoint As SelectionPointType = SelectionPointType.Selection) As Boolean

        Dim DocumentEnd = New SelectionPoint(This.ParentDocument, This.ParentDocument.Children.Count)
        If GetPoint(MovedPoint) = DocumentEnd Then

            ' No change needed
            Return False

        Else

            ' Modify the selection
            SetPoint(DocumentEnd, MovedPoint)
            Return True

        End If

    End Function

    ''' <summary>
    ''' Increments the specified point's child index of a certain amount. May throw an ArgumentException if Amount is invalid.
    ''' </summary>
    ''' <param name="Amount">The amount to add to the current child index</param>
    ''' <param name="MovedPoint">The point to move</param>
    Public Function Increment(Amount As Integer, Optional MovedPoint As SelectionPointType = SelectionPointType.Selection) As Boolean
        SetPoint(GetPoint(MovedPoint).Increment(Amount), MovedPoint) : Return True
    End Function

    Private Function GetSelectedElements() As IEnumerable(Of MathElement)
        If IsCollapsed Then Return New MathElement() {}
        Return New SiblingEnumeratorGenerator(LogicalStartPoint, LogicalEndPoint)
    End Function

    Private Function CloneSelectedElements() As IEnumerable(Of MathElement)
        Return GetSelectedElements().Select(Function(el) el.Clone())
    End Function

    Public Sub DeleteContents()
        For Each el In GetSelectedElements().ToArray()
            ParentElement.RemoveChild(el)
        Next
    End Sub

    Public Sub ReplaceContents(NewChilds As IEnumerable(Of MathElement))
        Dim EndPoint = ApparentSEP.NextSibling
        Me.DeleteContents()
        For Each el In NewChilds
            ParentElement.Children.InsertBefore(el, EndPoint)
        Next
        SetSelection(ApparentSEP, ApparentSEP)
    End Sub

    Public Sub ReplaceContents(NewChild As MathElement)
        Dim EndPoint = ApparentSEP.NextSibling
        Me.DeleteContents()
        ParentElement.Children.InsertBefore(NewChild, EndPoint)
        SetSelection(ApparentSEP, ApparentSEP)
    End Sub

End Class
