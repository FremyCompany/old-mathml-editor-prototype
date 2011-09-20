Namespace Parsers
    Public Class ColorParser : Inherits PropertyParser

        Public Overrides Function Serialize(Value As Object, Context As MathElement) As String

            If TypeOf Value Is Color Then

                ' Convert color as RGBA
                Dim C = DirectCast(Value, Color)
                Return "rgba(" & C.R & "," & C.G & "," & C.B & "," & C.A / 255 & ")"

            Else

                Return Nothing

            End If

        End Function

        Public Overrides Function TryParse(Str As String, Context As MathElement, ByRef Result As Object) As Boolean

            Try
                Dim DefaultColor = GetType(Colors).GetProperty(Str, Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Static Or Reflection.BindingFlags.Public)
                If DefaultColor IsNot Nothing Then

                    Result = DefaultColor.GetValue(Nothing, Nothing)
                    Return True

                ElseIf Str(0) = "#" Then

                    If Str.Length = 7 Then

                        Result = Color.FromRgb(
                            Integer.Parse(Str(1) & Str(2), Globalization.NumberStyles.AllowHexSpecifier),
                            Integer.Parse(Str(3) & Str(4), Globalization.NumberStyles.AllowHexSpecifier),
                            Integer.Parse(Str(5) & Str(6), Globalization.NumberStyles.AllowHexSpecifier)
                        )
                        Return True

                    ElseIf Str.Length = 4 Then

                        Result = Color.FromRgb(
                            Integer.Parse(Str(1), Globalization.NumberStyles.AllowHexSpecifier),
                            Integer.Parse(Str(2), Globalization.NumberStyles.AllowHexSpecifier),
                            Integer.Parse(Str(3), Globalization.NumberStyles.AllowHexSpecifier)
                        )
                        Return True

                    End If

                ElseIf Str.StartsWith("rgb(") AndAlso Str.EndsWith(")") Then

                    Dim Vals = Str.Substring(4, Str.Length - 5).Split(",") _
                               .Select(Function(s) s.Trim()) _
                               .Select(Function(s) Byte.Parse(s))

                    Result = Color.FromRgb(Vals(0), Vals(1), Vals(2))

                    Return True

                ElseIf Str.StartsWith("rgba(") AndAlso Str.EndsWith(")") Then

                    Dim Vals = Str.Substring(4, Str.Length - 5).Split(",") _
                               .Select(Function(s) s.Trim()) _
                               .Select(Function(s) Double.Parse(s))

                    Result = Color.FromArgb(CByte(Vals(3)), CByte(Vals(0)), CByte(Vals(1)), CByte(255 * Vals(2)))

                    Return True

                End If

            Catch : End Try

            Result = Nothing
            Return False

        End Function

    End Class
End Namespace