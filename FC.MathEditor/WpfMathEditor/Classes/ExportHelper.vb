Public MustInherit Class ExportHelper

    Public Function ToKeyboardInput() As String
        Dim SB As New System.Text.StringBuilder()
        AppendKeyboardInput(SB)
        Return SB.ToString()
    End Function

    Public Function ToLaTeX() As String
        Dim SB As New System.Text.StringBuilder()
        AppendLaTeX(SB)
        Return SB.ToString()
    End Function

    Public Function ToMathML() As String
        Dim SB As New System.Text.StringBuilder()
        AppendMathML(SB)
        Return SB.ToString()
    End Function

    Public Function ToBitmap() As DrawingImage
        Dim DV As New DrawingVisual()
        Dim DG = DV.RenderOpen()
        Draw(DG)
        Return New DrawingImage(DV.Drawing)
    End Function

    Public MustOverride Sub AppendKeyboardInput(ByVal SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendLaTeX(ByVal SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendMathML(ByVal SB As System.Text.StringBuilder)

    Public MustOverride Sub Draw(ByVal DG As DrawingContext)

End Class
