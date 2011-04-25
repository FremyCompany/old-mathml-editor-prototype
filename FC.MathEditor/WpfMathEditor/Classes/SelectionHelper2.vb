Partial Public Class SelectionHelper2

    Private WithEvents SH As MathElement
    Private SSI, SEI As Integer

    Private ReadOnly Property SS As MathElement
        Get
            Return SH.Children.ElementAt(SSI)
        End Get
    End Property

    Private ReadOnly Property SE As MathElement
        Get
            Return SH.Children.ElementAt(SEI)
        End Get
    End Property

    Public ReadOnly Property [Start]() As MathElement
        Get
            Return SS
        End Get
    End Property

    Public ReadOnly Property [End]() As MathElement
        Get
            Return SE
        End Get
    End Property

    Public ReadOnly Property [Host] As MathElement
        Get
            Return SH
        End Get
    End Property

    Public Class SelectionPoint

        Public Sub New(ByVal SelectionHost As MathElement, ByVal ChildIndex As Integer)

            ' A selection host can't be null, and the index should be valid
            If SelectionHost Is Nothing Then Throw New ArgumentNullException("SelectionHost", "A selection host can't be null")
            If ChildIndex < 0 Then Throw New ArgumentException("ChildIndex can't be negative. It should be a positive number between 0 and SelectionHost.Children.Count.")
            If ChildIndex > SelectionHost.Children.Count Then Throw New ArgumentException("ChildIndex can't be superior to the number of children. It should be a positive number between 0 and SelectionHost.Children.Count.")

            ' An element should have a parent document in order to be a valid SelectionHost target
            Document = SelectionHost.ParentDocument
            If Document Is Nothing Then Throw New ArgumentException("An element should have a parent document in order to be a valid SelectionHost target")

            ' Fields initialization
            Me.Host = SelectionHost : Me.Index = ChildIndex

            ' Parent initialization
            If SelectionHost.Parent IsNot Nothing Then
                Parent = New SelectionPoint(SelectionHost.Parent, SelectionHost.Parent.Children.IndexOf(SelectionHost))
            End If

        End Sub

        '++
        '++ Events
        '++

        Public Event ParentChanged As EventHandler(Of ChangeEventArgs)
        Public Event Changed As EventHandler(Of ChangeEventArgs)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
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

        Private _Index As Integer = 0
        Private Document As MathDocument
        Private WithEvents Host As MathElement
        Private WithEvents Parent As SelectionPoint
        Private Valid As Boolean

        Private Property Index As Integer
            Get
                Return _Index
            End Get
            Set(ByVal value As Integer)

                If Index <> value Then

                    Dim e = New SelectionPoint.ChangeEventArgs(Me, Index, value) : _Index = value
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
        Public ReadOnly Property SelectionHost As MathElement
            Get
                If IsValid Then Return Host
                Throw New InvalidOperationException("This selection point has been invalidated.")
            End Get
        End Property

        ''' <summary>
        ''' Gets the last element of the selection host that's before this point (or Nothing if there's no).
        ''' </summary>
        Public ReadOnly Property SelectionStart() As MathElement
            Get
                Return Host.Children.ElementAt(ChildIndex - 1)
            End Get
        End Property

        ''' <summary>
        ''' Gets the first element of the selection host that's after this point (or Nothing if there's no).
        ''' </summary>
        Public ReadOnly Property SelectionEnd() As MathElement
            Get
                Return Host.Children.ElementAt(ChildIndex)
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
                Return Parent
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

        '++
        '++ Detect changes
        '++

        Private Sub Host_ChildAdded(ByVal sender As Object, ByVal e As MathElement.TreeEventArgs) Handles Host.ChildAdded
            If e.ChildIndex <= Me.ChildIndex Then
                Me.Index += 1
            End If
        End Sub

        Private Sub Host_ChildRemoved(ByVal sender As Object, ByVal e As MathElement.TreeEventArgs) Handles Host.ChildRemoved
            If e.ChildIndex <= Me.ChildIndex Then
                Me.Index -= 1
            End If
        End Sub

        Private Sub Host_DetachedFromDocument(ByVal sender As Object, ByVal e As System.EventArgs) Handles Host.DetachedFromDocument
            Valid = False
        End Sub

        '++
        '++ Propatage changes
        '++

        Private Sub Parent_Changed(ByVal sender As Object, ByVal e As ChangeEventArgs) Handles Parent.Changed, Parent.ParentChanged
            RaiseEvent ParentChanged(Me, e)
        End Sub

    End Class

End Class
