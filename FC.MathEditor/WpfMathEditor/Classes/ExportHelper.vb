﻿Public MustInherit Class ExportHelper

    Protected WithEvents This As MathElement
    Public Sub New(ByVal This As MathElement)
        Me.This = This
    End Sub

    Public Function ToKeyboardInput() As String
        Dim SB As New System.Text.StringBuilder()
        AppendKeyboardInput(SB)
        Return SB.ToString()
    End Function

    Public Function ToLaTeX() As String
        Dim SB As New System.Text.StringBuilder()
        AppendLaTeX(SB)
        Return SB.ToString()
    End Function

    Public Function ToMathML() As String
        Dim SB As New System.Text.StringBuilder()
        AppendMathML(SB)
        Return SB.ToString()
    End Function

    Public Function ToBitmap() As DrawingImage
        Dim DV As New DrawingVisual()
        Dim DG = DV.RenderOpen()
        Draw(DG)
        Return New DrawingImage(DV.Drawing)
    End Function

    Public MustOverride Sub AppendKeyboardInput(ByVal SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendLaTeX(ByVal SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendMathML(ByVal SB As System.Text.StringBuilder)

    Public MustOverride Sub Draw(ByVal DG As DrawingContext)

    Private Property _IsLayoutToDate As Boolean
    Protected Overridable ReadOnly Property IsLayoutToDate As Boolean
        Get
            Return _IsLayoutToDate
        End Get
    End Property

    Public Sub InvalidateLayout()
        _IsLayoutToDate = False
    End Sub

    Public Property LayoutOptions As LayoutOptions

    Protected Sub PerformLayout()
        If Not IsLayoutToDate Then
            GenerateLayout() : _IsLayoutToDate = True
        End If
    End Sub

    ' Fields to be filled by GenerateLayout !
    Protected W As Double
    Protected H As Double
    Protected BH As Double

    Protected IM As Thickness
    Protected OM As Thickness

    Public ReadOnly Property Width As Double
        Get
            PerformLayout()
            Return W
        End Get
    End Property

    Public ReadOnly Property Height As Double
        Get
            PerformLayout()
            Return H
        End Get
    End Property

    Public ReadOnly Property DrawWidth As Double
        Get
            PerformLayout()
            Return W - IM.Left - IM.Right
        End Get
    End Property

    Public ReadOnly Property DrawHeight As Double
        Get
            PerformLayout()
            Return H - IM.Top - IM.Bottom
        End Get
    End Property

    Public ReadOnly Property BaseLine As Double
        Get
            PerformLayout()
            Return BH
        End Get
    End Property

    Public ReadOnly Property AboveBaseLineHeight As Double
        Get
            PerformLayout()
            Return Math.Max(0, H - BH)
        End Get
    End Property

    Public ReadOnly Property BelowBaseLineHeight As Double
        Get
            PerformLayout()
            Return Math.Max(0, BH)
        End Get
    End Property

    Public ReadOnly Property DrawRectangle() As Rect
        Get
            PerformLayout()
            Return New Rect(IM.Left, IM.Top, DrawWidth, DrawHeight)
        End Get
    End Property

    Public ReadOnly Property OuterMargin As Thickness
        Get
            PerformLayout()
            Return OM
        End Get
    End Property

    Private Shared Function FitRect(ByVal ChildRect As Rect, ByVal InitalParentRect As Rect, ByVal FinalParentRect As Rect) As Rect
        ChildRect.Offset(-CType(InitalParentRect.Location, Vector))
        ChildRect.Scale(FinalParentRect.Width / InitalParentRect.Width, FinalParentRect.Height / InitalParentRect.Height)
        ChildRect.Offset(FinalParentRect.Location)
        Return ChildRect
    End Function

    Public ReadOnly Property Size As Size
        Get
            PerformLayout()
            Return New Size(W, H)
        End Get
    End Property

    Public ReadOnly Property SizeRect As Rect
        Get
            Return New Rect(Size)
        End Get
    End Property

    Private _LocationInParent As Rect
    Public ReadOnly Property LocationInParent As Rect
        Get
            PerformLayout()
            If This.Parent Is Nothing Then
                Return SizeRect
            Else
                Return _LocationInParent 'This.Parent.Export.GetChildLocation(This)
            End If
        End Get
    End Property

    Public Sub SetLocationInParent(ByVal value As Rect)
        _LocationInParent = value
    End Sub

    Public ReadOnly Property LocationInRoot As Rect
        Get
            If This.Parent Is Nothing Then Return LocationInParent

            Dim L = LocationInParent
            Return FitRect(LocationInParent, This.Parent.Export.SizeRect, This.Parent.Export.LocationInRoot)

        End Get
    End Property

    Public MustOverride Sub GenerateLayout()


End Class

Public Enum LayoutOptions

    Inline = 2 ^ 0          ' Force inline if not  specified otherwhise
    InlineBlock = 2 ^ 1     ' Use inline if it's computed as beautiful
    Block = 2 ^ 2           ' Use block if not specified otherwhise

End Enum