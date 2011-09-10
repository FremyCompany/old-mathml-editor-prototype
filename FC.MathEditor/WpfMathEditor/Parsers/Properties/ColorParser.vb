Namespace Parsers
    Public Class ColorParser : Inherits PropertyParser

        Public Overrides Function Serialize(Value As Object, Context As MathElement) As String

            Return "#000000"

        End Function

        Public Overrides Function TryParse(Str As String, Context As MathElement, ByRef Result As Object) As Boolean

            Result = System.Windows.Media.Color.FromArgb(255, 0, 0, 0)
            Return False

        End Function

    End Class
End Namespace