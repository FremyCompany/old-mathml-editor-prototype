Public MustInherit Class ChildrenHelper
    Implements IEnumerable(Of MathElement)

    ''' <summary>
    ''' Returns the element for which the current children list is maintained.
    ''' </summary>
    Protected This As MathElement
    Public Sub New(ByVal This As MathElement)
        Me.This = This
    End Sub

    REM
    REM Fundamental traps
    REM

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

        This.StartBatchProcess()

        NewChild.Parent = This
        Add_Internal(NewChild)
        This.RaiseChanged()

        This.StopBatchProcess()

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
            OldChild.Parent = Nothing

        Finally

            This.RaiseChanged()
            This.StopBatchProcess()

        End Try

    End Sub

    ''' <summary>
    ''' Inserts a new child after an old one.
    ''' </summary>
    ''' <param name="NewChild">The new child.</param>
    ''' <param name="OldChild">The old child.</param>
    Public Sub InsertAfter(ByVal NewChild As MathElement, ByVal OldChild As MathElement)

        NewChild.Parent = This
        InsertAfter_Internal(NewChild, OldChild)
        This.RaiseChanged()

    End Sub

    Protected MustOverride Sub Add_Internal(ByVal NewChild As MathElement)
    Protected MustOverride Sub Remove_Internal(ByVal OldChild As MathElement)
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

    REM
    REM Operational traps
    REM

    Public MustOverride ReadOnly Property CanHave As Boolean
    Public MustOverride ReadOnly Property CanAdd As Boolean
    Public MustOverride ReadOnly Property CanRemove As Boolean

    Public MustOverride ReadOnly Property Count As Integer

    Public ReadOnly Property HasMany As Boolean
        Get
            Return Count > 1
        End Get
    End Property
    Public ReadOnly Property HasAny As Boolean
        Get
            Return Count <> 0
        End Get
    End Property
    Public ReadOnly Property HasOne As Boolean
        Get
            Return Count = 1
        End Get
    End Property
    Public ReadOnly Property HasNo As Boolean
        Get
            Return Count = 0
        End Get
    End Property


    Public Overridable ReadOnly Property CanReplace As Boolean
        Get
            Return CanAdd AndAlso CanRemove
        End Get
    End Property

    '++ Element Behavior

    Public MustOverride ReadOnly Property ElementType As MathElement.Type
    Public ReadOnly Property IsLayoutEngine As Boolean
        Get
            Return (ElementType And MathElement.Type.LayoutEngine) = MathElement.Type.LayoutEngine
        End Get
    End Property
    Public ReadOnly Property IsTextEdit As Boolean
        Get
            Return ElementType = MathElement.Type.TextEdit
        End Get
    End Property
    Public ReadOnly Property IsFormatter As Boolean
        Get
            Return ElementType = MathElement.Type.Formatter
        End Get
    End Property


    '++ Derived traps

    Public Function IsValidChild(ByVal Element As MathElement) As Boolean
        Return (Element.Parent Is Nothing) AndAlso IsValidChild_Internal(Element)
    End Function

    Protected Overridable Function IsValidChild_Internal(ByVal Element As MathElement) As Boolean
        Return Element IsNot Nothing
    End Function

    Public Sub InsertBefore(ByVal NewChild As MathElement, ByVal OldChild As MathElement)

        NewChild.Parent = This
        InsertBefore_Internal(NewChild, OldChild)
        This.RaiseChanged()

    End Sub

    Protected Overridable Sub InsertBefore_Internal(ByVal NewChild As MathElement, ByVal OldChild As MathElement)
        InsertAfter(NewChild, Before(OldChild))
    End Sub

    Public Sub Replace(ByVal OldChild As MathElement, ByVal NewChild As MathElement)

#If DEBUG Then
        Dim CurrentDate = Date.Now
#End If

        ' Launch user-defined code
        Replace_Internal(OldChild, NewChild)

#If DEBUG Then
        ' Assert everything was fine
        AssertTrue((This.LastChangeTimestamp - CurrentDate).Ticks > 0)
        AssertNull(OldChild.Parent)
        AssertEquals(NewChild.Parent, This)
#End If

    End Sub

    Protected Overridable Sub Replace_Internal(ByVal OldChild As MathElement, ByVal newchild As MathElement)
        InsertAfter(newchild, OldChild) : Remove(OldChild)
    End Sub

    Public Overridable Sub Swap(ByVal FirstChild As MathElement, ByVal SecondChild As MathElement)
        Dim BeforeFirstChild = Before(FirstChild)
        Remove(FirstChild)
        Replace(SecondChild, FirstChild)
        InsertAfter(SecondChild, BeforeFirstChild)
    End Sub

    Public Overridable Sub Wrap(ByVal InitialChild As MathElement, ByVal Wrapper As MathElement)
        Dim BeforeInitialChild = Before(InitialChild)
        Remove(InitialChild)
        Wrapper.AddChild(InitialChild)
        InsertAfter(Wrapper, BeforeInitialChild)
    End Sub

    Public Overridable Sub Wrap(ByVal InitialChild As MathElement, ByVal Wrapper As MathElement, ByVal FinalChild As MathElement)
        Dim BeforeInitialChild = Before(InitialChild)

        Dim Children = New SiblingEnumerator(InitialChild, FinalChild)
        Dim CurrentChild As MathElement
        While Children.MoveNext()
            CurrentChild = Children.Current
            Remove(CurrentChild)
            Wrapper.AddChild(CurrentChild)
        End While

        InsertAfter(Wrapper, BeforeInitialChild)
    End Sub

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
