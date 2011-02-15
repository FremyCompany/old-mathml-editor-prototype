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
        PerformLayout()
        For Each G In This.Children

            DG.PushTransform(New TranslateTransform(G.Export.LocationInParent.X, G.Export.LocationInParent.Y))

            Dim ScaleX As Double = G.Export.LocationInParent.Width / G.Export.Width
            Dim ScaleY As Double = G.Export.LocationInParent.Height / G.Export.Height

            If ScaleX <> 1 OrElse ScaleY <> 1 Then
                DG.PushTransform(New ScaleTransform(
                    ScaleX,
                    ScaleY
                ))
                G.Export.Draw(DG)
                DG.Pop()
            Else
                G.Export.Draw(DG)
            End If

            DG.Pop()
        Next

    End Sub

    Public Overrides Sub GenerateLayout()

        Dim BBH As Double = 0
        Dim ABH As Double = 0

        Dim OMT, OMB As Double

        For Each C In This.Children
            BBH = Math.Max(BBH, C.Export.BelowBaseLineHeight)
            ABH = Math.Max(ABH, C.Export.AboveBaseLineHeight)
            OMT = Math.Max(OMT, C.Export.OuterMargin.Top)
            OMB = Math.Max(OMB, C.Export.OuterMargin.Bottom)
        Next

        ABH = Math.Ceiling(ABH)
        BBH = Math.Floor(ABH)

        W = 0
        H = BBH + ABH
        BH = BBH

        For Each C In This.Children
            C.Export.SetLocationInParent(New Rect(New Point(W, ABH - C.Export.AboveBaseLineHeight), C.Export.Size))
            Me.W += C.Export.Width
            If C.NextSibling IsNot Nothing Then
                Me.W += Math.Max(C.Export.OuterMargin.Right, C.NextSibling.Export.OuterMargin.Left)
            End If
        Next

        Me.IM = New Thickness(0)
        Me.OM = New Thickness(0, OMT, 0, OMB)

    End Sub

End Class
