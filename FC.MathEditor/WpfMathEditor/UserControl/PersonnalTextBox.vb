Public Class PersonnalTextBox : Inherits FrameworkElement

    Public Sub New()
        MyBase.New()
        Me.Focusable = True
        Me.Focus()
    End Sub

    'Public Shared TextProperty = DependencyProperty.Register("Text", GetType(String), GetType(PersonnalTextBox))
    'Public Property Text As String
    '    Get
    '        Return GetValue(TextProperty)
    '    End Get
    '    Set(value As String)
    '        SetValue(TextProperty, value) : InvalidateVisual()
    '    End Set
    'End Property

    'Public Shared FontProperty = DependencyProperty.Register("Font", GetType(Typeface), GetType(PersonnalTextBox))
    'Public Property Font As Typeface
    '    Get
    '        Return GetValue(FontProperty)
    '    End Get
    '    Set(value As Typeface)
    '        SetValue(FontProperty, value)
    '    End Set
    'End Property

    Dim F As New Typeface(New FontFamily("Candara"), FontStyles.Italic, FontWeights.Normal, FontStretches.Normal)
    Dim WithEvents X As New MathDocument()

    Protected Overrides Sub OnRender(drawingContext As System.Windows.Media.DrawingContext)
        For Each El In X.Selection
            drawingContext.DrawRectangle(New SolidColorBrush(Color.FromArgb(50, 0, 148, 255)), Nothing, El.Export.SelectionRectInRoot)
        Next
        X.Export.Draw(drawingContext)
    End Sub

    Private Sub PersonnalTextBox_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown

        ' Handle the typed key, if it's a special key
        Select Case e.Key
            Case Key.Left
                If Keyboard.IsKeyDown(Key.LeftShift) OrElse Keyboard.IsKeyDown(Key.RightShift) Then
                    X.Selection.MoveLeft(SelectionHelper.SelectionPointType.EndPoint)
                Else
                    If X.Selection.IsEmpty Then
                        X.Selection.MoveLeft(SelectionHelper.SelectionPointType.Selection)
                    Else
                        X.Selection.CollapseToStart()
                    End If
                End If
            Case Key.Right
                If Keyboard.IsKeyDown(Key.LeftShift) OrElse Keyboard.IsKeyDown(Key.RightShift) Then
                    X.Selection.MoveRight(SelectionHelper.SelectionPointType.EndPoint)
                Else
                    If X.Selection.IsEmpty Then
                        X.Selection.MoveRight(SelectionHelper.SelectionPointType.Selection)
                    Else
                        X.Selection.CollapseToEnd()
                    End If
                End If
            Case Key.Delete
                X.Input.ProcessDelete()
            Case Key.Back
                X.Input.ProcessBackSpace()
            Case Else
                Exit Sub
        End Select

        ' By default, redraw the equation
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
        X.Input.ProcessString(e.Text)
    End Sub

    Private Sub X_Changed(sender As Object, e As System.EventArgs) Handles X.Changed
        Me.InvalidateVisual()
    End Sub

End Class
