Partial Public Class TestGlyph : Inherits MathElement

    Protected Overrides Function Clone_Internal(ByRef ShouldCloneChildren As Boolean) As MathElement
        Return New TestGlyph()
    End Function

    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New GlyphChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New TestGlyphExportHelper(Me)
    End Function

    Protected Overrides Function GetInitialInputHelper() As InputHelper
        Return New EmptyInputHelper(Me)
    End Function

    Public Class TestGlyphExportHelper : Inherits ExportHelper

        Public Sub New(El As MathElement)
            MyBase.New(El)
        End Sub

        Public Overrides Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
            '
        End Sub

        Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)
            '
        End Sub

        Public Overrides Sub AppendMathML(SB As System.Text.StringBuilder)
            '
        End Sub

        Protected Overrides Sub CalculateMinHeight_Internal()
            MinBBH = 20 - FontSize * 0.4
            MinABH = 40 - MinBBH
        End Sub

        Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)
            DG.DrawRectangle(Brushes.Black, Nothing, New Rect(0, 0, InternalWidth, 40))
        End Sub

        Protected Overrides Sub GenerateLayout_Internal()

            W = InternalWidth
            H = 40
            BH = MinBBH
            IM = New Thickness(0, 0, 0, 0)
            OM = New Thickness(0)

            SM = New Thickness(0)

        End Sub

        Protected Overrides ReadOnly Property PreferInlineContent_Internal As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub PrepareLayout_Internal(AvailABH As Double, AvailBBH As Double)
            InternalWidth = 40
        End Sub

        Public Overrides Sub AppendSimpleText(SB As System.Text.StringBuilder)
            SB.Append("#")
        End Sub

        Private InternalWidth As Double = 40
        Protected Overrides Function ProposeMoreSpace_Internal(ByRef AvailWidth As Double) As Boolean
            Me.InternalWidth += AvailWidth : AvailWidth = 0
            Return True
        End Function

    End Class

End Class
