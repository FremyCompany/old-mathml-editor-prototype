﻿Partial Public Class UnicodeGlyph : Inherits MathElement

    Public Sub New(ByVal C As Char)
        Me.New(C, Nothing)
    End Sub

    Public Sub New(ByVal C As Char, ByVal F As Typeface, Optional ByVal S As Double = 14.5)
        ' Field initialization
        Me.C = C : Me.F = F : Me.S = S

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

    Private GlyphRun As GlyphRun

    Private F As Typeface, S As Double
    Public ReadOnly Property Font As Typeface
        Get
            Return If(F, DefaultFont)
        End Get
    End Property

    Public Sub GenerateData()
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
    End Sub

    Public Overrides Function Clone() As MathElement
        Return New UnicodeGlyph(C, F, S)
    End Function
End Class