﻿Partial Public Class UnicodeGlyph

    Public Class UnicodeGlyphExportHelper : Inherits ExportHelper

        Public Sub New(ByVal This As MathElement)
            MyBase.New(This)
        End Sub

        Protected Shadows ReadOnly Property This As UnicodeGlyph
            Get
                Return MyBase.This
            End Get
        End Property

        Public Overrides Sub AppendKeyboardInput(ByVal SB As System.Text.StringBuilder)
            SB.Append(This.DisplayCharacter)
        End Sub

        Public Overrides Sub AppendLaTeX(ByVal SB As System.Text.StringBuilder)
            ' TODO: AppendLaTeX for unicode chars
        End Sub

        Public Overrides Sub AppendMathML(ByVal SB As System.Text.StringBuilder)
            ' TODO: AppendMathML for unicode chars
        End Sub

        Public Overrides Sub Draw(ByVal DG As System.Windows.Media.DrawingContext)
            ' IM is used to modify the drawing zone
            DG.PushTransform(New TranslateTransform(0 * IM.Left, IM.Top))

            ' OLD CODE WHICH IS NOT WORKING PROPERLY BUT HAS MORE BEAUTIFUL OUTPUT
            'Dim FT = New FormattedText(This.C.ToString(), Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, This.Font, This.FontSize, Brushes.Black)
            'DG.PushTransform(New TranslateTransform(0, H - BH - FT.Baseline))
            'DG.DrawText(FT, New Point(0, This.FontSize * This.GlyphFont.Baseline - FT.Baseline))

            DG.DrawGlyphRun(Brushes.Black, This.GlyphRun)
            DG.Pop()
        End Sub

        Public Overrides Sub GenerateLayout()

            Dim TopExtension = If(This.GlyphMargin.Top < 0, -This.GlyphMargin.Top, 0)
            Dim BottomExtension = If(This.GlyphMargin.Bottom < 0, -This.GlyphMargin.Bottom, 0)

            W = This.GlyphAvWidth
            H = This.GlyphHeight + TopExtension + BottomExtension
            BH = This.GlyphFont.DistancesFromHorizontalBaselineToBlackBoxBottom(This.GlyphIndex) * This.FontSize + BottomExtension + TopExtension
            IM = New Thickness(This.GlyphMargin.Left, 0, This.GlyphMargin.Right, 0)
            OM = New Thickness(0)

        End Sub

    End Class

End Class
