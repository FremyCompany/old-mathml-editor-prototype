Public Class TextEditChildrenHelper : Inherits ChildrenHelper

    Protected P As LayoutEngineChildrenHelper
    Protected Shadows ReadOnly Property This As TextEdit
        Get
            Return MyBase.This
        End Get
    End Property

    Public Sub New(ByVal This As TextEdit)
        MyBase.New(This)
        P = New LayoutEngineChildrenHelper(This)
    End Sub

    Public Overrides Sub Add_Internal(ByVal NewChild As MathElement)
        If IsNothing(TryCast(NewChild, UnicodeGlyph)) Then
            Throw New ArgumentException("Only UnicodeGlyph instances can bee added as children of a TextEdit element.")
        Else
            P.Add(NewChild)
        End If
    End Sub

    Public Overrides Function After(ByVal OldChild As MathElement) As MathElement
        Return P.After(OldChild)
    End Function

    Public Overrides Function Before(ByVal OldChild As MathElement) As MathElement
        Return P.Before(OldChild)
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
            Return MathElement.Type.TextEdit
        End Get
    End Property

    Public Overrides ReadOnly Property First As MathElement
        Get
            Return P.First
        End Get
    End Property

    Public Overrides Sub InsertAfter(ByVal NewChild As MathElement, ByVal OldChild As MathElement)
        If TryCast(NewChild, UnicodeGlyph) Is Nothing Then Throw New ArgumentException("")
        P.InsertAfter(NewChild, OldChild)
    End Sub

    Public Overrides Sub Remove_Internal(ByVal OldChild As MathElement)
        P.Remove_Internal(OldChild)
    End Sub

    Public Overrides Sub InsertBefore(ByVal NewChild As MathElement, ByVal OldChild As MathElement)
        If TryCast(NewChild, UnicodeGlyph) Is Nothing Then Throw New ArgumentException("")
        P.InsertBefore(NewChild, OldChild)
    End Sub

    Public Overrides ReadOnly Property Last As MathElement
        Get
            Return P.Last
        End Get
    End Property

    Public Overrides Sub Replace(ByVal OldChild As MathElement, ByVal NewChild As MathElement)
        If TryCast(NewChild, UnicodeGlyph) Is Nothing Then Throw New ArgumentException("")
        P.Replace(OldChild, NewChild)
    End Sub

    Public Overrides Sub Swap(ByVal FirstChild As MathElement, ByVal SecondChild As MathElement)
        P.Swap(FirstChild, SecondChild)
    End Sub

    Public Overrides Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement)
        Return P.GetEnumerator()
    End Function

End Class
