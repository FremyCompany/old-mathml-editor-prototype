Public Class Length

    Public Value As Double
    Public Unit As LengthUnit
    Public Context As MathElement

    ''' <summary>
    ''' Convert an unit to pixel
    ''' </summary>
    ''' <param name="RelativeUnit">The value to return if '100%' is converted</param>
    ''' <param name="FontUnit">The value to return if '1em' is converted</param>

    Public Function ToPixels(Optional RelativeUnit As Double = 1, Optional FontUnit As Double = Double.NaN) As Double
        Select Case Unit

            Case LengthUnit.px
                Return Value

            Case LengthUnit.cm
                Return Value * 96 / 2.54

            Case LengthUnit.mm
                Return Value * 96 / 25.4

            Case LengthUnit.[in]
                Return Value * 96

            Case LengthUnit.pt
                Return Value * 96 / 72

            Case LengthUnit.pc
                Return Value * 12 * 96 / 72


            Case LengthUnit.unitless
                Return Value * RelativeUnit

            Case LengthUnit.percentage
                Return Value / 100 * RelativeUnit



            Case LengthUnit.em
                If Double.IsNaN(FontUnit) Then FontUnit = MathElement.DefaultFontSize
                Return Value * FontUnit

            Case LengthUnit.ex
                If Double.IsNaN(FontUnit) Then FontUnit = MathElement.DefaultFontSize
                Return Value * FontUnit * 0.5

            Case LengthUnit.ch
                If Double.IsNaN(FontUnit) Then FontUnit = MathElement.DefaultFontSize
                Return Value * FontUnit * 0.75


            Case LengthUnit.[rem]
                Return Value * MathElement.DefaultFontSize



            Case Else
                Throw New NotImplementedException("Unité inconnue rencontrée")

        End Select
    End Function

    Public Function ToRoundPixels(Optional RelativeUnit As Double = 1) As Integer
        Return Math.Round(ToPixels(RelativeUnit))
    End Function

End Class

Public Enum LengthUnit

    px
    pt
    pc
    cm
    mm
    [in]

    unitless
    percentage

    [rem]
    em
    ex
    ch

End Enum