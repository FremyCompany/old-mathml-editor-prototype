Public MustInherit Class ExportHelper

    Protected WithEvents This As MathElement

    Public Sub New(This As MathElement)
        Me.This = This
    End Sub

    Public ReadOnly Property MinimalHeight As Double
        Get
            Return MinimalABH + MinimalBBH
        End Get
    End Property

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

    Public Function ToSimpleText() As String
        Dim SB As New System.Text.StringBuilder()
        AppendSimpleText(SB)
        Return SB.ToString()
    End Function

    Public Function ToBitmap() As DrawingImage
        Dim DV As New DrawingVisual()
        Dim DG = DV.RenderOpen()
        Draw(DG)
        Return New DrawingImage(DV.Drawing)
    End Function

    Public Overrides Function ToString() As String
        Dim SB As New System.Text.StringBuilder()
        AppendSimpleText(SB)
        Return SB.ToString()
    End Function

    Public MustOverride Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendLaTeX(SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendMathML(SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendSimpleText(SB As System.Text.StringBuilder)

    Protected MustOverride Sub Draw_Internal(DG As DrawingContext)
    Public Sub Draw(DG As DrawingContext)


        ' Shift the drawing zone using the inner margin
        ' TODO : Initially, I saw 0*IM.Left here. Maybe is there a reason... but I can't remember!
        DG.PushTransform(New TranslateTransform(IM.Left, IM.Top))

        ' Align on physical pixels
        If This.ElementType = MathElement.Type.Glyph Then
            Dim DPIMatrix = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice
            Dim trABH = AboveBaseLineHeight
            Try : Console.WriteLine(DirectCast(This, UnicodeGlyph).DisplayChar & "; " & AboveBaseLineHeight & "; " & BelowBaseLineHeight)
            Catch : End Try
            Dim gl As New GuidelineSet(New Double() {-0.5 / DPIMatrix.M11}, New Double() {trABH - IM.Top + 0 * 0.5 / DPIMatrix.M22})
            DG.PushGuidelineSet(gl)
        End If

        ' Draw background
        If Background <> Colors.Transparent Then
            Dim bgRect = SizeRect : bgRect.Offset(-IM.Left, -IM.Right)
            DG.DrawRectangle(New SolidColorBrush(Background), Nothing, bgRect)
        End If

        ' Draw content
        Draw_Internal(DG)

        ' Stop aligning on a pixel grid
        If This.ElementType = MathElement.Type.Glyph Then
            DG.Pop()
        End If

        ' Unshift the drawing zone
        DG.Pop()

    End Sub

    ''' <summary>
    ''' Draw all children on their specified location
    ''' </summary>
    ''' <param name="DG">The surface to draw on</param>
    Protected Sub DrawChildren(DG As DrawingContext)
        For Each G In This.Children

            DG.PushTransform(New TranslateTransform(G.Export.LocationInParent.X, G.Export.LocationInParent.Y))
            'DG.PushTransform(New TranslateTransform(Math.Round(G.Export.LocationInParent.X), Math.Round(G.Export.LocationInParent.Y)))

            Dim ScaleX As Double = G.Export.LocationInParent.Width / G.Export.Width
            Dim ScaleY As Double = G.Export.LocationInParent.Height / G.Export.Height

            If Double.IsNaN(ScaleX) Then ScaleX = 1
            If Double.IsNaN(ScaleY) Then ScaleY = 1

            If ScaleX <> 1 OrElse ScaleY <> 1 Then
                DG.PushTransform(New ScaleTransform(
                    ScaleX,
                    ScaleY
                ))
                G.Export.Draw(DG)
                DG.Pop()
            Else
                G.Export.Draw(DG)
            End If

            DG.Pop()
        Next
    End Sub

    ''' <summary>
    ''' Returns a value indicating whether layout state is "Completed".
    ''' </summary>
    Protected Overridable ReadOnly Property IsLayoutToDate As Boolean
        Get
            Return LayoutCompletion = LayoutCompletionState.Completed
        End Get
    End Property

    ''' <summary>
    ''' Reset layout state to None in order to trigger relayout on next use.
    ''' </summary>
    ''' <param name="ForwardToParent">If True, indicates that the parent element should be reset, too.</param>
    Public Sub InvalidateLayout(Optional ForwardToParent As Boolean = False)

        ' Notify the layout should be recomputed
        LayoutCompletion = LayoutCompletionState.None
        MinABH = Double.NaN
        MinBBH = Double.NaN

        ' Reset layout status
        _LayoutCompletion = LayoutCompletionState.None


        ' Forward to parent element
        If ForwardToParent AndAlso This.ParentElement IsNot Nothing Then
            This.ParentElement.Export.InvalidateLayout(True)
        End If

    End Sub

    ''' <summary>
    ''' Specify whether the element should be rendered using its Block or Inline style
    ''' </summary>
    Public ReadOnly Property LayoutStyle As LayoutModes
        Get
            If This.ParentElement Is Nothing Then

                ' By default, an element should use its prefered layout style
                Return MathEditor.LayoutModes.Block

            Else

                ' Unless there's a restriction inherited from parent
                Return This.ParentElement.Export.LayoutStyleOfChildren

            End If
        End Get
    End Property

    Protected MustOverride ReadOnly Property PreferInlineContent_Internal As Boolean
    Public ReadOnly Property PreferInlineContent As Boolean
        Get
            Return PreferInlineContent_Internal OrElse (This.ParentElement IsNot Nothing AndAlso This.ParentElement.Export.PreferInlineContentFor(This))
        End Get
    End Property

    Public Overridable ReadOnly Property PreferInlineContentFor(Child As MathElement) As Boolean
        Get
            Return PreferInlineContent
        End Get
    End Property

    Public ReadOnly Property LayoutStyleOfChildren As LayoutModes
        Get

            ' Either return current layout style or inline if preferred
            If Me.PreferInlineContent Then Return LayoutModes.Inline _
            Else Return Me.LayoutStyle

        End Get
    End Property

    Protected Sub PerformLayout()
        If (Not IsLayoutToDate) Then

            ' Regenerate layout completely
            GenerateLayout()

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

    Private _LayoutCompletion As LayoutCompletionState = LayoutCompletionState.None
    Public Property LayoutCompletion As LayoutCompletionState
        Get
            Return _LayoutCompletion
        End Get
        Protected Set(value As LayoutCompletionState)
            _LayoutCompletion = value
        End Set
    End Property

    Protected MustOverride Sub CalculateMinHeight_Internal()
    Protected Sub CalculateMinHeight()

        ' Avoid to recompute multiptle time the same thing
        If LayoutCompletion >= LayoutCompletionState.MinHeight Then Exit Sub

        ' Perform the operation and set the state
        CalculateMinHeight_Internal()
        AvailABH = Math.Max(AvailABH, MinABH) : AvailBBH = Math.Max(AvailBBH, MinBBH)
        _LayoutCompletion = LayoutCompletionState.MinHeight

    End Sub


    Protected MinABH As Double = Double.NaN

    ''' <summary>
    ''' Returns the minimum size of this element (above base line). If this element is fenced, it may be higher (but not smaller) than this size at render time.
    ''' </summary>
    Public ReadOnly Property MinimalABH As Double
        Get
            CalculateMinHeight()
            Return MinABH
        End Get
    End Property



    Protected MinBBH As Double = Double.NaN

    ''' <summary>
    ''' Returns the minimum size of this element (below base line). If this element is fenced, it may be higher (but not smaller) than this size at render time.
    ''' </summary>
    Public ReadOnly Property MinimalBBH As Double
        Get
            CalculateMinHeight()
            Return MinBBH
        End Get
    End Property


    ''' <summary>
    ''' Allows the element to invalidate its layout based on available height
    ''' </summary>
    ''' <param name="AvailABH">Available height above baseline</param>
    ''' <param name="AvailBBH">Available height below baseline</param>
    ''' <remarks></remarks>
    Public Sub PrepareLayout(AvailABH As Double, AvailBBH As Double)
        CalculateMinHeight()
        PrepareLayout_Internal(AvailABH, AvailBBH)
        Me.AvailABH = Math.Max(AvailABH, MinABH)
        Me.AvailBBH = Math.Max(AvailBBH, MinBBH)
        _LayoutCompletion = LayoutCompletionState.Prepared
    End Sub

    Protected MustOverride Sub PrepareLayout_Internal(AvailABH As Double, AvailBBH As Double)


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

    ''' <summary>
    ''' Returns the font size (in pixels) that should be used for the rendering
    ''' </summary>
    Public Property FontSize As Double
        Get
            Return This.FontSize
        End Get
        Set(value As Double)
            This.FontSize = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the line' tickness (in pixels) that should be used for the rendering (based on font size)
    ''' </summary>
    ''' <remarks>Should return a value of MathElement.DefaultLineHeight (1px?) at the default font size (MathElement.DefaultFontSize)</remarks>
    Public ReadOnly Property LineHeight As Double
        Get
            Return MathElement.DefaultLineHeight * (FontSize / MathElement.DefaultFontSize)
        End Get
    End Property

    Public Property Foreground As Color
        Get
            Dim X = This.Foreground
            If X.HasValue Then Return X.Value
            Return Colors.Black
        End Get
        Set(value As Color)
            This.Foreground = value
        End Set
    End Property

    Public Property Background As Color
        Get
            Dim X = This.Background
            If X.HasValue Then Return X.Value
            Return Colors.Transparent
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

            ' Special case
            If This.ParentElement Is Nothing Then Return LocationInParent

            ' Recursive case
            This.Root.Export.PerformLayout() ' only for recursivity reasons
            Return FitRect(LocationInParent, This.ParentElement.Export.SizeRect, This.ParentElement.Export.LocationInRoot)

        End Get
    End Property

    Public ReadOnly Property LocationIn(Ancestror As MathElement) As Rect
        Get

            ' Special cases
            If This Is Ancestror Then Return SizeRect
            If This.ParentElement Is Nothing Then Throw New ArgumentException("Ancestror wasn't an ancestror of the element whose LocationIn(Ancestror) has been requested")
            If This.ParentElement Is Ancestror Then Return LocationInParent

            ' Recursive case
            This.Root.Export.PerformLayout() ' only for recursivity reasons
            Return FitRect(LocationInParent, This.ParentElement.Export.SizeRect, This.ParentElement.Export.LocationIn(Ancestror))

        End Get
    End Property

    Public ReadOnly Property SelectionRectInParent() As Rect
        Get

            If This.ParentElement Is Nothing Then
                Return New Rect(SizeRect.X - SM.Left, SizeRect.Y - SM.Top, SizeRect.Width + SM.Right, SizeRect.Height + SM.Bottom)
            Else
                This.ParentElement.Export.PerformLayout()
                Dim L = _LocationInParent : Dim Zw = _LocationInParent.Width / SizeRect.Width : Dim Zh = _LocationInParent.Height / SizeRect.Height
                If Double.IsNaN(Zw) Then Zw = 1 : If Double.IsNaN(Zh) Then Zh = 1
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

    Private AvailABH, AvailBBH As Double
    Public ReadOnly Property AvailableABH As Double
        Get
            If LayoutCompletion <= LayoutCompletionState.MinHeight Then CalculateMinHeight()
            Return AvailABH
        End Get
    End Property
    Public ReadOnly Property AvailableBBH As Double
        Get
            If LayoutCompletion <= LayoutCompletionState.MinHeight Then CalculateMinHeight()
            Return AvailBBH
        End Get
    End Property

    Protected MustOverride Sub GenerateLayout_Internal()
    Public Sub GenerateLayout()
        Select Case LayoutCompletion
            Case LayoutCompletionState.None
None:           ' Start by calculating the minimum height
                CalculateMinHeight()
                GoTo MinHeight
            Case LayoutCompletionState.MinHeight
MinHeight:      ' Continue by preparing the layout
                PrepareLayout(AvailABH, AvailBBH)
                GoTo Prepared
            Case LayoutCompletionState.Prepared
Prepared:       ' Continue by generating the layout
                GenerateLayout_Internal()
                LayoutCompletion = LayoutCompletionState.Generated
                GoTo Generated
            Case LayoutCompletionState.Generated
Generated:      ' Continue by completing the layout
                CompleteLayout()
                GoTo Completed
            Case Else
Completed:      ' Do nothing
        End Select
    End Sub

    Protected Overridable Sub CompleteLayout()
        ' Do nothing. May do some operations and then reperform GenerateLayout in the future.
    End Sub

    Public Function ProposeMoreSpace(ByRef AvailWidth As Double) As Boolean

        ' Perform the real process
        If ProposeMoreSpace_Internal(AvailWidth) Then

            ' Reset the layout to the "prepared" state
            LayoutCompletion = LayoutCompletionState.Prepared
            Return True

        Else : Return False : End If

    End Function

    ''' <summary>
    ''' Adapts your layout if you're given more space to use by the parent
    ''' </summary>
    ''' <param name="AvailWidth">Space available to grow</param>
    ''' <returns>True if the layout changed, False otherwhite</returns>
    Protected Overridable Function ProposeMoreSpace_Internal(ByRef AvailWidth As Double) As Boolean
        Return False
    End Function

    Private Sub This_Changed(sender As Object, e As System.EventArgs) Handles This.Changed
        InvalidateLayout()
    End Sub

End Class

Public Enum LayoutModes

    ''' <summary>The element supports no display mode</summary>
    None = 0

    ''' <summary>The element supports the Inline display mode</summary>
    Inline = 2 ^ 0

    ''' <summary>The element supports the Block display mode</summary>
    Block = 2 ^ 1

    ''' <summary>The element supports both display modes</summary>
    InlineOrBlock = Inline Or Block

    ''' <summary>Mask that can be used</summary>
    MODE_MASK = Inline Or Block

End Enum

Public Enum LayoutCompletionState

    None = 0
    MinHeight = 1
    Prepared = 2
    Generated = 3
    Completed = 4

End Enum