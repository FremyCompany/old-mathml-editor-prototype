Partial Public Class SelectionHelper
    Implements IEnumerable(Of MathElement)

    '++
    '++ Constructor
    '++

    Public Sub New(Document As MathDocument)

        This = Document

        SSP = New SelectionPoint(Document, Document.Children.Count)
        SEP = New SelectionPoint(Document, Document.Children.Count)
        ApparentSSP = New SelectionPoint(Document, Document.Children.Count)
        ApparentSEP = New SelectionPoint(Document, Document.Children.Count)

    End Sub

    '++
    '++ Events
    '++

    ''' <summary>
    ''' Event risen when any of the selection point is being modified
    ''' </summary>
    Public Event Changing As EventHandler(Of SelectionChangedEventArgs)

    ''' <summary>
    ''' Event risen when any of the selection point is modified
    ''' </summary>
    Public Event Changed As EventHandler

    ''' <summary>
    ''' Called when the selection end or start point is being invalidated
    ''' </summary>
    Private Sub SP_Invalidated(sender As Object, e As System.EventArgs) Handles SEP.Invalidated, SSP.Invalidated
        SetSelection(SSP.FirstValidParent, SEP.FirstValidParent)
    End Sub

    '++
    '++ Private fields
    '++

    Private This As MathDocument

    Private WithEvents SSP As SelectionPoint
    Private WithEvents SEP As SelectionPoint

    Private WithEvents ApparentSSP As SelectionPoint
    Private WithEvents ApparentSEP As SelectionPoint

    ''' <summary>
    ''' Return LTR if the selection points are in the logical order, RTL otherly.
    ''' </summary>
    Public ReadOnly Property Direction As SelectionDirection
        Get
            If ApparentSEP.ChildIndex < ApparentSSP.ChildIndex Then
                Return SelectionDirection.RTL
            Else
                Return SelectionDirection.LTR
            End If
        End Get
    End Property

    Private ReadOnly Property SS As MathElement
        Get
            If Direction = SelectionDirection.LTR Then
                Return ApparentSSP.PreviousSibling
            Else
                Return ApparentSEP.PreviousSibling
            End If
        End Get
    End Property
    Private ReadOnly Property SE As MathElement
        Get
            If Direction = SelectionDirection.LTR Then
                Return ApparentSEP.NextSibling
            Else
                Return ApparentSSP.NextSibling
            End If
        End Get
    End Property

    ''' <summary>
    ''' Gets the last host' child that's before the selection (or Nothing)
    ''' </summary>
    Public ReadOnly Property PreviousSibling() As MathElement
        Get
            Return SS
        End Get
    End Property

    ''' <summary>
    ''' Gets the first host' child that's after the selection (or Nothing)
    ''' </summary>
    Public ReadOnly Property NextSibling() As MathElement
        Get
            Return SE
        End Get
    End Property

    ''' <summary>
    ''' Returns the first selection point, in the logical order
    ''' </summary>
    Public ReadOnly Property LogicalStartPoint() As SelectionPoint
        Get
            If Direction = SelectionDirection.LTR Then
                Return ApparentSSP
            Else
                Return ApparentSEP
            End If
        End Get
    End Property

    ''' <summary>
    ''' Returns the last selection point, in the logical order
    ''' </summary>
    Public ReadOnly Property LogicalEndPoint() As SelectionPoint
        Get
            If Direction = SelectionDirection.LTR Then
                Return ApparentSEP
            Else
                Return ApparentSSP
            End If
        End Get
    End Property

    ''' <summary>
    ''' Returns the selection start point
    ''' </summary>
    Public ReadOnly Property StartPoint() As SelectionPoint
        Get
            Return ApparentSSP
        End Get
    End Property

    ''' <summary>
    ''' Returns the selection end point
    ''' </summary>
    Public ReadOnly Property EndPoint() As SelectionPoint
        Get
            Return ApparentSEP
        End Get
    End Property

    ''' <summary>
    ''' Returns the element in which the selection occurs
    ''' </summary>
    Public ReadOnly Property ParentElement As MathElement
        Get
            Return ApparentSSP.ParentElement
        End Get
    End Property

    ''' <summary>
    ''' Returns the document in which the selection occurs
    ''' </summary>
    Public ReadOnly Property ParentDocument As MathDocument
        Get
            Return This
        End Get
    End Property

    Public Sub SetSelection(StartPoint As SelectionPoint, EndPoint As SelectionPoint)

        ' Find the apparent selection for the specified points
        Dim R = StartPoint.GetApparentSelection(EndPoint)

        ' Save the previous state
        Dim E As New SelectionChangedEventArgs(Me.SSP, Me.SEP, StartPoint, EndPoint)

        ' Raise the Chaning event
        RaiseEvent Changing(Me, E)

        ' Perform the changes
        SSP = StartPoint
        SEP = EndPoint
        ApparentSSP = R.Item1
        ApparentSEP = R.Item2

        ' Raise the Changed event
        RaiseEvent Changed(Me, E)

    End Sub

    Public Sub SetPoint(NewPoint As SelectionPoint, Optional PointToChange As SelectionPointType = SelectionPointType.Selection)
        Select Case PointToChange
            Case SelectionPointType.Selection
                SetSelection(NewPoint, NewPoint)
            Case SelectionPointType.StartPoint
                SetSelection(NewPoint, SEP)
            Case SelectionPointType.EndPoint
                SetSelection(SSP, NewPoint)
            Case Else
                Throw New ArgumentException("Unknown value for the selection point type", "PointToChange")
        End Select
    End Sub

    Public Function GetPoint(PointToRetreive As SelectionPointType) As SelectionPoint
        Select Case PointToRetreive
            Case SelectionPointType.Selection
                If IsCollapsed Then Return ApparentSEP
                Throw New InvalidOperationException("It's not possible to get a single point from a non-collapsed selection")
            Case SelectionPointType.StartPoint
                Return SSP
            Case SelectionPointType.EndPoint
                Return SEP
            Case Else
                Throw New ArgumentException("Unknown value for the selection point type", "PointToRetreive")
        End Select
    End Function

    Public Sub SetSelection2(CommonAncestror As MathElement, SelectionStart As MathElement, SelectionEnd As MathElement)

        ' Check if the selection is valid
        If (
            (CommonAncestror IsNot Nothing) _
            AndAlso (CommonAncestror.ParentDocument Is This) _
            AndAlso (SelectionStart Is Nothing OrElse SelectionStart.ParentElement Is CommonAncestror) _
            AndAlso (SelectionEnd Is Nothing OrElse SelectionEnd.ParentElement Is CommonAncestror)
        ) Then

            ' Set the selection
            SetSelection(
                New SelectionPoint(CommonAncestror, If(SelectionStart Is Nothing, 0, SelectionStart.ChildIndex + 1)),
                New SelectionPoint(CommonAncestror, If(SelectionEnd Is Nothing, CommonAncestror.Children.Count, SelectionEnd.ChildIndex))
            )

        Else

            ' Notify of an error
            Throw New ArgumentException("Selection was invalid.")

        End If

    End Sub

    '++
    '++ Enumerator
    '++

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement) Implements System.Collections.Generic.IEnumerable(Of MathElement).GetEnumerator
        Return New SiblingEnumerator(LogicalStartPoint, LogicalEndPoint)
    End Function

    Private Function GetGenericEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

End Class
