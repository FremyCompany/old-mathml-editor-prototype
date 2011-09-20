Namespace Parsers
    Public Class LengthParser : Inherits PropertyParser

        Public Overrides Function Serialize(Value As Object, Context As MathElement) As String

            If (TypeOf Value Is Double) OrElse (TypeOf Value Is Integer) OrElse (TypeOf Value Is Length) Then Return Value.ToString()
            Return Nothing

        End Function

        Dim RegExp As New System.Text.RegularExpressions.Regex("^(?'value'([0-9]|\.)*)(?'unit'.*)$")
        Public Overrides Function TryParse(Str As String, Context As MathElement, ByRef Result As Object) As Boolean

            Dim M = RegExp.Match(Str)
            If (M Is Nothing) OrElse (Not M.Success) Then Return False

            Dim Len As New Length() : Result = Len
            Len.Context = Context
            Len.Value = Val(M.Groups("value").Value)

            Dim UnitStr = M.Groups("unit").Value
            If UnitStr = "%" Then
                Len.Unit = LengthUnit.percentage : Return True
            Else
                If LengthUnit.TryParse(UnitStr, Len.Unit) Then
                    Return True
                Else
                    Result = Nothing : Return False
                End If
            End If

        End Function

    End Class
End Namespace