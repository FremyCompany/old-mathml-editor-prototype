Public Class TextEditChildrenHelper : Inherits ChildrenHelper


    ''' <summary>
    ''' Gets the element for which this children list is maintained
    ''' </summary>
    Protected Shadows ReadOnly Property This As TextEdit
        Get
            Return DirectCast(MyBase.This, TextEdit)
        End Get
    End Property

    ''' <summary>
    ''' Initializes a new instance of the <see cref="TextEditChildrenHelper" /> class.
    ''' </summary>
    ''' <param name="This">The base element</param>
    Sub New(This As TextEdit)
        MyBase.New(This)
    End Sub

    ''' <summary>
    ''' The list of all children maintained by this instance
    ''' </summary>
    Protected All As New List(Of UnicodeGlyph)

    ''' <summary>
    ''' Get the indexes the of specified element
    ''' </summary>
    ''' <param name="Element">The element to search for</param><returns></returns>
    Public Overrides Function IndexOf(Element As MathElement) As Integer
        Return All.IndexOf(Element)
    End Function

    ''' <summary>
    ''' Determines whether this children list contains the specified element.
    ''' </summary>
    ''' <param name="Element">The element to search for</param>
    Protected Overrides Function Contains_Internal(Element As MathElement) As Boolean
        Return All.Contains(Element)
    End Function

    ''' <summary>
    ''' Gets the last child of this list.
    ''' </summary>
    Public Overrides ReadOnly Property Last As MathElement
        Get
            If All.Count = 0 Then Return Nothing
            Return All(All.Count - 1)
        End Get
    End Property

    Protected Overrides Sub Add_Internal(NewChild As MathElement)
        All.Add(NewChild)
    End Sub

    Public Overrides Function After(OldChild As MathElement) As MathElement
        If OldChild Is Nothing Then Return Last
        Dim X As Integer = All.IndexOf(OldChild)
        If X = -1 Then Throw New ArgumentException("OldChild was not a child of this element.", "OldChild")
        If X <> All.Count - 1 Then Return All(X + 1)
        Return Nothing
    End Function

    Public Overrides Function Before(OldChild As MathElement) As MathElement
        If OldChild Is Nothing Then Return First
        Dim X As Integer = All.IndexOf(OldChild)
        If X = -1 Then Throw New ArgumentException("OldChild was not a child of this element.", "OldChild")
        If X <> 0 Then Return All(X - 1)
        Return Nothing
    End Function

    Public Overrides ReadOnly Property CanAdd As Boolean
        Get
            Return (True) AndAlso (HasNo OrElse This.CanHaveMultipleChild())
        End Get
    End Property

    Public Overrides ReadOnly Property CanReplace As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanHave As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanRemove As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property ElementType As MathElement.Type
        Get
            Return MathElement.Type.LayoutEngine
        End Get
    End Property

    Public Overrides ReadOnly Property First As MathElement
        Get
            If All.Count = 0 Then Return Nothing
            Return All(0)
        End Get
    End Property

    Protected Overrides Sub InsertAfter_Internal(NewChild As MathElement, OldChild As MathElement)

        If OldChild Is Nothing Then
            All.Insert(0, NewChild)
        Else
            All.Insert(All.IndexOf(OldChild) + 1, NewChild)
        End If

    End Sub

    Protected Overrides Sub InsertBefore_Internal(NewChild As MathElement, OldChild As MathElement)

        If OldChild Is Nothing Then
            All.Add(NewChild)
        Else
            All.Insert(All.IndexOf(OldChild) - 1, NewChild)
        End If

    End Sub

    Protected Overrides Sub Remove_Internal(OldChild As MathElement)
        All.Remove(OldChild)
    End Sub

    Protected Overrides Sub Replace_Internal(OldChild As MathElement, NewChild As MathElement)
        This.Attach(NewChild)
        All(All.IndexOf(OldChild)) = NewChild
        This.Detach(OldChild)
    End Sub

    Protected Overrides Sub Swap_Internal(FirstChild As MathElement, SecondChild As MathElement)

        Dim FCI = All.IndexOf(FirstChild)
        Dim SCI = All.IndexOf(SecondChild)

        If FCI = -1 OrElse SCI = -1 Then Throw New ArgumentException("The elements to swap are not child of this element.")

        All(FCI) = SecondChild
        All(SCI) = FirstChild

    End Sub

    Public Overrides Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement)
        Return All.GetEnumerator()
    End Function

    Public Overrides ReadOnly Property Count As Integer
        Get
            Return All.Count
        End Get
    End Property

End Class
