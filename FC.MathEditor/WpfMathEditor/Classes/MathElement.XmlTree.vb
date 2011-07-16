'?
'? Split MathElement into two classes: MathNode and MathElement (ME inherits from MN)
'?

Partial Public MustInherit Class MathElement

    '++ 
    '++ XML Hierarchy
    '++

    Private _Parent As MathElement

    ''' <summary>
    ''' Returns the element which attached this element.
    ''' </summary>
    Public ReadOnly Property ParentElement As MathElement
        Get
            Return _Parent
        End Get
    End Property

    ''' <summary>
    ''' Changes the Parent of NewChild to this element
    ''' </summary>
    ''' <param name="NewChild">The element to attach to this element</param>
    Public Sub Attach(NewChild As MathElement)
        If NewChild Is Nothing Then Throw New ArgumentNullException("NewChild", "The Attach method can't be used with a null argument.")
        NewChild.SetParent(Me)
    End Sub

    ''' <summary>
    ''' Reset the Parent of Child to null.
    ''' </summary>
    ''' <param name="Child"></param>
    ''' <remarks></remarks>
    Public Sub Detach(Child As MathElement)
        If Child.ParentElement Is Me Then Child.ResetParent() _
            Else Throw New ArgumentException("Child must be a child of this element.")
    End Sub

    ''' <summary>
    ''' Changes the value of the Parent property of this element.
    ''' </summary>
    ''' <param name="value">The new value for the Parent property</param>
    ''' <exception cref="InvalidOperationException">This functions throws an InvalidOperationException when this element already have a parent.</exception>
    ''' <exception cref="ArgumentException">This functions throws an ArgumentException when this element can't have 'value' as parent.</exception>
    Private Sub SetParent(value As MathElement)
        If _Parent IsNot Nothing Then

            If value Is _Parent Then

#If DEBUG Then
                ' DON'T DO ANYTHING
                Trace.TraceWarning("Multiple refefinition of the Parent property of an element. Spaghetti code suspected.")
#End If

            Else

                ' REMOVE THE CURRENT PARENT, AND RETRY
                Try

                    ResetParent()
                    SetParent(value)

                Catch ex As Exception

                    Throw New InvalidOperationException("Unable to modify the parent of an element after it has been added to a children list. Please remove this element from its parent, or use the Clone() function to get a parent-free copy of this element.", ex)

                End Try

            End If

        Else

            ' ADD THE NEW PARENT
            If value.Children.CanContains(Me) Then

                ' Perform the change
                _Parent = value

                ' Raise the corresponding event
                RaiseEvent AttachedToParent(Me, EventArgs.Empty)

            Else
                Throw New ArgumentException("This element is not recognised as a valid child of its new parent.", "Parent")
            End If

        End If
    End Sub

    ''' <summary>
    ''' Resets the Parent property for this element.
    ''' </summary>
    Private Sub ResetParent()

        If ParentElement.Children.Contains(Me) Then

            Throw New InvalidOperationException("Reseting the Parent property wasn't posssible because the parent still claims it owns the current element.")

        Else

            ' Perform some cleanup
            _Parent = Nothing
            _ParentDocument = Nothing
            _Selection = Nothing

            ' Raise the corresponding event
            RaiseDetachedFromParent()

        End If

    End Sub

    ''' <summary>
    ''' Returns the last element in the Parent chain. If this element is hosted in a document, this function returns the same as ParentDocument.
    ''' </summary>
    Public ReadOnly Property Root As MathElement
        Get
            If _ParentDocument IsNot Nothing Then Return _ParentDocument
            If _Parent Is Nothing Then Return Me
            Return ParentElement.Root
        End Get
    End Property

    Private _ParentDocument As MathDocument

    ''' <summary>
    ''' Returns the document that host this element, or Nothing if this element is not currently hosted.
    ''' </summary>
    Public ReadOnly Property ParentDocument As MathDocument
        Get

            ' Cache the parent document, if not already done
            If _ParentDocument Is Nothing Then
                If ParentElement IsNot Nothing Then
                    _ParentDocument = ParentElement.ParentDocument
                ElseIf TryCast(Me, MathDocument) IsNot Nothing Then
                    _ParentDocument = DirectCast(Me, MathDocument)
                End If
            End If

            ' Return the cached parent document
            Return _ParentDocument

        End Get
    End Property

    Public ReadOnly Property ChildIndex As Integer
        Get
            Return ParentElement.Children.IndexOf(Me)
        End Get
    End Property

    Public ReadOnly Property NextSibling As MathElement
        Get
            If _Parent Is Nothing Then Return Nothing
            Return _Parent.Children.After(Me)
        End Get
    End Property

    Public ReadOnly Property PreviousSibling As MathElement
        Get
            If _Parent Is Nothing Then Return Nothing
            Return _Parent.Children.Before(Me)
        End Get
    End Property

    Public Function IsBefore(Elm As MathElement) As Boolean

        If Elm Is Nothing Then Return True
        If Elm.ParentElement IsNot ParentElement Then Return False

        Do 'PreviousSiblig
            If (Elm Is Me) Then Return True
            Elm = ParentElement.Children.Before(Elm)
        Loop While (Elm IsNot Nothing)

        Return False
    End Function

    Public Function IsAfter(Elm As MathElement) As Boolean

        If Elm Is Nothing Then Return True
        Return Elm.IsBefore(Me)

    End Function

    Public Function Clone(Optional CloneChildren As Boolean = True) As MathElement
        Clone = Me.Clone_Internal(CloneChildren)
        If _IsFontStretchDefined Then Clone.FontStretch = Me.FontStretch
        If _IsFontWeightDefined Then Clone.FontWeight = Me.FontWeight
        If _IsFontStyleDefined Then Clone.FontStyle = Me.FontStyle
        If _IsFontFamilyDefined Then Clone.FontFamily = Me.FontFamily
        If _IsFontSizeDefined Then Clone.FontSize = Me.FontSize
        If _IsForegroundDefined Then Clone.Foreground = Me.Foreground
        If _IsBackgroundDefined Then Clone.Background = Me.Background
        Return Clone
    End Function
    Public MustOverride Function Clone_Internal(Optional ByVal CloneChildren As Boolean = True) As MathElement

    '++
    '++ XML Children
    '++

    Protected _Children As ChildrenHelper
    Public ReadOnly Property Children As ChildrenHelper
        Get
            Return _Children
        End Get
    End Property

    Public Sub AddChild(NewChild As MathElement)
        Children.Add(NewChild)
    End Sub

    Public Sub RemoveChild(OldChild As MathElement)
        Children.Remove(OldChild)
    End Sub

    Public Sub ReplaceChild(OldChild As MathElement, NewChild As MathElement)
        Children.Replace(OldChild, NewChild)
    End Sub

    Public ReadOnly Property FirstChild As MathElement
        Get
            Return Children.First
        End Get
    End Property

    Public ReadOnly Property LastChild As MathElement
        Get
            Return Children.Last
        End Get
    End Property

    Public ReadOnly Property TreeDepht As Integer
        Get
            TreeDepht = 0 : Dim P As MathElement = Me
            While P.ParentElement IsNot Nothing
                TreeDepht += 1 : P = P.ParentElement
            End While
            Return TreeDepht
        End Get
    End Property

    Public Function GetCommonAncestrorWith(el1 As MathElement) As MathElement
        Return GetCommonAncestrorBetween(el1, Me)
    End Function

    Public Shared Function GetCommonAncestrorBetween(el1 As MathElement, el2 As MathElement) As MathElement

        Dim Delta As Integer = el2.TreeDepht - el1.TreeDepht
        If Delta > 0 Then
            For x = 1 To Delta
                el2 = el2.ParentElement
            Next
        ElseIf Delta < 0 Then
            For x = 1 To -Delta
                el1 = el1.ParentElement
            Next
        End If

        While el2 IsNot el1
            el1 = el1.ParentElement : el2 = el2.ParentElement
        End While

        Return el1

    End Function

    '++
    '++ XML Events
    '++

    ''' <summary>
    ''' Risen when a property of the current element changed, or a property of one of its children
    ''' </summary>
    ''' <remarks>
    ''' When this event is risen, the layout of the current element should be recomputed.
    ''' </remarks>
    Public Event Changed As EventHandler
    Public Sub RaiseChanged()
        If ShouldRaiseChanged = 0 Then
            RaiseEvent Changed(Me, EventArgs.Empty)
        Else
            ChangePendings += 1
        End If
    End Sub

    ' TODO: Use StartBatchProcess in the current code
    Private ShouldRaiseChanged, ChangePendings As Integer
    Public Sub StartBatchProcess()
        ShouldRaiseChanged += 1
    End Sub

    Public Sub StopBatchProcess()
        ShouldRaiseChanged = 0
        If ChangePendings <> 0 Then
            ChangePendings = 0
            RaiseChanged()
        End If
    End Sub

#If DEBUG Then
    Public LastChangeTimestamp As Date
#End If

    Private Sub MathElement_Changed(sender As Object, e As System.EventArgs) Handles Me.Changed
#If DEBUG Then
        LastChangeTimestamp = Date.Now
#End If
        If _Parent IsNot Nothing Then ParentElement.RaiseChanged()
    End Sub

    ' TODO: Reviewing completely the event usage (drawing a schema)

    Public Event SubTreeModified As EventHandler(Of TreeEventArgs)
    Public Sub RaiseSubTreeModified(e As TreeEventArgs)
        RaiseEvent SubTreeModified(Me, e)
        RaiseEvent Changed(Me, EventArgs.Empty)
    End Sub

    Public Event AttachedToParent As EventHandler
    Public Sub RaiseAttachedToParent()
        RaiseEvent AttachedToParent(Me, EventArgs.Empty)
    End Sub

    Public Event AddedToParent As EventHandler
    Public Sub RaiseAddedToParent()
        RaiseEvent AddedToParent(Me, EventArgs.Empty)
    End Sub

    Public Event RemovedFromParent As EventHandler
    Public Sub RaiseRemovedFromParent()
        RaiseEvent RemovedFromParent(Me, EventArgs.Empty)
    End Sub

    Public Event DetachedFromParent As EventHandler
    Public Sub RaiseDetachedFromParent()
        RaiseEvent DetachedFromParent(Me, EventArgs.Empty)
    End Sub

    Public Event DetachedFromDocument As EventHandler
    Public Sub RaiseDetachedFromDocument()
        RaiseEvent DetachedFromDocument(Me, EventArgs.Empty)
    End Sub

    Public Event ChildAdded As EventHandler(Of TreeEventArgs)
    Public Sub RaiseChildAdded(ChildElement As MathElement, ChildIndex As Integer)
        Dim e = New TreeEventArgs(ChildElement, Me, ChildIndex, TreeEventArgs.TreeAction.Added)
        RaiseEvent ChildAdded(Me, e)
        RaiseEvent SubTreeModified(Me, e)
    End Sub

    Public Event ChildRemoved As EventHandler(Of TreeEventArgs)
    Public Sub RaiseChildRemoved(ChildElement As MathElement, ChildIndex As Integer)
        Dim e = New TreeEventArgs(ChildElement, Me, ChildIndex, TreeEventArgs.TreeAction.Removed)
        RaiseEvent ChildRemoved(Me, e)
        RaiseEvent SubTreeModified(Me, e)
    End Sub

    Public Class TreeEventArgs : Inherits EventArgs

        Public Property ParentElement As MathElement
        Public Property ChildElement As MathElement
        Public Property ChildIndex As Integer
        Public Property Action As TreeAction

        Public Enum TreeAction
            Added = +1 : Removed = -1 : Modified = 0
        End Enum

        Public Sub New(ChildElement As MathElement, ParentElement As MathElement, ChildIndex As Integer, Action As TreeAction)
            Me.ChildElement = ChildElement
            Me.ParentElement = ParentElement
            Me.ChildIndex = ChildIndex
            Me.Action = Action
        End Sub

    End Class

    Private Sub MathElement_ChildAdded(sender As Object, e As TreeEventArgs) Handles Me.ChildAdded
        e.ChildElement.RaiseAddedToParent()
    End Sub

    Private Sub MathElement_ChildRemoved(sender As Object, e As TreeEventArgs) Handles Me.ChildRemoved
        e.ChildElement.RaiseRemovedFromParent()
    End Sub

    Private Sub MathElement_DetachedFromDocument(sender As Object, e As System.EventArgs) Handles Me.DetachedFromDocument
        _ParentDocument = Nothing
        _Selection = Nothing
    End Sub

    Private Sub MathElement_DetachedFromParent(sender As Object, e As System.EventArgs) Handles Me.DetachedFromParent
        RaiseDetachedFromDocument()
        For Each Child In Children
            Child.RaiseDetachedFromDocument()
        Next
    End Sub

    Private Sub MathElement_SubTreeModified(sender As Object, e As TreeEventArgs) Handles Me.SubTreeModified
        If Me.ParentElement IsNot Nothing Then Me.ParentElement.RaiseSubTreeModified(e)
    End Sub
End Class
