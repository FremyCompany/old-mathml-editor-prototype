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
        For Each El In X.Selection
            drawingContext.DrawRectangle(New SolidColorBrush(Color.FromArgb(50, 0, 148, 255)), Nothing, El.Export.SelectionRectInRoot)
        Next

        If X.Selection.PreviousSibling IsNot Nothing Then
            drawingContext.DrawRectangle(New SolidColorBrush(Color.FromArgb(50, 255, 0, 0)), Nothing, X.Selection.PreviousSibling.Export.SelectionRectInRoot)
        End If

        If X.Selection.NextSibling IsNot Nothing Then
            drawingContext.DrawRectangle(New SolidColorBrush(Color.FromArgb(50, 0, 255, 0)), Nothing, X.Selection.NextSibling.Export.SelectionRectInRoot)
        End If

        X.Export.Draw(drawingContext)
    End Sub

    Private Sub PersonnalTextBox_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown

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
        'X.AddChild(New IdentifierTextEdit(CHL))
        'Me.InvalidateVisual()
        Me.Focus() : Keyboard.Focus(Me)
    End Sub

    Private Sub PersonnalTextBox_MouseDown(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseDown
        Me.Focus() : Keyboard.Focus(Me)
    End Sub

    Private Sub PersonnalTextBox_TextInput(sender As Object, e As System.Windows.Input.TextCompositionEventArgs) Handles Me.TextInput
        X.CurrentInput.ProcessString(e.Text)
    End Sub

    Private Sub X_Changed(sender As Object, e As System.EventArgs) Handles X.Changed
        Me.InvalidateVisual()
    End Sub

End Class
