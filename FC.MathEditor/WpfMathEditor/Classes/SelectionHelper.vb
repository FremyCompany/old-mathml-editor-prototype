Partial Public Class SelectionHelper

    Private This As MathDocument
    Public Sub New(ByVal This As MathDocument)
        Me.This = This
        SetSelection(This, Nothing, Nothing)
    End Sub

    Private IsSelectionChanging As Boolean
    Public Event BeforeSelectionChanged As EventHandler
    Public Event SelectionChanged As EventHandler

    Private Function GetSelectedElements() As IEnumerable(Of MathElement)
        Return New SiblingEnumerator(CommonAncestror.Children.After(SelectionStart), CommonAncestror.Children.Before(SelectionEnd))
    End Function

    Private Function CloneSelectedElements() As IEnumerable(Of MathElement)
        Return GetSelectedElements().Select(Function(el) el.Clone())
    End Function

    Public Sub DeleteContents()
        For Each el In GetSelectedElements().ToArray()
            CommonAncestror.RemoveChild(el)
        Next
    End Sub

    Private Sub SetSelection(ByVal StartPoint As Selection, ByVal EndPoint As Selection)

        ' Compute _Selection
        Dim CA = StartPoint.SelectionStart.GetCommonAncestrorWith(EndPoint.SelectionEnd)
        Dim SS = StartPoint.SelectionStart
        Dim SE = StartPoint.SelectionEnd

        While SS.Parent IsNot CA
            SS = SS.Parent
        End While

        While SE.Parent IsNot SE
            SE = SE.Parent
        End While

        ' Insert StartPoint and EndPoint if needed
        Dim Dir = SelectionDirection.LTR

        If SE.IsBefore(SS) Then
            Dir = SelectionDirection.RTL
            Dim Temp = StartPoint
            StartPoint = EndPoint
            EndPoint = Temp
        End If

        ' Set the selection
        SetSelection_Internal(New Selection(CA, SS, SE), StartPoint, EndPoint)

    End Sub

    Public Sub SetSelection(ByVal Point As Selection, Optional ByVal PointToChange As SelectionPointType = SelectionPointType.Selection)
        Select Case PointToChange
            Case SelectionPointType.Selection
                SetSelection(Point.CommonAncestror, Point.SelectionStart, Point.SelectionEnd)
            Case SelectionPointType.StartPoint
                SetSelection(Point, EndPoint)
            Case SelectionPointType.EndPoint
                SetSelection(StartPoint, Point)
            Case Else
                Throw New ArgumentException("Unknown value for the selection point type", "PointToChange")
        End Select
    End Sub

    Public Function GetSelection(ByVal PointToRetreive As SelectionPointType) As Selection
        Select Case PointToRetreive
            Case SelectionPointType.Selection
                Return _Selection
            Case SelectionPointType.StartPoint
                Return _StartPoint
            Case SelectionPointType.EndPoint
                Return _EndPoint
            Case Else
                Throw New ArgumentException("Unknown value for the selection point type", "PointToRetreive")
        End Select
    End Function

    Public Sub SetSelection(ByVal CommonAncestror As MathElement, ByVal SelectionStart As MathElement, ByVal SelectionEnd As MathElement)
        ' Check if the selection is valid
        If (CommonAncestror.ParentDocument Is This) AndAlso (SelectionStart.Parent Is CommonAncestror) AndAlso (SelectionEnd.Parent Is CommonAncestror) Then

            SetSelection_Internal(
                New Selection(CommonAncestror, SelectionStart, SelectionEnd),
                New Selection(CommonAncestror, SelectionStart, CommonAncestror.Children.After(SelectionStart)),
                New Selection(CommonAncestror, CommonAncestror.Children.Before(SelectionEnd), SelectionEnd)
            )

        End If
    End Sub

    Private Sub SetSelection_Internal(ByVal Selection As Selection, ByVal StartPoint As Selection, ByVal EndPoint As Selection)

        ' Verify the direction of the selection
        If Selection.SelectionEnd.IsBefore(SelectionStart) Then
            Dim Temp = Selection.SelectionEnd
            Selection.SelectionEnd = Selection.SelectionStart
            Selection.SelectionStart = Temp
            _Dir = SelectionDirection.RTL
        Else
            _Dir = SelectionDirection.LTR
        End If

        ' Raise the BeforeSelectionChanged event
        If Not IsSelectionChanging Then
            RaiseEvent BeforeSelectionChanged(Me, EventArgs.Empty)
        End If

        ' Perform the changes
        _Selection = Selection
        _StartPoint = StartPoint
        _EndPoint = EndPoint

    End Sub

    Public Sub ReverseSelection()
        Dim Temp = _StartPoint
        _StartPoint = _EndPoint
        _EndPoint = Temp
        _Dir = Not _Dir
    End Sub

    Private _Selection As Selection
    Public ReadOnly Property CommonAncestror As MathElement
        Get
            Return _Selection.CommonAncestror
        End Get
    End Property

    Public ReadOnly Property SelectionStart As MathElement
        Get
            Return _Selection.SelectionStart
        End Get
    End Property

    Public ReadOnly Property SelectionEnd As MathElement
        Get
            Return _Selection.SelectionEnd
        End Get
    End Property

    Private _StartPoint As Selection
    Public ReadOnly Property StartPoint As Selection
        Get
            Return _StartPoint
        End Get
    End Property

    Private _EndPoint As Selection
    Public ReadOnly Property EndPoint As Selection
        Get
            Return _EndPoint
        End Get
    End Property

    Private _Dir As SelectionDirection
    Public ReadOnly Property Direction As SelectionDirection
        Get
            Return _Dir
        End Get
    End Property

    Public Structure Selection

        ''' <summary>
        ''' Creates a new instance of the Selection object and fills its propertes
        ''' </summary>
        ''' <param name="CA">CA</param>
        ''' <param name="SS">SS</param>
        ''' <param name="SE">SE</param>
        Public Sub New(ByVal CA As MathElement, ByVal SS As MathElement, ByVal SE As MathElement)
            CommonAncestror = CA : SelectionStart = SS : SelectionEnd = SE
        End Sub

        Public CommonAncestror As MathElement
        Public SelectionStart As MathElement
        Public SelectionEnd As MathElement

    End Structure

    Public Enum SelectionDirection
        LTR = 0
        RTL = -1
    End Enum

End Class
