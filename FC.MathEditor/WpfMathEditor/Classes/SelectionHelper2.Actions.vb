Partial Public Class SelectionHelper

    Public Enum SelectionPointType
        StartPoint
        EndPoint
        Selection
    End Enum

    Public ReadOnly Property IsEmpty As Boolean
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
    Public Sub CollapseToEnd(Optional ByVal UseApparentSelection As Boolean = True)
        If UseApparentSelection Then
            SetSelection(ApparentSEP, ApparentSEP)
        Else
            SetSelection(SEP, SEP)
        End If
    End Sub

    ''' <summary>
    ''' Sets the selection end point to the selection start point
    ''' </summary>
    ''' <param name="UseApparentSelection">True if you want to collapse the selection to its apparent start point, false if you want it to collapse to its true start point</param>
    Public Sub CollapseToStart(Optional ByVal UseApparentSelection As Boolean = True)
        If UseApparentSelection Then
            SetSelection(ApparentSSP, ApparentSSP)
        Else
            SetSelection(SSP, SSP)
        End If
    End Sub

    ''' <summary>
    ''' Moves the selection to the next available parent location
    ''' </summary>
    ''' <param name="MovedPoint">The point to move</param>
    ''' <remarks>See MoveRight for a better result</remarks>
    Public Sub MoveNext(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        Dim P = GetSelection(MovedPoint)
        Dim E = P.ParentElement

        While E.ParentElement IsNot Nothing

            ' Try to find the next hot spot
            Select Case E.ParentElement.Children.ElementType

                Case MathElement.Type.Formatter
                    Dim Result = E.ParentElement.GetNextInputElement(E)
                    If Result IsNot Nothing Then
                        SetSelection(Result, Nothing, Result.FirstChild)
                        Exit Sub
                    End If

                Case MathElement.Type.LayoutEngine
                    Dim Result = E.NextSibling
                    If Result IsNot Nothing Then
                        SetSelection(E.ParentElement, E, Result)
                        Exit Sub
                    End If

            End Select

            ' Move the the parent level
            E = E.ParentElement

        End While

    End Sub

    ''' <summary>
    ''' Moves the selection to the next available parent location (backward)
    ''' </summary>
    ''' <param name="MovedPoint">The point to move</param>
    ''' <remarks>See MoveLeft for a better result</remarks>
    Public Sub MovePrevious(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        Dim P = GetSelection(MovedPoint)
        Dim E = P.ParentElement

        While E.ParentElement IsNot Nothing

            ' Try to find the next hot spot
            Select Case E.ParentElement.Children.ElementType

                Case MathElement.Type.Formatter
                    Dim Result = E.ParentElement.GetPreviousInputElement(E)
                    If Result IsNot Nothing Then
                        SetSelection(Result, Result.LastChild, Nothing)
                        Exit Sub
                    End If

                Case MathElement.Type.LayoutEngine
                    Dim Result = E.PreviousSibling
                    If Result IsNot Nothing Then
                        SetSelection(E.ParentElement, E, Result)
                        Exit Sub
                    End If

            End Select

            ' Move the the parent level
            E = E.ParentElement

        End While
    End Sub

    ''' <summary>
    ''' Moves the selection to the previous available insertion point (previous sibling or before current element in parent)
    ''' </summary>
    ''' <param name="MovedPoint">The selection point to move</param>
    Public Sub MoveLeft(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        Dim Sel = GetSelection(MovedPoint)

        If Sel.IsAtOrigin Then
            MovePrevious(MovedPoint)
        Else
            SetSelection(Sel.Increment(-1), MovedPoint)
        End If

    End Sub

    ''' <summary>
    ''' Moves the selection to the next available insertion point (next sibling or after current element in parent)
    ''' </summary>
    ''' <param name="MovedPoint">The selection point to move</param>
    Public Sub MoveRight(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        Dim Sel = GetSelection(MovedPoint)

        If Sel.IsAtEnd Then
            MoveNext(MovedPoint)
        Else
            SetSelection(Sel.Increment(1), MovedPoint)
        End If

    End Sub

    ''' <summary>
    ''' Moves the selection to the beginning of the current element
    ''' </summary>
    ''' <param name="MovedPoint">The selection point to move</param>
    Public Sub MoveStart(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        SetSelection(
            GetSelection(MovedPoint).GetOrigin(),
            MovedPoint
        )

    End Sub

    ''' <summary>
    ''' Moves the selection to the end of the current element
    ''' </summary>
    ''' <param name="MovedPoint">The selection point to move</param>
    Public Sub MoveEnd(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        SetSelection(
            GetSelection(MovedPoint).GetEnd(),
            MovedPoint
        )

    End Sub

    Public Sub MoveDocumentStart(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        SetSelection(
            New SelectionPoint(This.ParentDocument, 0),
            MovedPoint
        )

    End Sub

    Public Sub MoveDocumentEnd(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        SetSelection(
            New SelectionPoint(This.ParentDocument, This.ParentDocument.Children.Count),
            MovedPoint
        )

    End Sub

End Class