Public Class RootFormatterExportHelper : Inherits ExportHelper

    Protected Shadows ReadOnly Property This As RootFormatter
        Get
            Return MyBase.This
        End Get
    End Property

    Public Sub New(This As RootFormatter)
        MyBase.New(This)
    End Sub

    Public Overrides Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
        If This.HasExponent Then

            ' Appends a two-args root
            SB.Append("√"c)
            SB.Append("("c)
            This.Content.Export.AppendKeyboardInput(SB)
            SB.Append(","c)
            This.Exponent.Export.AppendKeyboardInput(SB)
            SB.Append(")"c)

        Else

            ' Appends a simple square root
            SB.Append("√"c)
            SB.Append("("c)
            This.Content.Export.AppendKeyboardInput(SB)
            SB.Append(")"c)

        End If
    End Sub

    Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)

        If This.HasExponent Then

            ' Appends a two-args root
            SB.Append("\sqrt[")
            This.Exponent.Export.AppendLaTeX(SB)
            SB.Append("]{")
            This.Content.Export.AppendLaTeX(SB)
            SB.Append("}"c)

        Else

            ' Appends a simple square root
            ' Appends a two-args root
            SB.Append("\sqrt{")
            This.Content.Export.AppendLaTeX(SB)
            SB.Append("}"c)
        End If

    End Sub

    Public Overrides Sub AppendMathML(SB As System.Text.StringBuilder)

        If This.HasExponent Then

            ' Appends a two-args root
            SB.Append("<mroot>")
            This.Content.Export.AppendMathML(SB)
            This.Exponent.Export.AppendMathML(SB)
            SB.Append("</mroot>")

        Else

            ' Appends a simple square root
            SB.Append("<msqrt>")
            This.Content.Export.AppendMathML(SB)
            This.Exponent.Export.AppendMathML(SB)
            SB.Append("</msqrt>")

        End If

    End Sub

    Public Overrides Sub AppendSimpleText(SB As System.Text.StringBuilder)
        If This.HasExponent Then

            ' Appends a two-args root
            ' (special formating for square and cube roots)
            Dim EStr = This.Exponent.ToString()
            Dim EInt As UInteger : If UInteger.TryParse(EStr, EInt) Then

                SB.Append("√"c) ' ⁰⁴⁵⁶⁷⁸⁹
                For Each C In EStr
                    Select Case C
                        Case "0"c : SB.Append("⁰"c)
                        Case "1"c : SB.Append("¹"c)
                        Case "2"c : SB.Append("²"c)
                        Case "3"c : SB.Append("³"c)
                        Case "4"c : SB.Append("⁴"c)
                        Case "5"c : SB.Append("⁵"c)
                        Case "6"c : SB.Append("⁶"c)
                        Case "7"c : SB.Append("⁷"c)
                        Case "8"c : SB.Append("⁸"c)
                        Case "9"c : SB.Append("⁹"c)
                    End Select
                Next
                SB.Append("("c)

            Else

                SB.Append("["c)
                This.Exponent.Export.AppendSimpleText(SB)
                SB.Append("]√(")

            End If

            This.Content.Export.AppendSimpleText(SB)
            SB.Append(")"c)

        Else

            ' Appends a simple square root
            SB.Append("√"c)
            SB.Append("("c)
            This.Content.Export.AppendSimpleText(SB)
            SB.Append(")"c)

        End If
    End Sub

    ''' <summary>
    ''' Number that specify how much the exponent is scaled down relative to its normal size (1)
    ''' </summary>
    ''' 
    ''' <remarks>
    ''' Should not be negative. I didn't test results in such cases.
    ''' </remarks>
    Private ExpScaleFactor As Double = 0.5

    ''' <summary>
    ''' Y-location of the top-left corner of the content (in drawig)
    ''' </summary>
    Private ContentTop As Double = 3

    ''' <summary>
    ''' X-location of the top-left corner of the square (in drawing)
    ''' </summary>
    ''' <remarks></remarks>
    Private ExpShift As Double = 0

    Protected Overrides Sub CalculateMinHeight_Internal()

        Dim Increment As Double = 0

        ' Basic sizing
        ContentTop = 3 * LineHeight
        MinABH = ContentTop + This.Content.Export.MinimalABH
        MinBBH = Math.Max(3, This.Content.Export.MinimalBBH)

        ' Hack. Used to align "v(x)" with "x"
        ContentTop -= 1

        ' Check if content isn't too small (min 16px)
        Increment = 16 * LineHeight - (MinBBH) - MinABH
        If Increment > 0 Then
            MinABH += Increment
            ContentTop += Increment
        End If

        ' Check if we don't need additionnal space to render 'exp'
        If This.HasExponent Then

            Increment = 9 * LineHeight + ExpScaleFactor * This.Exponent.Export.MinimalHeight - MinABH
            If Increment > 0 Then
                MinABH += Increment
                ContentTop += Increment
            End If

        End If

    End Sub

    Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)

        ' TODO: Refactor new Pen() & New Brush() for more speed

        ' Draw root sign
        Dim G = PathGeometry.Parse("M 22.946284 46.864213 2.5198268 8.968026 l -6.008253 3.032956 c -5.633303 2.843682 -6.088334 2.923437 -7.2915088 1.277998 -0.705791 -0.965228 -1.068996 -2.101641 -0.807121 -2.525363 0.540364 -0.874328 21.1973898 -11.9394794 22.265075 -11.9265194 0.382097 0.0046 7.894723 13.4905994 16.694723 29.9688024 15.9113 29.794277 16.018017 29.960542 19.25 29.991567 l 3.25 0.0312 0 13 0 13 -3.25 -0.0291 c -3.244379 -0.0291 -3.285331 -0.0947 -23.676458 -37.925322 z")
        ' Geometry is 86 pixels high (62 large). We usually draw at 8px at lineheight=1. We need to divise by 10.
        DG.PushTransform(New TranslateTransform(ExpShift, H - 8 * LineHeight))
        DG.PushTransform(New ScaleTransform(1 / 10, 1 / 11))
        DG.DrawGeometry(New SolidColorBrush(Foreground), Nothing, G)
        DG.Pop()
        DG.Pop()

        ' Draw vertical line
        DG.DrawLine(
            New Pen(New SolidColorBrush(Foreground), LineHeight),
            New Point(ExpShift + 5 * LineHeight, H - 0.5 * LineHeight),
            New Point(ExpShift + 9 * LineHeight, 0.5)
        )

        ' Draw horizontal line
        DG.DrawLine(
            New Pen(New SolidColorBrush(Foreground), LineHeight),
            New Point(ExpShift + 9 * LineHeight, 0.5 * LineHeight),
            New Point(W, 0.5 * LineHeight)
        )

        ' Draw content
        DrawChildren(DG)

    End Sub

    Protected Overrides Sub GenerateLayout_Internal()

        ''
        '' Calculate height
        ''
        Me.H = MinimalABH + MinimalBBH
        Me.BH = MinBBH



        ''
        '' Calculate width
        ''
        W = 0

        ''
        '' Assign the location of exponent
        ''
        ' Left can be -1 because OM is '1' so, most of the time, it will be fine
        If This.HasExponent Then
            This.Exponent.Export.SetLocationInParent(New Rect(
                New Point(
                    -1,
                    H - 9 * LineHeight - ExpScaleFactor * This.Exponent.Export.MinimalHeight
                ),
                New Size(
                    This.Exponent.Export.Width * ExpScaleFactor,
                    This.Exponent.Export.Height * ExpScaleFactor
                )
            ))

            ' Before the content, there's at least 5 pixel for graph
            W += Math.Max(5 * LineHeight, ExpScaleFactor * This.Exponent.Export.Width)
            ExpShift = W - 5 * LineHeight

        Else

            ' Before the content, there's at least 5 pixel for graph
            W += 5 * LineHeight
            ExpShift = 0

        End If


        ' Also, there's the bottom-top line
        W += 4 * LineHeight

        ' Assign location of content
        This.Content.Export.SetLocationInParent(
            New Rect(
                New Point(W, ContentTop),
                This.Content.Export.Size
            )
        )

        ' And then, there's the content
        W += This.Content.Export.Width

        ' And there's a very small gap
        W += 1



        ''
        '' Calculate margin
        ''
        IM = Nothing : OM = New Thickness(1 * LineHeight, 0, 1 * LineHeight, 0)

    End Sub

    '
    ' TODO: Some classes (Fraction...) don't use LineHeight as base unit for now. Should be fixed
    '

    Protected Overrides ReadOnly Property PreferInlineContent_Internal As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub PrepareLayout_Internal(AvailABH As Double, AvailBBH As Double)
        ' Do nothing, root are not stretchy
    End Sub

End Class
