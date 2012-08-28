Public Class PersonnalTextBox : Inherits FrameworkElement

    Public Sub New()
        MyBase.New()
        Me.Focusable = True
        Me.Focus()
    End Sub

    Dim F As New Typeface(New FontFamily("Candara"), FontStyles.Italic, FontWeights.Normal, FontStretches.Normal)
    Dim WithEvents X As New MathDocument()

    Protected Overrides Sub OnRender(drawingContext As System.Windows.Media.DrawingContext)
        drawingContext.DrawRectangle(Brushes.White, Nothing, New Rect(RenderSize))

        Dim INTS(CInt(Math.Ceiling(RenderSize.Width))) As Double
        For i As Integer = 0 To INTS.Length - 1
            INTS(i) = CDbl(i)
        Next
        drawingContext.PushGuidelineSet(New GuidelineSet(INTS, INTS))

        drawingContext.PushTransform(New TranslateTransform(3, 3))

        ' TODO: Draw containing bound selection instead (+1px outer margin)
        For Each El In X.Selection
            drawingContext.DrawRectangle(New SolidColorBrush(Color.FromArgb(50, 0, 148, 255)), Nothing, El.Export.SelectionRectInRoot)
        Next

        If X.Selection.PreviousSibling IsNot Nothing Then
            drawingContext.DrawRectangle(New SolidColorBrush(Color.FromArgb(50, 255, 0, 0)), Nothing, X.Selection.PreviousSibling.Export.SelectionRectInRoot)
        End If

        If X.Selection.NextSibling IsNot Nothing Then
            drawingContext.DrawRectangle(New SolidColorBrush(Color.FromArgb(50, 0, 255, 0)), Nothing, X.Selection.NextSibling.Export.SelectionRectInRoot)
        End If

        drawingContext.Pop()

        'To test blur issues
        'RenderOptions.SetEdgeMode(Me, EdgeMode.Aliased)
        drawingContext.PushTransform(New TranslateTransform(3, 3))
        X.Export.Draw(drawingContext)
        drawingContext.Pop()

        drawingContext.Pop() ' end guidelineset
    End Sub

    Private Sub PersonnalTextBox_KeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown

        ' Handle the typed key, if it's a special key
        Select Case e.Key
            Case Key.Left
                X.CurrentInput.ProcessLeftKey(
                    Keyboard.IsKeyDown(Key.LeftCtrl) OrElse Keyboard.IsKeyDown(Key.RightCtrl),
                    Keyboard.IsKeyDown(Key.LeftAlt) OrElse Keyboard.IsKeyDown(Key.RightAlt),
                    Keyboard.IsKeyDown(Key.LeftShift) OrElse Keyboard.IsKeyDown(Key.RightShift)
                )
                'If Keyboard.IsKeyDown(Key.LeftShift) OrElse Keyboard.IsKeyDown(Key.RightShift) Then
                '    X.Selection.MoveLeft(SelectionHelper.SelectionPointType.EndPoint)
                'Else
                '    If X.Selection.IsCollapsed Then
                '        X.Selection.MoveLeft(SelectionHelper.SelectionPointType.Selection)
                '    Else
                '        X.Selection.CollapseToStart()
                '    End If
                'End If
            Case Key.Right
                X.CurrentInput.ProcessRightKey(
                    Keyboard.IsKeyDown(Key.LeftCtrl) OrElse Keyboard.IsKeyDown(Key.RightCtrl),
                    Keyboard.IsKeyDown(Key.LeftAlt) OrElse Keyboard.IsKeyDown(Key.RightAlt),
                    Keyboard.IsKeyDown(Key.LeftShift) OrElse Keyboard.IsKeyDown(Key.RightShift)
                )
                'If Keyboard.IsKeyDown(Key.LeftShift) OrElse Keyboard.IsKeyDown(Key.RightShift) Then
                '    X.Selection.MoveRight(SelectionHelper.SelectionPointType.EndPoint)
                'Else
                '    If X.Selection.IsCollapsed Then
                '        X.Selection.MoveRight(SelectionHelper.SelectionPointType.Selection)
                '    Else
                '        X.Selection.CollapseToEnd()
                '    End If
                'End If
            Case Key.Delete
                X.CurrentInput.ProcessDelete(
                    Keyboard.IsKeyDown(Key.LeftCtrl) OrElse Keyboard.IsKeyDown(Key.RightCtrl),
                    Keyboard.IsKeyDown(Key.LeftAlt) OrElse Keyboard.IsKeyDown(Key.RightAlt),
                    Keyboard.IsKeyDown(Key.LeftShift) OrElse Keyboard.IsKeyDown(Key.RightShift)
                )
            Case Key.Back
                X.CurrentInput.ProcessBackSpace(
                    Keyboard.IsKeyDown(Key.LeftCtrl) OrElse Keyboard.IsKeyDown(Key.RightCtrl),
                    Keyboard.IsKeyDown(Key.LeftAlt) OrElse Keyboard.IsKeyDown(Key.RightAlt),
                    Keyboard.IsKeyDown(Key.LeftShift) OrElse Keyboard.IsKeyDown(Key.RightShift)
                )
            Case Key.D6
                X.Selection.ReplaceContents(New TestGlyph())
            Case Key.D7
                Dim op As OperatorTextEdit = New OperatorTextEdit(New MathElement() {New UnicodeGlyph("∑"c)})
                X.Selection.ReplaceContents(op)
            Case Else
                Exit Sub
        End Select

        ' By default, redraw the equation and handles the event
        e.Handled = True
        Me.InvalidateVisual()

    End Sub

    Private Sub PersonnalTextBox_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        'Dim CHL = New MathElement() {New UnicodeGlyph(AscW("x"), F), New UnicodeGlyph(AscW("y"), F), New UnicodeGlyph(AscW("f"), F), New UnicodeGlyph(AscW(" "), F), New UnicodeGlyph(120002 + 0 * 8747, Nothing), New UnicodeGlyph(AscW("s"), F), New UnicodeGlyph(AscW("i"), F), New UnicodeGlyph(AscW("n"), F), New UnicodeGlyph(AscW(" "), F), New UnicodeGlyph(AscW("x"), Nothing)}

        'Dim CHL As New List(Of MathElement)
        'For X As Double = 0 To 5 Step 0.25
        '    Dim Xo = New UnicodeGlyph("x"c)
        '    DirectCast(Xo.Export, UnicodeGlyph.UnicodeGlyphExportHelper).__DEBUG__AdditionnalABH = X
        '    CHL.Add(Xo)
        'Next

        'Dim EL = New IdentifierTextEdit(CHL)

        'Dim Den = New RowLayoutEngine(New MathElement() {EL})
        'Dim Num As New RowLayoutEngine(New MathElement() {New TestGlyph()})

        'X.AddChild(New FractionFormatter(Num, Den))
        Try
            X.AddChild(
                New RootFormatter(
                    New RowLayoutEngine(
                        New MathElement() {
                            New UnicodeGlyph(AscW("x"))
                        }
                    ),
                    New RowLayoutEngine(
                        New MathElement() {
                            New UnicodeGlyph(Asc("3"))
                        }
                    )
                )
            )
        Catch ex As Exception

            Console.WriteLine(ex.ToString())

        End Try

        Me.InvalidateVisual()

        Me.Focus() : Keyboard.Focus(Me)

    End Sub

    Private IsMouseDown As Boolean = False
    Private Sub PersonnalTextBox_MouseDown(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseDown

        ' Give focus to the text box
        Me.Focus() : Keyboard.Focus(Me)

        ' Activate selection mode
        IsMouseDown = True

        ' Move the selection point to clicked location
        Dim HitResult = X.GetSelectionPointFromRelativePoint(e.GetPosition(Me) - New Vector(3, 3))
        X.Selection.SetPoint(HitResult)

    End Sub

    Private Sub PersonnalTextBox_MouseMove(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles Me.MouseMove

        If IsMouseDown Then

            ' Update the selection end point based on mouse position
            Dim HitResult = X.GetSelectionPointFromRelativePoint(e.GetPosition(Me) - New Vector(3, 3))
            X.Selection.SetPoint(HitResult, SelectionHelper.SelectionPointType.EndPoint)


        End If

    End Sub

    Private Sub PersonnalTextBox_MouseUp(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseUp

        ' Update the selection end point based on mouse position
        Dim HitResult = X.GetSelectionPointFromRelativePoint(e.GetPosition(Me) - New Vector(3, 3))
        X.Selection.SetPoint(HitResult, SelectionHelper.SelectionPointType.EndPoint)

        ' Deactivate selection mode
        IsMouseDown = False

    End Sub

    Private Sub PersonnalTextBox_TextInput(sender As Object, e As System.Windows.Input.TextCompositionEventArgs) Handles Me.TextInput
        X.CurrentInput.ProcessString(e.Text)
    End Sub

    Private Sub X_Changed(sender As Object, e As System.EventArgs) Handles X.Changed
        Me.InvalidateVisual()
    End Sub

    Private Sub X_SelectionChanged(sender As Object, e As System.EventArgs) Handles X.SelectionChanged
        Me.InvalidateVisual()
    End Sub
End Class
