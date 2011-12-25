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

    Private ReadOnly Property SepHeight As Double
        Get
            ' TODO: Return This.SepLine + 3
            Return 1
        End Get
    End Property

    Protected Overrides Sub CalculateMinHeight_Internal()

        ' NOTE: Baseline aligned at the fraction line
        MinABH = This.Numerator.Export.MinimalHeight + 1 + SepHeight / 2
        MinBBH = SepHeight / 2 + 1 + This.Denominator.Export.MinimalHeight

    End Sub

    Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)

        PerformLayout()
        For Each G In This.Children

            DG.PushTransform(New TranslateTransform(G.Export.LocationInParent.X, G.Export.LocationInParent.Y))
            'DG.PushTransform(New TranslateTransform(Math.Round(G.Export.LocationInParent.X), Math.Round(G.Export.LocationInParent.Y)))

            Dim ScaleX As Double = G.Export.LocationInParent.Width / G.Export.Width
            Dim ScaleY As Double = G.Export.LocationInParent.Height / G.Export.Height

            If Double.IsNaN(ScaleX) Then ScaleX = 1
            If Double.IsNaN(ScaleY) Then ScaleY = 1

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

        ' TODO : Draw line

    End Sub

    Protected Overrides Sub GenerateLayout_Internal()

        ' Get side height
        Dim NumWidth = This.Numerator.Export.Width
        Dim DenWidth = This.Denominator.Export.Width

        ' Apply a 0.5 padding at each side of the element
        Dim GlobalWidth = Math.Max(NumWidth, DenWidth) + 2

        ' Propose more length to num/den
        If NumWidth > DenWidth Then
            If This.Denominator.Export.ProposeMoreSpace(NumWidth - DenWidth) Then
                DenWidth = This.Denominator.Export.Width
            End If
        ElseIf DenWidth > NumWidth Then
            If This.Numerator.Export.ProposeMoreSpace(DenWidth - NumWidth) Then
                NumWidth = This.Numerator.Export.Width
            End If
        End If

        ' Compute paddings
        Dim NumPad = Math.Round((GlobalWidth - NumWidth) / 2)
        Dim DenPad = Math.Round((GlobalWidth - DenWidth) / 2)

        ' Set location of numerator+denominator
        This.Numerator.Export.SetLocationInParent(New Rect(NumPad, 0, NumWidth, This.Numerator.Export.Height))
        This.Denominator.Export.SetLocationInParent(New Rect(DenPad, 2 + SepHeight + This.Numerator.Export.Height, DenWidth, This.Denominator.Export.Height))

        H = MinimalHeight
        BH = MinimalBBH
        W = GlobalWidth

        ' TODO : Use properties to compute outer margin
        IM = Nothing : OM = New Thickness(2, 0, 2, 0)

    End Sub

    Public Overrides ReadOnly Property PreferInlineContent_Interal As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub PrepareLayout_Internal(ByVal AvailABH As Double, ByVal AvailBBH As Double)
        ' Do nothing. We will never use height stretching.
    End Sub
End Class
