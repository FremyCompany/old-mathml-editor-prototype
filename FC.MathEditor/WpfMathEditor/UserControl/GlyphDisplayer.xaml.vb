Public Class GlyphDisplayer

    Private Sub GlyphDisplayer_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Dim TotalWidth As Double = 0
        Const fontSize = 14 + 2 / 3

        Try
            Dim Font = New Typeface(New FontFamily("Cambria Math"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, New FontFamily("Arial Unicode MS"))
            Dim GlyphFont As GlyphTypeface = Nothing : Font.TryGetGlyphTypeface(GlyphFont)
            Dim GlyphIndex = GlyphFont.CharacterToGlyphMap(AscW("g"c))
            Dim GlyphAvW = fontSize * GlyphFont.AdvanceWidths(GlyphIndex)
            Dim GlyphMargin = New Thickness(TotalWidth + fontSize * GlyphFont.LeftSideBearings(GlyphIndex), fontSize * GlyphFont.TopSideBearings(GlyphIndex), fontSize * GlyphFont.RightSideBearings(GlyphIndex), fontSize * GlyphFont.BottomSideBearings(GlyphIndex))
            Dim Glyph = New GlyphRun(GlyphFont, 0, False, fontSize, New List(Of UShort) From {GlyphIndex}, New Point(0, fontSize * GlyphFont.Baseline), New List(Of Double) From {GlyphAvW}, Nothing, "g".ToCharArray(), Nothing, Nothing, Nothing, Nothing)
            Glyph.BuildGeometry()

            Dim R As New Image() With {.Stretch = Stretch.None}
            R.Margin = GlyphMargin
            R.Source = New DrawingImage(New GeometryDrawing(Brushes.Black, New Pen(Brushes.Black, 0), Glyph.BuildGeometry)) 'New GlyphRunDrawing(Brushes.Black, Glyph))
            R.Width = GlyphAvW + TotalWidth - GlyphMargin.Left - GlyphMargin.Right
            R.Height = fontSize * GlyphFont.Height - GlyphMargin.Top - GlyphMargin.Bottom
            'R.SnapsToDevicePixels = True
            'R.UseLayoutRounding = True
            RenderOptions.SetBitmapScalingMode(R, BitmapScalingMode.HighQuality)
            Me.LayoutRoot.Children.Add(R)

            TotalWidth += GlyphAvW
        Catch ex As Exception

            Debugger.Break()

        End Try

        Try
            Dim Font = New Typeface(New FontFamily("Cambria Math"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, New FontFamily("Arial Unicode MS"))
            Dim GlyphFont As GlyphTypeface = Nothing : Font.TryGetGlyphTypeface(GlyphFont)
            Dim GlyphIndex = GlyphFont.CharacterToGlyphMap(AscW("f"c))
            Dim GlyphAvW = fontSize * GlyphFont.AdvanceWidths(GlyphIndex)
            Dim GlyphMargin = New Thickness(TotalWidth + fontSize * GlyphFont.LeftSideBearings(GlyphIndex), fontSize * GlyphFont.TopSideBearings(GlyphIndex), fontSize * GlyphFont.RightSideBearings(GlyphIndex), fontSize * GlyphFont.BottomSideBearings(GlyphIndex))
            Dim Glyph = New GlyphRun(GlyphFont, 0, False, fontSize, New List(Of UShort) From {GlyphIndex}, New Point(0, fontSize * GlyphFont.Baseline), New List(Of Double) From {GlyphAvW}, Nothing, "g".ToCharArray(), Nothing, Nothing, Nothing, Nothing)
            Glyph.BuildGeometry()

            Dim R As New Image() With {.Stretch = Stretch.None}
            R.Margin = GlyphMargin
            R.Source = New DrawingImage(New GeometryDrawing(Brushes.Black, New Pen(Brushes.Black, 0), Glyph.BuildGeometry)) 'New GlyphRunDrawing(Brushes.Black, Glyph))
            R.Width = GlyphAvW + TotalWidth - GlyphMargin.Left - GlyphMargin.Right
            R.Height = fontSize * GlyphFont.Height - GlyphMargin.Top - GlyphMargin.Bottom
            'R.SnapsToDevicePixels = True
            'R.UseLayoutRounding = True
            RenderOptions.SetBitmapScalingMode(R, BitmapScalingMode.HighQuality)
            Me.LayoutRoot.Children.Add(R)

            TotalWidth += GlyphAvW
        Catch ex As Exception

            Debugger.Break()

        End Try

        For i = 1 To 5
            Try
            Dim Font = New Typeface(New FontFamily("Cambria Math"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, New FontFamily("Arial Unicode MS"))
                Dim GlyphFont As GlyphTypeface = Nothing : Font.TryGetGlyphTypeface(GlyphFont)
                Dim GlyphIndex = GlyphFont.CharacterToGlyphMap(AscW("."c))
                Dim GlyphAvW = fontSize * GlyphFont.AdvanceWidths(GlyphIndex)
                Dim GlyphMargin = New Thickness(TotalWidth + fontSize * GlyphFont.LeftSideBearings(GlyphIndex), fontSize * GlyphFont.TopSideBearings(GlyphIndex), fontSize * GlyphFont.RightSideBearings(GlyphIndex), fontSize * GlyphFont.BottomSideBearings(GlyphIndex))
                Dim Glyph = New GlyphRun(GlyphFont, 0, False, fontSize, New List(Of UShort) From {GlyphIndex}, New Point(0, fontSize * GlyphFont.Baseline), New List(Of Double) From {GlyphAvW}, Nothing, "g".ToCharArray(), Nothing, Nothing, Nothing, Nothing)
                Glyph.BuildGeometry()

                Dim R As New Image() With {.Stretch = Stretch.None}
                R.Margin = GlyphMargin
                R.Source = New DrawingImage(New GeometryDrawing(Brushes.Black, New Pen(Brushes.Black, 0), Glyph.BuildGeometry)) 'New GlyphRunDrawing(Brushes.Black, Glyph))
                R.Width = GlyphAvW + TotalWidth - GlyphMargin.Left - GlyphMargin.Right
                R.Height = fontSize * GlyphFont.Height - GlyphMargin.Top - GlyphMargin.Bottom
                'R.SnapsToDevicePixels = True
                'R.UseLayoutRounding = True
                RenderOptions.SetBitmapScalingMode(R, BitmapScalingMode.HighQuality)
                Me.LayoutRoot.Children.Add(R)

                TotalWidth += GlyphAvW
            Catch ex As Exception

                Debugger.Break()

            End Try
        Next

        Try
            Dim FT As New FormattedText("gf.....ℝ∀ x ∈ ℂ ∧ ← ", Globalization.CultureInfo.CurrentCulture, Windows.FlowDirection.LeftToRight, New Typeface(New FontFamily("Palatino Linotype"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, New FontFamily("Cambria Math")), 16, Brushes.Black)
            Dim DV As New DrawingVisual()
            Dim DG = DV.RenderOpen()
            DG.DrawText(FT, New Point(0, 0))
            DG.Close()

            Dim I As New Image() With {.Stretch = Stretch.None}
            I.Source = New DrawingImage(DV.Drawing)
            I.Margin = New Thickness(0, 70, 0, 0)
            Me.LayoutRoot.Children.Add(I)
        Catch ex As Exception
        End Try

        Try
            Dim FT As New FormattedText("gf.....ℝ∀ x ∈ ℂ ∧ ← ", Globalization.CultureInfo.CurrentCulture, Windows.FlowDirection.LeftToRight, New Typeface(New FontFamily("Palatino Linotype"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, New FontFamily("Asana Math")), 16, Brushes.Black)
            Dim DV As New DrawingVisual()
            Dim DG = DV.RenderOpen()
            DG.DrawText(FT, New Point(0, 0))
            DG.Close()

            Dim I As New Image() With {.Stretch = Stretch.None}
            I.Source = New DrawingImage(DV.Drawing)
            I.Margin = New Thickness(0, 100, 0, 0)
            Me.LayoutRoot.Children.Add(I)
        Catch ex As Exception
        End Try

        Try
            Dim FT As New FormattedText("gf.....ℝ∀ x ∈ ℂ ∧ ← ", Globalization.CultureInfo.CurrentCulture, Windows.FlowDirection.LeftToRight, New Typeface(New FontFamily("Palatino Linotype"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, New FontFamily("DejaVu Serif")), 16, Brushes.Black)
            Dim DV As New DrawingVisual()
            Dim DG = DV.RenderOpen()
            DG.DrawText(FT, New Point(0, 0))
            DG.Close()

            Dim I As New Image() With {.Stretch = Stretch.None}
            I.Source = New DrawingImage(DV.Drawing)
            I.Margin = New Thickness(0, 130, 0, 0)
            Me.LayoutRoot.Children.Add(I)
        Catch ex As Exception
        End Try


    End Sub

End Class
