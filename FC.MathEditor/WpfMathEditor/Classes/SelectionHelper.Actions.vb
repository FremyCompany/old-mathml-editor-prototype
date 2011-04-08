Partial Public Class SelectionHelper

    Public Enum SelectionPointType
        StartPoint
        EndPoint
        Selection
    End Enum

    Public ReadOnly Property IsEmpty As Boolean
        Get
            If SelectionStart Is Nothing Then
                If SelectionEnd Is Nothing Then
                    ' TODO: Implement HasChildren.... in ChildrenHelper
                    Return CommonAncestror.Children.First Is Nothing
                Else
                    Return False
                End If
            Else
                Return SelectionStart.NextSibling Is SelectionEnd
            End If
        End Get
    End Property

    Public Sub MoveNext(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)
        Dim Sel As Selection = GetSelection(MovedPoint)
        Dim E = Sel.CommonAncestror.ParentLayoutEngineChild
        Sel.CommonAncestror = E.Parent
        Sel.SelectionStart = E
        Sel.SelectionEnd = E.NextSibling
        SetSelection(Sel, MovedPoint)
    End Sub

    Public Sub MovePrevious(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)
        Dim Sel As Selection = GetSelection(MovedPoint)
        Dim E = Sel.CommonAncestror.ParentLayoutEngineChild
        Sel.CommonAncestror = E.Parent
        Sel.SelectionStart = E.PreviousSibling
        Sel.SelectionEnd = E
        SetSelection(Sel, MovedPoint)
    End Sub

    Public Sub MoveLeft(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        Dim Sel As Selection = GetSelection(MovedPoint)
        Dim E = Sel.SelectionStart

        If E Is Nothing Then
            MovePrevious(MovedPoint)
        Else
            Sel.CommonAncestror = E.Parent
            Sel.SelectionStart = E.PreviousSibling
            Sel.SelectionEnd = E
            SetSelection(Sel, MovedPoint)
        End If

    End Sub

    Public Sub MoveRight(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        Dim Sel As Selection = GetSelection(MovedPoint)
        Dim E = Sel.SelectionEnd

        If E Is Nothing Then
            MoveNext(MovedPoint)
        Else
            Sel.CommonAncestror = E.Parent
            Sel.SelectionStart = E
            Sel.SelectionEnd = E.NextSibling
            SetSelection(Sel, MovedPoint)
        End If

    End Sub

    Public Sub MoveStart(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        Dim Sel As Selection = GetSelection(MovedPoint)
        Sel.SelectionStart = Nothing
        Sel.SelectionEnd = Sel.CommonAncestror.FirstChild
        SetSelection(Sel, MovedPoint)

    End Sub

    Public Sub MoveEnd(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        Dim Sel As Selection = GetSelection(MovedPoint)
        Sel.SelectionEnd = Nothing
        Sel.SelectionStart = Sel.CommonAncestror.LastChild
        SetSelection(Sel, MovedPoint)

    End Sub

    Public Sub MoveDocumentStart(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        SetSelection(New Selection(CommonAncestror.Root, Nothing, CommonAncestror.Root.FirstChild), MovedPoint)

    End Sub

    Public Sub MoveDocumentEnd(Optional ByVal MovedPoint As SelectionPointType = SelectionPointType.Selection)

        SetSelection(New Selection(CommonAncestror.Root, CommonAncestror.Root.LastChild, Nothing), MovedPoint)

    End Sub

End Class
