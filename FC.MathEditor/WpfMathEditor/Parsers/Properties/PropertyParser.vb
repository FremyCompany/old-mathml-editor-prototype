Imports FC.MathEditor.Parsers

Public MustInherit Class PropertyParser

    Private Shared ParserCache As New Dictionary(Of String, PropertyParser)
    Shared Sub New()
        ' TODO : Prefetch predefined parsers to gain speed on first use
    End Sub

    ''' <summary>
    ''' Gets the specified parser from its type name.
    ''' </summary>
    ''' <param name="TypeName">Name of the type.</param><returns></returns>
    Public Shared Function [Get](TypeName As String) As PropertyParser

        Dim ReturnValue As PropertyParser = Nothing
        If ParserCache.TryGetValue(TypeName, ReturnValue) Then
            Return ReturnValue
        Else
            ReturnValue = Activator.CreateInstance(Type.GetType("FC.MathEditor.Parsers." + TypeName))
            ParserCache.Add(TypeName, ReturnValue)
            Return ReturnValue
        End If

    End Function

    ''' <summary>
    ''' Parses the specified string.
    ''' </summary>
    ''' <param name="Str">The textual representation of the attribute to parser.</param>
    ''' <param name="Context">The context in which the parse should occur (can be Nothing if no context is given).</param><returns></returns>
    Public Function Parse(Str As String, Context As MathElement) As Object
        Dim Result As Object = Nothing
        If TryParse(Str, Context, Result) Then
            Return Result
        Else
            Throw New ArgumentException("The attribute value was not recognized by this parser")
        End If
    End Function

    MustOverride Function TryParse(Str As String, Context As MathElement, ByRef Result As Object) As Boolean
    MustOverride Function Serialize(Value As Object, Context As MathElement) As String


End Class

Namespace Parsers

    <HideModuleName()>
    Public Module Instances

        Public ForFontStyle As New FontStyleParser()
        Public ForFontWeight As New FontWeightParser()
        Public ForFontFamily As New FontFamilyParser()
        Public ForColor As New ColorParser()
        Public ForBoolean As New BooleanParser()

    End Module
End Namespace