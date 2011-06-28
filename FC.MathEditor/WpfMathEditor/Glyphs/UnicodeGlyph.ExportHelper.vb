Partial Public Class UnicodeGlyph

    Public Class UnicodeGlyphExportHelper : Inherits ExportHelper

        Public Sub New(This As MathElement)
            MyBase.New(This)
        End Sub

        Protected Shadows ReadOnly Property This As UnicodeGlyph
            Get
                Return MyBase.This
            End Get
        End Property

        Public Overrides Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
            SB.Append(This.DisplayCharacter)
        End Sub

        Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)
            ' TODO: AppendLaTeX for unicode chars
        End Sub

        Public Overrides Sub AppendMathML(SB As System.Text.StringBuilder)
            ' TODO: AppendMathML for unicode chars
        End Sub

        Public Overrides Sub Draw(DG As System.Windows.Media.DrawingContext)
            ' IM is used to modify the drawing zone
            DG.PushTransform(New TranslateTransform(0 * IM.Left, IM.Top))

            ' OLD CODE WHICH IS NOT WORKING PROPERLY BUT HAS MORE BEAUTIFUL OUTPUT
            'Dim FT = New FormattedText(This.C.ToString(), Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, This.Font, This.FontSize, Brushes.Black)
            'DG.PushTransform(New TranslateTransform(0, H - BH - FT.Baseline))
            'DG.DrawText(FT, New Point(0, This.FontSize * GlyphFont.Baseline - FT.Baseline))

            DG.DrawGlyphRun(Brushes.Black, GlyphRun)
            DG.Pop()
        End Sub

        Private GlyphFont As GlyphTypeface
        Private GlyphIndex As Integer
        Private GlyphAvWidth As Double
        Private GlyphWidth As Double
        Private GlyphHeight As Double
        Private GlyphMargin As Thickness

        Private GlyphRun As GlyphRun


        Public Overrides Sub GenerateLayout()

            Dim S = FontSize

            ' Find the first (fallback, if needed) typeface that contains a valid glyph for the current char
            Font.TryGetGlyphTypeface(GlyphFont)
            If Not GlyphFont.CharacterToGlyphMap.ContainsKey(This.C) Then
                Call New Typeface(This.DefaultMathFontFamily, This.FontStyle, This.FontWeight, This.FontStretch).TryGetGlyphTypeface(GlyphFont)
            End If

            GlyphIndex = GlyphFont.CharacterToGlyphMap(This.C)
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

            Dim TopExtension = If(GlyphMargin.Top < 0, -GlyphMargin.Top, 0)
            Dim BottomExtension = If(GlyphMargin.Bottom < 0, -GlyphMargin.Bottom, 0)

            W = GlyphAvWidth
            H = GlyphHeight + TopExtension + BottomExtension
            BH = GlyphFont.DistancesFromHorizontalBaselineToBlackBoxBottom(GlyphIndex) * This.FontSize + BottomExtension + TopExtension
            IM = New Thickness(GlyphMargin.Left, 0, GlyphMargin.Right, 0)
            OM = New Thickness(0)

        End Sub

    End Class

End Class
