Namespace Parsers
    Public Class BooleanParser : Inherits PropertyParser

        Public Overrides Function Serialize(Value As Object, Context As MathElement) As String

            If Not (TypeOf Value Is Boolean) Then Return Nothing
            Return If(DirectCast(Value, Boolean), "true", "false")

        End Function

        Public Overrides Function TryParse(Str As String, Context As MathElement, ByRef Result As Object) As Boolean

            If (Str = "true") OrElse (Str = "True") Then Result = True : Return True
            If (Str = "false") OrElse (Str = "false") Then Result = False : Return True
            Return False

        End Function

    End Class
End Namespace