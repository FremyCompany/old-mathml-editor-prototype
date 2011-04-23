Public MustInherit Class ChildrenHelper
    Implements IEnumerable(Of MathElement)

    ''' <summary>
    ''' Returns the element for which the current children list is maintained.
    ''' </summary>
    Protected This As MathElement
    Public Sub New(ByVal This As MathElement)
        Me.This = This
    End Sub

    '++
    '++ Fundamental traps
    '++

    ''' <summary>
    ''' Determines whether this children list contains the specified element.
    ''' </summary>
    ''' <param name="Element">The element to search for</param>
    Public Function Contains(ByVal Element As MathElement) As Boolean
        Return (Element.Parent Is Me) AndAlso Contains_Internal(Element)
    End Function

    ''' <summary>
    ''' Determines whether this children list contains the specified element.
    ''' </summary>
    ''' <param name="Element">The element to check for</param>
    Protected MustOverride Function Contains_Internal(ByVal Element As MathElement) As Boolean

    ''' <summary>
    ''' Returns the index of the specified element in this children list, or -1 if it wasn't found.
    ''' </summary>
    ''' <param name="Element">The element to search for.</param><returns></returns>
    Public MustOverride Function IndexOf(ByVal Element As MathElement) As Integer

    ''' <summary>
    ''' Adds the specified new child.
    ''' </summary>
    ''' <param name="NewChild">The new child.</param>
    Public Sub Add(ByVal NewChild As MathElement)
        Try

            This.StartBatchProcess()

            This.Attach(NewChild)
            Add_Internal(NewChild)
            This.RaiseChildAdded(NewChild)

            This.RaiseChanged()

        Finally

            This.StopBatchProcess()

        End Try
    End Sub

    ''' <summary>
    ''' Removes the specified old child.
    ''' </summary>
    ''' <param name="OldChild">The old child.</param>
    Public Sub Remove(ByVal OldChild As MathElement)

        If OldChild.Parent IsNot This Then _
            Throw New ArgumentException("OldChild was not a child of this element.")

        Try

            This.StartBatchProcess()

            Remove_Internal(OldChild)
            This.RaiseChildRemoved(OldChild)
            This.Detach(OldChild)
            This.RaiseChanged()

        Finally

            This.StopBatchProcess()

        End Try

    End Sub

    ''' <summary>
    ''' Inserts a new child after an old one.
    ''' </summary>
    ''' <param name="NewChild">The new child.</param>
    ''' <param name="OldChild">The old child.</param>
    Public Sub InsertAfter(ByVal NewChild As MathElement, ByVal OldChild As MathElement)
        Try

            This.StartBatchProcess()

            This.Attach(NewChild)
            InsertAfter_Internal(NewChild, OldChild)
            This.RaiseChildAdded(NewChild)
            This.RaiseChanged()


        Finally

            This.StopBatchProcess()

        End Try
    End Sub

    ''' <summary>
    ''' Adds NewChild to the internal representation of the children list.
    ''' </summary>
    ''' <param name="NewChild">The child to add</param>
    Protected MustOverride Sub Add_Internal(ByVal NewChild As MathElement)

    ''' <summary>
    ''' Removes a child from the internal representation of the children list
    ''' </summary>
    ''' <param name="OldChild">The child to remove</param>
    Protected MustOverride Sub Remove_Internal(ByVal OldChild As MathElement)

    ''' <summary>
    ''' Inserts a child after another in the in the internal representation of the children list.
    ''' </summary>
    ''' <param name="NewChild">The child to insert</param>
    ''' <param name="OldChild">The child after which to insert</param>
    ''' <remarks></remarks>
    Protected MustOverride Sub InsertAfter_Internal(ByVal NewChild As MathElement, ByVal OldChild As MathElement)

    ''' <summary>
    ''' Returns the child located after the specified one in this children list
    ''' </summary>
    ''' <param name="OldChild">The old child.</param><returns></returns>
    Public MustOverride Function After(ByVal OldChild As MathElement) As MathElement

    ''' <summary>
    ''' Returns the child located before the specified one in this children list.
    ''' </summary>
    ''' <param name="OldChild">The old child.</param><returns></returns>
    Public MustOverride Function Before(ByVal OldChild As MathElement) As MathElement

    ''' <summary>
    ''' Gets the first child of this list.
    ''' </summary>
    Public MustOverride ReadOnly Property First() As MathElement

    ''' <summary>
    ''' Get the last child of this list.
    ''' </summary>
    Public MustOverride ReadOnly Property Last() As MathElement

    '++
    '++ Operational traps
    '++

    ''' <summary>
    ''' Gets a value indicating whether this instance can contains any child.
    ''' </summary>
    Public MustOverride ReadOnly Property CanHave As Boolean

    ''' <summary>
    ''' Returns True if a call to Add() may succeed.
    ''' </summary>
    Public MustOverride ReadOnly Property CanAdd As Boolean

    ''' <summary>
    ''' Returns True if a call to Remove() may succeed.
    ''' </summary>
    Public MustOverride ReadOnly Property CanRemove As Boolean

    ''' <summary>
    ''' Returns True if a call to Replace() may succeed.
    ''' </summary>
    Public Overridable ReadOnly Property CanReplace As Boolean
        Get
            Return CanAdd AndAlso CanRemove
        End Get
    End Property

    ''' <summary>
    ''' Returns True if a call to Swap() may succeed.
    ''' </summary>
    Public Overridable ReadOnly Property CanSwap As Boolean
        Get
            Return CanReplace
        End Get
    End Property

    ''' <summary>
    ''' Gets the total amount of children in this instance.
    ''' </summary>
    Public MustOverride ReadOnly Property Count As Integer

    ''' <summary>
    ''' Returns true if this list contains more than one child element.
    ''' </summary>
    Public ReadOnly Property HasMany As Boolean
        Get
            Return Count > 1
        End Get
    End Property

    ''' <summary>
    ''' Returns true if this list contains any child element.
    ''' </summary>
    Public ReadOnly Property HasAny As Boolean
        Get
            Return Count <> 0
        End Get
    End Property

    ''' <summary>
    ''' Returns true if this list contains exactly one child element.
    ''' </summary>
    Public ReadOnly Property HasOne As Boolean
        Get
            Return Count = 1
        End Get
    End Property

    ''' <summary>
    ''' Returns true if this list contains no child element.
    ''' </summary>
    Public ReadOnly Property HasNo As Boolean
        Get
            Return Count = 0
        End Get
    End Property

    '++
    '++ Element Behavior
    '++

    ''' <summary>
    ''' Returns the type of behavior you should expect from this children list.
    ''' </summary><remarks>
    ''' This won't make the use of the CanAdd, CanRemove, and IsValid functions unnecessary.
    ''' </remarks>
    Public MustOverride ReadOnly Property ElementType As MathElement.Type

    ''' <summary>
    ''' Returns True if this element is a Layout Engine (as many child as you want).
    ''' </summary>
    Public ReadOnly Property IsLayoutEngine As Boolean
        Get
            Return (ElementType And MathElement.Type.LayoutEngine) = MathElement.Type.LayoutEngine
        End Get
    End Property

    ''' <summary>
    ''' Returns True if this element is a Text Edit (can only contains UnicodeGlyph children).
    ''' </summary>
    Public ReadOnly Property IsTextEdit As Boolean
        Get
            Return ElementType = MathElement.Type.TextEdit
        End Get
    End Property

    ''' <summary>
    ''' Returns True if this element is a Formatter (can only contains a fixed number of children)
    ''' </summary>
    Public ReadOnly Property IsFormatter As Boolean
        Get
            Return ElementType = MathElement.Type.Formatter
        End Get
    End Property


    '++
    '++ Derived traps
    '++

    ''' <summary>
    ''' Determines whether this instance can contains the specified element.
    ''' </summary>
    ''' <param name="Element">The element to be added to the children list</param>
    Public Function CanContains(ByVal Element As MathElement) As Boolean
        Return (Element IsNot Nothing) AndAlso (Element.Parent Is Nothing) AndAlso CanContains_Internal(Element)
    End Function

    ''' <summary>
    ''' Returns True if the specified non-null element can be contained in the current children list.
    ''' </summary>
    ''' <param name="Element">The element to be checked</param>
    Protected Overridable Function CanContains_Internal(ByVal Element As MathElement) As Boolean
        Return True
    End Function

    ''' <summary>
    ''' Inserts a new element before an existing element in the children list.
    ''' </summary>
    ''' <param name="NewChild">The element to be inserted.</param>
    ''' <param name="OldChild">The element before which NewChild should be inserted.</param>
    Public Sub InsertBefore(ByVal NewChild As MathElement, ByVal OldChild As MathElement)
        Try

            This.StartBatchProcess()

            This.Attach(NewChild)
            InsertBefore_Internal(NewChild, OldChild)

            This.RaiseChanged()

        Finally

            This.StopBatchProcess()

        End Try
    End Sub

    ''' <summary>
    ''' Inserts (in the internal representation) a new element before an existing element.
    ''' </summary>
    ''' <param name="NewChild">The element to be inserted.</param>
    ''' <param name="OldChild">The element before which NewChild should be inserted.</param>
    Protected Overridable Sub InsertBefore_Internal(ByVal NewChild As MathElement, ByVal OldChild As MathElement)
        InsertAfter_Internal(NewChild, Before(OldChild))
    End Sub

    ''' <summary>
    ''' Replaces an exising element by another in the children list.
    ''' </summary>
    ''' <param name="OldChild">The element to be removed from the children list.</param>
    ''' <param name="NewChild">The element to be added to the children list.</param>
    Public Sub Replace(ByVal OldChild As MathElement, ByVal NewChild As MathElement)
        Try

            This.StartBatchProcess()

            ' Perform checks
            If OldChild Is NewChild Then Throw New ArgumentException("An element can be replaced by itself.")
            If OldChild.Parent IsNot Me Then Throw New ArgumentException("The element to replace is not attached to this element.")

            If NewChild.Parent Is Me Then Remove(NewChild)

            ' Launch user-defined code
            This.Attach(NewChild)
            Replace_Internal(OldChild, NewChild)
            This.Detach(OldChild)

            This.RaiseChanged()

        Finally

            This.StopBatchProcess()

        End Try
    End Sub

    ''' <summary>
    ''' Replaces an exising element by another in the (internal representation of the) children list.
    ''' </summary>
    ''' <param name="OldChild">The element to be removed from the children list.</param>
    ''' <param name="NewChild">The element to be added to the children list.</param>
    Protected Overridable Sub Replace_Internal(ByVal OldChild As MathElement, ByVal NewChild As MathElement)
        InsertAfter(NewChild, OldChild) : Remove(OldChild)
    End Sub

    ''' <summary>
    ''' Exchanges the position of two elements inside the list.
    ''' </summary>
    ''' <param name="FirstChild">The first child.</param>
    ''' <param name="SecondChild">The second child.</param>
    Public Sub Swap(ByVal FirstChild As MathElement, ByVal SecondChild As MathElement)
        Try

            This.StartBatchProcess()

            Swap_Internal(FirstChild, SecondChild)

            This.RaiseChanged()

        Finally

            This.StopBatchProcess()

        End Try
    End Sub

    ''' <summary>
    ''' Exchanges the position of two elements inside the (internal representation of the) list.
    ''' </summary>
    ''' <param name="FirstChild">The first child.</param>
    ''' <param name="SecondChild">The second child.</param>
    Protected Overridable Sub Swap_Internal(ByVal FirstChild As MathElement, ByVal SecondChild As MathElement)
        Remove(FirstChild) : InsertBefore(FirstChild, SecondChild)
    End Sub

    ''' <summary>
    ''' Wraps a element that's currently child of this element inside another.
    ''' Removes InitialChild from this list, adds it to the Wrapper and then add the Wrapper to this list.
    ''' </summary>
    ''' <param name="InitialChild">The child to wrap.</param>
    ''' <param name="Wrapper">The wrapper to use.</param>
    Public Sub Wrap(ByVal InitialChild As MathElement, ByVal Wrapper As MathElement)
        Try

            This.StartBatchProcess()
            Wrap_Internal(InitialChild, Wrapper)
            This.RaiseChanged()

        Finally

            This.StopBatchProcess()

        End Try
    End Sub

    ''' <summary>
    ''' Wraps a element that's currently child of this element inside another.
    ''' Removes InitialChild from this list, adds it to the Wrapper and then add the Wrapper to this list.
    ''' </summary>
    ''' <param name="InitialChild">The child to wrap.</param>
    ''' <param name="Wrapper">The wrapper to use.</param>
    Protected Overridable Sub Wrap_Internal(ByVal InitialChild As MathElement, ByVal Wrapper As MathElement)

        InsertBefore(Wrapper, InitialChild)
        Remove(InitialChild)
        Wrapper.AddChild(InitialChild)

    End Sub

    ''' <summary>
    ''' Wraps every element between InitialChild and FinalChild inside another.
    ''' </summary>
    ''' <param name="InitialChild">The first child to wrap</param>
    ''' <param name="Wrapper">The wrapper to use.</param>
    ''' <param name="FinalChild">The last child to wrap. If this element is not found, all children after InitialChild will be wrapped.</param>
    Public Sub Wrap(ByVal InitialChild As MathElement, ByVal Wrapper As MathElement, ByVal FinalChild As MathElement)
        Try

            This.StartBatchProcess()
            Wrap_Internal(InitialChild, Wrapper, FinalChild)
            This.RaiseChanged()

        Finally

            This.StopBatchProcess()

        End Try
    End Sub

    ''' <summary>
    ''' Wraps every element between InitialChild and FinalChild inside another.
    ''' </summary>
    ''' <param name="InitialChild">The first child to wrap</param>
    ''' <param name="Wrapper">The wrapper to use.</param>
    ''' <param name="FinalChild">The last child to wrap. If this element is not found, all children after InitialChild will be wrapped.</param>
    Protected Overridable Sub Wrap_Internal(ByVal InitialChild As MathElement, ByVal Wrapper As MathElement, ByVal FinalChild As MathElement)

        InsertBefore(Wrapper, InitialChild)

        Dim Children = New SiblingEnumerator(InitialChild, FinalChild)
        Dim CurrentChild As MathElement
        While Children.MoveNext()
            CurrentChild = Children.Current
            Remove(CurrentChild)
            Wrapper.AddChild(CurrentChild)
        End While

    End Sub

    ''' <summary>
    ''' Replaces a child element by all of its children in this children list.
    ''' </summary>
    ''' <param name="Wrapper">The element to replace.</param>
    Public Overridable Sub Unwrap(ByVal Wrapper As MathElement)
        Dim Children = Wrapper.Children.GetEnumerator()
        Dim LastChild = Wrapper, CurrentChild As MathElement
        While Children.MoveNext()
            CurrentChild = Children.Current
            Wrapper.RemoveChild(CurrentChild)
            InsertAfter(LastChild, CurrentChild)
            LastChild = CurrentChild
        End While
        Remove(Wrapper)
    End Sub

    ''' <summary>
    ''' Gets the enumerator.
    ''' </summary>
    Public Overridable Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement) Implements System.Collections.Generic.IEnumerable(Of MathElement).GetEnumerator
        Return New SiblingEnumerator(Me.First)
    End Function

    Private Function GetUntypedEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

End Class

Public Class SiblingEnumeratorGenerator : Implements IEnumerable(Of MathElement)

    Private FirstEl, LastEl As MathElement
    Public Sub New(ByVal FirstEl As MathElement)
        Me.New(FirstEl, Nothing)
    End Sub

    Public Sub New(ByVal FirstEl As MathElement, ByVal LastEl As MathElement)
        Me.FirstEl = FirstEl : Me.LastEl = LastEl
    End Sub

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement) Implements System.Collections.Generic.IEnumerable(Of MathElement).GetEnumerator
        Return New SiblingEnumerator(FirstEl, LastEl)
    End Function

    Public Function GetGenericEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

End Class

Public Class SiblingEnumerator : Implements IEnumerator(Of MathElement)

    Private FirstEl As MathElement, NextEl As MathElement, CurrentEl As MathElement
    Public Sub New(ByVal FirstEl As MathElement)
        Me.FirstEl = FirstEl : Me.NextEl = FirstEl
    End Sub

    Private LastEl As MathElement
    Public Sub New(ByVal FirstEl As MathElement, ByVal LastEl As MathElement)
        Me.FirstEl = FirstEl : Me.NextEl = FirstEl : Me.LastEl = LastEl
    End Sub

    Public ReadOnly Property Current As MathElement Implements System.Collections.Generic.IEnumerator(Of MathElement).Current
        Get
            Return CurrentEl
        End Get
    End Property

    Public ReadOnly Property CurrentUnTyped As Object Implements System.Collections.IEnumerator.Current
        Get
            Return CurrentEl
        End Get
    End Property

    Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
        If (NextEl Is Nothing) OrElse (LastEl IsNot Nothing AndAlso Current Is LastEl) Then
            CurrentEl = Nothing
            Return False
        Else
            CurrentEl = NextEl
            NextEl = CurrentEl.NextSibling
            Return True
        End If
    End Function

    Public Sub Reset() Implements System.Collections.IEnumerator.Reset
        NextEl = FirstEl : CurrentEl = Nothing
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        FirstEl = Nothing : NextEl = Nothing : CurrentEl = Nothing
    End Sub

End Class
