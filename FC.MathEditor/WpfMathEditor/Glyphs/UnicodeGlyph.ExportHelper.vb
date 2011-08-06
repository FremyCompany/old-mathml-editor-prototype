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

        Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)

            DG.PushTransform(New ScaleTransform(1, 2))
            DG.DrawGlyphRun(New SolidColorBrush(Foreground), GlyphRun)
            DG.Pop()

        End Sub

        Private GlyphFont As GlyphTypeface
        Private GlyphIndex As Integer
        Private GlyphAvWidth As Double
        Private GlyphAvHeight As Double
        Private GlyphWidth As Double
        Private GlyphHeight As Double
        Private GlyphMargin As Thickness

        Private GlyphRun As GlyphRun


        Public Overrides Sub GenerateLayout()

            Dim S = FontSize

            ' Find the first typeface (fallback, if needed) that contains a valid glyph for the current char
            Font.TryGetGlyphTypeface(GlyphFont)
            If Not GlyphFont.CharacterToGlyphMap.ContainsKey(This.C) Then
                Call New Typeface(This.DefaultMathFontFamily, This.FontStyle, This.FontWeight, This.FontStretch).TryGetGlyphTypeface(GlyphFont)
                If Not GlyphFont.CharacterToGlyphMap.ContainsKey(This.C) Then
                    Call New Typeface(This.DefaultMathFontFamily2, This.FontStyle, This.FontWeight, This.FontStretch).TryGetGlyphTypeface(GlyphFont)
                    If Not GlyphFont.CharacterToGlyphMap.ContainsKey(This.C) Then
                        ' The last font we use is Arial. This font is known for implementing many MS glyphs
                        Call New Typeface(New FontFamily("Arial"), This.FontStyle, This.FontWeight, This.FontStretch).TryGetGlyphTypeface(GlyphFont)
                        If Not GlyphFont.CharacterToGlyphMap.ContainsKey(This.C) Then
                            Font.TryGetGlyphTypeface(GlyphFont)
                            GlyphIndex = 0 : GoTo SkipGlyphIndexMapping
                        End If
                    End If
                End If
            End If

            GlyphIndex = GlyphFont.CharacterToGlyphMap(This.C)
SkipGlyphIndexMapping:
            GlyphAvWidth = S * GlyphFont.AdvanceWidths(GlyphIndex)
            GlyphAvHeight = S * GlyphFont.AdvanceHeights(GlyphIndex)

            GlyphMargin = New Thickness(
                S * GlyphFont.LeftSideBearings(GlyphIndex),
                S * GlyphFont.TopSideBearings(GlyphIndex),
                S * GlyphFont.RightSideBearings(GlyphIndex),
                S * GlyphFont.BottomSideBearings(GlyphIndex)
            )

            GlyphWidth = GlyphAvWidth - GlyphMargin.Left - GlyphMargin.Right
            GlyphHeight = GlyphAvHeight - GlyphMargin.Top - GlyphMargin.Bottom

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

            SM = New Thickness(0, GlyphFont.Baseline * This.FontSize - GlyphHeight + BH, 0, (Font.FontFamily.LineSpacing - GlyphFont.Baseline) * This.FontSize - BH)


        End Sub

        Protected Overrides Function GetMinABH() As Double
            Return Me.AboveBaseLineHeight
        End Function

        Protected Overrides Function GetMinBBH() As Double
            Return Me.BelowBaseLineHeight
        End Function

        Private AvailABH, AvailBBH As Double
        Public Overrides Sub PrepareLayout(AvailABH As Double, AvailBBH As Double)
            If (Me.AvailABH <> AvailABH) OrElse (Me.AvailBBH <> AvailBBH) Then
                Me.AvailABH = AvailABH : Me.AvailBBH = AvailBBH
                If (This.ParentElement Is Nothing) OrElse This.ParentElement.Export.IsFence Then

                End If
            End If
        End Sub

    End Class

End Class
