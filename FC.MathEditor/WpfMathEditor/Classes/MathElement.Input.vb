﻿Partial Public Class MathElement

    Private _Input As InputHelper = GetInitialInputHelper()
    Public Property Input As InputHelper
        Get
            If Me.ParentDocument Is Nothing Then Return Nothing
            Return _Input
        End Get
        Set(value As InputHelper)
            _Input = value
        End Set
    End Property

    Protected MustOverride Function GetInitialInputHelper() As InputHelper
    Protected MustOverride Function GetInitialExportHelper() As ExportHelper
    Protected MustOverride Function GetInitialChildrenHelper() As ChildrenHelper

    Private _Selection As SelectionHelper
    Public Overridable ReadOnly Property Selection As SelectionHelper
        Get
            If Me.ParentDocument Is Nothing Then Return Nothing
            If _Selection Is Nothing Then _Selection = ParentDocument.Selection
            Return _Selection
        End Get
    End Property

    Public Function GetSelectionAtOrigin() As SelectionHelper.SelectionPoint
        Return New SelectionHelper.SelectionPoint(Me, 0)
    End Function

    Public Function GetSelectionAtEnd() As SelectionHelper.SelectionPoint
        Return New SelectionHelper.SelectionPoint(Me, Me.Children.Count)
    End Function

    Public Function GetSelectionAt(Index As Integer) As SelectionHelper.SelectionPoint
        Return New SelectionHelper.SelectionPoint(Me, Index)
    End Function

    Public Function GetSelectionAfter() As SelectionHelper.SelectionPoint
        If ParentElement Is Nothing Then Return GetSelectionAtEnd()
        Return New SelectionHelper.SelectionPoint(ParentElement, Me.ChildIndex + 1)
    End Function

    Public Function GetSelectionBefore() As SelectionHelper.SelectionPoint
        If ParentElement Is Nothing Then Return GetSelectionAtOrigin()
        Return New SelectionHelper.SelectionPoint(ParentElement, Me.ChildIndex)
    End Function

    Public Event GotFocus As EventHandler
    Public Event LostFocus As EventHandler

    Private _Export As ExportHelper = GetInitialExportHelper()
    Public Property Export As ExportHelper
        Get
            Return _Export
        End Get
        Set(value As ExportHelper)
            _Export = value
        End Set
    End Property

    Public Overridable Function GetNextInputElement(CurrentElement As MathElement) As MathElement
        If Me.IsFormatter Then
            If CurrentElement Is Nothing Then Return Children.First
            Return Children.After(CurrentElement)
        Else
            Return Nothing
        End If
    End Function

    Public Overridable Function GetPreviousInputElement(CurrentElement As MathElement) As MathElement
        If Me.IsFormatter Then
            If CurrentElement Is Nothing Then Return Children.Last
            Return Children.Before(CurrentElement)
        Else
            Return Nothing
        End If
    End Function

    Public ReadOnly Property ParentLayoutEngine As MathElement
        Get
            Dim P As MathElement = ParentElement
            If ParentElement Is Nothing Then
                Throw New InvalidOperationException("Unable to retreive a ParentLayoutEngine from an unparented element.")
            End If

            While P IsNot Nothing
                If P.IsLayoutEngine Then
                    Return P
                Else
                    P = P.ParentElement
                End If
            End While

            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ParentLayoutEngineChild As MathElement
        Get
            Dim P As MathElement = Me
            If ParentElement Is Nothing Then
                Throw New InvalidOperationException("Unable to retreive a ParentLayoutEngine from an unparented element.")
            End If

            While P IsNot Nothing
                If P.ParentElement.IsLayoutEngine Then
                    Return P
                Else
                    P = P.ParentElement
                End If
            End While

            Return Nothing
        End Get
    End Property

    Public Sub New()
        _Children = GetInitialChildrenHelper()
        _Export = GetInitialExportHelper()
        _Input = GetInitialInputHelper()
        If _Input Is Nothing Then
            Trace.TraceError("Bug!")
        End If
    End Sub

    Public Sub RemoveFromParent()
        ParentElement.RemoveChild(Me)
    End Sub

End Class
