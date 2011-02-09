Public Class UnicodeGlyph : Inherits MathElement

    Public Sub New(ByVal C As Char)
        Me.New(C, Nothing)
    End Sub

    Public Sub New(ByVal C As Char, ByVal F As Typeface)
        ' Field initialization
        Me.C = C : Me.Font = F

        ' MathElement properties
        Export = Nothing
        _Children = Nothing
    End Sub

    Private C As Char
    Public ReadOnly Property DisplayCharacter() As Char
        Get
            Return C
        End Get
    End Property

    Public Shared DefaultFont As New Typeface(New FontFamily("Palatino Linotype, Cambria Math"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal)

    Private GlyphFont As GlyphTypeface
    Private GlyphIndex As Integer
    Private GlyphAvWidth As Double
    Private GlyphWidth As Double
    Private GlyphHeight As Double
    Private GlyphMargin As Thickness

    Private F As Typeface
    Public Property Font As Typeface
        Get
            Return If(F, DefaultFont)
        End Get
        Set(ByVal value As Typeface)
            F = value

            Font.TryGetGlyphTypeface(GlyphFont)
            GlyphIndex = GlyphFont.CharacterToGlyphMap(Char.ConvertToUtf32(C.ToString(), 0))
            GlyphAvWidth = GlyphFont.AdvanceWidths(GlyphIndex)

            GlyphMargin = New Thickness(
                GlyphFont.LeftSideBearings(GlyphIndex),
                GlyphFont.TopSideBearings(GlyphIndex),
                GlyphFont.RightSideBearings(GlyphIndex),
                GlyphFont.BottomSideBearings(GlyphIndex)
            )

            GlyphWidth = GlyphAvWidth - GlyphMargin.Left - GlyphMargin.Right
            GlyphHeight = GlyphFont.AdvanceHeights(GlyphIndex) - GlyphMargin.Top - GlyphMargin.Bottom

        End Set
    End Property

    Public Overrides Function Clone() As MathElement
        Return New UnicodeGlyph(C)
    End Function
End Class

Public Class UnicodeGlyphExportHelper : Inherits ExportHelper

    Public Sub New(ByVal This As MathElement)
        MyBase.New(This)
    End Sub

    Public Overrides Sub AppendKeyboardInput(ByVal SB As System.Text.StringBuilder)

    End Sub

    Public Overrides Sub AppendLaTeX(ByVal SB As System.Text.StringBuilder)

    End Sub

    Public Overrides Sub AppendMathML(ByVal SB As System.Text.StringBuilder)

    End Sub

    Public Overrides Sub Draw(ByVal DG As System.Windows.Media.DrawingContext)

    End Sub

    Public Overrides Sub GenerateLayout()

    End Sub

    Public Overrides Function GetChildLocation(ByVal El As MathElement) As System.Windows.Rect

    End Function
End Class