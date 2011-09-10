Namespace Parsers
    Public Class BooleanParser : Inherits PropertyParser

        Public Overrides Function Serialize(Value As Object, Context As MathElement) As String

            Return If(Value, "true", "false")

        End Function

        Public Overrides Function TryParse(Str As String, Context As MathElement, ByRef Result As Object) As Boolean

            If (Str = "true") OrElse (Str = "True") Then Return True Else Return False

        End Function

    End Class
End Namespace