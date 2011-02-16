Partial Public Class UnicodeGlyph : Inherits MathElement

    Public Sub New(ByVal C As Char)
        Me.New(C, Nothing, Nothing)
    End Sub

    Public Sub New(ByVal C As Char, ByVal F As Typeface)
        Me.New(C, F, Nothing)
    End Sub

    Public Sub New(ByVal C As Char, ByVal F As Typeface, ByVal S As Double?)
        ' Field initialization
        Me.C = C : Me.Font = F : Me.FontSize = S

        ' MathElement properties
        Export = New UnicodeGlyphExportHelper(Me)
        _Children = New GlyphChildrenHelper(Me)

        ' End init
        GenerateData()
    End Sub

    Private C As Char
    Public ReadOnly Property DisplayCharacter() As Char
        Get
            Return C
        End Get
    End Property

    Private GlyphFont As GlyphTypeface
    Private GlyphIndex As Integer
    Private GlyphAvWidth As Double
    Private GlyphWidth As Double
    Private GlyphHeight As Double
    Private GlyphMargin As Thickness

    Private GlyphRun As GlyphRun

    Public Sub GenerateData()

        Dim S = FontSize

        Font.TryGetGlyphTypeface(GlyphFont)
        GlyphIndex = GlyphFont.CharacterToGlyphMap(Char.ConvertToUtf32(C.ToString(), 0))
        GlyphAvWidth = S * GlyphFont.AdvanceWidths(GlyphIndex)

        GlyphMargin = New Thickness(
            S * GlyphFont.LeftSideBearings(GlyphIndex),
            S * GlyphFont.TopSideBearings(GlyphIndex),
            S * GlyphFont.RightSideBearings(GlyphIndex),
            S * GlyphFont.BottomSideBearings(GlyphIndex)
        )

        GlyphWidth = GlyphAvWidth - GlyphMargin.Left - GlyphMargin.Right
        GlyphHeight = S * GlyphFont.AdvanceHeights(GlyphIndex) - GlyphMargin.Top - GlyphMargin.Bottom

        GlyphRun = New GlyphRun(
            GlyphFont, 0, False, S,
            New UShort() {GlyphIndex},
            New Point(0, GlyphHeight - S * GlyphFont.DistancesFromHorizontalBaselineToBlackBoxBottom(GlyphIndex)),
            New Double() {GlyphAvWidth},
            Nothing, Nothing, Nothing,
            Nothing, Nothing, Nothing
        )

    End Sub

    Public Overrides Function Clone_Internal() As MathElement
        Return New UnicodeGlyph(C)
    End Function

    Public Overrides Function ToString() As String
        Return C
    End Function

End Class