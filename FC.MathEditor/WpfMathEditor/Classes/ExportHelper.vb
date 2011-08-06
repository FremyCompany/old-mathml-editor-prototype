Public MustInherit Class ExportHelper

    Protected WithEvents This As MathElement
    Public Sub New(This As MathElement)
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

    Public MustOverride Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendLaTeX(SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendMathML(SB As System.Text.StringBuilder)

    Protected MustOverride Sub Draw_Internal(DG As DrawingContext)
    Public Sub Draw(DG As DrawingContext)

        ' Draw background
        If Background <> Colors.Transparent Then
            DG.DrawRectangle(New SolidColorBrush(Background), Nothing, SizeRect)
        End If

        ' Shift the drawing zone using the inner margin
        ' TODO : Initially, I saw 0*IM.Left here. Maybe is there a reason... but I can't remember!
        DG.PushTransform(New TranslateTransform(0 * IM.Left, IM.Top))

        ' Draw content
        Draw_Internal(DG)

        ' Unshift the drawing zone
        DG.Pop()

    End Sub


    Private Property _IsLayoutToDate As Boolean
    Protected Overridable ReadOnly Property IsLayoutToDate As Boolean
        Get
            Return _IsLayoutToDate
        End Get
    End Property

    Public Sub InvalidateLayout(Optional ByVal ForwardToParent As Boolean = False)

        ' Notify the layout should be recomputed
        _IsLayoutToDate = False
        Cached_MinABH = Double.NaN

        ' Forward to parent element
        If ForwardToParent AndAlso This.ParentElement IsNot Nothing Then
            This.ParentElement.Export.InvalidateLayout(True)
        End If

    End Sub

    Public Property LayoutOptions As LayoutOptions

    Protected Sub PerformLayout()
        If Not IsLayoutToDate Then
            ' Compute the minimum size before generating the layout
            GenerateLayout() : _IsLayoutToDate = True
        End If
    End Sub

    ' Fields to be filled by GenerateLayout !
    Protected W As Double
    Protected H As Double
    Protected BH As Double

    ''' <summary>
    ''' Inner margin: used to define the draw box from the black box (glyph run). This margin is not collapsible and is used internally.
    ''' </summary>
    Protected IM As Thickness

    ''' <summary>
    ''' Outer margin: useed to define a margin outside the black box. This margin is collapsible.
    ''' </summary>
    Protected OM As Thickness

    ''' <summary>
    ''' Selection margin: used to define a margin outside the black box that it's used to compute the element selectable area.
    ''' </summary>
    Protected SM As Thickness

    '++
    '++ PrepareLayout phase
    '++

    Private Cached_MinABH As Double = Double.NaN
    Protected MustOverride Function GetMinABH() As Double

    ''' <summary>
    ''' Returns the minimum size of this element (above base line). If this element is fenced, it may be higher (but not smaller) than this size at render time.
    ''' </summary>
    Public ReadOnly Property MinABH As Double
        Get
            If Double.IsNaN(Cached_MinABH) Then Cached_MinABH = GetMinABH()
            Return Cached_MinABH
        End Get
    End Property



    Private Cached_MinBBH As Double = Double.NaN
    Protected MustOverride Function GetMinBBH() As Double

    ''' <summary>
    ''' Returns the minimum size of this element (below base line). If this element is fenced, it may be higher (but not smaller) than this size at render time.
    ''' </summary>
    Public ReadOnly Property MinBBH As Double
        Get
            If Double.IsNaN(Cached_MinBBH) Then Cached_MinBBH = GetMinBBH()
            Return Cached_MinBBH
        End Get
    End Property


    ''' <summary>
    ''' Allows the element to invalidate its layout based on available height
    ''' </summary>
    ''' <param name="AvailABH">Available height above baseline</param>
    ''' <param name="AvailBBH">Available height below baseline</param>
    ''' <remarks></remarks>
    Public MustOverride Sub PrepareLayout(AvailABH As Double, AvailBBH As Double)


    '++
    '++ Other Properties & Methods
    '++


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

    Public ReadOnly Property SelectionMargin As Thickness
        Get
            PerformLayout()
            Return SM
        End Get
    End Property

    Private Shared Function FitRect(ChildRect As Rect, InitalParentRect As Rect, FinalParentRect As Rect) As Rect
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

    Public Property Font As Typeface
        Get
            Return This.Font
        End Get
        Set(value As Typeface)
            This.Font = value
        End Set
    End Property

    Public Property FontSize As Double
        Get
            Return This.FontSize
        End Get
        Set(value As Double)
            This.FontSize = value
        End Set
    End Property

    Public Property Foreground As Color
        Get
            Return This.Foreground
        End Get
        Set(value As Color)
            This.Foreground = value
        End Set
    End Property

    Public Property Background As Color
        Get
            Return This.Background
        End Get
        Set(value As Color)
            This.Background = value
        End Set
    End Property

    Private _LocationInParent As Rect
    Public ReadOnly Property LocationInParent As Rect
        Get
            If This.ParentElement Is Nothing Then
                Return SizeRect
            Else
                This.ParentElement.Export.PerformLayout()
                Return _LocationInParent
            End If
        End Get
    End Property

    Public Sub SetLocationInParent(value As Rect)
        _LocationInParent = value
    End Sub

    Public ReadOnly Property LocationInRoot As Rect
        Get
            If This.ParentElement Is Nothing Then Return LocationInParent

            This.Root.Export.PerformLayout()
            Return FitRect(LocationInParent, This.ParentElement.Export.SizeRect, This.ParentElement.Export.LocationInRoot)

        End Get
    End Property

    Public ReadOnly Property SelectionRectInParent() As Rect
        Get

            If This.ParentElement Is Nothing Then
                Return New Rect(SizeRect.X - SM.Left, SizeRect.Y - SM.Top, SizeRect.Width + SM.Right, SizeRect.Height + SM.Bottom)
            Else
                This.ParentElement.Export.PerformLayout()
                Dim L = _LocationInParent : Dim Zw = _LocationInParent.Width / SizeRect.Width : Dim Zh = _LocationInParent.Height / SizeRect.Height
                Return New Rect(L.X - SM.Left * Zw, L.Y - SM.Top * Zh, L.Width + (SM.Left + SM.Right) * Zw, L.Height + (SM.Top + SM.Bottom) * Zh)
            End If

        End Get
    End Property

    Public ReadOnly Property SelectionRectInRoot() As Rect
        Get
            If This.ParentElement Is Nothing Then Return SelectionRectInParent

            This.Root.Export.PerformLayout()
            Return FitRect(SelectionRectInParent, This.ParentElement.Export.SizeRect, This.ParentElement.Export.LocationInRoot)

        End Get
    End Property

    Public MustOverride Sub GenerateLayout()

    Private Sub This_Changed(sender As Object, e As System.EventArgs) Handles This.Changed
        _IsLayoutToDate = False
    End Sub
End Class

Public Enum LayoutOptions

    Inline = 2 ^ 0          ' Force inline if not  specified otherwhise
    InlineBlock = 2 ^ 1     ' Use inline if it's computed as beautiful
    Block = 2 ^ 2           ' Use block if not specified otherwhise

End Enum