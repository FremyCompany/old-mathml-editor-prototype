Public Class LayoutEngineChildrenHelper : Inherits ChildrenHelper

    Public Sub New(ByVal This As MathElement)
        MyBase.New(This)
    End Sub

    Protected All As New List(Of MathElement)

    ''' <summary>
    ''' Gets the total amount of children in this instance.
    ''' </summary>
    Public Overrides ReadOnly Property Count As Integer
        Get
            Return All.Count
        End Get
    End Property

    ''' <summary>
    ''' Returns the index of the specified element in this children list, or -1 if it wasn't found.
    ''' </summary>
    ''' <param name="Element">The element to search for.</param><returns></returns>
    Public Overrides Function IndexOf(ByVal Element As MathElement) As Integer
        Return All.IndexOf(Element)
    End Function

    ''' <summary>
    ''' Determines whether this children list contains the specified element.
    ''' </summary>
    ''' <param name="Element">The element to check for</param><returns></returns>
    Protected Overrides Function Contains_Internal(ByVal Element As MathElement) As Boolean
        Return All.Contains(Element)
    End Function

    ''' <summary>
    ''' Returns True if the specified non-null element can be contained in the current children list.
    ''' </summary>
    ''' <param name="Element">The element to be checked</param><returns></returns>
    Protected Overrides Function CanContains_Internal(ByVal Element As MathElement) As Boolean
        Return MyBase.CanContains_Internal(Element)
    End Function

    ''' <summary>
    ''' Get the last child of this list.
    ''' </summary>
    Public Overrides ReadOnly Property Last As MathElement
        Get
            If All.Count = 0 Then Return Nothing
            Return All(All.Count - 1)
        End Get
    End Property

    ''' <summary>
    ''' Adds NewChild to the internal representation of the children list.
    ''' </summary>
    ''' <param name="NewChild">The child to add</param>
    Protected Overrides Sub Add_Internal(ByVal NewChild As MathElement)
        All.Add(NewChild)
    End Sub

    ''' <summary>
    ''' Returns the child located after the specified one in this children list
    ''' </summary>
    ''' <param name="OldChild">The old child.</param><returns></returns>
    Public Overrides Function After(ByVal OldChild As MathElement) As MathElement
        If OldChild Is Nothing Then Return Last
        Dim X As Integer = All.IndexOf(OldChild)
        If X = -1 Then Throw New ArgumentException("OldChild was not a child of this element.", "OldChild")
        If X <> All.Count - 1 Then Return All(X + 1)
        Return Nothing
    End Function

    ''' <summary>
    ''' Returns the child located before the specified one in this children list.
    ''' </summary>
    ''' <param name="OldChild">The old child.</param><returns></returns>
    Public Overrides Function Before(ByVal OldChild As MathElement) As MathElement
        If OldChild Is Nothing Then Return First
        Dim X As Integer = All.IndexOf(OldChild)
        If X = -1 Then Throw New ArgumentException("OldChild was not a child of this element.", "OldChild")
        If X <> 0 Then Return All(X - 1)
        Return Nothing
    End Function

    ''' <summary>
    ''' Returns True if a call to Add() may succeed.
    ''' </summary>
    Public Overrides ReadOnly Property CanAdd As Boolean
        Get
            Return True
        End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether this instance can contains any child.
    ''' </summary>
    ''' 
    Public Overrides ReadOnly Property CanHave As Boolean
        Get
            Return True
        End Get
    End Property

    ''' <summary>
    ''' Returns True if a call to Remove() may succeed.
    ''' </summary>
    ''' 
    Public Overrides ReadOnly Property CanRemove As Boolean
        Get
            Return True
        End Get
    End Property

    ''' <summary>
    ''' Returns the type of behavior you should expect from this children list.
    ''' </summary>
    ''' 
    Public Overrides ReadOnly Property ElementType As MathElement.Type
        Get
            Return MathElement.Type.LayoutEngine
        End Get
    End Property

    ''' <summary>
    ''' Gets the first child of this list.
    ''' </summary>
    ''' 
    Public Overrides ReadOnly Property First As MathElement
        Get
            If All.Count = 0 Then Return Nothing
            Return All(0)
        End Get
    End Property

    ''' <summary>
    ''' Inserts a child after another in the in the internal representation of the children list.
    ''' </summary>
    ''' <param name="NewChild">The child to insert</param>
    ''' <param name="OldChild">The child after which to insert</param>
    Protected Overrides Sub InsertAfter_Internal(ByVal NewChild As MathElement, ByVal OldChild As MathElement)

        If OldChild Is Nothing Then
            All.Insert(0, NewChild)
        Else
            All.Insert(All.IndexOf(OldChild) + 1, NewChild)
        End If

    End Sub

    ''' <summary>
    ''' Inserts (in the internal representation) a new element before an existing element.
    ''' </summary>
    ''' <param name="NewChild">The element to be inserted.</param>
    ''' <param name="OldChild">The element before which NewChild should be inserted.</param>
    Protected Overrides Sub InsertBefore_Internal(ByVal NewChild As MathElement, ByVal OldChild As MathElement)

        If OldChild Is Nothing Then
            All.Add(NewChild)
        Else
            All.Insert(All.IndexOf(OldChild) - 1, NewChild)
        End If

    End Sub

    ''' <summary>
    ''' Removes a child from the internal representation of the children list
    ''' </summary>
    ''' <param name="OldChild">The child to remove</param>
    Protected Overrides Sub Remove_Internal(ByVal OldChild As MathElement)
        All.Remove(OldChild)
    End Sub

    ''' <summary>
    ''' Replaces an exising element by another in the (internal representation of the) children list.
    ''' </summary>
    ''' <param name="OldChild">The element to be removed from the children list.</param>
    ''' <param name="NewChild">The element to be added to the children list.</param>
    Protected Overrides Sub Replace_Internal(ByVal OldChild As MathElement, ByVal NewChild As MathElement)

        All(All.IndexOf(OldChild)) = NewChild

    End Sub

    ''' <summary>
    ''' Exchanges the position of two elements inside the (internal representation of the) list.
    ''' </summary>
    ''' <param name="FirstChild">The first child.</param>
    ''' <param name="SecondChild">The second child.</param>
    Protected Overrides Sub Swap_Internal(ByVal FirstChild As MathElement, ByVal SecondChild As MathElement)

        Dim FCI = All.IndexOf(FirstChild)
        Dim SCI = All.IndexOf(SecondChild)

        If FCI = -1 OrElse SCI = -1 Then Throw New ArgumentException("The elements to swap are not child of this element.")

        All(FCI) = SecondChild
        All(SCI) = FirstChild

    End Sub

    ''' <summary>
    ''' Gets the enumerator.
    ''' </summary><returns></returns>
    Public Overrides Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement)
        Return All.GetEnumerator()
    End Function

End Class
