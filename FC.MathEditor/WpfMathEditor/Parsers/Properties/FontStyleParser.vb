Namespace Parsers
    Public Class FontStyleParser : Inherits PropertyParser

        Public Overrides Function Serialize(Value As Object, Context As MathElement) As String

            If Value Is Nothing Then Return Nothing
            If Not (TypeOf (Value) Is System.Windows.FontStyle) Then Return Nothing

            Dim V = DirectCast(Value, System.Windows.FontStyle)

            If V = System.Windows.FontStyles.Italic Then Return "italic"
            If V = System.Windows.FontStyles.Oblique Then Return "oblique"
            If V = System.Windows.FontStyles.Normal Then Return "normal"

            Return Nothing

        End Function

        Public Overrides Function TryParse(Str As String, Context As MathElement, ByRef Result As Object) As Boolean
            Select Case Str
                Case "italic"
                    Result = FontStyles.Italic : Return True
                Case "oblique"
                    Result = FontStyles.Oblique : Return True
                Case "normal"
                    Result = FontStyles.Normal : Return True
                Case Else
                    Return False
            End Select
        End Function

    End Class
End Namespace