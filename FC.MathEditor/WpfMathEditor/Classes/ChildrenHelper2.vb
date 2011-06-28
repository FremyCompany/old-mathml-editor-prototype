Public MustInherit Class ChildrenHelper2
    Implements IEnumerable(Of MathElement)

    '++
    '++ Private fields
    '++
    Protected This As MathElement
    Private Elms As New List(Of MathElement)

    '++
    '++ Properties
    '++

#Region "Element type"
    Public MustOverride Property ElementType As MathElement.Type
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

    Public MustOverride ReadOnly Property CanAdd() As Boolean
    Public MustOverride ReadOnly Property CanInsert() As Boolean
    Public MustOverride ReadOnly Property CanRemove() As Boolean
    Public MustOverride ReadOnly Property CanSwap() As Boolean

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

    Protected MustOverride Sub ValidateInsert_Internal(NewElement As MathElement, Index As Integer)
    Public Sub ValidateInsert(NewElement As MathElement, Index As Integer)

        If NewElement Is Nothing Then Throw New ArgumentNullException("NewElement", "Attempt to insert Nothing in a children list has been rejected")
        If NewElement.ParentElement IsNot Nothing Then Throw New ArgumentException("NewElement", "Attempt to insert a parented element in a children list has been rejected")

        If Index < 0 Then Throw New ArgumentOutOfRangeException("Index", "Attempt to insert an element at a negative index has been rejected")
        If Index > Elms.Count Then Throw New ArgumentOutOfRangeException("Index", "Attempt to insert an element at a position further than the children list count has been rejected")

        If This.IsTextEdit AndAlso Not NewElement.IsGlyph Then Throw New ArgumentException("Attemp to insert a non-glyph element inside a text editor has been rejected")
        ValidateInsert_Internal(NewElement, Index)

    End Sub

    Protected MustOverride Sub ValidateSwap_Internal(OldPos As Integer, NewPos As Integer)
    Public Sub ValidateSwap(OldPos As Integer, NewPos As Integer)

    End Sub

    '++
    '++ [...]
    '++


    '++
    '++ Action Methods
    '++

    Public Sub Add(NewElement As MathElement)

    End Sub

    Public Sub InsertAt(NewElement As MathElement, Index As Integer)
        If Index = Elms.Count Then Add(NewElement) : Exit Sub
        Elms.Insert(Index, NewElement)
    End Sub

    Public Sub InsertAfter(NewElement As MathElement, OldElement As MathElement)
        If OldElement Is Nothing Then InsertAt(NewElement, 0)
    End Sub

    Public Sub InsertBefore(NewElement As MathElement, OldElement As MathElement)

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
