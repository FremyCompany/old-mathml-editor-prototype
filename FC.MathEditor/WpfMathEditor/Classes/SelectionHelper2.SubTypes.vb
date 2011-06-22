Partial Public Class SelectionHelper

    ''' <summary>
    ''' Enumeration containing the direction of a selection
    ''' </summary>
    Public Enum SelectionDirection
        LTR = +1 : RTL = -1
    End Enum

    ''' <summary>
    ''' Class that constructs a reference to a specific location in a tree and track his change in a document
    ''' </summary>
    Public Class SelectionPoint

        '++
        '++ Constructors
        '++

        Public Sub New(ByVal SelectionHost As MathElement, ByVal ChildIndex As Integer)

            ' A selection host can't be null, and the index should be valid
            If SelectionHost Is Nothing Then Throw New ArgumentNullException("SelectionHost", "A selection host can't be null")
            If ChildIndex < 0 Then Throw New ArgumentException("ChildIndex can't be negative. It should be a positive number between 0 and SelectionHost.Children.Count.")
            If ChildIndex > SelectionHost.Children.Count Then Throw New ArgumentException("ChildIndex can't be superior to the number of children. It should be a positive number between 0 and SelectionHost.Children.Count.")

            ' An element should have a parent document in order to be a valid SelectionHost target
            Document = SelectionHost.ParentDocument
            If Document Is Nothing Then Throw New ArgumentException("An element should have a parent document in order to be a valid SelectionHost target")

            ' Fields initialization
            Me._Host = SelectionHost : Me.Index = ChildIndex

            ' Parent initialization
            If SelectionHost.ParentElement IsNot Nothing Then
                _Parent = New SelectionPoint(SelectionHost.ParentElement, SelectionHost.ParentElement.Children.IndexOf(SelectionHost))
            End If

        End Sub

        '++
        '++ Events
        '++

        Public Event ParentChanged As EventHandler(Of ChangeEventArgs)
        Public Event Changed As EventHandler(Of ChangeEventArgs)
        Public Event Invalidated As EventHandler

        ''' <summary>
        ''' Defines the arguments sent to callbacks when a selection point has moved
        ''' </summary>
        Public Class ChangeEventArgs : Inherits EventArgs

            Public Property MovedPoint As SelectionPoint
            Public Property OldIndex As Integer
            Public Property NewIndex As Integer

            Public Sub New(ByVal MovedPoint As SelectionPoint, ByVal OldIndex As Integer, ByVal NewIndex As Integer)
                Me.MovedPoint = MovedPoint : Me.OldIndex = OldIndex : Me.NewIndex = NewIndex
            End Sub

        End Class

        '++
        '++ Properties
        '++

        Private __Index As Integer = 0
        Private Document As MathDocument
        Private WithEvents _Host As MathElement
        Private WithEvents _Parent As SelectionPoint
        Private Valid As Boolean

        Private Property Index As Integer
            Get
                Return __Index
            End Get
            Set(ByVal value As Integer)

                If Index <> value Then

                    Dim e = New SelectionPoint.ChangeEventArgs(Me, Index, value) : __Index = value
                    RaiseEvent Changed(Me, e)

                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets the index of the child.
        ''' </summary>
        ''' <value>
        ''' The index of the child.
        ''' </value>
        Public ReadOnly Property ChildIndex As Integer
            Get
                If IsValid Then Return Index
                Throw New InvalidOperationException("This selection point has been invalidated.")
            End Get
        End Property

        ''' <summary>
        ''' Gets the selection host.
        ''' </summary>
        Public ReadOnly Property ParentElement As MathElement
            Get
                If IsValid Then Return _Host
                Throw New InvalidOperationException("This selection point has been invalidated.")
            End Get
        End Property

        ''' <summary>
        ''' Gets the last element of the selection host that's before this point (or Nothing if there's no).
        ''' </summary>
        Public ReadOnly Property Start() As MathElement
            Get
                If ChildIndex = 0 Then Return Nothing
                Return ParentElement.Children.ElementAt(ChildIndex - 1)
            End Get
        End Property

        ''' <summary>
        ''' Gets the first element of the selection host that's after this point (or Nothing if there's no).
        ''' </summary>
        Public ReadOnly Property [End]() As MathElement
            Get
                Return ParentElement.Children.ElementAt(ChildIndex)
            End Get
        End Property

        ''' <summary>
        ''' Gets the parent document.
        ''' </summary>
        Public ReadOnly Property ParentDocument As MathDocument
            Get
                Return Document
            End Get
        End Property

        ''' <summary>
        ''' Gets the parent selection point.
        ''' </summary>
        Public ReadOnly Property ParentSelectionPoint As SelectionPoint
            Get
                Return _Parent
            End Get
        End Property

        ''' <summary>
        ''' Gets the first valid parent.
        ''' </summary>
        Public ReadOnly Property FirstValidParent() As SelectionPoint
            Get

                ' Seek valid point
                FirstValidParent = Me
                While Not FirstValidParent.IsValid : FirstValidParent = FirstValidParent.ParentSelectionPoint : End While

                ' Return result
                Return FirstValidParent

            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance is valid.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property IsValid As Boolean
            Get
                Return Valid
            End Get
        End Property

        ''' <summary>
        ''' Returns True when the selection point is before every ParentElement child
        ''' </summary>
        Public ReadOnly Property IsAtOrigin As Boolean
            Get
                Return ChildIndex = 0
            End Get
        End Property

        ''' <summary>
        ''' Returns True when the selection point is after every ParentElement child
        ''' </summary>
        Public ReadOnly Property IsAtEnd As Boolean
            Get
                Return ChildIndex = ParentElement.Children.Count
            End Get
        End Property


        '++
        '++ Work with multiple points
        '++

        ''' <summary>
        ''' Gets the apparent selection resulting from the specified start and end points.
        ''' </summary>
        ''' <param name="StartPoint">The start point.</param>
        ''' <param name="EndPoint">The end point.</param>
        Public Shared Function GetApparentSelection(ByVal StartPoint As SelectionPoint, ByVal EndPoint As SelectionPoint) As Tuple(Of SelectionPoint, SelectionPoint)
            Return StartPoint.GetApparentSelection(EndPoint)
        End Function

        ''' <summary>
        ''' Gets the apparent selection resulting from the specified start and end points.
        ''' </summary>
        ''' <param name="EndPoint">The end point. The start point is the object on which this function is called.</param>
        Public Function GetApparentSelection(ByVal EndPoint As SelectionPoint) As Tuple(Of SelectionPoint, SelectionPoint)

            Dim StartPoint = Me

            ' Start be removing the depht difference between the two points
            Dim DephtDiff = StartPoint.ParentElement.TreeDepht - EndPoint.ParentElement.TreeDepht
            If DephtDiff > 0 Then
                For X As Integer = 1 To DephtDiff
                    StartPoint = StartPoint.ParentSelectionPoint
                Next
            ElseIf DephtDiff < 0 Then
                For X As Integer = 1 To -DephtDiff
                    EndPoint = EndPoint.ParentSelectionPoint
                Next
            End If

            ' Now, move to parent until the host are identical
            While StartPoint.ParentElement IsNot EndPoint.ParentElement
                StartPoint = StartPoint.ParentSelectionPoint
                EndPoint = EndPoint.ParentSelectionPoint
            End While

            ' Return the response
            Return New Tuple(Of SelectionPoint, SelectionPoint)(StartPoint, EndPoint)

        End Function

        ''' <summary>
        ''' Returns a new selection point located in the same parent element, but with a different child index
        ''' </summary>
        ''' <param name="ChildIndexChange">The number to add to the current child index</param>
        Public Function Increment(ByVal ChildIndexChange As Integer) As SelectionPoint
            Return New SelectionPoint(ParentElement, ChildIndex + ChildIndexChange)
        End Function

        ''' <summary>
        ''' Returns a new selection point located in the same parent element, but at its origin
        ''' </summary>
        Public Function GetOrigin() As SelectionPoint
            Return New SelectionPoint(ParentElement, 0)
        End Function

        ''' <summary>
        ''' Returns a new selection point located in the same parent element, but at its end
        ''' </summary>
        Public Function GetEnd() As SelectionPoint
            Return New SelectionPoint(ParentElement, ParentElement.Children.Count)
        End Function

        '++
        '++ Detect changes
        '++

        Private Sub Host_ChildAdded(ByVal sender As Object, ByVal e As MathElement.TreeEventArgs) Handles _Host.ChildAdded
            If e.ChildIndex <= Me.ChildIndex Then
                Me.Index += 1
            End If
        End Sub

        Private Sub Host_ChildRemoved(ByVal sender As Object, ByVal e As MathElement.TreeEventArgs) Handles _Host.ChildRemoved
            If e.ChildIndex <= Me.ChildIndex Then
                Me.Index -= 1
            End If
        End Sub

        Private Sub Host_DetachedFromDocument(ByVal sender As Object, ByVal e As System.EventArgs) Handles _Host.DetachedFromDocument
            Valid = False : RaiseEvent Invalidated(Me, EventArgs.Empty)
        End Sub

        '++
        '++ Propagate changes
        '++

        Private Sub Parent_Changed(ByVal sender As Object, ByVal e As ChangeEventArgs) Handles _Parent.Changed, _Parent.ParentChanged
            RaiseEvent ParentChanged(Me, e)
        End Sub

    End Class

    ''' <summary>
    ''' Selection changed event args
    ''' </summary>
    Public Class SelectionChangedEventArgs : Inherits EventArgs

        Private OldStartPoint, OldEndPoint, NewStartPoint, NewEndPoint As SelectionPoint
        Public Sub New(ByVal OldStartPoint As SelectionPoint, ByVal OldEndPoint As SelectionPoint, ByVal NewStartPoint As SelectionPoint, ByVal NewEndPoint As SelectionPoint)
            Me.NewStartPoint = NewStartPoint
            Me.NewEndPoint = NewEndPoint
            Me.OldStartPoint = OldStartPoint
            Me.OldEndPoint = OldEndPoint
        End Sub

    End Class


End Class
