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
        SB.Append("("c)
        This.Numerator.Export.AppendKeyboardInput(SB)
        SB.Append(")/(")
        This.Denominator.Export.AppendKeyboardInput(SB)
        SB.Append(")"c)
    End Sub

    Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)
        SB.Append("\frac")
        This.Numerator.Export.AppendLaTeX(SB)
        This.Denominator.Export.AppendLaTeX(SB)
    End Sub

    Public Overrides Sub AppendMathML(SB As System.Text.StringBuilder)
        SB.Append("<mfrac>")
        This.Numerator.Export.AppendMathML(SB)
        This.Denominator.Export.AppendMathML(SB)
        SB.Append("</mfrac>")
    End Sub

    Public Overrides Sub AppendSimpleText(SB As System.Text.StringBuilder)
        SB.Append("("c)
        This.Numerator.Export.AppendSimpleText(SB)
        SB.Append(")/(")
        This.Denominator.Export.AppendSimpleText(SB)
        SB.Append(")"c)
    End Sub

    ''' <summary>
    ''' Tickness of the whitespace at the bottom/top of the fraction line (in pixels)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property SepGap As Double
        Get
            ' Todo: Return This.SepGap
            Return 1 * LineHeight
        End Get
    End Property

    ''' <summary>
    ''' Tickness of the fraction line (in pixels)
    ''' </summary>
    Private ReadOnly Property SepHeight As Double
        Get
            ' TODO: Return This.SepLine + 3
            Return 1 * LineHeight
        End Get
    End Property

    Protected Overrides Sub CalculateMinHeight_Internal()

        ' NOTE: Baseline aligned at the middle of the fraction line

        MinABH = (
            This.Numerator.Export.MinimalHeight + _
            SepGap +
            SepHeight / 2
        )

        MinBBH = (
            SepHeight / 2 +
            SepGap +
            This.Denominator.Export.MinimalHeight
        )

    End Sub

    Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)

        PerformLayout()

        DG.DrawLine(New Pen(New SolidColorBrush(Foreground), LineHeight), New Point(0, AboveBaseLineHeight), New Point(W, AboveBaseLineHeight))

        DrawChildren(DG)

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

    Protected Overrides ReadOnly Property PreferInlineContent_Internal As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub PrepareLayout_Internal(ByVal AvailABH As Double, ByVal AvailBBH As Double)
        ' Do nothing. We will never use height stretching.
    End Sub
End Class
