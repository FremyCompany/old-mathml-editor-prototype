Public MustInherit Class ChildrenHelper
    Implements IEnumerable(Of MathElement)


    '++
    '++ Private fields
    '++
    Protected This As MathElement
    Protected Elms As New List(Of MathElement)

    Public Sub New(This As MathElement)
        Me.This = This
    End Sub

    '++
    '++ Properties
    '++

#Region "Element type"
    Public MustOverride ReadOnly Property ElementType As MathElement.Type
    Public ReadOnly Property IsFormatter() As Boolean
        Get
            Return ElementType And MathElement.Type.Formatter
        End Get
    End Property
    Public ReadOnly Property IsLayoutEngine As Boolean
        Get
            Return ElementType And MathElement.Type.LayoutEngine
        End Get
    End Property
    Public ReadOnly Property IsGlyph As Boolean
        Get
            Return ElementType And MathElement.Type.Glyph
        End Get
    End Property
    Public ReadOnly Property IsTextEdit As Boolean
        Get
            Return ElementType And MathElement.Type.TextEdit
        End Get
    End Property
#End Region

#Region "Possible actions"

    Public MustOverride ReadOnly Property CanHave() As Boolean

    Public ReadOnly Property CanAdd() As Boolean
        Get
            Return CanInsert
        End Get
    End Property
    Public MustOverride ReadOnly Property CanInsert() As Boolean
    Public MustOverride ReadOnly Property CanRemove() As Boolean
    Public MustOverride ReadOnly Property CanSwap() As Boolean
    Public Overridable ReadOnly Property CanReplace() As Boolean
        Get
            Return CanInsert And CanRemove
        End Get
    End Property

    ''' <summary>
    ''' Returns True if the element can be inserted in this children list
    ''' </summary>
    ''' <param name="NewElement">The element to check</param>
    Public Function CanContains(NewElement As MathElement) As Boolean
        Try
            ValidateNewElement(NewElement)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

#End Region

#Region "Count property"

    ''' <summary>
    ''' Returns the number of child element in this collection
    ''' </summary>
    Public ReadOnly Property Count As Integer
        Get
            Return Elms.Count
        End Get
    End Property

    ''' <summary>
    ''' Returns the first element of the child collection
    ''' </summary>
    Public ReadOnly Property First() As MathElement
        Get
            Return If(Elms.Count = 0, Nothing, Elms(0))
        End Get
    End Property

    ''' <summary>
    ''' Returns the last element of the child collection
    ''' </summary>
    Public ReadOnly Property Last() As MathElement
        Get
            Return If(Elms.Count = 0, Nothing, Elms(Elms.Count - 1))
        End Get
    End Property

    ''' <summary>
    ''' Returns True if there's one or more children in this collection
    ''' </summary>
    Public ReadOnly Property HasAny() As Boolean
        Get
            Return Elms.Count <> 0
        End Get
    End Property

    ''' <summary>
    ''' Returns True if there's no child in this collection
    ''' </summary>
    Public ReadOnly Property HasNo() As Boolean
        Get
            Return Elms.Count = 0
        End Get
    End Property

    ''' <summary>
    ''' Returns True if there's exactly one child in this collection
    ''' </summary>
    Public ReadOnly Property HasOne() As Boolean
        Get
            Return Elms.Count = 1
        End Get
    End Property

    ''' <summary>
    ''' Returns True if there's more than one child in this collection
    ''' </summary>
    Public ReadOnly Property HasMany As Boolean
        Get
            Return Elms.Count > 1
        End Get
    End Property

#End Region

    '++
    '++ Property Methods
    '++

    ''' <summary>
    ''' Returns True if the specified element is contained in the collection
    ''' </summary>
    ''' <param name="Element">The element to check</param>
    Public Function Contains(Element As MathElement) As Boolean
        Return Elms.Contains(Element)
    End Function

    ''' <summary>
    ''' Returns the index of the specified element, if it's contained in the collection
    ''' </summary>
    ''' <param name="Element">The element to check</param>
    Public Function IndexOf(Element As MathElement) As Integer
        Return Elms.IndexOf(Element)
    End Function

    ''' <summary>
    ''' Returns the previous element in the collection
    ''' </summary>
    ''' <param name="Element">The element whose previous item should be returned</param>
    Public Function Before(Element As MathElement) As MathElement
        Dim I = IndexOf(Element)
        If I = 0 Then Return Nothing
        Return Elms(I - 1)
    End Function

    ''' <summary>
    ''' Returns the next element in the collection
    ''' </summary>
    ''' <param name="Element">The element whose following item should be returned</param>
    Public Function After(Element As MathElement) As MathElement
        Dim I = IndexOf(Element) + 1
        If I = Elms.Count Then Return Nothing
        Return Elms(I)
    End Function

    '++
    '++ Check Methods
    '++

    Protected Sub ValidateIndex(Index As Integer)
        If Index < 0 Then Throw New ArgumentOutOfRangeException("Index", "An attempt to perform an action in a children list at a negative index has been rejected.")
        If Index > Elms.Count Then Throw New ArgumentOutOfRangeException("Index", "Attempt to perform an action in a children collection at a position further than the children count has been rejected.")
    End Sub

    Protected Sub ValidateExistingIndex(Index As Integer)
        ValidateIndex(Index) : If Index = Count Then Throw New ArgumentOutOfRangeException("Index can't be equal to Count in a RemoveAt operation.")
    End Sub

    Protected Sub ValidateNewElement(NewElement As MathElement)
        If NewElement Is Nothing Then Throw New ArgumentNullException("NewElement", "An attempt to add a null element inside a children list has been rejected.")
        If NewElement.ParentDocument IsNot Nothing Then Throw New ArgumentException("A new child element shouldn't have a parent nor be a document itself. To insert a document content inside another element, use the Unwrap(Document) method.")
        ValidateNewElement_Internal(NewElement)
    End Sub

    Protected Overridable Sub ValidateNewElement_Internal(NewElement As MathElement)
        ' Do nothing
    End Sub

    Protected Function ValidateOldElement(OldElement As MathElement) As Integer

        If OldElement Is Nothing Then Throw New ArgumentNullException("OldElement")
        If OldElement.ParentElement IsNot This Then Throw New ArgumentException("An old element should have this element as parent.")

        Dim Index = OldElement.ChildIndex : If Index = -1 Then Throw New ArgumentException("This element has not been found in this child collection.")
        Return Index

    End Function

    Protected Overridable Sub ValidateInsert_Internal(NewElement As MathElement, Index As Integer)
        ' Do nothing
    End Sub

    Protected Sub ValidateInsert(NewElement As MathElement, Index As Integer)

        ' Verify property
        If Not CanInsert Then Throw New InvalidOperationException("Attempt to insert an element in an insert-disabled children list has been rejected.")

        ' Validate the element
        ValidateNewElement(NewElement)

        ' Validate the index
        ValidateIndex(Index)

        ' Validate the context
        If This.IsTextEdit AndAlso Not NewElement.IsGlyph Then Throw New ArgumentException("Attemp to insert a non-glyph element inside a text editor has been rejected.")

        ' Perform custom checks
        ValidateInsert_Internal(NewElement, Index)

    End Sub

    Protected Overridable Sub ValidateRemove_Internal(Index As Integer)
        ' Do nothing
    End Sub
    Protected Sub ValidateRemove(Index As Integer)

        ' Verify property
        If Not CanRemove Then Throw New InvalidOperationException("Attempt to remove an element in an remove-disabled children list has been rejected.")

        ' Validate the index
        ValidateExistingIndex(Index)

        ' Perform custom checks
        ValidateRemove_Internal(Index)

    End Sub

    Protected Overridable Sub ValidateReplace_Internal(OldElement As MathElement, NewElement As MathElement)
        ' Do nothing
    End Sub
    Protected Sub ValidateReplace(OldElement As MathElement, NewElement As MathElement)

        ' Verify property
        If Not CanReplace Then Throw New InvalidOperationException("Attempt to replace an element in an replace-disabled children list has been rejected.")

        ' Verify that both are not null
        If OldElement Is Nothing Then Throw New ArgumentNullException("OldElement", "Attempt to use Nothing has an element has been rejected.")
        If NewElement Is Nothing Then Throw New ArgumentNullException("OldElement", "Attempt to use Nothing has an element has been rejected.")

        ' Verify old & new states
        ValidateOldElement(OldElement)
        ValidateNewElement(NewElement)

        ' Preform custom checks
        ValidateReplace_Internal(OldElement, NewElement)

    End Sub

    Protected Overridable Sub ValidateSwap_Internal(OldPos As Integer, NewPos As Integer)
        ' Do nothing
    End Sub
    Public Sub ValidateSwap(OldPos As Integer, NewPos As Integer)

        ' Verify property
        If Not CanSwap Then Throw New InvalidOperationException("Attempt to swap elements in a swap-disabled children list has been rejected.")

        ' Validate the indexes
        ValidateExistingIndex(OldPos)
        ValidateExistingIndex(NewPos)

        ' Perform custom checks
        ValidateSwap_Internal(OldPos, NewPos)

    End Sub

    '++
    '++ [...]
    '++


    '++
    '++ Action Methods
    '++

    Public Sub Add(NewElement As MathElement)

        ' Check arguments
        If NewElement Is Nothing Then Throw New ArgumentNullException("NewElement", "Attempt to add Nothing as a child to an element has been rejected.")
        If NewElement.ParentElement IsNot Nothing Then Throw New ArgumentException("Attempt to add an element to a second parent has been rejected", "NewElement")

        ' Validate the addition
        ValidateInsert(NewElement, Count)

        ' Perfrom the insertion
        This.Attach(NewElement)
        Elms.Add(NewElement)

        ' Raise events
        This.RaiseChildAdded(NewElement, Count - 1)

    End Sub

    Public Sub InsertAt(NewElement As MathElement, Index As Integer)

        ' Special case: inserting an element at the end is a simple add operation
        If Index = Elms.Count Then Add(NewElement) : Exit Sub

        ' Validate the insertion
        ValidateInsert(NewElement, Index)

        ' Perform the insertion
        This.Attach(NewElement)
        Elms.Insert(Index, NewElement)

        ' Raise events
        This.RaiseChildAdded(NewElement, Index)

    End Sub

    Public Sub InsertAfter(NewElement As MathElement, OldElement As MathElement)

        ' Specal case: inserting an element after Nothing insert it at the beginning
        If OldElement Is Nothing Then InsertAt(NewElement, 0) : Exit Sub

        ' General case: perform InsertAt with the right index
        Dim Index = IndexOf(OldElement) : If Index = -1 Then Throw New ArgumentException("OldElement was not in the children list.")
        InsertAt(NewElement, 1 + Index)

    End Sub

    Public Sub InsertBefore(NewElement As MathElement, OldElement As MathElement)

        ' Specal case: inserting an element before Nothing insert it at the end
        If OldElement Is Nothing Then Add(NewElement) : Exit Sub

        ' General case: perform InsertAt with the right index
        Dim Index = IndexOf(OldElement) : If Index = -1 Then Throw New ArgumentException("OldElement was not in the children list.")
        InsertAt(NewElement, Index)

    End Sub

    Public Sub Replace(OldElement As MathElement, NewElement As MathElement)

        ' Validate replace
        ValidateReplace(OldElement, NewElement)

        ' Preform the replace
        This.Attach(NewElement)

        Dim Index = IndexOf(OldElement)
        Elms.Insert(Index, NewElement)

        This.RaiseChildAdded(NewElement, Index)

        Elms.RemoveAt(Index + 1)
        OldElement.RaiseDetachedFromParent()

        This.RaiseChildRemoved(OldElement, Index)

    End Sub

    Public Sub Remove(OldElement As MathElement)

        ' Validate
        ValidateOldElement(OldElement)

        Dim Index = OldElement.ChildIndex
        ValidateRemove(Index)

        ' Perform
        Elms.RemoveAt(Index)
        OldElement.RaiseDetachedFromParent()

        ' Raise events
        This.RaiseChildRemoved(OldElement, Index)

    End Sub

    Public Sub RemoveAt(Index As Integer)

        ' Validate
        ValidateRemove(Index)

        ' Get element
        Dim OldElement = Elms(Index)

        ' Perform
        Elms.RemoveAt(Index)
        OldElement.RaiseDetachedFromParent()

        ' Raise events
        This.RaiseChildRemoved(OldElement, Index)

    End Sub

    Public Sub Swap(FirstElement As MathElement, SecondElement As MathElement)

        ' Validate
        Dim I1 = ValidateOldElement(FirstElement)
        Dim I2 = ValidateOldElement(SecondElement)
        ValidateSwap(I1, I2)

        ' Perform swap
        Elms(I1) = SecondElement
        Elms(I2) = FirstElement

        ' Raise events
        This.StartBatchProcess()
        This.RaiseSubTreeModified(New MathElement.TreeEventArgs(FirstElement, This, I2, MathElement.TreeEventArgs.TreeAction.Modified))
        This.RaiseSubTreeModified(New MathElement.TreeEventArgs(SecondElement, This, I1, MathElement.TreeEventArgs.TreeAction.Modified))
        This.StopBatchProcess()

    End Sub

    ''' <summary>
    ''' Wraps a element that's currently child of this element inside another.
    ''' Removes InitialChild from this list, adds it to the Wrapper and then add the Wrapper to this list.
    ''' </summary>
    ''' <param name="InitialChild">The child to wrap.</param>
    ''' <param name="Wrapper">The wrapper to use.</param>
    Public Sub Wrap(InitialChild As MathElement, Wrapper As MathElement)
        ' TODO : Check this sub (and all Wrap functions)
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
    Protected Overridable Sub Wrap_Internal(InitialChild As MathElement, Wrapper As MathElement)

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
    Public Sub Wrap(InitialChild As MathElement, Wrapper As MathElement, FinalChild As MathElement)
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
    Protected Overridable Sub Wrap_Internal(InitialChild As MathElement, Wrapper As MathElement, FinalChild As MathElement)

        InsertBefore(Wrapper, InitialChild)

        Dim ICI = If(InitialChild Is Nothing, 0, InitialChild.ChildIndex)
        Dim FCI = If(FinalChild Is Nothing, Me.Count, FinalChild.ChildIndex)
        Dim Children = New SiblingEnumerator(New SelectionHelper.SelectionPoint(This, ICI), New SelectionHelper.SelectionPoint(This, FCI))


        ' TODO: Write test on this matter. SiblingEnumerator implementation changed
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
    Public Overridable Sub Unwrap(Wrapper As MathElement)
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
    ''' Returns an enumerator for the collection.
    ''' </summary>
    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement) Implements System.Collections.Generic.IEnumerable(Of MathElement).GetEnumerator
        Return Elms.GetEnumerator()
    End Function

    Private Function GetGenericEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

End Class
