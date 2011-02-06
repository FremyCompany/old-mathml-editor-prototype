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

    Private Sub DeleteContents()
        For Each el In GetSelectedElements().ToArray()
            CommonAncestror.RemoveChild(el)
        Next
    End Sub

    Private Sub SetSelection(ByVal StartPoint As Selection, ByVal EndPoint As Selection)
        ' Compute _Selection
        _StartPoint = StartPoint
        _EndPoint = EndPoint
    End Sub

    Private Sub SetSelection(ByVal CommonAncestror As MathElement, ByVal SelectionStart As MathElement, ByVal SelectionEnd As MathElement)
        ' Check if the selection is valid
        If (CommonAncestror.ParentDocument Is This) AndAlso (SelectionStart.Parent Is CommonAncestror) AndAlso (SelectionEnd.Parent Is CommonAncestror) Then


        End If
    End Sub

    Private Sub SetSelection_Internal(ByVal Selection As Selection, ByVal StartPoint As Selection, ByVal EndPoint As Selection)
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

        ' Perform the change
        _StartPoint = New Selection() With {.CommonAncestror = CommonAncestror, .SelectionStart = SelectionStart, .SelectionEnd = CommonAncestror.Children.After(SelectionStart)}
        _EndPoint = New Selection() With {.CommonAncestror = CommonAncestror, .SelectionStart = CommonAncestror.Children.Before(SelectionEnd), .SelectionEnd = SelectionEnd}

    End Sub

    Public Sub ReverseSelection()
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
