Partial Public Class UnicodeGlyph

    Public Class UnicodeGlyphExportHelper : Inherits ExportHelper

        '++
        '++ Code
        '++

        Public Sub New(This As MathElement)
            MyBase.New(This)
        End Sub

        Protected Shadows ReadOnly Property This As UnicodeGlyph
            Get
                Return MyBase.This
            End Get
        End Property

        Public Overrides Sub AppendSimpleText(SB As System.Text.StringBuilder)
            SB.Append(This.DisplayChar)
        End Sub

        Public Overrides Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
            SB.Append(This.DisplayChar)
        End Sub

        Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)
            ' TODO: AppendLaTeX for unicode chars
        End Sub

        Public Overrides Sub AppendMathML(SB As System.Text.StringBuilder)
            ' TODO: AppendMathML for unicode chars
        End Sub

        Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)

            If IsSymmetric Then
                DG.DrawGlyphRun(New SolidColorBrush(Foreground), GlyphRun)
            Else
                DG.PushTransform(New ScaleTransform(1, StretchRelative, 0, 0))
                DG.DrawGlyphRun(New SolidColorBrush(Foreground), GlyphRun)
                DG.Pop()
            End If

        End Sub

        Private GlyphFont As GlyphTypeface
        Private GlyphIndex As Integer = 0
        Private GlyphAvWidth As Double
        Private GlyphAvHeight As Double
        Private GlyphWidth As Double
        Private GlyphHeight As Double
        Private GlyphMargin As Thickness

        Private GlyphRun As GlyphRun

        Protected Overrides Sub CalculateMinHeight_Internal()
            Dim S = FontSize

            ' Get glyph from char
            GetGlyph()

            Dim GlyphAvHeight = S * GlyphFont.AdvanceHeights(GlyphIndex)

            Dim GlyphMargin = New Thickness(
                S * GlyphFont.LeftSideBearings(GlyphIndex),
                S * GlyphFont.TopSideBearings(GlyphIndex),
                S * GlyphFont.RightSideBearings(GlyphIndex),
                S * GlyphFont.BottomSideBearings(GlyphIndex)
            )

            Dim GlyphHeight = GlyphAvHeight - GlyphMargin.Top - GlyphMargin.Bottom

            Dim TopExtension = If(GlyphMargin.Top < 0, -GlyphMargin.Top, 0)
            Dim BottomExtension = If(GlyphMargin.Bottom < 0, -GlyphMargin.Bottom, 0)

            Dim H = GlyphHeight + TopExtension + BottomExtension
            Dim BH = GlyphFont.DistancesFromHorizontalBaselineToBlackBoxBottom(GlyphIndex) * S + BottomExtension + TopExtension

            MinABH = Math.Max(0, H - BH)
            MinBBH = Math.Max(0, BH)

        End Sub

        '' DEBUG CODE
        Public __DEBUG__AdditionnalABH As Double

        Protected Overrides Sub GenerateLayout_Internal()
            Dim S = FontSize * FontSizeRelative

            ' Compute char metrics
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
            H = GlyphHeight + TopExtension + BottomExtension + __DEBUG__AdditionnalABH
            BH = GlyphFont.DistancesFromHorizontalBaselineToBlackBoxBottom(GlyphIndex) * S + BottomExtension + TopExtension

            ' Compute the position of the baseline
            If IsSymmetric Then

                ' Inner margin (blackbox vs charbox)
                IM = New Thickness(GlyphMargin.Left, __DEBUG__AdditionnalABH, GlyphMargin.Right, 0)

                ' Outer margin (none by default)
                OM = New Thickness(0)

                ' Selection margin (linebox vs charbox)
                SM = New Thickness(0, GlyphFont.Baseline * S - GlyphHeight + BH, 0, (Font.FontFamily.LineSpacing - GlyphFont.Baseline) * S - BH)

            Else

                StretchRelative = (AvailableABH + AvailableBBH) / H
                H = AvailableABH + AvailableBBH : BH = AvailableBBH

                ' Inner margin (blackbox vs charbox)
                IM = New Thickness(GlyphMargin.Left, 0, GlyphMargin.Right, 0)

                ' Outer margin (none by default)
                OM = New Thickness(0)

                ' Selection margin (only charbox)
                SM = New Thickness(0, 0, 0, 0)

            End If

        End Sub

        Dim IsStretchy As Boolean = False
        Dim IsSymmetric As Boolean = False
        Private FontSizeRelative, StretchRelative As Double
        Protected Overrides Sub PrepareLayout_Internal(AvailABH As Double, AvailBBH As Double)


            ' If the char is stretchy
            If (This.ParentElement IsNot Nothing) AndAlso
                (TypeOf This.ParentElement Is OperatorTextEdit) AndAlso
                DirectCast(This.ParentElement, OperatorTextEdit).IsStretchy Then

                ' If the char should grow symetrically
                If DirectCast(This.ParentElement, OperatorTextEdit).IsSymmetric Then


                    ' Symetric stretchy layout
                    PrepareLayout_SymStretchy(AvailABH, AvailBBH)

                Else

                    ' Asymetric stretchy layout
                    PrepareLayout_AsymStretchy(AvailABH, AvailBBH)

                End If

            Else


                ' Classic layout
                PrepareLayout_Classic()

            End If

        End Sub

        Private _LastFont As Typeface
        Private Sub GetGlyph()

            ' Check if font has changed since last execution
            If Font IsNot _LastFont Then

                ' Save state from next time
                _LastFont = Font

                ' Find the first typeface (fallback, if needed) that contains a valid glyph for the current char
                Font.TryGetGlyphTypeface(GlyphFont)
                If (GlyphFont Is Nothing) OrElse (Not GlyphFont.CharacterToGlyphMap.ContainsKey(This.C)) Then

                    ' Walk fonts for the specified Font Type
                    For Each xFontFamily In DefaultFonts(This.FontType)
                        Call New Typeface(xFontFamily, This.FontStyle, This.FontWeight, FontStretches.Normal).TryGetGlyphTypeface(GlyphFont)
                        If (GlyphFont Is Nothing) OrElse (GlyphFont.CharacterToGlyphMap.ContainsKey(This.C)) Then
                            Continue For
                        Else
                            Exit For
                        End If
                    Next

                    ' Fallback to null char if not font has been found
                    If (GlyphFont Is Nothing) OrElse (GlyphFont.CharacterToGlyphMap.ContainsKey(This.C)) Then
                        Font.TryGetGlyphTypeface(GlyphFont)
                        GlyphIndex = 0 : Exit Sub
                    End If
                End If

                GlyphIndex = GlyphFont.CharacterToGlyphMap(This.C)

            End If

        End Sub

        Protected Overrides ReadOnly Property PreferInlineContent_Internal As Boolean
            Get
                Return False
            End Get
        End Property

        Private Sub PrepareLayout_Classic()
            FontSizeRelative = 1
            IsStretchy = False
            IsSymmetric = True
        End Sub

        Private Sub PrepareLayout_SymStretchy(AvailABH As Double, AvailBBH As Double)

            ' (1/1+1/2) will look like that :

            '   (            )
            '  (       1      )
            ' (================)
            '  (    1 +  1    )
            '   (        2   )

            FontSizeRelative = Math.Max(1, Math.Max(AvailABH / MinimalABH, AvailBBH / MinimalBBH))
            IsStretchy = True
            IsSymmetric = True

        End Sub

        Private Sub PrepareLayout_AsymStretchy(ByVal AvailABH As Double, ByVal AvailBBH As Double)


            ' (1/1+1/2) will look like that :

            '  (       1      )
            ' (================)
            ' (     1 +  1     )
            '  (         2    )

            FontSizeRelative = Math.Max(1, Math.Min(AvailABH / MinimalABH, AvailBBH / MinimalBBH))
            IsStretchy = True
            IsSymmetric = False
        End Sub

    End Class

End Class
