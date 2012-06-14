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

        Public Sub New(SelectionHost As MathElement, ChildIndex As Integer)

            ' Fields initialization
            Me._Host = SelectionHost : Me.Index = ChildIndex : Me.Document = SelectionHost.ParentDocument

            ' A selection host can't be null, and the index should be valid
            If SelectionHost Is Nothing Then Throw New ArgumentNullException("SelectionHost", "A selection host can't be null")
            ValidateIndex(ChildIndex)

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

            Public Sub New(MovedPoint As SelectionPoint, OldIndex As Integer, NewIndex As Integer)
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
        Private Valid As Boolean = True

        Protected Sub ValidateIndex(ChildIndex As Integer)
            If ChildIndex < 0 Then Throw New ArgumentException("ChildIndex can't be negative. It should be a positive number between 0 and SelectionHost.Children.Count.")
            If ChildIndex > ParentElement.Children.Count Then Throw New ArgumentException("ChildIndex can't be superior to the number of children. It should be a positive number between 0 and SelectionHost.Children.Count.")
        End Sub

        Private Property Index As Integer
            Get
                Return __Index
            End Get
            Set(value As Integer)

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
        Public ReadOnly Property PreviousSibling() As MathElement
            Get
                If ChildIndex = 0 Then Return Nothing
                Return ParentElement.Children.ElementAt(ChildIndex - 1)
            End Get
        End Property

        ''' <summary>
        ''' Gets the first element of the selection host that's after this point (or Nothing if there's no).
        ''' </summary>
        Public ReadOnly Property NextSibling() As MathElement
            Get
                If ChildIndex = ParentElement.Children.Count Then Return Nothing
                Return ParentElement.Children.ElementAt(ChildIndex)
            End Get
        End Property

        ''' <summary>
        ''' Gets the parent document.
        ''' </summary>
        Public ReadOnly Property ParentDocument As MathDocument
            Get
                If IsValid Then
                    If Document Is Nothing Then Document = ParentElement.ParentDocument
                    Return Document
                Else
                    Throw New InvalidOperationException("This selection point has been invalidated.")
                End If
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
                Return (Valid) AndAlso (_Host.ParentDocument Is Document)
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
        Public Shared Function GetApparentSelection(StartPoint As SelectionPoint, EndPoint As SelectionPoint) As Tuple(Of SelectionPoint, SelectionPoint)
            Return StartPoint.GetApparentSelection(EndPoint)
        End Function

        Private Shared Sub GetParentPoint(ByRef Point As SelectionPoint, ByRef Advanced As Boolean)

            If (TypeOf Point.ParentElement Is TextEdit) Then

                ' TextEdit's have special rules
                If Point.IsAtEnd Then
                    Point = Point.ParentSelectionPoint.Increment(1)
                    Advanced = False
                ElseIf Point.IsAtOrigin Then
                    Point = Point.ParentSelectionPoint
                    Advanced = False
                Else
                    Point = Point.ParentSelectionPoint
                    Advanced = True
                End If

            Else

                ' Others are just advanced to parent
                Point = Point.ParentSelectionPoint
                Advanced = True

            End If

        End Sub

        ''' <summary>
        ''' Gets the apparent selection resulting from the specified start and end points.
        ''' </summary>
        ''' <param name="EndPoint">The end point. The start point is the object on which this function is called.</param>
        Public Function GetApparentSelection(EndPoint As SelectionPoint) As Tuple(Of SelectionPoint, SelectionPoint)

            Dim StartPoint = Me

            Dim StartPointAdvanced As Boolean = False
            Dim EndPointAdvanced As Boolean = False

            ' In case we have a document mismatch, throw an exception
            If StartPoint.ParentDocument IsNot EndPoint.ParentDocument Then
                Throw New ArgumentException("EndPoint and StartPoint of a selection should be located in the same document.")
                'Return New Tuple(Of SelectionPoint, SelectionPoint)(StartPoint, StartPoint)
            End If

            ' Start be removing the depht difference between the two points
            Dim DephtDiff = StartPoint.ParentElement.TreeDepht - EndPoint.ParentElement.TreeDepht
            If DephtDiff > 0 Then
                For X As Integer = 1 To DephtDiff
                    GetParentPoint(StartPoint, StartPointAdvanced)
                Next
            ElseIf DephtDiff < 0 Then
                For X As Integer = 1 To -DephtDiff
                    GetParentPoint(EndPoint, EndPointAdvanced)
                Next
            End If

            ' Now, move to parent until the host are identical
            While StartPoint.ParentElement IsNot EndPoint.ParentElement
                GetParentPoint(StartPoint, StartPointAdvanced)
                GetParentPoint(EndPoint, EndPointAdvanced)
            End While

            ' EXCEPTION: Selecting full content inside a text edit select the text edit itself
            If StartPoint.ParentElement.IsTextEdit AndAlso StartPoint.ParentElement.IsLayoutEngine Then
                If StartPoint.IsAtOrigin AndAlso StartPoint.IsAtEnd Then
                    ' First case: Left to Right order
                    StartPoint = StartPoint.ParentElement.GetSelectionBefore()
                    EndPoint = StartPoint.Increment(1)
                    StartPointAdvanced = False
                    EndPointAdvanced = False
                ElseIf StartPoint.IsAtEnd AndAlso EndPoint.IsAtOrigin Then
                    ' Second case: Right to left order
                    EndPoint = StartPoint.ParentElement.GetSelectionBefore()
                    StartPoint = EndPoint.Increment(1)
                    StartPointAdvanced = False
                    EndPointAdvanced = False
                ElseIf StartPoint = EndPoint Then
                    If StartPoint.IsAtOrigin Then
                        ' Third case: Both at left
                        StartPoint = StartPoint.ParentElement.GetSelectionBefore()
                        EndPoint = StartPoint
                        StartPointAdvanced = False
                        EndPointAdvanced = False
                    ElseIf StartPoint.IsAtEnd Then
                        ' Fourth case: Both at right
                        StartPoint = StartPoint.ParentElement.GetSelectionAfter()
                        EndPoint = StartPoint
                        StartPointAdvanced = False
                        EndPointAdvanced = False
                    End If
                End If
            End If

            ' Return the final result
            If StartPoint.ChildIndex < EndPoint.ChildIndex Then
                If EndPointAdvanced Then
                    Return New Tuple(Of SelectionPoint, SelectionPoint)(StartPoint, EndPoint.Increment(1))
                Else
                    Return New Tuple(Of SelectionPoint, SelectionPoint)(StartPoint, EndPoint)
                End If
            Else
                If StartPointAdvanced Then
                    Return New Tuple(Of SelectionPoint, SelectionPoint)(StartPoint.Increment(1), EndPoint)
                Else
                    Return New Tuple(Of SelectionPoint, SelectionPoint)(StartPoint, EndPoint)
                End If
            End If

        End Function

        ''' <summary>
        ''' Returns a new selection point located in the same parent element, but with a different child index
        ''' </summary>
        ''' <param name="ChildIndexChange">The number to add to the current child index</param>
        Public Function Increment(ChildIndexChange As Integer) As SelectionPoint
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

        Private Sub Host_ChildAdded(sender As Object, e As MathElement.TreeEventArgs) Handles _Host.ChildAdded
            ' NOTE : < is not possible because the insertion point should move after the inserted element in the input flow
            If e.ChildIndex <= Me.ChildIndex Then
                Me.Index += 1
            End If
        End Sub

        Private Sub Host_ChildRemoved(sender As Object, e As MathElement.TreeEventArgs) Handles _Host.ChildRemoved
            ' NOTE : <= is not possible because, when the selection content is deleted, end and start point should be egal
            If e.ChildIndex < Me.ChildIndex Then
                Me.Index -= 1
            End If
        End Sub

        Private Sub Host_DetachedFromDocument(sender As Object, e As System.EventArgs) Handles _Host.DetachedFromDocument
            Valid = False : RaiseEvent Invalidated(Me, EventArgs.Empty)
        End Sub

        '++
        '++ Propagate changes
        '++

        Private Sub Parent_Changed(sender As Object, e As ChangeEventArgs) Handles _Parent.Changed, _Parent.ParentChanged
            RaiseEvent ParentChanged(Me, e)
        End Sub

        '++
        '++ Operators
        '++

        Public Shared Operator =(A As SelectionPoint, B As SelectionPoint) As Boolean
            Return (A.ParentElement Is B.ParentElement) AndAlso (A.ChildIndex = B.ChildIndex)
        End Operator

        Public Shared Operator <>(A As SelectionPoint, B As SelectionPoint) As Boolean
            Return Not A = B
        End Operator

        Public Shared Operator <(StartPoint As SelectionPoint, EndPoint As SelectionPoint) As Boolean

            ' In case we have a document mismatch, throw an exception
            If StartPoint.ParentDocument IsNot EndPoint.ParentDocument Then
                Throw New ArgumentException("EndPoint and StartPoint of a selection should be located in the same document.")
                'Return New Tuple(Of SelectionPoint, SelectionPoint)(StartPoint, StartPoint)
            End If

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

            ' Return the final result
            If StartPoint.ChildIndex > EndPoint.ChildIndex Then
                Return False
            Else
                Return False
            End If
        End Operator

        Public Shared Operator >(A As SelectionPoint, B As SelectionPoint) As Boolean
            If A = B Then Return False
            Return Not (A < B)
        End Operator

        Public Shared Operator <=(A As SelectionPoint, B As SelectionPoint) As Boolean
            If A = B Then Return True
            Return A < B
        End Operator

        Public Shared Operator >=(A As SelectionPoint, B As SelectionPoint) As Boolean
            If A = B Then Return True
            Return A > B
        End Operator

    End Class

    ''' <summary>
    ''' Selection changed event args
    ''' </summary>
    Public Class SelectionChangedEventArgs : Inherits EventArgs

        Private OldStartPoint, OldEndPoint, NewStartPoint, NewEndPoint As SelectionPoint
        Public Sub New(OldStartPoint As SelectionPoint, OldEndPoint As SelectionPoint, NewStartPoint As SelectionPoint, NewEndPoint As SelectionPoint)
            Me.NewStartPoint = NewStartPoint
            Me.NewEndPoint = NewEndPoint
            Me.OldStartPoint = OldStartPoint
            Me.OldEndPoint = OldEndPoint
        End Sub

    End Class

End Class
