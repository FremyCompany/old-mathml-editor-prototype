Public Class RowLayoutEngineExportHelper : Inherits ExportHelper

    Protected This As MathElement
    Public Sub New(ByVal This As MathElement)
        Me.This = This
    End Sub

    Public Overrides Sub AppendKeyboardInput(ByVal SB As System.Text.StringBuilder)
        SB.Append("(")
        For Each Child In This.Children
            Child.Export.AppendKeyboardInput(SB)
        Next
        SB.Append(")")
    End Sub

    Public Overrides Sub AppendLaTeX(ByVal SB As System.Text.StringBuilder)
        SB.Append("{")
        For Each Child In This.Children
            Child.Export.AppendLaTeX(SB)
        Next
        SB.Append("}")
    End Sub

    Public Overrides Sub AppendMathML(ByVal SB As System.Text.StringBuilder)
        ' TODO: Implement MathML for "mrow" elements
    End Sub

    Public Overrides Sub Draw(ByVal DG As System.Windows.Media.DrawingContext)
        ' TODO: Draw element list
    End Sub

End Class
