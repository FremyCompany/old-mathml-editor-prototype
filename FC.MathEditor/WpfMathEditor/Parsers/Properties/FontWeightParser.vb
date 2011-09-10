Namespace Parsers

    Public Class FontWeightParser : Inherits PropertyParser

        Public Overrides Function Serialize(Value As Object, Context As MathElement) As String

            If Value Is Nothing Then Return Nothing
            If Not (TypeOf (Value) Is System.Windows.FontWeight) Then Return Nothing

            Dim V = DirectCast(Value, System.Windows.FontWeight)

            ' TODO : It's currently not possible to define other values than bold and normal
            ' As far as I can see the MathML specification don't allow it.
            If V.ToOpenTypeWeight() >= System.Windows.FontWeights.Bold.ToOpenTypeWeight() Then Return "bold"
            Return "normal"

        End Function

        Public Overrides Function TryParse(Str As String, Context As MathElement, ByRef Result As Object) As Boolean
            Select Case Str
                Case "bold"
                    Result = FontWeights.Bold : Return True
                Case "normal"
                    Result = FontWeights.Normal : Return True
                Case Else
                    Return False
            End Select
        End Function

    End Class

End Namespace