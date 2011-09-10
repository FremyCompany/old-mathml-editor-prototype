﻿Partial Public Class TestGlyph : Inherits MathElement

    Public Overrides Function Clone_Internal() As MathElement
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
            MinBBH = 0
            MinABH = 40 - MinBBH
        End Sub

        Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)
            DG.DrawRectangle(Brushes.Black, Nothing, New Rect(0, 0, 40, 40))
        End Sub

        Protected Overrides Sub GenerateLayout_Internal()

            W = 40
            H = 40
            BH = MinBBH
            IM = New Thickness(0, 0, 0, 0)
            OM = New Thickness(0)

            SM = New Thickness(0)

        End Sub

        Public Overrides ReadOnly Property PreferInlineContent_Interal As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub PrepareLayout_Internal(AvailABH As Double, AvailBBH As Double)
            '
        End Sub
    End Class

End Class
