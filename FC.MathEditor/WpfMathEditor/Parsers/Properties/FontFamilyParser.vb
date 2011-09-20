Namespace Parsers

    Public Class FontFamilyParser : Inherits PropertyParser

        Public Overrides Function Serialize(Value As Object, Context As MathElement) As String
            If Not (TypeOf Value Is FontFamily) Then Return Nothing
            Return Value.ToString()
        End Function

        Public Overrides Function TryParse(Str As String, Context As MathElement, ByRef Result As Object) As Boolean

            Try
                Result = New System.Windows.Media.FontFamily(Str)
                Return True
            Catch ex As Exception
                Result = Nothing
                Return False
            End Try

        End Function
    End Class

End Namespace
