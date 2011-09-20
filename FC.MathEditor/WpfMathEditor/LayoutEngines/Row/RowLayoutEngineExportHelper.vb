' TODO : Don't polluate the global namespace with intern class implementation by using subclasses
Public Class RowLayoutEngineExportHelper : Inherits ExportHelper

    Public Sub New(This As MathElement)
        MyBase.New(This)
    End Sub

    Public Overrides Sub AppendSimpleText(SB As System.Text.StringBuilder)
        For Each Child In This.Children
            Child.Export.AppendSimpleText(SB)
        Next
    End Sub

    Public Overrides Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
        SB.Append("(")
        For Each Child In This.Children
            Child.Export.AppendKeyboardInput(SB)
        Next
        SB.Append(")")
    End Sub

    Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)
        SB.Append("{")
        For Each Child In This.Children
            Child.Export.AppendLaTeX(SB)
        Next
        SB.Append("}")
    End Sub

    Public Overrides Sub AppendMathML(SB As System.Text.StringBuilder)
        ' TODO: Implement MathML for "mrow" elements
    End Sub

    Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)
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

    Public Overridable ReadOnly Property InitialAboveBaseLineHeight() As Double
        Get
            Return 0
        End Get
    End Property

    Public Overridable ReadOnly Property InitialBelowBaseLineHeight() As Double
        Get
            Return 0
        End Get
    End Property

    Protected Overrides Sub GenerateLayout_Internal()

        Dim BBH As Double = InitialBelowBaseLineHeight
        Dim ABH As Double = InitialAboveBaseLineHeight

        Dim OMT, OMB As Double

        For Each C In This.Children
            BBH = Math.Max(BBH, C.Export.BelowBaseLineHeight)
            ABH = Math.Max(ABH, C.Export.AboveBaseLineHeight)

            ' TODO : fix that; if a small element has a big margin, 
            ' the big margin is applied on the big element that's sibling to him
            OMT = Math.Max(OMT, C.Export.OuterMargin.Top)
            OMB = Math.Max(OMB, C.Export.OuterMargin.Bottom)
        Next

        ABH = Math.Ceiling(ABH)
        BBH = Math.Ceiling(BBH)

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

    Protected Overrides Sub PrepareLayout_Internal(AvailABH As Double, AvailBBH As Double)
        For Each Child In This.Children
            Child.Export.PrepareLayout(AvailABH, AvailBBH)
        Next
    End Sub

    Protected Overrides Sub CalculateMinHeight_Internal()
        If This.Children.HasAny Then
            MinABH = This.Children.Select(Function(x) x.Export.MinimumABH).Max()
            MinBBH = This.Children.Select(Function(x) x.Export.MinimumBBH).Max()
        Else
            MinABH = InitialAboveBaseLineHeight
            MinBBH = InitialBelowBaseLineHeight
        End If
    End Sub

    Public Overrides ReadOnly Property PreferInlineContent_Interal As Boolean
        Get
            Return False
        End Get
    End Property

End Class
