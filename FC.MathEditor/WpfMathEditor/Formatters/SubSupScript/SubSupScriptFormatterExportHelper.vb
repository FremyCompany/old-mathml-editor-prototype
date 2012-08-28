Public Class SubSupScriptFormatterExportHelper : Inherits ExportHelper

    Public Sub New(This As SubSupScriptFormatter)
        MyBase.New(This)
    End Sub

    Public Shadows ReadOnly Property This As SubSupScriptFormatter
        Get
            Return MyBase.This
        End Get
    End Property

    Public Overrides Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
        This.Base.Export.AppendKeyboardInput(SB)
        SB.Append("_"c)
        This.SubScript.Export.AppendKeyboardInput(SB)
    End Sub

    Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)
        This.Base.Export.AppendLaTeX(SB)
        SB.Append("_"c)
        This.SubScript.Export.AppendLaTeX(SB)
    End Sub

    Public Overrides Sub AppendMathML(SB As System.Text.StringBuilder)

    End Sub

    Public Overrides Sub AppendSimpleText(SB As System.Text.StringBuilder)
        This.Base.Export.AppendSimpleText(SB)
        SB.Append("_"c)
        This.SubScript.Export.AppendSimpleText(SB)
    End Sub

    Protected Overrides Sub CalculateMinHeight_Internal()

    End Sub

    Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)

    End Sub

    Protected Overrides Sub GenerateLayout_Internal()

    End Sub

    Protected Overrides ReadOnly Property PreferInlineContent_Internal As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property PreferInlineContentFor(Child As MathElement) As Boolean
        Get

            ' Only the scripts have a special behavior
            If Child IsNot This.Base Then Return True

            ' We need to keep inheritance for the base
            Return MyBase.PreferInlineContentFor(Child)

        End Get
    End Property

    Protected Overrides Sub PrepareLayout_Internal(AvailABH As Double, AvailBBH As Double)

    End Sub
End Class
