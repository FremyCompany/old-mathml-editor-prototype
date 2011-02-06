﻿Public MustInherit Class ChildrenHelper
    Implements IEnumerable(Of MathElement)

    Protected This As MathElement
    Public Sub New(ByVal This As MathElement)
        Me.This = This
    End Sub

    REM
    REM Fundamental traps
    REM

    Public MustOverride Sub Add(ByVal NewChild As MathElement)
    Public MustOverride Sub Remove(ByVal OldChild As MathElement)
    Public MustOverride Sub InsertAfter(ByVal NewChild As MathElement, ByVal OldChild As MathElement)

    Public MustOverride Function After(ByVal OldChild As MathElement) As MathElement
    Public MustOverride Function Before(ByVal OldChild As MathElement) As MathElement

    Public MustOverride ReadOnly Property First() As MathElement

    REM
    REM Operational traps
    REM

    Public MustOverride ReadOnly Property CanHave As Boolean
    Public MustOverride ReadOnly Property CanAdd As Boolean
    Public MustOverride ReadOnly Property CanRemove As Boolean

    Public Overridable ReadOnly Property CanReplace As Boolean
        Get
            Return CanAdd AndAlso CanRemove
        End Get
    End Property

    REM
    REM Element Behavior
    REM

    Public MustOverride ReadOnly Property ElementType As MathElement.Type
    Public ReadOnly Property IsLayoutEngine As Boolean
        Get
            Return ElementType = MathElement.Type.LayoutEngine
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


    REM
    REM Derived traps
    REM

    Public Overridable Sub InsertBefore(ByVal NewChild As MathElement, ByVal OldChild As MathElement)
        InsertAfter(NewChild, Before(OldChild))
    End Sub

    Public Overridable Sub Replace(ByVal OldChild As MathElement, ByVal NewChild As MathElement)
        InsertAfter(NewChild, OldChild) : Remove(OldChild)
    End Sub

    Public Overridable Sub Swap(ByVal FirstChild As MathElement, ByVal SecondChild As MathElement)
        Dim BeforeFirstChild = Before(FirstChild)
        Remove(FirstChild)
        Replace(SecondChild, FirstChild)
        InsertAfter(SecondChild, BeforeFirstChild)
    End Sub

    Public Overridable Sub Wrap(ByVal InitialChild As MathElement, ByVal Wrapper As MathElement)
        Wrapper.AddChild(InitialChild.Clone())
        Replace(InitialChild, Wrapper)
    End Sub

    Public Overridable Sub Unwrap(ByVal Wrapper As MathElement)
        Dim Children = Wrapper.Children.GetEnumerator()
        Dim LastChild = Wrapper, CurrentChild As MathElement
        While Children.MoveNext()
            CurrentChild = Children.Current.Clone()
            InsertAfter(LastChild, CurrentChild)
            LastChild = CurrentChild
        End While
        Remove(Wrapper)
    End Sub

    Public Overridable ReadOnly Property Last() As MathElement
        Get
            Return CType(Me, IEnumerable(Of MathElement)).Last()
        End Get
    End Property

    Public Overridable Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of MathElement) Implements System.Collections.Generic.IEnumerable(Of MathElement).GetEnumerator
        Return New SiblingEnumerator(Me.First)
    End Function

    Private Function GetUntypedEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
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
        If (NextEl Is Nothing) OrElse (Current Is LastEl) Then
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
