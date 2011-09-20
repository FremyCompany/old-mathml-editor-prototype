Public Class FractionFormatterExportHelper : Inherits ExportHelper

    Public Sub New(This As FractionFormatter)
        MyBase.New(This)
    End Sub

    Protected Shadows ReadOnly Property This As FractionFormatter
        Get
            Return MyBase.This
        End Get
    End Property

    Public Overrides Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
        ' TODO
    End Sub

    Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)
        ' TODO
    End Sub

    Public Overrides Sub AppendMathML(SB As System.Text.StringBuilder)
        ' TODO
    End Sub

    Public Overrides Sub AppendSimpleText(SB As System.Text.StringBuilder)
        SB.Append("("c)
        This.Numerator.Export.AppendSimpleText(SB)
        SB.Append(")/(")
        This.Denominator.Export.AppendSimpleText(SB)
        SB.Append(")")
    End Sub

    Protected Overrides Sub CalculateMinHeight_Internal()

    End Sub

    Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)

    End Sub

    Protected Overrides Sub GenerateLayout_Internal()

    End Sub

    Public Overrides ReadOnly Property PreferInlineContent_Interal As Boolean
        Get

        End Get
    End Property

    Protected Overrides Sub PrepareLayout_Internal(AvailABH As Double, AvailBBH As Double)

    End Sub
End Class
