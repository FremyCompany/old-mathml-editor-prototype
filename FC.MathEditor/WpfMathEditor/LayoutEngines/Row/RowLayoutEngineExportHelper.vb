﻿' TODO : Don't polluate the global namespace with intern class implementation by using subclasses
Public Class RowLayoutEngineExportHelper : Inherits ExportHelper

    Public Sub New(This As MathElement)
        MyBase.New(This)
    End Sub

    Public Overrides Sub AppendSimpleText(SB As System.Text.StringBuilder)
        If This.Children.HasOne AndAlso This.Children.First.IsTextEdit Then

            ' If there's only one child and that it's text, we can simply add it
            This.Children.First.Export.AppendSimpleText(SB)

        Else

            ' Or else we need parenthesis
            SB.Append("("c)
            For Each Child In This.Children
                Child.Export.AppendSimpleText(SB)
            Next
            SB.Append(")"c)

        End If
    End Sub

    Public Overrides Sub AppendKeyboardInput(SB As System.Text.StringBuilder)
        If This.Children.HasOne AndAlso This.Children.First.IsTextEdit Then

            ' If there's only one child and that it's text, we can simply add it
            This.Children.First.Export.AppendKeyboardInput(SB)

        Else

            ' Or else we need parenthesis
            SB.Append("("c)
            For Each Child In This.Children
                Child.Export.AppendKeyboardInput(SB)
            Next
            SB.Append(")"c)

        End If
    End Sub

    Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)
        If This.Children.HasOne AndAlso This.Children.First.IsTextEdit Then

            ' If there's only one child and that it's text, we can simply add it
            This.Children.First.Export.AppendLaTeX(SB)

        Else

            ' Or else we need parenthesis
            SB.Append("{")
            For Each Child In This.Children
                Child.Export.AppendLaTeX(SB)
            Next
            SB.Append("}")

        End If
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

            ' if a small element has a big margin, the big margin should not apply on the big element that's sibling to him
            OMT = Math.Max(OMT, C.Export.AboveBaseLineHeight + C.Export.OuterMargin.Top)
            OMB = Math.Max(OMB, C.Export.BelowBaseLineHeight + C.Export.OuterMargin.Bottom)
        Next

        ' TODO : Removed to fix Fraction formatter blur problem
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
        Me.OM = New Thickness(0, OMT - ABH, 0, OMB - BBH)

    End Sub

    Protected Overrides Function ProposeMoreSpace_Internal(ByRef AvailWidth As Double) As Boolean
        Dim HasChanged As Boolean = False

        For Each El In This.Children
            If El.Export.ProposeMoreSpace(AvailWidth) Then
                LayoutCompletion = LayoutCompletionState.Prepared : HasChanged = True
                If AvailWidth = 0 Then Return True
            End If
        Next

        Return HasChanged
    End Function

    Protected Overrides Sub PrepareLayout_Internal(AvailABH As Double, AvailBBH As Double)
        For Each Child In This.Children
            Child.Export.PrepareLayout(AvailABH, AvailBBH)
        Next
    End Sub

    Protected Overrides Sub CalculateMinHeight_Internal()
        If This.Children.HasAny Then
            MinABH = Math.Ceiling(Math.Max(This.Children.Select(Function(x) x.Export.MinimalABH).Max(), InitialAboveBaseLineHeight))
            MinBBH = Math.Ceiling(Math.Max(This.Children.Select(Function(x) x.Export.MinimalBBH).Max(), InitialBelowBaseLineHeight))
        Else
            MinABH = InitialAboveBaseLineHeight
            MinBBH = InitialBelowBaseLineHeight
        End If
    End Sub

    Protected Overrides ReadOnly Property PreferInlineContent_Internal As Boolean
        Get
            Return False
        End Get
    End Property

End Class
