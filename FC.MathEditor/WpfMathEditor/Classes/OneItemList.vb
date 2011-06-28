Public Class OneItemList(Of T) : Implements IList(Of T)

    Private Content As T, Len As Integer

    Public Sub Add(item As T) Implements System.Collections.Generic.ICollection(Of T).Add
        If Len = 0 Then Content = item Else Throw New IndexOutOfRangeException()
    End Sub

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of T).Clear
        Content = Nothing : Len = 0
    End Sub

    Public Function Contains(item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
        Return Len = 1 AndAlso Object.Equals(item, Content)
    End Function

    Public Sub CopyTo(array() As T, arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
        If Len = 1 Then array(arrayIndex) = Content
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of T).Count
        Get
            Return Len
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of T).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function Remove(item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
        If Object.Equals(item, Content) Then
            Content = Nothing : Len = 0
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator
        Return New OneItemListEnumerator(Me)
    End Function

    Public Function IndexOf(item As T) As Integer Implements System.Collections.Generic.IList(Of T).IndexOf
        If Contains(item) Then Return 1
        Return -1
    End Function

    Public Sub Insert(index As Integer, item As T) Implements System.Collections.Generic.IList(Of T).Insert
        Throw New IndexOutOfRangeException()
    End Sub

    Default Public Property Item(index As Integer) As T Implements System.Collections.Generic.IList(Of T).Item
        Get
            If index > 0 AndAlso index < Len Then Return Content
            Throw New IndexOutOfRangeException()
        End Get
        Set(value As T)
            If index = 0 Then Content = value : Len = 1 Else Throw New IndexOutOfRangeException()
        End Set
    End Property

    Public Sub RemoveAt(index As Integer) Implements System.Collections.Generic.IList(Of T).RemoveAt
        If index = 0 Then Content = Nothing
    End Sub

    Private Function GetEnumerator_Generic() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function


    Class OneItemListEnumerator : Implements IEnumerator(Of T)

        Private _oneItemList As OneItemList(Of T)

        Sub New(oneItemList As OneItemList(Of T))
            _oneItemList = oneItemList
        End Sub

        Public ReadOnly Property Current As T Implements System.Collections.Generic.IEnumerator(Of T).Current
            Get
                If Index = 0 Then Return _oneItemList.Content
                Throw New IndexOutOfRangeException()
            End Get
        End Property

        Private ReadOnly Property Current1 As Object Implements System.Collections.IEnumerator.Current
            Get
                Return Current
            End Get
        End Property

        Private Index As Integer = -1
        Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
            Index += 1
            Return Index = 0
        End Function

        Public Sub Reset() Implements System.Collections.IEnumerator.Reset
            Index = -1
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do nothing
        End Sub

    End Class

End Class