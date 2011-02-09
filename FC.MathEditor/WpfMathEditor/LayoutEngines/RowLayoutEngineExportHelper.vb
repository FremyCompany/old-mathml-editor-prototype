Public Class RowLayoutEngineExportHelper : Inherits ExportHelper

    Public Sub New(ByVal This As MathElement)
        MyBase.New(This)
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

    Public Overrides Sub GenerateLayout()

    End Sub

    Public Overrides Function GetChildLocation(ByVal El As MathElement) As System.Windows.Rect

    End Function

End Class
