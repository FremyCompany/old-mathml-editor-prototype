Public Class SiblingEnumeratorGenerator : Implements IEnumerable(Of MathElement)

    Private StartPoint, EndPoint As SelectionHelper.SelectionPoint

    Public Sub New(ByVal ParentToWalk As MathElement)
        StartPoint = New SelectionHelper.SelectionPoint(ParentToWalk, 0)
        EndPoint = StartPoint.GetEnd()
    End Sub

    Public Sub New(ByVal StartPoint As SelectionHelper.SelectionPoint, ByVal EndPoint As SelectionHelper.SelectionPoint)
        Me.StartPoint = StartPoint
        Me.EndPoint = EndPoint
    End Sub

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement) Implements System.Collections.Generic.IEnumerable(Of MathElement).GetEnumerator
        Return New SiblingEnumerator(StartPoint, EndPoint)
    End Function

    Private Function GetGenericEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

End Class

Public Class SiblingEnumerator : Implements IEnumerator(Of MathElement)

    Public Sub New(ByVal ParentElement As MathElement, ByVal BoolToDelete As Boolean)

    End Sub

    Public Sub New(ByVal StartPoint As SelectionHelper.SelectionPoint)
        Me.StartPoint = StartPoint : CurrentPoint = StartPoint : EndPoint = StartPoint.GetEnd()
    End Sub

    ''' <summary>
    ''' Retourne les éléments situés après StartPoint et avant EndPoint dans leur parent commun. S'ils n'ont pas le même parent, une erreur est retournée.
    ''' </summary>
    ''' <param name="StartPoint">Point de départ de l'énumération</param>
    ''' <param name="EndPoint">Point d'arrivée de l'énumération</param>
    Public Sub New(ByVal StartPoint As SelectionHelper.SelectionPoint, ByVal EndPoint As SelectionHelper.SelectionPoint)

        ' Vérifie la cohérence des données
        If EndPoint Is Nothing Then EndPoint = StartPoint.GetEnd()
        If StartPoint.ParentElement IsNot EndPoint.ParentElement Then Throw New ArgumentException("Un énumérateur d'enfant ne peut avoir son début et sa fin dans des éléments différents.")

        ' Ecrit les données en mémoire
        Me.StartPoint = StartPoint : Me.CurrentPoint = StartPoint : Me.EndPoint = EndPoint

    End Sub

    Private StartPoint As SelectionHelper.SelectionPoint
    Private CurrentPoint As SelectionHelper.SelectionPoint
    Private EndPoint As SelectionHelper.SelectionPoint

    ''' <summary>
    ''' Retourne l'élément en cours
    ''' </summary>
    Public ReadOnly Property Current As MathElement Implements System.Collections.Generic.IEnumerator(Of MathElement).Current
        Get
            AssertNotDisposed()
            Return CurrentPoint.PreviousSibling
        End Get
    End Property

    Private ReadOnly Property GenericCurrent As Object Implements System.Collections.IEnumerator.Current
        Get
            Return Current
        End Get
    End Property

    ''' <summary>
    ''' Augmente la position du pointeur de un élément
    ''' </summary>
    ''' <returns>True si l'incrément a pu être effectué, False sinon</returns>
    ''' <remarks>Cette fonction peut retourner une InvalidOperationException si son pointeur courrant est devenu invalide</remarks>
    Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
        AssertNotDisposed()

        If CurrentPoint.IsValid Then
            ' Passe au point suivant, si on peut (<max) et si il existe (!at end)
            If CurrentPoint.ChildIndex >= EndPoint.ChildIndex OrElse CurrentPoint.IsAtEnd Then
                Return False
            Else
                CurrentPoint = CurrentPoint.Increment(1)
                Return True
            End If
        Else
            ' En cas d'invalidation, on retourne une erreur
            Throw New InvalidOperationException("The enumerated element was invalidated. Unable to continue the enumeration.")
        End If
    End Function

    ''' <summary>
    ''' Retourne le pointeur courrant à son état initial
    ''' </summary>
    Public Sub Reset() Implements System.Collections.IEnumerator.Reset
        AssertNotDisposed()
        CurrentPoint = StartPoint
    End Sub

    Private Disposed As Boolean = False
    Public Sub Dispose() Implements IDisposable.Dispose
        Me.Disposed = True
    End Sub

    Private Sub AssertNotDisposed()
        If Disposed Then Throw New InvalidOperationException("The enumerator has been disposed. Unable to continue the enumeration.")
    End Sub

End Class


'Public Class SiblingEnumeratorGenerator : Implements IEnumerable(Of MathElement)

'    Private FirstEl, LastEl As MathElement
'    Public Sub New(ByVal FirstEl As MathElement)
'        Me.New(FirstEl, Nothing)
'    End Sub

'    Public Sub New(ByVal FirstEl As MathElement, ByVal LastEl As MathElement)
'        Me.FirstEl = FirstEl : Me.LastEl = LastEl
'    End Sub

'    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement) Implements System.Collections.Generic.IEnumerable(Of MathElement).GetEnumerator
'        Return New SiblingEnumerator(FirstEl, LastEl)
'    End Function

'    Public Function GetGenericEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
'        Return GetEnumerator()
'    End Function

'End Class

'Public Class SiblingEnumerator : Implements IEnumerator(Of MathElement)

'    Private FirstEl As MathElement, NextEl As MathElement, CurrentEl As MathElement
'    Public Sub New(ByVal FirstEl As MathElement)
'        Me.FirstEl = FirstEl : Me.NextEl = FirstEl
'    End Sub

'    Private LastEl As MathElement
'    Public Sub New(ByVal FirstEl As MathElement, ByVal LastEl As MathElement)
'        Me.FirstEl = FirstEl : Me.NextEl = FirstEl : Me.LastEl = LastEl
'    End Sub

'    Public ReadOnly Property Current As MathElement Implements System.Collections.Generic.IEnumerator(Of MathElement).Current
'        Get
'            Return CurrentEl
'        End Get
'    End Property

'    Public ReadOnly Property CurrentUnTyped As Object Implements System.Collections.IEnumerator.Current
'        Get
'            Return CurrentEl
'        End Get
'    End Property

'    Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
'        If (NextEl Is Nothing) OrElse (LastEl IsNot Nothing AndAlso Current Is LastEl) Then
'            CurrentEl = Nothing
'            Return False
'        Else
'            CurrentEl = NextEl
'            NextEl = CurrentEl.NextSibling
'            Return True
'        End If
'    End Function

'    Public Sub Reset() Implements System.Collections.IEnumerator.Reset
'        NextEl = FirstEl : CurrentEl = Nothing
'    End Sub

'    Public Sub Dispose() Implements IDisposable.Dispose
'        FirstEl = Nothing : NextEl = Nothing : CurrentEl = Nothing
'    End Sub

'End Class
