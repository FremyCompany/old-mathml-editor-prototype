Public Class LayoutEngineChildrenHelper : Inherits ChildrenHelper

    Public Sub New(ByVal This As MathElement)
        MyBase.New(This)
    End Sub

    Protected All As New List(Of MathElement)
    Public Overrides ReadOnly Property Count As Integer
        Get
            Return All.Count
        End Get
    End Property

    Public Overrides Function IndexOf(ByVal Element As MathElement) As Integer
        Return All.IndexOf(Element)
    End Function

    Public Overrides Function Contains(ByVal Element As MathElement) As Boolean
        Return All.Contains(Element)
    End Function

    Protected Overrides Function IsValidChild_Internal(ByVal Element As MathElement) As Boolean
        ' Does not accept UnicodeGlyph as a LayoutEngine child
        ' UnicodeGlyph should be wrapped into a TextEdit
        Return MyBase.IsValidChild_Internal(Element) AndAlso TryCast(Element, UnicodeGlyph) Is Nothing
    End Function

    Public Overrides ReadOnly Property Last As MathElement
        Get
            If All.Count = 0 Then Return Nothing
            Return All(All.Count - 1)
        End Get
    End Property

    Protected Overrides Sub Add_Internal(ByVal NewChild As MathElement)
        All.Add(NewChild)
    End Sub

    Public Overrides Function After(ByVal OldChild As MathElement) As MathElement
        If OldChild Is Nothing Then Return Last
        Dim X As Integer = All.IndexOf(OldChild)
        If X = -1 Then Throw New ArgumentException("OldChild was not a child of this element.", "OldChild")
        If X <> All.Count - 1 Then Return All(X + 1)
        Return Nothing
    End Function

    Public Overrides Function Before(ByVal OldChild As MathElement) As MathElement
        If OldChild Is Nothing Then Return First
        Dim X As Integer = All.IndexOf(OldChild)
        If X = -1 Then Throw New ArgumentException("OldChild was not a child of this element.", "OldChild")
        If X <> 0 Then Return All(X - 1)
        Return Nothing
    End Function

    Public Overrides ReadOnly Property CanAdd As Boolean
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

    Protected Overrides Sub InsertAfter_Internal(ByVal NewChild As MathElement, ByVal OldChild As MathElement)

        If OldChild Is Nothing Then
            All.Insert(0, NewChild)
        Else
            All.Insert(All.IndexOf(OldChild) + 1, NewChild)
        End If

    End Sub

    Protected Overrides Sub InsertBefore_Internal(ByVal NewChild As MathElement, ByVal OldChild As MathElement)

        If OldChild Is Nothing Then
            All.Add(NewChild)
        Else
            All.Insert(All.IndexOf(OldChild) - 1, NewChild)
        End If

    End Sub

    Protected Overrides Sub Remove_Internal(ByVal OldChild As MathElement)
        All.Remove(OldChild)
    End Sub

    Protected Overrides Sub Replace_Internal(ByVal OldChild As MathElement, ByVal NewChild As MathElement)
        NewChild.Parent = This
        All(All.IndexOf(OldChild)) = NewChild
        OldChild.Parent = Nothing
    End Sub

    Public Overrides Sub Swap(ByVal FirstChild As MathElement, ByVal SecondChild As MathElement)

        Dim FCI = All.IndexOf(FirstChild)
        Dim SCI = All.IndexOf(SecondChild)

        If FCI = -1 OrElse SCI = -1 Then Throw New ArgumentException("The elements to swap are not child of this element.")

        All(FCI) = SecondChild
        All(SCI) = FirstChild

    End Sub

    Public Overrides Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement)
        Return All.GetEnumerator()
    End Function

End Class
