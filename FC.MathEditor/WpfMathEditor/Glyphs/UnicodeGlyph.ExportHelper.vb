Partial Public Class UnicodeGlyph

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
            ' TODO
        End Sub

        Public Overrides Sub Draw(ByVal DG As System.Windows.Media.DrawingContext)
            DG.DrawGlyphRun(Brushes.Black, This.GlyphRun)
        End Sub

        Public Overrides Sub GenerateLayout()
            ' Nothing to do, everything is already computed
        End Sub

        Public Overrides Function GetChildLocation(ByVal El As MathElement) As System.Windows.Rect
            Throw New InvalidOperationException()
        End Function
    End Class

End Class
