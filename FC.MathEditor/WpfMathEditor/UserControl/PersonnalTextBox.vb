Public Class PersonnalTextBox : Inherits FrameworkElement

    Public Sub New()
        MyBase.New()
        'Me.Text = Char.ConvertFromUtf32(120008) & "afg1" & Char.ConvertFromUtf32(10765) & Char.ConvertFromUtf32(8747) & Char.ConvertFromUtf32(9183)
        'Me.Font = New Typeface(New FontFamily("Cambria Math"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, New FontFamily("Cambria Math, DejaVu Serif, Arial Unicode MS"))
        Me.Focusable = True
        Me.Focus()
    End Sub

    'Public Shared TextProperty = DependencyProperty.Register("Text", GetType(String), GetType(PersonnalTextBox))
    'Public Property Text As String
    '    Get
    '        Return GetValue(TextProperty)
    '    End Get
    '    Set(ByVal value As String)
    '        SetValue(TextProperty, value) : InvalidateVisual()
    '    End Set
    'End Property

    'Public Shared FontProperty = DependencyProperty.Register("Font", GetType(Typeface), GetType(PersonnalTextBox))
    'Public Property Font As Typeface
    '    Get
    '        Return GetValue(FontProperty)
    '    End Get
    '    Set(ByVal value As Typeface)
    '        SetValue(FontProperty, value)
    '    End Set
    'End Property

    Dim X As New RowLayoutEngine(New MathElement() {New UnicodeGlyph("x", Nothing), New UnicodeGlyph("y", Nothing), New UnicodeGlyph("f", Nothing), New UnicodeGlyph(Char.ConvertFromUtf32(8747)(0), Nothing)})
    Protected Overrides Sub OnRender(ByVal drawingContext As System.Windows.Media.DrawingContext)
        X.Export.Draw(drawingContext)

        Dim C = 3
        drawingContext.DrawText(New FormattedText("xyf∫", Globalization.CultureInfo.CurrentCulture, Windows.FlowDirection.LeftToRight, UnicodeGlyph.DefaultFont, C * (14 + 2 / 3), Brushes.Black), New Point(0, 20 * C))

    End Sub

    'Const fontSize = 14 + 2 / 3

    'Dim FT As New FormattedText(Text, Globalization.CultureInfo.CurrentCulture, Windows.FlowDirection.LeftToRight, Font, fontSize, Brushes.Black)

    'Dim GlyphFont As GlyphTypeface = Nothing
    'Font.TryGetGlyphTypeface(GlyphFont)

    'Dim TotalWidth As Double = 0
    'Dim IsShown As Boolean = True
    'For I As Integer = 0 To Text.Length - 1

    '    If IsShown Then
    '        IsShown = False
    '    Else
    '        IsShown = True
    '    End If

    '    Dim Chr = Char.ConvertToUtf32(Text, I) : If Chr > UShort.MaxValue Then I += Chr \ UShort.MaxValue
    '    Dim GlyphIndex = GlyphFont.CharacterToGlyphMap(Chr)

    '    Dim GlyphAvW = fontSize * GlyphFont.AdvanceWidths(GlyphIndex)
    '    Dim GlyphMargin = New Thickness(
    '                        TotalWidth + fontSize * GlyphFont.LeftSideBearings(GlyphIndex),
    '                        fontSize * GlyphFont.TopSideBearings(GlyphIndex),
    '                        fontSize * GlyphFont.RightSideBearings(GlyphIndex),
    '                        fontSize * GlyphFont.BottomSideBearings(GlyphIndex)
    '                        )

    '    Dim GlyphWidth = GlyphAvW + TotalWidth - GlyphMargin.Left - GlyphMargin.Right
    '    Dim GlyphHeight = fontSize * GlyphFont.Height - GlyphMargin.Top - GlyphMargin.Bottom
    '    Dim DeltaH = 5 + FT.Baseline

    '    ''drawingContext.DrawRectangle(
    '    ''    If(IsShown, Brushes.Yellow, Brushes.Aqua),
    '    ''    Nothing,
    '    ''    New Rect(
    '    ''        GlyphMargin.Left,
    '    ''        DeltaH + GlyphMargin.Top,
    '    ''        GlyphWidth,
    '    ''        GlyphHeight
    '    ''    )
    '    '')

    '    TotalWidth += GlyphAvW

    'Next

    'drawingContext.DrawText(FT, New Point(0, 5))

    'End Sub

    'Private SS As UInteger
    'Public Property SelectionStart As UInteger
    '    Get
    '        Return If(SS < Text.Length, SS, Text.Length)
    '    End Get
    '    Set(ByVal value As UInteger)
    '        SS = value
    '    End Set
    'End Property

    'Private SE As UInteger
    'Public Property SelectionEnd As UInteger
    '    Get
    '        Return If(SE < Text.Length, If(SE > SelectionStart, SE, SelectionStart), SelectionStart)
    '    End Get
    '    Set(ByVal value As UInteger)
    '        SE = value
    '    End Set
    'End Property

    'Private Sub PersonnalTextBox_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown

    'End Sub

    'Private Sub PersonnalTextBox_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseDown
    '    Me.Focus()
    'End Sub

    'Private Sub PersonnalTextBox_TextInput(ByVal sender As Object, ByVal e As System.Windows.Input.TextCompositionEventArgs) Handles Me.TextInput
    '    If e.Text = vbBack Then
    '        If Me.Text.Length <> 0 Then
    '            Me.Text = Me.Text.Substring(0, Me.Text.Length - 1)
    '        End If
    '    Else
    '        Me.Text &= e.Text
    '    End If
    'End Sub
End Class
